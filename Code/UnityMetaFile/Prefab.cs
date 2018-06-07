using System;
using System.Text;
namespace Wahren.UnityMetaFile
{
    public static partial class MetaFileMaker
    {
        public static string Prefab(Guid guid) => buf
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