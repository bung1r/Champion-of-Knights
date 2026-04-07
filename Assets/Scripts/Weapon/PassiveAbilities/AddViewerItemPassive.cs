using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

[Serializable]
[CreateAssetMenu(fileName = "New Add Viewer Item Passive", menuName = "Passive Abilities/Add Viewer Item Buff")]
public class AddViewerItemPassive : PassiveAbilityBase
{
    public DatabaseItemData databaseItemData;
    public override void OnUpdate()
    {
        base.OnUpdate();
    }
}
public class AddViewerItemRuntime : PassiveAbilityRuntime
{
    public StatManager statManager;
    public AddViewerItemPassive passiveBase;
    public override void Init(GameObject owner)
    {
        RoundManager.Instance.databaseItemDatas.Add(passiveBase.databaseItemData);
        statManager.RemovePassive(passiveBase);
    }   

    public AddViewerItemRuntime(){}
    public AddViewerItemRuntime(AddViewerItemPassive passive, StatManager manager)
    {
        statManager = manager;
        passiveBase = passive;
    }
}