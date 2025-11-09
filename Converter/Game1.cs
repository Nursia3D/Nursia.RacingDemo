using AssetManagementBase;
using DigitalRiseModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nursia;
using Nursia.Env.Sky;
using Nursia.SceneGraph;
using System;
using System.IO;

namespace Converter
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		private readonly GraphicsDeviceManager _graphics;

		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = 1200,
				PreferredBackBufferHeight = 800
			};

			Window.AllowUserResizing = true;
			IsMouseVisible = true;
		}

		protected override void LoadContent()
		{
			Nrs.Game = this;

			var models = Directory.EnumerateFiles(Utility.InputFolder, "*.glb");

			foreach (var model in models)
			{
				var modelNode = Conversion.LoadFromFile(GraphicsDevice, model);

				var storedScene = new StoredScene(modelNode);

				var file = Path.GetFileName(model);
				var outputPath = Path.Combine(Utility.OutputFolder, Path.ChangeExtension(file, "scene"));

				Console.WriteLine($"Writing {outputPath}");
				storedScene.SaveToFile(outputPath);
			}

			var folder = Path.GetDirectoryName(Utility.InputFolder);
			var trackDatas = Directory.EnumerateFiles(folder, "*.Track");
			foreach (var model in trackDatas)
			{
				var modelNode = Conversion.FromTrackData(model);
				var storedScene = new StoredScene(modelNode);

				storedScene.RenderEnvironment.Sky = new Skybox
				{
					DiffuseTexturePath = "../Textures/SkyCubeMap.dds"
				};

				var file = Path.GetFileName(model);
				var outputPath = Path.Combine(Utility.OutputFolder, Path.ChangeExtension(file, "scene"));

				Console.WriteLine($"Writing {outputPath}");
				storedScene.SaveToFile(outputPath);
			}

			Exit();
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}

		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			base.Draw(gameTime);
		}
	}
}