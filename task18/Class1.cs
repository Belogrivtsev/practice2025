namespace task18;
using System.Collections.Concurrent;

public interface ICommand
{
    bool Execute();
}
public interface IScheduler
{
    bool HasCommand();
    ICommand Select();
    void Add(ICommand cmd);
}

public class RoundRobinScheduler : IScheduler
{
    public readonly Queue<ICommand> commands = new Queue<ICommand>();
    public readonly object Lock = new object();

    public bool HasCommand()
    {
        lock (Lock)
        {
            return commands.Count > 0;
        }
    }

    public ICommand Select()
    {
        lock (Lock)
        {
            if (commands.Count == 0) { throw new InvalidOperationException("No commands available"); }
            var command = commands.Dequeue();
            commands.Enqueue(command);
            return command;
        }
    }

    public void Add(ICommand cmd)
    {
        if (cmd == null) { throw new ArgumentNullException(nameof(cmd)); }
        lock (Lock)
        {
            commands.Enqueue(cmd);
        }
    }
    public void Remove(ICommand command)
    {
        lock (Lock)
        {
            var newQueue = new Queue<ICommand>(commands.Where(c => !ReferenceEquals(c, command)));
            commands.Clear();
            foreach (var cmd in newQueue)
            {
                commands.Enqueue(cmd);
            }
        }
    }
}
public class LongRunningCommand : ICommand
{
    public int _remainingWork;
    
    public LongRunningCommand(int totalWorkUnits)
    {
        _remainingWork = totalWorkUnits;
    }

    public bool Execute()
    {
        if (_remainingWork <= 0) { return true; }
        Thread.Sleep(100); 
        _remainingWork--;
        return _remainingWork <= 0;
    }
}
public class ServerThread : IDisposable
{
    public BlockingCollection<ICommand> commandsQueue = new BlockingCollection<ICommand>();
    public IScheduler Scheduler;
    public volatile bool flag;
    public Thread? workThread;
    public CancellationTokenSource cts = new CancellationTokenSource();

    public ServerThread(IScheduler scheduler)
    {
        Scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
    }

    public void Start()
    {
        if (workThread != null) { throw new InvalidOperationException("ServerThread started"); }
        
        flag = true;
        workThread = new Thread(() => ProcessCommands(cts.Token)) 
        { 
            IsBackground = true 
        };
        workThread.Start();
    }

    public void ProcessCommands(CancellationToken cancellationToken)
    {
        try
        {
            while (flag && !cancellationToken.IsCancellationRequested)
            {
                while (commandsQueue.TryTake(out var newCommand, 100, cancellationToken))
                {
                    Scheduler.Add(newCommand);
                }
                if (Scheduler.HasCommand())
                {
                    var command = Scheduler.Select();
                    bool isCompleted = command.Execute();
                    if (isCompleted) { RemoveCompletedCommand(command); }
                }
                else if (!cancellationToken.IsCancellationRequested)
                {
                    if (commandsQueue.TryTake(out var command, 100, cancellationToken)) { Scheduler.Add(command); }
                }
            }
        }
        catch (OperationCanceledException) {}
    }

    public void RemoveCompletedCommand(ICommand command)
    {
        if (Scheduler is RoundRobinScheduler roundRobinScheduler) { roundRobinScheduler.Remove(command); }
    }

    public void AddCommand(ICommand command)
    {
        if (!flag) { throw new InvalidOperationException("ServerThread not running"); }
        
        commandsQueue.Add(command);
    }

    public void Dispose()
    {
        flag = false;
        cts.Cancel();
        commandsQueue.CompleteAdding();
        
        workThread?.Join(2000);
        if (workThread?.IsAlive == true) { workThread?.Interrupt(); }
        commandsQueue.Dispose();
        cts.Dispose();
    }
}
