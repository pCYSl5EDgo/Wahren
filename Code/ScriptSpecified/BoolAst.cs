using System;
using System.Collections.Generic;
using System.Linq;

namespace Wahren.Specific
{
    //書き換え可能な構造体とか本当に酷いものだ
    //構文解析器を独自に実装したけれど……　もうこれ自分でも何書いてるかわかんねえな
    public class Tree
    {
        public Token Token;
        public LexicalTree_Function Function;
        public Tree[] Children;
        //0 Operator + == &&
        //1 Function
        //2 figure
        //3 id
        //4 variable
        //5 text
        //6 @
        public byte Type;
        public Tree(ref Token token)
        {
            Token = token;
            Function = null;
            Children = null;
            if (token.Symbol1 == '@')
                Type = 6;
            else if (token.Type == 2)
                Type = 2;
            else if (token.Type == 1)
                switch (token.Symbol1)
                {
                    case '+':
                    case '-':
                    case '*':
                    case '/':
                    case '%':
                    case '(':
                    case ')':
                        if (token.IsDoubleSymbol) throw new Exception(token.DebugInfo);
                        Type = 0;
                        Children = new Tree[2];
                        break;
                    case '&':
                        if (token.IsSingleSymbol || token.Symbol2 != '&') throw new Exception(token.DebugInfo);
                        Type = 0;
                        Children = new Tree[2];
                        break;
                    case '|':
                        if (token.IsSingleSymbol || token.Symbol2 != '|') throw new Exception(token.DebugInfo);
                        Type = 0;
                        Children = new Tree[2];
                        break;
                    case '!':
                    case '=':
                        if (token.IsSingleSymbol || token.Symbol2 != '=') throw new Exception(token.DebugInfo);
                        Type = 0;
                        Children = new Tree[2];
                        break;
                    case '>':
                    case '<':
                        if (token.IsDoubleSymbol && token.Symbol2 != '=') throw new Exception(token.DebugInfo);
                        Type = 0;
                        Children = new Tree[2];
                        break;
                    default:
                        throw new Exception();
                }
            else if (ScenarioData2Helper.Identifier.IsMatch(token.Content))
                Type = 3;
            else if (ScenarioData2Helper.Variable.IsMatch(token.Content))
                Type = 4;
            else
                Type = 5;
        }
        public Tree(LexicalTree_Function function)
        {
            Token = default(Token);
            Function = function;
            Children = null;
            Type = 1;
        }
        public Tree CreateOperatorTree(ref Token op, ref Tree left, ref Tree right)
        {
            var ans = new Tree(ref op);
            ans.Children[0] = left;
            ans.Children[1] = right;
            return ans;
        }
    }

    internal class InterpretTreeMachine
    {
        internal static Tree Convert(LexicalTree_Function tree) => new Tree(tree);
        internal static Tree Convert(LexicalTree_BoolParen.SingleContent singleContent) => new Tree(ref singleContent.Content);
        internal static Tree Convert(LexicalTree tree)
        {
            switch (tree)
            {
                case LexicalTree_Function _1:
                    return Convert(_1);
                default:
                    return Convert(tree as LexicalTree_BoolParen.SingleContent);
            }
        }

        internal InterpretTreeMachine(LexicalTree_BoolParen boolParen)
            => this.input = boolParen?.Children ?? new List<LexicalTree>();
        int Classify
        {
            get
            {
                if (input.Count == location)
                    return 13;
                if (input[location].Type == LexicalTree.TreeType.Function)
                    return 1;
                var content = input[location] as LexicalTree_BoolParen.SingleContent;
                switch (content.Content.Type)
                {
                    case 0:
                        if (ScenarioData2Helper.Variable.IsMatch(content.Content.Content))
                            return 5;
                        if (ScenarioData2Helper.Identifier.IsMatch(content.Content.Content))
                            return 2;
                        return 4;
                    case 1:
                        switch (content.Content.Symbol1)
                        {
                            case '@': return 3;
                            case '(': return 6;
                            case ')': return 7;
                            case '+':
                            case '-':
                                return 8;
                            case '*':
                            case '/':
                            case '%':
                                return 9;
                            case '=':
                            case '!':
                            case '<':
                            case '>':
                                return 10;
                            case '&': return 11;
                            case '|': return 12;
                        }
                        break;
                    case 2: return 0;
                }
                return 6;
            }
        }
        private Tree _tree;
        internal Tree Tree => _tree != null ? _tree : (_tree = this.Run());
        private Tree Run()
        {
            int loopcount = 0;
            //ここでNullReferenceExceptionを吐いた場合はまずthis.inputを見ましょう。
            //if文中に間違いが見つかるはずです。
            //見つからなければこのプログラムのバグです。
            try
            {
                while (!Table[modeStack.Peek()][Classify](this))
                    if (loopcount++ >= 100000)
                        throw new Exception("無限ループ防止機構により無限ループが終了されました。");
                return treeStack.Pop();
            }
            catch (NullReferenceException)
            {
                foreach (var item in this.input)
                    Console.Error.WriteLine($@"{item.File}/{item.Line + 1}/{item.Column}");
                throw;
            }
        }
        Stack<Tree> treeStack = new Stack<Tree>();
        Stack<byte> modeStack = new Stack<byte>(new byte[1] { 0 });
        List<LexicalTree> input;
        int location = 0;


