using System.Collections;
using System.Collections.Generic;
using AgeOfWar;
using UnityEngine;

namespace AgeOfWar
{
    public interface IAttackBehaviour
    {
        float Cooldown { get; }
        IEnumerator Attack(CellGem gem);
    }
}