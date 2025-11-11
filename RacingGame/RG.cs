using AssetManagementBase;
using Microsoft.Xna.Framework;
using Myra;
using Nursia;
using RacingGame.GameLogic;
using RacingGame.GameScreens;
using RacingGame.Utilities;
using System.IO;

namespace RacingGame
{
	public static partial class RG
	{
		private static IGameScreen2 _currentScreen { get; set; }

		public static AssetManager Assets { get; private set; }

		public static GameTime GameTime { get; set; }

		public static float TotalMs => (float)GameTime.TotalGameTime.TotalMilliseconds;
		public static float TotalTime => TotalMs / 1000.0f;
		public static float ElapsedMs => (float)GameTime.ElapsedGameTime.TotalMilliseconds;
		public static float ElapsedTime => ElapsedMs / 1000.0f;

		public static bool InGame => false;
		public static bool IsAppActive => true;

		public static int Width => Nrs.GraphicsDevice.Viewport.Width;
		public static int Height => Nrs.GraphicsDevice.Viewport.Height;

		public static IGameScreen2 CurrentScreen
		{
			get => _currentScreen;

			set
			{
				if (_currentScreen != null)
				{
					_currentScreen.OnUnset();
				}

				value.OnSet();
				_currentScreen = value;

				// Update Mouse Visible
				Nrs.Game.IsMouseVisible = _currentScreen.IsMouseVisible;
			}
		}

		public static void Initialize(Game game)
		{
			Nrs.Game = game;
			MyraEnvironment.Game = game;

			// Asset Manager
			var contentPath = Path.Combine(PathUtils.ExecutingAssemblyDirectory, "Assets");
			Assets = AssetManager.CreateFileAssetManager(contentPath);

			Resources.Initialize();
			Highscores.Initialize();

			// But start with splash screen, if user clicks or presses Start,
			// we are back in the main menu.
			CurrentScreen = new SplashScreen2();

			// Nrs.GraphicsSettings.ShadowType = Nursia.SceneGraph.Lights.ShadowType.None;
		}

		public static void Update(GameTime gameTime)
		{
			GameTime = gameTime;

			Input.Update();

			CurrentScreen?.Update();
		}

		public static void Render(GameTime gameTime)
		{
			GameTime = gameTime;
			CurrentScreen?.Render();
		}
	}
}
