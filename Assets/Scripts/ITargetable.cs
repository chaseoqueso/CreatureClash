using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetable
{
    enum TargetType {
        row,
        creature,
        player
    }

    TargetType getTargetType();
    List<ITargetable> getTargets();
    void updateCurrentHealth(float num);
}
