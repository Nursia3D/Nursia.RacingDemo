using AssetManagementBase;
using DigitalRiseModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nursia.Materials;
using Nursia.SceneGraph;
using Nursia.SceneGraph.Landscape;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Converter
{
	public class Conversion
	{
		const int GridWidth = 257, GridHeight = 257;

		static readonly Matrix objectMatrix = Matrix.CreateRotationX(MathHelper.Pi / 2.0f);
		private class MaterialData
		{
			public string Effect { get; set; }
			public string Technique { get; set; }

			public Dictionary<string, JsonElement> Parameters { get; set; }
		}

		private class MaterialData2
		{
			public Dictionary<string, MaterialData> Materials { get; set; }
			public Dictionary<string, string[]> MeshesMaterials { get; set; }
		}

		private static string UpdatePath(string path)
		{
			return path.Replace("../textures", "../Textures");
		}

		public static NursiaModelNode LoadFromFile(GraphicsDevice device, string file)
		{
			Console.WriteLine(file);

			var assetManager = AssetManager.CreateFileAssetManager(Path.GetDirectoryName(file));

			var model = assetManager.LoadModel(device, file);
			var result = new NursiaModelNode
			{
				Model = model,
				ModelPath = Utility.TryToMakePathRelativeTo(file, Utility.OutputFolder)
			};

			var materialFile = Path.ChangeExtension(file, "material");
			var data = File.ReadAllText(materialFile);

			var materialData = JsonSerializer.Deserialize<MaterialData2>(data);

			if (materialData.Materials.Count == 0)
			{
				return result;
			}

			var materials = new Dictionary<string, LitSolidMaterial>();
			foreach (var pair in materialData.Materials)
			{
				var md = pair.Value;

				var material = new LitSolidMaterial();

				// Set parameters
				foreach (var pair2 in md.Parameters)
				{
					var val = pair2.Value;

					switch (pair2.Key)
					{
						case "diffuseTexture":
							material.DiffuseTexturePath = UpdatePath(val.GetString());
							break;

						case "normalTexture":
							material.NormalTexturePath = UpdatePath(val.GetString());
							break;

						case "ambientColor":
							material.AmbientLightColor = val.ToColor();
							break;

						case "diffuseColor":
							material.DiffuseColor = val.ToColor();
							break;

						case "specularColor":
							material.SpecularColor = val.ToColor();
							break;

						case "shininess":
							material.SpecularPower = val.ToFloat();
							break;

						case "lightDir":
						case "reflectionCubeTexture":
						case "NormalizeCubeTexture":
						case "shadowCarColor":
						case "carHueColorChange":
							break;

						default:
							Console.WriteLine($"Skipped parameter {pair2.Key}");
							break;

					}
				}

				materials[pair.Key] = material;
			}

			foreach (var pair in materialData.MeshesMaterials)
			{
				for (var i = 0; i < model.Meshes.Length; ++i)
				{
					var mesh = model.Meshes[i];
					var name = mesh.Name ?? mesh.ParentBone.Name;

					if (pair.Key == name)
					{
						for (var j = 0; j < mesh.MeshParts.Count; ++j)
						{
							if (pair.Value.Length != 0)
							{
								result.Materials[i][j] = materials[pair.Value[j]];
							}
							else
							{
								result.Materials[i][j] = materials.First().Value;
							}
						}
					}
				}
			}

			return result;
		}


		public static SceneNode FromTrackData(string file)
		{
			var result = new SceneNode();

			HeightField hf;

			var folder = Path.GetDirectoryName(file);
			var heightFile = Path.Combine(folder, "LandscapeHeights.data");
			using (var stream = File.OpenRead(heightFile))
			{
				hf = HeightField.FromStreamR8(stream, GridWidth, GridHeight);
			}

			using (var stream = File.Create(Path.Combine(folder, "Scenes/LandscapeHeights.hf")))
			{
				hf.SaveToHf(stream);
			}

			// Terrain
			var material = new TerrainMaterial
			{
				AmbientLightColor = new Color(88, 88, 88),
				DiffuseColor = new Color(234, 234, 234),
				SpecularColor = new Color(33, 33, 33),
				DetailMap1Path = "../Textures/Landscape.tga",
				WeightMap1Path = "../Textures/LandscapeHeights.png",
				DetailTiling = new Vector2(1, 1)
			};

			var terrainNode = new TerrainNode
			{
				HeightFieldPath = "LandscapeHeights.hf",
				TerrainSize = new Vector3(2560, 300, 2560),
				Material = material,
				HeightField = hf
			};
			
			result.Children.Add(terrainNode);

			var trackData = TrackData.Load(file);

			SubsceneNode subscene;
			Matrix transform;
			foreach (var obj in trackData.NeutralsObjects)
			{
				if (obj.modelName.StartsWith("Track"))
				{
					// Skip self-reference
					continue;
				}

				if (obj.modelName.StartsWith("Combi"))
				{
					var combiPath = Path.Combine(folder, $"{obj.modelName}.CombiModel");
					if (!File.Exists(combiPath))
					{
						continue;
					}

					var combiModels = new TrackCombiModels(combiPath);
					foreach (var combiObj in combiModels.Objects)
					{
						var subscenePath = Path.Combine(folder, $"Scenes/{combiObj.modelName}.scene");
						if (!File.Exists(subscenePath))
						{
							Console.WriteLine($"Skipping non-existance scene '{subscenePath}'");
							continue;
						}

						subscene = new SubsceneNode
						{
							NodePath = $"{combiObj.modelName}.scene"
						};

						// Original game had Y axis pointed forward and Z pointed up
						// We need to transform it into Nursia system, where Y pointed up and Z pointed backwards(to viewer)
						// Also original terrain spanned from 0 to 2560 on both axies
						// Nursia spans from -1280 to 1280
						// We need to correctly map it all
						transform = objectMatrix * combiObj.matrix * obj.matrix * Matrix.Invert(objectMatrix);

						var pos = transform.Translation;

						pos.X -= 1280;
						pos.Z = -1280 - pos.Z;

						var height = terrainNode.GetHeight(new Vector2(pos.X, pos.Z));

						if (pos.Y < height)
						{
							pos.Y = height;
						}
						
						transform.Translation = pos;

						subscene.SetTransform(transform);

						result.Children.Add(subscene);
					}
				}
				else
				{
					var subscenePath = Path.Combine(folder, $"Scenes/{obj.modelName}.scene");
					if (!File.Exists(subscenePath))
					{
						Console.WriteLine($"Skipping non-existance scene '{subscenePath}'");
						continue;
					}

					subscene = new SubsceneNode
					{
						NodePath = $"{obj.modelName}.scene"
					};

					// Original game had Y axis pointed forward and Z pointed up
					// We need to transform it into Nursia system, where Y pointed up and Z pointed backwards(to viewer)
					// Also original terrain spanned from 0 to 2560 on both axies
					// Nursia spans from -1280 to 1280
					// We need to correctly map it all
					transform = objectMatrix * obj.matrix * Matrix.Invert(objectMatrix);

					var pos = transform.Translation;

					pos.X -= 1280;
					pos.Z = 2560 - pos.Z;

					var height = terrainNode.GetHeight(new Vector2(pos.X, pos.Z));

					if (pos.Y < height)
					{
						pos.Y = height;
					}

					transform.Translation = pos;

					subscene.SetTransform(transform);

					result.Children.Add(subscene);
				}
			}

			return result;
		}
	}
}
