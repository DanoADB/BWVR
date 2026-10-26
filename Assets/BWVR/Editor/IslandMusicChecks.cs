using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class IslandMusicChecks {
 static void Require(bool ok,string why){if(!ok)throw new Exception(why);}
 [MenuItem("BWVR/Verify island music (Play mode)")]
 public static void Verify(){
  Require(EditorApplication.isPlaying,"Enter Play mode");var audio=IslandAudio.Current;Require(audio!=null,"No soundscape");var state=UnityEngine.Object.FindAnyObjectByType<VillageResources>().Season;
  float master=audio.Master,ambience=audio.Ambience,effects=audio.Effects,level=audio.Music,rain=state.Weather.rain,wind=state.Weather.wind;bool paused=state.paused;var kind=state.Weather.kind;bool had=PlayerPrefs.HasKey("BWVR.Audio.music");float oldPreference=PlayerPrefs.GetFloat("BWVR.Audio.music");
  void Step(float seconds){int count=Mathf.CeilToInt(seconds/.1f);for(int i=0;i<count;i++)audio.Advance(.1f,false);}
  try{
   state.paused=false;state.Weather.kind=IslandWeatherKind.Clear;state.Weather.rain=0;state.Weather.wind=1;audio.Suspend(false);audio.SetMix(1,0,0,false);audio.SetMusic(0,false);
   var music=audio.GetComponentsInChildren<AudioSource>().Single(s=>s.name=="Quiet Island music");Require(music.clip!=null&&music.clip.channels==2&&music.clip.loadType==AudioClipLoadType.Streaming&&!music.loop&&music.spatialBlend==0&&music.priority>150,"Music source/import is not one streamed background voice");
   int starts=audio.MusicStarts;audio.SetMusic(.75f,false);Step(4);Require(!audio.MusicPlaying&&audio.MusicStarts==starts,"Unmute skipped quiet lead-in");Step(2);Require(audio.MusicPlaying&&audio.MusicStarts==starts+1&&audio.MusicLevel>0&&audio.MusicLevel<.075f,"Music did not fade in once");Step(5);float full=audio.MusicLevel;Require(full>.07f,"Music did not reach calm level");
   audio.SetMix(1,0,1,false);var at=Camera.main.transform.position+Camera.main.transform.forward*audio.NearDistance;Require(audio.Emit(IslandSound.Miracle,at,9911),"Cannot stage audible interaction");Step(.3f);float ducked=audio.MusicLevel;Require(ducked<full*.6f&&ducked>0,"Interaction did not duck music");Step(4);Require(audio.MusicLevel>ducked,"Music did not recover smoothly");
   starts=audio.MusicStarts;audio.SetFrozen(true);int position=music.timeSamples;audio.Advance(20,true);Require(music.timeSamples==position&&audio.MusicStarts==starts,"Pause advanced/restarted track");audio.SetFrozen(false);audio.Suspend(true);audio.Advance(20,false);Require(music.timeSamples==position,"Suspend advanced track");audio.Suspend(false);
   audio.SetMusic(0,false);Require(!audio.MusicPlaying&&audio.MusicLevel==0&&audio.Master==1&&audio.Ambience==0&&audio.Effects==1,"Independent music mute changed other groups");audio.SetMusic(.25f);Require(PlayerPrefs.GetFloat("BWVR.Audio.music")==.25f,"Music preference not stored");audio.SetMusic(float.NaN,false);Require(audio.Music==.25f,"Invalid level corrupted music");audio.SetMusic(.75f,false);Step(8);Require(audio.MusicPlaying,"Music did not restart after unmute");
   state.Weather.kind=IslandWeatherKind.Thunderstorm;Step(3);Require(!audio.MusicPlaying&&audio.MusicLevel==0,"Storm failed to fade out music");starts=audio.MusicStarts;Step(65);Require(audio.MusicStarts==starts&&!audio.MusicPlaying,"Storm started queued music");state.Weather.kind=IslandWeatherKind.Clear;Step(.2f);Require(audio.MusicStarts==starts+1&&audio.MusicPlaying,"Clear weather did not allow one fresh cue");
   starts=audio.MusicStarts;Step(music.clip.length+1);Require(!audio.MusicPlaying,"Completed music did not leave silence");Step(20);Require(!audio.MusicPlaying&&audio.MusicStarts==starts,"Music repeated without a quiet gap");
   Debug.Log("BWVR_MUSIC_PASS: one streamed stereo background voice; quiet lead-in, once-only start, fade envelope, interaction duck/recovery, pause/suspend position, independent persistent mute and invalid-value guard, storm fade/no catch-up queue, quiet interval after ending. Native output and headset audition are separate checks.");
  }finally{state.paused=paused;state.Weather.kind=kind;state.Weather.rain=rain;state.Weather.wind=wind;audio.SetMix(master,ambience,effects,false);audio.SetMusic(level,false);if(had)PlayerPrefs.SetFloat("BWVR.Audio.music",oldPreference);else PlayerPrefs.DeleteKey("BWVR.Audio.music");PlayerPrefs.Save();audio.SetFrozen(paused);}
 }
}
}
