using Gauss;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace HttpServerApp
{
    class Program
    {
        class HttpServer
        {
            public static HttpListener listener;
            public static string url = "http://localhost:8000/";
            public static int requestCount = 0;
            public static int clientCount = 2;
            public static uint num = 2;
            public static List<HttpListenerContext> contexts = new List<HttpListenerContext>();
            public static GaussMethod gaussMethod;

            public static void Main(string[] args)
            {
                gaussMethod = new GaussMethod(num, num);
                gaussMethod.RandomInit(gaussMethod.rowCount, gaussMethod.columCount);

                // Create a Http server and start listening for incoming connections
                listener = new HttpListener();
                listener.Prefixes.Add(url);
                listener.Start();
                Console.WriteLine(DateTime.Now.ToString() + " - Listening for connections on {0}", url);

                try
                {
                    Console.Write(DateTime.Now.ToString() + " - Enter client's count: ");
                    clientCount = Int32.Parse(Console.ReadLine());
                }
                catch (FormatException)
                {
                    Console.WriteLine(DateTime.Now.ToString() + " - Input error... Repeat one more time...");
                }

                Console.WriteLine(DateTime.Now.ToString() + " - Waiting for connections");              

                // Handle requests
                Task listenTask = HandleIncomingConnections(clientCount);
                listenTask.GetAwaiter().GetResult();
                listenTask.Wait();

                Task sendMatrixTask = HandleMatrixSend(clientCount);
                sendMatrixTask.GetAwaiter().GetResult();

                // Close the listener
                listener.Close();
                Console.ReadKey();
            }

            public static async Task HandleIncomingConnections(int clientCount)
            {
                bool runServer = true;
                int count = 0;

                // While a user hasn't visited the `shutdown` url, keep on handling requests
                while (runServer)
                {
                    // Will wait here until we hear from a connection
                    HttpListenerContext ctx = await listener.GetContextAsync();

                    // Peel out the requests and response objects
                    HttpListenerRequest req = ctx.Request;
                    HttpListenerResponse resp = ctx.Response;

                    // Print out some info about the request
                    PrintRequestInfo(req);

                    // If `shutdown` url requested w/ POST, then shutdown the server after serving the page
                    //(req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/shutdown")

                    string result = String.Empty;

                    if (req.HttpMethod == "POST")
                    {
                        StreamReader reader = new StreamReader(req.InputStream, req.ContentEncoding);

                        result = reader.ReadToEnd();

                        Console.WriteLine(DateTime.Now.ToString() + " - " + result + "\n");

                    }

                    // Write the response info
                    byte[] data = Encoding.UTF8.GetBytes("Hello, " + result);
                    resp.ContentType = "text/html";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;

                    // Write out to the response stream (asynchronously), then close it
                    await resp.OutputStream.WriteAsync(data, 0, data.Length);

                    contexts.Add(ctx);

                    /*BinaryFormatter binaryFormatter = new BinaryFormatter();

                    using (MemoryStream stream = new MemoryStream())
                    {
                        binaryFormatter.Serialize(stream, gaussMethod);
                        byte[] output = stream.ToArray();
                        resp.ContentType = "text/html";
                        resp.ContentEncoding = Encoding.UTF8;
                        resp.ContentLength64 = output.LongLength;

                        // Write out to the response stream (asynchronously), then close it
                        await resp.OutputStream.FlushAsync();
                        await resp.OutputStream.WriteAsync(output, 0, output.Length);
                        resp.Close();
                    }*/

                    if (++count == clientCount * 2)
                    {
                        Console.WriteLine(DateTime.Now.ToString() + " - Max clients...");
                        runServer = false;
                    }
                }

                /*while (true)
                {
                    HttpListenerContext c = contexts[0];
                    HttpListenerRequest r = c.Request;
                    HttpListenerResponse p = c.Response;

                    PrintRequestInfo(r);

                    byte[] a = Encoding.UTF8.GetBytes("fdsf");
                    p.ContentType = "text/html";
                    p.ContentEncoding = Encoding.UTF8;
                    p.ContentLength64 = a.LongLength;

                    await p.OutputStream.FlushAsync();
                    await p.OutputStream.WriteAsync(a, 0, a.Length);

                }*/

                //GaussMethod gaussMethod = new GaussMethod(num, num);
                //gaussMethod.RandomInit(gaussMethod.rowCount, gaussMethod.columCount);

                //Console.WriteLine(gaussMethod.ToString());

                //Console.WriteLine("\n" + contexts.Count + "\n");



                /*for (int i = 0; i < contexts.Count; i++)
                {
                    HttpListenerRequest req = contexts[i].Request;
                    HttpListenerResponse resp = contexts[i].Response;

                    PrintRequestInfo(req);

                    BinaryFormatter binaryFormatter = new BinaryFormatter();

                    using (MemoryStream stream = new MemoryStream())
                    {
                        binaryFormatter.Serialize(stream, gaussMethod);
                        byte[] data = stream.ToArray();
                        resp.ContentType = "text/html";
                        resp.ContentEncoding = Encoding.UTF8;
                        resp.ContentLength64 = data.LongLength;

                        // Write out to the response stream (asynchronously), then close it
                        await resp.OutputStream.FlushAsync();
                        await resp.OutputStream.WriteAsync(data, 0, data.Length);
                        resp.Close();

                    }
                }*/
            }

            public static async Task HandleMatrixSend(int clientCount)
            {
                bool runServer = true;
                int count = 0;

                // While a user hasn't visited the `shutdown` url, keep on handling requests
                while (runServer)
                {
                    // Will wait here until we hear from a connection
                    HttpListenerContext ctx = await listener.GetContextAsync();

                    // Peel out the requests and response objects
                    HttpListenerRequest req = ctx.Request;
                    HttpListenerResponse resp = ctx.Response;

                    // Print out some info about the request
                    PrintRequestInfo(req);

                    BinaryFormatter binaryFormatter = new BinaryFormatter();

                    using (MemoryStream stream = new MemoryStream())
                    {
                        binaryFormatter.Serialize(stream, gaussMethod);
                        byte[] output = stream.ToArray();
                        resp.ContentType = "text/html";
                        resp.ContentEncoding = Encoding.UTF8;
                        resp.ContentLength64 = output.LongLength;

                        // Write out to the response stream (asynchronously), then close it
                        await resp.OutputStream.FlushAsync();
                        await resp.OutputStream.WriteAsync(output, 0, output.Length);
                        //resp.Close();
                    }

                    // Write out to the response stream (asynchronously), then close it
                    //await resp.OutputStream.WriteAsync(data, 0, data.Length);
                }
            }
            private static void PrintRequestInfo(HttpListenerRequest req)
            {
                Console.WriteLine("Request #: {0}", ++requestCount);
                Console.WriteLine(req.Url.ToString());
                Console.WriteLine(req.HttpMethod);
                Console.WriteLine(req.UserHostName);
                Console.WriteLine(req.UserAgent);
                Console.WriteLine();
            }

        }
    }
}
