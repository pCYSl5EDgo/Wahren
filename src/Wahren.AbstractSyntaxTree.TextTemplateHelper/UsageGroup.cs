using System.Linq;

namespace Wahren.AbstractSyntaxTree.TextTemplateHelper;

public record struct UsageGroup(string Type, IGrouping<UsagePair, ElementInfo>[] Groups)
{
}
