using System;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using WebApplication3.Api.V1.Providers.Clients.Ftp.Contracts;

namespace WebApplication3.Api.V1.Providers.Clients.Ftp
{
    public class FtpProvider : FtpProviderBase
    {
        public FtpProvider(string url, HttpRequest parameters) : this(url, parameters, DateTime.Now) { }
        public FtpProvider(string url, HttpRequest parameters, DateTime time)
        {
            Url = url != null ? url : "";
            Parameters = parameters;
            Time = time;
        }
        public new string Url { get; private set; }
        public new HttpRequest Parameters { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public new DateTime Time { get; private set; }
        public async Task<IEnumerable<string>> CreateFtpRequest()
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Url);
            request.Credentials = new NetworkCredential(Username, Password);

            return new string[] { "Success" };
        }
        public void SetNetworkCridentials()
        {
            //Username = Parameters.QueryString["FtpUsername"];
        }
    }
}
