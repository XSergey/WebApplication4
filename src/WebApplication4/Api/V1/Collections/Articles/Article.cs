using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication3.Api.V1.Collections.Articles
{
    /// <summary>
    /// Article collection class provide Article collection object.
    /// This object use by mongodb provider class.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class Article
    {
        [BsonElement("id")]
        public ObjectId ArticleId { get; set; }

        [BsonElement("number")]
        public string ArticleNumber { get; set; }

        [BsonElement("brand")]
        public string ArticleBrand { get; set; }

        [BsonElement("status")]
        public int ArticleStatus { get; set; }
    }
}
