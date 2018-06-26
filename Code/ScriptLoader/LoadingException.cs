using System;

namespace Wahren
{
    public sealed class ScriptLoadingException : ApplicationException
    {
        public ScriptLoadingException(LexicalTree_Assign assign) : base($"{assign.Name.ToLower()}@{assign.DebugInfo}") { }
        public ScriptLoadingException(Token token) : base($"{token.ToString()}@{token.DebugInfo}") { }
    }
}