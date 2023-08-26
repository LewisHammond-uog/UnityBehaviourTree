using UnityEngine;

namespace BT.AI.BehaviourTree.BaseTypes.Nodes
{
    /// <summary>
    /// Random Choice node that randomly selects a child to execute on entry
    /// </summary>
    public class RandomChoiceNode : CompositeNode
    {
        //Random child we have selected to execute
        private Node selectedChild;
    
        protected override void OnEnterNode()
        {
            //If we have no children then abort
            if (children == null || children.Count == 0)
            {
                return;
            }
        
            //Select a random child on entry
            int randomChildIndex = Random.Range(0, children.Count);
            selectedChild = children[randomChildIndex];
            //selectedChild = children[2];
        }

        protected override NodeStatus Update_Internal()
        {
            //If we have not successfully selected a child then fail
            if (selectedChild == null)
            {
                return NodeStatus.Fail;
            }

            return selectedChild.Update();
        }
    }
}
