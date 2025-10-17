using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace AgeOfWar
{
    public class GridStateAlign : AGridState
    {
        public GridStateAlign(GridManager gridManager) : base(gridManager) { }

        public override void Enter()
        {
            CellObject selectedcell = gridManager.selectedCell;
            Debug.Assert(selectedcell != null);

            GridManager.ActionCompletedAlign += OnCompletedAlign;
            selectedcell.AlignGem();
        }

        public override void Exit()
        {
            GridManager.ActionCompletedAlign -= OnCompletedAlign;
        }

        private void OnCompletedAlign()
        {
            gridManager.SwitchState(GridState.Select);
        }
    }
}