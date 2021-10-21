﻿#nullable enable
// <auto-generated>
// THIS (.cs) FILE IS GENERATED BY T4. DO NOT CHANGE IT. CHANGE THE .tt FILE INSTEAD.
// </auto-generated>
using Wahren.AbstractSyntaxTree.Parser;

namespace Wahren.AbstractSyntaxTree.Project;

public sealed partial class Project
{
	public StringSpanKeyTrackableSet<AmbiguousNameReference>.Single AmbiguousDictionary_Dungeon = default;
	public StringSpanKeyTrackableSet<AmbiguousNameReference>.Single AmbiguousDictionary_Field = default;
	public StringSpanKeyTrackableSet<AmbiguousNameReference>.Single AmbiguousDictionary_Movetype = default;
	public StringSpanKeyTrackableSet<AmbiguousNameReference>.Single AmbiguousDictionary_Object = default;
	public StringSpanKeyTrackableSet<AmbiguousNameReference>.Single AmbiguousDictionary_Voice = default;
	public StringSpanKeyTrackableSet<AmbiguousNameReference>.Single AmbiguousDictionary_Scenario = default;
	public StringSpanKeyTrackableSet<AmbiguousNameReference>.Single AmbiguousDictionary_Event = default;
	public StringSpanKeyTrackableSet<AmbiguousNameReference>.Single AmbiguousDictionary_Story = default;

    public void Dispose()
    {
        Files.Dispose();
        FileAnalysisList.Dispose();
        ErrorBag.Clear();

        AmbiguousDictionary_SkillSkillset.Dispose();
        AmbiguousDictionary_UnitClassPowerSpotRace.Dispose();
    	AmbiguousDictionary_Dungeon.Dispose();
    	AmbiguousDictionary_Field.Dispose();
    	AmbiguousDictionary_Movetype.Dispose();
    	AmbiguousDictionary_Object.Dispose();
    	AmbiguousDictionary_Voice.Dispose();
    	AmbiguousDictionary_Scenario.Dispose();
    	AmbiguousDictionary_Event.Dispose();
    	AmbiguousDictionary_Story.Dispose();
    }

