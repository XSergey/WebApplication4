using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication4.Api.V1.Collections.Articles
{
    /// <summary>
    /// Article collection class provide Article collection object.
    /// This object use by mongodb provider class.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class ArticleToProvider
    {
        [BsonId]
        public ObjectId ArticleToProviderId { get; set; }

        [BsonElement("article_id")]
        public ObjectId ArticleId { get; set; }

        [BsonElement("provider_id")]
        public ObjectId ProviderId { get; set; }

        [BsonElement("price")]
        public double ArticleToProviderPrice { get; set; }

        [BsonElement("currency")]
        public string ArticleToProviderCurrency { get; set; }

        [BsonElement("quantity")]
        public int ArticleToProviderQuantity { get; set; }

        [BsonElement("period")]
        public int ArticleToProviderPeriod { get; set; }

        [BsonElement("prefix")]
        public string ArticleToProviderPrefix { get; set; }

        [BsonElement("status")]
        public int ArticleToProviderStatus { get; set; }

        [BsonElement("is_imported")]
        public int ArticleToProviderIsImported { get; set; }
    }
}
