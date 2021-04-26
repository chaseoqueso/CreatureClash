using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "NAMEOFFILE", menuName = "ScriptableObjects/NAMEOFSTATUS", order = 10)]
public abstract class SpecialEffectObject : ScriptableObject
{
    public abstract void performStatusEffect(ITargetable attacker, ITargetable target);
}
