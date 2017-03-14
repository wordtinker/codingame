using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

interface IThorState
{
	// one shot method on entering state
    void Enter(Thor thor);
    // main state method
	void Execute(Thor thor);
    // one shot method on exiting state
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
        int prevX = thor.X;
        int prevY = thor.Y;
        // Calculate the mass center
        int x = thor.Board.Giants.Sum(g => g.X) / thor.Board.Giants.Count;
        int y = thor.Board.Giants.Sum(g => g.Y) / thor.Board.Giants.Count;
        Console.Error.WriteLine($"Mass center: {x},{y}");
        string direction = string.Empty;
        if (y < thor.Y) { direction += "N"; thor.Y--; }
        if (y > thor.Y) { direction += "S"; thor.Y++; }
        if (x > thor.X) { direction += "E"; thor.X++; }
        if (x < thor.X) { direction += "W"; thor.X--; }
        if (IsSafeTravel(thor, direction))
        {
            Console.WriteLine(direction);
        }
        else
        {
            // revert position
            thor.X = prevX;
            thor.Y = prevY;
            Console.WriteLine("WAIT");
        }
    }   
    public void Exit(Thor thor)
    {
        Console.Error.WriteLine("Leaving Move state.");
    }
    private bool IsSafeTravel(Thor thor, string direction)
    {
        if (string.IsNullOrEmpty(direction)) { return false; }
        if (thor.Board.GiantsNearSpot(thor.X, thor.Y, 1) != 0) { return false;}
        return true;
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
        Console.WriteLine("STRIKE");
    }   
    public void Exit(Thor thor)
    {
        Console.Error.WriteLine("Leaving Strike state.");
    }
}

class Herding : IThorState
{
    public void Enter(Thor thor)
    {
        Console.Error.WriteLine("Entering Herding state.");
    }
    public void Execute(Thor thor)
    {
        //Giant farthest = thor.NearbyGiants
        //                     .OrderByDescending(g => g.X *g.X + g.Y * g.Y)
        //                     .First();
        //Console.Error.WriteLine($"{farthest.X}, {farthest.Y}");
        Console.WriteLine("STRIKE");
    }   
    public void Exit(Thor thor)
    {
        Console.Error.WriteLine("Leaving Herding state.");
    }
}

/// NB!
/// 1. Could implement state as singletone to share state among
/// many similar actors and save resources.
/// 2. Could use generic IThorState<T> where T : BaseActor to 
/// reuse behavior.
/// 
class Global : IThorState
{
    public void Enter(Thor thor)
    {
        Console.Error.WriteLine("Entering Global state.");
    }
    public void Execute(Thor thor)
    {
        // FSM is simple enough to use external state switch
        if (IsSafe(thor))
        {
            thor.FSM.State = thor.FSM.MoveState;
        }
        else if (thor.H > 1 || EveryGiantIsNear(thor))
        {
            thor.FSM.State = thor.FSM.StrikeState;
        }
        else
        {
            thor.FSM.State = thor.FSM.HerdingState;
        }
    }   
    public void Exit(Thor thor)
    {
        Console.Error.WriteLine("Leaving Global state.");
    }
    private bool IsSafe(Thor thor)
    {
        int safeDistance = 1;
        return thor.Board.GiantsNearSpot(thor.X, thor.Y, safeDistance) == 0;
    }
    private bool EveryGiantIsNear(Thor thor)
    {
        int strikeDistance = 3;
        return thor.Board.GiantsNearSpot(thor.X, thor.Y, strikeDistance) ==
            thor.Board.Giants.Count;
    }
}

/// Simple FSM
class StateMachine
{
    private Thor owner;
    public IThorState MoveState { get; } = new Move();
    public IThorState StrikeState { get; } = new Strike();
    public IThorState HerdingState {get; } = new Herding();
    public IThorState GlobalState {get; } = new Global();
    private IThorState currentState;
    public IThorState State
    {
        get { return currentState; }
        set
        {
            currentState?.Exit(this.owner);
            currentState = value;
            currentState?.Enter(this.owner);   
        }
    }
    public StateMachine(Thor owner) { this.owner = owner; }
    
    public void Update()
    {
        GlobalState.Execute(this.owner);
        State.Execute(this.owner);
    }
}

class Thor
{   
    public int X { get; set; }
    public int Y { get; set; }
    public int H { get; set; }
    public StateMachine FSM { get; private set; }
    public Board Board { get; } = new Board();
    
    public Thor()
    {
        FSM = new StateMachine(this);
        FSM.State = FSM.MoveState;
    }
    public void Act()
    {
        FSM.Update();
    }
}

class Giant
{
    public int X { get; set; }
    public int Y { get; set; }
}

class Board
{
    public List<Giant> Giants { get; } = new List<Giant>();
    public void  Clear() { Giants.Clear(); }
    public void Add(Giant giant ) { Giants.Add(giant); }
    public int GiantsNearSpot(int x, int y, int distance)
    {
        return Giants.Count(g => Math.Abs(g.X - x) <= distance &&
                                      Math.Abs(g.Y - y) <= distance);
    }
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
            thor.Board.Clear();
            int N = int.Parse(inputs[1]); // the number of giants which are still present on the map.
            for (int i = 0; i < N; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int X = int.Parse(inputs[0]);
                int Y = int.Parse(inputs[1]);
                thor.Board.Add(new Giant{X = X, Y = Y});
                Console.Error.WriteLine($"Giant @ {X},{Y}");
            }
            thor.Act();
        }
    }
}