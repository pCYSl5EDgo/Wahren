using System.Collections.Generic;
using System.Linq;

namespace Wahren
{
    public static class LexicalTreeParser
    {
        //Unit構造体やContext構造体
        public static IEnumerable<LexicalTree_Block> Parse(this List<Token> input, bool isDebug)
        {
            IEnumerator<Token> c;
            if (isDebug) c = input.RemoveCommentWhiteSpace().GetEnumerator();
            else c = input.RemoveDebugCommentWhiteSpace().GetEnumerator();
            while (c.MoveNext())
            {
                var token = c.Current;
                switch (token.Type)
                {
                    case 0:
                        switch (token.ToLowerString())
                        {
                            case "power":
                            case "spot":
                            case "unit":
                            case "class":
                            case "skill":
                            case "skillset":
                            case "race":
                            case "field":
                            case "movetype":
                            case "dungeon":
                            case "voice":
                            case "object":
                                yield return c.ParseNoFunction();
                                break;
                            case "world":
                            case "scenario":
                            case "event":
                            case "story":
                            case "fight":
                            case "function":
                            case "deploy":
                                yield return c.ParseYesFunction();
                                break;
                            case "attribute":
                            case "context":
                            case "sound":
                            case "workspace":
                            case "detail":
                                yield return c.ParseNoNameStructure();
                                break;
                            default: throw new LexicalTreeConstructionException(token);
                        }
                        break;
                    default:
                        throw new LexicalTreeConstructionException(token);
                }
            }
        }

