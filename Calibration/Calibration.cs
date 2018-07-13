using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Windows;
using System.Data;
using SettingsHelper;

namespace Calibration
{
    public class LinearCalibration : INotifyPropertyChanged, ILinearCalibration
    {
        static SqlConnection connection;
        public int CalibrationID { get; set; }

        private ChemicalElemetCalibration _calibrationType = ChemicalElemetCalibration.Kalium;
        public ChemicalElemetCalibration CalibrationType
        {
            get { return _calibrationType; }
            set {
                _calibrationType = value;
                OnPropertyChanged("CalibrationType");
            }
        }

        private decimal[] _intercept;
        public decimal[] Intercept
        {
            get { return _intercept; }
            set {
                _intercept = value;
                OnPropertyChanged("Intercept");
            }
        }

        private decimal[] _slope;
        public decimal[] Slope
        {
            get { return _slope; }
            set {
                _slope = value;
                OnPropertyChanged("Slope");
            }
        }

        private decimal[] _rSquared;
        public decimal[] RSquared
        {
            get { return _rSquared; }
            private set //is calculated and set internally
            {
                _rSquared = value;
                OnPropertyChanged("RSquared");
            }
        }

        private ObservableCollection<DataPoint>[] _linearCalibrationData;
        public ObservableCollection<DataPoint>[] LinearCalibrationData
        {
            get { return _linearCalibrationData; }
            set
            {
                _linearCalibrationData = value;
                OnPropertyChanged("LinearCalibrationData");
            }
        }

        private string _description = "Введите описание";
        public string Description
        {
            get { return _description; }
            set
            {
                if (value == null || value.Trim() == "")
                    throw new ArgumentNullException("Description", "Введите описание!");
                _description = value;
                OnPropertyChanged("Description");
            }
        }

        private DateTime _date;
        public DateTime CalibrationDate
        {
            get { return _date; }
            set
            {
                if (value <= DateTime.Today)
                {
                    _date = value;
                    OnPropertyChanged("CalibrationDate");
                }
                else throw new ArgumentOutOfRangeException("CalibrationDate", 
                    "Дата калибровки не может лежать в будущем!");
            }
        }
       
        static LinearCalibration()
        {
            if (connection is null) connection = new SqlConnection(ConnectionStringGiver.GetValidConnectionString(String.Empty));
        }

        public LinearCalibration()
        {
            if (connection is null) connection = new SqlConnection(ConnectionStringGiver.GetValidConnectionString(String.Empty));
            LinearCalibrationData = new ObservableCollection<DataPoint>[2];
            LinearCalibrationData[0] = new ObservableCollection<DataPoint>();
            LinearCalibrationData[1] = new ObservableCollection<DataPoint>();
            Slope = new decimal[2];
            RSquared = new decimal[2];
            Intercept = new decimal[2];
            CalibrationDate = DateTime.Today;
        }

        public LinearCalibration(IEnumerable<Tuple<int, decimal, decimal>> lst):this()
        {
            foreach (Tuple<int, decimal, decimal> tpl in lst.ToList()) 
            {
                if (tpl.Item1 == 0) LinearCalibrationData[0].Add(new DataPoint(tpl.Item2, tpl.Item3));
                else if (tpl.Item1 == 1) LinearCalibrationData[1].Add(new DataPoint(tpl.Item2, tpl.Item3));
            }
            try
            {
                GetLinearCoefficients();
            }
            catch { }
        }

