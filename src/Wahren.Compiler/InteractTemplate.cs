namespace Wahren.Compiler;

public partial class Program
{
    private const string StructureKinds = "unit, class, spot, power, skill, skillset, event, scenario, story, race, movetype, field, object, dungeon, num, str, gnum, gstr";
	
    private static void ProcessCommandReferences(Project project, ReadOnlySpan<char> readOnlySpan, ColorTheme? theme)
    {
#if JAPANESE
        const string NotFound = "見つかりませんでした。";
#else
        const string NotFound = "Not Found.";
#endif
        bool @switch = false;
        var name = readOnlySpan.Slice(readOnlySpan.LastIndexOf(' ') + 1);
        var analysisFiles = project.FileAnalysisList.AsSpan();
        var files = project.Files.AsSpan();
        if (readOnlySpan.StartsWith("num "))
        {
            bool any = false;
            for (int i = 0; i < analysisFiles.Length; ++i)
            {
                ref var set = ref analysisFiles[i].NumberVariableReaderSet;
                ref var file = ref files[i];
                ref var tokenList = ref file.TokenList;
                if (set.TryGet(name, out var id))
                {
                    any = true;
                    ref var references = ref set.References[id];
                    foreach (var reference in references.AsSpan())
                    {
                        if (theme is not null)
                        {
                            @switch = !@switch;
                            Console.ForegroundColor = @switch ? theme.Normal0 : theme.Normal1;
                        }
                        Console.WriteLine($"{file.FilePath}({tokenList.GetLine(reference) + 1}, {tokenList.GetOffset(reference) + 1}) R");
                    }
                }
                set = ref analysisFiles[i].NumberVariableWriterSet;
                if (set.TryGet(name, out id))
                {
                    any = true;
                    ref var references = ref set.References[id];
                    foreach (var reference in references.AsSpan())
                    {
                        if (theme is not null)
                        {
                            @switch = !@switch;
                            Console.ForegroundColor = @switch ? theme.Normal0 : theme.Normal1;
                        }
                        Console.WriteLine($"{file.FilePath}({tokenList.GetLine(reference) + 1}, {tokenList.GetOffset(reference) + 1}) W");
                    }
                }
            }

            if (!any)
            {
                if (theme is not null)
                {
                    Console.ForegroundColor = theme.Error;
                }
                Console.WriteLine(NotFound);
            }
        }
        else if (readOnlySpan.StartsWith("str "))
        {
            bool any = false;
            for (int i = 0; i < analysisFiles.Length; ++i)
            {
                ref var set = ref analysisFiles[i].StringVariableReaderSet;
                ref var file = ref files[i];
                ref var tokenList = ref file.TokenList;
                if (set.TryGet(name, out var id))
                {
                    any = true;
                    ref var references = ref set.References[id];
                    foreach (var reference in references.AsSpan())
                    {
                        if (theme is not null)
                        {
                            @switch = !@switch;
                            Console.ForegroundColor = @switch ? theme.Normal0 : theme.Normal1;
                        }
                        Console.WriteLine($"{file.FilePath}({tokenList.GetLine(reference) + 1}, {tokenList.GetOffset(reference) + 1}) R");
                    }
                }
                set = ref analysisFiles[i].StringVariableWriterSet;
                if (set.TryGet(name, out id))
                {
                    any = true;
                    ref var references = ref set.References[id];
                    foreach (var reference in references.AsSpan())
                    {
                        if (theme is not null)
                        {
                            @switch = !@switch;
                            Console.ForegroundColor = @switch ? theme.Normal0 : theme.Normal1;
                        }
                        Console.WriteLine($"{file.FilePath}({tokenList.GetLine(reference) + 1}, {tokenList.GetOffset(reference) + 1}) W");
                    }
                }
            }

            if (!any)
            {
                if (theme is not null)
                {
                    Console.ForegroundColor = theme.Error;
                }
                Console.WriteLine(NotFound);
            }
        }
        else if (readOnlySpan.StartsWith("gnum "))
        {
            bool any = false;
            for (int i = 0; i < analysisFiles.Length; ++i)
            {
                ref var set = ref analysisFiles[i].GlobalVariableReaderSet;
                ref var file = ref files[i];
                ref var tokenList = ref file.TokenList;
                if (set.TryGet(name, out var id))
                {
                    any = true;
                    ref var references = ref set.References[id];
                    foreach (var reference in references.AsSpan())
                    {
                        if (theme is not null)
                        {
                            @switch = !@switch;
                            Console.ForegroundColor = @switch ? theme.Normal0 : theme.Normal1;
                        }
                        Console.WriteLine($"{file.FilePath}({tokenList.GetLine(reference) + 1}, {tokenList.GetOffset(reference) + 1}) R");
                    }
                }
                set = ref analysisFiles[i].GlobalVariableWriterSet;
                if (set.TryGet(name, out id))
                {
                    any = true;
                    ref var references = ref set.References[id];
                    foreach (var reference in references.AsSpan())
                    {
                        if (theme is not null)
                        {
                            @switch = !@switch;
                            Console.ForegroundColor = @switch ? theme.Normal0 : theme.Normal1;
                        }
                        Console.WriteLine($"{file.FilePath}({tokenList.GetLine(reference) + 1}, {tokenList.GetOffset(reference) + 1}) W");
                    }
                }
            }

            if (!any)
            {
                if (theme is not null)
                {
                    Console.ForegroundColor = theme.Error;
                }
                Console.WriteLine(NotFound);
            }
        }
        else if (readOnlySpan.StartsWith("gstr "))
        {
            bool any = false;
            for (int i = 0; i < analysisFiles.Length; ++i)
            {
                ref var set = ref analysisFiles[i].GlobalStringVariableReaderSet;
                ref var file = ref files[i];
                ref var tokenList = ref file.TokenList;
                if (set.TryGet(name, out var id))
                {
                    any = true;
                    ref var references = ref set.References[id];
                    foreach (var reference in references.AsSpan())
                    {
                        if (theme is not null)
                        {
                            @switch = !@switch;
                            Console.ForegroundColor = @switch ? theme.Normal0 : theme.Normal1;
                        }
                        Console.WriteLine($"{file.FilePath}({tokenList.GetLine(reference) + 1}, {tokenList.GetOffset(reference) + 1}) R");
                    }
                }
                set = ref analysisFiles[i].GlobalStringVariableWriterSet;
                if (set.TryGet(name, out id))
                {
                    any = true;
                    ref var references = ref set.References[id];
                    foreach (var reference in references.AsSpan())
                    {
                        if (theme is not null)
                        {
                            @switch = !@switch;
                            Console.ForegroundColor = @switch ? theme.Normal0 : theme.Normal1;
                        }
                        Console.WriteLine($"{file.FilePath}({tokenList.GetLine(reference) + 1}, {tokenList.GetOffset(reference) + 1}) W");
                    }
                }
            }

            if (!any)
            {
                if (theme is not null)
                {
                    Console.ForegroundColor = theme.Error;
                }
                Console.WriteLine(NotFound);
            }
        }
        else if (readOnlySpan.StartsWith("unit "))
        {
            bool any = false;
            for (int i = 0; i < analysisFiles.Length; ++i)
            {
                ref var set = ref analysisFiles[i].UnitSet;
                ref var file = ref files[i];
                ref var tokenList = ref file.TokenList;
                if (set.TryGet(name, out var id))
                {
                    any = true;
                    ref var references = ref set.References[id];
                    foreach (var reference in references.AsSpan())
                    {
                        if (theme is not null)
                        {
                            @switch = !@switch;
                            Console.ForegroundColor = @switch ? theme.Normal0 : theme.Normal1;
                        }
                        Console.WriteLine($"{file.FilePath}({tokenList.GetLine(reference) + 1}, {tokenList.GetOffset(reference) + 1})");
                    }
                }
            }

            if (!any)
            {
                if (theme is not null)
                {
                    Console.ForegroundColor = theme.Error;
                }
                Console.WriteLine(NotFound);
            }
        }
        else if (readOnlySpan.StartsWith("class "))
        {
            bool any = false;
            for (int i = 0; i < analysisFiles.Length; ++i)
            {
                ref var set = ref analysisFiles[i].ClassSet;
                ref var file = ref files[i];
                ref var tokenList = ref file.TokenList;
                if (set.TryGet(name, out var id))
                {
                    any = true;
                    ref var references = ref set.References[id];
                    foreach (var reference in references.AsSpan())
                    {
                        if (theme is not null)
                        {
                            @switch = !@switch;
                            Console.ForegroundColor = @switch ? theme.Normal0 : theme.Normal1;
                        }
                        Console.WriteLine($"{file.FilePath}({tokenList.GetLine(reference) + 1}, {tokenList.GetOffset(reference) + 1})");
                    }
                }
            }

            if (!any)
            {
                if (theme is not null)
                {
                    Console.ForegroundColor = theme.Error;
                }
                Console.WriteLine(NotFound);
            }
        }
        else if (readOnlySpan.StartsWith("spot "))
        {
            bool any = false;
            for (int i = 0; i < analysisFiles.Length; ++i)
            {
                ref var set = ref analysisFiles[i].SpotSet;
                ref var file = ref files[i];
                ref var tokenList = ref file.TokenList;
                if (set.TryGet(name, out var id))
                {
                    any = true;
                    ref var references = ref set.References[id];
                    foreach (var reference in references.AsSpan())
                    {
                        if (theme is not null)
                        {
                            @switch = !@switch;
                            Console.ForegroundColor = @switch ? theme.Normal0 : theme.Normal1;
                        }
                        Console.WriteLine($"{file.FilePath}({tokenList.GetLine(reference) + 1}, {tokenList.GetOffset(reference) + 1})");
                    }
                }
            }

            if (!any)
            {
                if (theme is not null)
                {
                    Console.ForegroundColor = theme.Error;
                }
                Console.WriteLine(NotFound);
            }
        }
        else if (readOnlySpan.StartsWith("power "))
        {
            bool any = false;
            for (int i = 0; i < analysisFiles.Length; ++i)
            {
                ref var set = ref analysisFiles[i].PowerSet;
                ref var file = ref files[i];
                ref var tokenList = ref file.TokenList;
                if (set.TryGet(name, out var id))
                {
                    any = true;
                    ref var references = ref set.References[id];
                    foreach (var reference in references.AsSpan())
                    {
                        if (theme is not null)
                        {
                            @switch = !@switch;
                            Console.ForegroundColor = @switch ? theme.Normal0 : theme.Normal1;
                        }
                        Console.WriteLine($"{file.FilePath}({tokenList.GetLine(reference) + 1}, {tokenList.GetOffset(reference) + 1})");
                    }
                }
            }

            if (!any)
            {
                if (theme is not null)
                {
                    Console.ForegroundColor = theme.Error;
                }
                Console.WriteLine(NotFound);
            }
        }
        else if (readOnlySpan.StartsWith("skill "))
        {
            bool any = false;
            for (int i = 0; i < analysisFiles.Length; ++i)
            {
                ref var set = ref analysisFiles[i].SkillSet;
                ref var file = ref files[i];
                ref var tokenList = ref file.TokenList;
                if (set.TryGet(name, out var id))
                {
                    any = true;
                    ref var references = ref set.References[id];
                    foreach (var reference in references.AsSpan())
                    {
                        if (theme is not null)
                        {
                            @switch = !@switch;
                            Console.ForegroundColor = @switch ? theme.Normal0 : theme.Normal1;
                        }
                        Console.WriteLine($"{file.FilePath}({tokenList.GetLine(reference) + 1}, {tokenList.GetOffset(reference) + 1})");
                    }
                }
            }

            if (!any)
            {
                if (theme is not null)
                {
                    Console.ForegroundColor = theme.Error;
                }
                Console.WriteLine(NotFound);
            }
        }
        else if (readOnlySpan.StartsWith("skillset "))
        {
            bool any = false;
            for (int i = 0; i < analysisFiles.Length; ++i)
            {
                ref var set = ref analysisFiles[i].SkillsetSet;
                ref var file = ref files[i];
                ref var tokenList = ref file.TokenList;
                if (set.TryGet(name, out var id))
                {
                    any = true;
                    ref var references = ref set.References[id];
                    foreach (var reference in references.AsSpan())
                    {
                        if (theme is not null)
                        {
                            @switch = !@switch;
                            Console.ForegroundColor = @switch ? theme.Normal0 : theme.Normal1;
                        }
                        Console.WriteLine($"{file.FilePath}({tokenList.GetLine(reference) + 1}, {tokenList.GetOffset(reference) + 1})");
                    }
                }
            }

            if (!any)
            {
                if (theme is not null)
                {
                    Console.ForegroundColor = theme.Error;
                }
                Console.WriteLine(NotFound);
            }
        }
        else if (readOnlySpan.StartsWith("event "))
        {
            bool any = false;
            for (int i = 0; i < analysisFiles.Length; ++i)
            {
                ref var set = ref analysisFiles[i].EventSet;
                ref var file = ref files[i];
                ref var tokenList = ref file.TokenList;
                if (set.TryGet(name, out var id))
                {
                    any = true;
                    ref var references = ref set.References[id];
                    foreach (var reference in references.AsSpan())
                    {
                        if (theme is not null)
                        {
                            @switch = !@switch;
                            Console.ForegroundColor = @switch ? theme.Normal0 : theme.Normal1;
                        }
                        Console.WriteLine($"{file.FilePath}({tokenList.GetLine(reference) + 1}, {tokenList.GetOffset(reference) + 1})");
                    }
                }
            }

            if (!any)
            {
                if (theme is not null)
                {
                    Console.ForegroundColor = theme.Error;
                }
                Console.WriteLine(NotFound);
            }
        }
        else if (readOnlySpan.StartsWith("scenario "))
        {
            bool any = false;
            for (int i = 0; i < analysisFiles.Length; ++i)
            {
                ref var set = ref analysisFiles[i].ScenarioSet;
                ref var file = ref files[i];
                ref var tokenList = ref file.TokenList;
                if (set.TryGet(name, out var id))
                {
                    any = true;
                    ref var references = ref set.References[id];
                    foreach (var reference in references.AsSpan())
                    {
                        if (theme is not null)
                        {
                            @switch = !@switch;
                            Console.ForegroundColor = @switch ? theme.Normal0 : theme.Normal1;
                        }
                        Console.WriteLine($"{file.FilePath}({tokenList.GetLine(reference) + 1}, {tokenList.GetOffset(reference) + 1})");
                    }
                }
            }

            if (!any)
            {
                if (theme is not null)
                {
                    Console.ForegroundColor = theme.Error;
                }
                Console.WriteLine(NotFound);
            }
        }
        else if (readOnlySpan.StartsWith("story "))
        {
            bool any = false;
            for (int i = 0; i < analysisFiles.Length; ++i)
            {
                ref var set = ref analysisFiles[i].StorySet;
                ref var file = ref files[i];
                ref var tokenList = ref file.TokenList;
                if (set.TryGet(name, out var id))
                {
                    any = true;
                    ref var references = ref set.References[id];
                    foreach (var reference in references.AsSpan())
                    {
                        if (theme is not null)
                        {
                            @switch = !@switch;
                            Console.ForegroundColor = @switch ? theme.Normal0 : theme.Normal1;
                        }
                        Console.WriteLine($"{file.FilePath}({tokenList.GetLine(reference) + 1}, {tokenList.GetOffset(reference) + 1})");
                    }
                }
            }

            if (!any)
            {
                if (theme is not null)
                {
                    Console.ForegroundColor = theme.Error;
                }
                Console.WriteLine(NotFound);
            }
        }
        else if (readOnlySpan.StartsWith("race "))
        {
            bool any = false;
            for (int i = 0; i < analysisFiles.Length; ++i)
            {
                ref var set = ref analysisFiles[i].RaceSet;
                ref var file = ref files[i];
                ref var tokenList = ref file.TokenList;
                if (set.TryGet(name, out var id))
                {
                    any = true;
                    ref var references = ref set.References[id];
                    foreach (var reference in references.AsSpan())
                    {
                        if (theme is not null)
                        {
                            @switch = !@switch;
                            Console.ForegroundColor = @switch ? theme.Normal0 : theme.Normal1;
                        }
                        Console.WriteLine($"{file.FilePath}({tokenList.GetLine(reference) + 1}, {tokenList.GetOffset(reference) + 1})");
                    }
                }
            }

            if (!any)
            {
                if (theme is not null)
                {
                    Console.ForegroundColor = theme.Error;
                }
                Console.WriteLine(NotFound);
            }
        }
        else if (readOnlySpan.StartsWith("movetype "))
        {
            bool any = false;
            for (int i = 0; i < analysisFiles.Length; ++i)
            {
                ref var set = ref analysisFiles[i].MovetypeSet;
                ref var file = ref files[i];
                ref var tokenList = ref file.TokenList;
                if (set.TryGet(name, out var id))
                {
                    any = true;
                    ref var references = ref set.References[id];
                    foreach (var reference in references.AsSpan())
                    {
                        if (theme is not null)
                        {
                            @switch = !@switch;
                            Console.ForegroundColor = @switch ? theme.Normal0 : theme.Normal1;
                        }
                        Console.WriteLine($"{file.FilePath}({tokenList.GetLine(reference) + 1}, {tokenList.GetOffset(reference) + 1})");
                    }
                }
            }

            if (!any)
            {
                if (theme is not null)
                {
                    Console.ForegroundColor = theme.Error;
                }
                Console.WriteLine(NotFound);
            }
        }
        else if (readOnlySpan.StartsWith("field "))
        {
            bool any = false;
            for (int i = 0; i < analysisFiles.Length; ++i)
            {
                ref var set = ref analysisFiles[i].FieldSet;
                ref var file = ref files[i];
                ref var tokenList = ref file.TokenList;
                if (set.TryGet(name, out var id))
                {
                    any = true;
                    ref var references = ref set.References[id];
                    foreach (var reference in references.AsSpan())
                    {
                        if (theme is not null)
                        {
                            @switch = !@switch;
                            Console.ForegroundColor = @switch ? theme.Normal0 : theme.Normal1;
                        }
                        Console.WriteLine($"{file.FilePath}({tokenList.GetLine(reference) + 1}, {tokenList.GetOffset(reference) + 1})");
                    }
                }
            }

            if (!any)
            {
                if (theme is not null)
                {
                    Console.ForegroundColor = theme.Error;
                }
                Console.WriteLine(NotFound);
            }
        }
        else if (readOnlySpan.StartsWith("object "))
        {
            bool any = false;
            for (int i = 0; i < analysisFiles.Length; ++i)
            {
                ref var set = ref analysisFiles[i].ObjectSet;
                ref var file = ref files[i];
                ref var tokenList = ref file.TokenList;
                if (set.TryGet(name, out var id))
                {
                    any = true;
                    ref var references = ref set.References[id];
                    foreach (var reference in references.AsSpan())
                    {
                        if (theme is not null)
                        {
                            @switch = !@switch;
                            Console.ForegroundColor = @switch ? theme.Normal0 : theme.Normal1;
                        }
                        Console.WriteLine($"{file.FilePath}({tokenList.GetLine(reference) + 1}, {tokenList.GetOffset(reference) + 1})");
                    }
                }
            }

            if (!any)
            {
                if (theme is not null)
                {
                    Console.ForegroundColor = theme.Error;
                }
                Console.WriteLine(NotFound);
            }
        }
        else if (readOnlySpan.StartsWith("dungeon "))
        {
            bool any = false;
            for (int i = 0; i < analysisFiles.Length; ++i)
            {
                ref var set = ref analysisFiles[i].DungeonSet;
                ref var file = ref files[i];
                ref var tokenList = ref file.TokenList;
                if (set.TryGet(name, out var id))
                {
                    any = true;
                    ref var references = ref set.References[id];
                    foreach (var reference in references.AsSpan())
                    {
                        if (theme is not null)
                        {
                            @switch = !@switch;
                            Console.ForegroundColor = @switch ? theme.Normal0 : theme.Normal1;
                        }
                        Console.WriteLine($"{file.FilePath}({tokenList.GetLine(reference) + 1}, {tokenList.GetOffset(reference) + 1})");
                    }
                }
            }

            if (!any)
            {
                if (theme is not null)
                {
                    Console.ForegroundColor = theme.Error;
                }
                Console.WriteLine(NotFound);
            }
        }
        if (theme is not null)
        {
            Console.ResetColor();
        }
    }

