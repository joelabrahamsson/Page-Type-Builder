namespace PageTypeBuilder
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Web.Hosting;
    using Configuration;

    public class TimingsLogger : IDisposable
    {
        private readonly string _message;
        private readonly Stopwatch _stopwatch;
        private readonly DirectoryInfo _logDirectory;
        private static string _siteId;

        public TimingsLogger(string message)
        {
            _message = message;
            _logDirectory = GetLogDirectory();

            if (_logDirectory != null && _logDirectory.Exists)
                _stopwatch = Stopwatch.StartNew();
        }

        public static void Clear()
        {
            DirectoryInfo logDirectory = GetLogDirectory();

            if (logDirectory == null || !logDirectory.Exists)
                return;

            string siteId = GetSiteId();
            string logFileName = GetLogFileName(logDirectory, siteId);

            if (!File.Exists(logFileName)) 
                return;

            try
            {
                File.Delete(logFileName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Error while trying to delete file '{0}'. {1}", logFileName, ex.Message));
            }
        }

        private static DirectoryInfo GetLogDirectory()
        {
            string directoryPath = PageTypeBuilderConfiguration.GetConfiguration().TimingsLogFileDirectoryPath;

            if (string.IsNullOrEmpty(directoryPath))
                return null;

            if (!directoryPath.EndsWith(@"\"))
                directoryPath += @"\";

            DirectoryInfo logDirectory = new DirectoryInfo(directoryPath);
            return !logDirectory.Exists ? null : logDirectory;
        }

        private static string GetSiteId()
        {
            try
            {
                return string.Format("{0}_{1}", HostingEnvironment.ApplicationHost.GetSiteName(), HostingEnvironment.ApplicationHost.GetSiteID());
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string GetLogFileName(DirectoryInfo logDirectory, string siteId)
        {
            return string.Format("{0}PageTypeBuilderTimingsLog_{1}.txt", logDirectory.FullName, siteId);
        }

        public void Dispose()
        {
            if (_logDirectory == null || !_logDirectory.Exists)
                return;

            if (_siteId == null)
                _siteId = GetSiteId();

            try
            {
                string message = string.Format("{0} - {1}: {2}{3}{3}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), _message, _stopwatch.ElapsedMilliseconds, Environment.NewLine);
                string logFileName = GetLogFileName(_logDirectory, _siteId);
                File.AppendAllText(logFileName, message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error while trying to log messages using PageTypeBuilder.TimingsLogger. " + ex.Message);
            }
            
        }
    }
}
