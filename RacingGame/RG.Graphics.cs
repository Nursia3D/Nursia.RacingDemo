using Myra.Graphics2D.UI;
using Nursia;
using Nursia.Env;
using Nursia.Rendering;
using Nursia.SceneGraph;
using Nursia.SceneGraph.Cameras;

namespace RacingGame
{
	partial class RG
	{
		public static class Graphics
		{
			private static readonly ForwardRenderer _renderer = new ForwardRenderer();
			private static readonly SceneNode _root = new SceneNode();

			public static RenderStatistics Statistics => _renderer.Statistics;

			public static Desktop UIDesktop { get; } = new Desktop();

			public static void AddToRender3D(SceneNode node)
			{
				_root.Children.Add(node);
			}

			public static void DoRender3D(Camera camera)
			{
				var vp = Nrs.GraphicsDevice.Viewport;

				camera.SetViewport(vp.Width, vp.Height);
				_renderer.Render(_root, camera, Resources.RenderEnvironment);
				_root.Children.Clear();
			}
		}
	}
}
