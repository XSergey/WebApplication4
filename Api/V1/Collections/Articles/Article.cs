using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication4.Api.V1.Collections.Articles
{
    /// <summary>
    /// Article collection class provide Article collection object.
    /// This object use by mongodb provider class.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class Article
    {
        [BsonId]
        public ObjectId ArticleId { get; set; }

        [BsonElement("number")]
        public string ArticleNumber { get; set; }

        [BsonElement("name")]
        public string ArticleName { get; set; }

        [BsonElement("brand")]
        public string ArticleBrand { get; set; }

        [BsonElement("status")]
        public int ArticleStatus { get; set; }

        [BsonElement("is_imported")]
        public int ArticleIsImported { get; set; }
    }
}
