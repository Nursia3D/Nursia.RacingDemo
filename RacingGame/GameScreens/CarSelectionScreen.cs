using Microsoft.Xna.Framework;
using Nursia.SceneGraph;
using Nursia.SceneGraph.Cameras;
using Nursia.SceneGraph.Lights;
using RacingGame.Graphics;
using System;

namespace RacingGame.GameScreens
{
	class CarSelectionScreen : IGameScreen2
	{
		/// <summary>
		/// Helper for rotating the 3 cars in the car selection screen.
		/// </summary>
		private float _carSelectionRotationZ = 0.0f;
		private readonly DirectLight _directLight = new DirectLight
		{
			MaxShadowDistance = Constants.DefaultMaxShadowDistance,
			Direction = -LensFlare.DefaultLightPos
		};
		private PerspectiveCamera _camera = new PerspectiveCamera
		{
			ViewAngle = Constants.FieldOfViewInDegrees,
			NearPlaneDistance = Constants.NearPlane,
			FarPlaneDistance = Constants.FarPlane
		};

		private NursiaModelNode[] _carSelectionPlates;
		private NursiaModelNode[] _cars;

		public bool IsMouseVisible => true;

		public void OnSet()
		{
			_carSelectionPlates = new NursiaModelNode[3];
			_cars = new NursiaModelNode[3];

			for(var i = 0; i < _cars.Length; ++i)
			{
				_carSelectionPlates[i] = RG.Resources.LoadModel("CarSelectionPlate");
				_cars[i] = RG.Resources.CreateCar(i);
			}
		}

		public void OnUnset()
		{
		}

		public void Update()
		{
		}

		#region Helpers

		/// <summary>
		/// Helper function for RotateSlowly, max. distance between
		/// sourceRot and desiredRot is PI, this allows very easy checks.
		/// </summary>
		public static void AdjustRotRange(ref float desiredRot, float sourceRot)
		{
			if (desiredRot >= sourceRot + (float)Math.PI)
				desiredRot -= (float)Math.PI * 2.0f;
			if (desiredRot < sourceRot - (float)Math.PI)
				desiredRot += (float)Math.PI * 2.0f;
		}

		/// <summary>
		/// Adjust rotation to -PI - PI range
		/// </summary>
		public static void AdjustRotToPIRange(ref float desiredRot)
		{
			if (desiredRot <= -(float)Math.PI)
				desiredRot += (float)Math.PI * 2.0f;
			if (desiredRot > (float)Math.PI)
				desiredRot -= (float)Math.PI * 2.0f;
		}

		/// <summary>
		/// Interpolate rotation
		/// </summary>
		/// <param name="rot">Rot</param>
		/// <param name="targetRot">Target rot</param>
		/// <param name="nearlyEqualRot">Nearly equal rot</param>
		/// <returns>Float</returns>
		public static float InterpolateRotation(
			float rot, float targetRot, float nearlyEqualRot)
		{
			AdjustRotRange(ref targetRot, rot);

			if (rot > targetRot)
			{
				if (Math.Abs(rot - targetRot) < nearlyEqualRot)
					rot = targetRot;
				else
					rot -= nearlyEqualRot;
			}
			else if (rot < targetRot)
			{
				if (Math.Abs(rot - targetRot) < nearlyEqualRot)
					rot = targetRot;
				else
					rot += nearlyEqualRot;
			}

			// Check if rot is in -PI-PI range (for easier calculations!)
			AdjustRotToPIRange(ref rot);

			return rot;
		}
		#endregion

		public void Render()
		{
			// Let camera point directly at the center, around 10 units away.
			var viewMatrix = Matrix.CreateLookAt(
				new Vector3(0, 10.45f, 2.75f),
				new Vector3(0, 0, -1),
				new Vector3(0, 0, 1));

			// Let the light come from the front!
			Vector3 lightDir = -LensFlare.DefaultLightPos;
			lightDir = new Vector3(lightDir.X, lightDir.Y, -lightDir.Z);

			// LightDirection will normalize
			_directLight.Direction = -lightDir;

			// Show 3d cars
			// Rotate all 3 cars depending on the current selection
			float perCarRot = MathHelper.Pi * 2.0f / 3.0f;
			float newCarSelectionRotationZ =
				Globals.CarNumber * perCarRot;
			_carSelectionRotationZ = InterpolateRotation(
				_carSelectionRotationZ, newCarSelectionRotationZ,
				RG.ElapsedTime * 5.0f);
			// Prebuild all render matrices, we will use them for several times
			// here.
			Matrix[] renderMatrices = new Matrix[3];
			for (int carNum = 0; carNum < 3; carNum++)
				renderMatrices[carNum] =
					Matrix.CreateRotationZ(RG.TotalTime / 3.9f) *
					Matrix.CreateTranslation(new Vector3(0, 5.0f, 0)) *
					Matrix.CreateRotationZ(-_carSelectionRotationZ + carNum * perCarRot) *
					Matrix.CreateTranslation(new Vector3(1.5f, 0.0f, 1.0f));
			// Last translation translates the position of the cars in the UI

			// For shadows make sure the car position is the origin
			/*RacingGameManager.Player.SetCarPosition(Vector3.Zero,
				new Vector3(0, 1, 0), new Vector3(0, 0, 1));*/

			// Now do the real rendering:
			RG.Graphics.AddToRender3D(_directLight);
			for (var i = 0; i < _cars.Length; ++i)
			{
				_carSelectionPlates[i].GlobalTransform = Constants.objectMatrix * renderMatrices[i];
				RG.Graphics.AddToRender3D(_carSelectionPlates[i]);

				_cars[i].GlobalTransform = Constants.objectMatrix * renderMatrices[i];
				RG.Graphics.AddToRender3D(_cars[i]);
			}

			_camera.View = viewMatrix;
			RG.Graphics.DoRender3D(_camera);
		}
	}
}
