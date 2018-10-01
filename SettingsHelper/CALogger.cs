using System;
using System.Windows;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;   

namespace SettingsHelper
{
    public static class CALogger
    {
        static string LogFileName;
        public static void InitLogFile(string logFile)
        {
            LogFileName = logFile;
        }

        public static void ShowLogFile()
        {
            if (LogFileName!=null || LogFileName != String.Empty)
            {
                try
                {
                    string path = AssocQueryString(AssocStr.Executable, ".txt");
                    if (path != null && path != string.Empty)
                    {
                        Process.Start(path, LogFileName);
                    }
                    else MessageBox.Show("Не удается отобразить файл журнала", "Ошибка!",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch { }
            }
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

        [DllImport("Shlwapi.dll", CharSet = CharSet.Unicode)]
        public static extern uint AssocQueryString(
            AssocF flags,
            AssocStr str,
            string pszAssoc,
            string pszExtra,
            [Out] StringBuilder pszOut,
            ref uint pcchOut
        );

        [Flags]
        public enum AssocF
        {
            None = 0,
            Init_NoRemapCLSID = 0x1,
            Init_ByExeName = 0x2,
            Open_ByExeName = 0x2,
            Init_DefaultToStar = 0x4,
            Init_DefaultToFolder = 0x8,
            NoUserSettings = 0x10,
            NoTruncate = 0x20,
            Verify = 0x40,
            RemapRunDll = 0x80,
            NoFixUps = 0x100,
            IgnoreBaseClass = 0x200,
            Init_IgnoreUnknown = 0x400,
            Init_Fixed_ProgId = 0x800,
            Is_Protocol = 0x1000,
            Init_For_File = 0x2000
        }

        public enum AssocStr
        {
            Command = 1,
            Executable,
            FriendlyDocName,
            FriendlyAppName,
            NoOpen,
            ShellNewValue,
            DDECommand,
            DDEIfExec,
            DDEApplication,
            DDETopic,
            InfoTip,
            QuickTip,
            TileInfo,
            ContentType,
            DefaultIcon,
            ShellExtension,
            DropTarget,
            DelegateExecute,
            Supported_Uri_Protocols,
            ProgID,
            AppID,
            AppPublisher,
            AppIconReference,
            Max
        }

        static string AssocQueryString(AssocStr association, string extension)
        {
            const int S_OK = 0;
            const int S_FALSE = 1;

            uint length = 0;
            uint ret = AssocQueryString(AssocF.None, association, extension, null, null, ref length);
            if (ret != S_FALSE)
            {
                throw new InvalidOperationException("Could not determine associated string");
            }

            var sb = new StringBuilder((int)length); // (length-1) will probably work too as the marshaller adds null termination
            ret = AssocQueryString(AssocF.None, association, extension, null, sb, ref length);
            if (ret != S_OK)
            {
                throw new InvalidOperationException("Could not determine associated string");
            }

            return sb.ToString();
        }
    }
}