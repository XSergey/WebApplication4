using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net.FtpClient.Async;

using WebApplication4.Api.V1.Providers.Clients.Ftp.Contracts;
using WebApplication4.Api.V1.Providers.Readers.Excel;
using Microsoft.Extensions.Primitives;
using System.Net.FtpClient;
using Microsoft.AspNet.Hosting;

namespace WebApplication4.Api.V1.Providers.Clients.Ftp
{
    public class FtpProvider : FtpProviderBase
    {
        //private StringValues _Url;
        private StringValues _Username;
        private StringValues _Password;
        private StringValues _Filename;
        private StringValues _FtpPath;
        private StringValues _FtpHost;

        public FtpProvider(string url, HttpRequest parameters) : this(url, parameters, DateTime.Now) { }
        public FtpProvider(string url, HttpRequest parameters, DateTime time)
        {
            //parameters.Query.TryGetValue("FtpUrl", out _Url);
            parameters.Query.TryGetValue("FtpUsername", out _Username);
            parameters.Query.TryGetValue("FtpPassword", out _Password);
            parameters.Query.TryGetValue("FtpFile", out _Filename);
            parameters.Query.TryGetValue("FtpPath", out _FtpPath);
            parameters.Query.TryGetValue("FtpHost", out _FtpHost);
            //Url = _Url;
            Host = _FtpHost;
            
            IHostingEnvironment _hostingEnvironment = new HostingEnvironment();
            if (string.IsNullOrWhiteSpace(_hostingEnvironment.WebRootPath))
            {
                _hostingEnvironment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }
            //var bc = _hostingEnvironment.WebRootPath;
            Storage = _hostingEnvironment.WebRootPath + "\\Storage\\Uploads\\";
            //var asd = Path.Combine(_hostingEnvironment.WebRootPath, "/wwwroot/Storage/Uploads/");
            //Storage = Path.Combine(_hostingEnvironment.WebRootPath, "/wwwroot/Storage/Uploads/");
        }
        private FtpClient CreateFtpClient()
        {
            FtpClient ftpClient = new FtpClient();
            ftpClient.Host = Host;
            ftpClient.Credentials = new NetworkCredential(Username, Password);
            return ftpClient;
        }
        public new string Url { get; set; }
        public string Host { get; set; }
        public string Filename { get; set; }
        public string FtpPath { get; set; }
        public string Storage { get; set; }
        public new HttpRequest Parameters { get; set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public new DateTime Time { get; private set; }
        public async Task<bool> SetNetworkCridentials()
        {
            Username = _Username;
            Password = _Password;
            //Parameters = parameters;
            Filename = _Filename;
            FtpPath = _FtpPath;
            return true;/*Task.Run(() => { return true; }).Result; */
        }
        public Task<string> DownloadFileAsync(string fileName, string workingDirectory)
        {
            return _DownloadFileAsync(Filename, FtpPath);
        }

        public async Task<string> _DownloadFileAsync(string fileName, string workingDirectory)
        {
            string fileToWrite = Storage + fileName;
            await DownloadFileAsync1(fileName, workingDirectory, fileToWrite).ConfigureAwait(false);
            return fileToWrite;
        }

        public async Task DownloadFileAsync1(string fileName, string workingDirectory, string filePathToWrite)
        {
            using (FtpClient ftpClient = CreateFtpClient())
            {
                await Task.Factory.FromAsync(ftpClient.BeginConnect, ftpClient.EndConnect, state: null).ConfigureAwait(false);
                bool doesDirectoryExist = await Task<bool>.Factory.FromAsync<string>(ftpClient.BeginDirectoryExists, ftpClient.EndDirectoryExists, workingDirectory, state: null).ConfigureAwait(false);
                if (doesDirectoryExist == true)
                {
                    await Task.Factory.FromAsync<string>(ftpClient.BeginSetWorkingDirectory, ftpClient.EndSetWorkingDirectory, workingDirectory, state: null).ConfigureAwait(false);
                    bool doesFileExist = await Task<bool>.Factory.FromAsync<string>(ftpClient.BeginFileExists, ftpClient.EndFileExists, fileName, state: null).ConfigureAwait(false);
                    if (doesFileExist == true)
                    {
                        using (Stream streamToRead = await Task<Stream>.Factory.FromAsync<string>(ftpClient.BeginOpenRead, ftpClient.EndOpenRead, fileName, state: null).ConfigureAwait(false))
                        using (Stream streamToWrite = File.Open(filePathToWrite, FileMode.Create))
                        {
                            await streamToRead.CopyToAsync(streamToWrite).ConfigureAwait(false);
                        }
                    }
                }
            }
        }
    }
}
