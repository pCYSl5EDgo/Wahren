using System;
using System.Text;
namespace Wahren.UnityMetaFile
{
    public static partial class MetaFileMaker
    {
        public static byte[] CSharpFileBytes(this Guid guid){
            var answer = new byte[_CSharpFileBytes.Length];
            Buffer.BlockCopy(_CSharpFileBytes, 0, answer, 0, answer.Length);
            Buffer.BlockCopy(guid.Convert(), 0, answer, _FileFormatVersion_Guid.Length, 32);
            return answer;
        }
        private static readonly byte[] _CSharpFileBytes;
        public static string CSharpFile(Guid guid) => buf
        .Clear()
        .Append(@"fileFormatVersion: 2
guid: ").Append(guid.ToString("N"))
        .Append(@"
MonoImporter:
  externalObjects: {}
  serializedVersion: 2
  defaultReferences: []
  executionOrder: 0
  icon: {instanceID: 0}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
").ToString();
    }
}