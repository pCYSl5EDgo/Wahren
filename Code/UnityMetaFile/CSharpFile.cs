using System;
using System.Text;
namespace Wahren.UnityMetaFile
{
    public static partial class MetaFileMaker
    {
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