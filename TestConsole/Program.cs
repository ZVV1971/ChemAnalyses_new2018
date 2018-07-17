using System.Linq;
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
                var smpl = context.Samples.Find(18);
                if (smpl != null)
                {
                    var sa = smpl.SaltAnalysisDatas.Where(p=>p.IDSample == smpl.IDSample).FirstOrDefault();
                    sa.CalcDryValues();
                    sa.CalcSchemeResults();
                    sa.KDry = sa.CalcKaliumValue();
                }
                context.SaveChanges();
            }
        }
    }
}