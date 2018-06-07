using System;
using System.Text;
namespace Wahren.UnityMetaFile
{
    public static partial class MetaFileMaker
    {
        public static byte[] SceneBytes(this Guid guid)
        {
            var answer = new byte[_SceneBytes.Length];
            Buffer.BlockCopy(_SceneBytes, 0, answer, 0, answer.Length);
            Buffer.BlockCopy(guid.Convert(), 0, answer, _FileFormatVersion_Guid.Length, 32);
            return answer;
        }
        private static readonly byte[] _SceneBytes;
        public static string Scene(this Guid guid) => buf
        .Clear()
        .Append(@"fileFormatVersion: 2
guid: ").Append(guid.ToString("N"))
        .Append(@"
DefaultImporter:
  externalObjects: {}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
").ToString();
    }
}