using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SplitView : TwoPaneSplitView
{
    //Factory that allows unity to see this Element
    public new class UxmlFactory : UxmlFactory<SplitView, UxmlTraits>
    {
    }
}