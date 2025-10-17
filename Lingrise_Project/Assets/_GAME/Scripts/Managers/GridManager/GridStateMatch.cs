using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;

namespace AgeOfWar
{
    public class GridStateMatch : AGridState
    {
        public GridStateMatch(GridManager gridManager) : base(gridManager) { }

        private Grid<CellObject> grid;

        private Dictionary<Vector2Int, GemChangeType> matchChanges = new();
        private Dictionary<Vector2Int, Vector2Int> matchChecks;

        public override void Enter()
        {
            GridManager.ActionCompletedMatch += OnCompletedMatch;

            grid = gridManager.Grid;
            matchChecks = gridManager.matchChecks;
            matchChanges.Clear();

            HandleMatching();
        }

        public override void Exit()
        {
            GridManager.ActionCompletedMatch -= OnCompletedMatch;
        }

        private void OnCompletedMatch()
        {
            gridManager.matchChanges = matchChanges;
            gridManager.matchChecks.Clear();

            Debug.Assert(matchChanges != null);
            if (matchChanges.Count > 0)
            {
                gridManager.SwitchState(GridState.Drop);
                return;
            }
            gridManager.SwitchState(GridState.Select);
        }

        private void HandleMatching()
        {
            Debug.Assert(matchChecks != null);

            matchChanges = new();
            foreach (var matchCheck in matchChecks)
            {
                Vector2Int checkPos = matchCheck.Key;
                Vector2Int moveDir = matchCheck.Value;
                Debug.Log($"{nameof(HandleMatching)}: {nameof(checkPos)}={checkPos} {nameof(moveDir)}={moveDir}");

                int cntMatch3 = 0;
                int cntMatchSqr2 = 0;

                // MATCH 3
                foreach (Vector2Int dir in Constants.CheckDirsMatch3)
                {
                    if (dir == moveDir * -1) continue;
                    //Debug.Log($"{nameof(HandleMatching)}: MATCH3 {nameof(dir)}={dir} isMatch={CheckMatch3(checkPos, dir)}");
                    if (!CheckMatch3(checkPos, dir)) continue;

                    matchChanges[checkPos + dir * 2] = GemChangeType.ChangeToBlank;
                    matchChanges[checkPos + dir] = GemChangeType.LevelUp;
                    cntMatch3++;
                }

                // MATCH SQR2
                Vector2Int[] skipDirsMatchSqr2 = GetDiagonalDirs(moveDir * -1);
                Debug.Assert(skipDirsMatchSqr2 != null);
                foreach (Vector2Int dir in Constants.CheckDirsMatchSqr2)
                {
                    if (skipDirsMatchSqr2.Contains(dir)) continue;
                    //Debug.Log($"{nameof(HandleMatching)}: MATCHSQR2 {nameof(dir)}={dir} isMatch={CheckMatchSqr2(checkPos, dir)}");
                    if (!CheckMatchSqr2(checkPos, dir)) continue;

                    matchChanges[checkPos + dir] = GemChangeType.ChangeToBlank;
                    if (!matchChanges.ContainsKey(checkPos + Vector2Int.right * dir.x))
                    {
                        matchChanges[checkPos + Vector2Int.right * dir.x] = GemChangeType.ChangeToBlank;
                    }
                    if (!matchChanges.ContainsKey(checkPos + Vector2Int.up * dir.y))
                    {
                        matchChanges[checkPos + Vector2Int.up * dir.y] = GemChangeType.ChangeToBlank;
                    }
                    cntMatchSqr2 = 1;
                    break;
                }

                // CENTER CHECK
                if (cntMatchSqr2 >= 1)
                {
                    matchChanges[checkPos] = GemChangeType.ChangeToOmni;
                    continue;
                }

                if (cntMatch3 >= 2)
                {
                    matchChanges[checkPos] = GemChangeType.LevelUp;
                    //
                    continue;
                }

                foreach (Vector2Int dir in Constants.CheckDirsCenter)
                {
                    if (Mathf.Abs(dir.x) == moveDir.x && Mathf.Abs(dir.y) == moveDir.y) continue;

                    Vector2Int centerCheckPosB = checkPos + dir;
                    Vector2Int centerCheckPosE = checkPos + dir * -1;
                    Vector2Int centerCheckDir = dir * -1;

                    //Debug.Log($"{nameof(HandleMatching)}: CENTER CHECK {nameof(dir)}={dir} "
                    //    + $"isMatch={grid.IsInGrid(centerCheckPosB) && CheckMatch3(centerCheckPosB, centerCheckDir)}");

                    if (!grid.IsInGrid(centerCheckPosB)
                        || !CheckMatch3(centerCheckPosB, centerCheckDir)) continue;

                    matchChanges[checkPos] = GemChangeType.LevelUp;
                    if (!matchChanges.TryGetValue(centerCheckPosB, out var changeTypeB)
                        || changeTypeB != GemChangeType.LevelUp)
                    {
                        matchChanges[centerCheckPosB] = GemChangeType.ChangeToBlank;
                    }
                    if (!matchChanges.TryGetValue(centerCheckPosE, out var changeTypeE)
                        || changeTypeE != GemChangeType.LevelUp)
                    {
                        matchChanges[centerCheckPosE] = GemChangeType.ChangeToBlank;
                    }

                    break;
                }

                if (!matchChanges.TryGetValue(checkPos, out var changeType)
                    || changeType != GemChangeType.LevelUp)
                {
                    if (cntMatch3 >= 1)
                    {
                        matchChanges[checkPos] = GemChangeType.ChangeToBlank;
                    }
                }
            }

            Debug.Log($"{nameof(HandleMatching)}.matchChanges: "
                + string.Join(", ", matchChanges.Select(kvp => $"[{kvp.Key}]={kvp.Value}")));

            GridManager.ActionCompletedMatch.Invoke();
        }

