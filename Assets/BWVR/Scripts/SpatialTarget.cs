using System;
using UnityEngine;
namespace BWVR {
public sealed class SpatialTarget:MonoBehaviour { public Action invoke; public void Activate()=>invoke?.Invoke(); }
}
