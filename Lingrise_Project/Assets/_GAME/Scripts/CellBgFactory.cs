using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AgeOfWar
{
    [CreateAssetMenu(fileName = "CellBgFactory", menuName = "AgeOfWar/SO/CellBgFactory")]
    public class CellBgFactory : ScriptableObject
    {
        [SerializeField] private CellBg cellBgPf;
        [SerializeField] private Sprite bg00, bgX0, bg0Y, bgXY, bgDf;
        [SerializeField] private Color bgColor1, bgColor2;

        public CellBg CreateCellBg(Grid<CellObject> grid, Vector2Int gridPos, Transform parent)
        {
            CellBg cellBg = Instantiate(cellBgPf, parent);
            cellBg.transform.position = grid.GetGridObjectPos(gridPos);

            Sprite bgSprite;
            if (gridPos.x == 0 && gridPos.y == 0) bgSprite = bg00;
            else if (gridPos.x == 0 && gridPos.y == grid.Rows-1) bgSprite = bg0Y;
            else if (gridPos.x == grid.Cols-1 && gridPos.y == 0) bgSprite = bgX0;
            else if (gridPos.x == grid.Cols-1 && gridPos.y == grid.Rows-1) bgSprite = bgXY;
            else bgSprite = bgDf;

            Color bgColor = (gridPos.x + gridPos.y) % 2 == 0 ? bgColor1 : bgColor2;

            cellBg.Setup(bgSprite, bgColor);

            return cellBg;
        }
    }
}

