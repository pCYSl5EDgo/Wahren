using System;
using System.Text;
namespace Wahren.UnityMetaFile
{
    public static partial class MetaFileMaker
    {
        public static byte[] PrefabBytes(this Guid guid)
        {
            var answer = new byte[_PrefabBytes.Length];
            Buffer.BlockCopy(_PrefabBytes, 0, answer, 0, answer.Length);
            Buffer.BlockCopy(guid.Convert(), 0, answer, _FileFormatVersion_Guid.Length, 32);
            return answer;
        }
        private static readonly byte[] _PrefabBytes;
        public static string Prefab(this Guid guid) => buf
        .Clear()
        .Append(@"fileFormatVersion: 2
guid: ").Append(guid.ToString("N"))
        .Append(@"
NativeFormatImporter:
  externalObjects: {}
  mainObjectFileID: 100100000
  userData: 
  assetBundleName: 
  assetBundleVariant: 
").ToString();
    }
}