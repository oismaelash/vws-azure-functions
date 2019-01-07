using System;
using System.Net;

namespace VuforiaWebServices
{
    public static class Utils
    {
        public static string DownloadTarget(string address)
        {
            using (WebClient client = new WebClient())
            {
                byte[] imageBytes = client.DownloadData(address);
                return Convert.ToBase64String(imageBytes);
            }
        }
    }
}