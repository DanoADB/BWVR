using UnityEngine;
namespace BWVR {
// One streamed calm cue, separated by silence; independent of the saved simulation.
public sealed partial class IslandAudio {
 public float Music{get;private set;}
 public float MusicLevel=>music!=null?music.volume:0;
 public bool MusicPlaying{get;private set;}
 public int MusicStarts{get;private set;}
 AudioSource music;float musicElapsed,nextMusicAt=12,musicDuck=1,musicWeather=1;
 void InitializeMusic(){Music=Read("music",.4f);music=Source("Quiet Island music",false);music.priority=210;music.loop=false;music.volume=0;music.clip=Resources.Load<AudioClip>("IslandMusic/QuietIsland");}
 public void SetMusic(float value,bool persist=true){
  if(!Finite(value))return;float prior=Music;Music=Mathf.Clamp01(value);
  if(Music<=0)StopMusic(5);else if(prior<=0)nextMusicAt=clock+5;ApplyMusicMix();
  if(persist){PlayerPrefs.SetFloat("BWVR.Audio.music",Music);PlayerPrefs.Save();}
 }
 void StopMusic(float quiet){if(music!=null){music.Stop();music.volume=0;}MusicPlaying=false;musicElapsed=0;nextMusicAt=clock+quiet;}
 void PauseMusic(bool pause){if(music==null||!MusicPlaying)return;if(pause)music.Pause();else music.UnPause();}
 void AdvanceMusic(float dt){
  if(music==null||music.clip==null)return;
  var w=season().Weather;bool storm=w.kind==IslandWeatherKind.Thunderstorm||w.kind==IslandWeatherKind.TidalWave;
  if(Master<=0||Music<=0){if(MusicPlaying)StopMusic(5);return;}
  if(!MusicPlaying){
   // No catch-up queue: one gentle start when quiet conditions return.
   if(clock<nextMusicAt||storm||w.rain>.5f)return;
   MusicPlaying=true;musicElapsed=0;musicWeather=1;musicDuck=1;music.volume=0;music.time=0;music.Play();MusicStarts++;
  }
  musicElapsed+=dt;
  musicDuck=Mathf.MoveTowards(musicDuck,duck>0?.35f:1,dt*(duck>0?3:.3f));
  musicWeather=Mathf.MoveTowards(musicWeather,storm?0:1-w.rain*.7f,dt*.5f);
  if(musicElapsed>=music.clip.length||(storm&&musicWeather<=0)){StopMusic(40+(MusicStarts*17)%24);return;}
 }
 void ApplyMusicMix(){if(music==null)return;float envelope=MusicPlaying?Mathf.Clamp01(Mathf.Min(musicElapsed/4,(music.clip.length-musicElapsed)/4)):0;music.volume=Master*Music*.1f*envelope*musicDuck*musicWeather;}
}
}
