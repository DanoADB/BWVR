using UnityEngine;
namespace BWVR {
// Physical fill posts and additional shelving stay inside the existing carved footprint.
public sealed partial class VillageResources {
 Transform woodFill,wheatFill,overflow;GameObject[] shelves;Transform workBeam;
 void BuildStorageMarkers(){float u=Navigation.Scale;
  Part("Timber capacity post",PrimitiveType.Cube,Store+new Vector3(-4,2,-4.3f)*u,new Vector3(.8f,4,.5f),roof);
  Part("Grain capacity post",PrimitiveType.Cube,Store+new Vector3(4,2,-4.3f)*u,new Vector3(.8f,4,.5f),roof);
  woodFill=Part("Timber fullness",PrimitiveType.Cube,Store,new Vector3(.5f,1,.65f),timber).transform;
  wheatFill=Part("Grain fullness",PrimitiveType.Cube,Store,new Vector3(.5f,1,.65f),grain).transform;
  overflow=Part("Uncovered grain at risk",PrimitiveType.Sphere,Store+new Vector3(-1.5f,.5f,-5)*u,new Vector3(2,1,1.5f),grain).transform;
  workBeam=Part("Storage construction progress",PrimitiveType.Cube,Store+new Vector3(0,3.8f,-4.3f)*u,new Vector3(8,.25f,.3f),timber).transform;
  shelves=new GameObject[3];for(int i=0;i<3;i++)shelves[i]=Part("Built storage shelf "+(i+1),PrimitiveType.Cube,Store+new Vector3(0,1.3f+i,-.2f)*u,new Vector3(8,.18f,6),timber);
 }
 void Fill(Transform marker,float ratio,float x){float h=Mathf.Max(.04f,ratio*3.7f);marker.localScale=new Vector3(.5f,h,.65f)*Navigation.Scale;marker.position=Store+new Vector3(x,h*.5f,-4.4f)*Navigation.Scale;}
 void RefreshStorage(){if(woodFill==null)return;Fill(woodFill,State.wood/State.Limit(VillageResource.Wood),-4);Fill(wheatFill,State.wheat/State.Limit(VillageResource.Wheat),4);overflow.gameObject.SetActive(State.exposedWheat>.01f);overflow.localScale=new Vector3(2,Mathf.Max(.1f,State.exposedWheat/12),1.5f)*Navigation.Scale;for(int i=0;i<3;i++)shelves[i].SetActive(i<State.storageUpgrades);workBeam.gameObject.SetActive(State.storageWork>0);workBeam.localScale=new Vector3(8*State.storageWork/20,.25f,.3f)*Navigation.Scale;}
}
}
