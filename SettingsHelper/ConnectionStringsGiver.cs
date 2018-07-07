using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows;
using Microsoft.Win32;


namespace SettingsHelper
{
    public class ConnectionStringGiver
    {
        static string connection;

        public static Func<string, string> GetValidConnectionString = (szUserLevelDBPAth) =>//(ref string szUserLevelDBPAth)
        {
            string connstr;
            //if connection has already been made and new is not needed just return static connection
            if (!(connection is null || connection == "")) return connection;

            try
            {
                connstr = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).
                    ConnectionStrings.ConnectionStrings["ChemicalAnalyses.Properties.Settings.ChemAnConnectionString"].ToString();
            }
            catch
            {
                MessageBox.Show("Неверная строка подключения!", "Ошибка при подключении БД",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            SqlConnection sqlconnection = new SqlConnection(connstr);
            try
            {
                sqlconnection.Open();
                sqlconnection.Close();
                return connection = sqlconnection.ConnectionString;
            }
            catch
            {
                //something went wrong! 
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connstr);
                //will try to use other filename stored in the user config
                builder.AttachDBFilename = szUserLevelDBPAth;
                sqlconnection = new SqlConnection(builder.ConnectionString);
                try
                {
                    sqlconnection.Open();
                    sqlconnection.Close();
                    return connection = sqlconnection.ConnectionString;
                }
                catch
                {
                    //this was wrong too select another one
                    OpenFileDialog ofDlg = new OpenFileDialog();
                    ofDlg.FileName = builder.AttachDBFilename;
                    ofDlg.CheckFileExists = true;
                    ofDlg.Filter = "MS SQL server files|*.mdf";
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
                                szUserLevelDBPAth = ofDlg.FileName;
                                sqlconnection.Close();
                                return connection = sqlconnection.ConnectionString;
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
        };
    }
}