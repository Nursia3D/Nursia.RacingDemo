using Microsoft.Xna.Framework;

namespace RacingGame
{
	internal static class Constants
	{
		/// <summary>
		/// Default object matrix to fix models from 3ds max to our engine!
		/// </summary>
		public static readonly Matrix objectMatrix =
			//right handed models: Matrix.CreateRotationX(MathHelper.Pi);// *
			//Matrix.CreateScale(MaxModelScaling);
			// left handed models (else everything is mirrored with x files)
			Matrix.CreateRotationX(MathHelper.Pi / 2.0f);

		/// <summary>
		/// Default color values are:
		/// 0.15f for ambient and 1.0f for diffuse and 1.0f specular.
		/// </summary>
		public static readonly Color
			DefaultAmbientColor = new Color(40, 40, 40),
			DefaultDiffuseColor = new Color(210, 210, 210),
			DefaultSpecularColor = new Color(255, 255, 255);

		/// <summary>
		/// Default specular power (24)
		/// </summary>
		const float DefaultSpecularPower = 24.0f;

		/// <summary>
		/// Parallax amount for parallax and offset shaders.
		/// </summary>
		public const float DefaultParallaxAmount = 0.04f;
	}
}
