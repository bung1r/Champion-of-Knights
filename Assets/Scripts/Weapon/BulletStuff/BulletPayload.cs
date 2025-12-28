using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class BulletPayload : ScriptableObject
{
    public abstract void OnPierce(Transform self, Transform target, DamageData damageData, Vector3 hitPoint);
    // this runs whenever the bullet is destroyed when it runs out of pierce (i.e., hits a target)
    public abstract void OnNormalDestroy(Transform self, Transform target, DamageData damageData, Vector3 hitPoint);

    // runs just before the bullet is destroyed in all cases. 
    public abstract void OnAllDestroy(Transform self, Transform target, DamageData damageData, Vector3 hitPoint);

}