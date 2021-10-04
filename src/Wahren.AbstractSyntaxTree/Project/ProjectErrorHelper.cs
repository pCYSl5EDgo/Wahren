namespace Wahren.AbstractSyntaxTree.Project;

public static class ProjectErrorHelper
{
    public static void ErrorAdd_FileNotFound(this Project project, ReadOnlySpan<char> folder, ReadOnlySpan<char> fileName)
    {
#if DEBUG
        project.ErrorBag.Add(new($"{folder}フォルダに'{fileName}'という名前のファイルが見つかりません。"));
#else
        project.ErrorBag.Add(new($"file {folder} '{fileName}' is not found."));
#endif
    }

    public static void ErrorAdd_InsufficientMemory(this Project project, ReadOnlySpan<char> filePath, long fileSize)
    {
#if DEBUG
        project.ErrorBag.Add(new($"ファイルサイズが想定よりも大きすぎます。\n  パス: {filePath}\n  サイズ: {fileSize}"));
#else
        project.ErrorBag.Add(new($"Too large file.\n  Path: {filePath}\n  Size: {fileSize}"));
#endif
    }
}
