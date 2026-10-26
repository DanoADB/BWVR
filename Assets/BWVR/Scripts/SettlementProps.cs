using UnityEngine;
namespace BWVR {
// Prefabs are normalized to one unit tall, with their feet at zero.
public sealed partial class VillageResources {
 GameObject Prop(string model,string label,Vector3 at,float height,Transform parent=null){
  var prefab=Resources.Load<GameObject>("SettlementProps/"+model);
  if(prefab==null)throw new System.InvalidOperationException("Missing settlement prop: "+model);
  var go=Instantiate(prefab,parent==null?transform:parent);go.name=label;go.transform.position=at;go.transform.localScale=Vector3.one*height*Navigation.Scale;return go;
 }
 GameObject HandTool(string model,string label,float height){var root=new GameObject(label);root.transform.SetParent(transform,false);root.transform.localScale=Vector3.one*Navigation.Scale;
  var art=Prop(model,"Kenney tool model",Vector3.zero,height,root.transform);art.transform.localScale=Vector3.one*height;art.transform.localPosition=Vector3.down*height*.3f;return root;
 }
 void ResourceSign(string label,float x,VillageResource resource){float u=Navigation.Scale;var sign=Prop("signpost-single",label,Store+new Vector3(x,.3f,-3.5f)*u,3.5f);var at=Store+new Vector3(x,2.9f,-3.8f)*u;
  if(resource==VillageResource.Wood){var logs=Prop("resource-wood","Log symbol",at,.65f);logs.transform.localRotation=Quaternion.Euler(0,90,0);}
  else if(resource==VillageResource.Wheat){var wheat=WheatPatch(at);wheat.name="Wheat symbol";wheat.transform.localScale*=.55f;}
  else {var jar=ResinJar("Resin jar symbol",at);jar.transform.localScale=Vector3.one*.6f;}
 }
 void BuildWorkshop(){float u=Navigation.Scale;Prop("workbench","Toolmaking workbench",Store+new Vector3(0,.3f,0)*u,2);var axe=Prop("tool-axe","Workbench axe",Store+new Vector3(-.8f,2.3f,0)*u,1.3f);axe.transform.localRotation=Quaternion.Euler(85,0,0);var hoe=Prop("tool-hoe","Workbench hoe",Store+new Vector3(.8f,.3f,0)*u,2.4f);hoe.transform.localRotation=Quaternion.Euler(0,0,-12);}
}
}
