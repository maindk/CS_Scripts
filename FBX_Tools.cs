using System.IO;
using UnityEditor;
using UnityEngine;

public class FBX_Tools : EditorWindow
{
    private int FirstNLetter;
    private int LastNLetter;

    private string SuffixString;
    private string PrefixString;

    private string MaterialFolderString = "Assets/Hivemind/MedievalFantasyVillage/Art";

    [MenuItem("Tools/FBX_Tools")]
    public static void ShowWindow()
    {
        GetWindow<FBX_Tools>("FBX Tools");
    }

    private void OnGUI()
    {
        //Sets the amterial import settings for the FBX assets.
        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Set Materials Creation Mode to Material Description"))
            {
                MaterialCreationModeMaterialDescription();
            }

            if (GUILayout.Button("Set Materials Creation Mode to None"))
            {
                MaterialCreationModeNone();
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.Space(15);

        //Material assignment and location button
        {
            //Textfield for setting the fold location for the materials
            MaterialFolderString = EditorGUILayout.TextField("Location of Materials", MaterialFolderString);


            if (GUILayout.Button("Assigns materials to selected fbx"))
            {
                AssignMaterial();
            }

        }

        GUILayout.Space(15);

        //Remove first and last letter buttons.
        GUILayout.BeginHorizontal();
        {
            FirstNLetter = EditorGUILayout.IntField("", FirstNLetter);

            if (GUILayout.Button("Remove First N Letter"))
            {
                RemoveFirstNLetter();
            }

            LastNLetter = EditorGUILayout.IntField("", LastNLetter);

            if (GUILayout.Button("Remove Last N Letter"))
            {
                RemoveLastNLetter();
            }
        }
        GUILayout.EndHorizontal();

        //Prefix and suffix buttons
        GUILayout.BeginHorizontal();
        {
            PrefixString = EditorGUILayout.TextField("", PrefixString);

            if (GUILayout.Button("Add Prefix"))
            {
                AddPrefix();
            }

            SuffixString = EditorGUILayout.TextField("", SuffixString);

            if (GUILayout.Button("Add Suffix"))
            {
                AddSuffix();
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(15);
        
        //Creats and assigns a convex hull to selected objects.
        if (GUILayout.Button("Assign Convex Hull"))
        {
            AssignConvexHull();
        }

        GUILayout.Space(15);

        //assigns items to static settings.
        if (GUILayout.Button("Set As Static"))
        {
            SetAsStatic();
        }

        //refresh the ONGUI screen
        Repaint();
    }

    private void AssignConvexHull()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("No objects selected.");
            return;
        }

        int count = 0;
        foreach (GameObject obj in selectedObjects)
        {
            // Check if it's a prefab asset or an instance in the scene
            if (PrefabUtility.GetPrefabAssetType(obj) != PrefabAssetType.NotAPrefab || PrefabUtility.GetCorrespondingObjectFromSource(obj) != null)
            {
                // Add or get existing MeshCollider
                MeshCollider collider = obj.GetComponent<MeshCollider>();
                if (collider == null)
                {
                    collider = obj.AddComponent<MeshCollider>();
                }

                // Set Convex
                collider.convex = true;

                // If it's a prefab asset, we need to save changes
                if (PrefabUtility.IsPartOfPrefabAsset(obj))
                {
                    EditorUtility.SetDirty(obj);
                    // PrefabUtility.SavePrefabAsset(obj); // Optional: Save immediately
                }

                count++;
            }
        }
        Debug.Log($"Assigned convex colliders to {count} prefabs.");
    }

    private void SetAsStatic()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        foreach (GameObject obj in selectedObjects)
        {
            // Define the flags (e.g., Everything, or custom bits)
            StaticEditorFlags flags = StaticEditorFlags.OccluderStatic | StaticEditorFlags.OccludeeStatic | StaticEditorFlags.BatchingStatic;

            // Apply the flags to the GameObject
            GameObjectUtility.SetStaticEditorFlags(obj, flags);

            // Optional: Mark prefab as dirty to save changes
            EditorUtility.SetDirty(obj);
            Debug.Log(obj.name + " set to static.", obj);
        }
    }

    private void AssignMaterial()
    {
        // Find all GUIDs of Materials within the specified folder
        string[] materialGUIDs = Directory.GetFiles(MaterialFolderString, "*.mat", SearchOption.TopDirectoryOnly);

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
        Debug.Log($"Found {materialGUIDs.Length} materials in {MaterialFolderString}");
    }

    private void RemoveFirstNLetter()
    {
        // Iterate through all selected assets
        foreach (Object obj in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(obj);

                string fileName = Path.GetFileNameWithoutExtension(path);

                // Ensure name is long enough to have a first letter
                if (fileName.Length > 1)
                {
                    string newName = fileName.Substring(FirstNLetter);
                    string result = AssetDatabase.RenameAsset(path, newName);

                    if (string.IsNullOrEmpty(result))
                    {
                        Debug.Log($"Renamed {fileName} to {newName}");
                    }
                    else
                    {
                        Debug.LogError($"Error renaming {fileName}: {result}");
                    }
                }
        }
    }

    private void RemoveLastNLetter()
    {
        foreach (Object obj in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(obj);


                string fileName = Path.GetFileNameWithoutExtension(path);

                // Ensure name is long enough to have a first letter
                if (fileName.Length > 1)
                {
                    // REMOVE LAST LETTER HERE
                    string newName = fileName.Substring(0, fileName.Length - LastNLetter);

                    // Rename the asset
                    string result = AssetDatabase.RenameAsset(path, newName);

                    if (string.IsNullOrEmpty(result))
                        Debug.Log("Renamed to: " + newName);
                    else
                        Debug.LogError("Rename failed: " + result);
                }
        }
    }

    private void AddPrefix()
    {
        // Iterate through selected objects in the Project window
        foreach (Object obj in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            string directory = Path.GetDirectoryName(path);
            string fileName = Path.GetFileNameWithoutExtension(path);
            string newName = PrefixString + fileName;

            // Rename the asset
            string result = AssetDatabase.RenameAsset(path, newName);

            if (!string.IsNullOrEmpty(result))
            {
                Debug.LogError("Error renaming: " + result);
            }
        }
        AssetDatabase.SaveAssets();
        Debug.Log("Suffix addition complete.");
    }

    private void AddSuffix()
    {
        // Iterate through selected objects in the Project window
        foreach (Object obj in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            string directory = Path.GetDirectoryName(path);
            string fileName = Path.GetFileNameWithoutExtension(path);
            string newName = fileName + SuffixString;

            // Rename the asset
            string result = AssetDatabase.RenameAsset(path, newName);

            if (!string.IsNullOrEmpty(result))
            {
                Debug.LogError("Error renaming: " + result);
            }
            
        }
        AssetDatabase.SaveAssets();
        Debug.Log("Suffix addition complete.");
    }

    private void MaterialCreationModeNone()
    {
        // Iterate through selected assets in the Project window
        foreach (Object obj in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;

            if (importer != null)
            {
                importer.materialImportMode = ModelImporterMaterialImportMode.None;
                //importer.materialLocation = ModelImporterMaterialLocation.InPrefab;
                //importer.materialName = ModelImporterMaterialName.BasedOnMaterialName;

                // Save and trigger reimport
                AssetDatabase.WriteImportSettingsIfDirty(path);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);


                Debug.Log($"Updated: " + obj.name + " to Material Creation Mode: None");
            }
        }
    }

    private void MaterialCreationModeMaterialDescription()
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


                Debug.Log($"Updated: " + obj.name + " to Material Creation Mode: Import via Material Description");
            }
        }
    }

}
