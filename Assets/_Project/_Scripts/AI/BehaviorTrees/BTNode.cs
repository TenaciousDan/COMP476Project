using UnityEngine;

using System;
using System.Collections.Generic;

namespace Game.AI
{
    public abstract class BTNode
    {
        public enum EState { Running, Success, Failure }

        public EState State { get; protected set; }

        public abstract EState Evaluate();

        public float order;
    }
}
