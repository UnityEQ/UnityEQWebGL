using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Xml;
 
public class LightImport : EditorWindow
{
    [MenuItem("Window/EQ Object Importer")]
    static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(LightImport));
    }
     
    void OnGUI()
    {
        GUILayout.BeginHorizontal();
         
        if (GUILayout.Button("Load XML File"))
        {
            string path = EditorUtility.OpenFilePanel("Select an XML File", Application.dataPath, "xml");
            if (path.Length > 0)
            {
                using (XmlTextReader reader = new XmlTextReader(path))
                {
                    string objectName;
					float posx;
					float posy;
					float posz;
					float radius;
					byte colorR;
					byte colorG;
					byte colorB;
                     
                    GameObject container = new GameObject("Lights");
                 
                    while (reader.Read())
                    {
                        
                            objectName = reader.GetAttribute("name");
                            while (reader.Read())
                            {
                                if (reader.IsStartElement("object"))
                                {
									objectName = reader.GetAttribute("name");
									posx = float.Parse(reader.GetAttribute("positionx"));
									posy = float.Parse(reader.GetAttribute("positiony"));
									posz = float.Parse(reader.GetAttribute("positionz"));
									radius = float.Parse(reader.GetAttribute("radius"));
									colorR = byte.Parse(reader.GetAttribute("colorR"));
									colorG = byte.Parse(reader.GetAttribute("colorG"));
									colorB = byte.Parse(reader.GetAttribute("colorB"));
									
                                    CreateObject(container.transform, objectName, ref posx, ref posy, ref posz, ref radius, ref colorR, ref colorG, ref colorB);
                                    break;
                                }
                            }
                        
                    }
                }
            }
        }
 
        GUILayout.EndHorizontal();
    }
     
    void CreateObject(Transform parent, string name, ref float posx, ref float posy, ref float posz, ref float radius, ref byte colorR, ref byte colorG, ref byte colorB)
    {
        GameObject prefab = Resources.Load("Prefab/Pointlight") as GameObject;
         
        if (prefab != null)
        {
            GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            go.transform.position = new Vector3(-posx, posz, posy);
            go.GetComponent<Light>().color = new Color32(colorR, colorG, colorB, 200);
			go.GetComponent<Light>().range = radius;
            go.transform.parent = parent;
            go.name = name;
        }
    }
}