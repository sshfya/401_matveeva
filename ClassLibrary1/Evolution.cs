using Microsoft.VisualBasic.FileIO;
using System;


namespace ClassLibrary1;

public class Generation {
    public Route[] Individuals { get; set; }
    public int[,] Dist { get; set; }
    public int Size { get; set; }
    public double MutationRate { get; set; }
    public int NumberGeneration = 0;
    public int BestDistance = Int32.MaxValue;
    public Generation(int n, int[,] dist, double mutation_rate=0.25){
        MutationRate = mutation_rate;
        Dist = dist;
        Size = n;
        Individuals = new Route[n];
        for (int i = 0; i < n; ++i) {
            Individuals[i] = new Route(Dist.GetUpperBound(0) + 1, Dist);
        }
        Array.Sort(Individuals);
        BestDistance = Individuals[Individuals.Length-1].TotalDistance;
    }
    public void Step() {
        ++NumberGeneration;
        int updated_individuals = (int)Math.Ceiling(MutationRate * Size);
        Random rand = new Random();
        for (int i = 0; i < Math.Ceiling(MutationRate * Size); ++i){
            int next = rand.Next(Individuals.Length - updated_individuals) + updated_individuals;
            Individuals[i] = Individuals[next].ChangeOrder();
        }
        Array.Sort(Individuals);
        BestDistance = Individuals[Individuals.Length-1].TotalDistance;
    }
}