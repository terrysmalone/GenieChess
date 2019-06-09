using System.Collections.Generic;

namespace ResourceLoading
{
    public interface IResourceLoader
    {
        string GetGameResourcePath(string fileName);

        string GetTestResourcePath(string fileName);

        List<PerfTPosition> LoadPerfTPositions();

        List<TestPosition> LoadTestPositions(string fileName);
    }
}
