using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AgeOfWar
{
    public interface ITouchHandler
    {
        // TouchEnum TouchEnum { get; }
        bool TouchBegan();
        bool TouchMoved();
        bool TouchEnded();
    }
}

