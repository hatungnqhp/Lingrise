using System;
using System.Collections;
using UnityEngine;

namespace AgeOfWar
{
    public interface IGridState
    {
        void Enter();
        void Exit();
        bool Update();
    }

    public abstract class AGridState : IGridState
    {
        protected GridManager gridManager;
        protected AGridState(GridManager gridManager)
        {
            Debug.Assert(gridManager != null);
            this.gridManager = gridManager;
        }

        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual bool Update() { return false; }
    }
}