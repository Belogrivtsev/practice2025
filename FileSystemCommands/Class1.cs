﻿namespace FileSystemCommands;

using System;
using System.IO;
using CommandLib;
using System.Linq;

public class DirectorySizeCommand : ICommand
{
    public string DirectoryPath;
    public DirectorySizeCommand(string directoryPath)
    {
        DirectoryPath = directoryPath;
    }
    public void Execute()
    {
        if (!Directory.Exists(DirectoryPath)) { throw new DirectoryNotFoundException($"Directory {DirectoryPath} not found"); }
        var files = Directory.GetFiles(DirectoryPath, "*", SearchOption.AllDirectories);
        long size = Array.ConvertAll(files, f => new FileInfo(f).Length).Sum();
        Console.WriteLine($"Directory size: {size} bytes");
    }
}
public class FindFilesCommand : ICommand
{
    public string DirectoryPath;
    public string SearchPattern;
    public FindFilesCommand(string directoryPath, string searchPattern)
    {
        DirectoryPath = directoryPath;
        SearchPattern = searchPattern;
    }
    public void Execute()
    {
        if (!Directory.Exists(DirectoryPath)) { throw new DirectoryNotFoundException($"Directory {DirectoryPath} not found"); }
        var files = Directory.GetFiles(DirectoryPath, SearchPattern);
        Console.WriteLine($"Found {files.Length} files:");
        Array.ForEach(files, f => Console.WriteLine(Path.GetFileName(f)));
    }
}
