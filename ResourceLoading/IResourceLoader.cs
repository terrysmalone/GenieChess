namespace ResourceLoading
{
    public interface IResourceLoader
    {
        string GetGameResourcePath(string fileName);

        string GetTestResourcePath(string fileName);
    }
}
