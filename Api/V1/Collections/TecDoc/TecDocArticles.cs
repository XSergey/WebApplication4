using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication4.Api.V1.Collections.TecDoc
{
    /// <summary>
    /// Article collection class provide Article collection object.
    /// This object use by mongodb provider class.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class TecDocArticles
    {
        [BsonId]
        public ObjectId ArlId { get; set; }

        [BsonElement("ART_ID")]
        public int ArtId { get; set; }

        [BsonElement("ART_ARTICLE_NR")]
        public string ArtArticleNr { get; set; }
    }
}
