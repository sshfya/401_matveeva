using Microsoft.AspNetCore.Mvc;
using ClassLibrary1;
using System.Diagnostics.Metrics;
using System;
using System.Text.Json;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Contracts;

namespace Api.Controllers;

[ApiController]
[Route("experiments")]
public class EvolController : ControllerBase
{
    private static object lock_obj = new object();
    private static Random rnd = new Random();

    [HttpPost("{id}")]
    public ActionResult<Result> Post(int id)
    {
        Console.WriteLine($"POST: {id}");
        lock (lock_obj) {
            string runs_str = System.IO.File.ReadAllText("runs.json");
            List<(string, string)> runs = JsonConvert.DeserializeObject<List<(string, string)>>(runs_str);
            if (runs == null)
            {
                runs = new List<(string, string)>();
            }

            if (!runs.Select(x => x.Item1).Contains(id.ToString()))
                return StatusCode(404, "No such id");

            string file_name = runs.Where(x => x.Item1 == id.ToString()).ToArray()[0].Item2;
            
            Generation generation = JsonConvert.DeserializeObject<Generation>(System.IO.File.ReadAllText(file_name));
            try {
                generation.Step();
            }
            catch (Exception ex ){
                return StatusCode(500, ex.Message);
            }
            System.IO.File.WriteAllText(file_name, JsonConvert.SerializeObject(generation));
            Result res = new Result();
            res.Route = generation.BestOrder;
            res.Metric = generation.BestDistance.ToString();
            return res;
        }
    }
    static double[,] ConvertTo2DArray(double[][] a)
    {
        int rowCount = a.Length;
        int columnCount = a[0].Length;

        double[,] result = new double[rowCount, columnCount];

        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < columnCount; j++)
            {
                result[i, j] = a[i][j];
            }
        }

        return result;
    }
    [HttpPut]
    public ActionResult<string> Put([FromBody] Args obj){
        lock (lock_obj) {
            if (!System.IO.File.Exists("runs.json"))
                System.IO.File.WriteAllText("runs.json", "[]");

            Console.WriteLine("PUT");

            string runs_str = System.IO.File.ReadAllText("runs.json");
            List<(string, string)> runs = JsonConvert.DeserializeObject<List<(string, string)>>(runs_str);
            Console.WriteLine($"PUT runs");

            Generation generation = new Generation(obj.N, ConvertTo2DArray(obj.Dist), obj.MutationRate);
            Console.WriteLine($"PUT generation");

            int id = rnd.Next();
            string FileName = id.ToString() + ".json";
            runs.Add((id.ToString(), FileName));
            string gen_str;
            Console.WriteLine($"PUT runs");
            try {
                gen_str = JsonConvert.SerializeObject(generation);
            } catch {
                return StatusCode(500, "Serialization error");
            }
            System.IO.File.WriteAllText(FileName, gen_str);
            System.IO.File.WriteAllText("runs.json", JsonConvert.SerializeObject(runs));
            Console.WriteLine($"PUT id {id}");
            return id.ToString();
        }
    }
    [HttpDelete("{id}")]
    public ActionResult<bool> Delete(int id)
    {   
        Console.WriteLine($"DELETE: {id}");
        lock (lock_obj) {
            string runs_str = System.IO.File.ReadAllText("runs.json");
            List<(string, string)> runs = JsonConvert.DeserializeObject<List<(string, string)>>(runs_str);
            if (!runs.Select(x => x.Item1).Contains(id.ToString()))
                return StatusCode(404, "No such id");

            string file_name = runs.Where(x => x.Item1 == id.ToString()).ToArray()[0].Item2;
            
            System.IO.File.Delete(file_name);

            runs.Remove((id.ToString(), file_name));
            System.IO.File.WriteAllText("runs.json", JsonConvert.SerializeObject(runs));
            return true;
        }
    }
}
