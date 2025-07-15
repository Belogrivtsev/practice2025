namespace Commands;

using System;
using System.Collections.Concurrent;
using System.Threading;

public interface ICommand
{
    void Execute();
}

public class ServerThread : IDisposable
{
    public BlockingCollection<ICommand> commandsQueue = new BlockingCollection<ICommand>();
    public volatile bool flag;
    public Thread? workThread;
    public Action<ICommand, Exception>? ExceptionHandler { get; set; }

    public void Start()
    {
        if (workThread != null) { throw new InvalidOperationException("ServerThread started"); }
        flag = true;
        workThread = new Thread(ProcessCommandsRecursive) { IsBackground = true };
        workThread.Start();
    }

    public void ProcessCommandsRecursive()
    {
        if (!flag) { return; }
        try
        {
            var command = commandsQueue.Take();
            ExecuteCommand(command);
            if (flag) { ProcessCommandsRecursive(); }
        }
        catch (ThreadInterruptedException)
        {
            return;
        }
        catch (InvalidOperationException)
        {
            return;
        }
        catch (Exception ex)
        {
            ExceptionHandler?.Invoke(null!, ex);
            if (flag) { ProcessCommandsRecursive(); }
        }
    }

    public void ExecuteCommand(ICommand command)
    {
        try
        {
            command.Execute();
        }
        catch (Exception ex)
        {
            ExceptionHandler?.Invoke(command, ex);
        }
    }

    public void AddCommand(ICommand command)
    {
        if (!flag) { throw new InvalidOperationException("ServerThread is not running"); }
        commandsQueue.Add(command);
    }

    public virtual void SoftStop(bool skipThreadCheck = false)
    {
        if (!skipThreadCheck && Thread.CurrentThread != workThread) { throw new InvalidOperationException("Not work thread"); }
        flag = false;
        commandsQueue.CompleteAdding();
    }

    public virtual void HardStop(bool skipThreadCheck = false)
    {
        if (!skipThreadCheck && Thread.CurrentThread != workThread) { throw new InvalidOperationException("Not work thread"); }
        flag = false;
        workThread?.Interrupt();
    }

    public void QueueSoftStop()
    {
        AddCommand(new SoftStopCommand(this));
    }

    public void QueueHardStop()
    {
        AddCommand(new HardStopCommand(this));
    }

    public void Dispose()
    {
        flag = false;
        commandsQueue.CompleteAdding();
        workThread?.Interrupt();
        workThread?.Join();
        commandsQueue.Dispose();
    }

    public class SoftStopCommand : ICommand
    {
        public readonly ServerThread Server;

        public SoftStopCommand(ServerThread server)
        {
            Server = server;
        }

        public void Execute()
        {
            Server.SoftStop();
        }
    }

    public class HardStopCommand : ICommand
    {
        public readonly ServerThread Server;

        public HardStopCommand(ServerThread server)
        {
            Server = server;
        }

        public void Execute()
        {
            Server.HardStop();
        }
    }
}
