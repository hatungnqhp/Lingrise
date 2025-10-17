using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace AgeOfWar
{
    public class GridStateSwap : AGridState
    {
        public GridStateSwap(GridManager gridManager) : base(gridManager) { }

        bool doMoveBack;
        bool doNotActivateAndMatch;

        public override void Enter()
        {
            GridManager.ActionCompletedSwap += OnCompletedSwap;

            CellObject selectedCell = gridManager.selectedCell;
            CellObject targetCell = gridManager.targetCell;
            //Vector2Int selectDir = gridManager.selectDir;
            Debug.Assert(selectedCell != null && targetCell != null);

            doMoveBack = CellGem.AreSame(new CellGem[] { selectedCell?.Gem, targetCell?.Gem })
                && !CellGem.AreOmni(selectedCell?.Gem, targetCell?.Gem);
            doNotActivateAndMatch = CellGem.AreOmni(selectedCell?.Gem, targetCell?.Gem);
            // if (!doMoveBack)
            // {
            //     //turns--;
            // }

            CellObject.SwapGem(selectedCell, targetCell, doMoveBack);
        }

        public override void Exit()
        {
            GridManager.ActionCompletedSwap -= OnCompletedSwap;
        }

        private void OnCompletedSwap()
        {
            if (doMoveBack || doNotActivateAndMatch)
            {
                gridManager.SwitchState(GridState.Select);
                return;
            }

            gridManager.SwitchState(GridState.Activate);
        }
    }
}