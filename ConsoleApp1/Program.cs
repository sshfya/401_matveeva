using System.Diagnostics;
using ClassLibrary1;
using System.Threading;
using System.Runtime.InteropServices;
public class App
{
    protected class myHandler {
        public Generation Gen;
        public myHandler (Generation gen) => Gen = gen;
        public void runHandler(object? sender, ConsoleCancelEventArgs args)
        {
            args.Cancel = true;
            var pos = Console.GetCursorPosition();
            Console.SetCursorPosition(pos.Left - 2, pos.Top);
            foreach (var city in Gen.Individuals[Gen.Individuals.Length-1].Order) {
                Console.Write(city.ToString() + " ");
            }
            Console.WriteLine();
            Process.GetCurrentProcess().Kill();
        }
    }
    public static void Main(string[] args) {
        int gen_size = 10;
        Random rand = new Random();
        int[,] dist = new int[7, 7];
        for (int i = 0; i < 7; ++i){
            for (int j = 0; j < 7; ++j){
                dist[i, j] = rand.Next(100);
            }
            dist[i,i] = 0;
        }
        for (int i = 0; i < 7; ++i){
            for (int j = 0; j < 7; ++j){
                Console.Write($"{dist[i, j]} ");
            }
            Console.WriteLine();
        }  
        double mutation_rate = 0.5;
        Generation gen = new Generation(gen_size, dist, mutation_rate);
        myHandler handler = new myHandler(gen);
        Console.CancelKeyPress += new ConsoleCancelEventHandler(handler.runHandler);
        while (true)
        {
            Console.WriteLine($" {gen.NumberGeneration} {gen.BestDistance}");
            gen.Step();
            Thread.Sleep(100);
        }  
    }
}