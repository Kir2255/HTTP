using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Gauss;

namespace HttpClientApp
{
    class Program
    {
        public static string url = "http://localhost:8000/";
        public static string nickname;
        public static GaussMethod gaussMethod;
        static async Task Main(string[] args)
        {
            try
            {
                Console.Write(DateTime.Now.ToString() + " - Enter nickname: ");
                nickname = Console.ReadLine();

                HttpClient httpClient = new HttpClient();
                HttpResponseMessage status = await httpClient.GetAsync(url);
                switch (status.StatusCode)
                {
                    case HttpStatusCode.OK:
                        Console.WriteLine(DateTime.Now.ToString() + " - Клиент подключен к серверу по адресу - " + url);
                        break;
                    case HttpStatusCode.Forbidden:
                        Console.WriteLine(DateTime.Now.ToString() + " - У клиента отсутствует доступ к серверу по адресу - " + url);
                        break;
                    case HttpStatusCode.NotFound:
                        Console.WriteLine(DateTime.Now.ToString() + " - Сервер по адресу - " + url + " не найден");
                        break;
                    case HttpStatusCode.ServiceUnavailable:
                        Console.WriteLine(DateTime.Now.ToString() + " - Сервер по адресу - " + url + " недоступен");
                        break;
                    default:
                        break;
                }

               
               
                HttpResponseMessage msg = await httpClient.PostAsync(url, new StringContent(nickname, Encoding.UTF8));
                string result = msg.Content.ReadAsStringAsync().Result;
                Console.WriteLine(DateTime.Now.ToString() + " - " + result);

                byte[] content = await httpClient.GetByteArrayAsync(url);

                BinaryFormatter binaryFormatter = new BinaryFormatter();

                using (MemoryStream stream = new MemoryStream(content))
                {
                    gaussMethod = (GaussMethod)binaryFormatter.Deserialize(stream);
                }

                gaussMethod.SolveMatrix();

                Console.WriteLine(gaussMethod.ToString());

                /*string content = await httpClient.GetStringAsync(url);
                Console.WriteLine(content);*/
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }          

            Console.ReadKey();
        }
    }
}
