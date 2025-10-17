using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfWar
{
    public class GridManager : Singleton<GridManager>, IManager
    {
        private Dictionary<GridState, AGridState> states;
        private GridState currentStateType;
        private GridState lastStateType;
        private AGridState currentState;

        public GridState LastStateType => lastStateType;
        public AGridState CurrentState => currentState;

        public CellObject selectedCell;
        public CellObject targetCell;
        public Vector2Int selectDir;
        public bool doMoveBack;

        public Dictionary<Vector2Int, Vector2Int> matchChecks = new();
        public Dictionary<Vector2Int, GemChangeType> matchChanges;

        public static Action<CellObject, Vector2Int> ActionCompletedSelect;
        public static Action<CellObject, Vector2> ActionMovingGem;
        public static Action ActionCompletedAlign;
        public static Action<Vector2Int> ActionCompletedDiscard;
        public static Action ActionCompletedSwap;
        public static Action<CellObject> ActionCompletedActivate;
        public static Action ActionCompletedMatch;
        public static Action ActionCompletedDrop;

        [SerializeField] private GridState gridState;

        [Header("Grid")]
        [SerializeField] private int rows = 6;
        [SerializeField] private int cols = 6;
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private Vector3 gridPos = Vector3.zero;

        public int Rows => rows;
        public int Cols => cols;
        public float CellSize => cellSize;
        public float Width => cellSize * cols;
        public float Height => cellSize * rows;
        public Vector3 GridPos => gridPos;

        private GemEnum[] selectedGems;
        public GemEnum[] SelectedGems => selectedGems;

        public Grid<CellObject> Grid;

        [Header("Grid Visual")]
        [SerializeField] private Image bg;
        [SerializeField] private float borderSize = 0.2f;

        public Image Bg => bg;
        public float BgWidth => Width + borderSize * 2;
        public float BgHeight => Height + borderSize * 2;
        public float BorderSize => borderSize;

        [Header("CellObject")]
        [SerializeField] private Transform cellBgParent;
        [SerializeField] private Transform cellGemParent;

        [SerializeField] private CellBgFactory cellBgFactory;
        [SerializeField] private CellGemFactory cellGemFactory;

        public Transform CellBgParent => cellBgParent;
        public Transform CellGemParent => cellGemParent;

        public CellBgFactory CellBgFactory => cellBgFactory;
        public CellGemFactory CellGemFactory => cellGemFactory;

        public void OnInit()
        {
            selectedGems = SelectionManager.Instance.SelectedGems;

            states = new Dictionary<GridState, AGridState>
            {
                { GridState.Init, new GridStateInit(this) },
                { GridState.Select, new GridStateSelect(this) },
                { GridState.Align, new GridStateAlign(this) },
                { GridState.Swap, new GridStateSwap(this) },
                { GridState.Discard, new GridStateDiscard(this) },
                { GridState.Activate, new GridStateActivate(this) },
                { GridState.Match, new GridStateMatch(this) },
                { GridState.Drop, new GridStateDrop(this) }
            };
            currentStateType = GridState.Init;
            currentState = states[GridState.Init];
            currentState.Enter();
        }

        public bool OnUpdateGameplay()
        {
            Debug.Assert(currentState != null);
            return currentState.Update();
        }

        public void SwitchState(GridState newStateType)
        {
            Debug.Assert(currentStateType != newStateType);

            Debug.Log($"<color=cyan>{nameof(SwitchState)}: {currentStateType} -> {newStateType}</color>");

            currentState?.Exit();
            lastStateType = currentStateType;
            currentStateType = newStateType;
            currentState = states[newStateType];
            currentState.Enter();
        }
    }
}

