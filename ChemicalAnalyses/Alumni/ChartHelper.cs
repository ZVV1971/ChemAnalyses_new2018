using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Controls.DataVisualization;
using System.Windows;
using System.ComponentModel;

namespace ChemicalAnalyses.Alumni
{
    public static class ChartHelpers
    {
        static ChartHelpers()
        {
            HideLegendStyle = new Style(typeof(Legend));
            HideLegendStyle.Setters.Add(new Setter(Legend.WidthProperty, 0.0));
            HideLegendStyle.Setters.Add(new Setter(Legend.HeightProperty, 0.0));
            HideLegendStyle.Setters.Add(new Setter(Legend.VisibilityProperty, Visibility.Collapsed));
        }

        /// <summary>Gets a <see cref="Style"/> to hide the legend.</summary>
        public static readonly Style HideLegendStyle;

        #region IsLegendHidden

        [Category("Common")]
        [AttachedPropertyBrowsableForType(typeof(Chart))]
        public static bool GetIsLegendHidden(Chart chart)
        {
            return (bool)chart.GetValue(IsLegendHiddenProperty);
        }
        public static void SetIsLegendHidden(Chart chart, bool value)
        {
            chart.SetValue(IsLegendHiddenProperty, value);
        }

        public static readonly DependencyProperty IsLegendHiddenProperty =
            DependencyProperty.RegisterAttached(
                "IsLegendHidden",
                typeof(bool), // type
                typeof(ChartHelpers), // containing static class
                new PropertyMetadata(default(bool), OnIsLegendHiddenChanged)
                );

        private static void OnIsLegendHiddenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OnIsLegendHiddenChanged((Chart)d, (bool)e.NewValue);
        }
        private static void OnIsLegendHiddenChanged(Chart chart, bool isHidden)
        {
            if (isHidden)
            {
                chart.LegendStyle = HideLegendStyle;
            }
        }

        #endregion IsLegendHidden
    }
}