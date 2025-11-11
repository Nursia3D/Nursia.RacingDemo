using DigitalRiseModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Nursia.Materials;
using Nursia.Rendering;
using Nursia.SceneGraph.Lights;
using System.ComponentModel;

namespace RacingGame.Materials
{
	public class ReflectionSimpleGlass : IMaterial
	{
		private static EffectBinding _effectBinding;

		[Browsable(false)]
		[JsonIgnore]
		public BlendState BlendState => null;

		[Browsable(false)]
		[JsonIgnore]
		public DepthStencilState DepthStencilState => null;

		[Browsable(false)]
		[JsonIgnore]
		public RasterizerState RasterizerState => null;

		[Browsable(false)]
		[JsonIgnore]
		public MaterialFlags Flags => MaterialFlags.AcceptsDirectionalLight | MaterialFlags.CastsShadows;

		public Color AmbientColor { get; set; } = new Color(0.15f, 0.15f, 0.15f, 1.0f);
		public Color DiffuseColor { get; set; } = new Color(0.25f, 0.25f, 0.25f, 1.0f);
		public Color SpecularColor { get; set; } = new Color(1.0f, 1.0f, 1.0f, 1.0f);

		[DefaultValue(24.0f)]
		public float Shininess { get; set; } = 24.0f;

		[DefaultValue(0.66f)]
		public float AlphaFactor { get; set; } = 0.66f;

		[DefaultValue(0.5f)]
		public float FresnelBias { get; set; } = 0.5f;

		[DefaultValue(1.5f)]
		public float FresnelPower { get; set; } = 1.5f;

		[DefaultValue(1.0f)]
		public float ReflectionAmount { get; set; } = 1.0f;


		public IMaterial Clone() => new ReflectionSimpleGlass()
		{
			AmbientColor = AmbientColor,
			DiffuseColor = DiffuseColor,
			SpecularColor = SpecularColor,
			Shininess = Shininess,
			AlphaFactor = AlphaFactor,
			FresnelBias = FresnelBias,
			FresnelPower = FresnelPower,
			ReflectionAmount = ReflectionAmount
		};

		public EffectBinding GetEffectBinding(LightTechnique technique, ShadowType shadow, bool translucent, DrMeshPart mesh, bool clipPlane)
		{
			if (_effectBinding == null)
			{
				var binding = new EffectBinding(Assets.LoadEffect("ReflectionSimpleGlass"));

				binding.AddMaterialLevelSetter<ReflectionSimpleGlass>("ambientColor", (m, p) => p.SetValue(m.AmbientColor.ToVector4()));
				binding.AddMaterialLevelSetter<ReflectionSimpleGlass>("diffuseColor", (m, p) => p.SetValue(m.DiffuseColor.ToVector4()));
				binding.AddMaterialLevelSetter<ReflectionSimpleGlass>("specularColor", (m, p) => p.SetValue(m.SpecularColor.ToVector4()));
				binding.AddMaterialLevelSetter<ReflectionSimpleGlass>("shininess", (m, p) => p.SetValue(m.Shininess));
				binding.AddMaterialLevelSetter<ReflectionSimpleGlass>("alphaFactor", (m, p) => p.SetValue(m.AlphaFactor));
				binding.AddMaterialLevelSetter<ReflectionSimpleGlass>("fresnelBias", (m, p) => p.SetValue(m.FresnelBias));
				binding.AddMaterialLevelSetter<ReflectionSimpleGlass>("fresnelPower", (m, p) => p.SetValue(m.FresnelPower));
				binding.AddMaterialLevelSetter<ReflectionSimpleGlass>("reflectionAmount", (m, p) => p.SetValue(m.ReflectionAmount));

				_effectBinding = binding;
			}

			return _effectBinding;
		}
	}
}
