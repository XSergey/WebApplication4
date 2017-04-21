using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

using WebApplication4.Api.V1.Providers.Services.Technomir;
using Microsoft.Extensions.Primitives;

namespace WebApplication3.Api.V1.Controllers
{
    [Route("api/[controller]")]
    public class ArticlesController : Controller
    {
        // GET: api/import
        [HttpGet("{ArticleNumber}")]
        public async Task<JsonResult> Get(string ArticleNumber)
        {

            StringValues brandId = "";
            StringValues brandName = "";
            StringValues isReady = "0";
            Request.Query.TryGetValue("BrandId", out brandId);
            Request.Query.TryGetValue("BrandName", out brandName);
            Request.Query.TryGetValue("isReady", out isReady);

            brandId = brandId.ToString();
            brandName = brandName.ToString();
            string _isReady = isReady.ToString();

            var json = Json(new {/* Empty */});

            if (!string.IsNullOrEmpty(brandId))
            {
                Technomir technomir = new Technomir();

                await technomir.technomirGetArticlesByBrand(ArticleNumber, brandId, brandName, _isReady);

                var _articles = technomir._articles;

                json = Json(
                        new
                        {
                            Status = "Success",
                            Message = "This is a test API for article search by Sergey Martishin.",
                            //String = _priceWithCross._a,/*new JavaScriptSerializer().Serialize(_priceWithCross._a)*//*.ToArray()*/
                            Result = _articles
                        }
                    );
            }
            else
            {
                Technomir technomir = new Technomir();

                //technomir.technomirGetBrands(ArticleNumber, _isReady);

                //var _brands = technomir._brands;
                var _brands = await technomir.technomirGetBrands(ArticleNumber, _isReady);
                json = Json(
                        new
                        {
                            Status = "Success",
                            Message = "This is a test API for article search by Sergey Martishin.",
                            //String = _priceWithCross._a,/*new JavaScriptSerializer().Serialize(_priceWithCross._a)*//*.ToArray()*/
                            Result = _brands
                        }
                    );

            }

            // Response section
            return json;
        }
    }
}
