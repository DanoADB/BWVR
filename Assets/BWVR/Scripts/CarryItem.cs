using UnityEngine;
namespace BWVR {
public enum Offering { Wood, Food, Water, Villager }
public sealed class CarryItem:MonoBehaviour {public Offering kind;public bool held;public Vector3 home;}
}
