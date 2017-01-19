using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class Solution
{
    
    public class Defib
    {
        public string Name { get; set; }
        public double Long { get; set; }
        public double Latt { get; set; }
        
        public double getDistance(double lon, double lat)
        {
            double x = (lon - Long) * Math.Cos((lat + Latt) / 2);
            double y = lat - Latt;
            
            return Math.Sqrt(x * x + y * y);
        }
    }
    
    static void Main(string[] args)
    {
        string LON = Console.ReadLine();
        string LAT = Console.ReadLine();
        int N = int.Parse(Console.ReadLine());
        List<Defib> defibs = new List<Defib>();
        for (int i = 0; i < N; i++)
        {
            // DEFIB
            string[] specs = Console.ReadLine().Split(';');
            defibs.Add(new Defib
            {
                Name = specs[1],
                Long = Double.Parse(specs[4].Replace(',','.')),
                Latt = Double.Parse(specs[5].Replace(',','.'))
            });
        }
        
        double lon =  Double.Parse(LON.Replace(',','.'));
        double lat = Double.Parse(LAT.Replace(',','.'));
        
        defibs = defibs.OrderBy(d => d.getDistance(lon, lat)).ToList();
        // Write an action using Console.WriteLine()
        // To debug: Console.Error.WriteLine("Debug messages...");

        Console.WriteLine(defibs[0].Name);
    }
}