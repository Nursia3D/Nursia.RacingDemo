namespace RacingGame.GameScreens
{
	public interface IGameScreen2
	{
		bool IsMouseVisible { get; }

		void OnSet();
		void OnUnset();

		void Update();
		void Render();
	}
}
