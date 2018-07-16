using System;
using System.IO;

namespace SettingsHelper
{
    public static class CALogger
    {
        static string LogFileName;
        public static void InitLogFile(string logFile)
        {
            LogFileName = logFile;
        }

        public static async void WriteToLogFile (string msg)
        {
            string logFilePath = (LogFileName == null) ? 
                (Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"log.txt") 
                : LogFileName;
            try
            {
                using (StreamWriter sw = new StreamWriter(new FileStream(logFilePath,
                    FileMode.Append, FileAccess.Write, FileShare.ReadWrite)))
                {
                    await sw.WriteLineAsync(String.Format("{0}:{1} -- {2}", 
                        DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString(), msg));
                }
            }
            catch (IOException)
            {
                if (!File.Exists(logFilePath))
                {
                    File.Create(logFilePath);
                }
            }
        }
    }
}