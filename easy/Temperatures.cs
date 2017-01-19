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
    static void Main(string[] args)
    {
        int n = int.Parse(Console.ReadLine()); // the number of temperatures to analyse
        
        if (n ==0)
        {
            Console.WriteLine(0);
        }
        else
        {
            string temps = Console.ReadLine(); // the n temperatures expressed as integers ranging from -273 to 5526
            List<int> temperatures = temps.Split(' ').Select(s => Convert.ToInt32(s)).ToList();
            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");
            int min = 5527;
            foreach(int t in temperatures)
            {
                if (min + t == 0)
                {
                    min = Math.Max(min, t);
                    continue;
                }
                min = (Math.Abs(min) < Math.Abs(t)) ? min : t;
            }
            
            Console.WriteLine(min);
        }
    }
}