        static readonly Dictionary<int, Func<InterpretTreeMachine, bool>> ShiftTable = new Dictionary<int, Func<InterpretTreeMachine, bool>>(60);

        static Func<InterpretTreeMachine, bool> ShiftGen(int mode)
            => (ShiftTable.TryGetValue(mode, out var ans))
            ? ans
            : (ShiftTable[mode] = (_) =>
            {
                _.treeStack.Push(Convert(_.input[_.location]));
                ++_.location;
                _.modeStack.Push((byte)mode);
                return false;
            });

        static void ReduceCommon1(InterpretTreeMachine machine)
        {
            machine.modeStack.Pop();
        }
        static void ReduceCommon3_Paren(InterpretTreeMachine machine)
        {
            machine.treeStack.Pop();
            var second = machine.treeStack.Pop();
            machine.treeStack.Pop();
            machine.modeStack.Pop();
            machine.modeStack.Pop();
            machine.modeStack.Pop();
            machine.treeStack.Push(second);
        }
        static void ReduceCommon3(InterpretTreeMachine machine)
        {
            var third = machine.treeStack.Pop();
            var second = machine.treeStack.Pop();
            var first = machine.treeStack.Pop();
            second.Children[1] = third;
            second.Children[0] = first;
            machine.modeStack.Pop();
            machine.modeStack.Pop();
            machine.modeStack.Pop();
            machine.treeStack.Push(second);
        }

