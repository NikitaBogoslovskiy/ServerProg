using System.Globalization;
using System.Net;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using MimeTypes;

namespace Lab01
{
    static class Program
    {
        class Server
        {
            class LogInfo
            {
                public DateTime Date { get; }
                public string IP { get; }
                public string Path { get; }
                public int ResponseCode { get; }

                public LogInfo(DateTime date, string ip, string path, int responseCode)
                {
                    Date = date;
                    IP = ip;
                    Path = path;
                    ResponseCode = responseCode;
                }
            }

            private HttpListener listener = new HttpListener();
            private string ip;
            private string port;
            private string projectDirectory;
            private string directory;
            private string defaultPath = "\\index.html";
            private List<LogInfo> logs = new List<LogInfo>();

            public Server(string ip = "localhost", string port = "8001")
            {
                this.ip = ip;
                this.port = port;
                listener.Prefixes.Add($"http://{this.ip}:{this.port}/");
                projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\"));
                directory = projectDirectory + "files";

                if (!File.Exists(projectDirectory + "logs.csv"))
                {
                    using var writer = new StreamWriter(projectDirectory + "logs.csv");
                    using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
                    csv.WriteRecords(logs);
                }
            }

            public void Run()
            {
                listener.Start();
                while (true)
                {
                    HttpListenerContext ctx = listener.GetContext();
                    HttpListenerRequest req = ctx.Request;
                    var fullPath = directory;
                    fullPath += req.Url == null || req.Url.LocalPath == "/" ? defaultPath : req.Url?.LocalPath.Replace("/", "\\");

                    HttpListenerResponse resp = ctx.Response;
                    byte[] buf;
                    if (!File.Exists(fullPath))
                    {
                        resp.StatusCode = (int)HttpStatusCode.NotFound;
                        resp.StatusDescription = "File not found";
                        string message = $"Could not find file by path:\n{req.Url.LocalPath}";
                        buf = Encoding.UTF8.GetBytes(message);
                    }
                    else
                    {
                        resp.StatusCode = (int)HttpStatusCode.OK;
                        resp.StatusDescription = "Status OK";
                        string contentType = MimeTypeMap.GetMimeType(Path.GetExtension(fullPath)[1..]);
                        resp.Headers.Set("Content-Type", contentType);
                        buf = File.ReadAllBytes(fullPath);
                    }
                    resp.ContentLength64 = buf.Length;
                    Stream ros = resp.OutputStream;
                    ros.Write(buf, 0, buf.Length);
                    Log(ctx);
                }
            }

            private void Log(HttpListenerContext ctx)
            {
                logs.Add(new LogInfo(DateTime.Now, 
                                     ctx.Request.RemoteEndPoint.ToString(), 
                                     ctx.Request.Url?.LocalPath ?? "", 
                                     ctx.Response.StatusCode));

                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = false,
                };
                using var stream = File.Open(projectDirectory + "logs.csv", FileMode.Append);
                using var writer = new StreamWriter(stream);
                using var csv = new CsvWriter(writer, config);
                csv.WriteRecords(logs);
                logs.Clear();
            }
        }

        static void Main()
        {
            var server = new Server();
            server.Run();
        }
    }
}