using System.Runtime.InteropServices;
using System;
using System.Linq;


namespace ClassLibrary1;

public class Route : IComparable
{
    public int[] Order { get; set; }
    public double TotalDistance { get; set; }
    public double[,] Dist { get; set; }
    public Route(int N, double[,] dist)
    {
        Random rand = new Random();
        Dist = dist;
        Order = Enumerable.Range(0, N).OrderBy(x => rand.Next()).ToArray();
        CalculateTotalDistance();
    }
    public Route(int[] order, double[,] dist)
    {
        Order = new int[order.Length];
        for (int i = 0; i < order.Length; ++i)
        {
            Order[i] = order[i];
        }
        Dist = dist;
        CalculateTotalDistance();
    }
    public Route ChangeOrder()
    {
        Random rand = new Random();
        int[] newOrder = new int[Order.Length]; 
        Array.Copy(Order, newOrder, Order.Length);
        int First = rand.Next(newOrder.Length);
        int Second = rand.Next(newOrder.Length);
        int buf = newOrder[First];
        newOrder[First] = newOrder[Second];
        newOrder[Second] = buf;
        return new Route(newOrder, Dist);
    }
    private void CalculateTotalDistance()
    {
        double Sum = 0;
        for (int i = 0; i < Order.Length - 1; ++i)
        {
            Sum += Dist[Order[i], Order[i + 1]];
        }
        Sum += Dist[Order[0], Order[Order.Length - 1]];
        TotalDistance = Sum;
    }
    public int CompareTo(object? o)
    {
        if (o is null) throw new ArgumentException("Некорректное значение параметра");
        if (((Route)o).TotalDistance - TotalDistance > 0) return 1;
        if (((Route)o).TotalDistance - TotalDistance == 0) return 0;
        else { return -1; }
    }

}
