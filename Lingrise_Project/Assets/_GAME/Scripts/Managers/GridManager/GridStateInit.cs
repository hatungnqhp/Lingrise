using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfWar
{
    public class GridStateInit : AGridState
    {
        public GridStateInit(GridManager gridManager) : base(gridManager) { }

        public override void Enter()
        {
            SpawnGrid();
            SetVisual();
            gridManager.SwitchState(GridState.Select);
        }

        public override void Exit() { }

        private void SetVisual()
        {
            Image bg = gridManager.Bg;
            if (bg == null) return;
            if (bg.TryGetComponent(out RectTransform rect))
            {
                rect.sizeDelta = new Vector2(gridManager.BgWidth, gridManager.BgHeight);
                rect.anchoredPosition = new Vector2(-1, -1) * gridManager.BorderSize;
            }
        }

        private void SpawnGrid()
        {
            string debugGemGrid = string.Empty;

            gridManager.Grid = new Grid<CellObject>(gridManager.Rows, gridManager.Cols,
                gridManager.CellSize, gridManager.GridPos, (grid, cellPos) =>
            {
                GemEnum gemEnum = RandGemForSpawn(cellPos, grid);
                CellObject cellObject = new(grid, cellPos, gemEnum);
                Debug.Assert(cellObject != null);

                debugGemGrid += gemEnum + " ";
                if (cellPos.x == gridManager.Rows - 1) debugGemGrid += "\n";
                return cellObject;
            });
            Debug.Log(debugGemGrid);
        }

        public GemEnum RandGemForSpawn(Vector2Int pos, Grid<CellObject> grid)
        {
            Debug.Assert(grid != null);

            List<GemEnum> possibleGems = new(gridManager.SelectedGems);
            if (RemoveGemMatch3(pos, grid, out var removeGemEnums))
            {
                possibleGems.RemoveAll(x => removeGemEnums.Contains(x));
            }
            if (RemoveGemMatchSqr2(pos, grid, out var removeGemEnum))
            {
                possibleGems.Remove(removeGemEnum);
            }

            GemEnum gemEnum = possibleGems[Random.Range(0, possibleGems.Count)];
            return gemEnum;
        }

        private bool RemoveGemMatch3(Vector2Int pos, Grid<CellObject> grid, out List<GemEnum> removeGemEnums)
        {
            Debug.Assert(grid != null);

            removeGemEnums = new List<GemEnum>();
            foreach (var dir in new Vector2Int[] { Vector2Int.left, Vector2Int.down })
            {
                Vector2Int pos1 = pos + dir * 2;
                if (!grid.IsInGrid(pos1)) continue;

                Vector2Int pos0 = pos + dir;
                CellGem gem0 = grid.GetGridObject(pos0)?.Gem;
                CellGem gem1 = grid.GetGridObject(pos1)?.Gem;
                Debug.Assert(gem0 != null && gem1 != null);

                if (CellGem.AreSame(new CellGem[] { gem0, gem1 })
                    && !removeGemEnums.Contains(gem0.GemEnum))
                {
                    removeGemEnums.Add(gem0.GemEnum);
                }
            }
            return removeGemEnums.Count > 0;
        }

        private bool RemoveGemMatchSqr2(Vector2Int pos, Grid<CellObject> grid, out GemEnum gemEnum)
        {
            Debug.Assert(grid != null);

            gemEnum = GemEnum.Blank;

            Vector2Int pos2 = pos + Vector2Int.one * -1;
            if (!grid.IsInGrid(pos2)) return false;

            Vector2Int pos0 = pos + Vector2Int.left;
            Vector2Int pos1 = pos + Vector2Int.down;

            CellGem gem0 = grid.GetGridObject(pos0)?.Gem;
            CellGem gem1 = grid.GetGridObject(pos1)?.Gem;
            CellGem gem2 = grid.GetGridObject(pos2)?.Gem;
            Debug.Assert(gem0 != null && gem1 != null && gem2 != null);

            gemEnum = gem0.GemEnum;
            return CellGem.AreSame(new CellGem[] { gem0, gem1, gem2 });
        }
    }
}