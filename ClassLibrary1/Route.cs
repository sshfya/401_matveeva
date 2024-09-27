using System.Runtime.InteropServices;
using System;
using System.Linq;


namespace ClassLibrary1;

public class Route : IComparable
{
    public int[] Order{ get; set; }
    public int TotalDistance{ get; set; }
    public int[,] Dist{ get; set; }
    public Route(int N, int[,] dist){
        Random rand = new Random();
        Dist = dist;
        Order = Enumerable.Range(0, N).OrderBy(x => rand.Next()).ToArray();
        CalculateTotalDistance();
    }
    public Route(int[] order, int[,] dist) {
        Order = new int[order.Length];
        for (int i = 0; i < order.Length; ++i) {
            Order[i] = order[i];
        }
        Dist = dist;
        CalculateTotalDistance();
    }
    public Route ChangeOrder(){
        Random rand = new Random();
        int First = rand.Next(Order.Length);
        int Second = rand.Next(Order.Length);
        int buf = Order[First];
        Order[First] = Order[Second];
        Order[Second] = buf;
        return new Route(Order, Dist);
    }
    private void CalculateTotalDistance(){
        int Sum = 0;
        for (int i = 0; i < Order.Length - 1; ++i){
            Sum += Dist[Order[i], Order[i + 1]];
        }
        Sum += Dist[Order[0], Order[Order.Length - 1]];
        TotalDistance = Sum;
    }
    public int CompareTo(object? o){
        if(o is null) throw new ArgumentException("Некорректное значение параметра");
        return ((Route)o).TotalDistance - TotalDistance;
    }
    
}
