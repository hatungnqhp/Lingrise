using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace AgeOfWar
{
    public partial class CellGem : MonoBehaviour
    {
        // private CellGemDefault cellGemDefault = new CellGemDefault();
        // private CellGemCharged cellGemCharged = new CellGemCharged();
        // private ACellGem current;

        private CellObject cell;
        private CellGemVisualPack visualPack;
        private GemAttackBehaviourPack attackBehaviourPack;
        //public CellGemVisualPack CellGemVisualPack => cellGemVisualPack;

        private int _gemLevel;
        public int GemLevel
        {
            private set { _gemLevel = value; SetVisual(); }
            get => _gemLevel;
        }
        private bool _isOmni;
        public bool IsOmni
        {
            private set { _isOmni = value; SetVisual(); }
            get => _isOmni;
        }

        private GemEnum gemEnum;
        public GemEnum GemEnum => gemEnum;
        public bool IsBlank => gemEnum == GemEnum.Blank;

        [SerializeField] private Transform centerAnchor;
        [SerializeField] private SpriteRenderer visual;
        [SerializeField] private GemAttackBehaviourBase attackBehaviour;
        public Transform CenterAnchor => centerAnchor;
        private Vector3 originalCenterPos;

        static private readonly float alignDuration = 0.2f;
        static private readonly float swapDuration = 0.5f;
        static private readonly float dropDuration = 0.5f;

        public void Setup(Vector3 pos, CellObject cell, GemEnum gemEnum,
            CellGemVisualPack cellGemVisualPack, GemAttackBehaviourPack gemAttackBehaviourPack)
        {
            transform.position = pos;
            this.cell = cell;
            visualPack = cellGemVisualPack;
            attackBehaviourPack = gemAttackBehaviourPack;
            this.gemEnum = gemEnum;
            GemLevel = 0;
            IsOmni = false;
            originalCenterPos = CenterAnchor.position;
        }

        public void MovingGem(Vector2 delta)
        {
            float moveDistant = delta.magnitude;
            moveDistant = Mathf.Clamp(moveDistant, 0f, GridManager.Instance.CellSize);
            delta = delta.normalized * moveDistant;

            //Debug.Log($"{nameof(MovingGem)}: {nameof(moveDistant)}={moveDistant} {nameof(delta)}={delta}");

            centerAnchor.position = originalCenterPos + (Vector3)delta;
        }

        public Tween AlignGem()
        {
            return centerAnchor
                .DOMove(originalCenterPos, alignDuration)
                .From(centerAnchor.position);
        }

        public Tween DiscardGem(Vector2Int selectDir)
        {
            Vector3 moveOffset =
                new Vector3(selectDir.x, selectDir.y, 0) * GridManager.Instance.CellSize;
            Tween tween = centerAnchor
                .DOMove(originalCenterPos + moveOffset, dropDuration)
                .From(centerAnchor.position);

            //cell = null;
            return tween;
        }

        static public Sequence SwapGem(CellGem startGem, CellGem targetGem, bool doMoveBack = false)
        {
            if (startGem == null || targetGem == null) return null;

            Transform startAnchor = startGem.centerAnchor;
            Transform targetAnchor = targetGem.centerAnchor;
            Vector3 startPos = startGem.originalCenterPos;
            Vector3 targetPos = targetGem.originalCenterPos;

            Tween startTween;
            Tween targetTween;
            Tween startTweenMoveBack;
            Tween targetTweenMoveBack;

            Sequence seq = DOTween.Sequence().Pause();

            if (doMoveBack)
            {
                startTween = startAnchor
                    .DOMove(targetPos, swapDuration / 2)
                    .From(startAnchor.position);
                startTweenMoveBack = startAnchor
                    .DOMove(startPos, swapDuration / 2)
                    .From(targetPos);

                targetTween = targetAnchor
                    .DOMove(startPos, swapDuration / 2)
                    .From(targetAnchor.position);
                targetTweenMoveBack = targetAnchor
                    .DOMove(targetPos, swapDuration / 2)
                    .From(startPos);

                Sequence startSeq = DOTween.Sequence();
                startSeq.Append(startTween);
                startSeq.Append(startTweenMoveBack);

                Sequence targetSeq = DOTween.Sequence();
                targetSeq.Append(targetTween);
                targetSeq.Append(targetTweenMoveBack);

                seq.Append(startSeq);
                seq.Join(targetSeq);
            }
            else
            {
                startTween = startAnchor
                    .DOMove(targetPos, swapDuration)
                    .From(startAnchor.position);
                targetTween = targetAnchor
                    .DOMove(startPos, swapDuration)
                    .From(targetAnchor.position);

                (startGem.cell, targetGem.cell) = (targetGem.cell, startGem.cell);
                (startGem.originalCenterPos, targetGem.originalCenterPos) =
                    (targetGem.originalCenterPos, startGem.originalCenterPos);

                seq.Append(startTween);
                seq.Join(targetTween);
            }

            return seq;
        }

        public Tween ActivateGem(GemEnum targetGemEnum = GemEnum.Blank)
        {
            Debug.Assert(gemEnum != GemEnum.Blank);
            if (!IsOmni) return null;

            if (targetGemEnum == GemEnum.Blank)
            {
                IsOmni = false;
                return null; //
            }

            gemEnum = targetGemEnum;
            IsOmni = false;
            Debug.Log($"{nameof(ActivateGem)}: {nameof(gemEnum)}|{cell?.Pos}");
            return null; //
        }

        static public Tween DropGem(CellGem startGem, CellGem targetGem)
        {
            if (startGem == null || targetGem == null) return null;

            Transform startAnchor = startGem.centerAnchor;
            Transform targetAnchor = targetGem.centerAnchor;
            Vector3 startPos = startGem.originalCenterPos;
            Vector3 targetPos = targetGem.originalCenterPos;

            (startGem.cell, targetGem.cell) = (targetGem.cell, startGem.cell);
            (startGem.originalCenterPos, targetGem.originalCenterPos) =
                (targetGem.originalCenterPos, startGem.originalCenterPos);

            Tween tween = startAnchor.DOMove(targetPos, dropDuration).From(startAnchor.position).Pause();
            targetAnchor.position = startPos;

            return tween;
        }

        public void LevelUp() // when cant lvup
        {
            if (IsBlank || IsOmni)
            {
                Debug.LogError($"{nameof(LevelUp)}: {nameof(IsBlank)}={IsBlank} {nameof(IsOmni)}={IsOmni}");
                return;
            }

            int newGemLevel = GemLevel + 1;
            if (newGemLevel > Constants.GAME_GEM_MAX_LEVEL)
            {
                Debug.LogWarning($"{nameof(LevelUp)}: Max lv reached");
                return;
            }
            GemLevel = newGemLevel;
        }

        public void ChangeToBlank()
        {
            gemEnum = GemEnum.Blank;
        }

        public void ChangeToOmni()
        {
            IsOmni = true;
        }

        public static bool AreSame(CellGem[] cellGems)
        {
            Debug.Assert(cellGems != null && cellGems.Length >= 2);

            CellGem sampleGem = cellGems[0];
            if (sampleGem == null || sampleGem.IsBlank) return false;

            for (int i = 1; i < cellGems.Length; i++)
            {
                CellGem gem = cellGems[i];
                if (gem == null || gem.IsBlank) return false;

                if (gem.GemEnum != sampleGem.GemEnum
                    || gem.GemLevel != sampleGem.GemLevel) return false;
            }
            return true;
        }

        public static bool AreMatch(CellGem[] cellGems)
        {
            Debug.Assert(cellGems != null && cellGems.Length >= 2);

            CellGem sampleGem = cellGems[0];
            if (sampleGem == null || sampleGem.IsBlank || sampleGem.IsOmni) return false;

            for (int i = 1; i < cellGems.Length; i++)
            {
                CellGem gem = cellGems[i];
                if (gem == null || gem.IsBlank || gem.IsOmni) return false;

                if (gem.GemEnum != sampleGem.GemEnum
                    || gem.GemLevel != sampleGem.GemLevel) return false;
            }
            return true;
        }

        public static bool AreOmni(CellGem gem1, CellGem gem2)
        {
            if (gem1 == null || gem2 == null) return false;
            return gem1.IsOmni && gem2.IsOmni;
        }

        private void SetVisual()
        {
            if (gemEnum == GemEnum.Blank)
            {
                visual.sprite = null;
                return;
            }

            visual.sprite = visualPack.GetVisual(IsOmni ? GemEnum.Omni : gemEnum, GemLevel);
        }

        // private void Start()
        // {
        //     current = cellGemDefault;
        //     current.EnterState(this);
        // }

        // private void SwitchState(ACellGem state)
        // {
        //     current = state;
        //     state.EnterState(this);
        // }

        // private void Update()
        // {
        //     //state.Update(this);
        // }
    }
}

