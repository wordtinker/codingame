using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * The while loop represents the game.
 * Each iteration represents a turn of the game
 * where you are given inputs (the heights of the mountains)
 * and where you have to print an output (the index of the mountain to fire on)
 * The inputs you are given are automatically updated according to your last actions.
 **/
class Player
{
    static void Main(string[] args)
    {

        // game loop
        while (true)
        {
            List<int> mountains = new List<int>();
            for (int i = 0; i < 8; i++)
            {
                mountains.Add(int.Parse(Console.ReadLine())); // represents the height of one mountain.
            }
            int maxHeight = mountains.Max();
            int position = mountains.IndexOf(maxHeight);

            Console.WriteLine(position); // The index of the mountain to fire on.
        }
    }
}