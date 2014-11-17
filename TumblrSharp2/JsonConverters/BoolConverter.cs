﻿namespace TumblrSharp2.JsonConverters
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
	/// Converts "Y" and "N" to boolean.
	/// </summary>
	public class BoolConverter : JsonConverter
	{
		/// <exclude/>
		public override bool CanConvert(Type objectType)
		{
			return objectType.Equals(typeof(bool));
		}

		/// <exclude/>
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteValue((bool)value ? "Y" : "N");
		}

		/// <exclude/>
		public override object ReadJson(JsonReader reader, Type	objectType, object existingValue, JsonSerializer serializer)
		{
			var value = reader.Value.ToString();
			return String.Compare(value, "Y", StringComparison.OrdinalIgnoreCase) == 0;
		}
	}
}
