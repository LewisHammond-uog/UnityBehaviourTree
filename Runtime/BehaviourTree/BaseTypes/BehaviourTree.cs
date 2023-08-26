using System.Collections.Generic;
using System.Linq;
using BT.AI.BehaviourTree.BaseTypes.Nodes;
using BT.AI.BehaviourTrees;
using BT.AI.Blackboard;
using UnityEditor;
using UnityEngine;

namespace BT.AI.BehaviourTree.BaseTypes
{
    [CreateAssetMenu(menuName = "AI/Behaviour Tree")]
    public class BehaviourTree : ScriptableObject
    {
        //Owner (runner) of the tree
        private BTAgent owner;
        
        //Blackboard used for this tree instance
        public AIBlackboard selfBlackboard;
        
        public RootNode rootNode;
        public NodeStatus treeState;

        //All of the nodes in the tree - including those that are not connected to
        //the root
        [SerializeField] private List<Node> nodes;
        public List<Node> AllTreeNodes
        {
            set => nodes = value;
            get => nodes;
        }

        public BehaviourTree()
        {
            treeState = NodeStatus.Running;
            nodes = new List<Node>();
            
            //Create a blackboard and set it in the child nodes
            selfBlackboard = new AIBlackboard();
            TraverseChildNodes(rootNode, node => { node.AgentBlackboard = selfBlackboard; });
        }
        
        public NodeStatus Update()
        {
            return rootNode.Update();
        }

        /// <summary>
        /// Do a depth first traversal of the tree, triggering an event each time we reach a new node
        /// </summary>
        /// <param name="node">Start Node</param>
        /// <param name="visitor">Event Triggered when we find a node</param>
        private void TraverseChildNodes(Node node, System.Action<Node> visitor)
        {
            if (node)
            {
                visitor.Invoke(node);
                List<Node> children = GetChildren(node);
                foreach (Node child in children)
                {
                    TraverseChildNodes(child, visitor);
                }
            }
        }

        /// <summary>
        /// Get a clone of this behaviour tree
        /// </summary>
        /// <returns></returns>
        public BehaviourTree Clone()
        {
            BehaviourTree tree = Instantiate(this);
            tree.rootNode = (RootNode) tree.rootNode.Clone();
            tree.AllTreeNodes = new List<Node>();
            //Traverse the tree to find all of the children of the root node and add to the list of nodes
            TraverseChildNodes(tree.rootNode, (n) =>{ tree.AllTreeNodes.Add(n);});
            
            selfBlackboard = new AIBlackboard();

            return tree;
        }

        /// <summary>
        /// Set the owner of this behaviour tree
        /// </summary>
        /// <param name="newOwner"></param>
        public void SetOwner(BTAgent newOwner)
        {
            //Set the newOwner of all the child nodes
            TraverseChildNodes(rootNode, node => { node.Owner = newOwner; });
            TraverseChildNodes(rootNode, node => { node.AgentBlackboard = selfBlackboard; });
        }
        
        public Node CreateNode(System.Type nodeType)
        {
            //Create an instance of the node
            Node node = ScriptableObject.CreateInstance(nodeType) as Node;
            if (!node)
            {
                return null;
            }
            
            //Set the node properties and give it a GUID to be a unique indenifier
            node.name = nodeType.Name;
            node.guid = System.Guid.NewGuid().ToString();
            AllTreeNodes.Add(node);

            //Add this as a sub asset of the tree asset
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(node, this);
                AssetDatabase.SaveAssets();
            }
            #endif

            return node;
        }

        /// <summary>
        /// Delete a node from the tree
        /// </summary>
        /// <param name="node"></param>
        public void DeleteNode(Node node)
        {
            //Null check node
            if (node == null)
            {
                return;
            }
            
            //Remove node from our refrence list and remove it as sub asset
            AllTreeNodes.Remove(node);
            #if UNITY_EDITOR
            AssetDatabase.RemoveObjectFromAsset(node);
            AssetDatabase.SaveAssets();
            #endif
        }

        /// <summary>
        /// Add a child to a given parent node
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="childNode"></param>
        public void AddChild(Node parentNode, Node childNode)
        {
            if (parentNode == null || childNode == null)
            {
                return;
            }

            switch (parentNode)
            {
                //Change node with single children
                case IHasChild nodeWithSingleChild:
                    nodeWithSingleChild.SetChild(childNode);
                    break;
                //Change nodes with multiple children
                case IHasChildren nodeWithMultipleChildren:
                    nodeWithMultipleChildren.AddChild(childNode);
                    break;
            }
        }

        /// <summary>
        /// Remove a child from a given parent node
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="childNode"></param>
        public void RemoveChild(Node parentNode, Node childNode)
        {   
            if (parentNode == null || childNode == null)
            {
                return;
            }

            switch (parentNode)
            {
                //Remove the single child from the decorator node - only if the supplied child is the actual child
                case IHasChild nodeWithSingleChild:
                    nodeWithSingleChild.SetChild(null);
                    return;
                //Remove child from the list of children in the Composite Node
                case IHasChildren nodeWithMultipleChildren:
                    nodeWithMultipleChildren.RemoveChild(childNode);
                    return;
            }
        }

        /// <summary>
        /// Get all of the children of a given node
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public List<Node> GetChildren(Node parent)
        {
            List<Node> children = new List<Node>();

            switch (parent)
            {
                //Add the single child to the list as we only have one child
                case IHasChild nodeWithSingleChild when nodeWithSingleChild.GetChild() != null:
                    children.Add(nodeWithSingleChild.GetChild());
                    break;
                //Set the list to be all of the nodes children
                case IHasChildren nodeWithMultipleChildren:
                    children = nodeWithMultipleChildren.GetChildren().ToList();
                    break;
            }

            return children;
        }
    }
}