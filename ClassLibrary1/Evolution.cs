using Microsoft.VisualBasic.FileIO;
using System;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ClassLibrary1;

public class Generation
{
    public Route[] Individuals { get; set; }
    public double[,] Dist { get; set; }
    public int Size { get; set; }
    public double MutationRate { get; set; }
    public int NumberGeneration = 0;
    public double BestDistance = Int32.MaxValue;
    public string BestOrder {
        get
        {
            string res = "";
            foreach (var e in Individuals[Individuals.Length - 1].Order)
            {
                res += e.ToString() + " ";
            }
            return res;
        } 
    }
    public string BestDistances
    {
        get
        {
            string res = "";
            for (int i = 0; i < Individuals[Individuals.Length - 1].Order.Length - 1; i++)
            {
                res += String.Format("{0:F2}", Dist[Individuals[Individuals.Length - 1].Order[i] , Individuals[Individuals.Length - 1].Order[i + 1]]) + " ";
            }
            return res;
        }
    }
    public Generation(int n, double[,] dist, double mutation_rate = 0.25)
    {
        MutationRate = mutation_rate;
        Dist = dist;
        Size = n;
        Individuals = new Route[n];
        Task[] tasks = new Task[n];
        for (int i = 0; i < n; ++i)
        {
            Individuals[i] = new Route(Dist.GetUpperBound(0) + 1, Dist);
        }
        Array.Sort(Individuals);
        BestDistance = Individuals[Individuals.Length - 1].TotalDistance;
    }
    public Generation() { }
    async public static Task<Generation> MakeGeneration(int n, double[,] dist, double mutation_rate = 0.25)
    {
        Generation new_generation = new Generation();
        new_generation.MutationRate = mutation_rate;
        new_generation.Dist = dist;
        new_generation.Size = n;
        new_generation.Individuals = new Route[n];
        Task[] tasks = new Task[n];

        for (int i = 0; i < n; ++i)
        {
            tasks[i] = Task.Factory.StartNew(j => {
                int i = (int)j;
                new_generation.Individuals[i] = new Route(dist.GetUpperBound(0) + 1, dist);
            }, i);
        }
        Task.WaitAll(tasks);
        Array.Sort(new_generation.Individuals);
        new_generation.BestDistance = new_generation.Individuals[new_generation.Individuals.Length - 1].TotalDistance;
        return new_generation;
    }
    public async void Step()
    {
        ++NumberGeneration;
        int updated_individuals = (int)Math.Ceiling(MutationRate * Size);
        Random rand = new Random();
        Task[] tasks = new Task[(int)Math.Ceiling(MutationRate * Size)];
        for (int i = 0; i < updated_individuals; ++i)
        {
            int next = rand.Next(Individuals.Length - updated_individuals) + updated_individuals;
            tasks[i] = Task.Factory.StartNew(j => {
                int i = (int)j;
                Individuals[i] = Individuals[next].ChangeOrder();
            }, i);
        }
        Task.WaitAll(tasks);
        Array.Sort(Individuals);
        BestDistance = Individuals[Individuals.Length - 1].TotalDistance;
    }
}

public class City
{
    public double x { get; set; }
    public double y { get; set; }
    public City(double x, double y) { this.x = x; this.y = y; }
    public static double Get_S(City a, City b)
    {
        return Math.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y));
    }

}

