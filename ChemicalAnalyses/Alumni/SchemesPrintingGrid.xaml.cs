using System.Collections.Generic;
using System.Windows.Controls;
//using SaltAnalysisDatas;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Windows;
using SA_EF;
using SA_EF.Interfaces;

namespace ChemicalAnalyses.Alumni
{
    public partial class SchemesPrintingGrid : UserControl, INotifyPropertyChanged
    {
        public IEnumerable<ISaltAnalysisCalcResults> lSA { get; set; }
       
        public SchemesPrintingGrid(IEnumerable<ISaltAnalysisCalcResults> lsa)
        {
            InitializeComponent();
            lSA = lsa;
            DataContext = this;
        }
        //Autonumbering
        private void dgrdMain_LoadingRow(object sender, DataGridRowEventArgs e)
        {e.Row.Header = (e.Row.GetIndex() + 1).ToString();}

        private bool _showHygroscopicWaterForAll = true;
        public bool ShowHygroscopicWaterForAll
        {
            get { return _showHygroscopicWaterForAll; }
            set { _showHygroscopicWaterForAll = value;
                OnPropertyChanged(nameof(ShowHygroscopicWaterForAll));
            }}

        private bool _useBKRespresentationVariant = true;
        public bool UseBKRespresentationVariant
        {
            get { return _useBKRespresentationVariant; }
            set { _useBKRespresentationVariant = value;
                OnPropertyChanged(nameof(UseBKRespresentationVariant));
            }}
        
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public SaltCalculationSchemes ResultsType
        {
            get { return (SaltCalculationSchemes)GetValue(ResultsTypeProperty); }
            set { SetValue(ResultsTypeProperty, value); }
        }

        public static readonly DependencyProperty ResultsTypeProperty =
           DependencyProperty.Register("ResultsType",
                typeof(SaltCalculationSchemes), typeof(SchemesPrintingGrid),
                new FrameworkPropertyMetadata(SaltCalculationSchemes.Chloride));
    }
}