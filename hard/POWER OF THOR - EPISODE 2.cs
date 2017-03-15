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
        // Get basic direction toward mass center
        string direction = thor.FSM.GlobalState.GetMoveDirection(thor);
        int newX;
        int newY;
        if (!string.IsNullOrEmpty(direction) &&
            thor.Board.IsSafeTravel(direction, out newX, out newY))
        {
            // move to safe spot
            thor.X = newX;
            thor.Y = newY;
            Console.WriteLine(direction);
        }
        else
        {
            Console.WriteLine("WAIT");
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
        // Get basic direction toward mass center
        string direction = thor.FSM.GlobalState.GetMoveDirection(thor);
        // if we are at the center move somewhere
        if (string.IsNullOrEmpty(direction)) { direction = "N"; }
        
        int newX;
        int newY;
        // Move to first left safe spot
        for (int i = 0; i < 8 ;i++)
        {
            if (thor.Board.IsSafeTravel(direction, out newX, out newY))
            {
                thor.X = newX;
                thor.Y = newY;
                Console.WriteLine(direction);
                return;
            }
            else
            {
                direction = RotateLeft(direction);
            }
        }
        // Dead End
        Console.WriteLine("STRIKE");
    }   
    public void Exit(Thor thor)
    {
        Console.Error.WriteLine("Leaving Herding state.");
    }
    private string RotateLeft(string direction)
    {
        var shift = new Dictionary<string, string>()
        {
            {"N", "NW"},
            {"NW", "W"},
            {"W", "SW"},
            {"SW", "S"},
            {"S", "SE"},
            {"SE", "E"},
            {"E", "NE"},
            {"NE", "N"}
        };
        return shift[direction];
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
        else if (EnoughGiantsAreNear(thor))
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
        int x, y;
        return thor.Board.IsSafeTravel(string.Empty, out x, out y);
    }
    private bool EnoughGiantsAreNear(Thor thor)
    {
        int perHammerLeft = thor.Board.Giants.Count / thor.H;
        int strikeDistance = 4;
        return thor.Board.GiantsNearSpot(thor.X, thor.Y, strikeDistance) >=
            perHammerLeft;
    }
    public string GetMoveDirection(Thor thor)
    {
        // Calculate the mass center
        Tuple<int, int> center = thor.Board.GetMassCenter();   
        string direction = string.Empty;
        if (center.Item2 < thor.Y) { direction += "N"; }
        if (center.Item2 > thor.Y) { direction += "S"; }
        if (center.Item1 > thor.X) { direction += "E"; }
        if (center.Item1 < thor.X) { direction += "W"; }
        return direction;
    }
}

/// Simple FSM
class StateMachine
{
    private Thor owner;
    public IThorState MoveState { get; } = new Move();
    public IThorState StrikeState { get; } = new Strike();
    public IThorState HerdingState {get; } = new Herding();
    public Global GlobalState {get; } = new Global();
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
    public Board Board { get; set; }
    
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
    public Thor Thor { get; set; }
    
    public void  Clear() { Giants.Clear(); }
    public void Add(Giant giant) { Giants.Add(giant); }
    public int GiantsNearSpot(int x, int y, int distance)
    {
        return Giants.Count(g => Math.Abs(g.X - x) <= distance &&
                                      Math.Abs(g.Y - y) <= distance);
    }
    public bool IsSafeTravel(string direction, out int x, out int y)
    {
        x = Thor.X;
        y = Thor.Y;
        if (direction.Contains("N")) { y--; }
        if (direction.Contains("S")) { y++; }
        if (direction.Contains("W")) { x--; }
        if (direction.Contains("E")) { x++; }
        return GiantsNearSpot(x, y, 1) == 0;
    }
    public Tuple<int, int> GetMassCenter()
    {
        return Tuple.Create(
            Giants.Sum(g => g.X) / Giants.Count,
            Giants.Sum(g => g.Y) / Giants.Count);
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
        Board board = new Board();
        Thor thor = new Thor{X = TX, Y = TY};
        board.Thor = thor;
        thor.Board = board;
        // game loop
        while (true)
        {
            inputs = Console.ReadLine().Split(' ');
            int H = int.Parse(inputs[0]); // the remaining number of hammer strikes.
            thor.H = H;
            board.Clear();
            int N = int.Parse(inputs[1]); // the number of giants which are still present on the map.
            for (int i = 0; i < N; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int X = int.Parse(inputs[0]);
                int Y = int.Parse(inputs[1]);
                board.Add(new Giant{X = X, Y = Y});
                //Console.Error.WriteLine($"Giant @ {X},{Y}");
            }
            thor.Act();
        }
    }
}