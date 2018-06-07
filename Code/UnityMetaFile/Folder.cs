using System;
using System.Text;
namespace Wahren.UnityMetaFile
{
    public static partial class MetaFileMaker
    {
        private static readonly byte[] _FolderBytes;
        public static byte[] FolderBytes(this Guid guid)
        {
            var answer = new byte[_FolderBytes.Length];
            Buffer.BlockCopy(_FolderBytes, 0, answer, 0, answer.Length);
            Buffer.BlockCopy(guid.Convert(), 0, answer, _FileFormatVersion_Guid.Length, 32);
            return answer;
        }
        private static StringBuilder buf = new StringBuilder(4000);
        public static string Folder(this Guid guid) => buf
        .Clear()
        .Append(@"fileFormatVersion: 2
guid: ").Append(guid.ToString("N"))
        .Append(@"
folderAsset: yes
DefaultImporter:
  externalObjects: {}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
").ToString();
    }
}