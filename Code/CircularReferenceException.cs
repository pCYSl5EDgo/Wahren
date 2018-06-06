using System;
using System.Runtime.Serialization;

namespace Wahren
{
    [Serializable]
    public class CircularReferenceException : Exception
    {
        public CircularReferenceException() { }
        public CircularReferenceException(string message) : base(message) { }
        public CircularReferenceException(string message, Exception inner) : base(message, inner) { }
        protected CircularReferenceException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }
}