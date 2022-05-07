/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace PFC.Toolkit.Core
{
    [ExecuteInEditMode]
    [InitializeOnLoad]
    public static class Dependencies
    {
        public static bool VRCSDK = false;

        static Dependencies() 
        {
            Debug.Log("PFCToolkit, Checking Dependencies..."); 
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var types = assembly.GetTypes();
                foreach (var type in assembly.GetTypes())
                {
                    if (type.ToString().Contains("VRC_AvatarDescriptor")) VRCSDK = true;
                }
            }

            Debug.Log($"Dependencies Found: {(VRCSDK?"VRCSDK":"")}");
        }
    }
}
*/