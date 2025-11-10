using AssetManagementBase;
using Microsoft.Xna.Framework;
using Nursia;
using Nursia.Materials;
using Nursia.SceneGraph;
using System.Linq;

namespace RacingGame.Graphics
{
	public class CarWrapper
	{
		/// <summary>
		/// Car colors for the car selection screen.
		/// </summary>
		public static Color[] CarColors = new Color[]
			{
				Color.White,
				Color.Yellow,
				Color.Blue,
				Color.Purple,
				Color.Red,
				Color.Green,
				Color.Teal,
				Color.Gray,
				Color.Chocolate,
				Color.Orange,
				Color.SeaGreen,
			};

		private readonly NursiaModelNode[] _cars;

		/// <summary>
		/// Number of car texture types
		/// </summary>
		/// <returns>Int</returns>
		public int NumberOfCars
		{
			get
			{
				return _cars.Length;
			}
		}

		public CarWrapper()
		{
			_cars = new NursiaModelNode[3];

			_cars[0] = CreateCarNode("RacerCar");
			_cars[1] = CreateCarNode("RacerCar2");
			_cars[2] = CreateCarNode("RacerCar3");
		}

		private NursiaModelNode CreateCarNode(string textureName)
		{
			var texture = BaseGame.Content.LoadTexture2D(Nrs.GraphicsDevice, "$Textures/{textureName}.tga");

			var result = (NursiaModelNode)BaseGame.Content.LoadSceneNode("Scenes/Car.scene");
			var wheelsBoneIndices = (from b in result.Model.Bones where b.Mesh != null && b.Mesh.MeshParts.Count == 2 select b.Index).ToArray();

			// Add wheels turning
			result.PreRender += () =>
			{
				var wheelNumber = 0;

				for (var i = 0; i < wheelsBoneIndices.Length; ++i)
				{
					var idx = wheelsBoneIndices[i];
					wheelNumber++;

					var rotationMatrix = Matrix.CreateRotationX(
						// Rotate left 2 wheels forward, the other 2 backward!
						(wheelNumber == 2 || wheelNumber == 4 ? 1 : -1) *
						RacingGameManager.Player.CarWheelPos);

					result.ModelInstance.SetBoneLocalTransform(idx, rotationMatrix);
				}
			};

			// Set texture
			for (var i = 0; i < result.Materials.Length; ++i)
			{
				for (var j = 0; j < result.Materials[i].Length; ++j)
				{
					var mat = (LitSolidMaterial)result.Materials[i][j];

					mat.DiffuseTexture = texture;
				}
			}

			return result;
		}

		public NursiaModelNode GetCar(int number) => _cars[number];
	}
}
