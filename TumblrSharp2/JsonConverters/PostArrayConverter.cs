namespace TumblrSharp2.JsonConverters
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using TumblrSharp2.Responses.Posts;

    /// <summary>
	/// Converts post objects to the proper post type.
	/// </summary>
    public class PostArrayConverter : JsonConverter
    {
		/// <exclude/>
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Post[]));
        }

		/// <exclude/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            List<Post> list = new List<Post>();
            reader.Read();
            do
            {
                if (reader.TokenType == JsonToken.EndArray)
                    break;

                JObject jo = JObject.Load(reader);
                switch (jo["type"].ToString())
                {
                    case "text":
                        list.Add(jo.ToObject<TextPost>());
                        break;

                    case "quote":
                        list.Add(jo.ToObject<QuotePost>());
                        break;

                    case "photo":
                        list.Add(jo.ToObject<PhotoPost>());
                        break;

                    case "link":
                        list.Add(jo.ToObject<LinkPost>());
                        break;
                    
                    case "answer":
                        list.Add(jo.ToObject<AnswerPost>());
                        break;
                    
                    case "audio":
                    case "chat":
                    case "video":
                        break;
                }
            }
            while (reader.Read() && reader.TokenType != JsonToken.EndArray);

            return list.ToArray();
        }

		/// <exclude/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
