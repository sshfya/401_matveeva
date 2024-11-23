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
using System.Text.Json;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;

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
            if (!File.Exists("runs.json"))
                File.Create("runs.json");
        }
        public bool Save(string filename, string content)
        {
            try 
            {
                File.WriteAllText(filename, content);
            }
            catch
            {
                Delete(filename);
                return false;
            }
            return true;
        }
        public string Load(string filename)
        {
            try 
            {
                return File.ReadAllText(filename);
            }
            catch
            {
                return string.Empty;
            }
        }
        public string LoadRuns()
        {
            try
            {
                return File.ReadAllText("runs.json");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string GetFilename(List<(string, string)> runs)
        {
            Files dlg_window = new Files();
            if (runs.Count == 0) return null;
            dlg_window.list_files.ItemsSource = runs.Select(x => x.Item1);
            dlg_window.ShowDialog();
            if ((bool)dlg_window.DialogResult)
            {
                return runs[dlg_window.list_files.SelectedIndex].Item2;
            }
            return null;
        }
        public void Delete(string filename)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
        }
        public bool SaveRuns(string content)
        {
            try
            {
                File.WriteAllText("new_runs.json", content);
            }
            catch (Exception ex) {
                Delete("new_runs.json");
                return false;
            }
            File.Move("new_runs.json", "runs.json", true);
            Delete("new_runs.json");  
            return true;
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