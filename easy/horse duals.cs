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
        int N = int.Parse(Console.ReadLine());
        List<int> horses = new List<int>();
        for (int i = 0; i < N; i++)
        {
            horses.Add(int.Parse(Console.ReadLine()));
        }
        horses.Sort();
        int gap = horses[1] - horses[0];
        for (int i = 0; i < horses.Count - 1; i++)
        {
            gap = Math.Min(gap, horses[i + 1] - horses[i]);
        }
        // Write an action using Console.WriteLine()
        // To debug: Console.Error.WriteLine("Debug messages...");

        Console.WriteLine(gap);
    }
}