using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace AgeOfWar
{
    public class GridStateActivate : AGridState
    {
        public GridStateActivate(GridManager gridManager) : base(gridManager) { }

        CellObject selectedCell;
        CellObject targetCell;

        public override void Enter()
        {
            selectedCell = gridManager.selectedCell;
            targetCell = gridManager.targetCell;
            Debug.Assert(selectedCell != null && targetCell != null);

            GridManager.ActionCompletedActivate += OnCompletedActivate;
            CellObject.TryActivate(selectedCell, targetCell);
        }

        public override void Exit()
        {
            GridManager.ActionCompletedActivate -= OnCompletedActivate;
        }

        private void OnCompletedActivate(CellObject activatedCell)
        {
            // selected -> target   | another        .. activated
            // Omni-lvN -> Unit-lvN | Unit-lvN[zero] .. Unit-lvN
            // Omni-lvN -> Unit-lvM | Unit-lvM[left] .. Unit-lvN
            // Unit-lvN -> Unit-lvM

            CellObject anotherCell = CellObject.AreSame(activatedCell, selectedCell) ?
                targetCell : selectedCell;
            Debug.Assert(activatedCell != null && anotherCell != null);
            Debug.Assert(activatedCell.Gem != null && anotherCell.Gem != null);

            gridManager.matchChecks.Clear();
            if (CellGem.AreSame(new CellGem[] { activatedCell.Gem, anotherCell.Gem }))
            {
                gridManager.matchChecks[anotherCell.Pos] = Vector2Int.zero;
            }
            else
            {
                gridManager.matchChecks[activatedCell.Pos] = anotherCell.GetOffsetTo(activatedCell);
                gridManager.matchChecks[anotherCell.Pos] = activatedCell.GetOffsetTo(anotherCell);
            }
            gridManager.SwitchState(GridState.Match);
        }
    }
}