#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using AI.BehaviourTree.BaseTypes;
using AI.BehaviourTree.BaseTypes.Nodes;
using BehaviourTreeEditorWindow;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using BTNode = AI.BehaviourTree.BaseTypes.Nodes.Node;
using GraphNode = UnityEditor.Experimental.GraphView.Node;

public class BehaviourTreeView : GraphView
{
    //Action for when a node in the graph is selected
    public Action<NodeView> OnNodeSelected;
    
    public new class  UxmlFactory : UxmlFactory<BehaviourTreeView, GraphView.UxmlTraits> {}

    private BehaviourTree currentTree;
    
    private NodeSearchWindow searchWindow;
    private EditorWindow owningEditorWindow;

    public BehaviourTreeView()
    {
        //Create a grid background
        Insert(0, new GridBackground());
        
        //Add mainpulators so that we can move around the grid view
        this.AddManipulator(new ContentDragger()); //Allow to pan around the graph
        this.AddManipulator(new ContentZoomer()); //Allow zooming in the graph
        this.AddManipulator(new SelectionDragger()); //Allow the movement of nodes in the graph
        this.AddManipulator(new RectangleSelector()); //Allow box selection

        //Subscribe to any graph view changes
        graphViewChanged += OnGraphViewChanged;
        
        // Add stylesheet to give elements their style - similar to CSS
        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{BTEditorWindowLocations.styleSheetLocation}BehaviourTreeEditor.uss");
        styleSheets.Add(styleSheet);
    }

    public void SetOwner(EditorWindow owner)
    {
        owningEditorWindow = owner;
        AddSearchWindow();
    }

    /// <summary>
    /// Add the search window to the graph
    /// </summary>
    private void AddSearchWindow()
    {
        searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
        searchWindow.Init(owningEditorWindow , this);
        nodeCreationRequest += context =>
            SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
    }

    /// <summary>
    /// Clear the tree of all nodes
    /// </summary>
    public void ClearGraph()
    {
        //Delete anything from an old population of the BT view
        //ignore changed elements while we do this or we will delete from the 
        //actual BT
        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;
    }

    /// <summary>
    /// Populate the behaviour tree view with the nodes from a behaviour tree
    /// </summary>
    /// <param name="tree">Tree to populate the view with</param>
    public void PopulateView(BehaviourTree tree)
    {
        this.currentTree = tree;
        
        ClearGraph();
        
        //Create a root node if one does not already exist
        if (tree.rootNode == null)
        {
            tree.rootNode = (RootNode) tree.CreateNode(typeof(RootNode));
            EditorUtility.SetDirty(tree);
            AssetDatabase.SaveAssets();
        }
        
        //Create AllTreeNodes for all of the nodes in the tree
        foreach (BTNode node in currentTree.AllTreeNodes)
        {
            if (node == null)
            {
                Debug.LogWarning("Null Node in Tree");
                continue;
            }
            
            CreateNodeView(node, node.position);
        }
        
        //Create Edges by getting the children of each node and connecting their children
        foreach (BTNode currentTreeNode in currentTree.AllTreeNodes)
        {
            //Warning is caught further up
            if (currentTreeNode == null)
            {
                continue;
            }
            
            List<BTNode> children = tree.GetChildren(currentTreeNode);
            NodeView parentView = FindNodeView(currentTreeNode);
            if (parentView == null)
            {
                continue;
            }
            
            foreach (BTNode child in children)
            {
                NodeView childView = FindNodeView(child);
                if (childView == null)
                {
                    continue;
                }
 
                Edge connection = parentView.OutputPort.ConnectTo(childView.InputPort);
                AddElement(connection);
            }
        }
    }

    //Called when any graph view elements are changed
    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        //Remove elements that are deleted from the graph in the Behaviour Tree
        if (graphViewChange.elementsToRemove != null)
        {
            foreach (GraphElement graphElement in graphViewChange.elementsToRemove)
            {
                //Remove nodes when they are deleted
                if (graphElement is NodeView nodeView)
                {
                    currentTree.DeleteNode(nodeView.node);
                }
                
                //Remove edges when they are deleted
                if (graphElement is Edge edge)
                {
                    if (edge.output.node is NodeView parentNodeView && edge.input.node is NodeView childNodeView)
                    {
                        currentTree.RemoveChild(parentNodeView.node, childNodeView.node);
                    }
                }
            }
        }

        if (graphViewChange.edgesToCreate != null)
        {
            foreach (Edge edge in graphViewChange.edgesToCreate)
            {
                //Parent node is the node that we output from
                //Child node is the node that we are inputting to
                if (edge.output.node is NodeView parentView && edge.input.node is NodeView childView)
                {
                    currentTree.AddChild(parentView.node, childView.node);
                }
            }
        }
        
        //Check if we moved elements if we did then reorder children so thier execuation order matches those on the graph
        if (graphViewChange.movedElements != null)
        {
            foreach (GraphNode node in nodes)
            {
                NodeView nodeView = node as NodeView;
                nodeView?.SortChildren();
            }
        }
        
        EditorUtility.SetDirty(currentTree);
        
        
        return graphViewChange;
    }

    //Override GetCompatiblePorts to define which ports can connect to which other ports
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(endPort =>
            endPort.direction != startPort.direction && //Make sure that we are connecting to different port directions (no input -> input)
            endPort.node != startPort.node //Do not allow nodes to connect to themselves
        ).ToList();
    }
    
    //Override the default behabiour of the content menu (when you right click on the graph)
    //to show the nodes we can create
    /*public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        //base.BuildContextualMenu(evt);
        TypeCache.TypeCollection actionTypes = TypeCache.GetTypesDerivedFrom<BTNode>();

        foreach (Type type in actionTypes)
        {
            //Exclude abstract classes
            if (type.IsAbstract)
            {
                continue;
            }

            if (type.BaseType != null)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type, Vector2.zero));
            }
        }
    }*/

    /// <summary>
    /// Create a node in the BT and create a visual element to represent it
    /// </summary>
    /// <param name="type"></param>
    public void CreateNode(System.Type type, Vector2 position)
    {
        if (!currentTree)
        {
            return;
        }

        BTNode createdNode = currentTree.CreateNode(type);
        if (createdNode)
        {
            CreateNodeView(createdNode, position);
        }
    }

    /// <summary>
    /// Create a visual representation of a BT node in the graph view
    /// </summary>
    /// <param name="node"></param>
    private void CreateNodeView(BTNode node, Vector2 position)
    {
        NodeView nodeVisualRep = new NodeView(node);
        nodeVisualRep.OnNodeSelected += OnNodeSelected;
        nodeVisualRep.SetPosition(position);
        AddElement(nodeVisualRep);
    }   

    /// <summary>
    /// Find a node view from a given BT Node
    /// </summary>
    /// <param name="btNode"> Behavior Tree node to find node view of</param>
    /// <returns>Node View of the given behaviour tree node</returns>
    private NodeView FindNodeView(BTNode btNode)
    {
        if (btNode == null)
        {
            Debug.LogError("Cannot find GUID of NULL node");
            return null;
        }
        
        return GetNodeByGuid(btNode.guid) as NodeView;
    }
}
#endif