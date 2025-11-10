using Nursia;
using Nursia.Rendering;
using Nursia.SceneGraph;
using Nursia.SceneGraph.Cameras;

namespace RacingGame.Graphics
{
	public class RenderQueue
	{
		private readonly ForwardRenderer _renderer = new ForwardRenderer();
		private readonly SceneNode _root = new SceneNode();

		public void AddToRender(SceneNode node)
		{
			_root.Children.Add(node);
		}

		public void DoRender(Camera camera)
		{
			var vp = Nrs.GraphicsDevice.Viewport;

			camera.SetViewport(vp.Width, vp.Height);
			_renderer.Render(_root, camera);
			_root.Children.Clear();
		}
	}
}
