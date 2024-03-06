using System.Collections.Concurrent;
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
            private int asyncTasksLimit;
            private TimeSpan tasksWaitingTimeout = TimeSpan.FromSeconds(60);
            private string projectDirectory;
            private string directory;
            private string defaultPath = "\\index.html";
            private BlockingCollection<LogInfo> logs = new();
            private List<Task> requestTasksQueue = new();

            public Server(string ip = "localhost", string port = "8001", int asyncTasksLimit = 10)
            {
                this.ip = ip;
                this.port = port;
                this.asyncTasksLimit = asyncTasksLimit;
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
                StartLogging();
                while (true)
                {
                    if (requestTasksQueue.Count < asyncTasksLimit)
                    {
                        var newRequestTask = listener.GetContextAsync().ContinueWith(task =>
                        {
                            if (task.IsCompletedSuccessfully)
                                ProcessRequest(task.Result);
                        });
                        requestTasksQueue.Add(newRequestTask);
                    }
                    else
                    {
                        Task.WaitAny(requestTasksQueue.ToArray(), tasksWaitingTimeout);
                        requestTasksQueue.RemoveAll(task => task.IsCompleted);
                    }
                }
            }

            private void ProcessRequest(HttpListenerContext ctx)
            {
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
                AddToLogs(ctx);
            }

            private void AddToLogs(HttpListenerContext ctx)
            {
                logs.Add(new LogInfo(DateTime.Now,
                                     ctx.Request.RemoteEndPoint.ToString(),
                                     ctx.Request.Url?.LocalPath ?? "",
                                     ctx.Response.StatusCode));
            }

            private void StartLogging()
            {
                Task.Run(() => Logging());
            }

            private void Logging()
            {
                while(true)
                {
                    LogInfo logInfo = logs.Take();
                    if (logInfo != null)
                    {
                        WriteLog(logInfo);
                    }
                }
            }

            private void WriteLog(LogInfo logInfo)
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = false,
                };
                using var stream = File.Open(projectDirectory + "logs.csv", FileMode.Append);
                using var writer = new StreamWriter(stream);
                using var csv = new CsvWriter(writer, config);
                csv.WriteRecord(logInfo);
                csv.NextRecord();
            }
        }

        static void Main()
        {
            var server = new Server();
            server.Run();
        }
    }
}