using Wahren.PooledList;

namespace Wahren.ResourceFileSystem;

public sealed class StageDictionary : IDisposable
{
    private readonly StringSpanKeyDictionary<Map> dictionary = new();

    public StageDictionary(string folderPath)
    {
        foreach (var path in Directory.EnumerateFiles(folderPath, "*.map", SearchOption.TopDirectoryOnly))
        {
            var name = Path.GetFileNameWithoutExtension(path);
            dictionary.TryAdd(name, new(path));
        }
    }

    public void Dispose()
    {
        dictionary.Dispose();
    }
}

public record class Map(string Name)
{
}
