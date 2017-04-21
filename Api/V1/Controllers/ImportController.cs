using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using MongoDB.Driver;
using WebApplication4.Api.V1.Collections;
using WebApplication4.Api.V1.Collections.Articles;
using WebApplication4.Api.V1.Providers.Clients.Ftp;
using WebApplication4.Api.V1.Providers.Readers.Excel;
using WebApplication4.Api.V1.Containers.Articles;
using MongoDB.Bson;

namespace WebApplication3.Api.V1.Controllers
{
    [Route("api/[controller]")]
    public class ImportController : Controller
    {
        // GET: api/import
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            //var progressHandler = new Progress<int>() as IProgress<int>;
            int count = 0;
            // 1. Ftp Provider
            FtpProvider Ftp = new FtpProvider("", Request) {/*Url="", Parameters = Request*/};
            await Ftp.SetNetworkCridentials();

            dynamic ftpTask = Task.Run(() => {
                return Ftp.DownloadFileAsync("filename.xlsx", "/");
            });
            await Task.WhenAll(new Task[] { ftpTask });
            string filePath = ftpTask.Result;
            //count = tableTask.Result;

            // 2. Excel Reader Provider
            ExcelReader excelReader = new ExcelReader(filePath);

            dynamic tableTask = Task.Run(() => { return excelReader.ReadExcel(); });
            await Task.WhenAll(new Task[] { tableTask });

            count = tableTask.Result.Count;

