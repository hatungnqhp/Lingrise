using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace AgeOfWar
{
    public class CellObject
    {
        private readonly Grid<CellObject> grid;
        protected Vector2Int pos;

        public Vector2Int Pos => pos;

        private readonly CellBg bg;
        private CellGem gem;

        public CellBg Bg => bg;
        public CellGem Gem => gem;
        public GemEnum GemEnum => gem.GemEnum;

        public CellObject(Grid<CellObject> grid, Vector2Int pos, GemEnum gemEnum)
        {
            this.grid = grid;
            this.pos = pos;

            bg = GridManager.Instance.CellBgFactory.CreateCellBg(
                grid, pos, GridManager.Instance.CellBgParent);
            gem = GridManager.Instance.CellGemFactory.CreateCellGem(
                grid, pos, this, gemEnum, GridManager.Instance.CellGemParent);
        }

        public void MovingGem(Vector2 delta)
        {
            Debug.Assert(gem != null);
            gem.MovingGem(delta);
        }

        public void AlignGem()
        {
            Debug.Assert(gem != null);
            Tween tween = gem.AlignGem();
            if (tween == null)
            {
                GridManager.ActionCompletedAlign?.Invoke();
                return;
            }
            tween.OnComplete(() => { GridManager.ActionCompletedAlign?.Invoke(); }).Play();
        }

        public void DiscardGem(Vector2Int selectDir)
        {
            Debug.Assert(gem != null);

            gem.ChangeToBlank();

            Tween tween = gem.DiscardGem(selectDir);
            Debug.Assert(tween != null);
            tween.OnComplete(() =>
            {
                //GameObject.Destroy(gem);
                GridManager.ActionCompletedDiscard?.Invoke(pos);
            });

            //gem = null;
        }

        static public void SwapGem(CellObject startCell, CellObject targetCell, bool doMoveBack = false)
        {
            Debug.Assert(startCell != null && targetCell != null);
            Debug.Assert(startCell.gem != null && targetCell.gem != null);

            Debug.Log($"{nameof(SwapGem)}: {startCell.Gem.GemEnum} {targetCell.Gem.GemEnum} {nameof(doMoveBack)}={doMoveBack}");
            Sequence seq = CellGem.SwapGem(startCell.gem, targetCell.gem, doMoveBack);
            if (seq == null) return;

            seq.Play().OnComplete(() =>
            {
                GridManager.ActionCompletedSwap?.Invoke();
            });

            if (!doMoveBack)
            {
                (startCell.gem, targetCell.gem) = (targetCell.gem, startCell.gem);
            }
        }

        public static void TryActivate(CellObject cell1, CellObject cell2)
        {
            Debug.Assert(cell1 != null && cell2 != null);
            Debug.Assert(cell1.Gem != null && cell2.Gem != null);

            if (CellGem.AreOmni(cell1.Gem, cell2.Gem)) return;

            Tween tween;
            CellObject activatedCell;
            if (cell1.gem.IsOmni)
            {
                tween = cell1.gem.ActivateGem(cell2.GemEnum);
                activatedCell = cell1;
            }
            else
            {
                tween = cell2.gem.ActivateGem(cell1.GemEnum);
                activatedCell = cell1;
            }

            //Debug.Assert(tween != null);
            if (tween == null)
            {
                GridManager.ActionCompletedActivate?.Invoke(activatedCell);
            }
            else
            {
                tween.OnComplete(() => { GridManager.ActionCompletedActivate?.Invoke(activatedCell); }).Play();
            }
        }

        static public Tween DropGem(Vector2Int startPos, int dropCells, Func<Vector2Int, CellGem> CreateGem)
        {
            Vector2Int targetPos = startPos + Vector2Int.down * dropCells;
            CellObject targetCell = GridManager.Instance.Grid.GetGridObject(targetPos);
            Debug.Assert(targetCell != null);

            CellGem targetGem = targetCell.gem;
            Debug.Assert(targetGem != null && targetGem.IsBlank);

            CellObject startCell;
            CellGem startGem;
            if (GridManager.Instance.Grid.IsInGrid(startPos))
            {
                startCell = GridManager.Instance.Grid.GetGridObject(startPos);
                Debug.Assert(startCell != null && startCell.gem != null);
                startGem = startCell.gem;
            }
            else
            {
                startCell = null;
                startGem = CreateGem(targetPos);
                Debug.Assert(startGem != null);
            }
            Debug.Log($"{nameof(DropGem)}: {nameof(startPos)}={startPos} to {nameof(targetPos)}={targetPos}");

            Tween tween = CellGem.DropGem(startGem, targetGem);

            if (startCell == null)
            {
                //Debug.Log($"{nameof(DropGem)}: Destroy {nameof(targetGem)}");
                GameObject.Destroy(targetGem.gameObject);
                targetCell.gem = startGem;
            }
            else
            {
                (startCell.gem, targetCell.gem) = (targetCell.gem, startCell.gem);
            }

            return tween;
        }

        public void LevelUpGem()
        {
            Debug.Assert(gem != null);
            gem.LevelUp();
        }

        public void ChangeToBlank()
        {
            Debug.Assert(gem != null);
            gem.ChangeToBlank();
        }

        public void ChangeGemToOmni()
        {
            Debug.Assert(gem != null);
            gem.ChangeToOmni();
        }

        public static bool AreSame(CellObject cell1, CellObject cell2)
        {
            if (cell1 == null || cell2 == null) return false;
            return cell1.Pos == cell2.Pos;
        }

        public static bool AreAdjacent(CellObject cell1, CellObject cell2)
        {
            if (cell1 == null || cell2 == null) return false;

            Vector2Int offset = cell1.GetOffsetTo(cell2);
            if (Mathf.Abs(offset.x) > 1 || Mathf.Abs(offset.y) > 1) return false;
            return Math.Abs(offset.x) == 1 != (Math.Abs(offset.y) == 1);
        }

        // public bool IsSame(CellObject targetCell)
        // {
        //     return this == targetCell;
        // }

        // public bool IsAdjacentTo(CellObject targetCell)
        // {
        //     if (targetCell == null) return false;

        //     Vector2Int offset = GetOffsetTo(targetCell);
        //     if (Mathf.Abs(offset.x) > 1 || Mathf.Abs(offset.y) > 1) return false;
        //     return Math.Abs(offset.x) == 1 != (Math.Abs(offset.y) == 1);
        // }

        // public bool IsSameOrAdjacent(CellObject targetCell)
        // {
        //     if (targetCell == null) return false;

        //     return IsSame(targetCell) || IsAdjacentTo(targetCell);
        // }

        public Vector2Int GetOffsetTo(CellObject targetCell)
        {
            Debug.Assert(targetCell != null);
            return new Vector2Int(targetCell.pos.x - pos.x, targetCell.pos.y - pos.y);
        }
    }
}