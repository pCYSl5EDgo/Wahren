using System;
using System.Text;
namespace Wahren.UnityMetaFile
{
    using static System.Buffer;
    public static partial class MetaFileMaker
    {
        private static readonly byte[] _TrueTypeFontBytes0;
        private static readonly byte[] _TrueTypeFontBytes1;
        private static readonly byte[] _TrueTypeFontBytes2;
        public static byte[] TrueTypeFontBytes(this Guid guid, string name)
        {
            var nameBytes = Encoding.UTF8.GetBytes(name);
            byte[] answer = new byte[(nameBytes.Length << 1) + _TrueTypeFontBytes0.Length + _TrueTypeFontBytes1.Length + _TrueTypeFontBytes2.Length + _FileFormatVersion_Guid.Length + 32];
            var index = 0;
            BlockCopy(_FileFormatVersion_Guid, 0, answer, index, _FileFormatVersion_Guid.Length);
            index += _FileFormatVersion_Guid.Length;
            BlockCopy(guid.Convert(), 0, answer, index, 32);
            index += 32;
            BlockCopy(_TrueTypeFontBytes0, 0, answer, index, _TrueTypeFontBytes0.Length);
            index += _TrueTypeFontBytes0.Length;
            BlockCopy(nameBytes, 0, answer, index, nameBytes.Length);
            index += nameBytes.Length;
            BlockCopy(_TrueTypeFontBytes1, 0, answer, index, _TrueTypeFontBytes1.Length);
            index += _TrueTypeFontBytes1.Length;
            BlockCopy(nameBytes, 0, answer, index, nameBytes.Length);
            index += nameBytes.Length;
            BlockCopy(_TrueTypeFontBytes2, 0, answer, index, _TrueTypeFontBytes2.Length);
            return answer;
        }
        public static string TrueTypeFont(this Guid guid, string name) => buf
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