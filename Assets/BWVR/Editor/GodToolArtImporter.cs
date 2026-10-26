using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public sealed class GodToolArtImporter:AssetPostprocessor {
 void OnPreprocessTexture(){if(assetPath!="Assets/BWVR/Resources/GodToolIcons/atlas.png")return;var importer=(TextureImporter)assetImporter;importer.textureType=TextureImporterType.Default;importer.alphaIsTransparency=true;importer.sRGBTexture=true;importer.mipmapEnabled=true;importer.isReadable=false;importer.npotScale=TextureImporterNPOTScale.None;importer.maxTextureSize=2048;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.wrapMode=TextureWrapMode.Clamp;importer.filterMode=FilterMode.Bilinear;}
}
}
