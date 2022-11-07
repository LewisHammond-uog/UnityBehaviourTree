#if UNITY_EDITOR
using System;
using System.Diagnostics;
using AI.BehaviourTree.BaseTypes;
using AI.BehaviourTree.BaseTypes.Nodes;
using BehaviourTreeEditorWindow;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using GraphNode = UnityEditor.Experimental.GraphView.Node;
using BTNode = AI.BehaviourTree.BaseTypes.Nodes.Node;


//Visual represnetation of a Behaviour Tree node in the BehaviourTreeView
public sealed class NodeView : GraphNode
{
    //The Behaviour Tree node that this node view is displaying
    public readonly BTNode node;
    
    //Input and output ports of this node
    public Port InputPort { private set; get; }
    public Port OutputPort { private set; get; }
    
    //Event for when this node is selected
    public Action<NodeView> OnNodeSelected;
    
    public NodeView(BTNode node) : base($"{BTEditorWindowLocations.styleSheetLocation}NodeView.uxml") //Set the uxml for the node layout
    {
        this.node = node;
        this.title = node.name;
        this.viewDataKey = node.guid;

        style.left = node.position.x;
        style.top = node.position.y;

        //Subscribe to when the BT node updates so we can update our visual state
        node.OnNodeUpdate += OnBTNodeUpdate;
        
        CreateInputPorts();
        CreateOutputPorts();
        SetupUXMLClasses();
    }

    /// <summary>
    /// Sets up the UXML class from the class of this node
    /// </summary>
    private void SetupUXMLClasses()
    {
        switch (node)
        {
            case ActionNode _:
                AddToClassList("action-node");
                break;
            case CompositeNode _:
                AddToClassList("composite-node");
                break;
            case DecoratorNode _:
                AddToClassList("decorator-node");
                break;
            case RootNode _:
                AddToClassList("root-node");
                break;
        }
    }

    public NodeView()
    {
        
    }

    private void CreateInputPorts()
    {
        //Root nodes do not have input ports
        if (node is RootNode)
        {
            return;
        }
        
        //Create input node - all types have the same number of inputs allowed - 1
        InputPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        InputPort.portName = "";
        InputPort.style.flexDirection = FlexDirection.Column;
        inputContainer.Add(InputPort);
    }
    
    private void CreateOutputPorts()
    {
        //Action nodes do not have outputs
        if (node is ActionNode)
        {
            return;
        }
        
        Port.Capacity outputPortCapacity = Port.Capacity.Single;

        switch (node)
        {
            case IHasChildren _:
                //Mutli Children = Multiple Outputs
                outputPortCapacity = Port.Capacity.Multi;
                break;
            case IHasChild _:
                //One AllTreeNodes can only have 1 output
                outputPortCapacity = Port.Capacity.Single;
                break;
        }
        
        OutputPort = InstantiatePort(Orientation.Vertical, Direction.Output, outputPortCapacity, typeof(bool));
        OutputPort.portName = "";
        OutputPort.style.flexDirection = FlexDirection.ColumnReverse;
        outputContainer.Add(OutputPort);
    }

    /// <summary>
    /// Set the position of the node
    /// </summary>
    /// <param name="position"></param>
    public void SetPosition(Vector2 position)
    {
        Rect rect = GetPosition();
        rect.x = position.x;
        rect.y = position.y;
        SetPosition(rect);
    }

    //Override position setting so that we can store it in the node
    public override void SetPosition(Rect newPos)
    {
        
        base.SetPosition(newPos);
        if (node)
        {
            node.position.x = newPos.xMin;
            node.position.y = newPos.yMin;
        }
    }

    public override void OnSelected()
    {
        base.OnSelected();
        OnNodeSelected?.Invoke(this);
    }

    /// <summary>
    /// Sort the children of the node view by their horizontal position
    /// This edits the children of the BT node
    /// </summary>
    public void SortChildren()
    {
        //Sort nodes by their horizontal position if they are composite nodes
        if(node is CompositeNode compositeNode)
        {
            compositeNode.SortChildren(SortByHorizontalPosition);
        }
    }

    /// <summary>
    /// Sort nodes by horizonal position - returns -1 if the right if further to the left
    /// and 1 if the oposite is true
    /// </summary>
    /// <remarks>Used to sort nodes in the tree by horziontal value</remarks>
    /// <param name="nodeA"></param>
    /// <param name="nodeB"></param>
    /// <returns></returns>
    private static int SortByHorizontalPosition(BTNode nodeA, BTNode nodeB)
    {
        return nodeA.position.x < nodeB.position.x ? -1 : 1;
    }
    
    //Function triggered when the BT Node updates
    private void OnBTNodeUpdate(in NodeStatus currentStatus)
    {
        UpdateVisualState(currentStatus);
    }

    /// <summary>
    /// Update the visual state of the node base on the current BT node state
    /// </summary>
    private void UpdateVisualState(in NodeStatus currentStatus)
    {
        //Define the names of classes that are used in UXML to edit the nodes
        //visuals based on state
        const string runningUXMLClass = "state-running";
        const string successUXMLClass = "state-success";
        const string failureUXMLClass = "state-failure";
        
        //Remove classes from previous update 
        RemoveFromClassList(runningUXMLClass);
        RemoveFromClassList(successUXMLClass);
        RemoveFromClassList(failureUXMLClass);
        
        
        if (Application.isPlaying)
        {
            switch (currentStatus)
            {
                case NodeStatus.Running:
                    if (node.IsRunning)
                    {
                        AddToClassList(runningUXMLClass);
                    }
                    break;
                case NodeStatus.Success:
                    AddToClassList(successUXMLClass);
                    break;
                case NodeStatus.Fail:
                    AddToClassList(failureUXMLClass);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(currentStatus), currentStatus, null);
            }
        }
    }

}
#endif