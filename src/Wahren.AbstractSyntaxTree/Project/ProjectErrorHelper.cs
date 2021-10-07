using Wahren.AbstractSyntaxTree.Parser;

namespace Wahren.AbstractSyntaxTree.Project;

public static class ProjectErrorHelper
{
    public static void ErrorAdd_FileNotFound(this Project project, ReadOnlySpan<char> folder, ReadOnlySpan<char> fileName, ref Result result, ReadOnlySpan<uint> references)
    {
        DefaultInterpolatedStringHandler handler;
#if DEBUG
        handler = $"{folder}フォルダに'{fileName}'という名前のファイルが見つかりません。";
#else
        handler = $"file {folder} '{fileName}' is not found in this project.";
#endif
        if (result.FilePath is not null)
        {
            foreach (var reference in references)
            {
                ref var position = ref result.TokenList[reference].Position;
                handler.AppendLiteral("\n  ");
                handler.AppendLiteral(result.FilePath);
                handler.AppendFormatted('(');
                handler.AppendFormatted(position.Line + 1);
                handler.AppendLiteral(", ");
                handler.AppendFormatted(position.Offset + 1);
                handler.AppendFormatted(')');
            }
        }

        project.ErrorBag.Add(new(handler.ToStringAndClear()));
    }

    public static void ErrorAdd_FileNotFound(this Project project, ReadOnlySpan<char> folder, ReadOnlySpan<char> fileName)
    {
        DefaultInterpolatedStringHandler handler;
#if DEBUG
        handler = $"{folder}フォルダに'{fileName}'という名前のファイルが見つかりません。";
#else
        handler = $"file {folder} '{fileName}' is not found in this project.";
#endif

        project.ErrorBag.Add(new(handler.ToStringAndClear()));
    }

    public static void ErrorAdd_NameStructureNotFound(this Project project, ReadOnlySpan<char> kind, ReadOnlySpan<char> name, ref Result result, ReadOnlySpan<uint> references)
    {
        DefaultInterpolatedStringHandler handler;
#if DEBUG
        handler = $"{kind}構造体'{name}'が見つかりません。";
#else
        handler = $"structure {kind} '{name}' is not found in this project.";
#endif
        if (result.FilePath is not null)
        {
            foreach (var reference in references)
            {
                ref var position = ref result.TokenList[reference].Position;
                handler.AppendLiteral("\n  ");
                handler.AppendLiteral(result.FilePath);
                handler.AppendFormatted('(');
                handler.AppendFormatted(position.Line + 1);
                handler.AppendLiteral(", ");
                handler.AppendFormatted(position.Offset + 1);
                handler.AppendFormatted(')');
            }
        }
        project.ErrorBag.Add(new(handler.ToStringAndClear()));
    }

    public static void ErrorAdd_CorrespondingWritingNotFound(this Project project, ReadOnlySpan<char> typeName, ReadOnlySpan<char> name, ref Result result, ReadOnlySpan<uint> references)
    {
        DefaultInterpolatedStringHandler handler;
#if DEBUG
        handler = $"{typeName}型の変数'{name}'に対する書き込みが見つかりません。";
#else
        handler = $"Corresponding writing to the '{name}' of {typeName} is not found in this project.";
#endif
        if (result.FilePath is not null)
        {
            foreach (var reference in references)
            {
                ref var position = ref result.TokenList[reference].Position;
                handler.AppendLiteral("\n  ");
                handler.AppendLiteral(result.FilePath);
                handler.AppendFormatted('(');
                handler.AppendFormatted(position.Line + 1);
                handler.AppendLiteral(", ");
                handler.AppendFormatted(position.Offset + 1);
                handler.AppendFormatted(')');
            }
        }

        project.ErrorBag.Add(new(handler.ToStringAndClear()));
    }

    public static void ErrorAdd_AttributeTypeNotFound(this Project project, ReadOnlySpan<char> name, ref Result result, ReadOnlySpan<uint> references)
    {
        DefaultInterpolatedStringHandler handler;
#if DEBUG
        handler = $"属性'{name}'が見つかりません。";
#else
        handler = $"attribute type '{name}' is not found in this solution.";
#endif
        if (result.FilePath is not null)
        {
            foreach (var reference in references)
            {
                ref var position = ref result.TokenList[reference].Position;
                handler.AppendLiteral("\n  ");
                handler.AppendLiteral(result.FilePath);
                handler.AppendFormatted('(');
                handler.AppendFormatted(position.Line + 1);
                handler.AppendLiteral(", ");
                handler.AppendFormatted(position.Offset + 1);
                handler.AppendFormatted(')');
            }
        }

        project.ErrorBag.Add(new(handler.ToStringAndClear()));
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
