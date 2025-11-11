using Nursia;

namespace RacingGame.GameScreens
{
	public class MainMenuScreen: IGameScreen2
	{
		private readonly UI.MainMenu _mainMenu = new UI.MainMenu();

		public bool IsMouseVisible => true;

		public MainMenuScreen()
		{
			_mainMenu._menuItemStartGame.Selected += (s, a) => RG.CurrentScreen = new CarSelectionScreen();
			_mainMenu._menuQuit.Selected += (s, a) => Nrs.Game.Exit();
		}

		public void OnSet()
		{
			RG.Graphics.UIDesktop.Root = _mainMenu;
		}

		public void OnUnset()
		{
			RG.Graphics.UIDesktop.Root = null;
		}


		public void Update()
		{
		}

		/// <summary>
		/// Render
		/// </summary>
		public void Render()
		{
			RG.Graphics.UIDesktop.Render();
		}
	}
}
