using System;
using System.Collections.Concurrent;
using System.Threading;

public interface ICommand
{
    void Execute();
}

public interface IScheduler
{
    bool HasCommand();
    ICommand Select();
    void Add(ICommand cmd);
    void Remove(ICommand cmd);
}

public class RoundRobinScheduler : IScheduler
{
    private Queue<ICommand> Commands = new Queue<ICommand>();
    private object Lock = new object();

    public bool HasCommand()
    {
        lock (Lock) { return Commands.Count > 0; }
    }

    public ICommand Select()
    {
        lock (Lock)
        {
            if (Commands.Count == 0) { throw new InvalidOperationException("No commands available"); }
            var command = Commands.Dequeue();
            Commands.Enqueue(command);
            return command;
        }
    }

    public void Add(ICommand cmd)
    {
        lock (Lock) { Commands.Enqueue(cmd); }
    }

    public void Remove(ICommand cmd)
    {
        lock (Lock)
        {
            var temp = new Queue<ICommand>();
            while (Commands.Count > 0)
            {
                var current = Commands.Dequeue();
                if (!ReferenceEquals(current, cmd)) { temp.Enqueue(current); }
            }
            Commands = temp;
        }
    }
}

public class TestCommand : ICommand
{
    private int id;
    private int counter = 0;
    private int MaxExecute;

    public TestCommand(int Id, int maxExecutions = 3)
    {
        id = Id;
        MaxExecute = maxExecutions;
    }

    public void Execute()
    {
        if (counter >= MaxExecute) { return; }
        
        Console.WriteLine($"Поток {id} вызов {++counter}");
        Thread.Sleep(100);

        if (counter >= MaxExecute) { Console.WriteLine($"Поток {id} завершен"); }
    }

    public bool IsCompleted => counter >= MaxExecute;
}

public class ServerThread : IDisposable
{
    private BlockingCollection<ICommand> NewCommands = new BlockingCollection<ICommand>();
    private IScheduler Scheduler;
    private volatile bool flag;
    private Thread? workThread;

    public ServerThread(IScheduler scheduler)
    {
        Scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
    }

    public void Start()
    {
        if (workThread != null) { throw new InvalidOperationException("Server started"); }
        flag = true;
        workThread = new Thread(ProcessCommands) { IsBackground = true };
        workThread.Start();
    }

    private void ProcessCommands()
    {
        while (flag)
        {
            while (NewCommands.TryTake(out var newCommand))
            {
                Scheduler.Add(newCommand);
            }
            if (Scheduler.HasCommand())
            {
                var command = Scheduler.Select();
                command.Execute();
                if (command is TestCommand testCmd && testCmd.IsCompleted)
                {
                    Scheduler.Remove(command);
                }
            }
            else
            {
                var command = NewCommands.Take();
                Scheduler.Add(command);
            }
        }
    }

    public void AddCommand(ICommand command)
    {
        if (!flag) { throw new InvalidOperationException("Server is not running"); }
        NewCommands.Add(command);
    }

    public void HardStop()
    {
        if (Thread.CurrentThread != workThread) { throw new InvalidOperationException("Not work thread"); }
        flag = false;
        workThread.Interrupt();
    }

    public void Dispose()
    {
        flag = false;
        NewCommands.CompleteAdding();
        workThread?.Interrupt();
        workThread?.Join();
        NewCommands.Dispose();
    }
}

class App
{
    static void Main()
    {
        var scheduler = new RoundRobinScheduler();
        var server = new ServerThread(scheduler);
        server.Start();
        for (int i = 1; i <= 10; i++)
        {
            server.AddCommand(new TestCommand(i));
        }

        Thread.Sleep(2000);

        Console.WriteLine("HardStop");
        server.Dispose();
    }
}
