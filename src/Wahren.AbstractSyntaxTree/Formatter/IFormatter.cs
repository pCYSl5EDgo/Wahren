﻿using Wahren.AbstractSyntaxTree.Parser;

namespace Wahren.AbstractSyntaxTree.Formatter;

public interface IFormatter<T> : IDisposable
    where T : unmanaged
{
    bool TryFormat(ref Result result, ref List<T> destination);
}
