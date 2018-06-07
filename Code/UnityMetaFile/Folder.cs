using System;
using System.Text;
namespace Wahren.UnityMetaFile
{
    public static partial class MetaFileMaker
    {
        private static StringBuilder buf = new StringBuilder(4000);
        public static string Folder(Guid guid) => buf
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