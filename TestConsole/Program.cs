using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SA_EF;
using System.Diagnostics;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new ChemicalAnalysesEntities())
            {
                context.Database.Log = s => { Debug.WriteLine(s); };
                //var dp = context.DataPoints.Where(p => p.IDCalibration == 11).ToList();
                //var clb = context.LineaCalibrations.Add(new LinearCalibration()
                //{
                //    CalibrationData = new List<DataPoint>()
                //    {
                //        new DataPoint(){ Diapason =1, Concentration =0.001M, Value = 1M},
                //        new DataPoint(){ Diapason =1, Concentration =0.002M, Value = 2M},
                //        new DataPoint(){ Diapason =2, Concentration =0.01M, Value = 10M},
                //        new DataPoint(){ Diapason =2, Concentration =0.02M, Value = 20M}
                //    }
                //});

                //var clb = context.LineaCalibrations.Find(11);//.Where(p => p.CalibrationID == 11).FirstOrDefault();
                //if (clb != null)
                //{
                //    clb.GetLinearCoefficients();
                //}
                //var dp = clb.CalibrationData.SingleOrDefault(p => p.Diapason == 1 && p.Value == 1M);
                //dp.Value = 8M;
                //context.DataPoints.RemoveRange(context.DataPoints.Where(p => p.IDCalibration == 20));
                //context.LineaCalibrations.Remove(context.LineaCalibrations.Single(p => p.CalibrationID == 20));
                var smpl = context.Samples.Find(18);
                if (smpl != null)
                {
                    var sa = smpl.SaltAnalysisDatas.ToList().FirstOrDefault();
                    //var sa = context.SaltAnalysisDatas.Where(p=>p.IDSaltAnalysis
                    //    == saID.IDSaltAnalysis).FirstOrDefault();
                    sa.CalcValues();
                    sa.CalcSchemeResults();
                    sa.CalcKaliumValue();
                }
                context.SaveChanges();
            }
        }
    }
}
