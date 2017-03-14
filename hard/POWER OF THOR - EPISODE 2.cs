using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

interface IThorState
{
    void Enter(Thor thor);
    void Execute(Thor thor);
    void Exit(Thor thor);
}

class Move : IThorState
{
    public void Enter(Thor thor)
    {
        Console.Error.WriteLine("Entering Move state.");
    }
    public void Execute(Thor thor)
    {
        if (thor.IsSafe())
        {
            string direction = string.Empty;
            Giant north = thor.NearbyGiants.OrderByDescending(g => g.Y)
                              .FirstOrDefault(g => g.Y <= thor.Y);
            Giant south = thor.NearbyGiants.OrderBy(g => g.Y)
                              .FirstOrDefault(g => g.Y > thor.Y);
            Console.Error.WriteLine($"NS: {north?.Y},{south?.Y}");
            if (north == null)
            {
                if (thor.Y - south.Y < -2)
                {
                    direction = "S";
                    thor.Y++;
                }
            }
            else if (south == null)
            {
                if (thor.Y - north.Y > 2)
                {
                    direction = "N";
                    thor.Y--;
                }
            }
            else if (south.Y + north.Y - 2 * thor.Y > 2)
            {
                direction = "S";
                thor.Y++;
            }
            else if (south.Y + north.Y - 2 * thor.Y < -2)
            {
                direction = "N";
                thor.Y--;
            }
            
            Giant west = thor.NearbyGiants.OrderBy(g => g.X)
                             .FirstOrDefault(g => g.X > thor.X);
            Giant east = thor.NearbyGiants.OrderByDescending(g => g.X)
                              .FirstOrDefault(g => g.X <= thor.X);
            Console.Error.WriteLine($"EW: {east?.X},{west?.X}");
            if (west == null)
            {
                if (thor.X - east.X > 2)
                {
                    direction += "W";
                    thor.X--;
                }
            }
            else if (east == null)
            {
                if (thor.X - west.X < -2)
                {
                    direction += "E";
                    thor.X++;
                }
            }
            else if (east.X + west.X - 2 * thor.X > 2)
            {
                direction += "E";
                thor.X++;
            }
            else if (east.X + west.X - 2 * thor.X < -2)
            {
                direction += "W";
                thor.X--;
            }
            Console.WriteLine(string.IsNullOrEmpty(direction) ? "WAIT" : direction);
        }
        else
        {
            thor.State = thor.StrikeState;
            thor.Act();
        }
    }   
    public void Exit(Thor thor)
    {
        Console.Error.WriteLine("Leaving Move state.");
    }
}

class Strike : IThorState
{
    public void Enter(Thor thor)
    {
        Console.Error.WriteLine("Entering Strike state.");
    }
    public void Execute(Thor thor)
    {
        if (thor.IsSafe())
        {
            thor.State = thor.MoveState;
            thor.Act();
        }
        else
        {
            Console.WriteLine("STRIKE");
        }
    }   
    public void Exit(Thor thor)
    {
        Console.Error.WriteLine("Leaving Strike state.");
    }
}

/// Simple FSM
class Thor
{
    public IThorState MoveState { get; } = new Move();
    public IThorState StrikeState { get; } = new Strike();
    private IThorState currentState;
    public IThorState State
    {
        get { return currentState; }
        set
        {
            currentState?.Exit(this);
            currentState = value;
            currentState?.Enter(this);   
        }
    }
    
    public int X { get; set; }
    public int Y { get; set; }
    public int H { get; set; }
    public List<Giant> NearbyGiants { get; } = new List<Giant>();
    
    public Thor()
    {
        State = MoveState;
    }
    
    public void Act()
    {
        State.Execute(this);
    }
    
    public bool IsSafe()
    {
        int safeDistance = 1;
        return !NearbyGiants.Any(g => Math.Abs(g.X - X) <= safeDistance &&
                                      Math.Abs(g.Y - Y) <= safeDistance);
    }
}

class Giant
{
    public int X { get; set; }
    public int Y { get; set; }
}

class Player
{
    static void Main(string[] args)
    {
        string[] inputs;
        inputs = Console.ReadLine().Split(' ');
        int TX = int.Parse(inputs[0]);
        int TY = int.Parse(inputs[1]);
        Thor thor = new Thor{X = TX, Y = TY};
        // game loop
        while (true)
        {
            inputs = Console.ReadLine().Split(' ');
            int H = int.Parse(inputs[0]); // the remaining number of hammer strikes.
            thor.H = H;
            thor.NearbyGiants.Clear();
            int N = int.Parse(inputs[1]); // the number of giants which are still present on the map.
            for (int i = 0; i < N; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int X = int.Parse(inputs[0]);
                int Y = int.Parse(inputs[1]);
                thor.NearbyGiants.Add(new Giant{X = X, Y = Y});
                Console.Error.WriteLine($"Giant @ {X},{Y}");
            }

            // The movement or action to be carried out: WAIT STRIKE N NE E SE S SW W or N
            // Console.WriteLine("WAIT");
            thor.Act();
        }
    }
}