using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using System.IO;
 
public class UnpackAssets : MonoBehaviour
{
static void delRecursive(string path)
{
try
{
foreach (string d in Directory.GetFiles(path))
{
File.Delete(d);
}
foreach (string d in Directory.GetDirectories(path))
{
delRecursive(d);
}
Directory.Delete(path);
}
catch { }
}
 
[MenuItem("Assets/Unpack %u")]
static void Unpack()
{
string file = EditorUtility.OpenFilePanel("Select assets file to unpack", "", "assets");
if (file.Length == 0) return;
Object[] objs1 = AssetDatabase.LoadAllAssetsAtPath(file);
string ImportDir = @"/Import_" + Path.GetFileNameWithoutExtension(file) + "/";
delRecursive(Application.dataPath + ImportDir);
Directory.CreateDirectory(Application.dataPath + ImportDir);
foreach (Object obj in objs1)
{
if (obj == null) continue;
System.Type type = obj.GetType();
string ext = "asset";
 
if (type == typeof(Material) || (type.IsSubclassOf(typeof(Material))))
ext = "mat";
else if (type == typeof(AnimationClip) || (type.IsSubclassOf(typeof(AnimationClip))))
ext = "anim";
else if (type == typeof(GameObject) || (type.IsSubclassOf(typeof(GameObject))))
ext = "prefab";
 
if (type == typeof(Component) || (type.IsSubclassOf(typeof(Component))))
{
continue;
}
string ThisName = obj.name.Replace('/', '_');
if (string.IsNullOrEmpty(ThisName)) continue;
string f = @"Assets" + ImportDir + type.Name + @"/" + ThisName + "." + ext;
Object o = obj;
if (!Directory.Exists(Application.dataPath + ImportDir + type.Name))
Directory.CreateDirectory(Application.dataPath + ImportDir + type.Name);
 
if (type == typeof(GameObject) || (type.IsSubclassOf(typeof(GameObject))))
{
Object prefab = PrefabUtility.CreateEmptyPrefab(f);
if (obj == null)
{
Debug.LogError("NullException");
}
AssetDatabase.Refresh();
PrefabUtility.ReplacePrefab((GameObject)(obj), prefab);
continue;
}
else
{
try { o = Object.Instantiate(obj); } catch { o = obj; }
}
 
if (o == null) 
continue; 
 
AssetDatabase.CreateAsset(o, f);
}
AssetDatabase.Refresh();
File.Copy(file, Application.dataPath + ImportDir + Path.GetFileNameWithoutExtension(file) + ".asset");
AssetDatabase.Refresh();
Debug.Log("Done");
}
} 