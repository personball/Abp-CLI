using System;
using System.Net;

namespace AbpTools.Utils
{
    public class DownLoadHelper
    {
        public static void DownLoadZipFile(string zip_url, string filePath)
        {
            using (var webClient = new WebClient())
            {
                webClient.Headers.Add("Accept-Language", " en-US");
                webClient.Headers.Add("Accept", " text/html, application/xhtml+xml, */*");
                webClient.Headers.Add("User-Agent", Consts.UserAgent);

                Console.WriteLine($"Start download zip file:{zip_url}");
                Console.WriteLine($"Downloading...");

                try
                {
                    webClient.DownloadFile(zip_url, filePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }

                Console.WriteLine($"Download success and save as {filePath}");
            }
        }
    }
}