            Task.Run(async () => {

                foreach (var row in tableTask.Result)
                {
                    //MongoClient Client = new MongoClient("mongodb://138.201.142.157:27017");
                    //if the database is not exist, creates the database
                    //IMongoDatabase Db = Client.GetDatabase("uniparts");

                    //var asdzxc = await Db.GetCollection<Article>("uniparts_articles").Find(new BsonDocument()).ToListAsync();

                    Import import = new Import(Request);
                    dynamic prepareAttributes = Task.Run(() => { return import.PrepareAttributes(); });
                    await Task.WhenAll(new Task[] { prepareAttributes });

                    var providerName = import.getColumn("provider", row);
                    // Remove articles by provider
                    ProvidersCollection ProviderFirst = new ProvidersCollection();
                    dynamic ProviderFindForDestroyTask = await Task.Run(() => { return ProviderFirst.Find(providerName); });
                    await Task.WhenAll(new Task[] { ProviderFindForDestroyTask });
                    if (ProviderFindForDestroyTask.Result.Count > 0)
                    {
                        // Article to provider collection
                        var pr = ProviderFindForDestroyTask.Result;
                        var pid = pr[0].ProviderId;

                        ArticlesToProvidersCollection ArticleToProvider = new ArticlesToProvidersCollection();
                        dynamic ArticleToProviderFindTask = await Task.Run(() => { return ArticleToProvider.FindByProviderId(pid); });
                        await Task.WhenAll(new Task[] { ArticleToProviderFindTask });

                        if (ArticleToProviderFindTask.Result.Count > 0)
                        {
                            foreach (var ArtToProv in ArticleToProviderFindTask.Result)
                            {
                                //ArticlesCollection Article = new ArticlesCollection();
                                //dynamic ArticleFindTask = await Task.Run(() => { return Article.FindById(ArtToProv.ArticleId); });
                                //await Task.WhenAll(new Task[] { ArticleFindTask });

                                //if (ArticleFindTask.Result.Count > 0)
                                //{
                                //Destroy articles
                                //ArticlesCollection Article = new ArticlesCollection();
                                //dynamic ArticleDestroyTask = await Task.Run(() => { return Article.Destroy(ArtToProv.ArticleId); });
                                //await Task.WhenAll(new Task[] { ArticleDestroyTask });
                                //}
                                dynamic ArticleToProviderDestroyTask = await Task.Run(() => { return ArticleToProvider.Destroy(ArtToProv.ArticleToProviderId); });
                                await Task.WhenAll(new Task[] { ArticleToProviderDestroyTask });
                            }
                            
                        }

                        //dynamic ProviderDestroyTask = await Task.Run(() => { return ProviderFirst.Destroy(pid);});
                        //await Task.WhenAll(new Task[] { ProviderDestroyTask });
                    }

                    //ArticlesCollection Article = new ArticlesCollection();
                    //ProvidersCollection Provider = new ProvidersCollection();
                    //ArticlesToProvidersCollection ArticleToProvider = new ArticlesToProvidersCollection();

                    //var article_q = import.getColumn("quantity", row);

                    var quantity = Int32.Parse(import.getColumn("quantity", row));
                    //var article_num = import.getColumn("article", row);
                    //var article_brand = import.getColumn("brand", row);
                    
                    if (quantity > 0)
                    {
                        // Article creation
                        ArticlesCollection Article = new ArticlesCollection();
                        dynamic ArticleFindTask = await Task.Run(() => { return Article.Find(import.getColumn("article", row), import.getColumn("brand", row)); });
                        await Task.WhenAll(new Task[] { ArticleFindTask });

                        if (ArticleFindTask.Result.Count == 0)
                        {
                            dynamic ArticleCreateTask = await Task.Run(() => { return Article.Create(import.getColumn("article", row), import.getColumn("desc", row), import.getColumn("brand", row)); });
                            await Task.WhenAll(new Task[] { ArticleCreateTask });
                            ArticleFindTask = await Task.Run(() => { return Article.Find(import.getColumn("article", row), import.getColumn("brand", row)); });
                            await Task.WhenAll(new Task[] { ArticleFindTask });
                        }

                        // Provider creation
                        ProvidersCollection Provider = new ProvidersCollection();
                        dynamic ProviderFindTask = await Task.Run(() => { return Provider.Find(import.getColumn("provider", row)); });
                        await Task.WhenAll(new Task[] { ProviderFindTask });

                        if (ProviderFindTask.Result.Count == 0)
                        {
                            dynamic ProviderCreateTask = await Task.Run(() => { return Provider.Create(import.getColumn("provider", row), import.getField("MarkUp")); });
                            await Task.WhenAll(new Task[] { ProviderCreateTask });
                            ProviderFindTask = await Task.Run(() => { return Provider.Find(import.getColumn("provider", row)); });
                            await Task.WhenAll(new Task[] { ProviderFindTask });
                        }

                        // Article to provider collection
                        var ar = ArticleFindTask.Result;
                        var pr = ProviderFindTask.Result;
                        var aid = ar[0].ArticleId;
                        var pid = pr[0].ProviderId;

                        ArticlesToProvidersCollection ArticleToProvider = new ArticlesToProvidersCollection();
                        dynamic ArticleToProviderFindTask = await Task.Run(() => { return ArticleToProvider.Find(aid, pid); });
                        await Task.WhenAll(new Task[] { ArticleToProviderFindTask });
                    
                        if (ArticleToProviderFindTask.Result.Count == 0)
                        {
                            dynamic ArticleToProviderCreateTask = await Task.Run (() => {
                                var sas = import.getColumn("price", row);
                                var zaz = import.getColumn("period", row);
                                string currency = import.getField("CurrencyId").ToString();

                                int period = Int32.Parse(import.getColumn("period", row));

                                string prefix = import.getColumn("prefix", row).ToString();
                                
                                double price = Math.Round(float.Parse(import.getColumn("price", row)), 4, MidpointRounding.AwayFromZero);

                                return ArticleToProvider.Create(aid, pid, price, quantity, currency, period, prefix);
                            });
                            await Task.WhenAll(new Task[] { ArticleToProviderCreateTask });
                            //ArticleToProviderFindTask = await Task.Run(() => { return ArticleToProvider.Find(import.getColumn("provider", row)); });
                            //await Task.WhenAll(new Task[] { ArticleToProviderFindTask });
                        } else
                        {
                            dynamic ArticleToProviderUpdateTask = await Task.Run(() => {
                                //var sas = import.getColumn("price", row);
                                string currency = import.getField("CurrencyId").ToString();

                                int period = Int32.Parse(import.getColumn("period", row));

                                string prefix = import.getColumn("prefix", row).ToString();

                                double price = Math.Round(float.Parse(import.getColumn("price", row)), 4, MidpointRounding.AwayFromZero);

                                return ArticleToProvider.Update(ArticleToProviderFindTask.Result[0].ArticleToProviderId, price, quantity, currency, period, prefix);
                            });
                            await Task.WhenAll(new Task[] { ArticleToProviderUpdateTask });
                        }
                    }
                    //if (progressHandler != null)
                        //progressHandler.Report(count++);
                }
            });
            

            string[] s = { "Success", count.ToString() };
            return new ObjectResult(s); 
        }
    }
}
