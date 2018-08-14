using System;

namespace Wahren.Analysis.Data.ECS
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ISharedComponentDataReferenceAttribute : Attribute
    {
        private Type[] _types;

        public ref Type this[int i] => ref _types[i];
        public int Length => _types.Length;
        public ISharedComponentDataReferenceAttribute(params Type[] args) => _types = args;
    }
}