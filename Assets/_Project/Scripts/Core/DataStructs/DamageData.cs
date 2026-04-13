using Game.Scripts.Core.DataStructs;
using UnityEngine;

public struct DamageData
{
    public float amount;
    public DamageType damageType;
    public GameObject source;
    
    public DamageData(float amount, DamageType damageType, GameObject source)
    {
        this.amount = amount;
        this.damageType = damageType;
        this.source = source;
    }
}
