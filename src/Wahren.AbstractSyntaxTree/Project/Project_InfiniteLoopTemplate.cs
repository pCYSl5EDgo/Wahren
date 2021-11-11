using Wahren.AbstractSyntaxTree.Parser;

namespace Wahren.AbstractSyntaxTree.Project;

public sealed partial class Project
{
    public bool DetectInfiniteLoop()
    {
        var files = Files.AsSpan();
        DualList<byte> dualList = new();
        ArrayPoolList<(uint fileId, uint nodeId)> trackingList = new();
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
                list.Add(node.HasSuper ? NotYet : Processed);
            }
        }
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            var nodes = file.PowerNodeList.AsSpan();
            var listSpan = dualList[i].AsSpan();
            for (int j = 0; j < listSpan.Length; j++)
            {
                if (listSpan[j] != NotYet)
                {
                    continue;
                }

                trackingList.Clear();
                listSpan[j] = Processing;
                ref var node = ref nodes[j];
                var superSpan = file.GetSpan(node.Super);
                ref var superFile = ref TryGetPowerNode(superSpan, out var superNodeIndex);
                do
                {
                    ref var superStatus = ref dualList[superFile.Id][superNodeIndex];
                    if (superStatus == Processed)
                    {
                        foreach (var (fileId, nodeId) in trackingList.AsSpan())
                        {
                            dualList[fileId][nodeId] = Processed;
                        }
                        break;
                    }

                    if (superStatus == Processing)
                    {
                        file.ErrorAdd_InfiniteLoop("Power", file.GetSpan(node.Name), superSpan, node.Name);
                        return false;
                    }

                    superStatus = Processing;
                    trackingList.Add((superFile.Id, superNodeIndex));
                    superSpan = superFile.GetSpan(superFile.PowerNodeList[superNodeIndex].Super);
                    superFile = ref TryGetPowerNode(superSpan, out superNodeIndex);
                } while (true);
                listSpan[j] = Processed;
            }
        }
        dualList.Dispose();
        trackingList.Dispose();
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
                list.Add(node.HasSuper ? NotYet : Processed);
            }
        }
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            var nodes = file.ClassNodeList.AsSpan();
            var listSpan = dualList[i].AsSpan();
            for (int j = 0; j < listSpan.Length; j++)
            {
                if (listSpan[j] != NotYet)
                {
                    continue;
                }

                trackingList.Clear();
                listSpan[j] = Processing;
                ref var node = ref nodes[j];
                var superSpan = file.GetSpan(node.Super);
                ref var superFile = ref TryGetClassNode(superSpan, out var superNodeIndex);
                do
                {
                    ref var superStatus = ref dualList[superFile.Id][superNodeIndex];
                    if (superStatus == Processed)
                    {
                        foreach (var (fileId, nodeId) in trackingList.AsSpan())
                        {
                            dualList[fileId][nodeId] = Processed;
                        }
                        break;
                    }

                    if (superStatus == Processing)
                    {
                        file.ErrorAdd_InfiniteLoop("Class", file.GetSpan(node.Name), superSpan, node.Name);
                        return false;
                    }

                    superStatus = Processing;
                    trackingList.Add((superFile.Id, superNodeIndex));
                    superSpan = superFile.GetSpan(superFile.ClassNodeList[superNodeIndex].Super);
                    superFile = ref TryGetClassNode(superSpan, out superNodeIndex);
                } while (true);
                listSpan[j] = Processed;
            }
        }
        dualList.Dispose();
        trackingList.Dispose();
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
                list.Add(node.HasSuper ? NotYet : Processed);
            }
        }
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            var nodes = file.DungeonNodeList.AsSpan();
            var listSpan = dualList[i].AsSpan();
            for (int j = 0; j < listSpan.Length; j++)
            {
                if (listSpan[j] != NotYet)
                {
                    continue;
                }

                trackingList.Clear();
                listSpan[j] = Processing;
                ref var node = ref nodes[j];
                var superSpan = file.GetSpan(node.Super);
                ref var superFile = ref TryGetDungeonNode(superSpan, out var superNodeIndex);
                do
                {
                    ref var superStatus = ref dualList[superFile.Id][superNodeIndex];
                    if (superStatus == Processed)
                    {
                        foreach (var (fileId, nodeId) in trackingList.AsSpan())
                        {
                            dualList[fileId][nodeId] = Processed;
                        }
                        break;
                    }

                    if (superStatus == Processing)
                    {
                        file.ErrorAdd_InfiniteLoop("Dungeon", file.GetSpan(node.Name), superSpan, node.Name);
                        return false;
                    }

                    superStatus = Processing;
                    trackingList.Add((superFile.Id, superNodeIndex));
                    superSpan = superFile.GetSpan(superFile.DungeonNodeList[superNodeIndex].Super);
                    superFile = ref TryGetDungeonNode(superSpan, out superNodeIndex);
                } while (true);
                listSpan[j] = Processed;
            }
        }
        dualList.Dispose();
        trackingList.Dispose();
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
                list.Add(node.HasSuper ? NotYet : Processed);
            }
        }
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            var nodes = file.FieldNodeList.AsSpan();
            var listSpan = dualList[i].AsSpan();
            for (int j = 0; j < listSpan.Length; j++)
            {
                if (listSpan[j] != NotYet)
                {
                    continue;
                }

                trackingList.Clear();
                listSpan[j] = Processing;
                ref var node = ref nodes[j];
                var superSpan = file.GetSpan(node.Super);
                ref var superFile = ref TryGetFieldNode(superSpan, out var superNodeIndex);
                do
                {
                    ref var superStatus = ref dualList[superFile.Id][superNodeIndex];
                    if (superStatus == Processed)
                    {
                        foreach (var (fileId, nodeId) in trackingList.AsSpan())
                        {
                            dualList[fileId][nodeId] = Processed;
                        }
                        break;
                    }

                    if (superStatus == Processing)
                    {
                        file.ErrorAdd_InfiniteLoop("Field", file.GetSpan(node.Name), superSpan, node.Name);
                        return false;
                    }

                    superStatus = Processing;
                    trackingList.Add((superFile.Id, superNodeIndex));
                    superSpan = superFile.GetSpan(superFile.FieldNodeList[superNodeIndex].Super);
                    superFile = ref TryGetFieldNode(superSpan, out superNodeIndex);
                } while (true);
                listSpan[j] = Processed;
            }
        }
        dualList.Dispose();
        trackingList.Dispose();
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
                list.Add(node.HasSuper ? NotYet : Processed);
            }
        }
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            var nodes = file.MovetypeNodeList.AsSpan();
            var listSpan = dualList[i].AsSpan();
            for (int j = 0; j < listSpan.Length; j++)
            {
                if (listSpan[j] != NotYet)
                {
                    continue;
                }

                trackingList.Clear();
                listSpan[j] = Processing;
                ref var node = ref nodes[j];
                var superSpan = file.GetSpan(node.Super);
                ref var superFile = ref TryGetMovetypeNode(superSpan, out var superNodeIndex);
                do
                {
                    ref var superStatus = ref dualList[superFile.Id][superNodeIndex];
                    if (superStatus == Processed)
                    {
                        foreach (var (fileId, nodeId) in trackingList.AsSpan())
                        {
                            dualList[fileId][nodeId] = Processed;
                        }
                        break;
                    }

                    if (superStatus == Processing)
                    {
                        file.ErrorAdd_InfiniteLoop("Movetype", file.GetSpan(node.Name), superSpan, node.Name);
                        return false;
                    }

                    superStatus = Processing;
                    trackingList.Add((superFile.Id, superNodeIndex));
                    superSpan = superFile.GetSpan(superFile.MovetypeNodeList[superNodeIndex].Super);
                    superFile = ref TryGetMovetypeNode(superSpan, out superNodeIndex);
                } while (true);
                listSpan[j] = Processed;
            }
        }
        dualList.Dispose();
        trackingList.Dispose();
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
                list.Add(node.HasSuper ? NotYet : Processed);
            }
        }
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            var nodes = file.ObjectNodeList.AsSpan();
            var listSpan = dualList[i].AsSpan();
            for (int j = 0; j < listSpan.Length; j++)
            {
                if (listSpan[j] != NotYet)
                {
                    continue;
                }

                trackingList.Clear();
                listSpan[j] = Processing;
                ref var node = ref nodes[j];
                var superSpan = file.GetSpan(node.Super);
                ref var superFile = ref TryGetObjectNode(superSpan, out var superNodeIndex);
                do
                {
                    ref var superStatus = ref dualList[superFile.Id][superNodeIndex];
                    if (superStatus == Processed)
                    {
                        foreach (var (fileId, nodeId) in trackingList.AsSpan())
                        {
                            dualList[fileId][nodeId] = Processed;
                        }
                        break;
                    }

                    if (superStatus == Processing)
                    {
                        file.ErrorAdd_InfiniteLoop("Object", file.GetSpan(node.Name), superSpan, node.Name);
                        return false;
                    }

                    superStatus = Processing;
                    trackingList.Add((superFile.Id, superNodeIndex));
                    superSpan = superFile.GetSpan(superFile.ObjectNodeList[superNodeIndex].Super);
                    superFile = ref TryGetObjectNode(superSpan, out superNodeIndex);
                } while (true);
                listSpan[j] = Processed;
            }
        }
        dualList.Dispose();
        trackingList.Dispose();
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
                list.Add(node.HasSuper ? NotYet : Processed);
            }
        }
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            var nodes = file.RaceNodeList.AsSpan();
            var listSpan = dualList[i].AsSpan();
            for (int j = 0; j < listSpan.Length; j++)
            {
                if (listSpan[j] != NotYet)
                {
                    continue;
                }

                trackingList.Clear();
                listSpan[j] = Processing;
                ref var node = ref nodes[j];
                var superSpan = file.GetSpan(node.Super);
                ref var superFile = ref TryGetRaceNode(superSpan, out var superNodeIndex);
                do
                {
                    ref var superStatus = ref dualList[superFile.Id][superNodeIndex];
                    if (superStatus == Processed)
                    {
                        foreach (var (fileId, nodeId) in trackingList.AsSpan())
                        {
                            dualList[fileId][nodeId] = Processed;
                        }
                        break;
                    }

                    if (superStatus == Processing)
                    {
                        file.ErrorAdd_InfiniteLoop("Race", file.GetSpan(node.Name), superSpan, node.Name);
                        return false;
                    }

                    superStatus = Processing;
                    trackingList.Add((superFile.Id, superNodeIndex));
                    superSpan = superFile.GetSpan(superFile.RaceNodeList[superNodeIndex].Super);
                    superFile = ref TryGetRaceNode(superSpan, out superNodeIndex);
                } while (true);
                listSpan[j] = Processed;
            }
        }
        dualList.Dispose();
        trackingList.Dispose();
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
                list.Add(node.HasSuper ? NotYet : Processed);
            }
        }
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            var nodes = file.SkillNodeList.AsSpan();
            var listSpan = dualList[i].AsSpan();
            for (int j = 0; j < listSpan.Length; j++)
            {
                if (listSpan[j] != NotYet)
                {
                    continue;
                }

                trackingList.Clear();
                listSpan[j] = Processing;
                ref var node = ref nodes[j];
                var superSpan = file.GetSpan(node.Super);
                ref var superFile = ref TryGetSkillNode(superSpan, out var superNodeIndex);
                do
                {
                    ref var superStatus = ref dualList[superFile.Id][superNodeIndex];
                    if (superStatus == Processed)
                    {
                        foreach (var (fileId, nodeId) in trackingList.AsSpan())
                        {
                            dualList[fileId][nodeId] = Processed;
                        }
                        break;
                    }

                    if (superStatus == Processing)
                    {
                        file.ErrorAdd_InfiniteLoop("Skill", file.GetSpan(node.Name), superSpan, node.Name);
                        return false;
                    }

                    superStatus = Processing;
                    trackingList.Add((superFile.Id, superNodeIndex));
                    superSpan = superFile.GetSpan(superFile.SkillNodeList[superNodeIndex].Super);
                    superFile = ref TryGetSkillNode(superSpan, out superNodeIndex);
                } while (true);
                listSpan[j] = Processed;
            }
        }
        dualList.Dispose();
        trackingList.Dispose();
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
                list.Add(node.HasSuper ? NotYet : Processed);
            }
        }
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            var nodes = file.SkillsetNodeList.AsSpan();
            var listSpan = dualList[i].AsSpan();
            for (int j = 0; j < listSpan.Length; j++)
            {
                if (listSpan[j] != NotYet)
                {
                    continue;
                }

                trackingList.Clear();
                listSpan[j] = Processing;
                ref var node = ref nodes[j];
                var superSpan = file.GetSpan(node.Super);
                ref var superFile = ref TryGetSkillsetNode(superSpan, out var superNodeIndex);
                do
                {
                    ref var superStatus = ref dualList[superFile.Id][superNodeIndex];
                    if (superStatus == Processed)
                    {
                        foreach (var (fileId, nodeId) in trackingList.AsSpan())
                        {
                            dualList[fileId][nodeId] = Processed;
                        }
                        break;
                    }

                    if (superStatus == Processing)
                    {
                        file.ErrorAdd_InfiniteLoop("Skillset", file.GetSpan(node.Name), superSpan, node.Name);
                        return false;
                    }

                    superStatus = Processing;
                    trackingList.Add((superFile.Id, superNodeIndex));
                    superSpan = superFile.GetSpan(superFile.SkillsetNodeList[superNodeIndex].Super);
                    superFile = ref TryGetSkillsetNode(superSpan, out superNodeIndex);
                } while (true);
                listSpan[j] = Processed;
            }
        }
        dualList.Dispose();
        trackingList.Dispose();
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
                list.Add(node.HasSuper ? NotYet : Processed);
            }
        }
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            var nodes = file.SpotNodeList.AsSpan();
            var listSpan = dualList[i].AsSpan();
            for (int j = 0; j < listSpan.Length; j++)
            {
                if (listSpan[j] != NotYet)
                {
                    continue;
                }

                trackingList.Clear();
                listSpan[j] = Processing;
                ref var node = ref nodes[j];
                var superSpan = file.GetSpan(node.Super);
                ref var superFile = ref TryGetSpotNode(superSpan, out var superNodeIndex);
                do
                {
                    ref var superStatus = ref dualList[superFile.Id][superNodeIndex];
                    if (superStatus == Processed)
                    {
                        foreach (var (fileId, nodeId) in trackingList.AsSpan())
                        {
                            dualList[fileId][nodeId] = Processed;
                        }
                        break;
                    }

                    if (superStatus == Processing)
                    {
                        file.ErrorAdd_InfiniteLoop("Spot", file.GetSpan(node.Name), superSpan, node.Name);
                        return false;
                    }

                    superStatus = Processing;
                    trackingList.Add((superFile.Id, superNodeIndex));
                    superSpan = superFile.GetSpan(superFile.SpotNodeList[superNodeIndex].Super);
                    superFile = ref TryGetSpotNode(superSpan, out superNodeIndex);
                } while (true);
                listSpan[j] = Processed;
            }
        }
        dualList.Dispose();
        trackingList.Dispose();
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
                list.Add(node.HasSuper ? NotYet : Processed);
            }
        }
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            var nodes = file.UnitNodeList.AsSpan();
            var listSpan = dualList[i].AsSpan();
            for (int j = 0; j < listSpan.Length; j++)
            {
                if (listSpan[j] != NotYet)
                {
                    continue;
                }

                trackingList.Clear();
                listSpan[j] = Processing;
                ref var node = ref nodes[j];
                var superSpan = file.GetSpan(node.Super);
                ref var superFile = ref TryGetUnitNode(superSpan, out var superNodeIndex);
                do
                {
                    ref var superStatus = ref dualList[superFile.Id][superNodeIndex];
                    if (superStatus == Processed)
                    {
                        foreach (var (fileId, nodeId) in trackingList.AsSpan())
                        {
                            dualList[fileId][nodeId] = Processed;
                        }
                        break;
                    }

                    if (superStatus == Processing)
                    {
                        file.ErrorAdd_InfiniteLoop("Unit", file.GetSpan(node.Name), superSpan, node.Name);
                        return false;
                    }

                    superStatus = Processing;
                    trackingList.Add((superFile.Id, superNodeIndex));
                    superSpan = superFile.GetSpan(superFile.UnitNodeList[superNodeIndex].Super);
                    superFile = ref TryGetUnitNode(superSpan, out superNodeIndex);
                } while (true);
                listSpan[j] = Processed;
            }
        }
        dualList.Dispose();
        trackingList.Dispose();
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
                list.Add(node.HasSuper ? NotYet : Processed);
            }
        }
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            var nodes = file.VoiceNodeList.AsSpan();
            var listSpan = dualList[i].AsSpan();
            for (int j = 0; j < listSpan.Length; j++)
            {
                if (listSpan[j] != NotYet)
                {
                    continue;
                }

                trackingList.Clear();
                listSpan[j] = Processing;
                ref var node = ref nodes[j];
                var superSpan = file.GetSpan(node.Super);
                ref var superFile = ref TryGetVoiceNode(superSpan, out var superNodeIndex);
                do
                {
                    ref var superStatus = ref dualList[superFile.Id][superNodeIndex];
                    if (superStatus == Processed)
                    {
                        foreach (var (fileId, nodeId) in trackingList.AsSpan())
                        {
                            dualList[fileId][nodeId] = Processed;
                        }
                        break;
                    }

                    if (superStatus == Processing)
                    {
                        file.ErrorAdd_InfiniteLoop("Voice", file.GetSpan(node.Name), superSpan, node.Name);
                        return false;
                    }

                    superStatus = Processing;
                    trackingList.Add((superFile.Id, superNodeIndex));
                    superSpan = superFile.GetSpan(superFile.VoiceNodeList[superNodeIndex].Super);
                    superFile = ref TryGetVoiceNode(superSpan, out superNodeIndex);
                } while (true);
                listSpan[j] = Processed;
            }
        }
        dualList.Dispose();
        trackingList.Dispose();
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
                list.Add(node.HasSuper ? NotYet : Processed);
            }
        }
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            var nodes = file.ScenarioNodeList.AsSpan();
            var listSpan = dualList[i].AsSpan();
            for (int j = 0; j < listSpan.Length; j++)
            {
                if (listSpan[j] != NotYet)
                {
                    continue;
                }

                trackingList.Clear();
                listSpan[j] = Processing;
                ref var node = ref nodes[j];
                var superSpan = file.GetSpan(node.Super);
                ref var superFile = ref TryGetScenarioNode(superSpan, out var superNodeIndex);
                do
                {
                    ref var superStatus = ref dualList[superFile.Id][superNodeIndex];
                    if (superStatus == Processed)
                    {
                        foreach (var (fileId, nodeId) in trackingList.AsSpan())
                        {
                            dualList[fileId][nodeId] = Processed;
                        }
                        break;
                    }

                    if (superStatus == Processing)
                    {
                        file.ErrorAdd_InfiniteLoop("Scenario", file.GetSpan(node.Name), superSpan, node.Name);
                        return false;
                    }

                    superStatus = Processing;
                    trackingList.Add((superFile.Id, superNodeIndex));
                    superSpan = superFile.GetSpan(superFile.ScenarioNodeList[superNodeIndex].Super);
                    superFile = ref TryGetScenarioNode(superSpan, out superNodeIndex);
                } while (true);
                listSpan[j] = Processed;
            }
        }
        dualList.Dispose();
        trackingList.Dispose();
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
                list.Add(node.HasSuper ? NotYet : Processed);
            }
        }
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            var nodes = file.EventNodeList.AsSpan();
            var listSpan = dualList[i].AsSpan();
            for (int j = 0; j < listSpan.Length; j++)
            {
                if (listSpan[j] != NotYet)
                {
                    continue;
                }

                trackingList.Clear();
                listSpan[j] = Processing;
                ref var node = ref nodes[j];
                var superSpan = file.GetSpan(node.Super);
                ref var superFile = ref TryGetEventNode(superSpan, out var superNodeIndex);
                do
                {
                    ref var superStatus = ref dualList[superFile.Id][superNodeIndex];
                    if (superStatus == Processed)
                    {
                        foreach (var (fileId, nodeId) in trackingList.AsSpan())
                        {
                            dualList[fileId][nodeId] = Processed;
                        }
                        break;
                    }

                    if (superStatus == Processing)
                    {
                        file.ErrorAdd_InfiniteLoop("Event", file.GetSpan(node.Name), superSpan, node.Name);
                        return false;
                    }

                    superStatus = Processing;
                    trackingList.Add((superFile.Id, superNodeIndex));
                    superSpan = superFile.GetSpan(superFile.EventNodeList[superNodeIndex].Super);
                    superFile = ref TryGetEventNode(superSpan, out superNodeIndex);
                } while (true);
                listSpan[j] = Processed;
            }
        }
        dualList.Dispose();
        trackingList.Dispose();
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
                list.Add(node.HasSuper ? NotYet : Processed);
            }
        }
        for (int i = 0; i < files.Length; i++)
        {
            ref var file = ref files[i];
            var nodes = file.StoryNodeList.AsSpan();
            var listSpan = dualList[i].AsSpan();
            for (int j = 0; j < listSpan.Length; j++)
            {
                if (listSpan[j] != NotYet)
                {
                    continue;
                }

                trackingList.Clear();
                listSpan[j] = Processing;
                ref var node = ref nodes[j];
                var superSpan = file.GetSpan(node.Super);
                ref var superFile = ref TryGetStoryNode(superSpan, out var superNodeIndex);
                do
                {
                    ref var superStatus = ref dualList[superFile.Id][superNodeIndex];
                    if (superStatus == Processed)
                    {
                        foreach (var (fileId, nodeId) in trackingList.AsSpan())
                        {
                            dualList[fileId][nodeId] = Processed;
                        }
                        break;
                    }

                    if (superStatus == Processing)
                    {
                        file.ErrorAdd_InfiniteLoop("Story", file.GetSpan(node.Name), superSpan, node.Name);
                        return false;
                    }

                    superStatus = Processing;
                    trackingList.Add((superFile.Id, superNodeIndex));
                    superSpan = superFile.GetSpan(superFile.StoryNodeList[superNodeIndex].Super);
                    superFile = ref TryGetStoryNode(superSpan, out superNodeIndex);
                } while (true);
                listSpan[j] = Processed;
            }
        }
        dualList.Dispose();
        trackingList.Dispose();
        return true;
    }
}
