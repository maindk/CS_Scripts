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
      var materials = Selection.GetFiltered<Material>(SelectionMode.Assets);
      int count = 0;

      Material selectedMAterial = Selection.activeObject as Material;
      string materialPath = AssetDatabase.GetAssetPath(selectedMaterial);

      if (string.IsNullOrEmpty(materialPath) || !materialPath.ToLower().Endswith(".maty"))
      {
        debug.LogError("Please select a material file");
        return;
      }

      foreach (var mat in materials)
      {
        string[] textures = AssetDatabase.FindAssets("t:Textures2D", null);

        foreach (string guid in textures)
        {
          string path = AssetDatabase.GUIDToAssetPath(guid);
          string texName = Path.GetFileNameWithoutExtension(path);

          if (texName.Contains(mat.name))
          {
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (texName.Contains("_Albedo") || texName.Contains("BaseColor))
              mat.SetTexture("_BaseColorMap", tex);
            else if ( (texName.Contains("Normal") || texName.Contains("normal))
              mat.SetTexture("_NormalMap", tex);
            count++;
          }
        }
      }
      AssetDatabase.SaveAssets();
      Debug.Log($"Matched {count} textures to materials")
    }
    
    void ExtractMaterials()
    {
      Objects selectedObjects = Selection.activeGameObject;
      string assetPath = AssetDatabase.GetAssetPath(selectedObject);

      if(string.IsNullOrEmpty(assetPath) || !assetPath.ToLower().EndsWith(".fbx"))
      {
        Debug.LogError("PLease select a FBX File")
        return;
      }

      string destinationPath = Path.GetDirectoryName(assetPath);

      var subAssets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
      var materials - subAssets.Where(x => x is Material);

      foreach (var material in materials)
      {
        string newPath = Path.Combine(destinationPath, material.name + ".mat")
        newPath = AssetDatabase.ExtractAsset(material, newPath);

        if (string.IsNullOrEmpty(error))
        {
          AssetDatabase.WriteImportSettingsIfDirty(assetPath);
          AsserDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        }
        else
        {
          Debug.LogWarning($"Failed to extarct {material.name}: {error}")
        }
      }
      
    }
}
