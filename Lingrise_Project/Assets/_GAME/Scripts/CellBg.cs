using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AgeOfWar
{
    public class CellBg : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer bg;

        [SerializeField] private Transform botLetfAnchor;

        public void Setup(Sprite bgSprite, Color bgColor)
        {
            bg.sprite = bgSprite;
            bg.color = bgColor;
        }

        public void SetBg(SpriteRenderer bg)
        {
            this.bg = bg;
        }

        public void SetColor(Color color)
        {
            bg.color = color;
        }
    }
}

