using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "NAMEOFFILE" (without .cs at the end), menuName = "ScriptableObjects/NAMEOFEFFECT", order = 10)]
public abstract class SpecialEffectObject : ScriptableObject
{
    public abstract void performStatusEffect(ITargetable attacker, List<ITargetable> targets);
}
