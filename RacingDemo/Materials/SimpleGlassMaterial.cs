using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Nursia.Materials;
using Nursia.Rendering;
using System.ComponentModel;

namespace RacingDemo.Materials
{
	public class SimpleGlassMaterial : BaseMaterial
	{
		private static EffectBinding _effectBinding;

		[Browsable(false)]
		[JsonIgnore]
		public override MaterialFlags Flags => MaterialFlags.AcceptsDirectionalLight | MaterialFlags.CastsShadows;

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


		public override IMaterial Clone() => new SimpleGlassMaterial()
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

		public override EffectBinding GetColorTechnique(MaterialTechnique materialTechnique, LightTechnique lightTechnique, bool shadow, bool translucent, bool normalMapping, bool clipPlane)
		{
			if (_effectBinding == null)
			{
				var binding = new EffectBinding(Assets.LoadEffect("ReflectionSimpleGlass"));

				binding.AddMaterialLevelSetter<SimpleGlassMaterial>("ambientColor", (m, p) => p.SetValue(m.AmbientColor.ToVector4()));
				binding.AddMaterialLevelSetter<SimpleGlassMaterial>("diffuseColor", (m, p) => p.SetValue(m.DiffuseColor.ToVector4()));
				binding.AddMaterialLevelSetter<SimpleGlassMaterial>("specularColor", (m, p) => p.SetValue(m.SpecularColor.ToVector4()));
				binding.AddMaterialLevelSetter<SimpleGlassMaterial>("shininess", (m, p) => p.SetValue(m.Shininess));
				binding.AddMaterialLevelSetter<SimpleGlassMaterial>("alphaFactor", (m, p) => p.SetValue(m.AlphaFactor));
				binding.AddMaterialLevelSetter<SimpleGlassMaterial>("fresnelBias", (m, p) => p.SetValue(m.FresnelBias));
				binding.AddMaterialLevelSetter<SimpleGlassMaterial>("fresnelPower", (m, p) => p.SetValue(m.FresnelPower));
				binding.AddMaterialLevelSetter<SimpleGlassMaterial>("reflectionAmount", (m, p) => p.SetValue(m.ReflectionAmount));

				_effectBinding = binding;
			}

			return _effectBinding;
		}
	}
}
