using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AgeOfWar
{
    public class DragHandler : TouchHandlerBase
    {
        //public override TouchEnum TouchEnum => TouchEnum.Touch;

        private Vector2 startPoint = new Vector2(-1, -1);
        private Vector2 endPoint;
        private Vector2Int lastGridObjectPos;

        public DragHandler(Grid<CellObject> grid) : base(grid) {}

        private void HandleDrag()
        {
            CellObject cell = grid.GetGridObject(Utilities.GetMouseWorldPos());
            if (cell == null || cell.Pos == lastGridObjectPos) return;


        }

        private Vector2 CenterPoint(Vector2 point)
        {
            CellObject cell = grid.GetGridObject(Utilities.GetMouseWorldPos());

            //
            return Vector2.one;
        }

        public override bool TouchBegan()
        {
            // start point

            return true;
        }

        public override bool TouchMoved()
        {
            // offset cellgem
            return true;
        }

        public override bool TouchEnded()
        {
            // endpoint
            // calculate
            // sending
            return true;
        }
    }
}