        static bool Reduce2_3(InterpretTreeMachine machine)
        {
            ReduceCommon1(machine);
            switch (machine.modeStack.Peek())
            {
                case 0:
                    machine.modeStack.Push(59);
                    break;
                default: throw new Exception();
            }
            return false;
        }
        static bool Reduce4(InterpretTreeMachine machine)
        {
            ReduceCommon3(machine);
            switch (machine.modeStack.Peek())
            {
                case 0:
                    machine.modeStack.Push(57);
                    break;
                case 18:
                case 50:
                    machine.modeStack.Push(19);
                    break;
                default: throw new Exception();
            }
            return false;
        }
        static bool Reduce5(InterpretTreeMachine machine)
        {
            ReduceCommon1(machine);
            switch (machine.modeStack.Peek())
            {
                case 0:
                    machine.modeStack.Push(57);
                    break;
                case 18:
                case 50:
                    machine.modeStack.Push(19);
                    break;
                default: throw new Exception();
            }
            return false;
        }
        static bool Reduce6(InterpretTreeMachine machine)
        {
            ReduceCommon3(machine);
            switch (machine.modeStack.Peek())
            {
                case 0:
                case 18:
                case 50:
                    machine.modeStack.Push(51);
                    break;
                case 21:
                    machine.modeStack.Push(22);
                    break;
                default: throw new Exception();
            }
            return false;
        }
        static bool Reduce7(InterpretTreeMachine machine)
        {
            ReduceCommon1(machine);
            switch (machine.modeStack.Peek())
            {
                case 0:
                case 18:
                case 50:
                    machine.modeStack.Push(51);
                    break;
                case 21:
                    machine.modeStack.Push(22);
                    break;
                default: throw new Exception();
            }
            return false;
        }
        static bool Reduce13(InterpretTreeMachine machine)
        {
            ReduceCommon3_Paren(machine);
            switch (machine.modeStack.Peek())
            {
                case 0:
                case 18:
                case 21:
                case 50:
                    machine.modeStack.Push(7);
                    break;
                case 23:
                    machine.modeStack.Push(24);
                    break;
                default: throw new Exception();
            }
            return false;
        }
        static bool Reduce8_12(InterpretTreeMachine machine)
        {
            ReduceCommon3(machine);
            switch (machine.modeStack.Peek())
            {
                case 0:
                case 18:
                case 21:
                case 50:
                    machine.modeStack.Push(7);
                    break;
                case 23:
                    machine.modeStack.Push(24);
                    break;
                default: throw new Exception();
            }
            return false;
        }
        static bool Reduce14(InterpretTreeMachine machine)
        {
            ReduceCommon3(machine);
            switch (machine.modeStack.Peek())
            {
                case 0:
                    machine.modeStack.Push(58);
                    break;
                case 18:
                case 50:
                    machine.modeStack.Push(54);
                    break;
                case 21:
                case 23:
                    machine.modeStack.Push(25);
                    break;
                case 26:
                    machine.modeStack.Push(27);
                    break;
                case 32:
                    machine.modeStack.Push(33);
                    break;
                default: throw new Exception();
            }
            return false;
        }
        static bool Reduce15(InterpretTreeMachine machine)
        {
            ReduceCommon1(machine);
            switch (machine.modeStack.Peek())
            {
                case 0:
                    machine.modeStack.Push(58);
                    break;
                case 18:
                case 50:
                    machine.modeStack.Push(54);
                    break;
                case 21:
                case 23:
                    machine.modeStack.Push(25);
                    break;
                case 26:
                    machine.modeStack.Push(27);
                    break;
                case 32:
                    machine.modeStack.Push(33);
                    break;
                default: throw new Exception();
            }
            return false;
        }
        static bool Reduce16(InterpretTreeMachine machine)
        {
            ReduceCommon3(machine);
            switch (machine.modeStack.Peek())
            {
                case 0:
                case 18:
                case 21:
                case 23:
                case 26:
                case 32:
                case 50:
                    machine.modeStack.Push(35);
                    break;
                case 28:
                    machine.modeStack.Push(29);
                    break;
                default: throw new Exception();
            }
            return false;
        }
        static bool Reduce17(InterpretTreeMachine machine)
        {
            ReduceCommon1(machine);
            switch (machine.modeStack.Peek())
            {
                case 0:
                case 18:
                case 21:
                case 23:
                case 26:
                case 32:
                case 50:
                    machine.modeStack.Push(35);
                    break;
                case 28:
                    machine.modeStack.Push(29);
                    break;
                default: throw new Exception();
            }
            return false;
        }
        static bool Reduce18(InterpretTreeMachine machine)
        {
            ReduceCommon3_Paren(machine);
            switch (machine.modeStack.Peek())
            {
                case 0:
                case 18:
                case 21:
                case 23:
                case 26:
                case 28:
                case 32:
                case 50:
                    machine.modeStack.Push(8);
                    break;
                case 30:
                    machine.modeStack.Push(31);
                    break;
                default: throw new Exception();
            }
            return false;
        }
        static bool Reduce19_21(InterpretTreeMachine machine)
        {
            ReduceCommon1(machine);
            switch (machine.modeStack.Peek())
            {
                case 0:
                case 18:
                case 21:
                case 23:
                case 26:
                case 28:
                case 32:
                case 50:
                    machine.modeStack.Push(8);
                    break;
                case 30:
                    machine.modeStack.Push(31);
                    break;
                default: throw new Exception();
            }
            return false;
        }
        static bool Reduce22(InterpretTreeMachine machine)
        {
            ReduceCommon3_Paren(machine);
            switch (machine.modeStack.Peek())
            {
                case 0:
                case 21:
                case 23:
                    machine.modeStack.Push(45);
                    break;
                case 18:
                case 50:
                    machine.modeStack.Push(53);
                    break;
                case 26:
                case 28:
                case 30:
                    machine.modeStack.Push(38);
                    break;
                case 32:
                    machine.modeStack.Push(36);
                    break;
                case 40:
                    machine.modeStack.Push(41);
                    break;
                case 43:
                    machine.modeStack.Push(44);
                    break;
                default: throw new Exception();
            }
            return false;
        }
        static bool Reduce23(InterpretTreeMachine machine)
        {
            ReduceCommon1(machine);
            switch (machine.modeStack.Peek())
            {
                case 0:
                case 21:
                case 23:
                    machine.modeStack.Push(45);
                    break;
                case 18:
                case 50:
                    machine.modeStack.Push(53);
                    break;
                case 26:
                case 28:
                case 30:
                    machine.modeStack.Push(38);
                    break;
                case 32:
                    machine.modeStack.Push(36);
                    break;
                case 40:
                    machine.modeStack.Push(41);
                    break;
                case 43:
                    machine.modeStack.Push(44);
                    break;
                default: throw new Exception();
            }
            return false;
        }
        static bool Reduce24(InterpretTreeMachine machine)
        {
            ReduceCommon3_Paren(machine);
            switch (machine.modeStack.Peek())
            {
                case 0:
                case 18:
                case 21:
                case 23:
                    machine.modeStack.Push(9);
                    break;
                case 10:
                    machine.modeStack.Push(11);
                    break;
                case 12:
                case 43:
                    machine.modeStack.Push(14);
                    break;
                case 40:
                    machine.modeStack.Push(42);
                    break;
                case 50:
                    machine.modeStack.Push(52);
                    break;
                default: throw new Exception();
            }
            return false;
        }
        static bool Reduce25_27(InterpretTreeMachine machine)
        {
            ReduceCommon1(machine);
            switch (machine.modeStack.Peek())
            {
                case 0:
                case 18:
                case 21:
                case 23:
                    machine.modeStack.Push(9);
                    break;
                case 10:
                    machine.modeStack.Push(11);
                    break;
                case 12:
                case 43:
                    machine.modeStack.Push(14);
                    break;
                case 40:
                    machine.modeStack.Push(42);
                    break;
                case 50:
                    machine.modeStack.Push(52);
                    break;
                default: throw new Exception();
            }
            return false;
        }
        static bool Reduce28(InterpretTreeMachine machine)
        {
            ReduceCommon3_Paren(machine);
            switch (machine.modeStack.Peek())
            {
                case 0:
                case 23:
                    machine.modeStack.Push(39);
                    break;
                case 10:
                case 40:
                    machine.modeStack.Push(13);
                    break;
                case 12:
                case 43:
                    machine.modeStack.Push(15);
                    break;
                case 18:
                case 50:
                    machine.modeStack.Push(55);
                    break;
                case 21:
                    machine.modeStack.Push(56);
                    break;
                case 46:
                    machine.modeStack.Push(47);
                    break;
                case 48:
                    machine.modeStack.Push(49);
                    break;
                default: throw new Exception();
            }
            return false;
        }
        static bool Reduce29(InterpretTreeMachine machine)
        {
            ReduceCommon1(machine);
            switch (machine.modeStack.Peek())
            {
                case 0:
                case 23:
                    machine.modeStack.Push(39);
                    break;
                case 10:
                case 40:
                    machine.modeStack.Push(13);
                    break;
                case 12:
                case 43:
                    machine.modeStack.Push(15);
                    break;
                case 18:
                case 50:
                    machine.modeStack.Push(55);
                    break;
                case 21:
                    machine.modeStack.Push(56);
                    break;
                case 46:
                    machine.modeStack.Push(47);
                    break;
                case 48:
                    machine.modeStack.Push(49);
                    break;
                default: throw new Exception();
            }
            return false;
        }

