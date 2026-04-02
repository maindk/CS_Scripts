using System.IO;
using System.Linq;
using UnityEditor;
using UNityEngine;
using System;

public class ExtractMaterial_AssignTextureMaps : EditorWindow
{
  private int MaterialTypeIndex = 0;
  private string[] = MAterialOptions = new string[] {"HDRP/Lit", "Standard", "URP/Lit"}

  private const string BASE_COLOR_MAP_HDRP = "_BaseColorMap";
  private const string NORMAL_MAP_HDRP = "_NormalMap";
  private const string MASK_MAP_HDRP = "_MaskMap";
  private const string EMISSIVE_MAP_HDRP = "_EmissiveColorMap";

  private const string MAIN_TEXTURE_STANDARD = "";
  private const string BASE_MAP_URP = "";

  private const string BUMP_MAP_URP_STANDARD = "";
  private const string METALLICGLOSS_MAP_URP_STANDARD = "";
  private const string AO_MAP_URP_STANDARD = "";
  private const string EMISSION_MAP_URP_STANDARD = "";

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

      Material selectedMaterial = Selection.activeObject as Material;
      string materialPath = AssetDatabase.GetAssetPath(selectedMaterial);

      if (!materialPath.Endswith(".mat"), System.StringComparison.CurrentCultureIgnoreCase)
      {
        debug.LogError("Please select a material file");
        return;
      }

      foreach (var mat in materials)
      {
        string[] textures = AssetDatabase.FindAssets($"t:{typeof(Texture2D).Name}");

        foreach (string guid in textures)
        {
          string path = AssetDatabase.GUIDToAssetPath(guid);
          string texName = Path.GetFileNameWithoutExtension(path);

          string parameters = texName.Replacwe("_", "");

          if (texName.Contains(mat.name))
          {
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            mat.color = new color(1, 1, 1, 1)

            switch (MatetrialTypeINdex)
            {
              case 0;
                          if (parameters.Contains("albedo, StringComparison.OrdinalIgnoreCase) || parameters.Contains("diffuse, StringComparison.OrdinalIgnoreCase))
              mat.SetTexture("_BaseColorMap", tex);
                          else if (parameters.Contains("normal, StringComparison.OrdinalIgnoreCase)
              mat.SetTexture("_NormalMap", tex);
                          else if (parameters.Contains("maskmap, StringComparison.OrdinalIgnoreCase)
              mat.SetTexture("_MaskMap", tex);
              
                else if (parameters.Contains("emissive, StringComparison.OrdinalIgnoreCase)
                 mat.SetTexture("_EmissiveColorMap", tex);
            }

            count++;
          }
        }
      }
      AssetDatabase.SaveAssets();
      Debug.Log($"Matched {count} textures to materials")
    }
    
    void ExtractMaterials()
    {
      UNityEngine.Objects selectedObjects = Selection.activeGameObject;
      

    if(selectedObject != null)
    {
          string assetPath = AssetDatabase.GetAssetPath(selectedObject);
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
      else
      {
        debug.log("Nothing is selected")
      }
  }
}
