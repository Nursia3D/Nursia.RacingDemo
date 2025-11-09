#region File Description
//-----------------------------------------------------------------------------
// TrackCombiModels.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using directives
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
#endregion

namespace Converter
{
	/// <summary>
	/// Little helper class to load combinations of objects, which simplifies
	/// our level creation process. Also used for randomly generated objects
	/// near the track.
	/// </summary>
	public class TrackCombiModels
	{
		#region Variables
		/// <summary>
		/// CombiObject for every object in this combi model
		/// </summary>
		[Serializable]
		public class CombiObject
		{
			/// <summary>
			/// Model name
			/// </summary>
			public string modelName;
			/// <summary>
			/// Matrix
			/// </summary>
			public Matrix matrix;

			/// <summary>
			/// Create combi object
			/// </summary>
			public CombiObject()
			{
			}

			/// <summary>
			/// Create CombiObject
			/// </summary>
			/// <param name="setModelName">Set model name</param>
			/// <param name="setMatrix">Set matrix</param>
			public CombiObject(string setModelName, Matrix setMatrix)
			{
				modelName = setModelName;
				matrix = setMatrix;
			}
		}

		/// <summary>
		/// Name of this combi model (extracted from filename).
		/// </summary>
		private string name = "";

		/// <summary>
		/// Size of this combi model.
		/// </summary>
		private float size = 10;
		#endregion

		#region Properties
		/// <summary>
		/// Name
		/// </summary>
		/// <returns>String</returns>
		public string Name
		{
			get
			{
				return name;
			}
		}

		/// <summary>
		/// Size
		/// </summary>
		public float Size
		{
			get
			{
				// Return size 10 for palms, stones and ruins, rest gets size = 50
				return size;
			}
		}

		public List<CombiObject> Objects { get; } = new List<CombiObject>();
		#endregion

		#region Load
		/// <summary>
		/// Load track combi models
		/// </summary>
		/// <param name="filename">Filename</param>
		public TrackCombiModels(string filename)
		{
			using (var stream = File.OpenRead(filename))
			using (var file = new StreamReader(stream))
			{
				// Load everything into this class with help of the XmlSerializer.
				Objects = (List<TrackCombiModels.CombiObject>)
					new XmlSerializer(typeof(List<TrackCombiModels.CombiObject>)).
					Deserialize(file.BaseStream);

				// Close the file
				file.Close();

				name = Path.GetFileNameWithoutExtension(filename);

				// Return size 10 for palms, stones and ruins, rest gets size = 50
				size = (Name == "CombiPalms" ||
						Name == "CombiPalms2" ||
						Name == "CombiRuins" ||
						Name == "CombiRuins2" ||
						Name == "CombiStones" ||
						Name == "CombiStones2") ? 10 : 50;
			}
		}
		#endregion
	}
}
