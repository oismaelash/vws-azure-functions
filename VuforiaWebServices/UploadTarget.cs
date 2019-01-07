using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Net;

using Amazon.Lambda.Core;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
//[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace VuforiaWebServices
{
    class UploadTarget
    {
        private static string endpoint = "https://vws.vuforia.com/targets";

        public static string FunctionHandler(VWSGet input, ILambdaContext context)
        {
            return AddTarget(input);
        }

        public static string FunctionHandler(VWSGet input)
        {
            return AddTarget(input);
        }

        public static string AddTarget(VWSGet vwsGet)
        {
            VWSJson payload = new VWSJson();
            payload.name = vwsGet.name;
            payload.image = vwsGet.image;
            payload.width = vwsGet.width;
            payload.active_flag = vwsGet.active_flag;
            payload.application_metadata = vwsGet.application_metadata;

            string json = JsonConvert.SerializeObject(payload);
            var data = Encoding.ASCII.GetBytes(json);

            var request = (HttpWebRequest)WebRequest.Create(endpoint);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Date = DateTime.Now.ToUniversalTime();
            request.Headers.Set(HttpRequestHeader.Authorization, string.Format("VWS {0}:{1}", vwsGet.accessKey, GetSignatureWithPayload(vwsGet.secretKey, json, request.Method, "/targets")));

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            try
            {
                using (var response = request.GetResponse())
                {
                    var streamData = response.GetResponseStream();
                    StreamReader reader = new StreamReader(streamData);
                    object result = reader.ReadToEnd();
                    streamData.Close();
                    response.Close();
                    return result.ToString();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private static string GetSignatureWithPayload(string secretKey, string payload, string method, string route)
        {
            string date = string.Format("{0:r}", DateTime.Now.ToUniversalTime());

            MD5 md5 = MD5.Create();
            var contentMD5bytes = md5.ComputeHash(Encoding.ASCII.GetBytes(payload));

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < contentMD5bytes.Length; i++)
            {
                sb.Append(contentMD5bytes[i].ToString("x2"));
            }

            string contentMD5 = sb.ToString();
            string stringToSign = string.Format("{0}\n{1}\n{2}\n{3}\n{4}", method, contentMD5, "application/json", date, route);

            HMACSHA1 sha1 = new HMACSHA1(Encoding.ASCII.GetBytes(secretKey));
            byte[] sha1Bytes = Encoding.ASCII.GetBytes(stringToSign);
            MemoryStream stream = new MemoryStream(sha1Bytes);
            byte[] sha1Hash = sha1.ComputeHash(stream);
            string signature = Convert.ToBase64String(sha1Hash);

            return signature;
        }

        // Method Utils

        public static string DownloadTarget(string address)
        {
            using (WebClient client = new WebClient())
            {
                byte[] imageBytes = client.DownloadData(address);
                return Convert.ToBase64String(imageBytes);
            }
        }

        // Class Utils

        //[Serializable]
        //public class VWSGet
        //{
        //    public string acessKey;
        //    public string secretKey;
        //    public string name;
        //    public float width;
        //    public string image;
        //    public int active_flag;
        //    public string application_metadata;
        //}

        //[Serializable]
        //public class VWSJson
        //{
        //    public string name;
        //    public float width;
        //    public string image;
        //    public int active_flag;
        //    public string application_metadata;
        //}
    }
}