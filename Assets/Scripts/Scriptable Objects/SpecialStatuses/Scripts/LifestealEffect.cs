using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LifestealEffect", menuName = "ScriptableObjects/Lifesteal", order = 10)]
public class LifestealEffect : SpecialEffectObject
{
    [SerializeField] private float damageMulti;
    [SerializeField] private float lifestealPercent;

    public override void performStatusEffect(ITargetable attacker, List<ITargetable> targets)
    {
        float damage;
        if(attacker is Player)
        {
            damage = ( (Player)attacker ).currentDamage * damageMulti;
        }
        else
        {
            damage = ( (Creature)attacker ).currentDamage * damageMulti;
        }
        
        foreach(ITargetable target in targets)
        {
            float damageDealt = target.updateCurrentHealth(damage);
            attacker.updateCurrentHealth(-damageDealt * lifestealPercent);
        }
    }
}
