using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Linq;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows;
using System.Data;
using Microsoft.Win32;


namespace ChemicalAnalyses.Alumni
{
    public class ConnectionStringGiver
    {
        static SqlConnection connection;

        public static SqlConnection GetValidConnectionString(bool bGetNewConnection = false)
        {
            //if connection has already been made and new is not needed just return static connection
            if (!(connection is null) && !bGetNewConnection) return connection;

            string connstr = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).
                ConnectionStrings.ConnectionStrings["ChemicalAnalyses.Properties.Settings.ChemAnConnectionString"].ToString();
            connection = new SqlConnection(connstr);
            try
            {
                connection.Open();
                connection.Close();
                return connection;
            }
            catch
            {
                //something went wrong! 
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connstr);
                //will try to use other filename stored in the user config
                builder.AttachDBFilename = Properties.Settings.Default.DBFilePath;
                connection = new SqlConnection(builder.ConnectionString);
                try
                {
                    connection.Open();
                    connection.Close();
                    return connection;
                }
                catch
                {
                    //this was wrong too
                    //select another one
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
                                connection = new SqlConnection(builder.ConnectionString);
                                connection.Open();
                                Properties.Settings.Default.DBFilePath = ofDlg.FileName;
                                connection.Close();
                                return connection;
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
    }
}