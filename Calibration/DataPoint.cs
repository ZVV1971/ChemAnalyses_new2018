using System;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Windows;
using System.Data;
using SettingsHelper;

namespace Calibration
{
    public class DataPoint : INotifyPropertyChanged, IEquatable<DataPoint>
    {
        static SqlConnection connection;

        private decimal conc = (decimal)0.001;
        public decimal Concentration
        {
            get { return conc; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("Concentration",
                    "Концентрация должна быть положительным числом!");
                else { conc = value; OnPropertyChanged("Concentration"); }
            }
        }
        private decimal val = 1;
        public decimal Value
        {
            get { return val; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("Value",
                    "Показание прибора должно быть положительным числом!");
                else { val = value; OnPropertyChanged("value"); }
            }
        }
        public DataPoint(decimal Item1, decimal Item2)
        {
            Concentration = Item1;
            Value = Item2;
        }

        public DataPoint()
        {
            if (connection is null) connection = new SqlConnection(ConnectionStringGiver.GetValidConnectionString(String.Empty));
        }

        static DataPoint()
        {
            if (connection is null) connection = new SqlConnection(ConnectionStringGiver.GetValidConnectionString(String.Empty));
        }

        public int IDCalibration { get; set; }
        public int IDCalibrationData { get; set; }
        public int Diapason { get; set; }

        /// <summary>
        /// DataPoints are only be read on the C# side
        /// All other operations (deleting, inserting and updating)
        /// are done on DB side in SQL SP through MERGE instruction
        /// </summary>
        /// <param name="calibrationID">ID of calibration to retrieve data for</param>
        /// <returns></returns>
        public static IEnumerable<DataPoint> GetAllDP(int calibrationID)
        {
            string commandString = "SELECT * FROM [CalibrationData] WHERE [IDCalibration] = @calibrationID";
            SqlCommand getAllSamplesCommand = new SqlCommand(commandString, connection);
            getAllSamplesCommand.Parameters.Add(new SqlParameter("calibrationID", calibrationID));

            try
            {
                if (connection.State != ConnectionState.Open) connection.Open();
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
                        var iDCalibration = reader.GetInt32(0);
                        var iDCalibrationData = reader.GetInt32(1);
                        var diapason = reader.GetInt32(2);
                        var concentration = reader.GetDecimal(3);
                        var _value = reader.GetDecimal(4);

                        var dp = new DataPoint
                        {
                            IDCalibration = iDCalibration,
                            IDCalibrationData = iDCalibrationData,
                            Diapason = diapason,
                            Concentration = concentration,
                            Value = _value
                        };
                        yield return dp;
                    }
                };
            }
            connection.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as DataPoint);
        }

        public bool Equals(DataPoint dp)
        {
            if (dp == null) return false;
            return Concentration.Equals(dp.Concentration) && Value.Equals(dp.Value);
        }

        public override int GetHashCode()
        {
            return (int)(Value * 26440451) + (int)(Concentration * 334216273);
        }

        public static bool operator ==(DataPoint dp1, DataPoint dp2)
        {
            if (ReferenceEquals(dp1, dp2)) return true;
            if (ReferenceEquals(dp1, null)) return false;
            if (ReferenceEquals(dp2, null)) return false;
            return dp1.Equals(dp2);
        }

        public static bool operator !=(DataPoint dp1, DataPoint dp2)
        {
            return !(dp1 == dp2);
        }
    }
}