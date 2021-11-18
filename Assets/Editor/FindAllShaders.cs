using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Internal;
using UnityEditor;
using UnityEngine;

public class FindAllShaders 
{
    [MenuItem("Test/ChangeShader")]
    public static void TestShaders()
    {
        var allgos = GetAllObjectsOnlyInScene();
        var sparkle = GOWithName(allgos, "WandSparkleGreen");
        HashSet<string> shaderNames = new HashSet<string>();
        var simpleLit = Shader.Find("Universal Render Pipeline/Simple Lit");
        Debug.Log($"Is simple lit there {simpleLit != null}");

        void ForeachSharedMaterial(Action<Material> materialAction)
        {
            foreach (var gameObject in allgos)
            {
                foreach (var renderer in gameObject.GetComponents<Renderer>())
                {
                    foreach (var sharedMaterial in renderer.sharedMaterials)
                    {
                        materialAction.Invoke(sharedMaterial);
                    }
                }
            }
            
        }
        ForeachSharedMaterial((material => shaderNames.Add(material?.shader?.name)));
        Debug.Log($"All shader names in scene before: {String.Join(",",shaderNames)}");

        ForeachSharedMaterial((material =>
        {
            if(material != null)
                material.shader = simpleLit;
        }));
        
        shaderNames.Clear();
        ForeachSharedMaterial((material => shaderNames.Add(material?.shader?.name)));
        Debug.Log($"All shader names in scene after: {String.Join(",",shaderNames)}");


        //Debug.Log($"Has WandSparkleGreen: {HasGOWithName(allgos,"WandSparkleGreen")}");
        
    }
    static GameObject GOWithName(IEnumerable<GameObject> gos,string name)
    {
        foreach (var gameObject in gos)
        {
            if (gameObject == null) continue;
            if (gameObject.name == name) return gameObject;
        }

        return null;
    }
    static bool HasGOWithName(IEnumerable<GameObject> gos,string name)
    {
        return GOWithName(gos,name) != null;
    }
    static List<GameObject> GetAllObjectsOnlyInScene()
    {
        List<GameObject> objectsInScene = new List<GameObject>();

        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            if (!EditorUtility.IsPersistent(go.transform.root.gameObject) && !(go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave))
                objectsInScene.Add(go);
        }

        return objectsInScene;
    }
}
