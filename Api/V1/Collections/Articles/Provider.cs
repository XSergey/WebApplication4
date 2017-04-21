using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication4.Api.V1.Collections.Articles
{
    /// <summary>
    /// Article collection class provide Article collection object.
    /// This object use by mongodb provider class.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class Provider
    {
        [BsonId]
        public ObjectId ProviderId { get; set; }

        [BsonElement("name")]
        public string ProviderName { get; set; }

        [BsonElement("mark_up")]
        public int ProviderMarkUp { get; set; }

        [BsonElement("status")]
        public int ProviderStatus { get; set; }

        [BsonElement("is_imported")]
        public int ProviderIsImported { get; set; }
    }
}
