using UnityEngine;

namespace AgeOfWar
{
    public static class Constants
    {
        public const int SELECTION_GEM_SLOTS = 4;
        public const int GAME_GEM_MAX_LEVEL = 4;

        public static readonly Vector2Int[] MoveDirs =
        {
            Vector2Int.zero,
            Vector2Int.right,
            Vector2Int.up,
            Vector2Int.left,
            Vector2Int.down
        };

        public static readonly Vector2Int[] CheckDirsMatch3 =
        {
            Vector2Int.right,
            Vector2Int.up,
            Vector2Int.left,
            Vector2Int.down
        };

        public static readonly Vector2Int[] CheckDirsMatchSqr2 =
        {
            new(1, 1),
            new(-1, 1),
            new(-1, -1),
            new(1, -1)
        };

        public static readonly Vector2Int[] CheckDirsCenter =
        {
            Vector2Int.right,
            Vector2Int.up
        };
    }
}