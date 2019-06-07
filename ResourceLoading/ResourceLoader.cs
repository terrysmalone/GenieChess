using System.IO;

namespace ResourceLoading
{
    public sealed class ResourceLoader : IResourceLoader
    {
        public string GetGameResourcePath(string fileName)
        {
            var fullFileName = Path.Combine(new [] { GetSolutionDirectory(), "//Game//", fileName });

            return fullFileName;
        }

        public string GetTestResourcePath(string fileName)
        {
            var fullFileName = Path.Combine(new[] { GetSolutionDirectory(), "//Test//", fileName });

            return fullFileName;
        }

        private static string GetSolutionDirectory()
        {
            // ReSharper disable PossibleNullReferenceException
            var solutionDirectory = 
                Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
            // ReSharper restore PossibleNullReferenceException

            return solutionDirectory;
        }


        //public static PerfTPosition LoadPerfTPosition(string perfTName)
        //{
        //    var positions = LoadPerfTPositions();

        //    var position = positions.Find(p => p.Name.ToLowerInvariant().Equals(perfTName.ToLowerInvariant()));

        //    return position;
        //}

        //public static List<PerfTPosition> LoadPerfTPositions()
        //{
        //    var position = new List<PerfTPosition>();

        //    var perfTFile = GetEvaluationResourcePath() + "PerfTPositions.txt";

        //    var lines = File.ReadAllLines(perfTFile);

        //    foreach (var line in lines)
        //    {
        //        var parts = line.Split(',');

        //        var perfTPos = new PerfTPosition
        //        {
        //            Name = parts[0],
        //            FenPosition = parts[1]
        //        };

        //        var results = new List<ulong>();

        //        for (var i = 2; i < parts.Length; i++)
        //        {
        //            results.Add(Convert.ToUInt64(parts[i]));
        //        }

        //        perfTPos.Results = results;

        //        position.Add(perfTPos);
        //    }

        //    return position;
        //}

        //public static List<TestPosition> LoadKaufmanTestPositions()
        //{
        //    var perfTFile = GetEvaluationResourcePath() + "KaufmanTestPositions.txt";

        //    return LoadTestPositions(perfTFile);
        //}

        //public static List<TestPosition> LoadBratkoKopecPositions()
        //{
        //    var perfTFile = GetEvaluationResourcePath() + "BratkoKopecPositions.txt";

        //    return LoadTestPositions(perfTFile);
        //}

        //private static List<TestPosition> LoadTestPositions(string testFilePath)
        //{
        //    var position = new List<TestPosition>();

        //    var lines = File.ReadAllLines(testFilePath);

        //    foreach (var line in lines)
        //    {
        //        var parts = line.Split(',');

        //        var testPos = new TestPosition
        //        {
        //            FenPosition = parts[0],
        //            bestMoveFEN = parts[1],
        //            Name = parts[2]
        //        };


        //        position.Add(testPos);
        //    }

        //    return position;
        //}

        //#region resource paths

        //private static string GetEvaluationResourcePath()
        //{
        //    var current = Environment.CurrentDirectory;
        //    var parent = Path.GetFullPath(Path.Combine(current, @"..\..\..\..\"));
        //    var evaluationResources = parent + @"EngineEvaluation\Resources\";

        //    return evaluationResources;
        //}

        //#endregion resource paths 

        //public static string GetResourcePath(string fileName)
        //{
        //    var current = Environment.CurrentDirectory;
        //    var parent = Path.GetFullPath(Path.Combine(current, @"..\..\..\"));
        //    var resources = parent + @"ChessGame\Resources\";

        //    return resources + fileName;
        //}

        //internal static string GetTestResourcePath(string fileName)
        //{
        //    var current = Environment.CurrentDirectory;
        //    var parent = Path.GetFullPath(Path.Combine(current, @"..\..\..\"));
        //    var resources = parent + @"Resources\";

        //    return resources + fileName;
        //}
    }
}