    public void AddReferenceAndValidate()
    {
        var fileSpan = Files.AsSpan();
        StringSpanKeyTrackableSet<AmbiguousNameReference> ambiguousDictionary_UnitClassPowerSpotRace = new();
        StringSpanKeyTrackableSet<AmbiguousNameReference> ambiguousDictionary_SkillSkillset = new();
    	StringSpanKeyTrackableSet<AmbiguousNameReference> ambiguousDictionary_Dungeon = new();
    	StringSpanKeyTrackableSet<AmbiguousNameReference> ambiguousDictionary_Field = new();
    	StringSpanKeyTrackableSet<AmbiguousNameReference> ambiguousDictionary_Movetype = new();
    	StringSpanKeyTrackableSet<AmbiguousNameReference> ambiguousDictionary_Object = new();
    	StringSpanKeyTrackableSet<AmbiguousNameReference> ambiguousDictionary_Voice = new();
    	StringSpanKeyTrackableSet<AmbiguousNameReference> ambiguousDictionary_Scenario = new();
    	StringSpanKeyTrackableSet<AmbiguousNameReference> ambiguousDictionary_Event = new();
    	StringSpanKeyTrackableSet<AmbiguousNameReference> ambiguousDictionary_Story = new();
        try
        {
            var noDuplication = true;
            for (int fileIndex = 0; fileIndex < fileSpan.Length; ++fileIndex)
            {
                ref var file = ref fileSpan[fileIndex];

                var span_Power = file.PowerNodeList.AsSpan();
                for (int i = 0; i < span_Power.Length; ++i)
                {
                    ref var node = ref span_Power[i];
                    noDuplication &= ambiguousDictionary_UnitClassPowerSpotRace.TryRegisterTrack(file.GetSpan(node.Name), new(fileIndex, i, ReferenceKind.Power, node.Name));
                }

                var span_Class = file.ClassNodeList.AsSpan();
                for (int i = 0; i < span_Class.Length; ++i)
                {
                    ref var node = ref span_Class[i];
                    noDuplication &= ambiguousDictionary_UnitClassPowerSpotRace.TryRegisterTrack(file.GetSpan(node.Name), new(fileIndex, i, ReferenceKind.Class, node.Name));
                }

                var span_Dungeon = file.DungeonNodeList.AsSpan();
                for (int i = 0; i < span_Dungeon.Length; ++i)
                {
                    ref var node = ref span_Dungeon[i];
                    noDuplication &= ambiguousDictionary_Dungeon.TryRegisterTrack(file.GetSpan(node.Name), new(fileIndex, i, ReferenceKind.Dungeon, node.Name));
                }

                var span_Field = file.FieldNodeList.AsSpan();
                for (int i = 0; i < span_Field.Length; ++i)
                {
                    ref var node = ref span_Field[i];
                    noDuplication &= ambiguousDictionary_Field.TryRegisterTrack(file.GetSpan(node.Name), new(fileIndex, i, ReferenceKind.Field, node.Name));
                }

                var span_Movetype = file.MovetypeNodeList.AsSpan();
                for (int i = 0; i < span_Movetype.Length; ++i)
                {
                    ref var node = ref span_Movetype[i];
                    noDuplication &= ambiguousDictionary_Movetype.TryRegisterTrack(file.GetSpan(node.Name), new(fileIndex, i, ReferenceKind.Movetype, node.Name));
                }

                var span_Object = file.ObjectNodeList.AsSpan();
                for (int i = 0; i < span_Object.Length; ++i)
                {
                    ref var node = ref span_Object[i];
                    noDuplication &= ambiguousDictionary_Object.TryRegisterTrack(file.GetSpan(node.Name), new(fileIndex, i, ReferenceKind.Object, node.Name));
                }

                var span_Race = file.RaceNodeList.AsSpan();
                for (int i = 0; i < span_Race.Length; ++i)
                {
                    ref var node = ref span_Race[i];
                    noDuplication &= ambiguousDictionary_UnitClassPowerSpotRace.TryRegisterTrack(file.GetSpan(node.Name), new(fileIndex, i, ReferenceKind.Race, node.Name));
                }

                var span_Skill = file.SkillNodeList.AsSpan();
                for (int i = 0; i < span_Skill.Length; ++i)
                {
                    ref var node = ref span_Skill[i];
                    noDuplication &= ambiguousDictionary_SkillSkillset.TryRegisterTrack(file.GetSpan(node.Name), new(fileIndex, i, ReferenceKind.Skill, node.Name));
                }

                var span_Skillset = file.SkillsetNodeList.AsSpan();
                for (int i = 0; i < span_Skillset.Length; ++i)
                {
                    ref var node = ref span_Skillset[i];
                    noDuplication &= ambiguousDictionary_SkillSkillset.TryRegisterTrack(file.GetSpan(node.Name), new(fileIndex, i, ReferenceKind.Skillset, node.Name));
                }

                var span_Spot = file.SpotNodeList.AsSpan();
                for (int i = 0; i < span_Spot.Length; ++i)
                {
                    ref var node = ref span_Spot[i];
                    noDuplication &= ambiguousDictionary_UnitClassPowerSpotRace.TryRegisterTrack(file.GetSpan(node.Name), new(fileIndex, i, ReferenceKind.Spot, node.Name));
                }

                var span_Unit = file.UnitNodeList.AsSpan();
                for (int i = 0; i < span_Unit.Length; ++i)
                {
                    ref var node = ref span_Unit[i];
                    noDuplication &= ambiguousDictionary_UnitClassPowerSpotRace.TryRegisterTrack(file.GetSpan(node.Name), new(fileIndex, i, ReferenceKind.Unit, node.Name));
                }

                var span_Voice = file.VoiceNodeList.AsSpan();
                for (int i = 0; i < span_Voice.Length; ++i)
                {
                    ref var node = ref span_Voice[i];
                    noDuplication &= ambiguousDictionary_Voice.TryRegisterTrack(file.GetSpan(node.Name), new(fileIndex, i, ReferenceKind.Voice, node.Name));
                }

                var span_Scenario = file.ScenarioNodeList.AsSpan();
                for (int i = 0; i < span_Scenario.Length; ++i)
                {
                    ref var node = ref span_Scenario[i];
                    noDuplication &= ambiguousDictionary_Scenario.TryRegisterTrack(file.GetSpan(node.Name), new(fileIndex, i, ReferenceKind.Scenario, node.Name));
                }

                var span_Event = file.EventNodeList.AsSpan();
                for (int i = 0; i < span_Event.Length; ++i)
                {
                    ref var node = ref span_Event[i];
                    noDuplication &= ambiguousDictionary_Event.TryRegisterTrack(file.GetSpan(node.Name), new(fileIndex, i, ReferenceKind.Event, node.Name));
                }

                var span_Story = file.StoryNodeList.AsSpan();
                for (int i = 0; i < span_Story.Length; ++i)
                {
                    ref var node = ref span_Story[i];
                    noDuplication &= ambiguousDictionary_Story.TryRegisterTrack(file.GetSpan(node.Name), new(fileIndex, i, ReferenceKind.Story, node.Name));
                }
            }

            if (noDuplication)
            {
                AmbiguousDictionary_UnitClassPowerSpotRace = ambiguousDictionary_UnitClassPowerSpotRace.ToSingle();
                AmbiguousDictionary_SkillSkillset = ambiguousDictionary_SkillSkillset.ToSingle();
    	        AmbiguousDictionary_Dungeon = ambiguousDictionary_Dungeon.ToSingle();
    	        AmbiguousDictionary_Field = ambiguousDictionary_Field.ToSingle();
    	        AmbiguousDictionary_Movetype = ambiguousDictionary_Movetype.ToSingle();
    	        AmbiguousDictionary_Object = ambiguousDictionary_Object.ToSingle();
    	        AmbiguousDictionary_Voice = ambiguousDictionary_Voice.ToSingle();
    	        AmbiguousDictionary_Scenario = ambiguousDictionary_Scenario.ToSingle();
    	        AmbiguousDictionary_Event = ambiguousDictionary_Event.ToSingle();
    	        AmbiguousDictionary_Story = ambiguousDictionary_Story.ToSingle();
            }
            else
            {
                PerResultValidator.CollectError(fileSpan, ErrorBag, ref ambiguousDictionary_UnitClassPowerSpotRace);
                PerResultValidator.CollectError(fileSpan, ErrorBag, ref ambiguousDictionary_SkillSkillset);
    	        PerResultValidator.CollectError(fileSpan, ErrorBag, ref ambiguousDictionary_Dungeon);
    	        PerResultValidator.CollectError(fileSpan, ErrorBag, ref ambiguousDictionary_Field);
    	        PerResultValidator.CollectError(fileSpan, ErrorBag, ref ambiguousDictionary_Movetype);
    	        PerResultValidator.CollectError(fileSpan, ErrorBag, ref ambiguousDictionary_Object);
    	        PerResultValidator.CollectError(fileSpan, ErrorBag, ref ambiguousDictionary_Voice);
    	        PerResultValidator.CollectError(fileSpan, ErrorBag, ref ambiguousDictionary_Scenario);
    	        PerResultValidator.CollectError(fileSpan, ErrorBag, ref ambiguousDictionary_Event);
    	        PerResultValidator.CollectError(fileSpan, ErrorBag, ref ambiguousDictionary_Story);
            }
        }
        finally
        {
            ambiguousDictionary_UnitClassPowerSpotRace.Dispose();
            ambiguousDictionary_SkillSkillset.Dispose();
    	    ambiguousDictionary_Dungeon.Dispose();
    	    ambiguousDictionary_Field.Dispose();
    	    ambiguousDictionary_Movetype.Dispose();
    	    ambiguousDictionary_Object.Dispose();
    	    ambiguousDictionary_Voice.Dispose();
    	    ambiguousDictionary_Scenario.Dispose();
    	    ambiguousDictionary_Event.Dispose();
    	    ambiguousDictionary_Story.Dispose();
        }

        for (int fileIndex = 0; fileIndex < fileSpan.Length; ++fileIndex)
        {
            ref var file = ref fileSpan[fileIndex];
            var analysisResult = FileAnalysisList[fileIndex];
            foreach (ref var node in file.PowerNodeList.AsSpan())
            {
                AddReferenceAndValidate(ref file, analysisResult, ref node);
            }
            foreach (ref var node in file.ClassNodeList.AsSpan())
            {
                AddReferenceAndValidate(ref file, analysisResult, ref node);
            }
            foreach (ref var node in file.DungeonNodeList.AsSpan())
            {
                AddReferenceAndValidate(ref file, analysisResult, ref node);
            }
            foreach (ref var node in file.FieldNodeList.AsSpan())
            {
                AddReferenceAndValidate(ref file, analysisResult, ref node);
            }
            foreach (ref var node in file.MovetypeNodeList.AsSpan())
            {
                AddReferenceAndValidate(ref file, analysisResult, ref node);
            }
            foreach (ref var node in file.ObjectNodeList.AsSpan())
            {
                AddReferenceAndValidate(ref file, analysisResult, ref node);
            }
            foreach (ref var node in file.RaceNodeList.AsSpan())
            {
                AddReferenceAndValidate(ref file, analysisResult, ref node);
            }
            foreach (ref var node in file.SkillNodeList.AsSpan())
            {
                AddReferenceAndValidate(ref file, analysisResult, ref node);
            }
            foreach (ref var node in file.SkillsetNodeList.AsSpan())
            {
                AddReferenceAndValidate(ref file, analysisResult, ref node);
            }
            foreach (ref var node in file.SpotNodeList.AsSpan())
            {
                AddReferenceAndValidate(ref file, analysisResult, ref node);
            }
            foreach (ref var node in file.UnitNodeList.AsSpan())
            {
                AddReferenceAndValidate(ref file, analysisResult, ref node);
            }
            foreach (ref var node in file.VoiceNodeList.AsSpan())
            {
                AddReferenceAndValidate(ref file, analysisResult, ref node);
            }
            foreach (ref var node in file.ScenarioNodeList.AsSpan())
            {
                AddReferenceAndValidate(ref file, analysisResult, ref node);
            }
            foreach (ref var node in file.EventNodeList.AsSpan())
            {
                AddReferenceAndValidate(ref file, analysisResult, ref node);
            }
            foreach (ref var node in file.StoryNodeList.AsSpan())
            {
                AddReferenceAndValidate(ref file, analysisResult, ref node);
            }
        }
    }
}