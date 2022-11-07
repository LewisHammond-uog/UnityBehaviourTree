using System;
using System.Collections.Generic;
using UnityEngine;

namespace AI.BehaviourTree.BaseTypes.Nodes
{
    public abstract class CompositeNode : Node, IHasChildren
    {
        
        //Connected Children of this node
        [SerializeField] protected List<Node> children = new List<Node>();
        //List of nodes that should always be checked when executing the composite
        [SerializeField] protected List<Node> alwaysCheckNodes = new List<Node>();

        /// <summary>
        /// Creates the list of nodes to always check
        /// </summary>
        protected void CollectAlwaysCheckNodes()
        {
            foreach (Node child in children)
            {
                if (child.AlwaysCheck && !alwaysCheckNodes.Contains(child))
                {
                    alwaysCheckNodes.Add(child);
                }
            }
        }

        /// <summary>
        /// Check all of the nodes that we should always check, returning success if they all passed
        /// and fail if one is failed or is "running"
        /// </summary>
        /// <returns></returns>
        protected NodeStatus CheckAlwaysCheckNodes()
        {
            //If there are no nodes to check then we shouldn't report a failure
            if (alwaysCheckNodes == null || alwaysCheckNodes.Count == 0)
            {
                return NodeStatus.Success;
            }
            
            foreach (Node node in alwaysCheckNodes)
            {
                NodeStatus status = node.Update();

                switch (status)
                {
                    case NodeStatus.Fail:
                        return NodeStatus.Fail;
                    case NodeStatus.Running:
                        Debug.LogWarning("A Always check node returned running, avoid having nodes that are always checked return running becuase they could get suck");
                        return NodeStatus.Fail;
                    case NodeStatus.Success:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            //No nodes failed or were running - success!
            return NodeStatus.Success;
        }

        /// <summary>
        /// Add a child to the composite node at the end of the list
        /// </summary>
        /// <param name="node">Node to add to children</param>
        public void AddChild(Node node)
        {
            children ??= new List<Node>();
            children.Add(node);
        }

        /// <summary>
        /// Remove a child from the composite node
        /// </summary>
        /// <param name="node">Node to removes</param>
        public void RemoveChild(Node node)
        {
            children ??= new List<Node>();
            children.Remove(node);
        }

        /// <summary>
        /// Get the children of this node
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Node> GetChildren()
        {
            children ??= new List<Node>();
            return children;
        }

        /// <summary>
        /// Sort children by a given comparision
        /// </summary>
        /// <param name="comparison">Comparision to use</param>
        public void SortChildren(Comparison<Node> comparison)
        {
            children?.Sort(comparison);
        }
        
        /// <summary>
        /// Create a clone of this node and it's children
        /// </summary>
        /// <returns></returns>
        public override Node Clone()
        {
            CompositeNode node = Instantiate(this);
            node.children = children.ConvertAll(child => child.Clone());
            return node;
        }
    }
}