using UnityEngine;
namespace BWVR {
public sealed partial class VillageResources {
 GameObject saplingPrefab;
 public bool CanReplant(GatheringSource node)=>node!=null&&node.tree!=null&&node.sapling!=null&&node.owner==null&&State.DepletedTree(node.id)&&Safe(node);
 // Fit the existing CC0 tree art to a one-unit height; callers control its world size.
 GameObject Seedling(string name,Vector3 at,string modelKey=null){if(saplingPrefab==null)saplingPrefab=Resources.Load<EnvironmentArt>("EnvironmentArt")?.Find("CommonTree_1");var prefab=string.IsNullOrEmpty(modelKey)?saplingPrefab:(Resources.Load<EnvironmentArt>("EnvironmentArt")?.Find(modelKey)??saplingPrefab);var root=new GameObject(name);root.transform.SetParent(transform,false);root.transform.position=at;if(prefab==null){AxePart(root.transform,"Sapling stem",new Vector3(0,.35f,0),new Vector3(.09f,.7f,.09f),timber);AxePart(root.transform,"Sapling foliage",new Vector3(0,.75f,0),new Vector3(.6f,.5f,.6f),roof);return root;}var model=Instantiate(prefab,root.transform);model.transform.localPosition=Vector3.zero;model.transform.localRotation=Quaternion.identity;model.transform.localScale=Vector3.one;var bounds=TreePlayWorld.VisualBounds(model);float height=Mathf.Max(.001f,bounds.size.y);model.transform.localPosition=Vector3.up*(at.y-bounds.min.y)/height;model.transform.localScale=Vector3.one/height;foreach(var collider in model.GetComponentsInChildren<Collider>()){collider.enabled=false;Destroy(collider);}return root;}
 Transform CreateSapling(GatheringSource node){var go=Seedling("Replanted tree "+node.id,node.at,node.tree.GetComponent<IslandPlacementView>()?.modelKey);if(go==null)return null;go.SetActive(false);return go.transform;}
 public GameObject CreateSeedlingLoad(){var go=Seedling("Carried tree seedling",Store);if(go==null)throw new System.InvalidOperationException("Missing CommonTree_1 art for replanting");go.transform.localScale=Vector3.one*Navigation.Scale*1.4f;return go;}
}
}
