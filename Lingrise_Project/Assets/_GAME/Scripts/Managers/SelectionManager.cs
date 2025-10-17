using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AgeOfWar;
using UnityEngine;

namespace AgeOfWar
{
    public class SelectionManager : MonoBehaviour
    {
        public static SelectionManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        [SerializeField] private GemEnum[] selectedGems = new GemEnum[Constants.SELECTION_GEM_SLOTS];
        public GemEnum[] SelectedGems
        {
            get
            {
                if (selectedGems.Length != Constants.SELECTION_GEM_SLOTS) return null;
                foreach (GemEnum gem in selectedGems)
                {
                    if (selectedGems.Count(x => x == gem) > 1) return null;
                }
                return selectedGems;
            }
        }
    }
}