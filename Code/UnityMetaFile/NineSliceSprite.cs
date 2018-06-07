using System;
using System.Text;
namespace Wahren.UnityMetaFile
{
    using static Buffer;
    public static partial class MetaFileMaker
    {
        private static byte[] NineSliceSpriteBytes(this Guid guid0, Guid guid1, byte[] _x, byte[] _y, byte[] _z, byte[] _w)
        {
            var answer = new byte[_FileFormatVersion_Guid.Length + 64 + _NineSliceSprite0.Length + _NineSliceSprite1.Length + _NineSliceSprite2.Length + _NineSliceSprite3.Length + _NineSliceSprite4.Length + _NineSliceSprite5.Length + _x.Length + _y.Length + _z.Length + _w.Length];
            int index = 0;
            BlockCopy(_FileFormatVersion_Guid, 0, answer, index, _FileFormatVersion_Guid.Length);
            index += _FileFormatVersion_Guid.Length;
            BlockCopy(guid0.Convert(), 0, answer, index, 32);
            index += 32;
            BlockCopy(_NineSliceSprite0, 0, answer, index, _NineSliceSprite0.Length);
            index += _NineSliceSprite0.Length;
            BlockCopy(_x, 0, answer, index, _x.Length);
            index += _x.Length;
            BlockCopy(_NineSliceSprite1, 0, answer, index, _NineSliceSprite1.Length);
            index += _NineSliceSprite1.Length;
            BlockCopy(_y, 0, answer, index, _y.Length);
            index += _y.Length;
            BlockCopy(_NineSliceSprite2, 0, answer, index, _NineSliceSprite2.Length);
            index += _NineSliceSprite2.Length;
            BlockCopy(_z, 0, answer, index, _z.Length);
            index += _z.Length;
            BlockCopy(_NineSliceSprite3, 0, answer, index, _NineSliceSprite3.Length);
            index += _NineSliceSprite3.Length;
            BlockCopy(_w, 0, answer, index, _w.Length);
            index += _w.Length;
            BlockCopy(_NineSliceSprite4, 0, answer, index, _NineSliceSprite4.Length);
            index += _NineSliceSprite4.Length;
            BlockCopy(guid1.Convert(), 0, answer, index, 32);
            index += 32;
            BlockCopy(_NineSliceSprite5, 0, answer, index, _NineSliceSprite5.Length);
            return answer;
        }
        public static byte[] NineSliceSpriteBytes(this Guid guid0, Guid guid1, int border)
        {
            var _x = Encoding.UTF8.GetBytes(border.ToString());
            return guid0.NineSliceSpriteBytes(guid1, _x, _x, _x, _x);
        }
        public static byte[] NineSliceSpriteBytes(this Guid guid0, Guid guid1, int x, int y, int z, int w)
        => guid0.NineSliceSpriteBytes(guid1, Encoding.UTF8.GetBytes(x.ToString()), Encoding.UTF8.GetBytes(y.ToString()), Encoding.UTF8.GetBytes(z.ToString()), Encoding.UTF8.GetBytes(w.ToString()));
        private static readonly byte[] _NineSliceSprite0;
        private static readonly byte[] _NineSliceSprite1;
        private static readonly byte[] _NineSliceSprite2;
        private static readonly byte[] _NineSliceSprite3;
        private static readonly byte[] _NineSliceSprite4;
        private static readonly byte[] _NineSliceSprite5;
        public static string NineSliceSprite(this Guid guid0, Guid guid1, int border) => NineSliceSprite(guid0, guid1, border, border, border, border);
        public static string NineSliceSprite(this Guid guid0, Guid guid1, int x, int y, int z, int w) => buf
        .Clear()
        .Append(@"fileFormatVersion: 2
guid: ").Append(guid0.ToString("N"))
        .Append(@"
TextureImporter:
  fileIDToRecycleName: {}
  externalObjects: {}
  serializedVersion: 5
  mipmaps:
    mipMapMode: 0
    enableMipMap: 0
    sRGBTexture: 1
    linearTexture: 0
    fadeOut: 0
    borderMipMap: 0
    mipMapsPreserveCoverage: 0
    alphaTestReferenceValue: 0.5
    mipMapFadeDistanceStart: 1
    mipMapFadeDistanceEnd: 3
  bumpmap:
    convertToNormalMap: 0
    externalNormalMap: 0
    heightScale: 0.25
    normalMapFilter: 0
  isReadable: 0
  grayScaleToAlpha: 0
  generateCubemap: 6
  cubemapConvolution: 0
  seamlessCubemap: 0
  textureFormat: 1
  maxTextureSize: 2048
  textureSettings:
    serializedVersion: 2
    filterMode: -1
    aniso: -1
    mipBias: -1
    wrapU: 1
    wrapV: 1
    wrapW: -1
  nPOTScale: 0
  lightmap: 0
  compressionQuality: 50
  spriteMode: 1
  spriteExtrude: 1
  spriteMeshType: 0
  alignment: 0
  spritePivot: {x: 0.5, y: 0.5}
  spritePixelsToUnits: 100
  spriteBorder: {x: ")
    .Append(x)
    .Append(", y: ")
    .Append(y)
    .Append(", z: ")
    .Append(z)
    .Append(", w: ")
    .Append(w)
    .Append(@"}
  spriteGenerateFallbackPhysicsShape: 1
  alphaUsage: 1
  alphaIsTransparency: 1
  spriteTessellationDetail: -1
  textureType: 8
  textureShape: 1
  singleChannelComponent: 0
  maxTextureSizeSet: 0
  compressionQualitySet: 0
  textureFormatSet: 0
  platformSettings:
  - serializedVersion: 2
    buildTarget: DefaultTexturePlatform
    maxTextureSize: 2048
    resizeAlgorithm: 0
    textureFormat: -1
    textureCompression: 1
    compressionQuality: 50
    crunchedCompression: 0
    allowsAlphaSplitting: 0
    overridden: 0
    androidETC2FallbackOverride: 0
  - serializedVersion: 2
    buildTarget: Standalone
    maxTextureSize: 2048
    resizeAlgorithm: 0
    textureFormat: -1
    textureCompression: 1
    compressionQuality: 50
    crunchedCompression: 0
    allowsAlphaSplitting: 0
    overridden: 0
    androidETC2FallbackOverride: 0
  - serializedVersion: 2
    buildTarget: WebGL
    maxTextureSize: 2048
    resizeAlgorithm: 0
    textureFormat: -1
    textureCompression: 1
    compressionQuality: 50
    crunchedCompression: 0
    allowsAlphaSplitting: 0
    overridden: 0
    androidETC2FallbackOverride: 0
  spriteSheet:
    serializedVersion: 2
    sprites: []
    outline: []
    physicsShape: []
    bones: []
    spriteID: ")
      .Append(guid1.ToString("N"))
      .Append(@"
    vertices: []
    indices: 
    edges: []
    weights: []
  spritePackingTag: 
  userData: 
  assetBundleName: 
  assetBundleVariant: 
").ToString();
    }
}