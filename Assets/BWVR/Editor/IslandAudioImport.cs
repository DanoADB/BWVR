using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
// Only new BWVR-owned sound assets; existing and third-party import settings stay untouched.
public sealed class IslandAudioImport:AssetPostprocessor {
 void OnPreprocessAudio(){bool music=assetPath.StartsWith("Assets/BWVR/Resources/IslandMusic/");if((!music&&!assetPath.StartsWith("Assets/BWVR/Resources/IslandAudio/"))||!assetPath.EndsWith(".wav"))return;var importer=(AudioImporter)assetImporter;importer.forceToMono=!music;importer.loadInBackground=music;var settings=importer.defaultSampleSettings;settings.preloadAudioData=!music;settings.loadType=music?AudioClipLoadType.Streaming:AudioClipLoadType.DecompressOnLoad;settings.compressionFormat=music?AudioCompressionFormat.Vorbis:AudioCompressionFormat.PCM;settings.quality=music?.65f:1;settings.sampleRateSetting=AudioSampleRateSetting.OverrideSampleRate;settings.sampleRateOverride=22050;importer.defaultSampleSettings=settings;}
}
}
