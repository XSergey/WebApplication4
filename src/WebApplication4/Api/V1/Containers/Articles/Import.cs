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
        public Import(HttpRequest parameters)
        {
            parameters.Query.TryGetValue("Columns", out _Columns);
            parameters.Query.TryGetValue("Numbers", out _Numbers);
            parameters.Query.TryGetValue("MarkUp", out _MarkUp);
            parameters.Query.TryGetValue("CurrencyId", out _CurrencyId);

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
            if (alias == "prefix")
            {
                var quantity = Columns.FirstOrDefault(x => x.Value == "period").Key;
                //res = field.Value[Int32.Parse(pr)];
                if (quantity.ToLower().Contains('>') || quantity.ToLower().Contains('+')) { res = "MORE"; }
                else if (quantity.ToLower().Contains('<') || quantity.ToLower().Contains('-')) { res = "LESS"; }
                else { res = "EQUAL"; }
            } else
            {
                var col = Columns.FirstOrDefault(x => x.Value == alias).Key;
                res = field.Value[Int32.Parse(col)];
            }

            //res = _field[Int32.Parse(col)];
            if (alias == "article")
            {
                res = Regex.Replace(res, @"[^A-Za-z0-9]+", "");
            }
            else if (alias == "quantity")
            {
                res = Regex.Replace(res, @"/\D+/g", "");
            }
            else if (alias == "period")
            {
                res = Regex.Replace(res, @"/\D+/g", "");
            }
            else if (alias == "provider")
            {
                res = res.ToUpper();
            }

            return res.Trim();
        }

        public string getField(string alias)
        {
            dynamic res = Fields.FirstOrDefault(x => x.Key == alias).Value;
            
            if (alias == "CurrencyId") { res = res.ToUpper(); } else { res = res.ToString(); }
            if (alias == "MarkUp") { res = res.Trim(); }

            return res;
        }
    }
}
