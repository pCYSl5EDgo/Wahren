using System;
using System.Runtime.Serialization;

namespace Wahren.Analysis.Specific
{
    [Serializable]
    public class NotFoundException : Exception
    {
        public NotFoundException() { }
        public NotFoundException(string message) : base(message) { }
        public NotFoundException(string message, Exception inner) : base(message, inner) { }
        protected NotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }
    [Serializable]
    public sealed class UnitNotFoundException : NotFoundException
    {
        public UnitNotFoundException() { }
        public UnitNotFoundException(string message) : base(message) { }
        public UnitNotFoundException(string message, Exception inner) : base(message, inner) { }
        UnitNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }
    [Serializable]
    public class UnitPowerNotFoundException : NotFoundException
    {
        public UnitPowerNotFoundException() { }
        public UnitPowerNotFoundException(string message) : base(message) { }
        public UnitPowerNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected UnitPowerNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }
    [Serializable]
    public class SpotPowerNotFoundException : NotFoundException
    {
        public SpotPowerNotFoundException() { }
        public SpotPowerNotFoundException(string message) : base(message) { }
        public SpotPowerNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected SpotPowerNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }
    [Serializable]
    public class SpotNotFoundException : NotFoundException
    {
        public SpotNotFoundException() { }
        public SpotNotFoundException(string message) : base(message) { }
        public SpotNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected SpotNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }
    [Serializable]
    public class RaceNotFoundException : NotFoundException
    {
        public RaceNotFoundException() { }
        public RaceNotFoundException(string message) : base(message) { }
        public RaceNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected RaceNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }
    [Serializable]
    public sealed class PowerNotFoundException : NotFoundException
    {
        public PowerNotFoundException() { }
        public PowerNotFoundException(string message) : base(message) { }
        public PowerNotFoundException(string message, Exception inner) : base(message, inner) { }
        PowerNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }
    [Serializable]
    public sealed class SkillNotFoundException : NotFoundException
    {
        public SkillNotFoundException() { }
        public SkillNotFoundException(string message) : base(message) { }
        public SkillNotFoundException(string message, Exception inner) : base(message, inner) { }
        SkillNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }
    [Serializable]
    public sealed class OnOffException : NotFoundException
    {
        public OnOffException() { }
        public OnOffException(string message) : base(message) { }
        public OnOffException(string message, Exception inner) : base(message, inner) { }
        OnOffException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }
    [Serializable]
    public class DungeonNotFoundException : NotFoundException
    {
        public DungeonNotFoundException() { }
        public DungeonNotFoundException(string message) : base(message) { }
        public DungeonNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected DungeonNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }
    [Serializable]
    public class UnitClassRaceNotFoundException : NotFoundException
    {
        public UnitClassRaceNotFoundException() { }
        public UnitClassRaceNotFoundException(string message) : base(message) { }
        public UnitClassRaceNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected UnitClassRaceNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }
    [Serializable]
    public sealed class UnitClassNotFoundException : NotFoundException
    {
        public UnitClassNotFoundException() { }
        public UnitClassNotFoundException(string message) : base(message) { }
        public UnitClassNotFoundException(string message, Exception inner) : base(message, inner) { }
        UnitClassNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }
    [Serializable]
    public sealed class ClassNotFoundException : NotFoundException
    {
        public ClassNotFoundException() { }
        public ClassNotFoundException(string message) : base(message) { }
        public ClassNotFoundException(string message, Exception inner) : base(message, inner) { }
        ClassNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }
    [Serializable]
    public sealed class EventNotFoundException : NotFoundException
    {
        public EventNotFoundException() { }
        public EventNotFoundException(string message) : base(message) { }
        public EventNotFoundException(string message, Exception inner) : base(message, inner) { }
        EventNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }
    [Serializable]
    public class UnitSpotNotFoundException : NotFoundException
    {
        public UnitSpotNotFoundException() { }
        public UnitSpotNotFoundException(string message) : base(message) { }
        public UnitSpotNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected UnitSpotNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }
}