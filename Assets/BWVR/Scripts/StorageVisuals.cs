using UnityEngine;
namespace BWVR {
// Recognizable resource signs and physical stock stay inside the carved footprint.
public sealed partial class VillageResources {
 Transform overflow;GameObject[] shelves;Transform workBeam;
 void BuildStorageMarkers(){float u=Navigation.Scale;
  ResourceSign("Timber store sign",-3,VillageResource.Wood);ResourceSign("Grain store sign",3,VillageResource.Wheat);BuildWorkshop();
  overflow=Part("Uncovered grain at risk",PrimitiveType.Sphere,Store+new Vector3(-1.5f,.5f,-5)*u,new Vector3(2,1,1.5f),grain).transform;
  workBeam=Part("Storage construction progress",PrimitiveType.Cube,Store+new Vector3(0,3.8f,-4.3f)*u,new Vector3(8,.25f,.3f),timber).transform;
  shelves=new GameObject[3];for(int i=0;i<3;i++)shelves[i]=Part("Built storage shelf "+(i+1),PrimitiveType.Cube,Store+new Vector3(0,1.3f+i,-.2f)*u,new Vector3(8,.18f,6),timber);
 }
 void RefreshStorage(){if(overflow==null)return;overflow.gameObject.SetActive(State.exposedWheat>.01f);overflow.localScale=new Vector3(2,Mathf.Max(.1f,State.exposedWheat/12),1.5f)*Navigation.Scale;for(int i=0;i<3;i++)shelves[i].SetActive(i<State.storageUpgrades);workBeam.gameObject.SetActive(State.storageWork>0);workBeam.localScale=new Vector3(8*State.storageWork/20,.25f,.3f)*Navigation.Scale;}
}
}
