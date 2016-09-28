using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

using WebApplication4.Api.V1.Collections;
using WebApplication4.Api.V1.Collections.Articles;
using WebApplication4.Api.V1.Providers.Clients.Ftp;
using WebApplication4.Api.V1.Providers.Readers.Excel;
using WebApplication4.Api.V1.Containers.Articles;

namespace WebApplication3.Api.V1.Controllers
{
    [Route("api/[controller]")]
    public class ArticlesController : Controller
    {
        // GET: api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {
                // Prepre task running
                //var cts = new System.Threading.CancellationTokenSource();
                //cts.CancelAfter(999999999);
                //await Task.Delay(999999999, cts.Token);

                

            Task.Run(async () => {
                // 1. Ftp Provider
                FtpProvider Ftp = new FtpProvider("", Request) {/*Url="", Parameters = Request*/};
                await Ftp.SetNetworkCridentials();

                dynamic ftpTask = Task.Run(() => {
                    return Ftp.DownloadFileAsync("filename.xlsx", "/").Result;
                });
                await Task.WhenAll(new Task[] { ftpTask });
                string filePath = ftpTask.Result;

                // 2. Excel Reader Provider
                ExcelReader excelReader = new ExcelReader(filePath);

                dynamic tableTask = Task.Run(() => { return excelReader.ReadExcel(); });
                await Task.WhenAll(new Task[] { tableTask });

                foreach (var row in tableTask.Result)
                {
                    dynamic abc = row;
                    Import import = new Import(Request);
                    dynamic prepareAttributes = Task.Run(() => { return import.PrepareAttributes(); });
                    await Task.WhenAll(new Task[] { prepareAttributes });

                    // Article creation
                    ArticlesCollection Article = new ArticlesCollection();
                    dynamic ArticleFindTask = await Task.Run(() => { return Article.Find(import.getColumn("article", row), import.getColumn("brand", row)); });
                    await Task.WhenAll(new Task[] { ArticleFindTask });

                    if (ArticleFindTask.Result.Count == 0)
                    {
                        dynamic ArticleCreateTask = await Task.Run(() => { return Article.Create(import.getColumn("article", row), import.getColumn("brand", row)); });
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
                    //var bc = pr[0].ProviderName;
                    ArticlesToProvidersCollection ArticleToProvider = new ArticlesToProvidersCollection();
                    dynamic ArticleToProviderFindTask = await Task.Run(() => { return ArticleToProvider.Find(ar[0].ArticleId, pr[0].ProviderId); });
                    await Task.WhenAll(new Task[] { ProviderFindTask });
                    
                    if (ArticleToProviderFindTask.Result.Count == 0)
                    {
                        //var cur = import.getField("CurrencyId");
                        //var per = Int32.Parse(import.getColumn("period", row));
                        //var pre = import.getColumn("prefix", row);
                        //var q = Int32.Parse(import.getColumn("quantity", row));
                        //var prc = float.Parse(import.getColumn("price", row));
                        dynamic ArticleToProviderCreateTask = await Task.Run(() => { return ArticleToProvider.Create(aid, pid/*, float.Parse(import.getColumn("price", row)), Int32.Parse(import.getColumn("quantity", row)), import.getField("CurrencyId"), Int32.Parse(import.getColumn("period", row)), import.getColumn("prefix", row)*/); });
                        await Task.WhenAll(new Task[] { ArticleToProviderCreateTask });
                        //ArticleToProviderFindTask = await Task.Run(() => { return ArticleToProvider.Find(import.getColumn("provider", row)); });
                        //await Task.WhenAll(new Task[] { ArticleToProviderFindTask });
                    }
                }
                //ArticlesCollection _articles = new ArticlesCollection();
                //var Articles = await _articles.Find("");
                //var Articles = await _articles.FindAll();
                /*foreach (var article in Articles)
                {
                    var abc = article;
                }

                return Articles;*/
            });
            string[] s = { "123",  "123"};
            return new ObjectResult(s); 
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
