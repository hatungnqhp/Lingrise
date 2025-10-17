using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace AgeOfWar
{
    public class GridStateDiscard : AGridState
    {
        public GridStateDiscard(GridManager gridManager) : base(gridManager) { }

        private CellObject selectedCell;
        private CellObject targetCell;
        private Vector2Int selectDir;
        public Dictionary<Vector2Int, GemChangeType> matchChanges = new();


        public override void Enter()
        {
            GridManager.ActionCompletedDiscard += OnCompletedDiscard;

            selectedCell = gridManager.selectedCell;
            targetCell = gridManager.targetCell;
            selectDir = gridManager.selectDir;
            Debug.Assert(selectedCell != null && targetCell == null);

            selectedCell.DiscardGem(selectDir);
        }

        public override void Exit()
        {
            GridManager.ActionCompletedDiscard -= OnCompletedDiscard;
        }

        private void OnCompletedDiscard(Vector2Int discardPos)
        {
            Debug.Assert(gridManager.Grid.GetGridObject(discardPos)?.Gem != null);

            matchChanges.Clear();
            matchChanges[discardPos] = GemChangeType.ChangeToBlank;

            gridManager.matchChanges = matchChanges;
            gridManager.SwitchState(GridState.Drop);
        }
    }
}