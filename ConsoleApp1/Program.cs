using System.Diagnostics;
using ClassLibrary1;
using System.Threading;
using System.Runtime.InteropServices;
using System.Linq.Expressions;
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
            //Process.GetCurrentProcess().Kill();
            args.Cancel = true;
            throw new OperationCanceledException();
        }
    }
    public static void Main(string[] args) {
        int gen_size = 10;
        Random rand = new Random();
        double[, ] dist = new double[10, 10];
        City[] towns = new City[10];
        double x, y;
        for (int i = 0; i < 10; ++i)
        {
            x = rand.NextDouble() * 100;
            y = rand.NextDouble() * 100;
            towns[i] = new City(x, y);
        }
        for (int i = 0; i < 10; ++i)
        {
            for (int j = 0; j < 10; ++j)
                dist[i, j] = City.Get_S(towns[i], towns[j]);
        }
        for (int i = 0; i < 10; ++i){
            for (int j = 0; j < 10; ++j){
                Console.Write($"{(int)dist[i, j]} ");
            }
            Console.WriteLine();
        }  
        double mutation_rate = 0.5;
        Generation gen = new Generation(gen_size, dist, mutation_rate);
        myHandler handler = new myHandler(gen);
        Console.CancelKeyPress += new ConsoleCancelEventHandler(handler.runHandler);
        try {
            while (true)
            {
                Console.WriteLine($" {gen.NumberGeneration} {gen.BestDistance}");
                gen.Step();
                Thread.Sleep(100);
            }
        }
        catch (OperationCanceledException ex){
            Console.WriteLine("");
        }
    }
}