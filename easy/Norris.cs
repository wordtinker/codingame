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
    private static ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
    
    static string ChuckFormat(char chr, int cnt)
    {
        return string.Format("{0} {1}",
                chr == '1' ? "0" : "00",
                String.Concat(Enumerable.Repeat("0", cnt)));
    }
    
    static IEnumerable<string> ChuckIt(string binary)
    {
        char lastseen = binary[0];
        int count = 1;
        for (int i=1;i<binary.Length;i++)
        {
            if (binary[i] == lastseen)
            {
                count++;
            }
            else
            {
                yield return ChuckFormat(lastseen, count);

                lastseen = binary[i];
                count = 1;
            }        
        }
        yield return ChuckFormat(lastseen, count);
    }
    
    static string Encode(string letter)
    {
        
        //Console.Error.WriteLine(letter);
        byte[] bytes = Encoding.ASCII.GetBytes(letter);
        var strBytes = bytes.Select(b => Convert.ToString(b, 2).PadLeft(7, '0'));
        //foreach(var s in strBytes) Console.Error.WriteLine(s);
        
        return String.Join(" ", ChuckIt(String.Join("", strBytes)));
    }
    
    static void Main(string[] args)
    {
        string message = Encode(Console.ReadLine());
        // Write an action using Console.WriteLine()
        // To debug: Console.Error.WriteLine("Debug messages...");

        Console.WriteLine(message);
    }
}