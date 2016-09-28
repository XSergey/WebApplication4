using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace WebApplication4.Api.V1.Providers.Clients.Ftp.Contracts
{
    public class FtpProviderBase
    {
        public string Url { get; set; }
        public HttpRequest Parameters { get; set; }
        public DateTime Time { get; set; }
    }
}
