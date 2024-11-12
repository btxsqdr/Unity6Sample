using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;


public class MainScreen : MonoBehaviour
{
    [SerializeField] private UIDocument _document;
    [SerializeField] private StyleSheet _styleSheet;

    private void Start()
    {
        StartCoroutine(Generate());
    }

    private void OnValidate()
    {
        if (Application.isPlaying) return;
        StartCoroutine(Generate());
    }

    private IEnumerator Generate()
    {
        yield return null;
            
        var root = _document.rootVisualElement;
        root.Clear();

        root.styleSheets.Add(_styleSheet);

        var container = Create("container", "bordered-box");
        
        var viewBox = Create("view-box", "bordered-box");
        container.Add(viewBox);
        
        var controlBox = Create("control-box", "bordered-box");
        container.Add(controlBox);
            
        root.Add(container);
    }
    
    private VisualElement Create(params string[] classNames)
    {
        return Create<VisualElement>(classNames);
    }

    private T Create<T>(params string[] classNames) where T : VisualElement, new()
    {
        var ele = new T();
        foreach (var className in classNames)
        {
            ele.AddToClassList(className);
        }

        return ele;
    }
}