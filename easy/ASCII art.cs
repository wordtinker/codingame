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
    class Letter
    {
        private string[] rows;
        
        public string this[int i]
        {
            get { return rows[i]; }
            set { rows[i] = value; }
        }
        
        public Letter(int size)
        {
            rows = new string[size];
        }
    }
    
    static void Main(string[] args)
    {
        string stringSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ?";
        Dictionary<char, Letter> letters = new Dictionary<char, Letter>();
        
        int L = int.Parse(Console.ReadLine());
        int H = int.Parse(Console.ReadLine());
        string T = Console.ReadLine();
                
        // Initialize letters
        foreach(char c in stringSet)
        {
            letters[c] = new Letter(H);
        }
        
        // Convert string into Letters
        for (int i = 0; i < H; i++)
        {
            string ROW = Console.ReadLine();
            int j = 0;
            while (j < ROW.Length)
            {
                Letter l = letters[stringSet[j / L]];
                l[i] = ROW.Substring(j, L);
                j += L;
            }
        }

        // print message
        for (int i = 0; i < H; i++)
        {
            string row = "";
            foreach(char c in T)
            {
                char l = Char.ToUpper(c);
                l = letters.ContainsKey(l) ? l : '?';
                row += letters[l][i];
            }
            Console.WriteLine(row);
        }

        // Write an action using Console.WriteLine()
        // To debug: Console.Error.WriteLine("Debug messages...");
    }
}