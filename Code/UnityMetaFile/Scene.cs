using System;
using System.Text;
namespace Wahren.UnityMetaFile
{
    public static partial class MetaFileMaker
    {
        public static string Scene(Guid guid) => buf
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