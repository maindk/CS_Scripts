using System.IO;
using System.Linq;
using Unity.APIComparison.Framework.Descriptors;
using UnityEditor;
using UnityEngine;

public class BatchMaterialImporter : EditorWindow
{
    private Material targetMaterial;

    private string folderPath = "Assets/Hivemind/Art/Meshes/Props/"; // Change to your specific folder

    [MenuItem("Tools/FBX Material Importer")]
    public static void ShowWindow() => GetWindow<BatchMaterialImporter>("FBX Importer");

    void OnGUI()
    {
        if (GUILayout.Button("Set Selected FBX to External Materials"))
        {
            // Iterate through selected assets in the Project window
            foreach (Object obj in Selection.objects)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;

                if (importer != null)
                {
                    importer.materialImportMode = ModelImporterMaterialImportMode.ImportViaMaterialDescription;
                    importer.materialLocation = ModelImporterMaterialLocation.InPrefab;
                    importer.materialName = ModelImporterMaterialName.BasedOnMaterialName;

                    // Save and trigger reimport
                    AssetDatabase.WriteImportSettingsIfDirty(path);
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

                    
                    Debug.Log($"Updated: {path}");
                }
            }
        }

        GUILayout.Space(20);

        targetMaterial = (Material)EditorGUILayout.ObjectField("Target Material", targetMaterial, typeof(Material), false);


        if (GUILayout.Button("Assign to Selected FBXs") && targetMaterial != null)
        {
            foreach (Object obj in Selection.objects)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;

                if (importer != null)
                {
                    // Remap all materials in the FBX to the target material
                    // You may need to specify the exact name of the internal material
                    // Replace "InternalMaterialName" with your FBX's material name
                    var identifier = new AssetImporter.SourceAssetIdentifier(typeof(Material), targetMaterial.name);
                    importer.AddRemap(identifier, targetMaterial);

                    AssetDatabase.WriteImportSettingsIfDirty(path);
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                }
            }
        }

        GUILayout.Space(20);

        if (GUILayout.Button("Remove First"))
        {
            foreach (Object obj in Selection.objects)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
                string directory = System.IO.Path.GetDirectoryName(path);

                // Check if file starts with a character, e.g., 'A'fbxName.fbx
                if (fileName.Length > 1)
                {
                    string newName = fileName.Substring(1); // Removes first letter

                    // Rename only if the name has changed to prevent infinite import loops
                    if (fileName != newName)
                    {
                        AssetDatabase.RenameAsset(path, newName);
                        Debug.Log($"Renamed {fileName} to {newName}");
                    }
                }
            }
        }

        GUILayout.Space(20);

        string prefix = "MI";

        if (GUILayout.Button("Add MI prefix"))
        {
            // Get all selected assets that are Materials
            Object[] selectedMaterials = Selection.GetFiltered(typeof(Material), SelectionMode.Assets);

            foreach (Object obj in selectedMaterials)
            {
                string oldPath = AssetDatabase.GetAssetPath(obj);
                string newName = prefix + obj.name;

                // Use AssetDatabase to rename the file safely
                AssetDatabase.RenameAsset(oldPath, newName);
            }
            AssetDatabase.SaveAssets();
        }


        if (GUILayout.Button("AssignMaterialsToSelectedFBX"))
        {
            // Find all GUIDs of Materials within the specified folder
            // 't:Material' filters by type
            //string[] materialGUIDs = AssetDatabase.FindAssets("t:Material", new[] { folderPath });
            string[] materialGUIDs = Directory.GetFiles(folderPath, "*.mat", SearchOption.TopDirectoryOnly);

            foreach (string guid in materialGUIDs)
            {
                Debug.Log(guid);
                Material newMaterial = AssetDatabase.LoadAssetAtPath<Material>(guid);

                foreach (Object obj in Selection.objects)
                {
                    string path = AssetDatabase.GetAssetPath(obj);
                    ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;

                    if (importer != null)
                    {
                        // Remap all materials in the FBX to the target material
                        // You may need to specify the exact name of the internal material
                        // Replace "InternalMaterialName" with your FBX's material name
                        var identifier = new AssetImporter.SourceAssetIdentifier(typeof(Material), newMaterial.name);
                        importer.AddRemap(identifier, newMaterial);

                        AssetDatabase.WriteImportSettingsIfDirty(path);
                        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                    }
                }

            }
            Debug.Log($"Found {materialGUIDs.Length} materials in {folderPath}");
        }
    }
}
