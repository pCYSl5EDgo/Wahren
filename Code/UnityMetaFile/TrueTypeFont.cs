using System;
using System.Text;
namespace Wahren.UnityMetaFile
{
    public static partial class MetaFileMaker
    {
        public static string TrueTypeFont(Guid guid, string name) => buf
        .Clear()
        .Append(@"fileFormatVersion: 2
guid: ").Append(guid.ToString("N"))
        .Append(@"
TrueTypeFontImporter:
  externalObjects: {}
  serializedVersion: 4
  fontSize: 16
  forceTextureCase: -1
  characterSpacing: 0
  characterPadding: 1
  includeFontData: 1
  fontName: ").Append(name).Append(@"
  fontNames:
  - ").Append(name).Append(@"
  fallbackFontReferences: []
  customCharacters: 
  fontRenderingMode: 0
  ascentCalculationMode: 1
  useLegacyBoundsCalculation: 0
  shouldRoundAdvanceValue: 1
  userData: 
  assetBundleName: 
  assetBundleVariant: 
").ToString();
    }
}