        private bool CheckMatch3(Vector2Int pos, Vector2Int dir)
        {
            Debug.Assert(grid.IsInGrid(pos) && Constants.CheckDirsMatch3.Contains(dir));

            if (!grid.IsInGrid(pos + dir * 2)) return false;

            CellGem gem0 = grid.GetGridObject(pos).Gem;
            CellGem gem1 = grid.GetGridObject(pos + dir).Gem;
            CellGem gem2 = grid.GetGridObject(pos + dir * 2).Gem;

            //Debug.Log($"{nameof(CheckMatch3)}: [{pos}]={gem0}, [{pos + dir}]={gem1}, [{pos + dir * 2}]={gem2}");

            return CellGem.AreMatch(new CellGem[] { gem0, gem1, gem2 });
        }

        private bool CheckMatchSqr2(Vector2Int pos, Vector2Int dir)
        {
            Debug.Assert(grid.IsInGrid(pos) && Constants.CheckDirsMatchSqr2.Contains(dir));

            Vector2Int pos3 = pos + dir;
            if (!grid.IsInGrid(pos3)) return false;

            Vector2Int pos1 = pos + Vector2Int.right * dir.x;
            Vector2Int pos2 = pos + Vector2Int.up * dir.y;

            CellGem gem0 = grid.GetGridObject(pos).Gem;
            CellGem gem1 = grid.GetGridObject(pos1).Gem;
            CellGem gem2 = grid.GetGridObject(pos2).Gem;
            CellGem gem3 = grid.GetGridObject(pos3).Gem;

            //Debug.Log($"{nameof(CheckMatchSqr2)}: " +
            //        $"[{pos}]={gem0}, [{pos1}]={gem1}, [{pos2}]={gem2}, [{pos3}]={gem3}");

            return CellGem.AreMatch(new CellGem[] { gem0, gem1, gem2, gem3 });
        }

        private Vector2Int[] GetDiagonalDirs(Vector2Int dir)
        {
            Debug.Assert(Constants.MoveDirs.Contains(dir));

            if (dir == Vector2Int.zero) return new Vector2Int[] { };

            return dir.x != 0
                ? new[] { new Vector2Int(dir.x, 1), new Vector2Int(dir.x, -1) }
                : new[] { new Vector2Int(1, dir.y), new Vector2Int(-1, dir.y) };
        }
    }
}