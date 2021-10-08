using Wahren.AbstractSyntaxTree.Parser;

namespace Wahren.AbstractSyntaxTree.Project;

public sealed partial class Project
{
    public bool DetectInfiniteLoop()
    {
        var files = Files.AsSpan();
        DualList<byte> dualList = new();
        const byte Processed = 2;
        const byte Processing = 1;
        const byte NotYet = 0;
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            dualList.AddEmpty();
            var nodes = file.PowerNodeList.AsSpan();
            ref var list = ref dualList.Last;
            list.PrepareAddRange(nodes.Length);
            for (int j = 0; j < nodes.Length; j++)
            {
                ref var node = ref nodes[j];
                list.Add(node.Super.HasValue ? NotYet : Processed);
            }
        }
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            var nodes = file.PowerNodeList.AsSpan();
            ref var superSet = ref file.PowerSet;
            var listSpan = dualList[i].AsSpan();
            for (int j = 0; j < listSpan.Length; j++)
            {
                if (listSpan[j] != NotYet)
                {
                    continue;
                }

                listSpan[j] = Processing;
                ref var node = ref nodes[j];
                var superSpan = superSet[node.Super!.Value];
                ref var superFile = ref TryGetPowerNode(superSpan, out var superNodeIndex);
                do
                {
                    ref var superStatus = ref dualList[superFile.Id][superNodeIndex];
                    if (superStatus == Processed)
                    {
                        break;
                    }

                    if (superStatus == Processing)
                    {
                        file.ErrorAdd_InfiniteLoop("Power", file.GetSpan(node.Name), superSpan, node.Name);
                        return false;
                    }

                    superStatus = Processing;
                    superSpan = superFile.PowerSet[superFile.PowerNodeList[superNodeIndex].Super!.Value];
                    superFile = ref TryGetPowerNode(superSpan, out superNodeIndex);
                } while (true);
            }
        }
        dualList.Dispose();
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            dualList.AddEmpty();
            var nodes = file.ClassNodeList.AsSpan();
            ref var list = ref dualList.Last;
            list.PrepareAddRange(nodes.Length);
            for (int j = 0; j < nodes.Length; j++)
            {
                ref var node = ref nodes[j];
                list.Add(node.Super.HasValue ? NotYet : Processed);
            }
        }
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            var nodes = file.ClassNodeList.AsSpan();
            ref var superSet = ref file.ClassSet;
            var listSpan = dualList[i].AsSpan();
            for (int j = 0; j < listSpan.Length; j++)
            {
                if (listSpan[j] != NotYet)
                {
                    continue;
                }

                listSpan[j] = Processing;
                ref var node = ref nodes[j];
                var superSpan = superSet[node.Super!.Value];
                ref var superFile = ref TryGetClassNode(superSpan, out var superNodeIndex);
                do
                {
                    ref var superStatus = ref dualList[superFile.Id][superNodeIndex];
                    if (superStatus == Processed)
                    {
                        break;
                    }

                    if (superStatus == Processing)
                    {
                        file.ErrorAdd_InfiniteLoop("Class", file.GetSpan(node.Name), superSpan, node.Name);
                        return false;
                    }

                    superStatus = Processing;
                    superSpan = superFile.ClassSet[superFile.ClassNodeList[superNodeIndex].Super!.Value];
                    superFile = ref TryGetClassNode(superSpan, out superNodeIndex);
                } while (true);
            }
        }
        dualList.Dispose();
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            dualList.AddEmpty();
            var nodes = file.DungeonNodeList.AsSpan();
            ref var list = ref dualList.Last;
            list.PrepareAddRange(nodes.Length);
            for (int j = 0; j < nodes.Length; j++)
            {
                ref var node = ref nodes[j];
                list.Add(node.Super.HasValue ? NotYet : Processed);
            }
        }
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            var nodes = file.DungeonNodeList.AsSpan();
            ref var superSet = ref file.DungeonSet;
            var listSpan = dualList[i].AsSpan();
            for (int j = 0; j < listSpan.Length; j++)
            {
                if (listSpan[j] != NotYet)
                {
                    continue;
                }

                listSpan[j] = Processing;
                ref var node = ref nodes[j];
                var superSpan = superSet[node.Super!.Value];
                ref var superFile = ref TryGetDungeonNode(superSpan, out var superNodeIndex);
                do
                {
                    ref var superStatus = ref dualList[superFile.Id][superNodeIndex];
                    if (superStatus == Processed)
                    {
                        break;
                    }

                    if (superStatus == Processing)
                    {
                        file.ErrorAdd_InfiniteLoop("Dungeon", file.GetSpan(node.Name), superSpan, node.Name);
                        return false;
                    }

                    superStatus = Processing;
                    superSpan = superFile.DungeonSet[superFile.DungeonNodeList[superNodeIndex].Super!.Value];
                    superFile = ref TryGetDungeonNode(superSpan, out superNodeIndex);
                } while (true);
            }
        }
        dualList.Dispose();
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            dualList.AddEmpty();
            var nodes = file.FieldNodeList.AsSpan();
            ref var list = ref dualList.Last;
            list.PrepareAddRange(nodes.Length);
            for (int j = 0; j < nodes.Length; j++)
            {
                ref var node = ref nodes[j];
                list.Add(node.Super.HasValue ? NotYet : Processed);
            }
        }
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            var nodes = file.FieldNodeList.AsSpan();
            ref var superSet = ref file.FieldSet;
            var listSpan = dualList[i].AsSpan();
            for (int j = 0; j < listSpan.Length; j++)
            {
                if (listSpan[j] != NotYet)
                {
                    continue;
                }

                listSpan[j] = Processing;
                ref var node = ref nodes[j];
                var superSpan = superSet[node.Super!.Value];
                ref var superFile = ref TryGetFieldNode(superSpan, out var superNodeIndex);
                do
                {
                    ref var superStatus = ref dualList[superFile.Id][superNodeIndex];
                    if (superStatus == Processed)
                    {
                        break;
                    }

                    if (superStatus == Processing)
                    {
                        file.ErrorAdd_InfiniteLoop("Field", file.GetSpan(node.Name), superSpan, node.Name);
                        return false;
                    }

                    superStatus = Processing;
                    superSpan = superFile.FieldSet[superFile.FieldNodeList[superNodeIndex].Super!.Value];
                    superFile = ref TryGetFieldNode(superSpan, out superNodeIndex);
                } while (true);
            }
        }
        dualList.Dispose();
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            dualList.AddEmpty();
            var nodes = file.MovetypeNodeList.AsSpan();
            ref var list = ref dualList.Last;
            list.PrepareAddRange(nodes.Length);
            for (int j = 0; j < nodes.Length; j++)
            {
                ref var node = ref nodes[j];
                list.Add(node.Super.HasValue ? NotYet : Processed);
            }
        }
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            var nodes = file.MovetypeNodeList.AsSpan();
            ref var superSet = ref file.MovetypeSet;
            var listSpan = dualList[i].AsSpan();
            for (int j = 0; j < listSpan.Length; j++)
            {
                if (listSpan[j] != NotYet)
                {
                    continue;
                }

                listSpan[j] = Processing;
                ref var node = ref nodes[j];
                var superSpan = superSet[node.Super!.Value];
                ref var superFile = ref TryGetMovetypeNode(superSpan, out var superNodeIndex);
                do
                {
                    ref var superStatus = ref dualList[superFile.Id][superNodeIndex];
                    if (superStatus == Processed)
                    {
                        break;
                    }

                    if (superStatus == Processing)
                    {
                        file.ErrorAdd_InfiniteLoop("Movetype", file.GetSpan(node.Name), superSpan, node.Name);
                        return false;
                    }

                    superStatus = Processing;
                    superSpan = superFile.MovetypeSet[superFile.MovetypeNodeList[superNodeIndex].Super!.Value];
                    superFile = ref TryGetMovetypeNode(superSpan, out superNodeIndex);
                } while (true);
            }
        }
        dualList.Dispose();
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            dualList.AddEmpty();
            var nodes = file.ObjectNodeList.AsSpan();
            ref var list = ref dualList.Last;
            list.PrepareAddRange(nodes.Length);
            for (int j = 0; j < nodes.Length; j++)
            {
                ref var node = ref nodes[j];
                list.Add(node.Super.HasValue ? NotYet : Processed);
            }
        }
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            var nodes = file.ObjectNodeList.AsSpan();
            ref var superSet = ref file.ObjectSet;
            var listSpan = dualList[i].AsSpan();
            for (int j = 0; j < listSpan.Length; j++)
            {
                if (listSpan[j] != NotYet)
                {
                    continue;
                }

                listSpan[j] = Processing;
                ref var node = ref nodes[j];
                var superSpan = superSet[node.Super!.Value];
                ref var superFile = ref TryGetObjectNode(superSpan, out var superNodeIndex);
                do
                {
                    ref var superStatus = ref dualList[superFile.Id][superNodeIndex];
                    if (superStatus == Processed)
                    {
                        break;
                    }

                    if (superStatus == Processing)
                    {
                        file.ErrorAdd_InfiniteLoop("Object", file.GetSpan(node.Name), superSpan, node.Name);
                        return false;
                    }

                    superStatus = Processing;
                    superSpan = superFile.ObjectSet[superFile.ObjectNodeList[superNodeIndex].Super!.Value];
                    superFile = ref TryGetObjectNode(superSpan, out superNodeIndex);
                } while (true);
            }
        }
        dualList.Dispose();
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            dualList.AddEmpty();
            var nodes = file.RaceNodeList.AsSpan();
            ref var list = ref dualList.Last;
            list.PrepareAddRange(nodes.Length);
            for (int j = 0; j < nodes.Length; j++)
            {
                ref var node = ref nodes[j];
                list.Add(node.Super.HasValue ? NotYet : Processed);
            }
        }
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            var nodes = file.RaceNodeList.AsSpan();
            ref var superSet = ref file.RaceSet;
            var listSpan = dualList[i].AsSpan();
            for (int j = 0; j < listSpan.Length; j++)
            {
                if (listSpan[j] != NotYet)
                {
                    continue;
                }

                listSpan[j] = Processing;
                ref var node = ref nodes[j];
                var superSpan = superSet[node.Super!.Value];
                ref var superFile = ref TryGetRaceNode(superSpan, out var superNodeIndex);
                do
                {
                    ref var superStatus = ref dualList[superFile.Id][superNodeIndex];
                    if (superStatus == Processed)
                    {
                        break;
                    }

                    if (superStatus == Processing)
                    {
                        file.ErrorAdd_InfiniteLoop("Race", file.GetSpan(node.Name), superSpan, node.Name);
                        return false;
                    }

                    superStatus = Processing;
                    superSpan = superFile.RaceSet[superFile.RaceNodeList[superNodeIndex].Super!.Value];
                    superFile = ref TryGetRaceNode(superSpan, out superNodeIndex);
                } while (true);
            }
        }
        dualList.Dispose();
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            dualList.AddEmpty();
            var nodes = file.SkillNodeList.AsSpan();
            ref var list = ref dualList.Last;
            list.PrepareAddRange(nodes.Length);
            for (int j = 0; j < nodes.Length; j++)
            {
                ref var node = ref nodes[j];
                list.Add(node.Super.HasValue ? NotYet : Processed);
            }
        }
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            var nodes = file.SkillNodeList.AsSpan();
            ref var superSet = ref file.SkillSet;
            var listSpan = dualList[i].AsSpan();
            for (int j = 0; j < listSpan.Length; j++)
            {
                if (listSpan[j] != NotYet)
                {
                    continue;
                }

                listSpan[j] = Processing;
                ref var node = ref nodes[j];
                var superSpan = superSet[node.Super!.Value];
                ref var superFile = ref TryGetSkillNode(superSpan, out var superNodeIndex);
                do
                {
                    ref var superStatus = ref dualList[superFile.Id][superNodeIndex];
                    if (superStatus == Processed)
                    {
                        break;
                    }

                    if (superStatus == Processing)
                    {
                        file.ErrorAdd_InfiniteLoop("Skill", file.GetSpan(node.Name), superSpan, node.Name);
                        return false;
                    }

                    superStatus = Processing;
                    superSpan = superFile.SkillSet[superFile.SkillNodeList[superNodeIndex].Super!.Value];
                    superFile = ref TryGetSkillNode(superSpan, out superNodeIndex);
                } while (true);
            }
        }
        dualList.Dispose();
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            dualList.AddEmpty();
            var nodes = file.SkillsetNodeList.AsSpan();
            ref var list = ref dualList.Last;
            list.PrepareAddRange(nodes.Length);
            for (int j = 0; j < nodes.Length; j++)
            {
                ref var node = ref nodes[j];
                list.Add(node.Super.HasValue ? NotYet : Processed);
            }
        }
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            var nodes = file.SkillsetNodeList.AsSpan();
            ref var superSet = ref file.SkillsetSet;
            var listSpan = dualList[i].AsSpan();
            for (int j = 0; j < listSpan.Length; j++)
            {
                if (listSpan[j] != NotYet)
                {
                    continue;
                }

                listSpan[j] = Processing;
                ref var node = ref nodes[j];
                var superSpan = superSet[node.Super!.Value];
                ref var superFile = ref TryGetSkillsetNode(superSpan, out var superNodeIndex);
                do
                {
                    ref var superStatus = ref dualList[superFile.Id][superNodeIndex];
                    if (superStatus == Processed)
                    {
                        break;
                    }

                    if (superStatus == Processing)
                    {
                        file.ErrorAdd_InfiniteLoop("Skillset", file.GetSpan(node.Name), superSpan, node.Name);
                        return false;
                    }

                    superStatus = Processing;
                    superSpan = superFile.SkillsetSet[superFile.SkillsetNodeList[superNodeIndex].Super!.Value];
                    superFile = ref TryGetSkillsetNode(superSpan, out superNodeIndex);
                } while (true);
            }
        }
        dualList.Dispose();
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            dualList.AddEmpty();
            var nodes = file.SpotNodeList.AsSpan();
            ref var list = ref dualList.Last;
            list.PrepareAddRange(nodes.Length);
            for (int j = 0; j < nodes.Length; j++)
            {
                ref var node = ref nodes[j];
                list.Add(node.Super.HasValue ? NotYet : Processed);
            }
        }
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            var nodes = file.SpotNodeList.AsSpan();
            ref var superSet = ref file.SpotSet;
            var listSpan = dualList[i].AsSpan();
            for (int j = 0; j < listSpan.Length; j++)
            {
                if (listSpan[j] != NotYet)
                {
                    continue;
                }

                listSpan[j] = Processing;
                ref var node = ref nodes[j];
                var superSpan = superSet[node.Super!.Value];
                ref var superFile = ref TryGetSpotNode(superSpan, out var superNodeIndex);
                do
                {
                    ref var superStatus = ref dualList[superFile.Id][superNodeIndex];
                    if (superStatus == Processed)
                    {
                        break;
                    }

                    if (superStatus == Processing)
                    {
                        file.ErrorAdd_InfiniteLoop("Spot", file.GetSpan(node.Name), superSpan, node.Name);
                        return false;
                    }

                    superStatus = Processing;
                    superSpan = superFile.SpotSet[superFile.SpotNodeList[superNodeIndex].Super!.Value];
                    superFile = ref TryGetSpotNode(superSpan, out superNodeIndex);
                } while (true);
            }
        }
        dualList.Dispose();
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            dualList.AddEmpty();
            var nodes = file.UnitNodeList.AsSpan();
            ref var list = ref dualList.Last;
            list.PrepareAddRange(nodes.Length);
            for (int j = 0; j < nodes.Length; j++)
            {
                ref var node = ref nodes[j];
                list.Add(node.Super.HasValue ? NotYet : Processed);
            }
        }
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            var nodes = file.UnitNodeList.AsSpan();
            ref var superSet = ref file.UnitSet;
            var listSpan = dualList[i].AsSpan();
            for (int j = 0; j < listSpan.Length; j++)
            {
                if (listSpan[j] != NotYet)
                {
                    continue;
                }

                listSpan[j] = Processing;
                ref var node = ref nodes[j];
                var superSpan = superSet[node.Super!.Value];
                ref var superFile = ref TryGetUnitNode(superSpan, out var superNodeIndex);
                do
                {
                    ref var superStatus = ref dualList[superFile.Id][superNodeIndex];
                    if (superStatus == Processed)
                    {
                        break;
                    }

                    if (superStatus == Processing)
                    {
                        file.ErrorAdd_InfiniteLoop("Unit", file.GetSpan(node.Name), superSpan, node.Name);
                        return false;
                    }

                    superStatus = Processing;
                    superSpan = superFile.UnitSet[superFile.UnitNodeList[superNodeIndex].Super!.Value];
                    superFile = ref TryGetUnitNode(superSpan, out superNodeIndex);
                } while (true);
            }
        }
        dualList.Dispose();
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            dualList.AddEmpty();
            var nodes = file.VoiceNodeList.AsSpan();
            ref var list = ref dualList.Last;
            list.PrepareAddRange(nodes.Length);
            for (int j = 0; j < nodes.Length; j++)
            {
                ref var node = ref nodes[j];
                list.Add(node.Super.HasValue ? NotYet : Processed);
            }
        }
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            var nodes = file.VoiceNodeList.AsSpan();
            ref var superSet = ref file.VoiceSet;
            var listSpan = dualList[i].AsSpan();
            for (int j = 0; j < listSpan.Length; j++)
            {
                if (listSpan[j] != NotYet)
                {
                    continue;
                }

                listSpan[j] = Processing;
                ref var node = ref nodes[j];
                var superSpan = superSet[node.Super!.Value];
                ref var superFile = ref TryGetVoiceNode(superSpan, out var superNodeIndex);
                do
                {
                    ref var superStatus = ref dualList[superFile.Id][superNodeIndex];
                    if (superStatus == Processed)
                    {
                        break;
                    }

                    if (superStatus == Processing)
                    {
                        file.ErrorAdd_InfiniteLoop("Voice", file.GetSpan(node.Name), superSpan, node.Name);
                        return false;
                    }

                    superStatus = Processing;
                    superSpan = superFile.VoiceSet[superFile.VoiceNodeList[superNodeIndex].Super!.Value];
                    superFile = ref TryGetVoiceNode(superSpan, out superNodeIndex);
                } while (true);
            }
        }
        dualList.Dispose();
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            dualList.AddEmpty();
            var nodes = file.ScenarioNodeList.AsSpan();
            ref var list = ref dualList.Last;
            list.PrepareAddRange(nodes.Length);
            for (int j = 0; j < nodes.Length; j++)
            {
                ref var node = ref nodes[j];
                list.Add(node.Super.HasValue ? NotYet : Processed);
            }
        }
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            var nodes = file.ScenarioNodeList.AsSpan();
            ref var superSet = ref file.ScenarioSet;
            var listSpan = dualList[i].AsSpan();
            for (int j = 0; j < listSpan.Length; j++)
            {
                if (listSpan[j] != NotYet)
                {
                    continue;
                }

                listSpan[j] = Processing;
                ref var node = ref nodes[j];
                var superSpan = superSet[node.Super!.Value];
                ref var superFile = ref TryGetScenarioNode(superSpan, out var superNodeIndex);
                do
                {
                    ref var superStatus = ref dualList[superFile.Id][superNodeIndex];
                    if (superStatus == Processed)
                    {
                        break;
                    }

                    if (superStatus == Processing)
                    {
                        file.ErrorAdd_InfiniteLoop("Scenario", file.GetSpan(node.Name), superSpan, node.Name);
                        return false;
                    }

                    superStatus = Processing;
                    superSpan = superFile.ScenarioSet[superFile.ScenarioNodeList[superNodeIndex].Super!.Value];
                    superFile = ref TryGetScenarioNode(superSpan, out superNodeIndex);
                } while (true);
            }
        }
        dualList.Dispose();
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            dualList.AddEmpty();
            var nodes = file.EventNodeList.AsSpan();
            ref var list = ref dualList.Last;
            list.PrepareAddRange(nodes.Length);
            for (int j = 0; j < nodes.Length; j++)
            {
                ref var node = ref nodes[j];
                list.Add(node.Super.HasValue ? NotYet : Processed);
            }
        }
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            var nodes = file.EventNodeList.AsSpan();
            ref var superSet = ref file.EventSet;
            var listSpan = dualList[i].AsSpan();
            for (int j = 0; j < listSpan.Length; j++)
            {
                if (listSpan[j] != NotYet)
                {
                    continue;
                }

                listSpan[j] = Processing;
                ref var node = ref nodes[j];
                var superSpan = superSet[node.Super!.Value];
                ref var superFile = ref TryGetEventNode(superSpan, out var superNodeIndex);
                do
                {
                    ref var superStatus = ref dualList[superFile.Id][superNodeIndex];
                    if (superStatus == Processed)
                    {
                        break;
                    }

                    if (superStatus == Processing)
                    {
                        file.ErrorAdd_InfiniteLoop("Event", file.GetSpan(node.Name), superSpan, node.Name);
                        return false;
                    }

                    superStatus = Processing;
                    superSpan = superFile.EventSet[superFile.EventNodeList[superNodeIndex].Super!.Value];
                    superFile = ref TryGetEventNode(superSpan, out superNodeIndex);
                } while (true);
            }
        }
        dualList.Dispose();
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            dualList.AddEmpty();
            var nodes = file.StoryNodeList.AsSpan();
            ref var list = ref dualList.Last;
            list.PrepareAddRange(nodes.Length);
            for (int j = 0; j < nodes.Length; j++)
            {
                ref var node = ref nodes[j];
                list.Add(node.Super.HasValue ? NotYet : Processed);
            }
        }
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            var nodes = file.StoryNodeList.AsSpan();
            ref var superSet = ref file.StorySet;
            var listSpan = dualList[i].AsSpan();
            for (int j = 0; j < listSpan.Length; j++)
            {
                if (listSpan[j] != NotYet)
                {
                    continue;
                }

                listSpan[j] = Processing;
                ref var node = ref nodes[j];
                var superSpan = superSet[node.Super!.Value];
                ref var superFile = ref TryGetStoryNode(superSpan, out var superNodeIndex);
                do
                {
                    ref var superStatus = ref dualList[superFile.Id][superNodeIndex];
                    if (superStatus == Processed)
                    {
                        break;
                    }

                    if (superStatus == Processing)
                    {
                        file.ErrorAdd_InfiniteLoop("Story", file.GetSpan(node.Name), superSpan, node.Name);
                        return false;
                    }

                    superStatus = Processing;
                    superSpan = superFile.StorySet[superFile.StoryNodeList[superNodeIndex].Super!.Value];
                    superFile = ref TryGetStoryNode(superSpan, out superNodeIndex);
                } while (true);
            }
        }
        dualList.Dispose();
        return true;
    }
}
