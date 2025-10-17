using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AgeOfWar
{
    public class TouchHandlerFactory
    {
        private static Vector2Int lastCellSelectedGridPos = new Vector2Int(-1, -1);
        private static float lastTimeSelected = Mathf.NegativeInfinity;

        public static TouchHandlerBase GetTouchHandler(Touch touch, Grid<CellObject> grid)
        {
            //Debug.Log($"GetTouchHandler start");
            if (grid == null || touch.phase != TouchPhase.Began) return null;

            // CellObject cellObject = grid.GetGridObject(Utilities.GetMouseWorldPos());
            // if (cellObject == null) return null;
            // Debug.Log($"GetTouchHandler cellObject.Pos {cellObject.Pos}");

            return true ? //UserData.ToggleSwipe && GameData.Instance.HasToggle ?
                new TouchHandler(grid) :
                new DragHandler(grid);
        }
    }
}

