using System.Text;

namespace Web.Models
{
    public static class ApiLoggerExtensions
    {
        private static IWebHostEnvironment webHostEnvironment;
        public static void Initialize(IWebHostEnvironment env)
        {
            webHostEnvironment = env;
        }
        public static void GenerateLogSuccess(HttpResponseMessage Response, TimeSpan ts)
        {
            StringBuilder responseLog = new StringBuilder();
            try
            {
                responseLog.Append($" Method Type: {Response.RequestMessage.Method}");
                responseLog.Append($" Request URL: {Response.RequestMessage.RequestUri}");
                responseLog.Append($" API execution time: {ts.Seconds} sec");
                responseLog.Append($" Status Code: {(int)Response.StatusCode}");
                responseLog.ToString().Log();
            }
            catch (HttpRequestException ex)
            {
            }
        }
        public static void GenerateLogError(string Type,string URL, TimeSpan ts, string Message)
        {
            StringBuilder responseLog = new StringBuilder();
            try
            {
                responseLog.Append($" Method Type: {Type}");
                responseLog.Append($" Request URL: {URL}");
                responseLog.Append($" API execution time: {ts.Seconds} sec");
                responseLog.Append($" Error: {Message}");
                responseLog.ToString().Log();
            }
            catch (HttpRequestException ex)
            {
            }
        }
        public static void GenerateLogSuccessPost(HttpResponseMessage Response, TimeSpan ts,string URL)
        {
            StringBuilder responseLog = new StringBuilder();
            try
            {
                responseLog.Append($" Method Type: {Response.RequestMessage.Method}");
                responseLog.Append($" Request URL: {URL}");
                responseLog.Append($" API execution time: {ts.Seconds} sec");
                responseLog.Append($" Status Code: {(int)Response.StatusCode}");
                responseLog.ToString().Log();
            }
            catch (HttpRequestException ex)
            {
            }
        }
        public static void Log(this string message)
        {
            if (webHostEnvironment == null)
            {
                throw new InvalidOperationException("WebHostEnvironment has not been initialized.");
            }
            string path = Path.Combine(webHostEnvironment.WebRootPath, "ApiLogFiles");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            VerifyDir(path);
            string fileName = DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + "_ApiLogs.txt";
            try
            {
                using (StreamWriter file = new StreamWriter(Path.Combine(path, fileName), true))
                {
                    file.WriteLine(DateTime.Now.ToString() + ": " + message);
                }
            }
            catch (Exception) { }
        }

        private static void VerifyDir(string path)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                if (!dir.Exists)
                {
                    dir.Create();
                }
            }
            catch { }
        }
    }

}