    private static void ProcessCommandListStructure(Project project, ReadOnlySpan<char> readOnlySpan, ColorTheme? theme)
    {
        System.Collections.Generic.HashSet<string> set = new();
        if (readOnlySpan.SequenceEqual("num"))
        {
            foreach (ref var file in project.FileAnalysisList.AsSpan())
            {
                ref var variableSet = ref file.NumberVariableWriterSet;
                for (uint i = 0, end = variableSet.Count; i < end; ++i)
                {
                    set.Add(new string(variableSet[i]));
                }
                variableSet = ref file.NumberVariableReaderSet;
                for (uint i = 0, end = variableSet.Count; i < end; ++i)
                {
                    set.Add(new string(variableSet[i]));
                }
            }
        }
        else if (readOnlySpan.SequenceEqual("str"))
        {
            foreach (ref var file in project.FileAnalysisList.AsSpan())
            {
                ref var variableSet = ref file.StringVariableWriterSet;
                for (uint i = 0, end = variableSet.Count; i < end; ++i)
                {
                    set.Add(new string(variableSet[i]));
                }
                variableSet = ref file.StringVariableReaderSet;
                for (uint i = 0, end = variableSet.Count; i < end; ++i)
                {
                    set.Add(new string(variableSet[i]));
                }
            }
        }
        else if (readOnlySpan.SequenceEqual("gnum"))
        {
            foreach (ref var file in project.FileAnalysisList.AsSpan())
            {
                ref var variableSet = ref file.GlobalVariableWriterSet;
                for (uint i = 0, end = variableSet.Count; i < end; ++i)
                {
                    set.Add(new string(variableSet[i]));
                }
                variableSet = ref file.GlobalVariableReaderSet;
                for (uint i = 0, end = variableSet.Count; i < end; ++i)
                {
                    set.Add(new string(variableSet[i]));
                }
            }
        }
        else if (readOnlySpan.SequenceEqual("gstr"))
        {
            foreach (ref var file in project.FileAnalysisList.AsSpan())
            {
                ref var variableSet = ref file.GlobalStringVariableWriterSet;
                for (uint i = 0, end = variableSet.Count; i < end; ++i)
                {
                    set.Add(new string(variableSet[i]));
                }
                variableSet = ref file.GlobalStringVariableReaderSet;
                for (uint i = 0, end = variableSet.Count; i < end; ++i)
                {
                    set.Add(new string(variableSet[i]));
                }
            }
        }
        else if (readOnlySpan.SequenceEqual("unit"))
        {
            foreach (ref var file in project.Files.AsSpan())
            {
                foreach (ref var node in file.UnitNodeList.AsSpan())
                {
                    set.Add(new string(file.GetSpan(node.Name)));
                }
            }
        }
        else if (readOnlySpan.SequenceEqual("class"))
        {
            foreach (ref var file in project.Files.AsSpan())
            {
                foreach (ref var node in file.ClassNodeList.AsSpan())
                {
                    set.Add(new string(file.GetSpan(node.Name)));
                }
            }
        }
        else if (readOnlySpan.SequenceEqual("spot"))
        {
            foreach (ref var file in project.Files.AsSpan())
            {
                foreach (ref var node in file.SpotNodeList.AsSpan())
                {
                    set.Add(new string(file.GetSpan(node.Name)));
                }
            }
        }
        else if (readOnlySpan.SequenceEqual("power"))
        {
            foreach (ref var file in project.Files.AsSpan())
            {
                foreach (ref var node in file.PowerNodeList.AsSpan())
                {
                    set.Add(new string(file.GetSpan(node.Name)));
                }
            }
        }
        else if (readOnlySpan.SequenceEqual("skill"))
        {
            foreach (ref var file in project.Files.AsSpan())
            {
                foreach (ref var node in file.SkillNodeList.AsSpan())
                {
                    set.Add(new string(file.GetSpan(node.Name)));
                }
            }
        }
        else if (readOnlySpan.SequenceEqual("skillset"))
        {
            foreach (ref var file in project.Files.AsSpan())
            {
                foreach (ref var node in file.SkillsetNodeList.AsSpan())
                {
                    set.Add(new string(file.GetSpan(node.Name)));
                }
            }
        }
        else if (readOnlySpan.SequenceEqual("event"))
        {
            foreach (ref var file in project.Files.AsSpan())
            {
                foreach (ref var node in file.EventNodeList.AsSpan())
                {
                    set.Add(new string(file.GetSpan(node.Name)));
                }
            }
        }
        else if (readOnlySpan.SequenceEqual("scenario"))
        {
            foreach (ref var file in project.Files.AsSpan())
            {
                foreach (ref var node in file.ScenarioNodeList.AsSpan())
                {
                    set.Add(new string(file.GetSpan(node.Name)));
                }
            }
        }
        else if (readOnlySpan.SequenceEqual("story"))
        {
            foreach (ref var file in project.Files.AsSpan())
            {
                foreach (ref var node in file.StoryNodeList.AsSpan())
                {
                    set.Add(new string(file.GetSpan(node.Name)));
                }
            }
        }
        else if (readOnlySpan.SequenceEqual("race"))
        {
            foreach (ref var file in project.Files.AsSpan())
            {
                foreach (ref var node in file.RaceNodeList.AsSpan())
                {
                    set.Add(new string(file.GetSpan(node.Name)));
                }
            }
        }
        else if (readOnlySpan.SequenceEqual("movetype"))
        {
            foreach (ref var file in project.Files.AsSpan())
            {
                foreach (ref var node in file.MovetypeNodeList.AsSpan())
                {
                    set.Add(new string(file.GetSpan(node.Name)));
                }
            }
        }
        else if (readOnlySpan.SequenceEqual("field"))
        {
            foreach (ref var file in project.Files.AsSpan())
            {
                foreach (ref var node in file.FieldNodeList.AsSpan())
                {
                    set.Add(new string(file.GetSpan(node.Name)));
                }
            }
        }
        else if (readOnlySpan.SequenceEqual("object"))
        {
            foreach (ref var file in project.Files.AsSpan())
            {
                foreach (ref var node in file.ObjectNodeList.AsSpan())
                {
                    set.Add(new string(file.GetSpan(node.Name)));
                }
            }
        }
        else if (readOnlySpan.SequenceEqual("dungeon"))
        {
            foreach (ref var file in project.Files.AsSpan())
            {
                foreach (ref var node in file.DungeonNodeList.AsSpan())
                {
                    set.Add(new string(file.GetSpan(node.Name)));
                }
            }
        }
        bool @switch = false;
        foreach (var name in set)
        {
            if (theme is not null)
            {
                @switch = !@switch;
                Console.ForegroundColor = @switch ? theme.Normal0 : theme.Normal1;
            }
            Console.WriteLine(name);
        }
        if (theme is not null)
        {
            Console.ResetColor();
        }
    }
}
