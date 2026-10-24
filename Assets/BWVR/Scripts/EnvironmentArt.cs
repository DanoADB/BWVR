using UnityEngine;
namespace BWVR {
public sealed class EnvironmentArt : ScriptableObject {
 public GameObject[] models;
 public GameObject Find(string key){foreach(var model in models)if(model.name==key)return model;return null;}
}
}
