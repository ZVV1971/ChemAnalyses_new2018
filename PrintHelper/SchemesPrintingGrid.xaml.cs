using Microsoft.Office.Interop.Excel;
using SA_EF;
using SA_EF.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PrintHelper
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
        { e.Row.Header = (e.Row.GetIndex() + 1).ToString(); }

        private bool _showHygroscopicWaterForAll = true;
        public bool ShowHygroscopicWaterForAll
        {
            get { return _showHygroscopicWaterForAll; }
            set
            {
                _showHygroscopicWaterForAll = value;
                OnPropertyChanged(nameof(ShowHygroscopicWaterForAll));
            }
        }

        private bool _useBKRespresentationVariant = true;
        public bool UseBKRespresentationVariant
        {
            get { return _useBKRespresentationVariant; }
            set
            {
                _useBKRespresentationVariant = value;
                OnPropertyChanged(nameof(UseBKRespresentationVariant));
            }
        }

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

        public void ExportToExcel (ref Workbook workbook)
        {
            if (workbook != null)
            {
                try
                {
                    Worksheet ws = workbook.Worksheets.Add();
                    ws.Name = Name;
                    int i = 1;
                    foreach (DataGridColumn clmn in dgrdMain.Columns)
                    {
                        if (clmn.Visibility == Visibility.Visible)
                        {
                            if (clmn.Header.GetType().Equals(typeof(TextBlock)))
                                ws.Cells[1, i] = ((TextBlock)clmn.Header).Text;
                            else if (clmn.Header.GetType().Equals(typeof(StackPanel)))
                            {
                                string s="";
                                foreach (TextBlock tb in ((StackPanel)clmn.Header).Children)
                                    s += tb.Text+Environment.NewLine;
                                ws.Cells[1, i] = s;
                            }

                            Binding bnd = null;
                            if (clmn.GetType().Equals(typeof(DataGridTextColumn)))
                                bnd = (Binding)(((DataGridTextColumn)clmn).Binding);
                                                                                    
                            int j = 2;
                            foreach (ISaltAnalysisCalcResults dr in dgrdMain.ItemsSource)
                            {
                                if (bnd != null)
                                    ws.Cells[j++, i] = dr.GetType().GetProperty(bnd?.Path.Path.ToString()).GetValue(dr)
                                        ?? "—"; //Null value palceholder
                            }
                            i++;
                        }
                    }
                }
                catch { }
            }
        }
    }
}