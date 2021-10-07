namespace Wahren.AbstractSyntaxTree.TextTemplateHelper;

public record struct SwitchGroupOuter(int len, IEnumerable<SwitchGroupInner> items)
{
}
