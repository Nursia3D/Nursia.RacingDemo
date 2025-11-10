using AssetManagementBase;
using Microsoft.Xna.Framework;
using Nursia;
using Nursia.SceneGraph.Landscape;
using RacingGame.GameScreens;
using RacingGame.Utilities;
using System.IO;

namespace RacingGame
{
	public static partial class RG
	{
		public static AssetManager Assets { get; private set; }

		/// <summary>
		/// Since we have one terrain for all levels
		/// It's better to store it globally
		/// </summary>
		public static TerrainNode Terrain { get; private set; }

		public static bool InGame => false;

		public static IGameScreen2 CurrentScreen { get; set; }

		public static void Initialize()
		{
			// Asset Manager
			var contentPath = Path.Combine(PathUtils.ExecutingAssemblyDirectory, "Assets");
			Assets = AssetManager.CreateFileAssetManager(contentPath);

			// Terrain Node
			Terrain = (TerrainNode)Assets.LoadSceneNode("Scenes/Landscape.scene");
			Terrain.Translation = new Vector3(1280, 1280, 0);
			Terrain.Rotation = new Vector3(90, 0, 0);

			Highscores.Initialize();

			// Initial screen is splash
			CurrentScreen = new SplashScreen2();
		}
	}
}
