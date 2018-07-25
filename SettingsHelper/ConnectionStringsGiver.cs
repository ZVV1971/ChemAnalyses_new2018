using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace SettingsHelper
{
    public class ConnectionStringGiver
    {
        private static string connection;

        public static Func< string, string> GetValidConnectionString = (szUserLevelDBPath) =>
        {
            string connstr;
            //if connection has already been made and new is not needed just return static connection
            if (!(connection is null || connection == "")) return connection;

            try
            {
                connstr = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).
                    ConnectionStrings.ConnectionStrings["CAEntities"].ToString();
            }
            catch
            {
                MessageBox.Show("Неверная строка подключения!", "Ошибка при подключении БД",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            //try to split the connection string in order to extract DB filename
            string pattern = @"(.+provider connection string=)(.+)";
            Regex regex = new Regex(pattern);
            MatchCollection match = regex.Matches(connstr);
            if (match.Count != 1 || match[0].Groups.Count != 3)
            {
                CALogger.WriteToLogFile("В конфигурации содержится ошибочная строка подключения!");
                return null;
            }
            string metadata = match[0].Groups[1].Value;
            connstr = match[0].Groups[2].Value.Trim('"');

            SqlConnection sqlconnection = new SqlConnection(connstr);
            try
            {
                sqlconnection.Open();
                sqlconnection.Close();
                return connection = metadata + '"'+ sqlconnection.ConnectionString + '"';
            }
            catch
            {
                //something went wrong! 
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connstr);
                //will try to use other filename stored in the user config
                builder.AttachDBFilename = szUserLevelDBPath;
                using (sqlconnection = new SqlConnection(builder.ConnectionString))
                {
                    try
                    {
                        sqlconnection.Open();
                        sqlconnection.Close();
                        return connection = metadata + '"' + sqlconnection.ConnectionString + '"';
                    }
                    catch
                    {
                        //this was wrong too select another one
                        OpenFileDialog ofDlg = new OpenFileDialog();
                        ofDlg.FileName = builder.AttachDBFilename;
                        ofDlg.CheckFileExists = true;
                        ofDlg.Filter = "MS SQL server files|*.mdf";
                        ofDlg.Title = "Укажите файл с БД";
                        while (true)
                        {
                            if (ofDlg.ShowDialog() == true)
                            {
                                builder.AttachDBFilename = ofDlg.FileName;
                                //try again
                                try
                                {
                                    sqlconnection = new SqlConnection(builder.ConnectionString);
                                    sqlconnection.Open();
                                    szUserLevelDBPath = ofDlg.FileName;
                                    sqlconnection.Close();
                                    szUserLevelDBPath = sqlconnection.ConnectionString;
                                    return connection = metadata + '"' + sqlconnection.ConnectionString + '"';
                                }
                                catch
                                {
                                    if (MessageBox.Show("Указана неверная база данных!" + Environment.NewLine +
                                        "Программа не может работать без базы данных!" + Environment.NewLine
                              + "Продолжить выбор базы данных?", "Необходима база данных!", MessageBoxButton.YesNo,
                              MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.No)
                                        return null;//the program will close
                                    else continue;
                                }
                            }
                            else
                            {
                                if (MessageBox.Show("Программа не может работать без базы данных!" + Environment.NewLine
                              + "Продолжить выбор базы данных?", "Необходима база данных!", MessageBoxButton.YesNo,
                              MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.No)
                                    return null;//the program will close
                            }
                        }
                    }
                }
            }
        };
    }
}