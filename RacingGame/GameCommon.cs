using DigitalRiseModel;
using Microsoft.Xna.Framework;
using Nursia;
using Nursia.Rendering;
using Nursia.SceneGraph;
using Nursia.SceneGraph.Cameras;
using RacingGame.Graphics;
using System.Linq;

namespace RacingGame
{
	internal static class GameCommon
	{
		private static NursiaModelNode _carModel;
		private static int[] _wheelsBoneIndices;
		private static readonly ForwardRenderer _renderer = new ForwardRenderer();
		private static readonly SceneNode _root = new SceneNode();

		public static NursiaModelNode CarModel
		{
			get
			{
				if (_carModel == null)
				{
					_carModel = (NursiaModelNode)BaseGame.Content.LoadSceneNode("Scenes/Car.scene");
					_wheelsBoneIndices = (from b in _carModel.Model.Bones where b.Mesh != null && b.Mesh.MeshParts.Count == 2 select b.Index).ToArray();
				}

				int wheelNumber = 0;

				for(var i = 0; i < _wheelsBoneIndices.Length; ++i)
				{
					var idx = _wheelsBoneIndices[i];
					wheelNumber++;

					var rotationMatrix = Matrix.CreateRotationX(
						// Rotate left 2 wheels forward, the other 2 backward!
						(wheelNumber == 2 || wheelNumber == 4 ? 1 : -1) *
						RacingGameManager.Player.CarWheelPos);

					_carModel.ModelInstance.SetBoneLocalTransform(idx, rotationMatrix);
				}

				return _carModel;
			}
		}

		public static void AddToRender(SceneNode node)
		{
			_root.Children.Add(node);
		}

		public static void DoRender(Camera camera)
		{
			var vp = Nrs.GraphicsDevice.Viewport;

			camera.SetViewport(vp.Width, vp.Height);
			_renderer.Render(_root, camera);
			_root.Children.Clear();
		}
	}
}
