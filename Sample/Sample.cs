using System;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Data;
using SettingsHelper;

namespace Samples
{
    public class Sample : INotifyPropertyChanged, ICloneable
    {
        static SqlConnection connection;

        public Sample ()
        {
            string s = "";
            if (connection is null) connection = new SqlConnection(ConnectionStringGiver.GetValidConnectionString(s));
        }

        static Sample()
        {
            string s = "";
            if (connection is null) connection = new SqlConnection(ConnectionStringGiver.GetValidConnectionString(s));
        }

        public int IDSample { get; set; } 

        private string _labnumber;
        public string LabNumber
        {
            get { return _labnumber; }
            set
            {
                if (value?.Length > 15 || value?.Length < 2)
                    throw new ArgumentOutOfRangeException("LabNumber", "Неверный формат номера!");
                _labnumber = value;
                OnPropertyChanged("LabNumber");
            }
        }

        private DateTime _samplingdate = DateTime.Today;
        public DateTime SamplingDate
        {
            get { return _samplingdate; }
            set
            {
                if (value > DateTime.Now)
                    throw new ArgumentOutOfRangeException("SamplingDate","Дата отбора не может лежать в будущем!");
                _samplingdate = value;
                OnPropertyChanged("SamplingDate");
            }
        }

        private string _desc;
        public string Description
        {
            get { return _desc; }
            set
            {
                if (value?.Length > 200) throw new ArgumentOutOfRangeException("Description", "Слишком длинная строка!");
                _desc = value;
                OnPropertyChanged("Description");
            }
        }

        public int SamplesCount { get; set; }

        public static IEnumerable<Sample> GetAllSamples(string conditionString)
        {
            string commandString = "SELECT * FROM[SamplesView] "
                + ((conditionString != null && conditionString != "") ? ("WHERE " + conditionString) : "");
            SqlCommand getAllSamplesCommand = new SqlCommand(commandString, connection);

            try
            {
                if (connection?.State != ConnectionState.Open) connection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Cannot establish connection!", MessageBoxButton.OK);
            }

            if (connection.State != ConnectionState.Open) yield break;

            using (SqlDataReader reader = getAllSamplesCommand.ExecuteReader(CommandBehavior.SequentialAccess))
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var iDSample = reader.GetInt32(0);
                        var labNumber = reader.GetString(1);
                        var samplingdate = reader.GetDateTime(2);
                        var desc = reader.GetString(3);
                        var cnt = reader.GetInt32(4);

                        var sample = new Sample
                        {
                            IDSample = iDSample,
                            LabNumber = labNumber.Trim(),
                            SamplingDate = samplingdate,
                            Description = desc,
                            SamplesCount = cnt
                        };
                        yield return sample;
                    }
                };
            }
            connection.Close();
        }

        public void Insert()
        {
            var commandString = "INSERT INTO Sample (LabNumber, SamplingDate, Description)" +
                "VALUES (@lbNumber, @smplDate, @descr)";
            SqlCommand insertCommand = new SqlCommand(commandString, connection);

            insertCommand.Parameters.AddRange(new SqlParameter[]
        {
                //new SqlParameter("IDs", IDSample),
                new SqlParameter("lbNumber", LabNumber),
                new SqlParameter("smplDate", SamplingDate),
                new SqlParameter("descr", Description)
        });
            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error opening connection!", MessageBoxButton.OK);
            }

            if (connection.State != ConnectionState.Open) return;

            try
            {
                insertCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);//Bubble up the original exception
            }
            finally
            {
                connection.Close(); //Close connection even if the querry failed
            }
        }

        public void Update()
        {
            var commandString = "UPDATE sample SET LabNumber=@labNumber, " +
                "SamplingDate=@smplDate, Description=@desc WHERE (IDSample = @IDsmpl)";
            SqlCommand updateCommand = new SqlCommand(commandString, connection);

            updateCommand.Parameters.AddRange(new SqlParameter[] {
                new SqlParameter("labNumber", LabNumber),
                new SqlParameter("smplDate", SamplingDate),
                new SqlParameter("desc", Description),
                new SqlParameter("IDsmpl", IDSample)
            });
            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error opening connection!", MessageBoxButton.OK);
            }

            if (connection.State != ConnectionState.Open) return;

            try
            {
                updateCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                connection.Close(); 
            }
        }

        public static void Delete(int idsmpl)
        {
            var commandString = "DELETE FROM Sample WHERE(IDSample= @IDsmpl)";
            SqlCommand deleteCommand = new SqlCommand(commandString, connection);
            deleteCommand.Parameters.AddWithValue("IDsmpl", idsmpl);
            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error opening connection!", MessageBoxButton.OK);
            }

            if (connection.State != ConnectionState.Open) return;

            try
            {
                deleteCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                connection.Close();
            }
        }

        public override string ToString()
        {
            return String.Format("ID={0}, Лабораторный номер: {1}, Дата отбора: {2:dd-MM-yyy}\n Описание: {3}",
                IDSample, LabNumber, SamplingDate, Description);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}