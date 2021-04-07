using UnityEngine;

using System;
using System.Collections.Generic;

using Tenacious.Data;

namespace Game.AI
{
    public class BehaviorTree : MonoBehaviour
    {
        

        protected BTNode rootNode;

        public BehaviorTree(BTNode root)
        {
            this.rootNode = root;
        }

        public static BehaviorTree ConstructFromBTGraphData(BTGraphData data)
        {
            // TODO: create concrete rootNode and construct tree from there.

            return null;
        }
    }
}