        static readonly Func<InterpretTreeMachine, bool>[][] Table = new Func<InterpretTreeMachine, bool>[60][];

        static bool Accept(InterpretTreeMachine machine) => true;

        static bool SpecialStatic1(InterpretTreeMachine machine)
        {
            var second = machine.treeStack.Pop();
            machine.modeStack.Pop();
            machine.modeStack.Pop();
            second.Children[0] = machine.treeStack.Pop();
            second.Children[1] = Convert(machine.input[machine.location]);
            ++machine.location;
            machine.treeStack.Push(second);
            switch (machine.modeStack.Peek())
            {
                case 0:
                case 18:
                case 21:
                case 50:
                    machine.modeStack.Push(7);
                    break;
                case 23:
                    machine.modeStack.Push(24);
                    break;
                default: throw new Exception();
            }
            return false;
        }
        static bool SpecialStatic2(InterpretTreeMachine machine)
        {
            machine.modeStack.Pop();
            machine.modeStack.Pop();
            machine.modeStack.Push(58);
            machine.modeStack.Push(26);
            return false;
        }
        static InterpretTreeMachine()
        {
            for (int i = 0; i < Table.Length; i++)
            {
                Table[i] = new Func<InterpretTreeMachine, bool>[14];
                switch (i)
                {
                    case 0:
                    case 18:
                    case 21:
                    case 23:
                        for (int j = 0; j < 6; j++)
                            Table[i][j] = ShiftGen(j + 1);
                        Table[i][6] = ShiftGen(18);
                        break;
                    case 1:
                    case 2:
                        for (int j = 7; j < Table[i].Length; j++)
                            Table[i][j] = Reduce19_21;
                        break;
                    case 3:
                        for (int j = 7; j < Table[i].Length; j++)
                            Table[i][j] = Reduce23;
                        break;
                    case 4:
                    case 5:
                        for (int j = 7; j < Table[i].Length; j++)
                            Table[i][j] = Reduce25_27;
                        break;
                    case 6:
                        for (int j = 7; j < Table[i].Length; j++)
                            Table[i][j] = Reduce29;
                        break;
                    case 7:
                        for (int j = 7; j < Table[i].Length; j++)
                            Table[i][j] = Reduce7;
                        break;
                    case 8:
                        for (int j = 7; j < Table[i].Length; j++)
                            Table[i][j] = Reduce17;
                        break;
                    case 9:
                        Table[i][10] = ShiftGen(10);
                        break;
                    case 10:
                    case 12:
                        for (int j = 3; j < 6; j++)
                            Table[i][j] = ShiftGen(j + 1);
                        Table[i][6] = ShiftGen(12);
                        break;
                    case 11:
                        for (int j = 7; j < Table[i].Length; j++)
                            Table[i][j] = Reduce8_12;
                        for (int j = 8; j < 11; j++)
                            Table[i][j] = null;
                        break;
                    case 13:
                        for (int j = 7; j < Table[i].Length; j++)
                            Table[i][j] = Reduce25_27;
                        Table[i][8] = null;
                        Table[i][9] = null;
                        break;
                    case 14:
                        Table[i][7] = ShiftGen(16);
                        break;
                    case 15:
                        Table[i][7] = ShiftGen(17);
                        for (int j = 10; j < Table[i].Length; j++)
                            Table[i][j] = Reduce25_27;
                        break;
                    case 16:
                        for (int j = 7; j < Table[i].Length; j++)
                            Table[i][j] = Reduce24;
                        Table[i][8] = null;
                        Table[i][9] = null;
                        break;
                    case 17:
                        for (int j = 7; j < Table[i].Length; j++)
                            Table[i][j] = Reduce28;
                        Table[i][8] = null;
                        Table[i][9] = null;
                        break;
                    case 19:
                        Table[i][7] = ShiftGen(20);
                        Table[i][12] = ShiftGen(21);
                        break;
                    case 20:
                        for (int j = 7; j < Table[i].Length; j++)
                            Table[i][j] = Reduce13;
                        Table[i][8] = null;
                        Table[i][9] = null;
                        break;
                    case 22:
                        Table[i][7] = Reduce4;
                        Table[i][11] = ShiftGen(23);
                        Table[i][12] = Reduce4;
                        Table[i][13] = Reduce4;
                        break;
                    case 24:
                        for (int j = 7; j < Table[i].Length; j++)
                            Table[i][j] = Reduce6;
                        Table[i][8] = null;
                        Table[i][9] = null;
                        Table[i][10] = null;
                        break;
                    case 25:
                        Table[i][9] = ShiftGen(28);
                        Table[i][10] = ShiftGen(26);
                        break;
                    case 26:
                    case 28:
                    case 30:
                    case 32:
                        for (int j = 0; j < 3; j++)
                            Table[i][j] = ShiftGen(j + 1);
                        Table[i][6] = ShiftGen(32);
                        break;
                    case 27:
                        Table[i][7] = Reduce8_12;
                        Table[i][9] = ShiftGen(28);
                        Table[i][11] = Reduce8_12;
                        Table[i][12] = Reduce8_12;
                        Table[i][13] = Reduce8_12;
                        break;
                    case 29:
                        for (int j = 7; j < Table[i].Length; j++)
                            Table[i][j] = Reduce14;
                        Table[i][8] = ShiftGen(30);
                        break;
                    case 31:
                        for (int j = 7; j < Table[i].Length; j++)
                            Table[i][j] = Reduce16;
                        break;
                    case 33:
                        Table[i][7] = ShiftGen(34);
                        Table[i][9] = ShiftGen(28);
                        break;
                    case 34:
                        for (int j = 7; j < Table[i].Length; j++)
                            Table[i][j] = Reduce18;
                        break;
                    case 35:
                        Table[i][7] = Reduce15;
                        Table[i][8] = ShiftGen(30);
                        for (int j = 9; j < Table[i].Length; j++)
                            Table[i][j] = Reduce15;
                        break;
                    case 36:
                        Table[i][7] = ShiftGen(37);
                        for (int j = 8; j < Table[i].Length; j++)
                            Table[i][j] = Reduce19_21;
                        break;
                    case 37:
                        for (int j = 7; j < Table[i].Length; j++)
                            Table[i][j] = Reduce22;
                        break;
                    case 38:
                        for (int j = 7; j < Table[i].Length; j++)
                            Table[i][j] = Reduce19_21;
                        break;
                    case 39:
                        Table[i][7] = Reduce25_27;
                        Table[i][10] = ShiftGen(40);
                        for (int j = 11; j < Table[i].Length; j++)
                            Table[i][j] = Reduce25_27;
                        break;
                    case 40:
                    case 43:
                        for (int j = 2; j < 6; j++)
                            Table[i][j] = ShiftGen(j + 1);
                        Table[i][6] = ShiftGen(43);
                        break;
                    case 41:
                    case 42:
                    case 47:
                        Table[i][7] = Reduce8_12;
                        Table[i][11] = Reduce8_12;
                        Table[i][12] = Reduce8_12;
                        Table[i][13] = Reduce8_12;
                        break;
                    case 44:
                        Table[i][7] = ShiftGen(37);
                        break;
                    case 45:
                        for (int j = 7; j < Table[i].Length; j++)
                            Table[i][j] = Reduce19_21;
                        Table[i][10] = ShiftGen(46);
                        break;
                    case 46:
                        Table[i][0] = SpecialStatic2;
                        Table[i][1] = SpecialStatic2;
                        Table[i][2] = SpecialStatic1;
                        Table[i][5] = ShiftGen(6);
                        Table[i][6] = ShiftGen(48);
                        break;
                    case 48:
                        Table[i][5] = ShiftGen(6);
                        Table[i][6] = ShiftGen(48);
                        break;
                    case 49:
                        Table[i][7] = ShiftGen(17);
                        break;
                    case 50:
                        for (int j = 0; j < 6; j++)
                            Table[i][j] = ShiftGen(j + 1);
                        Table[i][7] = ShiftGen(50);
                        break;
                    case 51:
                        Table[i][7] = Reduce5;
                        Table[i][11] = ShiftGen(23);
                        Table[i][12] = Reduce5;
                        Table[i][13] = Reduce5;
                        break;
                    case 52:
                        Table[i][7] = ShiftGen(16);
                        Table[i][10] = ShiftGen(10);
                        break;
                    case 53:
                        Table[i][7] = ShiftGen(37);
                        Table[i][8] = Reduce19_21;
                        Table[i][9] = Reduce19_21;
                        Table[i][10] = ShiftGen(46);
                        Table[i][11] = Reduce19_21;
                        Table[i][12] = Reduce19_21;
                        Table[i][13] = Reduce19_21;
                        break;
                    case 54:
                        Table[i][7] = ShiftGen(34);
                        Table[i][9] = ShiftGen(28);
                        Table[i][10] = ShiftGen(26);
                        break;
                    case 55:
                        Table[i][7] = ShiftGen(17);
                        Table[i][10] = ShiftGen(40);
                        Table[i][11] = Reduce25_27;
                        Table[i][12] = Reduce25_27;
                        Table[i][13] = Reduce25_27;
                        break;
                    case 56:
                        Table[i][10] = ShiftGen(40);
                        break;
                    case 57:
                        Table[i][7] = Reduce2_3;
                        Table[i][11] = Reduce2_3;
                        Table[i][12] = ShiftGen(21);
                        Table[i][13] = Reduce2_3;
                        break;
                    case 58:
                        Table[i][7] = Reduce2_3;
                        Table[i][8] = Reduce2_3;
                        Table[i][9] = ShiftGen(28);
                        Table[i][10] = ShiftGen(26);
                        Table[i][13] = Reduce2_3;
                        break;
                    case 59:
                        Table[i][13] = Accept;
                        break;
                }
            }
        }
    }
}