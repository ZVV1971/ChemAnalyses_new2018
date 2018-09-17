﻿using SA_EF;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace ChemicalAnalyses.Dialogs
{
    public partial class CalibrationViewDialog : Window
    {
        public LinearCalibration lcCalibration
        {
            get { return (LinearCalibration)GetValue(lcCalibrationProperty); }
            set { SetValue(lcCalibrationProperty, value); }
        }

        public static readonly DependencyProperty lcCalibrationProperty =
           DependencyProperty.Register("lcCalibration",
                typeof(LinearCalibration), typeof(CalibrationViewDialog),
                new PropertyMetadata());

        public CalibrationViewDialog(ref LinearCalibration lc)
        {
            InitializeComponent();
            lcCalibration = lc;
            grdMain.DataContext = this;
            try
            {
                    lnSeries1.ItemsSource = new ObservableCollection<Tuple<decimal, decimal>>()
                {
                    new Tuple<decimal, decimal>(0,lc.Intercept[0]),
                    new Tuple<decimal, decimal>(lc.LinearCalibrationData[0].Max(p=>p.Concentration),
                    lc.LinearCalibrationData[0].Max(p=>p.Concentration)*lc.Slope[0]+
                    lc.Intercept[0])
                };
                    lnSeries2.ItemsSource = new ObservableCollection<Tuple<decimal, decimal>>()
                {
                    new Tuple<decimal, decimal>(0,lc.Intercept[1]),
                    new Tuple<decimal, decimal>(lc.LinearCalibrationData[1].Max(p=>p.Concentration),
                    lc.LinearCalibrationData[1].Max(p=>p.Concentration)*lc.Slope[1]+
                    lc.Intercept[1])
                };
            }
            catch { }
        }
    }
}