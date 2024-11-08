﻿using System.Reflection;

namespace ResourceLoading;

public sealed class ResourceLoader : IResourceLoader
{
    public string GetGameResourcePath(string fileName)
    {
        var fullFileName = Path.Combine(new [] { GetSolutionDirectory(), "SharedResources", "Game", fileName });

        return fullFileName;
    }

    public string[] GetAllPerformanceEvaluationFilePaths(string directory)
    {
        return Directory.GetFiles(Path.Combine(new[] { GetSolutionDirectory(), "SharedResources", "Test", directory }));
    }

    public List<PerfTPosition> LoadPerfTPositions()
    {
        return LoadPerfTPositions(GetTestResourcePath("PerfTPositions.txt"));
    }

    private string GetTestResourcePath(string fileName)
    {
        var fullFileName = Path.Combine(new[] { GetSolutionDirectory(), "SharedResources", "Test", fileName });

        return fullFileName;
    }

    private static string GetSolutionDirectory()
    {
        // ReSharper disable PossibleNullReferenceException
        var codeBase = Assembly.GetExecutingAssembly().CodeBase;
        var uri = new UriBuilder(codeBase);
        var path = Uri.UnescapeDataString(uri.Path);

        var currentDirectory = Directory.GetParent(Path.GetDirectoryName(path));

        var carryOn = true;

        while (carryOn)
        {
            if (Directory.GetFiles(currentDirectory.FullName).Length > 0 &&
                Directory.GetFiles(currentDirectory.FullName, "*.sln").Length > 0)
            {
                carryOn = false;
            }
            else
            {
                currentDirectory = Directory.GetParent(currentDirectory.FullName);
            }
        }

        return currentDirectory.FullName;
    }

    public static List<PerfTPosition> LoadPerfTPositions(string perfTFile)
    {
        var position = new List<PerfTPosition>();

        var lines = File.ReadAllLines(perfTFile);

        foreach (var line in lines)
        {
            var parts = line.Split(',');

            var perfTPos = new PerfTPosition { Name = parts[0], FenPosition = parts[1] };

            var results = new List<long>();

            for (var i = 2; i < parts.Length; i++)
            {
                results.Add(Convert.ToInt64(parts[i]));
            }

            perfTPos.Results = results;

            position.Add(perfTPos);
        }

        return position;
    }

    public List<TestPosition> LoadTestPositions(string testFilePath, int maxToLoad)
    {
        var position = new List<TestPosition>();

        var lines = File.ReadAllLines(testFilePath);

        var toLoad = maxToLoad == 0 ? lines.Length : Math.Min(lines.Length, maxToLoad);

        for (var i = 0; i < toLoad; i++)
        {
            var parts = lines[i].Split(new [] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            var subParts = parts[0].Split(new string[] {"bm"}, StringSplitOptions.None);

            var id = parts[1];

            if (id.Contains("am"))  //skip the alternative move
            {
                id = parts[2];
            }

            var start = id.IndexOf("\"", StringComparison.Ordinal) + 1;   // Add one to not include quote
            var end = id.LastIndexOf("\"", StringComparison.Ordinal) - start;

        var testPos = new TestPosition
        {
            FenPosition = subParts[0].Trim(),
            BestMovePgn = subParts[1].Trim(),
            Name        = id.Substring(start, end)
        };

        position.Add(testPos);
    }

    return position;
}

public List<MateInXTestPosition> LoadMateInXPositions(string testFilePath, int maxToLoad)
{
    var position = new List<MateInXTestPosition>();

    var lines = File.ReadAllLines(testFilePath);

    var toLoad = maxToLoad == 0 ? lines.Length : Math.Min(lines.Length, maxToLoad);

    for (var i = 0; i < toLoad; i++)
    {
        var parts = lines[i].Split(new [] { ';' }, StringSplitOptions.RemoveEmptyEntries);

        var testPos = new MateInXTestPosition
        {
            FenPosition = parts[0].Trim(),
            MovesList   = parts[1].Split(' '),
            Name        = parts[2]
        };

        position.Add(testPos);
    }

    return position;
    }
}
