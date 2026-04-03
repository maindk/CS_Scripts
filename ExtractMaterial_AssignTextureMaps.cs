using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

    public class ExtractMaterial_AssignTextureMaps : EditorWindow
    {
        private int MaterialTypeIndex = 0;
        private string[] MaterialOptions = new string[] { "HDRP/Lit", "Standard", "URP/Lit" };

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
        GetWindow<ExtractMaterial_AssignTextureMaps>("Material Extractor and Assigner");
    }

    private void OnGUI()
    {
        GUILayout.Label("FBX Tool", EditorStyles.boldLabel);

        string selectedName = Selection.activeObject != null ? Selection.activeObject.name : "None";

        EditorGUILayout.LabelField("Currently Selected", selectedName);
        EditorGUILayout.SelectableLabel("Copyable ID:   " + selectedName);

        EditorGUILayout.Space(30);

        if (GUILayout.Button("Extract Materials"))
        {
            ExtractMaterials();
        }

        EditorGUILayout.Space(30);

        if (GUILayout.Button("Assign TExtureMaps"))
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

        if (!materialPath.EndsWith(".mat", StringComparison.OrdinalIgnoreCase))
        {
            Debug.LogError("Please select a material file");
            return;
        }

        foreach (var mat in materials)
        {
            string[] textures = AssetDatabase.FindAssets($"t:{typeof(Texture2D).Name}");

              foreach (string guid in textures)
              {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    string texName = Path.GetFileNameWithoutExtension(path);

                    string parameters = texName.Replace("_", "");

                    if (texName.Contains(mat.name))
                        {
                            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                            mat.color = new Color(1, 1, 1, 1);
  
                            switch (MaterialTypeIndex)
                            {
                                case 0:
                                    if (parameters.Contains("albedo", StringComparison.OrdinalIgnoreCase) || parameters.Contains("diffuse", StringComparison.OrdinalIgnoreCase))
                                        mat.SetTexture(BASE_COLOR_MAP_HDRP, tex);
                            
                                    else if (parameters.Contains("normal", StringComparison.OrdinalIgnoreCase))
                                        mat.SetTexture(NORMAL_MAP_HDRP, tex);
                                
                                    else if (parameters.Contains("maskmap", StringComparison.OrdinalIgnoreCase))
                                        mat.SetTexture(MASK_MAP_HDRP, tex);

                                    else if (parameters.Contains("emissive", StringComparison.OrdinalIgnoreCase))
                                        mat.SetTexture(EMISSIVE_MAP_HDRP, tex);
                                break;
                            }

                        count++;
                        }
              }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"Matched {count} textures to materials");
    }

    void ExtractMaterials()
    {
        UnityEngine.Object selectedObject = Selection.activeGameObject;


        if (selectedObject != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(selectedObject);
            string destinationPath = Path.GetDirectoryName(assetPath);

            var subAssets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            var materials = subAssets.Where(x => x is Material);
            foreach (var material in materials)
            {
                string newPath = Path.Combine(destinationPath, material.name + ".mat");
                string error = AssetDatabase.ExtractAsset(material, newPath);

                if (string.IsNullOrEmpty(error))
                {
                    AssetDatabase.WriteImportSettingsIfDirty(assetPath);
                    AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
                }
                else
                {
                    Debug.LogWarning($"Failed to extarct {material.name}: {error}");
                }
            }
        }
        else
        {
            Debug.Log("Nothing is selected");
        }
    }
}
