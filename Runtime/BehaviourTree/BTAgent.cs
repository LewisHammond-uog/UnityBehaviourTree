using System;
using UnityEngine;
using UnityEngine.AI;

namespace AI.BehaviourTrees
{
    public class BTAgent : AI.BaseAgent 
    {
        [SerializeField] private BehaviourTree.BaseTypes.BehaviourTree treePrefab;
        public BehaviourTree.BaseTypes.BehaviourTree RunningTree { get; private set; }

        private void Start()
        {
            InitTree();
        }

        private void Update()
        {
            if (RunningTree)
            {
                RunningTree.Update();
            }
        }

        /// <summary>
        /// Initalize the tree to run on this agent
        /// </summary>
        private void InitTree()
        {
            //Clone the tree prefab so we have an unique instance that we can run
            if (treePrefab != null)
            {
                RunningTree = treePrefab.Clone();
                RunningTree.SetOwner(this);
            }
        }

        /// <summary>
        /// Disable this agent
        /// </summary>
        public void DisableAgent()
        {
            //navComponent.Stop();
            this.enabled = false;
        }
    }
}