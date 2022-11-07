using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
public class InspectorView : VisualElement
{
    //Factory that allows unity to see this Element
    public new class  UxmlFactory : UxmlFactory<InspectorView, InspectorView.UxmlTraits> {}

    private Editor nodeInspector;

    public void UpdateSelection(NodeView nodeView)
    {
        //Clear previous elements
        Clear();
        UnityEngine.Object.DestroyImmediate(nodeInspector);
        
        if (nodeView == null || nodeView.node == null)
        {
            return;
        }
        
        //Create an editor (inspector) of the properties expose on the node
        nodeInspector = Editor.CreateEditor(nodeView.node);
        IMGUIContainer container = new IMGUIContainer(nodeInspector.OnInspectorGUI);
        Add(container);
    }
}
#endif