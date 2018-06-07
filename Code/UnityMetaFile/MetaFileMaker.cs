using System;
using System.Text;
namespace Wahren.UnityMetaFile
{
    using static System.Text.Encoding;
    using static System.Buffer;
    public static partial class MetaFileMaker
    {
        private static readonly byte[] _FileFormatVersion_Guid;
        private static byte[] Convert(this Guid guid) => UTF8.GetBytes(guid.ToString("N"));
        static MetaFileMaker()
        {
            _FileFormatVersion_Guid = UTF8.GetBytes("fileFormatVersion: 2\r\nguid: ");
            int length = _FileFormatVersion_Guid.Length;
            void BlkCpy(byte[] component, out byte[] dest)
            {
                dest = new byte[32 + length + component.Length];
                BlockCopy(_FileFormatVersion_Guid, 0, dest, 0, length);
                BlockCopy(component, 0, dest, 32 + length, component.Length);
            }
            {
                var csharp = UTF8.GetBytes("\r\nMonoImporter:\r\n  externalObjects: {}\r\n  serializedVersion: 2\r\n  defaultReferences: []\r\n  executionOrder: 0\r\n  icon: {instanceID: 0}\r\n  userData: \r\n  assetBundleName: \r\n  assetBundleVariant: \r\n");
                BlkCpy(csharp, out _CSharpFileBytes);
            }
            {
                var prefab = UTF8.GetBytes("\r\nNativeFormatImporter:\r\n  externalObjects: {}\r\n  mainObjectFileID: 100100000\r\n  userData: \r\n  assetBundleName: \r\n  assetBundleVariant: \r\n");
                BlkCpy(prefab, out _PrefabBytes);
            }
            {
                var scene = UTF8.GetBytes("\r\nDefaultImporter:\r\n  externalObjects: {}\r\n  userData: \r\n  assetBundleName: \r\n  assetBundleVariant: \r\n");
                BlkCpy(scene, out _SceneBytes);
            }
            {
                var singleSprite = UTF8.GetBytes("\r\nTextureImporter:\r\n  fileIDToRecycleName: {}\r\n  externalObjects: {}\r\n  serializedVersion: 5\r\n  mipmaps:\r\n    mipMapMode: 0\r\n    enableMipMap: 0\r\n    sRGBTexture: 1\r\n    linearTexture: 0\r\n    fadeOut: 0\r\n    borderMipMap: 0\r\n    mipMapsPreserveCoverage: 0\r\n    alphaTestReferenceValue: 0.5\r\n    mipMapFadeDistanceStart: 1\r\n    mipMapFadeDistanceEnd: 3\r\n  bumpmap:\r\n    convertToNormalMap: 0\r\n    externalNormalMap: 0\r\n    heightScale: 0.25\r\n    normalMapFilter: 0\r\n  isReadable: 0\r\n  grayScaleToAlpha: 0\r\n  generateCubemap: 6\r\n  cubemapConvolution: 0\r\n  seamlessCubemap: 0\r\n  textureFormat: 1\r\n  maxTextureSize: 2048\r\n  textureSettings:\r\n    serializedVersion: 2\r\n    filterMode: -1\r\n    aniso: -1\r\n    mipBias: -1\r\n    wrapU: 1\r\n    wrapV: 1\r\n    wrapW: -1\r\n  nPOTScale: 0\r\n  lightmap: 0\r\n  compressionQuality: 50\r\n  spriteMode: 1\r\n  spriteExtrude: 1\r\n  spriteMeshType: 0\r\n  alignment: 0\r\n  spritePivot: {x: 0.5, y: 0.5}\r\n  spritePixelsToUnits: 100\r\n  spriteBorder: {x: 0, y: 0, z: 0, w: 0}\r\n  spriteGenerateFallbackPhysicsShape: 1\r\n  alphaUsage: 1\r\n  alphaIsTransparency: 1\r\n  spriteTessellationDetail: -1\r\n  textureType: 8\r\n  textureShape: 1\r\n  singleChannelComponent: 0\r\n  maxTextureSizeSet: 0\r\n  compressionQualitySet: 0\r\n  textureFormatSet: 0\r\n  platformSettings:\r\n  - serializedVersion: 2\r\n    buildTarget: DefaultTexturePlatform\r\n    maxTextureSize: 2048\r\n    resizeAlgorithm: 0\r\n    textureFormat: -1\r\n    textureCompression: 1\r\n    compressionQuality: 50\r\n    crunchedCompression: 0\r\n    allowsAlphaSplitting: 0\r\n    overridden: 0\r\n    androidETC2FallbackOverride: 0\r\n  - serializedVersion: 2\r\n    buildTarget: Standalone\r\n    maxTextureSize: 2048\r\n    resizeAlgorithm: 0\r\n    textureFormat: -1\r\n    textureCompression: 1\r\n    compressionQuality: 50\r\n    crunchedCompression: 0\r\n    allowsAlphaSplitting: 0\r\n    overridden: 0\r\n    androidETC2FallbackOverride: 0\r\n  - serializedVersion: 2\r\n    buildTarget: WebGL\r\n    maxTextureSize: 2048\r\n    resizeAlgorithm: 0\r\n    textureFormat: -1\r\n    textureCompression: 1\r\n    compressionQuality: 50\r\n    crunchedCompression: 0\r\n    allowsAlphaSplitting: 0\r\n    overridden: 0\r\n    androidETC2FallbackOverride: 0\r\n  spriteSheet:\r\n    serializedVersion: 2\r\n    sprites: []\r\n    outline: []\r\n    physicsShape: []\r\n    bones: []\r\n    spriteID: \r\n    vertices: []\r\n    indices: \r\n    edges: []\r\n    weights: []\r\n  spritePackingTag: \r\n  userData: \r\n  assetBundleName: \r\n  assetBundleVariant: \r\n");
                BlkCpy(singleSprite, out _SingleSpriteBytes);
            }
            {
                var folder = UTF8.GetBytes("\r\nfolderAsset: yes\r\nDefaultImporter:\r\n  externalObjects: {}\r\n  userData: \r\n  assetBundleName: \r\n  assetBundleVariant: \r\n");
                BlkCpy(folder, out _FolderBytes);
            }
            _TrueTypeFontBytes0 = UTF8.GetBytes("\r\nTrueTypeFontImporter:\r\n  externalObjects: {}\r\n  serializedVersion: 4\r\n  fontSize: 16\r\n  forceTextureCase: -1\r\n  characterSpacing: 0\r\n  characterPadding: 1\r\n  includeFontData: 1\r\n  fontName: ");
            _TrueTypeFontBytes1 = UTF8.GetBytes("\r\n  fontNames:\r\n  - ");
            _TrueTypeFontBytes2 = UTF8.GetBytes("\r\n  fallbackFontReferences: []\r\n  customCharacters: \r\n  fontRenderingMode: 0\r\n  ascentCalculationMode: 1\r\n  useLegacyBoundsCalculation: 0\r\n  shouldRoundAdvanceValue: 1\r\n  userData: \r\n  assetBundleName: \r\n  assetBundleVariant: \r\n");
            _NineSliceSprite0 = UTF8.GetBytes(@"
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
  spriteBorder: {x: ");
            _NineSliceSprite1 = UTF8.GetBytes(@", y: ");
            _NineSliceSprite2 = UTF8.GetBytes(@", z: ");
            _NineSliceSprite3 = UTF8.GetBytes(@", w: ");
            _NineSliceSprite4 = UTF8.GetBytes(@"}
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
    spriteID: ");
            _NineSliceSprite5 = UTF8.GetBytes("\r\n    vertices: []\r\n    indices: \r\n    edges: []\r\n    weights: []\r\n  spritePackingTag: \r\n  userData: \r\n  assetBundleName: \r\n  assetBundleVariant: \r\n");
        }
    }
}