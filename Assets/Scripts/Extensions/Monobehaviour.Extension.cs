using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Unity6Sample {
  public static class MonobehaviourExtensions {
    public static void IsNull(this MonoBehaviour mono, object obj) {
      if (obj == null) throw new ArgumentNullException(nameof(obj)); ;
    }
    
    public static void IsNull(this MonoBehaviour mono, params object[] objs) {
      foreach (var obj in objs) {
        mono.IsNull(obj);
      }
    }
  }
}