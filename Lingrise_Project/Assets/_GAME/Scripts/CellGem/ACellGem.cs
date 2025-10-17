using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AgeOfWar
{
    public abstract class ACellGem
    {
        //public abstract GemState GemState { get; }

        public abstract void EnterState(CellGem cellGem);
        //public abstract void UpdateState();
    }
}

