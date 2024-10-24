using OxyPlot.Series;
using OxyPlot;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VIewModel;
using static System.Net.Mime.MediaTypeNames;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class DoubleToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            double doubleValue = System.Convert.ToDouble(value);

            return String.Format("{0:F}", doubleValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            double? result = null;

            try
            {
                result = System.Convert.ToDouble(value);
            }
            catch { }

            return result.HasValue ? (object)result.Value : DependencyProperty.UnsetValue;
        }
    }

    public class ListToPlotConverter : IValueConverter
    { 
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;
            List<double> Results = (List<double>)value;
            PlotModel plot = new PlotModel();
            ScatterSeries ScatterSeries = new ScatterSeries();
            LineSeries lineSeries = new LineSeries();
            int n = Results.Count() - 1;
            for (int i = 0; i < Math.Min(20, n); ++i)
            {
                lineSeries.Points.Add(new DataPoint(n - i, Results[n - i]));

                ScatterSeries.Points.Add(new ScatterPoint(n - i, Results[n - i]));
            }
            plot.Series.Add(lineSeries);
            plot.Series.Add(ScatterSeries);
            return plot;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
    public partial class MainWindow : Window, IUIServices
    {
        public MainWindow()
        {
            DataContext = new MainViewModel(this);
            InitializeComponent();
        }

        public void Dist(double[,] dist)
        {
            //DistMatrix.Children.Clear();
            //int n = dist.GetUpperBound(0);
            //for (int i = 0; i <= n; i++)
            //{
            //    var sp = new StackPanel();
            //    sp.Orientation = Orientation.Horizontal;
            //    for (int j = 0; j <= n; ++j)
            //    {
            //        var tb = new TextBlock();
            //        tb.Text = String.Format("{0:F}", dist[i, j]).PadRight(5, ' ').Substring(0, 5) + " ";
            //        sp.Children.Add(tb);
            //    }
            //    DistMatrix.Children.Add(sp);
            //}
        }
        public void Message(string message)
        {
            MessageBox.Show(message);
        }
    }
}