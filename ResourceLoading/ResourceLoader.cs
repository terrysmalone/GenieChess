using System;
using System.Collections.Generic;
using System.IO;

namespace ResourceLoading
{
    public sealed class ResourceLoader : IResourceLoader
    {
        public string GetGameResourcePath(string fileName)
        {
            var fullFileName = Path.Combine(new [] { GetSolutionDirectory(), "SharedResources", "Game", fileName });

            return fullFileName;
        }

        public string GetTestResourcePath(string fileName)
        {
            var fullFileName = Path.Combine(new[] { GetSolutionDirectory(), "SharedResources", "Test", fileName });

            return fullFileName;
        }

        public List<PerfTPosition> LoadPerfTPositions()
        {
            return LoadPerfTPositions(GetGameResourcePath("PerfTPositions.txt"));
        }

        public List<TestPosition> LoadBratkoKopecPositions()
        {
            return LoadTestPositions(GetGameResourcePath("BratkoKopecPositions.txt"));
        }

        public List<TestPosition> LoadKaufmanTestPositions()
        {
            return LoadTestPositions(GetGameResourcePath("KaufmanTestPositions.txt"));

        }

        private static string GetSolutionDirectory()
        {
            // ReSharper disable PossibleNullReferenceException
            var solutionDirectory = 
                Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
            // ReSharper restore PossibleNullReferenceException

            return solutionDirectory;
        }

        public static List<PerfTPosition> LoadPerfTPositions(string perfTFile)
        {
            var position = new List<PerfTPosition>();

            var lines = File.ReadAllLines(perfTFile);

            foreach (var line in lines)
            {
                var parts = line.Split(',');

                var perfTPos = new PerfTPosition { Name = parts[0], FenPosition = parts[1] };

                var results = new List<ulong>();

                for (var i = 2; i < parts.Length; i++)
                {
                    results.Add(Convert.ToUInt64(parts[i]));
                }

                perfTPos.Results = results;

                position.Add(perfTPos);
            }

            return position;
        }

        private static List<TestPosition> LoadTestPositions(string testFilePath)
        {
            var position = new List<TestPosition>();

            var lines = File.ReadAllLines(testFilePath);

            foreach (var line in lines)
            {
                var parts = line.Split(',');

                var testPos = new TestPosition
                {
                    FenPosition = parts[0],
                    bestMoveFEN = parts[1],
                    Name = parts[2]
                };
                
                position.Add(testPos);
            }

            return position;
        }
    }
}