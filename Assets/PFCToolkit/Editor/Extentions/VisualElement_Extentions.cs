using System;
using UnityEngine.UIElements;

namespace PFCToolkit.Lib { 
public static class VisualElement_Extentions
{
        public static T AddElement<T>(this VisualElement parent) where T : new()
        {
            T element = new T();
            if (element is VisualElement) parent.Add(element as VisualElement);
            else throw new Exception("Create Element was used on a non-VisualElement class.");
            return element;
        }
    }
}