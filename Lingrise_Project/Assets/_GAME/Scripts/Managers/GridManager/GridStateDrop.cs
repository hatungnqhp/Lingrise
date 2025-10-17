using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace AgeOfWar
{
    public class GridStateDrop : AGridState
    {
        public GridStateDrop(GridManager gridManager) : base(gridManager) { }

        private Grid<CellObject> grid;
        private Dictionary<Vector2Int, GemChangeType> matchChanges;
        private Dictionary<Vector2Int, Vector2Int> matchChecks = new();

        public override void Enter()
        {
            grid = gridManager.Grid;
            matchChanges = gridManager.matchChanges;
            matchChecks.Clear();

            GridManager.ActionCompletedDrop += OnCompletedDrop;
            HandleDropping();
        }

        public override void Exit()
        {
            GridManager.ActionCompletedDrop -= OnCompletedDrop;
        }

        private void OnCompletedDrop()
        {
            gridManager.matchChecks = matchChecks;
            gridManager.matchChanges.Clear();
            gridManager.SwitchState(GridState.Match);
        }

        private void HandleDropping()
        {
            Debug.Assert(matchChanges != null && matchChanges.Count > 0);

            List<int>[] colBlanks = new List<int>[gridManager.Cols];
            foreach (var matchChange in matchChanges)
            {
                Vector2Int pos = matchChange.Key;
                GemChangeType changeType = matchChange.Value;
                CellObject cell = grid.GetGridObject(pos);

                Debug.Assert(cell != null);

                switch (changeType)
                {
                    case GemChangeType.ChangeToBlank:
                        Debug.Assert(0 <= pos.x && pos.x < colBlanks.Length);
                        if (colBlanks[pos.x] == null) colBlanks[pos.x] = new();
                        colBlanks[pos.x].Add(pos.y);
                        cell.ChangeToBlank();
                        break;
                    case GemChangeType.ChangeToOmni:
                        cell.ChangeGemToOmni();
                        break;
                    case GemChangeType.LevelUp:
                        matchChecks[cell.Pos] = Vector2Int.zero;
                        cell.LevelUpGem();
                        break;
                    default:
                        Debug.Assert(true);
                        break;
                }
            }

            Sequence seq = DOTween.Sequence();
            for (int col = 0; col < colBlanks.Length; col++)
            {
                List<int> blankRows = colBlanks[col];
                if (blankRows == null) continue;
                Debug.Assert(blankRows.Count > 0);
                blankRows.Sort();

                int iblank = 0;
                for (int row = blankRows[0]; row < gridManager.Rows + blankRows.Count; row++) //
                {
                    if (iblank < blankRows.Count && row == blankRows[iblank])
                    {
                        iblank++;
                    }
                    else
                    {
                        Debug.Assert(iblank > 0);

                        Vector2Int startPos = new(col, row);
                        Tween tween = CellObject.DropGem(startPos, iblank, (targetPos) =>
                        {
                            GemEnum newGemType = gridManager.SelectedGems[Random.Range(0, gridManager.SelectedGems.Length)];
                            CellGem newGem = gridManager.CellGemFactory.CreateCellGem(
                                GridManager.Instance.Grid, startPos, null, newGemType, GridManager.Instance.CellGemParent);
                            Debug.Log($"{nameof(HandleDropping)}: {nameof(startPos)}={startPos} {nameof(newGemType)}={newGemType}");
                            return newGem;
                        });
                        if (tween == null) continue;
                        seq.Join(tween);
                    }

                    if (row < gridManager.Rows && iblank > 0)
                    {
                        Vector2Int checkPos = new(col, row);
                        CellObject checkCell = grid.GetGridObject(checkPos);
                        matchChecks[checkPos] = Vector2Int.down;
                    }
                }
            }
            seq.Play().OnComplete(() => { GridManager.ActionCompletedDrop?.Invoke(); });
        }

        // public GemEnum RandGemForDrop(Vector2Int pos, Grid<CellObject> grid, GemEnum[] selectedGems)
        // {
        //     GemEnum gemEnum = selectedGems[Random.Range(0, selectedGems.Length)];
        //     Debug.Log($"{nameof(RandGemForDrop)}: {nameof(pos)}={pos} {nameof(gemEnum)}={gemEnum}");
        //     return gemEnum;
        // }
    }
}