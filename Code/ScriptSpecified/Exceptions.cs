using System;
using System.Runtime.Serialization;
namespace Wahren.Specific
{
    [Serializable]
    public sealed class UnitNotFoundException : Exception
    {
        public UnitNotFoundException() { }
        public UnitNotFoundException(string message) : base(message) { }
        public UnitNotFoundException(string message, Exception inner) : base(message, inner) { }
        UnitNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }
    [System.Serializable]
    public class UnitPowerNotFoundException : System.Exception
    {
        public UnitPowerNotFoundException() { }
        public UnitPowerNotFoundException(string message) : base(message) { }
        public UnitPowerNotFoundException(string message, System.Exception inner) : base(message, inner) { }
        protected UnitPowerNotFoundException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
    [System.Serializable]
    public class SpotPowerNotFoundException : System.Exception
    {
        public SpotPowerNotFoundException() { }
        public SpotPowerNotFoundException(string message) : base(message) { }
        public SpotPowerNotFoundException(string message, System.Exception inner) : base(message, inner) { }
        protected SpotPowerNotFoundException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
    [System.Serializable]
    public class SpotNotFoundException : System.Exception
    {
        public SpotNotFoundException() { }
        public SpotNotFoundException(string message) : base(message) { }
        public SpotNotFoundException(string message, System.Exception inner) : base(message, inner) { }
        protected SpotNotFoundException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
    [System.Serializable]
    public class RaceNotFoundException : System.Exception
    {
        public RaceNotFoundException() { }
        public RaceNotFoundException(string message) : base(message) { }
        public RaceNotFoundException(string message, System.Exception inner) : base(message, inner) { }
        protected RaceNotFoundException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
    [System.Serializable]
    public sealed class PowerNotFoundException : System.Exception
    {
        public PowerNotFoundException() { }
        public PowerNotFoundException(string message) : base(message) { }
        public PowerNotFoundException(string message, System.Exception inner) : base(message, inner) { }
        PowerNotFoundException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
    [System.Serializable]
    public sealed class SkillNotFoundException : System.Exception
    {
        public SkillNotFoundException() { }
        public SkillNotFoundException(string message) : base(message) { }
        public SkillNotFoundException(string message, System.Exception inner) : base(message, inner) { }
        SkillNotFoundException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
    [System.Serializable]
    public sealed class OnOffException : System.Exception
    {
        public OnOffException() { }
        public OnOffException(string message) : base(message) { }
        public OnOffException(string message, System.Exception inner) : base(message, inner) { }
        OnOffException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
    [System.Serializable]
    public sealed class UnitClassNotFoundException : System.Exception
    {
        public UnitClassNotFoundException() { }
        public UnitClassNotFoundException(string message) : base(message) { }
        public UnitClassNotFoundException(string message, System.Exception inner) : base(message, inner) { }
        UnitClassNotFoundException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
    [System.Serializable]
    public sealed class ClassNotFoundException : System.Exception
    {
        public ClassNotFoundException() { }
        public ClassNotFoundException(string message) : base(message) { }
        public ClassNotFoundException(string message, System.Exception inner) : base(message, inner) { }
        ClassNotFoundException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
    [System.Serializable]
    public sealed class EventNotFoundException : System.Exception
    {
        public EventNotFoundException() { }
        public EventNotFoundException(string message) : base(message) { }
        public EventNotFoundException(string message, System.Exception inner) : base(message, inner) { }
        EventNotFoundException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
    [System.Serializable]
    public class UnitSpotNotFoundException : System.Exception
    {
        public UnitSpotNotFoundException() { }
        public UnitSpotNotFoundException(string message) : base(message) { }
        public UnitSpotNotFoundException(string message, System.Exception inner) : base(message, inner) { }
        protected UnitSpotNotFoundException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}