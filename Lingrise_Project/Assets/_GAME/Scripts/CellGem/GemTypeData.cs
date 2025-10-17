using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AgeOfWar
{
    public class GemTypeData : ScriptableObject
    {
        public GemEnum gemType;

        public float attackBaseDMG;
        public float attackRange;
        public float attackCooldown;
    }
}


