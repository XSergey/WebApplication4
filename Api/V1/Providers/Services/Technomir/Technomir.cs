using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Text;

using WebApplication4.Api.V1.Collections;
using WebApplication4.Api.V1.Collections.Articles;
using WebApplication4.Api.V1.Containers.Articles;
using System.Text.RegularExpressions;

namespace WebApplication4.Api.V1.Providers.Services.Technomir
{
    public class Technomir
    {
        //private string _apiUsrLogin = "";

        //private string _apiUsrPassword = "";

        private string _apiUrlGetPriceWithCross = "https://www.tehnomir.com.ua/ws/xml.php?act=GetPriceWithCrosses&usr_login=dpautogroup&usr_passwd=kas200";

        public XmlTextReader reader;

        public WebResponse response;

        public Dictionary<string, object> _responseFromXmlByUrl;

        public Dictionary<string, object> _list;

        public Dictionary<string, object> _brands;

        public Dictionary<string, object> _articles;

        public string _a;


        /**
         * Method - Technomir(string Query)
         * This method return all information about article.
         * @return Object type Dictionary<string, object>
         */
        public async Task<Dictionary<string, object>> TechnomirData(string Query, string isReady)
        {
            // Get dictionary data ftom XML resource.
            _getJsonFromXmlByUrl(_apiUrlGetPriceWithCross + "&PartNumber=" + Query);

            // Save it to data...
            dynamic _data = this._responseFromXmlByUrl;

            Dictionary<string, object> dictionary = new Dictionary<string, object>();


            // Prepare dictionary reading.
            // In some cases we can get Producers or Prices.

            //if data contain Producers and is not NULL.
            if (_data["PriceResult"]["Producers"] != null)
            {
                // Then we create new dictionary from Producers.
                foreach (var brand in _data["PriceResult"]["Producers"]["Producer"])
                {
                    //var b = brand["BrandId"];
                    if (!dictionary.ContainsKey(brand["BrandId"]))
                    {
                        Dictionary<string, object> articles = new Dictionary<string, object>();
                        Dictionary<string, object> analogs = new Dictionary<string, object>();

                        _getJsonFromXmlByUrl(_apiUrlGetPriceWithCross + "&PartNumber=" + Query + "&BrandId=" + brand["BrandId"]);

                        dynamic _dataFromBrands = this._responseFromXmlByUrl;

                        // Searching all articles by brand
                        // If article is empty in Prices then brand will be hidden
                        // Object -> Brands -> [Articles,Analogs] container will be ...

                        if (_dataFromBrands.ContainsKey("PriceResult"))
                        {
                            foreach (dynamic price in _dataFromBrands["PriceResult"]["Prices"]["Price"])
                            {
                                if (price.GetType() != typeof(KeyValuePair<string, object>))
                                {
                                    //var p = price;
                                    int ddc = 0;
                                    int dtc = 0;

                                    if (price.ContainsKey("DeliveryDays"))
                                    {
                                        if (string.IsNullOrWhiteSpace(price["DeliveryDays"]))
                                        {
                                            ddc = 0;
                                        } else
                                        {
                                            ddc = Int32.Parse(price["DeliveryDays"]);
                                        }
                                    }
                                    if (price.ContainsKey("DeliveryTime"))
                                    {
                                        dtc = Int32.Parse(price["DeliveryTime"]);
                                    }
                                    if (
                                            //ddc < 8 &&
                                            price["PartId"] != null &&
                                            price["PriceLogo"] != null &&
                                            price["Price"] != null &&
                                            price["Quantity"] != null &&
                                            price["Quantity"] != "0" //&&
                                            //(isReady == "1" && ddc == 0 ||
                                            //isReady == "0" && ddc < 8)
                                        )
                                    {
                                        if (Query != price["PartNumberShort"])
                                        {
                                            // Analogs dont relate with Articles data
                                            /*if (analogs.ContainsKey(price["Brand"]))
                                            {
                                                foreach (dynamic analog in analogs)
                                                {
                                                    if (analog.Key == price["Brand"])
                                                    {
                                                        analogs[price["Brand"]][price["PartNumberShort"]] = new Dictionary<string, object>
                                                        { { price["Brand"], price["PartNumberShort"] } };
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                analogs.Add(
                                                    price["Brand"],
                                                    new Dictionary<string, object> {
                                                        {
                                                            price["PartNumberShort"],
                                                            new Dictionary<string, object> {
                                                                {
                                                                    price["PartId"],
                                                                    price["PartNumberShort"]
                                                                }
                                                            }
                                                        }
                                                    }
                                                );
                                            }
                                            */
                                            if (isReady != "1")
                                            {
                                                analogs.Add(price["PartId"], price["PartNumberShort"]);
                                            }
                                            else
                                            {
                                                // Search analogs from DB
                                                if (analogs.Count < 1)
                                                {
                                                    //analogs.Add(price["PartId"], price["PartNumberShort"]);
                                                    dynamic importTask = await Task.Run(() => { return addImportedArticles("analogs", price, Query, ""); });
                                                    await Task.WhenAll(new Task[] { importTask });

                                                    if (importTask.Result.Count > 0)
                                                    {
                                                        foreach (var res in importTask.Result)
                                                        {
                                                            if (!analogs.ContainsKey(res.Key))
                                                            {
                                                                analogs.Add(res.Key, price["PartNumberShort"]);
                                                            }
                                                        }
                                                    }
                                                }

                                            }
                                        }
                                        else
                                        {
                                            if (isReady != "1")
                                            {
                                                articles.Add(price["PartId"], price["PartNumberShort"]);
                                            }
                                            else
                                            {
                                                // Search analogs from DB
                                                if(articles.Count < 1) {
                                                dynamic importTask = await Task.Run(() => { return addImportedArticles("analogs", price, Query, ""); });
                                                await Task.WhenAll(new Task[] { importTask });

                                                    if (importTask.Result.Count > 0)
                                                    {
                                                        foreach (var res in importTask.Result)
                                                        {
                                                            if (!articles.ContainsKey(res.Key))
                                                            {
                                                                articles.Add(
                                                                    res.Key,
                                                                    res.Value
                                                                );
                                                            }
                                                        }
                                                    }
                                                }

                                            }
                                        }
                                    }
                                } else {
                                    //Price type is KeyValuePair
                                    var priceV = _dataFromBrands["PriceResult"]["Prices"]["Price"];
                                    if (
                                        priceV["Quantity"] != null &&
                                        priceV["Quantity"] != "0"
                                    ) {
                                            if(!articles.ContainsKey(priceV["PartId"])) {
                                                articles.Add(priceV["PartId"], priceV["PartNumberShort"]);
                                            }
                                            // Search analogs from DB
                                            if (articles.Count < 1)
                                            {
                                                dynamic importTask = await Task.Run(() => { return addImportedArticles("analogs", priceV, Query, ""); });
                                                await Task.WhenAll(new Task[] { importTask });

                                                if (importTask.Result.Count > 0)
                                                {
                                                    foreach (var res in importTask.Result)
                                                    {
                                                        if (!articles.ContainsKey(res.Key))
                                                        {
                                                            articles.Add(
                                                                res.Key,
                                                                res.Value
                                                            );
                                                        }
                                                    }
                                                }
                                            }
                                    }
                                }
                            }
                        }

                        // Dictionary container
                        // for response
                        dictionary.Add(
                            brand["BrandId"],
                            new Dictionary<string, object> {
                                {
                                    "BrandId", brand["BrandId"]
                                },
                                {
                                    "BrandName", brand["Brand"]
                                },
                                {
                                    "Articles", articles
                                },
                                {
                                    "Analogs", analogs
                                }
                            }
                        );
                    }
                }
            }
            else
            {
                // Else we need to find all brands in Price

                // Before we can group, sort or optimize Price object
                var _optPrice = _data["PriceResult"]["Prices"]["Price"];

                Dictionary<string, object> articles = new Dictionary<string, object>();
                Dictionary<string, object> analogs = new Dictionary<string, object>();

                // Loop each price
                foreach (dynamic price in _data["PriceResult"]["Prices"]["Price"])
                {
                    if (price.GetType() != typeof(KeyValuePair<string, object>))
                    {
                        var p = price;
                        int ddc = 0; //+1 day for all providers
                        int dtc = 0; //+1 day for all providers

                        if (price.ContainsKey("DeliveryDays"))
                        {
                            if (string.IsNullOrWhiteSpace(price["DeliveryDays"]))
                            {
                                ddc = 0;
                            }
                            else
                            {
                                ddc += Int32.Parse(price["DeliveryDays"]);
                            }
                        }
                        if (price.ContainsKey("DeliveryTime"))
                        {
                            dtc += Int32.Parse(price["DeliveryTime"]);
                        }
                        if (
                                ddc < 15 &&
                                price["PartId"] != null &&
                                price["PriceLogo"] != null &&
                                price["Price"] != null &&
                                price["Quantity"] != null &&
                                price["Quantity"] != "0" //&&
                            )
                        {
                            if (Query != price["PartNumberShort"])
                            {
                                /*if (analogs.ContainsKey(price["Brand"]))
                                {
                                    foreach (dynamic analog in analogs)
                                    {
                                        if (analog.Key == price["Brand"])
                                        {
                                            analogs[price["Brand"]][price["PartNumberShort"]] = new Dictionary<string, object>
                                            { { price["PartId"], price["PartNumberShort"] } };
                                        }
                                    }
                                }
                                else
                                {
                                    analogs.Add(price["Brand"], price["PartNumberShort"]);
                                }
                                */
                                if (isReady != "1")
                                {
                                    articles.Add(price["PartId"], price["PartNumberShort"]);
                                }
                                else
                                {
                                    // Search analogs from DB
                                    if (analogs.Count < 1)
                                    {
                                        //articles.Add(price["PartId"], price["PartNumberShort"]);
                                        dynamic importTask = await Task.Run(() => { return addImportedArticles("analogs", price, Query, ""); });
                                        await Task.WhenAll(new Task[] { importTask });

                                        if (importTask.Result.Count > 0)
                                        {
                                            foreach (var res in importTask.Result)
                                            {
                                                if (!analogs.ContainsKey(res.Key))
                                                {
                                                    analogs.Add(res.Key, price["PartNumberShort"]);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if(isReady != "1") {
                                    articles.Add(price["PartId"], price["PartNumberShort"]);
                                } else {
                                    // Search analogs from DB
                                    if(articles.Count < 1) {
                                        dynamic importTask = await Task.Run(() => { return addImportedArticles("articles", price, Query, ""); });
                                        await Task.WhenAll(new Task[] { importTask });

                                        if (importTask.Result.Count > 0)
                                        {
                                            foreach (var res in importTask.Result)
                                            {
                                                if (!articles.ContainsKey(res.Key))
                                                {
                                                    articles.Add(res.Key, price["PartNumberShort"]);
                                                }
                                            }
                                        }
                                    }
                                    
                                }
                            }

                            // Dictionary container
                            // for response
                            if (
                                !dictionary.ContainsKey(price["BrandId"]) 
                                && price["PartNumberShort"] == Query //&&
                                //(articles != null/* || analogs != null*/)
                                )
                            {

                                dictionary.Add(
                                price["BrandId"],
                                new Dictionary<string, object> {
                                        {
                                            "BrandId", price["BrandId"]
                                        },
                                        {
                                            "BrandName", price["Brand"]
                                        },
                                        {
                                            "Articles", articles
                                        },
                                        {
                                            "Analogs", analogs
                                        }
                                    }
                                );
                            }
                        }
                    }
                }
            }

            // Summary dictionary
            return dictionary;
        }

        /*public ArrayList GetPriceWithCross()
        {
            //We can check here errors from reader and filter all response
            return this._a;
        }*/

        public async Task<Dictionary<string, object>> technomirGetBrands(string Query, string isReady)
        {
            //await TechnomirData(Query, isReady);

            Dictionary<string, object> brands = new Dictionary<string, object>();
            Dictionary<string, object> list = await TechnomirData(Query, isReady);

            foreach (dynamic brand in list)
            {
                if( list[brand.Key]["Articles"].Count > 0
                    || list[brand.Key]["Analogs"].Count > 0
                    //|| (isReady == "1" && list[brand.Key]["Articles"].Count > 0)
                )
                {
                    // Dictionary container for response
                    if (!brands.ContainsKey(brand.Key))
                    {
                        brands.Add(
                            brand.Key,
                            new Dictionary<string, object> {
                                {
                                    "BrandId", brand.Key
                                },
                                {
                                    "BrandName", list[brand.Key]["BrandName"].ToUpper()
                                },
                            }
                        );
                    }
                }
                else
                {
                    // Empty.
                }
            }
            //brands = this._list;
            return brands;
        }

        public async Task technomirGetArticlesByBrand(string Query, string BrandId, string BrandName, string isReady)
        {
            string _Query = Query.ToUpper();
            string _BrandName = BrandName;

            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            Dictionary<string, object> articles = new Dictionary<string, object>();
            //List<object> analogs = new List<object>();
            Dictionary<string, object> analogs = new Dictionary<string, object>();

            _getJsonFromXmlByUrl(_apiUrlGetPriceWithCross + "&PartNumber=" + _Query + "&BrandId=" + BrandId);

            dynamic _dataFromBrands = this._responseFromXmlByUrl;

            // First of all we can get articles by lookup
            // This option will add all articles from DB without Technomir service.
            dynamic FindLookupTask = await Task.Run(() => { return getArticlesLookupByQuery(_Query); });
            //await Task.WhenAll(new Task[] { FindLookupTask }); //dont use it.
            analogs = FindLookupTask;
            //var lookup = FindLookupTask;

            // Searching all articles by brand
            // If article is empty in Prices then brand will be hidden
            // Object -> Brands -> [Articles,Analogs] container will be ...

            if (_dataFromBrands.ContainsKey("PriceResult"))
            {
                foreach (dynamic price in _dataFromBrands["PriceResult"]["Prices"]["Price"])
                {
                    if (price.GetType() != typeof(KeyValuePair<string, object>))
                    {
                        //var p = price;
                        int ddc = 1;
                        int dtc = 1;
                        
                        if (price.ContainsKey("DeliveryDays"))
                        {
                            if (string.IsNullOrWhiteSpace(price["DeliveryDays"]))
                            {
                                ddc = 0;
                            }
                            else
                            {
                                ddc += Int32.Parse(price["DeliveryDays"]);
                            }
                        }
                        if (price.ContainsKey("DeliveryTime"))
                        {
                            dtc += Int32.Parse(price["DeliveryTime"]);
                        }
                        if (
                                //articles.ContainsKey(price["PartId"]) &&
                                //articles[price["PartId"]]["PriceLogo"] != price["PriceLogo"] &&
                                //price != null &&
                                ddc < 11 &&
                                price["PartId"] != null &&
                                price["PriceLogo"] != null &&
                                price["Price"] != null &&
                                price["Quantity"] != null &&
                                price["Quantity"] != "0" //&&
                                //(ddc < 11 || dtc < 11)
                                //price.ContainsValue(price["Price"]) == false
                            )
                        {
                            if (_Query != price["PartNumberShort"])
                            {
                                if (isReady == "0")
                                {
                                    analogs.Add(
                                        price["PartId"],
                                        formatPrice(price)
                                    );
                                    // Search analogs from DB
                                    dynamic importTask = await Task.Run(() => { return addImportedArticles("analogs", price, Query, ""); });
                                    await Task.WhenAll(new Task[] { importTask });

                                    if (importTask.Result.Count > 0)
                                    {
                                        foreach (var res in importTask.Result)
                                        {
                                            if (!analogs.ContainsKey(res.Key))
                                            {
                                                analogs.Add(
                                                    res.Key,
                                                    res.Value
                                                );
                                            }
                                        }
                                    }
                                }
                                else if (isReady == "1")
                                {
                                    if (ddc == 0)
                                    {
                                        analogs.Add(
                                            price["PartId"],
                                            formatPrice(price)
                                        );
                                    }
                                    // Search analogs from DB
                                    dynamic importTask = await Task.Run(() => { return addImportedArticles("analogs", price, Query, "", isReady); });
                                    await Task.WhenAll(new Task[] { importTask });

                                    if (importTask.Result.Count > 0)
                                    {
                                        foreach (var res in importTask.Result)
                                        {
                                            if (!analogs.ContainsKey(res.Key))
                                            {
                                                analogs.Add(
                                                    res.Key,
                                                    res.Value
                                                );
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (isReady == "0")
                                {
                                    articles.Add(
                                        price["PartId"],
                                        formatPrice(price)
                                    );
                                    // Search analogs from DB
                                    dynamic importTask = await Task.Run(() => { return addImportedArticles("articles", price, Query, ""); });
                                    await Task.WhenAll(new Task[] { importTask });

                                    if (importTask.Result.Count > 0)
                                    {
                                        foreach (var res in importTask.Result)
                                        {
                                            if (!articles.ContainsKey(res.Key))
                                            {
                                                articles.Add(
                                                    res.Key,
                                                    res.Value
                                                );
                                            }
                                        }
                                    }
                                }
                                else if (isReady == "1")
                                {
                                    if (ddc == 0)
                                    {
                                        articles.Add(
                                            price["PartId"],
                                            formatPrice(price)
                                        );
                                    }

                                    // Search analogs from DB
                                    dynamic importTask = await Task.Run(() => { return addImportedArticles("articles", price, Query, "", isReady); });
                                    await Task.WhenAll(new Task[] { importTask });

                                    if (importTask.Result.Count > 0)
                                    {
                                        foreach (var res in importTask.Result)
                                        {
                                            if (!articles.ContainsKey(res.Key))
                                            {
                                                articles.Add(
                                                    res.Key,
                                                    res.Value
                                                );
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    } else {
                        //Price type is KeyValuePair
                        var priceV = _dataFromBrands["PriceResult"]["Prices"]["Price"];
                        if (_Query == priceV["PartNumberShort"] && priceV["PartNumberShort"] != null) {
                            if (
                                priceV["Quantity"] != null &&
                                priceV["Quantity"] != "0"
                            )
                            {
                                if (isReady == "0")
                                {
                                    if (!articles.ContainsKey(priceV["PartId"]))
                                    {
                                        articles.Add(priceV["PartId"], formatPrice(priceV));
                                    }
                                    dynamic importTask = await Task.Run(() => { return addImportedArticles("analogs", priceV, Query, ""); });
                                    await Task.WhenAll(new Task[] { importTask });

                                    if (importTask.Result.Count > 0)
                                    {
                                        foreach (var res in importTask.Result)
                                        {
                                            if (!articles.ContainsKey(res.Key))
                                            {
                                                articles.Add(
                                                    res.Key,
                                                    res.Value
                                                );
                                            }
                                        }
                                    }
                                }
                                else if (isReady == "1")
                                {
                                    dynamic importTask = await Task.Run(() => { return addImportedArticles("analogs", priceV, Query, ""); });
                                    await Task.WhenAll(new Task[] { importTask });

                                    if (importTask.Result.Count > 0)
                                    {
                                        foreach (var res in importTask.Result)
                                        {
                                            if (!articles.ContainsKey(res.Key))
                                            {
                                                articles.Add(
                                                    res.Key,
                                                    res.Value
                                                );
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //Sorting

                    //var sortedDict = from entry in articles orderby entry.Value ascending select entry;

                    /* If empty articles then search by Query and BrandName
                     * Get imported articles from database
                     */
                    //if (articles.Count == 0)
                    //{
                        dynamic importTask1 = await Task.Run(() => { return addImportedArticles("empty", price, _Query, _BrandName); });
                        await Task.WhenAll(new Task[] { importTask1 });

                        if (importTask1.Result.Count > 0)
                        {
                            foreach (var res in importTask1.Result)
                            {
                                if (!articles.ContainsKey(res.Key))
                                {
                                    articles.Add(
                                        res.Key,
                                        res.Value
                                    );
                                }
                            }
                        }
                    //}
                }
                

                // Dictionary container
                // for response

                dictionary.Add(
                    BrandId,
                    new Dictionary<string, object> {
                        {
                            "BrandId", BrandId
                        },
                        {
                            "BrandName", Query
                        },
                        {
                            "Articles", articles
                        },
                        {
                            "Analogs", analogs
                        }
                    }
                );
            }

            this._articles = dictionary;
        }

        public Dictionary<string, string> formatPrice(object Price)
        {
            dynamic price = Price;
            Dictionary<string, string> list = new Dictionary<string, string>();
            
            if (price != null)
            {
                byte[] bytes = Encoding.Default.GetBytes(price["PartDescriptionRus"]);
                string priceName = Encoding.UTF8.GetString(bytes);

                int deliveryDays = Int32.Parse(price["DeliveryDays"]);
                string finalDeliveryDays = (deliveryDays + 1).ToString();

                list.Add("id", price["PartId"]);
                list.Add("name", priceName);
                list.Add("number", price["PartNumberShort"]);
                list.Add("brand", price["Brand"].ToUpper());
                list.Add("quantity", price["Quantity"]);
                list.Add("prefix", price["QuantityType"]);
                list.Add("provider", price["PriceLogo"]);
                list.Add("period", finalDeliveryDays);
                list.Add("orig_price", price["Price"]);
                list.Add("currency", price["Currency"]);
                list.Add("delivery_type", price["DeliveryType"]);
                list.Add("delivery_percent", price["DeliveryPercent"]);
                list.Add("weight", price["Weight"]);
                list.Add("is_imported", "0");
            }

            return list;
        }

        public async Task<Dictionary<string, object>> addImportedArticles(string type, dynamic price, string Query, string BrandName = "", string isReady = "0")
        {
            dynamic ArticleNumber = Query;
            dynamic ArticleBrandName = BrandName;
            if(type != "empty") {
                ArticleNumber = price["PartNumberShort"];
                ArticleBrandName = price["Brand"];
            }

            Dictionary<string, object> list = new Dictionary<string, object>();

            // Article find
            ArticlesCollection Article = new ArticlesCollection();
            dynamic ArticleFindTask = await Task.Run(() => { return Article.FindLikeBrand(ArticleNumber, ArticleBrandName.ToUpper()); });
            await Task.WhenAll(new Task[] { ArticleFindTask });
            //var aaa = ArticleFindTask;
            if(ArticleFindTask.Result.Count > 0)
            {
                foreach(var ArticleValues in ArticleFindTask.Result)
                {
                    ArticlesToProvidersCollection ArticleToProvider = new ArticlesToProvidersCollection();
                    dynamic ArticleToProviderFindTask = await Task.Run(() => { return ArticleToProvider.FindByArticleId(ArticleValues.ArticleId); });
                    await Task.WhenAll(new Task[] { ArticleToProviderFindTask });

                    if(ArticleToProviderFindTask.Result.Count > 0)
                    {
                        foreach (var Atp in ArticleToProviderFindTask.Result)
                        {
                            if(isReady == "1" && Atp.ArticleToProviderPeriod == 0) {
                                var ProviderId = Atp.ProviderId;
                                ProvidersCollection Provider = new ProvidersCollection();
                                dynamic ProviderFindTask = await Task.Run(() => { return Provider.FindById(ProviderId); });
                                await Task.WhenAll(new Task[] { ProviderFindTask });

                                list.Add(Atp.ArticleToProviderId.ToString(), new Dictionary<string, object>{
                                    { "id", Atp.ArticleToProviderId.ToString() },
                                    { "name", ArticleValues.ArticleName },
                                    { "number", ArticleValues.ArticleNumber },
                                    { "brand", ArticleValues.ArticleBrand },
                                    { "quantity", Atp.ArticleToProviderQuantity },
                                    { "prefix", Atp.ArticleToProviderPrefix },
                                    { "provider", ProviderFindTask.Result[0].ProviderName },
                                    { "period", Atp.ArticleToProviderPeriod },
                                    { "orig_price", Atp.ArticleToProviderPrice.ToString().Replace(",", ".") },
                                    { "currency", Atp.ArticleToProviderCurrency },
                                    { "is_imported", 0 }
                                });
                            } else {
                                var ProviderId = Atp.ProviderId;
                                ProvidersCollection Provider = new ProvidersCollection();
                                dynamic ProviderFindTask = await Task.Run(() => { return Provider.FindById(ProviderId); });
                                await Task.WhenAll(new Task[] { ProviderFindTask });
                                //var a1 = ArticleValues.ArticleId;
                                //var a2 = ArticleValues.ArticleId.ToString();
                                //var a3 = Atp.ArticleToProviderQuantity.ToString();
                                //var a4 = Atp.ArticleToProviderPrefix;
                                //var a5 = ProviderFindTask.Result[0].ProviderName;
                                //var a6 = Atp.ArticleToProviderPeriod.ToString();
                                //var a7 = Atp.ArticleToProviderPrice.ToString();
                                //var a8 = Atp.ArticleToProviderCurrency.ToString();
                                //var a9 = Atp.ArticleToProviderIsImported;

                                list.Add(Atp.ArticleToProviderId.ToString(), new Dictionary<string, object>{
                                    { "id", Atp.ArticleToProviderId.ToString() },
                                    { "name", ArticleValues.ArticleName },
                                    { "number", ArticleValues.ArticleNumber },
                                    { "brand", ArticleValues.ArticleBrand },
                                    { "quantity", Atp.ArticleToProviderQuantity },
                                    { "prefix", Atp.ArticleToProviderPrefix },
                                    { "provider", ProviderFindTask.Result[0].ProviderName },
                                    { "period", Atp.ArticleToProviderPeriod },
                                    { "orig_price", Atp.ArticleToProviderPrice.ToString().Replace(",", ".") },
                                    { "currency", Atp.ArticleToProviderCurrency },
                                    { "is_imported", 0 }
                                });
                            }
                            
                        }
                        //var Atp = ArticleToProviderFindTask.Result;
                        

                        /*list.Add("id", ArticleValues.ArticleId);
                        list.Add("name", ArticleValues.ArticleName);
                        list.Add("number", ArticleValues.ArticleNumber);
                        list.Add("brand", ArticleValues.ArticleBrand);
                        list.Add("quantity", Atp[0].ArticleToProviderQuantity.ToString());
                        list.Add("prefix", Atp[0].ArticleToProviderPrefix);
                        list.Add("provider", ProviderFindTask.Result[0].ProviderName);
                        list.Add("period", Atp[0].ArticleToProviderPeriod.ToString());
                        list.Add("orig_price", Atp[0].ArticleToProviderPrice.ToString());
                        list.Add("currency", Atp[0].ArticleToProviderCurrency.ToString());
                        list.Add("is_imported", Atp[0].ArticleToProviderIsImported,ToString());*/
                    }
                }
            }

            /*if(TecDocArtLookupFindTask.Result.Count > 0)
            {
                foreach(var Lookup in TecDocArtLookupFindTask.Result)
                {
                    TecDocBrandCollection TecDocBrand = new TecDocBrandCollection();
                    dynamic TecDocBrandFindTask = await Task.Run(() => { return TecDocBrand.FindByBrandId(Lookup.ArlBrandId); });
                    await Task.WhenAll(new Task[] { TecDocBrandFindTask });

                    var resultBrand = TecDocBrandFindTask.Result;

                    if (TecDocBrandFindTask.Result.Count > 0)
                    {
                        ArticlesCollection ArticleSecond = new ArticlesCollection();
                        dynamic ArticleByLookupFindTask = await Task.Run(() => { return ArticleSecond.Find(Lookup.ArlSearchNumber, TecDocBrandFindTask.Result[0].BraBrand); });
                        await Task.WhenAll(new Task[] { ArticleByLookupFindTask });

                        if (ArticleByLookupFindTask.Result.Count > 0)
                        {
                            var resultArt = ArticleByLookupFindTask.Result;
                        }
                    }
                }

            }*/

            /*if (type == "analogs")
            {
                // Article Lookup find
                ArticlesLookupCollection ArticlesLookup = new ArticlesLookupCollection();
                dynamic ArticlesLookupFindTask = await Task.Run(() => { return ArticlesLookup.FindLikeBrand(price["PartNumberShort"], price["Brand"]); });
                await Task.WhenAll(new Task[] { ArticlesLookupFindTask });
                //var aaa = ArticleFindTask;
                if (ArticlesLookupFindTask.Result.Count > 0)
                {
                    foreach (var ArticleLookupValues in ArticlesLookupFindTask.Result)
                    {
                        // Article find
                        var articleLookupNumber = ArticleLookupValues.ArticleLookupNumber;
                        var articleLookupBrand = ArticleLookupValues.ArticleLookupBrand;

                        ArticlesCollection ArticleSecond = new ArticlesCollection();
                        dynamic ArticleSecondFindTask = await Task.Run(() => { return ArticleSecond.Find(articleLookupNumber, articleLookupBrand); });
                        await Task.WhenAll(new Task[] { ArticleSecondFindTask });


                        if (ArticleSecondFindTask.Result.Count > 0)
                        {
                            foreach (var ArticleValues in ArticleSecondFindTask.Result)
                            {

                                var ArticleId = ArticleValues.ArticleId;

                                ArticlesToProvidersCollection ArticleToProviderSecond = new ArticlesToProvidersCollection();
                                dynamic ArticleToProviderSecondFindTask = await Task.Run(() => { return ArticleToProviderSecond.FindByArticleId(ArticleId); });
                                await Task.WhenAll(new Task[] { ArticleToProviderSecondFindTask });

                                if (ArticleToProviderSecondFindTask.Result.Count > 0)
                                {
                                    foreach (var Atp in ArticleToProviderSecondFindTask.Result)
                                    {
                                        var ProviderId = Atp.ProviderId;
                                        ProvidersCollection Provider = new ProvidersCollection();
                                        dynamic ProviderFindTask = await Task.Run(() => { return Provider.FindById(ProviderId); });
                                        await Task.WhenAll(new Task[] { ProviderFindTask });


                                        list.Add(Atp.ArticleToProviderId.ToString(), new Dictionary<string, object>{
                                                { "id", Atp.ArticleToProviderId.ToString() },
                                                { "name", ArticleValues.ArticleName },
                                                { "number", ArticleValues.ArticleNumber },
                                                { "brand", ArticleValues.ArticleBrand },
                                                { "quantity", Atp.ArticleToProviderQuantity },
                                                { "prefix", Atp.ArticleToProviderPrefix },
                                                { "provider", ProviderFindTask.Result[0].ProviderName },
                                                { "period", Atp.ArticleToProviderPeriod },
                                                { "orig_price", Atp.ArticleToProviderPrice.ToString().Replace(",", ".") },
                                                { "currency", Atp.ArticleToProviderCurrency },
                                                { "is_imported", Atp.ArticleToProviderIsImported }
                                            });

                                    }
                                }
                            }
                        }
                    }
                }
            }*/
            return list;
        }

        public async Task<Dictionary<string, object>> getArticlesLookupByQuery(string Query)
        {
            Dictionary<string, object> list = new Dictionary<string, object>();
            List<string> lookups = new List<string>();

            TecDocArtLookupCollection TecDocArtLookup = new TecDocArtLookupCollection();
            var TecDocArtLookupFindTask = await TecDocArtLookup.FindByNumber(Query);
            //var TecDocArtLookupFindTask = listLookup.Distinct().ToList();
            //var TecDocArtLookupFindTask = listLookup;
            //dynamic TecDocArtLookupFindTask = await Task.Run(() => { return TecDocArtLookup.FindByNumber(Query); });
            //await Task.WhenAll(new Task[] { TecDocArtLookupFindTask });

            //var resultLookup = TecDocArtLookupFindTask;
            if (TecDocArtLookupFindTask != null)
            {
                foreach (var Article in TecDocArtLookupFindTask)
                {
                    // Search 1
                    TecDocArticlesCollection TecDocArticle = new TecDocArticlesCollection();
                    dynamic ArticlesByTecDocFindTask = await Task.Run(() => { return TecDocArticle.FindByArticleId(Article.ArlArtId); });
                    //await Task.WhenAll(new Task[] { ArticlesByTecDocFindTask });
                    //var AlticlesList = ArticlesByTecDocFindTask;

                    if (ArticlesByTecDocFindTask != null)
                    {
                        foreach (var ArticlesValues in ArticlesByTecDocFindTask)
                        {
                            if(!lookups.Contains(ArticlesValues.ArtArticleNr.ToString())) {
                                lookups.Add(ArticlesValues.ArtArticleNr.ToString());
                            }
                        }
                    }
                }
                
            }
            foreach(var lookup in lookups) {
                string ArticleNumber = Regex.Replace(lookup.ToString(), @"[^A-Za-z0-9]+", "");
                ArticleNumber = Regex.Replace(ArticleNumber, " ", "");
                ArticlesCollection ArticleSecond = new ArticlesCollection();
                dynamic ArticleByLookupFindTask = await Task.Run(() => { return ArticleSecond.FindByArticleNumber(ArticleNumber); });
                //await Task.WhenAll(new Task[] { ArticleByLookupFindTask });

                if (ArticleByLookupFindTask.Count > 0)
                {
                    foreach (var ArticleValues in ArticleByLookupFindTask)
                    {
                        if (Query != ArticleValues.ArticleNumber)
                        { 
                            var ArticleId = ArticleValues.ArticleId;

                            ArticlesToProvidersCollection ArticleToProviderSecond = new ArticlesToProvidersCollection();
                            dynamic ArticleToProviderSecondFindTask = await Task.Run(() => { return ArticleToProviderSecond.FindByArticleId(ArticleId); });
                            await Task.WhenAll(new Task[] { ArticleToProviderSecondFindTask });

                            if (ArticleToProviderSecondFindTask.Result.Count > 0)
                            {
                                foreach (var Atp in ArticleToProviderSecondFindTask.Result)
                                {
                                    var ProviderId = Atp.ProviderId;
                                    ProvidersCollection Provider = new ProvidersCollection();
                                    dynamic ProviderFindTask = await Task.Run(() => { return Provider.FindById(ProviderId); });
                                    await Task.WhenAll(new Task[] { ProviderFindTask });

                                    if (!list.ContainsKey(Atp.ArticleToProviderId.ToString()))
                                    {

                                        list.Add(Atp.ArticleToProviderId.ToString(), new Dictionary<string, object>{
                                            { "id", Atp.ArticleToProviderId.ToString() },
                                            { "name", ArticleValues.ArticleName },
                                            { "number", ArticleValues.ArticleNumber },
                                            { "brand", ArticleValues.ArticleBrand },
                                            { "quantity", Atp.ArticleToProviderQuantity },
                                            { "prefix", Atp.ArticleToProviderPrefix },
                                            { "provider", ProviderFindTask.Result[0].ProviderName },
                                            { "period", Atp.ArticleToProviderPeriod },
                                            { "orig_price", Atp.ArticleToProviderPrice },
                                            { "currency", Atp.ArticleToProviderCurrency },
                                            { "is_imported", Atp.ArticleToProviderIsImported }
                                        });

                                    }
                                }
                            }
                        }
                    }
                }
            }

            //var subLookup = lookups;
            return list;
        }

        public string getPrice(string Price, string Code)
        {
            string calculatedPrice = "0.0";
            string currencyCode = "UAH";
            string currencyValue = "0.0";
            CurrencyCollection Currency = new CurrencyCollection();
            var CurrencyFindTask = Currency.FindByCurrencyIsDefault().Result;
            foreach (var cur in CurrencyFindTask) { currencyCode = cur.CurrencyCode; currencyValue = cur.CurrencyValue; }
            if (string.IsNullOrEmpty(currencyCode) && Code == currencyCode)
            {
                //calculatedPrice = 
            }

            return calculatedPrice;
        }



        public void _getJsonFromXmlByUrl(string Query)
        {
            string xmlStr;

            using (var wc = new WebClient())
            {
                xmlStr = wc.DownloadString(Query);
            }

            // Encode the XML string in a UTF-8 byte array
            byte[] encodedString = Encoding.UTF8.GetBytes(xmlStr);

            // Put the byte array into a stream and rewind it to the beginning
            MemoryStream ms = new MemoryStream(encodedString);
            ms.Flush();
            ms.Position = 0;
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(ms);

            var doc = XElement.Parse(xmlStr);

            string json = Newtonsoft.Json.JsonConvert.SerializeXmlNode(xmlDoc);

            dynamic _data = System.Web.Helpers.Json.Decode<dynamic>(json);
            //dynamic _data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            this._responseFromXmlByUrl = _data;
        }
    }
}
