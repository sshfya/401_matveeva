using ClassLibrary1;
using System.ComponentModel;
using System.Windows.Input;
using OxyPlot.Series;
using OxyPlot;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using System.IO.Enumeration;
using System.Text;

namespace VIewModel
{
    public interface IUIServices
    {
        public void Message(string message);
        public void Dist(double[,] dist);
        public bool Save(string filename, string content);
        public string Load(string filename);
        public string LoadRuns();
        public bool SaveRuns(string content);
        public void Delete(string filename);
        public string GetFilename(List<(string, string)> runs);

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
        public RelayCommand SaveCommand { get; private set; }
        public RelayCommand LoadCommand { get; private set; }
        public RelayCommand ClearCommand { get; private set; }
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
            SaveCommand = new RelayCommand(Save, () => !isRunning && generation != null);
            LoadCommand = new RelayCommand(Load, () => !isRunning);
            ClearCommand = new RelayCommand(Clear, () => !isRunning && generation != null);

        }
        public double MutationRate { get ; set; }
        public int SizeGeneration { get; set; }
        public int CountCities { get; set; }
        private bool isRunning = false;
        public double? BestResult { get; set; }
        public List<double> Results { get; set; }
        public string CityTourOrder {  get; set; }
        public string Distances { get; set; }
        public string NameExp { get; set; }
        public double[,] Dist {  get; set; }
        private Generation generation;
        public void Update()
        {
            if (generation != null)
            {
                BestResult = generation.BestDistance;
                Results.Add((double)BestResult);
                CityTourOrder = generation.BestOrder;
                Distances = generation.BestDistances;
            }
            else
            {
                BestResult = null;
                Results = new List<double>();
                CityTourOrder = "";
                Distances = "";
            }
            RaisePropertyChanged($"BestResult");
            RaisePropertyChanged($"CityTourOrder");
            RaisePropertyChanged($"Distances");
            RaisePropertyChanged($"Results");
        }
        public void CanExecuteChanged()
        {
            StartCommand.RaiseCanExecuteChanged();
            StopCommand.RaiseCanExecuteChanged();
            SaveCommand.RaiseCanExecuteChanged();
            LoadCommand.RaiseCanExecuteChanged();
            ClearCommand.RaiseCanExecuteChanged();

        }
        public void Start()
        {
            if (generation == null)
            {
                GenerateDist();
                View.Dist(Dist);
                generation = new Generation(SizeGeneration, Dist, MutationRate);
            }
            Results = new List<double>();
            isRunning = true;
            CanExecuteChanged();
            _ = Task.Factory.StartNew(() =>
            {
                while (isRunning)
                {
                    generation.Step();
                    Update();
                    //Thread.Sleep(1000);
                }
            }, TaskCreationOptions.LongRunning);
        }
        public void Stop()
        {
            isRunning = false;
            CanExecuteChanged();
        }
        public void Save()
        {
            if (NameExp == null || NameExp == "")
            {
                View.Message("Enter name");
                return;
            }
            string FileName = NameExp + ".json";
            string generation_serialized = JsonConvert.SerializeObject(generation);
            string runs_str;
            try
            {
                runs_str = View.LoadRuns();
            }
            catch (Exception ex)
            {
                View.Message("Unable to load runs");
                return;
            }
            List<(string, string)> runs = JsonConvert.DeserializeObject<List<(string, string)>>(runs_str);
            if (runs == null)
            {
                runs = new List<(string, string)>();    
            }
            if (runs.Select(x => x.Item1).Contains(NameExp))
            {
                View.Message("Experiment name is already taken.");
                return;
            }
            if (!View.Save(FileName, generation_serialized))
            {
                View.Message("Unable to save file");
                View.Delete(FileName);
                return;
            }
            runs.Add((NameExp, FileName));
            if (!View.SaveRuns(JsonConvert.SerializeObject(runs)))
            {
                View.Message("Unable to save runs");
                View.Delete(FileName);
                return;
            }
            View.Message("OK");
        }
        public void Load() 
        {

            string runs_str;
            try
            {
                runs_str = View.LoadRuns();
            }
            catch (Exception ex)
            {
                View.Message("Unable to load runs");
                return;
            }
            List<(string, string)> runs = JsonConvert.DeserializeObject<List<(string, string)>>(runs_str);
            if (runs == null)
            {
                runs = new List<(string, string)>();
            }
            string FileName = View.GetFilename(runs);
            string generation_serialized = View.Load(FileName);
            if (generation_serialized != null && generation_serialized != "") {
                generation = JsonConvert.DeserializeObject<Generation>(generation_serialized);
                Update();
            }
        }

        public void Clear()
        {
            Dist = null;
            generation = null;
            Update();
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
