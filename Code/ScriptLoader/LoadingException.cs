using System;

namespace Wahren.Analysis
{
    public sealed class ScriptLoadingException : ApplicationException
    {
        public ScriptLoadingException(LexicalTree_Assign assign) : base($"{assign.Name}@{assign.DebugInfo}") { }
        public ScriptLoadingException(Token token) : base($"{token.ToString()}@{token.DebugInfo}") { }
    }
}