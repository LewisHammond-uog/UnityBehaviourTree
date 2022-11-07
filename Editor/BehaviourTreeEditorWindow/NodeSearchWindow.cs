#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using AI.BehaviourTree.BaseTypes.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Node = AI.BehaviourTree.BaseTypes.Nodes.Node;

public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
{
    private BehaviourTreeView owningGraphView;
    private EditorWindow owningWindow;
    
    public void Init(EditorWindow window, BehaviourTreeView graphView)
    {
        owningWindow = window;
        owningGraphView = graphView;
    }
    
    //Create search tree
    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        //Get all of the node types
        TypeCache.TypeCollection nodeTypes = TypeCache.GetTypesDerivedFrom<Node>();
        
        //Add base level nodes
        List<SearchTreeEntry> searchTree = new List<SearchTreeEntry>()
        {
            new SearchTreeGroupEntry(new GUIContent("Create Elements"),0),
        };

        foreach (Type type in nodeTypes)
        {
            if (type.IsAbstract)
            {
                continue;
            }
            
            Node node = ScriptableObject.CreateInstance(type) as Node;
            if (node != null)
            {
                searchTree.Add(CreateTreeEntry(node));   
            }
            DestroyImmediate(node);
        }

        return searchTree;

    }
    
    /// <summary>
    /// Create a search entry to spawn a node when clicked
    /// </summary>
    /// <returns></returns>
    private SearchTreeEntry CreateTreeEntry(ScriptableObject node)
    {
        //Check we are operating on a node
        if (node == null || !(node is Node))
        {
            return null;
        }

        string parentClassName = node.GetType().BaseType != null ? node.GetType().BaseType.Name : string.Empty;
        string className = ObjectNames.NicifyVariableName(node.GetType().Name);

        string nodeName = $"[{parentClassName}] {className}";
        return new SearchTreeEntry(new GUIContent(nodeName))
        {
            level = 1,
            userData = node
        };
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        //Get the mouse position relative to the graph view
        Vector2 mousePosition =
            owningWindow.rootVisualElement.ChangeCoordinatesTo(owningWindow.rootVisualElement.parent,
                context.screenMousePosition - owningWindow.position.position);
        Vector2 localMousePosition = owningGraphView.contentViewContainer.WorldToLocal(mousePosition);
        
        owningGraphView.CreateNode(SearchTreeEntry.userData.GetType(),localMousePosition);
        return true;
    }
}
#endif