using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
// Only new BWVR-owned sound assets; existing and third-party import settings stay untouched.
public sealed class IslandAudioImport:AssetPostprocessor {
 void OnPreprocessAudio(){if(!assetPath.StartsWith("Assets/BWVR/Resources/IslandAudio/")||!assetPath.EndsWith(".wav"))return;var importer=(AudioImporter)assetImporter;importer.forceToMono=true;var settings=importer.defaultSampleSettings;settings.preloadAudioData=true;settings.loadType=AudioClipLoadType.DecompressOnLoad;settings.compressionFormat=AudioCompressionFormat.PCM;settings.sampleRateSetting=AudioSampleRateSetting.OverrideSampleRate;settings.sampleRateOverride=22050;importer.defaultSampleSettings=settings;}
}
}
