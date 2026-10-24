using UnityEngine;
namespace BWVR {
public sealed class GameArtCatalog : ScriptableObject {
    public GameObject cow, house1, house2, house3, woman, man, beech, oak, pine;
    public Material grass;
    public GameObject Find(string key) => key switch {
        "cow"=>cow,"house1"=>house1,"house2"=>house2,"house3"=>house3,
        "woman"=>woman,"man"=>man,"beech"=>beech,"oak"=>oak,"pine"=>pine,_=>null
    };
}
}
