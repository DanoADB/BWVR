using UnityEngine;
namespace BWVR {
// Original game motion baked locally into compact blend shapes by the art importer.
public sealed class OriginalCowMotion : MonoBehaviour {
    public bool paused;
    SkinnedMeshRenderer[] parts;
    Vector3 previous;
    float standTime,walkTime,walkWeight;
    void Awake(){parts=GetComponentsInChildren<SkinnedMeshRenderer>();previous=transform.position;}
    void Update(){
        float distance=Vector3.Distance(previous,transform.position);previous=transform.position;
        if(paused)return;
        standTime+=Time.deltaTime;walkTime+=distance/.12f*1.666f;
        walkWeight=Mathf.MoveTowards(walkWeight,distance>.00001f?1:0,Time.deltaTime/.18f);
        foreach(var part in parts){
            for(int i=0;i<part.sharedMesh.blendShapeCount;i++)part.SetBlendShapeWeight(i,0);
            Sample(part,"stand",5,standTime/.625f,1-walkWeight);
            Sample(part,"walk",14,walkTime/1.666f,walkWeight);
        }
    }
    static void Sample(SkinnedMeshRenderer part,string clip,int frames,float phase,float weight){
        float position=Mathf.Repeat(phase,1)*frames;int a=Mathf.FloorToInt(position),b=(a+1)%frames;float t=position-a;
        int ia=part.sharedMesh.GetBlendShapeIndex(clip+a),ib=part.sharedMesh.GetBlendShapeIndex(clip+b);
        if(ia>=0)part.SetBlendShapeWeight(ia,(1-t)*100*weight);
        if(ib>=0)part.SetBlendShapeWeight(ib,t*100*weight);
    }
}
}
