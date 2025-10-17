using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AgeOfWar
{
    public abstract class TouchHandlerBase : ITouchHandler
    {
        protected Grid<CellObject> grid;

        public TouchHandlerBase(Grid<CellObject> grid)
        {
            this.grid = grid;
        }

        // public abstract TouchEnum TouchEnum { get; }
        public abstract bool TouchBegan();
        public abstract bool TouchMoved();
        public abstract bool TouchEnded();
    }
}

