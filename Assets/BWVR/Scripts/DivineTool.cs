using UnityEngine;
namespace BWVR {
public enum SpiritualTool {Help,Protect,Investigate,Rain,Heal,RainBrush,WindFan,PlantSeeds}
public sealed class DivineTool:MonoBehaviour {
 public SpiritualTool kind;public DivineIntervention owner;
 public void ReturnHome(){var item=GetComponent<CarryItem>();item.held=false;transform.SetPositionAndRotation(item.home,Quaternion.identity);GetComponent<Collider>().enabled=true;}
}
}
