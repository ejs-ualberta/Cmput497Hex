using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class Display : MonoBehaviour
{
    //This class is the base unit of groups of UI elements. Currently DisplayManager assumes at most 1 display is shown at once but this assumption could be changed.

    //Treat other gameobjects as if they're children of this display so that they will be shown/hidden along with the actual children.
    //This is useful for UI objects that have different anchor constraints and therefore cannot be true children of the display object.
    [SerializeField] internal Transform[] VirtualChildren;
    private bool _isShowing;



    public bool IsShowing
    {
        get { return _isShowing; }
    }

    protected virtual void Awake()
    {
        _isShowing = false;
        foreach (Transform child in transform)
            _isShowing = _isShowing || child.gameObject.activeSelf;


    }

    public virtual void Show()
    {
        if (IsShowing)
            return;
        _isShowing = true;
        foreach(Transform child in transform)
            child.gameObject.SetActive(true);
        foreach(Transform child in VirtualChildren)
            child.gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        if (!IsShowing)
            return;
        _isShowing = false;
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
        foreach (Transform child in VirtualChildren)
            child.gameObject.SetActive(false);
    }

    public void ToggleVisibility()
    {

        if (IsShowing)
        {
            Hide();
            return;
        }
        Show();
    }
}
