using UnityEngine;
namespace BWVR {
public sealed partial class QuestSlice {
 void ShowAudioSettings(bool show){
  ShowSanctuaryPage(show?audioSettings:sanctuaryOptions);
 }
 void BuildAudioSettings(){
  audioSettings=new GameObject("Sound settings view",typeof(RectTransform));audioSettings.transform.SetParent(panel.transform,false);audioSettings.GetComponent<RectTransform>().sizeDelta=new Vector2(580,700);
  Label("Sound title",audioSettings.transform,new Vector3(0,255,0),new Vector2(540,80),1,32).text="ISLAND SOUNDS";
  Label("Sound help",audioSettings.transform,new Vector3(0,160,0),new Vector2(520,90),1,23).text="Tap a level to change it.\nSaved on this device.";
  GameObject master=null,ambience=null,effects=null,music=null;
  void Refresh(){master.GetComponentInChildren<UnityEngine.UI.Text>().text="Master · "+Mathf.RoundToInt(soundscape.Master*100)+"%";ambience.GetComponentInChildren<UnityEngine.UI.Text>().text="Ambience · "+Mathf.RoundToInt(soundscape.Ambience*100)+"%";effects.GetComponentInChildren<UnityEngine.UI.Text>().text="Effects · "+Mathf.RoundToInt(soundscape.Effects*100)+"%";music.GetComponentInChildren<UnityEngine.UI.Text>().text="Music · "+Mathf.RoundToInt(soundscape.Music*100)+"%";}
  float Next(float value)=>value<=.01f?1:Mathf.Max(0,(Mathf.Ceil(value*4)-1)/4);
  master=Button("Master",new Vector3(0,65,0),()=>{soundscape.SetMix(Next(soundscape.Master),soundscape.Ambience,soundscape.Effects);Refresh();},audioSettings.transform);
  ambience=Button("Ambience",new Vector3(0,-15,0),()=>{soundscape.SetMix(soundscape.Master,Next(soundscape.Ambience),soundscape.Effects);Refresh();},audioSettings.transform);
  effects=Button("Effects",new Vector3(0,-95,0),()=>{soundscape.SetMix(soundscape.Master,soundscape.Ambience,Next(soundscape.Effects));Refresh();},audioSettings.transform);
  music=Button("Music",new Vector3(0,-175,0),()=>{soundscape.SetMusic(Next(soundscape.Music));Refresh();},audioSettings.transform);
  Button("Back",new Vector3(0,-275,0),()=>ShowAudioSettings(false),audioSettings.transform);Refresh();StyleSanctuarySubview(audioSettings);audioSettings.SetActive(false);
 }
}
}
