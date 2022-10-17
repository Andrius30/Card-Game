using System;
using System.Collections.Generic;
using UnityEngine;

namespace Andrius.Core.Utils
{
    /// <summary>
    /// This class holds global accessable helper methods
    /// </summary>
    public static class StaticFunctions
    {
        public static Transform FindChild(Transform parent, string childName, bool includeInactive = true)
        {
            foreach (var ch in parent.GetComponentsInChildren<Transform>(includeInactive))
            {
                if (parent.name != ch.name)
                {
                    if (ch.childCount > 0)
                    {
                        foreach (var c in ch.GetComponentsInChildren<Transform>(includeInactive))
                        {
                            if (c.name == childName)
                            {
                                return c;
                            }
                        }
                    }
                    if (ch.name == childName)
                    {
                        return ch;
                    }
                }
            }
            return null;
        }
        public static T FindTypeInChildrens<T>(Transform parent, string name = "", bool includeInactive = true) where T : Component
        {
            if (!string.IsNullOrEmpty(name))
            {
                var obj = FindChild(parent, name);
                if(obj == null)
                {
                    return null;
                }
                T t = obj.GetComponent<T>();
                if (t != null)
                {
                    return t;
                }
            }
            foreach (var ch in parent.GetComponentsInChildren<T>(includeInactive))
            {
                if (parent.name != ch.transform.name)
                {
                    if (ch is T)
                    {
                        return ch;
                    }
                }
            }
            return null;
        }
        public static List<T> GetAllComponentsInChildren<T>(Transform parent, bool includeInactive = true)
        {
            List<T> components = new List<T>();
            foreach (var item in parent.GetComponentsInChildren<T>(includeInactive))
            {
                if (item != null)
                {
                    components.Add(item);
                }
            }
            return components;
        }
        
        
        // ============= MEMORY MANAGEMENT =========================

        public static T LoadFromResourcesAndCreateObject<T>(string pathToModel) where T : Component
        {
            var t = Resources.Load<GameObject>(pathToModel);
            GameObject gm = MonoBehaviour.Instantiate(t);
            T comp = gm.GetComponent<T>();
            return comp;
        }
    }
}
