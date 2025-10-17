using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AgeOfWar
{
    public abstract class GemAttackBehaviourBase : ScriptableObject, IAttackBehaviour
    {
        public float Cooldown { get; protected set; }
        public abstract IEnumerator Attack(CellGem gem);
    }
}


