using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Text.RegularExpressions;
using WebApplication4.Api.V1.Collections;
using WebApplication4.Api.V1.Collections.Articles;

namespace WebApplication4.Api.V1.Containers.Articles
{
    public class Import
    {
        protected Dictionary<string, string> Columns;
        protected Dictionary<string, string> Fields;
        private StringValues _Columns;
        private StringValues _Numbers;
        private StringValues _MarkUp;
        private StringValues _CurrencyId;
        private StringValues _isFormatted;
        private StringValues _ProviderName;
        private StringValues _ProviderPeriod;
        public Import(HttpRequest parameters)
        {
            parameters.Query.TryGetValue("Columns", out _Columns);
            parameters.Query.TryGetValue("Numbers", out _Numbers);
            parameters.Query.TryGetValue("MarkUp", out _MarkUp);
            parameters.Query.TryGetValue("CurrencyId", out _CurrencyId);
            parameters.Query.TryGetValue("isFormatted", out _isFormatted);
            parameters.Query.TryGetValue("ProviderName", out _ProviderName);
            parameters.Query.TryGetValue("ProviderPeriod", out _ProviderPeriod);

        }
        public async Task<List<string>> ParseRow(dynamic row)
        {
            var row1 = row;
            int x = 0;
            List<string> arts = new List<string>(); 
            foreach (var field in row)
            {
                //abc = await GetDataFromFields(field);

                    ArticlesCollection Article = new ArticlesCollection();
                    arts = await Article.Find(getColumn("article", field), getColumn("brand", field));
                    //var asd = ArticlesByFind.Result;

                    x++;
            }
            return await Task.Run(() => { return arts; });
        }
        public async Task GetDataFromFields(dynamic field)
        {
            ArticlesCollection Article = new ArticlesCollection();
            dynamic ArticlesByFind = await Article.Find(getColumn("article", field), getColumn("brand", field));
            var asd = ArticlesByFind.Result;
            
            foreach(var article in ArticlesByFind)
            {

            }
        }
        public async Task PrepareAttributes()
        {
            Dictionary<string, string> _Fields = new Dictionary<string, string>();
            _Fields.Add("MarkUp", _MarkUp);
            _Fields.Add("CurrencyId", _CurrencyId);
            _Fields.Add("isFormatted", _isFormatted);
            _Fields.Add("ProviderName", _ProviderName);
            _Fields.Add("ProviderPeriod", _ProviderPeriod);

            Fields = _Fields;

            string[] _columns = _Columns.ToString().Split(',');
            string[] _numbers = _Numbers.ToString().Split(',');

            Dictionary<string, string> _ColumnsN = new Dictionary<string, string>();
            for (int colNum = 0; colNum < _columns.Count(); colNum++)
            {
                int number_key = 0;

                for (int numbNum = 0; numbNum < _numbers.Count(); numbNum++)
                {
                    if (colNum == numbNum)
                    {
                        number_key = numbNum;
                    }
                }

                var num = _numbers[number_key];
                var col = _columns[colNum];
                _ColumnsN.Add(num, col);
            }
            Columns = _ColumnsN;
            //return Task.Run(()=> { });
        }
        public string getColumn(string alias, dynamic field)
        {
            string res = "";

            // Remove Quotes from string
            res = res.Replace("\"", "");
            res = res.Replace("'", "");


            if (alias == "prefix")
            {
                var quantity = Columns.FirstOrDefault(x => x.Value == "quantity").Key;

                if (quantity.ToLower().Contains('>') || quantity.ToLower().Contains('+')) { res = "MORE"; }
                else if (quantity.ToLower().Contains('<') || quantity.ToLower().Contains('-')) { res = "LESS"; }
                else { res = "EQUAL"; }
            } else
            {
                var col = Columns.FirstOrDefault(x => x.Value == alias).Key;
                var f = field.Value;
                var fv = field.Key;

                // Some columns for field may be empty.
                if (!string.IsNullOrWhiteSpace(col))
                {
                    if(field.Value.ContainsKey(Int32.Parse(col))) {
                        res = field.Value[Int32.Parse(col)];
                    }
                } 
            }


            if (alias == "article")
            {
                if(getField("isFormatted") == "1") {
                    // Remove first word from string
                    string word = res.Split(' ').FirstOrDefault();
                    res = res.Replace(word, "");
                    res = res.Trim();
                }
                res = Regex.Replace(res, @"[^A-Za-z0-9]+", "");
            }
            else if (alias == "quantity")
            {
                if (string.IsNullOrEmpty(res))
                {
                    res = "0";
                }
                else
                {
                    res = Regex.Replace(res, @"[^\d]", "").ToString();
                }
            }
            else if (alias == "period")
            {
                if (string.IsNullOrWhiteSpace(res)) {
                    res = "0";
                }

                if (!string.IsNullOrWhiteSpace(getField("ProviderPeriod")))
                {
                    res = getField("ProviderPeriod");
                }

                res = Regex.Replace(res, @"[^\d]", "");
            }
            else if (alias == "provider")
            {
                if(!string.IsNullOrWhiteSpace(getField("ProviderName"))) {
                    res = getField("ProviderName");
                }
                res = res.ToUpper();
            }
            else if (alias == "brand")
            {
                res = res.ToUpper();
            }
            else if (alias == "price")
            {
                res = res.ToString();
                if (string.IsNullOrWhiteSpace(res)) {
                    res = "0,0";
                }
                    

                res = res.Replace(".", ",");
            }
            else if(alias == "name")
            {
                if (string.IsNullOrWhiteSpace(res))
                {
                    res = "Автозапчасть";
                }
                // Remove Quotes from string
                res = res.Replace("\"", "");
                res = res.Replace("'", "");
                res = res.Replace("\"", "");
            }

            return res.Trim();
        }

        public string getField(string alias)
        {
            dynamic res = Fields.FirstOrDefault(x => x.Key == alias).Value;
            
            if (alias == "CurrencyId") { res = res.ToUpper(); } else { res = res.ToString(); }
            if (alias == "MarkUp") { res = res.Trim(); }
            if (alias == "isFormatted") { res = res.Trim(); }
            if (alias == "ProviderName") { res = res.Trim(); }
            if (alias == "ProviderPeriod") { res = res.Trim(); }

            return res;
        }
    }
}
