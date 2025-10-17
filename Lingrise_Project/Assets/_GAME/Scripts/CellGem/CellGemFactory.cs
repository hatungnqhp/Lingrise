using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;

namespace AgeOfWar
{
    [CreateAssetMenu(fileName = "CellGemFactory", menuName = "AgeOfWar/SO/CellGemFactory")]
    public class CellGemFactory : ScriptableObject
    {
        [SerializeField] private CellGem cellGemPf;
        [SerializeField] private CellGemMapping[] cellGemMappings;

        private CellGemVisualPack _visualPack;
        private CellGemVisualPack VisualPack
        {
            get
            {
                if (_visualPack == null)
                {
                    _visualPack = new CellGemVisualPack(cellGemMappings);
                }
                return _visualPack;
            }
        }

        private GemAttackBehaviourPack _gemAttackBehaviourPack;
        private GemAttackBehaviourPack GemAttackBehaviourPack
        {
            get
            {
                if (_gemAttackBehaviourPack == null)
                {
                    _gemAttackBehaviourPack = new GemAttackBehaviourPack(cellGemMappings);
                }
                return _gemAttackBehaviourPack;
            }
        }

        public CellGem CreateCellGem(Grid<CellObject> grid, Vector2Int gridPos,
            CellObject cell, GemEnum gemType, Transform parent)
        {
            Debug.Assert(grid != null);
            Debug.Assert(gemType != GemEnum.Blank);

            CellGem cellGem = Instantiate(cellGemPf, parent);
            Vector3 pos = grid.GetGridObjectPos(gridPos);

            cellGem.Setup(pos, cell, gemType, VisualPack, GemAttackBehaviourPack);
            return cellGem;
        }
    }

    [Serializable]
    public struct CellGemMapping
    {
        public GemEnum gemType;
        public Sprite[] visuals;
        public GemAttackBehaviourBase attackBehaviour;
    }

    public class CellGemVisualPack
    {
        private Dictionary<GemEnum, Sprite[]> visualPack;

        public CellGemVisualPack(CellGemMapping[] gemMappings)
        {
            Debug.Assert(gemMappings != null);

            visualPack = new();
            foreach (var gemMapping in gemMappings)
            {
                GemEnum gemType = gemMapping.gemType;
                Sprite[] visuals = gemMapping.visuals;
                Debug.Assert(gemType != GemEnum.Blank);
                Debug.Assert(!visualPack.ContainsKey(gemType));
                Debug.Assert(visuals != null && visuals.Length - 1 == Constants.GAME_GEM_MAX_LEVEL);

                visualPack[gemType] = visuals;
            }
        }

        public Sprite GetVisual(GemEnum gemEnum, int GemLevel)
        {
            Debug.Assert(visualPack != null);
            Debug.Assert(gemEnum != GemEnum.Blank);

            GemLevel = Math.Clamp(GemLevel, 0, Constants.GAME_GEM_MAX_LEVEL);
            if (visualPack.TryGetValue(gemEnum, out var visuals))
            {
                return visuals[GemLevel];
            }
            Debug.Assert(true);
            return null;
        }
    }

    public class GemAttackBehaviourPack
    {
        private Dictionary<GemEnum, GemAttackBehaviourBase> attackBehaviourPack;

        public GemAttackBehaviourPack(CellGemMapping[] cellGemMappings)
        {
            Debug.Assert(cellGemMappings != null);

            attackBehaviourPack = new();
            foreach (var cellGemMapping in cellGemMappings)
            {
                GemEnum gemType = cellGemMapping.gemType;
                GemAttackBehaviourBase attackBehaviour = cellGemMapping.attackBehaviour;
                Debug.Assert(gemType != GemEnum.Blank);
                Debug.Assert(!attackBehaviourPack.ContainsKey(gemType));
                Debug.Assert(attackBehaviour != null);

                attackBehaviourPack[gemType] = attackBehaviour;
            }
        }

        public GemAttackBehaviourBase GetAttackBehaviour(GemEnum gemType)
        {
            Debug.Assert(attackBehaviourPack != null);
            Debug.Assert(gemType != GemEnum.Blank);

            if (attackBehaviourPack.TryGetValue(gemType, out var attackBehaviour))
            {
                return attackBehaviour;
            }
            Debug.Assert(true);
            return null;
        }
    }
}