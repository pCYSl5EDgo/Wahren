﻿namespace Wahren.AbstractSyntaxTree.Element.Statement;

public sealed record class ReturnStatement(uint TokenId) : IStatement
{
    public string DisplayName => "return";

    public void Dispose()
    {
    }
}