        public static LexicalTree_NameStructure ParseYesFunction(this IEnumerator<Token> c)
        {
            Token token = c.Current;
            LexicalTree_NameStructure answer = new LexicalTree_NameStructure(token.Content) { File = token.File, Line = token.Line, Column = token.Column };
            switch (token.ToLowerString())
            {
                case "world":
                    answer.Structure = StructureDataType.World;
                    break;
                case "scenario":
                    answer.Structure = StructureDataType.Scenario;
                    break;
                case "event":
                    answer.Structure = StructureDataType.Event;
                    break;
                case "story":
                    answer.Structure = StructureDataType.Story;
                    break;
                case "fight":
                    answer.Structure = StructureDataType.Fight;
                    break;
                case "function":
                    answer.Structure = StructureDataType.Function;
                    break;
                case "deploy":
                    answer.Structure = StructureDataType.Deploy;
                    break;
            }
            if (!c.MoveNext()) throw new LexicalTreeConstructionException(token);
            token = c.Current;
            if (token.Type == 1) throw new LexicalTreeConstructionException(token);
            answer.Name = token.ToString().ToLower();
            if (!c.MoveNext()) throw new LexicalTreeConstructionException(token);
            token = c.Current;
            if (token.Type != 1 || token.IsDoubleSymbol) throw new LexicalTreeConstructionException(token);
            if (token.Symbol1 == ':')
            {
                if (!c.MoveNext()) throw new LexicalTreeConstructionException(token);
                token = c.Current;
                answer.Inherit = token.ToString().ToLower();
                if (!c.MoveNext()) throw new LexicalTreeConstructionException(token);
                token = c.Current;
                if (token.Type != 1 || token.IsDoubleSymbol || token.Symbol1 != '{') throw new LexicalTreeConstructionException(token);
                answer.Children.AddRange(c.ParseBlock());
                return answer;
            }
            else if (token.Symbol1 == '{')
            {
                answer.Inherit = "";
                answer.Children.AddRange(c.ParseBlock());
                return answer;
            }
            else throw new LexicalTreeConstructionException(token);
        }
        public static LexicalTree_NameStructure ParseNoFunction(this IEnumerator<Token> c)
        {
            var token = c.Current;
            var Block = new LexicalTree_NameStructure(token.Content) { File = token.File, Line = token.Line, Column = token.Column };
            switch (Block.Name)
            {
                case "power":
                    Block.Structure = StructureDataType.Power;
                    break;
                case "spot":
                    Block.Structure = StructureDataType.Spot;
                    break;
                case "unit":
                    Block.Structure = StructureDataType.Unit;
                    break;
                case "class":
                    Block.Structure = StructureDataType.Class;
                    break;
                case "skill":
                    Block.Structure = StructureDataType.Skill;
                    break;
                case "skillset":
                    Block.Structure = StructureDataType.Skillset;
                    break;
                case "race":
                    Block.Structure = StructureDataType.Race;
                    break;
                case "field":
                    Block.Structure = StructureDataType.Field;
                    break;
                case "movetype":
                    Block.Structure = StructureDataType.Movetype;
                    break;
                case "dungeon":
                    Block.Structure = StructureDataType.Dungeon;
                    break;
                case "voice":
                    Block.Structure = StructureDataType.Voice;
                    break;
                case "object":
                    Block.Structure = StructureDataType.Object;
                    break;
            }
            if (!c.MoveNext()) throw new LexicalTreeConstructionException(token);
            token = c.Current;
            if (token.Type == 1) throw new LexicalTreeConstructionException(token);
            Block.Name = token.ToString().ToLower();
            if (!c.MoveNext()) throw new LexicalTreeConstructionException(token);
            token = c.Current;
            if (token.Type != 1 || token.IsDoubleSymbol) throw new LexicalTreeConstructionException(token);
            if (token.Symbol1 == ':')
            {
                if (!c.MoveNext()) throw new LexicalTreeConstructionException(token);
                token = c.Current;
                if (token.Type == 1) throw new LexicalTreeConstructionException(token);
                Block.Inherit = token.ToString().ToLower();
                if (!c.MoveNext()) throw new LexicalTreeConstructionException(token);
                token = c.Current;
                if (token.Type != 1 || token.IsDoubleSymbol || token.Symbol1 != '{') throw new LexicalTreeConstructionException(token);
            }
            else if (token.Symbol1 == '{')
                Block.Inherit = "";
            else throw new LexicalTreeConstructionException(token);
            var stack = new Stack<Token>();
            int leftCount = 1;
            while (c.MoveNext())
            {
                token = c.Current;
                if (token.Symbol1 == '{') { ++leftCount; stack.Push(token); }
                else if (token.Symbol1 == '}') { if (--leftCount == 0) break; stack.Push(token); }
                else stack.Push(token);
            }
            Block.Children.AddRange(ParseAssigns(stack));
            return Block;
        }
        internal static LexicalTree_BoolParen ParseBoolParen(this IEnumerator<Token> c)
        {
            var token = c.Current;
            var answer = new LexicalTree_BoolParen() { File = token.File, Line = token.Line, Column = token.Column };
            string content;
            Token next;
            int count = 1;
            while (c.MoveNext())
            {
            Top:
                token = c.Current;
                content = token.Content?.ToLower();
                if (token.Type == 0)
                {
                    if (c.MoveNext())
                    {
                        next = c.Current;
                        if (next.Symbol1 == '(' && token.IsNext(next))
                        {
                            answer.Children.Add(new LexicalTree_Function(content) { File = token.File, Line = token.Line, Column = token.Column, Variable = c.ParseVariableParen() });
                        }
                        else
                        {
                            answer.Children.Add(new SingleContent(token));
                            goto Top;
                        }
                    }
                    else throw new LexicalTreeConstructionException(token);
                }
                else if (token.Type == 2)
                    answer.Children.Add(new SingleContent(token));
                else if (token.IsSingleSymbol)
                {
                    switch (token.Symbol1)
                    {
                        case ')':
                            --count;
                            if (count != 0)
                                answer.Children.Add(new SingleContent(token));
                            else return answer;
                            break;
                        case '(':
                            ++count;
                            answer.Children.Add(new SingleContent(token));
                            break;
                        case '+':
                            var last = answer.Children.LastOrDefault() as SingleContent;
                            if (last != null)
                            {
                                var last_token = last.Content;
                                if (last_token.Symbol1 == '+' || last_token.Symbol1 == '-' || last_token.Symbol1 == '*' || last_token.Symbol1 == '/' || last_token.Symbol1 == '%' || last_token.Symbol1 == '(' || last_token.Symbol1 == '=' || last_token.Symbol1 == '!' || last_token.Symbol1 == '<' || last_token.Symbol1 == '>')
                                {
                                    if (!c.MoveNext()) throw new LexicalTreeConstructionException(token);
                                    next = c.Current;
                                    if (token.IsNext(next) && next.Type == 2)
                                    {
                                        token = new Token(token.File, token.Line, token.Column, token.IsDebug, token.IsMemo, token.Number);
                                        answer.Children.Add(new SingleContent(token));
                                    }
                                    else goto Top;
                                }
                                else answer.Children.Add(new SingleContent(token));
                            }
                            else if (answer.Children.Count == 0)
                            {
                                if (!c.MoveNext()) throw new LexicalTreeConstructionException(token);
                                next = c.Current;
                                if (token.IsNext(next) && next.Type == 2)
                                {
                                    token = new Token(token.File, token.Line, token.Column, token.IsDebug, token.IsMemo, token.Number);
                                    answer.Children.Add(new SingleContent(token));
                                }
                                else goto Top;
                            }
                            else answer.Children.Add(new SingleContent(token));
                            break;
                        case '-':
                            var last2 = answer.Children.LastOrDefault() as SingleContent;
                            if (last2 != null)
                            {
                                var last_token2 = last2.Content;
                                if (last_token2.Symbol1 == '+' || last_token2.Symbol1 == '-' || last_token2.Symbol1 == '*' || last_token2.Symbol1 == '/' || last_token2.Symbol1 == '%' || last_token2.Symbol1 == '(' || last_token2.Symbol1 == '=' || last_token2.Symbol1 == '!' || last_token2.Symbol1 == '<' || last_token2.Symbol1 == '>')
                                {
                                    if (!c.MoveNext()) throw new LexicalTreeConstructionException(token);
                                    next = c.Current;
                                    if (token.IsNext(next) && next.Type == 2)
                                    {
                                        token = new Token(token.File, token.Line, token.Column, token.IsDebug, token.IsMemo, -token.Number);
                                        answer.Children.Add(new SingleContent(token));
                                    }
                                    else goto Top;
                                }
                                else answer.Children.Add(new SingleContent(token));
                            }
                            else if (answer.Children.Count == 0)
                            {
                                if (!c.MoveNext()) throw new LexicalTreeConstructionException(token);
                                next = c.Current;
                                if (token.IsNext(next) && next.Type == 2)
                                {
                                    token = new Token(token.File, token.Line, token.Column, token.IsDebug, token.IsMemo, -token.Number);
                                    answer.Children.Add(new SingleContent(token));
                                }
                                else goto Top;
                            }
                            else answer.Children.Add(new SingleContent(token));
                            break;
                        case '@':
                            if (c.MoveNext())
                            {
                                next = c.Current;
                                if (next.Type != 0 || !token.IsNext(next))
                                {
                                    answer.Children.Add(new SingleContent(token));
                                    goto Top;
                                }
                                token = token.Merge(next);
                                answer.Children.Add(new SingleContent(token));
                            }
                            else throw new LexicalTreeConstructionException(token);
                            break;
                        default:
                            answer.Children.Add(new SingleContent(token));
                            break;
                    }
                }
                else answer.Children.Add(new SingleContent(token));
            }
            throw new LexicalTreeConstructionException(token);
        }
        public static LexicalTree_VariableParen ParseVariableParen(this IEnumerator<Token> c)
        {
            var token = c.Current;
            Token? old = null;
            var answer = new LexicalTree_VariableParen("(") { File = token.File, Line = token.Line, Column = token.Column };
            while (c.MoveNext())
            {
                token = c.Current;
                if (old == null)
                {
                    if (token.Type == 1 && token.IsSingleSymbol && token.Symbol1 == ')') return answer;
                    else if (token.Type == 1 && token.IsSingleSymbol && token.Symbol1 == ',')
                        continue;
                    else
                        old = token;
                }
                else
                {
                    var _tmpOld = old.Value;
                    if (token.Type == 1 && token.IsSingleSymbol && token.Symbol1 == ')')
                    {
                        answer.Content.Add(_tmpOld);
                        return answer;
                    }
                    else if (token.Type == 1 && token.IsSingleSymbol && token.Symbol1 == ',')
                    {
                        answer.Content.Add(_tmpOld);
                        old = null;
                        continue;
                    }
                    else if (token.Type == 2 && _tmpOld.Type == 1 && _tmpOld.IsSingleSymbol && _tmpOld.IsNext(token))
                    {
                        if (_tmpOld.Symbol1 == '+')
                            old = token;
                        else if (old.Value.Symbol1 == '-')
                            old = new Token(token.File, token.Line, _tmpOld.Column, _tmpOld.IsDebug, false, -token.Number);
                        else old = old.Value.Merge(token);
                    }
                    else old = old.Value.Merge(token);
                }
            }
            throw new LexicalTreeConstructionException(token);
        }
        public static LexicalTree_Function ParseFunction(in this Token old, IEnumerator<Token> c)
        {
            var answer = new LexicalTree_Function(old.Content) { File = old.File, Line = old.Line, Column = old.Column };
            var token = c.Current;
            if (token.Type != 1 || token.IsDoubleSymbol || token.Symbol1 != '(') throw new LexicalTreeConstructionException(token);
            answer.Variable = c.ParseVariableParen();
            answer.Variable.Type = LexicalTree.TreeType.BoolParen;
            return answer;
        }
        public static List<LexicalTree> ParseBlock(this IEnumerator<Token> c)
        {
            var answer = new List<LexicalTree>();
            int leftParenCount = 0;
            var token = c.Current;
            if (token.Symbol1 == '{') ++leftParenCount;
            else throw new LexicalTreeConstructionException(token);
            var tmp = new Stack<Token>(1024);
            while (c.MoveNext())
            {
            Top:
                token = c.Current;
                if (token.Type == 1)
                {
                    if (token.IsDoubleSymbol)
                    {
                        tmp.Push(token);
                    }
                    else
                    {
                        if (token.Symbol1 == '{')
                        {
                            ++leftParenCount;
                            var block = new LexicalTree_Block("{") { File = token.File, Line = token.Line, Column = token.Column };
                            block.Children.AddRange(c.ParseBlock());
                            answer.Add(block);
                        }
                        else if (token.Symbol1 == '}') { if (--leftParenCount == 0) break; }
                        else tmp.Push(token);
                    }
                }
                else
                {
                    var content = token.ToString().ToLower();
                    switch (content)
                    {
                        case "if":
                        case "rif":
                        case "while":
                        case "switch":
                        case "case":
                        case "foreach":
                            var old = token;
                            if (!c.MoveNext()) throw new LexicalTreeConstructionException(token);
                            token = c.Current;
                            if (token.Type == 1 && token.IsSingleSymbol && token.Symbol1 == '(')
                            {
                                if (tmp.Count > 0) answer.AddRange(tmp.ParseAssigns());
                                var stmt = new LexicalTree_Statement(content) { File = old.File, Line = old.Line, Column = old.Column };
                                stmt.Paren = c.ParseBoolParen();
                                if (!c.MoveNext()) throw new LexicalTreeConstructionException(token);
                                token = c.Current;
                                if (token.Type != 1 || token.IsDoubleSymbol || token.Symbol1 != '{') throw new LexicalTreeConstructionException(token);
                                stmt.Children.AddRange(c.ParseBlock());
                                answer.Add(stmt);
                            }
                            else { tmp.Push(old); tmp.Push(token); }
                            break;
                        case "battle":
                        case "default":
                            old = token;
                            if (!c.MoveNext()) throw new LexicalTreeConstructionException(token);
                            token = c.Current;
                            if (token.Symbol1 == '{')
                            {
                                if (tmp.Count > 0)
                                    answer.AddRange(tmp.ParseAssigns());
                                var block = new LexicalTree_Statement(content) { File = old.File, Line = old.Line, Column = old.Column };
                                block.Children.AddRange(c.ParseBlock());
                            }
                            else if (token.Symbol1 == '=')
                            {
                                tmp.Push(old);
                                tmp.Push(token);
                            }
                            else
                            {
                                tmp.Push(old);
                                goto Top;
                            }
                            break;
                        case "else":
                            if (tmp.Count > 0)
                                answer.AddRange(tmp.ParseAssigns());
                            if (!c.MoveNext()) throw new LexicalTreeConstructionException(token);
                            token = c.Current;
                            if (token.Type == 1 && token.IsSingleSymbol && token.Symbol1 == '{')
                            {
                                var _else = new LexicalTree_Statement("else") { File = token.File, Line = token.Line, Column = token.Column };
                                _else.Children.AddRange(c.ParseBlock());
                                answer.Add(_else);
                            }
                            else if (token.Type == 0 && token.ToLowerString() == "if")
                            {
                                var _else = new LexicalTree_Statement("elseif") { File = token.File, Line = token.Line, Column = token.Column };
                                if (!c.MoveNext()) throw new LexicalTreeConstructionException(token);
                                _else.Paren = c.ParseBoolParen();
                                if (!c.MoveNext()) throw new LexicalTreeConstructionException(token);
                                _else.Children.AddRange(c.ParseBlock());
                                answer.Add(_else);
                            }
                            else throw new LexicalTreeConstructionException(token);
                            break;
                        default:
                            old = token;
                            if (!c.MoveNext()) throw new LexicalTreeConstructionException(token);
                            token = c.Current;
                            if (token.Symbol1 != '(' || !old.IsNext(token))
                            {
                                tmp.Push(old);
                                goto Top;
                            }
                            if (tmp.Count > 0)
                                answer.AddRange(tmp.ParseAssigns());
                            answer.Add(old.ParseFunction(c));
                            break;
                    }
                }
            }
            return answer;
        }
        public static LexicalTree_NoNameStructure ParseNoNameStructure(this IEnumerator<Token> c)
        {
            Token token = c.Current;
            LexicalTree_NoNameStructure answer = new LexicalTree_NoNameStructure(token.Content);
            switch (answer.Name)
            {
                case "attribute":
                    answer.Structure = StructureDataType.Attribute;
                    break;
                case "context":
                    answer.Structure = StructureDataType.Context;
                    break;
                case "sound":
                    answer.Structure = StructureDataType.Sound;
                    break;
                case "workspace":
                    answer.Structure = StructureDataType.Workspace;
                    break;
                case "detail":
                    answer.Structure = StructureDataType.Detail;
                    break;
            }
            if (!c.MoveNext()) throw new LexicalTreeConstructionException(token);
            token = c.Current;
            if (token.Type != 1 || token.Symbol1 != '{') throw new LexicalTreeConstructionException(token);
            var stack = new Stack<Token>();
            int leftCount = 1;
            while (c.MoveNext())
            {
                token = c.Current;
                if (token.Symbol1 == '{') { ++leftCount; stack.Push(token); }
                else if (token.Symbol1 == '}') { if (--leftCount == 0) break; stack.Push(token); }
                else stack.Push(token);
            }
            answer.Children.AddRange(stack.ParseAssigns());
            return answer;
        }
        //副作用として引数が消費される
        public static Stack<LexicalTree_Assign> ParseAssigns(this Stack<Token> stack)
        {
            Token token;
            var answer = new Stack<LexicalTree_Assign>();
            bool separated = false;
            var tmp = new Stack<Token>();
            while (stack.Count > 0)
            {
                token = stack.Pop();
                switch (token.Type)
                {
                    case 0:
                    case 2:
                        if (!separated && tmp.Count != 0)
                        {
                            var tmpToken = tmp.Pop();
                            tmp.Push(token.Merge(tmpToken));
                        }
                        else tmp.Push(token);
                        separated = false;
                        break;
                    case 1:
                        if (token.Symbol1 == ',' || token.Symbol1 == '*' || token.Symbol1 == ';')
                            separated = true;
                        else if (token.Symbol1 == '=')
                        {
                            Token name;
                            if (stack.Count > 0)
                            {
                                name = stack.Pop();
                                Token tmpName;
                                if (stack.Count != 0)
                                {
                                    tmpName = stack.Pop();
                                    if (tmpName.Type == 1 && tmpName.Symbol1 == '@' && tmpName.IsNext(name))
                                    {
                                        Token beforeIt;
                                        if (stack.Count != 0)
                                        {
                                            beforeIt = stack.Pop();
                                            if (beforeIt.Symbol1 != '=')
                                            {
                                                name = beforeIt.ForceMerge(tmpName).ForceMerge(name);
                                            }
                                            else
                                            {
                                                stack.Push(beforeIt);
                                                stack.Push(tmpName);
                                            }
                                        }
                                        else throw new LexicalTreeConstructionException(tmpName);
                                    }
                                    else
                                        stack.Push(tmpName);
                                    var tmpAdd = new LexicalTree_Assign(name.ToString()) { File = name.File, Line = name.Line, Column = name.Column };
                                    tmpAdd.Content.AddRange(tmp);
                                    tmp.Clear();
                                    answer.Push(tmpAdd);
                                }
                                else
                                {
                                    var tmpAdd = new LexicalTree_Assign(name.Content) { File = name.File, Line = name.Line, Column = name.Column };
                                    tmpAdd.Content.AddRange(tmp);
                                    tmp.Clear();
                                    answer.Push(tmpAdd);
                                }
                            }
                        }
                        else
                        {
                            if (!separated && tmp.Count != 0)
                            {
                                Token tmpToken = tmp.Pop();
                                tmp.Push(token.Merge(tmpToken));
                            }
                            else tmp.Push(token);
                            separated = false;
                        }
                        break;
                }
            }
            return answer;
        }
    }
}