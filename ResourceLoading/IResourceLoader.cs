using System.Collections.Generic;

namespace ResourceLoading
{
    public interface IResourceLoader
    {
        string GetGameResourcePath(string fileName);

        string[] GetAllPerformanceEvaluationFilePaths(string directory);

        List<PerfTPosition> LoadPerfTPositions();

        List<TestPosition> LoadTestPositions(string fileName, int maxToLoad);

        List<MateInXTestPosition> LoadMateInXPositions(string fileName, int maxToLoad);
    }
}
