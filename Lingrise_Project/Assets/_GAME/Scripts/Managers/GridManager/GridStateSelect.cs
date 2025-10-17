using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace AgeOfWar
{
    public class GridStateSelect : AGridState
    {
        public GridStateSelect(GridManager gridManager) : base(gridManager) { }

        private TouchHandlerBase touchHandler;
        private CellObject selectedCell;
        private CellObject targetCell;
        private Vector2Int selectDir;

        public override void Enter()
        {
            gridManager.selectedCell = null;
            gridManager.targetCell = null;

            GridManager.ActionCompletedSelect += OnCompletedSelect;
            GridManager.ActionMovingGem += OnMovingGem;
        }

        public override bool Update()
        {
            if (Input.touchCount != 1)
            {
                touchHandler = null;
                return false;
            }

            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchHandler = TouchHandlerFactory.GetTouchHandler(touch, gridManager.Grid);
                    return touchHandler != null && touchHandler.TouchBegan();

                case TouchPhase.Moved:
                    return touchHandler != null && touchHandler.TouchMoved();

                case TouchPhase.Ended or TouchPhase.Canceled:
                    bool rs = touchHandler != null && touchHandler.TouchEnded();
                    touchHandler = null;
                    return rs;
            }

            return true;
        }

        public override void Exit()
        {
            GridManager.ActionCompletedSelect -= OnCompletedSelect;
            GridManager.ActionMovingGem -= OnMovingGem;

            gridManager.selectedCell = selectedCell;
            gridManager.targetCell = targetCell;
            gridManager.selectDir = selectDir;
        }

        private void OnMovingGem(CellObject moveCell, Vector2 delta)
        {
            Debug.Assert(moveCell != null);

            moveCell.MovingGem(delta);
        }

        private void OnCompletedSelect(CellObject selectedCell, Vector2Int selectDir)
        {
            Debug.Assert(selectedCell != null);
            Debug.Assert(Constants.MoveDirs.Contains(selectDir));

            this.selectedCell = selectedCell;
            targetCell = gridManager.Grid.GetGridObject(selectedCell.Pos + selectDir);
            this.selectDir = selectDir;

            if (targetCell == null)
            {
                gridManager.SwitchState(GridState.Discard);
                return;
            }

            if (CellObject.AreSame(selectedCell, targetCell))
            {
                gridManager.SwitchState(GridState.Align);
                return;
            }

            Debug.Assert(CellObject.AreAdjacent(selectedCell, targetCell));
            gridManager.SwitchState(GridState.Swap);
        }
    }
}