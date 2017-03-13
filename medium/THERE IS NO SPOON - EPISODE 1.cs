using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Don't let the machines win. You are humanity's last hope...
 **/
class Player
{
    public class Node
    {
        public int X {get; set;}
        public int Y {get; set;}
    }
    
    static void Main(string[] args)
    {
        int width = int.Parse(Console.ReadLine()); // the number of cells on the X axis
        int height = int.Parse(Console.ReadLine()); // the number of cells on the Y axis
        List<Node> nodes = new List<Node>(); 
        for (int i = 0; i < height; i++)
        {
            string line = Console.ReadLine(); // width characters, each either 0 or .
            for (int j = 0; j < width; j++)
            {
                if (line[j] != '.') 
                {
                    nodes.Add(new Node
                    {
                        X = j,
                        Y = i 
                    });
                }
            }
        }

        // Write an action using Console.WriteLine()
        // To debug: Console.Error.WriteLine("Debug messages...");

        while (nodes.Count != 0)
        {
            // Node at 0 could not be right neighbor or below neighbor
            Node currNode = nodes[0];
            nodes.RemoveAt(0);
            
            Node right = (from n in nodes
                          where n.X > currNode.X &&
                          n.Y == currNode.Y
                          select n).FirstOrDefault();
            Node below = (from n in nodes
                          where n.X == currNode.X &&
                          n.Y > currNode.Y
                          select n).FirstOrDefault();
            // Three coordinates: a node, its right neighbor, its bottom neighbor
            Console.WriteLine(string.Format("{0} {1} {2} {3} {4} {5}",
                currNode.X,
                currNode.Y,
                right?.X ?? -1,
                right?.Y ?? -1,
                below?.X ?? -1,
                below?.Y ?? -1
            ));        
        }
    }
}