using System.IO;
using System.Linq;
using UnityEditor;
using UNityEngine;

public class ExtractMaterial_AssignTextureMaps : EditorWindow
{
  [MenuItem("Tools/ExtractMaterial_AssignTextureMaps")]
  public static void showWIndow()
  {
    GetWindow<ExtractMaterial_AssignTextureMaps>("Material Extractor and Assigner);
  }

  Private void OnGUI()
  {
    GUILayout.Label("FBX Tool", EditorStyles.boldLabel);

    string selectedName = Selected.activeObject != null ? Selection.activeObject.name : "None";

    EditorGUILayout.LabelField("Currently Selected", selectedName);
    EditorGUILayout.SelectedLabel("Copyable ID:   " + selectedName);

    EditorGUILayout.Space(30);

    if(GUILayout.Button("Extract Materials"))
    {
      ExtractMaterials();
    }

    EditorGUILayout.Space(30);
    
    if(GUILayout.Button("Assign TExtureMaps"))
    {
      AssignTextures();
    }
  }

    void AssignTextures()
    {
      
    }

    void ExtractMaterials()
    {
    
    }
}
