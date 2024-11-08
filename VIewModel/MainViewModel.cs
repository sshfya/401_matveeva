using ClassLibrary1;
using System.ComponentModel;
using System.Windows.Input;
using OxyPlot.Series;
using OxyPlot;

namespace VIewModel
{
    public interface IUIServices
    {
        public void Message(string message);
        public void Dist(double[,] dist);
    }

    public class RelayCommand : ICommand
    {
        private readonly Action execute;
        private readonly Func<bool> canexecute;
        public event EventHandler? CanExecuteChanged;
        public RelayCommand(Action execute, Func<bool> canexecute)
        {
            this.execute = execute;
            this.canexecute = canexecute;
        }
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());

        }
        public bool CanExecute(object? parameter)
        {
            return this.canexecute();
        }

        public void Execute(object? parameter)
        {
            this.execute();
        }
    }
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName = "") { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
            

        IUIServices View;
        public RelayCommand StartCommand { get; private set; }
        public RelayCommand StopCommand { get; private set; }
        public MainViewModel(IUIServices view)
        {
            MutationRate = 0.5;
            SizeGeneration = 10;
            CountCities = 10;
            RaisePropertyChanged("MutationRate");
            RaisePropertyChanged("SizeGeneration");
            RaisePropertyChanged("CountCities");
            View = view;
            StartCommand = new RelayCommand(Start, () => !isRunning);
            StopCommand = new RelayCommand(Stop, () => isRunning);

        }
        public double MutationRate { get ; set; }
        public int SizeGeneration { get; set; }
        public int CountCities { get; set; }
        private bool isRunning = false;
        public double BestResult { get; set; }
        public List<double> Results { get; set; }
        public string CityTourOrder {  get; set; }
        public string Distances { get; set; }
        public double[,] Dist {  get; set; }
        private Generation generation;
        public void Start()
        {
            if (Dist == null)
            {
                GenerateDist();
                View.Dist(Dist);
            }
            Results = new List<double>();
            generation = new Generation(SizeGeneration, Dist, MutationRate);
            isRunning = true;
            StartCommand.RaiseCanExecuteChanged();
            StopCommand.RaiseCanExecuteChanged();

            _ = Task.Factory.StartNew(() =>
            {
                while (isRunning)
                {
                    generation.Step();
                    BestResult = generation.BestDistance;
                    Results.Add(BestResult);
                    CityTourOrder = generation.BestOrder;
                    Distances = generation.BestDistances;
                    //Plot();
                    RaisePropertyChanged($"BestResult");
                    RaisePropertyChanged($"CityTourOrder");
                    RaisePropertyChanged($"Distances");
                    RaisePropertyChanged($"Results");
                    //Thread.Sleep(1000);
                }
                generation = null;
                Dist = null;
            }, TaskCreationOptions.LongRunning);
        }
        public void Stop()
        {
            isRunning = false;
            StartCommand.RaiseCanExecuteChanged();
            StopCommand.RaiseCanExecuteChanged();

        }

        private void GenerateDist()
        {
            Random rand = new Random();
            Dist = new double[CountCities, CountCities];
            City[] towns = new City[CountCities];
            double x, y;
            for (int i = 0; i < CountCities; ++i)
            {
                x = rand.NextDouble() * 100;
                y = rand.NextDouble() * 100;
                towns[i] = new City(x, y);
            }
            for (int i = 0; i < CountCities; ++i)
            {
                for (int j = 0; j < CountCities; ++j)
                    Dist[i, j] = City.Get_S(towns[i], towns[j]);
            }
        }
    }
}
