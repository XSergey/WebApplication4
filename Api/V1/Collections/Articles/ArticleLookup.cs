using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication4.Api.V1.Collections.Articles
{
    /// <summary>
    /// Article collection class provide Article collection object.
    /// This object use by mongodb provider class.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class ArticleLookup
    {
        [BsonId]
        public ObjectId ArticleLookupId { get; set; }

        [BsonElement("article")]
        public string ArticleLookupArticleNumber { get; set; }

        [BsonElement("brand")]
        public string ArticleLookupArticleBrand { get; set; }

        [BsonElement("lookup_article")]
        public string ArticleLookupNumber { get; set; }

        [BsonElement("lookup_brand")]
        public string ArticleLookupBrand { get; set; }
    }
}