        /// <summary>
        /// </summary>
        /// <param name="conditionString">Condition used in SQL WHERE clause</param>
        /// <param name="deepRead">Shall the attached Lists of DataPoints be filled</param>
        /// <returns>Collection of Calibrations</returns>
        public static IEnumerable<LinearCalibration> GetAllLC(string conditionString, bool deepRead = false)
        {
            string commandString = "SELECT * FROM [Calibration] "
                + ((conditionString != null && conditionString != "") ? ("WHERE " + conditionString) : "");
            SqlCommand getAllSamplesCommand = new SqlCommand(commandString, connection);

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
                        ObservableCollection<DataPoint>[] lcDP = new ObservableCollection<DataPoint>[2] {
                            new ObservableCollection<DataPoint>(), new ObservableCollection<DataPoint>()};
                        var iDCalibration = reader.GetInt32(0);
                        var date = reader.GetDateTime(1);
                        var comment = reader.GetSqlString(2);
                        var calibrationType = reader.GetString(3);

                        if (deepRead)
                        {
                            foreach (DataPoint dp in DataPoint.GetAllDP(iDCalibration))
                                if (dp.Diapason == 1) lcDP[0].Add(dp); else if (dp.Diapason == 0) lcDP[1].Add(dp);
                        }

                        var lc = new LinearCalibration
                        {
                            CalibrationID = iDCalibration,
                            CalibrationDate = date,
                            Description = comment.IsNull ? null : comment.Value,
                            CalibrationType = (ChemicalElemetCalibration)
                                Enum.Parse(typeof(ChemicalElemetCalibration), calibrationType),
                            LinearCalibrationData = lcDP
                        };
                        if (deepRead)
                            try { lc.GetLinearCoefficients(); }
                            catch { }
                        yield return lc;
                    }
                };
            }
            connection.Close();
        }

        public void Insert()
        {
            var commandString = "INSERT INTO Calibration ([Date], [Comment], [CalibrationType])" +
                "OUTPUT INSERTED.IDCalibration VALUES (@date, @comment, @type)";
            SqlCommand insertCommand = new SqlCommand(commandString, connection);
            SqlTransaction sqlTran = null;
            insertCommand.Parameters.AddRange(new SqlParameter[]
        {
                new SqlParameter("date", CalibrationDate),
                new SqlParameter("comment", Description),
                new SqlParameter("type", CalibrationType.ToString())
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
                sqlTran = connection.BeginTransaction();
                insertCommand.Transaction = sqlTran;
                int resID = (int)insertCommand.ExecuteScalar();
                sqlTran.Commit();
                
                DataTable dataTable = new DataTable("TempCalibrationData");
                dataTable.Columns.AddRange(new DataColumn[]
                {
                    new DataColumn("IDCalibration", typeof(int)),
                    new DataColumn("IDCalibrationData", typeof(int)),
                    new DataColumn("Diapason", typeof(int)),
                    new DataColumn("Concentration", typeof(decimal)),
                    new DataColumn("Value", typeof(decimal))
                });
                LinearCalibrationData[0].ToList().ForEach(p =>
                    dataTable.Rows.Add(resID, 0, 1, p.Concentration, p.Value));
                LinearCalibrationData[1].ToList().ForEach(p =>
                    dataTable.Rows.Add(resID, 0, 2, p.Concentration, p.Value));

                SqlCommand command = new SqlCommand("UpdateCalibrationData", connection);
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter parameter = new SqlParameter("@tmp", SqlDbType.Structured);
                parameter.Value = dataTable;
                command.Parameters.Add(parameter);
                command.ExecuteNonQuery();
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

        public static void Delete(int idclb)
        {
            SqlTransaction sqlTran = null;
            var commandString = "DELETE FROM Calibration WHERE (IDCalibration = @iDCalibration)";
            SqlCommand deleteClbCommand = new SqlCommand(commandString, connection);
            deleteClbCommand.Parameters.AddWithValue("iDCalibration", idclb);

            var deletePointString = "DELETE FROM CalibrationData WHERE (IDCalibration = @iDCalibration)";
            SqlCommand deletePointsCommand = new SqlCommand(deletePointString, connection);
            deletePointsCommand.Parameters.AddWithValue("iDCalibration", idclb);

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
                sqlTran = connection.BeginTransaction();
                deletePointsCommand.Transaction = sqlTran;
                deleteClbCommand.Transaction = sqlTran;
                deletePointsCommand.ExecuteScalar();
                deleteClbCommand.ExecuteScalar();
                sqlTran.Commit();
            }
            catch (SqlException ex) when (ex.Number==547)// FK violation
            {
                sqlTran.Rollback();
                throw new Exception(ex.Message, ex);
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

        public void Update()
        {
            var commandString = "UPDATE Calibration SET Date=@clDate, " +
                "Comment=@desc, CalibrationType=@type WHERE (IDCalibration = @iDCalibration)";
            SqlCommand updateCommand = new SqlCommand(commandString, connection);
            SqlTransaction sqlTran = null;
            updateCommand.Parameters.AddRange(new SqlParameter[] {
                new SqlParameter("clDate", CalibrationDate),
                new SqlParameter("desc", Description),
                new SqlParameter("type", CalibrationType.ToString()),
                new SqlParameter("iDCalibration", CalibrationID)
            });
            try
            {
                if (connection.State != ConnectionState.Open) connection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error opening connection!", MessageBoxButton.OK);
            }

            if (connection.State != ConnectionState.Open) return;

            try
            {
                sqlTran = connection.BeginTransaction("ChangeOfCalibration");
                updateCommand.Transaction = sqlTran;
                updateCommand.ExecuteNonQuery();
                sqlTran.Commit();

                DataTable dataTable = new DataTable("TempCalibrationData");
                dataTable.Columns.AddRange(new DataColumn[]
                {
                    new DataColumn("IDCalibration", typeof(int)),
                    new DataColumn("IDCalibrationData", typeof(int)),
                    new DataColumn("Diapason", typeof(int)),
                    new DataColumn("Concentration", typeof(decimal)),
                    new DataColumn("Value", typeof(decimal))
                });
                LinearCalibrationData[0].ToList().ForEach(p =>
                    dataTable.Rows.Add(CalibrationID, 0, 1, p.Concentration, p.Value));
                LinearCalibrationData[1].ToList().ForEach(p =>
                    dataTable.Rows.Add(CalibrationID, 0, 2, p.Concentration, p.Value));

                SqlCommand command = new SqlCommand("UpdateCalibrationData", connection);
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter parameter = new SqlParameter("@tmp", SqlDbType.Structured);
                parameter.Value = dataTable;
                command.Parameters.Add(parameter);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                try { sqlTran.Rollback("ChangeOfCalibration"); } catch { }
                throw new Exception(ex.Message, ex);//Bubble up the original exception
            }
            finally
            {
                connection.Close(); //Close connection even if the querry failed
            }
        }

        public override string ToString()
        {
            return CalibrationDate.ToShortDateString().ToString() + " " + Description;
         }

        public void GetLinearCoefficients()
        {
            for (int i = 0; i <= LinearCalibrationData.Rank; i++)
            {
                int Count = LinearCalibrationData[i].Count;
                decimal _sumConcentration = 0;
                decimal _sumValues = 0;
                decimal _sumSquares = 0;
                decimal _sumProducts = 0;

                foreach (DataPoint t in LinearCalibrationData[i])
                {
                    _sumConcentration += t.Concentration;
                    _sumValues += t.Value;
                    _sumSquares += t.Concentration * t.Concentration;
                    _sumProducts += t.Concentration * t.Value;
                }
                decimal delta = _sumSquares * Count - _sumConcentration * _sumConcentration;
                if (delta == 0)
                {
                    Slope[i] = decimal.MaxValue;
                    Intercept[i] = decimal.MaxValue;
                    RSquared[i] = 0;
                }
                else
                {
                    Slope[i] = (_sumProducts * Count - _sumConcentration * _sumValues) / delta;
                    Intercept[i] = (_sumValues * _sumSquares - _sumProducts * _sumConcentration) / delta;

                    decimal valueMean = LinearCalibrationData[i].Average(x => x.Value);
                    decimal d = LinearCalibrationData[i].Sum(p => (decimal)Math.Pow((double)(p.Value - valueMean), 2.0));
                    if (d == 0) RSquared[i] = 0;
                    else RSquared[i] = 1 - LinearCalibrationData[i].Sum(p => (decimal)Math.Pow((double)(p.Value - 
                        (Slope[i] * p.Concentration + Intercept[i])), 2.0)) / d;
                }
            }
        }
        /// <summary>
        /// Converts Value to concentration basing on calibration points
        /// </summary>
        /// <param name="val">Value to be converted to  concentration</param>
        /// <param name="diap">Number of the diapason</param>
        /// <returns>Concentration</returns>
        public decimal ValueToConcentration(decimal val, int diap)
        {
            if (diap < 0 || diap > 1) throw new ArgumentOutOfRangeException("Diapason", "Недопустимый номер диапазаона");
            if (val <= 0) throw new ArgumentOutOfRangeException("Value", "или неверное значение показателя");
            if (val < LinearCalibrationData[diap].Min(p => p.Value) || val > LinearCalibrationData[diap].Max(p => p.Value))
            {//calculate by coefficients
                try { return (val - Intercept[diap]) / Slope[diap]; }
                catch (Exception ex)
                {
                    throw new ArgumentOutOfRangeException("Нулевое значение углового коэффициента", ex);
                }
            }
            else
            {//calculate by the way of interpolation between two dots
                int i = LinearCalibrationData[diap].IndexOf(LinearCalibrationData[diap].Where(p => p.Value > val).First());
                    //LinearCalibrationData[diap].FindIndex(p => p.Value > val);
                return (val - LinearCalibrationData[diap][i - 1].Value)
                    * (LinearCalibrationData[diap][i].Concentration - LinearCalibrationData[diap][i - 1].Concentration)
                    / (LinearCalibrationData[diap][i].Value - LinearCalibrationData[diap][i - 1].Value)
                    + LinearCalibrationData[diap][i - 1].Concentration;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop)); }

        public bool ContainsEqualDataPoints(int diap)
        {
            if (diap < 0 || diap > 1) return true; // contains
            if (LinearCalibrationData != null && LinearCalibrationData[diap] != null)
            {
                try
                {
                   var d = (LinearCalibrationData[diap].ToList()).ToDictionary(p=>p.GetHashCode(),p=>p);
                    return false;
                }
                catch
                {
                    return true;
                }
            }
            else return true;
        }
    }

    /// <summary>
    /// Enumerates the possible chemical elements the calibration can be applied to
    /// </summary>
    public enum ChemicalElemetCalibration { Kalium, Natrium };

    public interface ILinearCalibration
    {
        int CalibrationID { get; set;}
        decimal[] Slope {get; set;}
        decimal[] Intercept { get; set; }
        decimal ValueToConcentration(decimal val, int diap);
    }
}