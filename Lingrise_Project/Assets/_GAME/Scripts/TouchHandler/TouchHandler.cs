using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AgeOfWar
{
    public class TouchHandler : TouchHandlerBase
    {
        private Vector2 startPos;
        private CellObject startSelectedCell;
        private CellObject endSelectedCell;

        public TouchHandler(Grid<CellObject> grid) : base(grid) { }

        public override bool TouchBegan()
        {
            startPos = Utilities.GetMouseWorldPos();
            startSelectedCell = grid.GetGridObject(startPos);
            return true;
        }

        public override bool TouchMoved()
        {
            if (startSelectedCell == null) return true;

            Vector2 currentPos = Utilities.GetMouseWorldPos();
            Vector2 delta = currentPos - startPos;

            //Debug.Log($"{nameof(TouchMoved)}: {nameof(currentPos)}={currentPos} {nameof(delta)}={delta}");
            GridManager.ActionMovingGem?.Invoke(startSelectedCell, delta);
            return true;
        }

        public override bool TouchEnded()
        {
            if (startSelectedCell == null) return true;

            Vector2 endPos = Utilities.GetMouseWorldPos();
            Vector2 delta = endPos - startPos;
            Vector2Int dir = delta.magnitude < GridManager.Instance.CellSize / 2 ?
                Vector2Int.zero :
                Mathf.Abs(delta.x) > Mathf.Abs(delta.y) ?
                    new Vector2Int((int)Mathf.Sign(delta.x), 0) :
                    new Vector2Int(0, (int)Mathf.Sign(delta.y));

            GridManager.ActionCompletedSelect?.Invoke(startSelectedCell, dir);
            return true;
        }
    }

    // public class TouchHandler : TouchHandlerBase
    // {
    //     static private CellObject firstSelectedCell;
    //     static private CellObject secondSelectedCell;

    //     private CellObject startSelectedCell;
    //     private CellObject endSelectedCell;

    //     public TouchHandler(Grid<CellObject> grid) : base(grid) { }

    //     public override bool TouchBegan()
    //     {
    //         startSelectedCell = grid.GetGridObject(Utilities.GetMouseWorldPos());
    //         return true;
    //     }

    //     public override bool TouchMoved()
    //     {
    //         return true;
    //     }

    //     public override bool TouchEnded()
    //     {
    //         endSelectedCell = grid.GetGridObject(Utilities.GetMouseWorldPos());

    //         if (startSelectedCell == null || endSelectedCell == null ||
    //             startSelectedCell.Pos != endSelectedCell.Pos)
    //         {
    //             firstSelectedCell = null;
    //             return true;
    //         }
    //         // Debug.Log($"TouchHandler startSelectedCell.Pos {startSelectedCell.Pos}");
    //         // Debug.Log($"TouchHandler endSelectedCell.Pos {endSelectedCell.Pos}");

    //         if (firstSelectedCell == null)
    //         {
    //             firstSelectedCell = startSelectedCell;
    //             Debug.Log($"TouchHandler firstSelectedCell is set");
    //         }
    //         else
    //         {
    //             secondSelectedCell = startSelectedCell;
    //             Debug.Log($"TouchHandler secondSelectedCell is set");
    //             GridManager.ActionSelecting?.Invoke(firstSelectedCell, secondSelectedCell);

    //             firstSelectedCell = null;
    //             secondSelectedCell = null;
    //         }

    //         return true;
    //     }
    // }
}

