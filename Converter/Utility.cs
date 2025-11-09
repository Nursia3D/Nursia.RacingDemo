using Microsoft.Xna.Framework;
using Nursia.SceneGraph;
using System;
using System.IO;
using System.Text.Json;

namespace Converter
{
	internal static class Utility
	{
		/// <summary>
		/// The value for which all absolute numbers smaller than are considered equal to zero.
		/// </summary>
		public const float ZeroTolerance = 1e-3f;

		public const string InputFolder = @"D:\Projects\Nursia.RacingGame\RacingGame\Assets\Models";
		public const string OutputFolder = @"D:\Projects\Nursia.RacingGame\RacingGame\Assets\Scenes";

		/// <summary>
		/// Compares two floating point numbers based on an epsilon zero tolerance.
		/// </summary>
		/// <param name="left">The first number to compare.</param>
		/// <param name="right">The second number to compare.</param>
		/// <param name="epsilon">The epsilon value to use for zero tolerance.</param>
		/// <returns><c>true</c> if <paramref name="left"/> is within epsilon of <paramref name="right"/>; otherwise, <c>false</c>.</returns>
		public static bool EpsilonEquals(this float left, float right, float epsilon = ZeroTolerance)
		{
			return Math.Abs(left - right) <= epsilon;
		}

		public static bool IsZero(this float a, float epsilon = ZeroTolerance)
		{
			return a.EpsilonEquals(0.0f, epsilon);
		}

		public static float ToFloat(this JsonElement val)
		{
			return val[0].GetSingle();
		}

		public static Color ToColor(this JsonElement val)
		{
			var value = new Vector4(val[0].GetSingle(), val[1].GetSingle(), val[2].GetSingle(), val[3].GetSingle());

			return new Color(value);
		}

		public static string TryToMakePathRelativeTo(string path, string pathRelativeTo)
		{
			try
			{
				var fullPathUri = new Uri(path, UriKind.Absolute);

				if (!pathRelativeTo.EndsWith(Path.DirectorySeparatorChar.ToString()))
				{
					pathRelativeTo += Path.DirectorySeparatorChar;
				}
				var folderPathUri = new Uri(pathRelativeTo, UriKind.Absolute);

				path = folderPathUri.MakeRelativeUri(fullPathUri).ToString();
			}
			catch (Exception)
			{
			}

			return path;
		}

		public static Vector3 ToEulerAngles(this Quaternion r)
		{
			return new Vector3
			{
				X = (float)Math.Asin(2.0f * (r.X * r.W - r.Y * r.Z)),
				Y = (float)Math.Atan2(2.0f * (r.Y * r.W + r.X * r.Z), 1.0f - 2.0f * (r.X * r.X + r.Y * r.Y)),
				Z = (float)Math.Atan2(2.0f * (r.X * r.Y + r.Z * r.W), 1.0f - 2.0f * (r.X * r.X + r.Z * r.Z))
			};
		}

		public static Vector3 ToDegrees(this Vector3 v) => new Vector3(MathHelper.ToDegrees(v.X), MathHelper.ToDegrees(v.Y), MathHelper.ToDegrees(v.Z));

		private static float UpdateAngle(this float angle)
		{
			var rangle = (float)Math.Round(angle);
			if (rangle.EpsilonEquals(angle))
			{
				angle = rangle;
			}

			return angle;
		}

		public static void SetTransform(this SceneNode node, Matrix transform)
		{
			transform.Decompose(out Vector3 scale, out Quaternion rotation, out Vector3 translation);
			node.Scale = scale;
			node.Rotation = rotation.ToEulerAngles().ToDegrees();
			node.Translation = translation;

			var rot = node.Rotation;
			rot.X = rot.X.UpdateAngle();
			rot.Y = rot.Y.UpdateAngle();
			rot.Z = rot.Z.UpdateAngle();
			node.Rotation = rot;

			var tr = node.Translation;
			if (tr.X.IsZero())
			{
				tr.X = 0;
			}

			if (tr.Y.IsZero())
			{
				tr.Y = 0;
			}

			if (tr.Z.IsZero())
			{
				tr.Z = 0;
			}
			node.Translation = tr;
		}
	}
}
