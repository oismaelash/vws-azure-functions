using System;
using System.Text;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace VuforiaWebServices
{
    class Program
    {
        private static string endpoint = "https://vws.vuforia.com/targets";

        static void Main(string[] args)
        {
            VWSGet input = new VWSGet();
            input.accessKey = "4b8ce36e57a8dac4d769d3268ac6b3c0ee5c4975";
            input.secretKey = "ecf230be08c31515dfa24cfeab0ad8857134a2ed";
            input.name = "TestUpdate";
            input.width = 1;
            input.active_flag = 1;
            input.application_metadata = Convert.ToBase64String(Encoding.ASCII.GetBytes("Update"));
            input.image = "";

            //Console.WriteLine("Write url target(image) and press enter:");
            //string urlImage = Console.ReadLine();
            //input.image = Utils.DownloadTarget(urlImage);

            Console.WriteLine("Start process...");
            //Console.WriteLine(UploadTarget.AddTarget(input));
            Console.WriteLine("Write id target and press enter:");
            string idTarget = Console.ReadLine();
            input.idTarget = idTarget;
            Console.WriteLine(UpdateTarget.UpdateNow(input));

            Console.WriteLine("...Finish process");
        }
    }
}