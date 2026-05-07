using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

public class FBX_Tools : EditorWindow
{
    private int FirstNLetter;
    private int LastNLetter;

    private string SuffixString;
    private string PrefixString;

    private string MaterialFolderString = "Assets/Hivemind/MedievalFantasyVillage/Art";

    private string objectNameToRemove = "Enter Name Here";

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
        }
        GUILayout.EndHorizontal();


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

        if (GUILayout.Button("Rip Animation"))
        {
            Rip_Anim();
        }

        GUILayout.Label("Remove Object From Prefab", EditorStyles.boldLabel);

        objectNameToRemove = EditorGUILayout.TextField("Object Name to Remove", objectNameToRemove);

        GUILayout.Space(10);

        if (GUILayout.Button("Remove Object"))
        {
            RemoveObjectFromPrefab();
        }

        if (GUILayout.Button("Remove all Colliders"))
        {
            RemoveAllColliders();
        }

        if (GUILayout.Button("Add Mesh Collider"))
        {
            AddMeshColldier();
        }

        if (GUILayout.Button("Check UV out of bounds"))
        {
            CheckSelectedMeshUVsLightmap();
        }


        GUILayout.Label("UV Overlap Checker", EditorStyles.boldLabel);



        //refresh the ONGUI screen
        Repaint();
    }




    private void CheckSelectedMeshUVsLightmap()
    {
        Object selectedObj = Selection.activeObject;

        if (selectedObj == null)
        {
            Debug.LogWarning("No object selected. Please select an FBX file in the Project window.");
            return;
        }

        // Get the path of the selected asset
        string path = AssetDatabase.GetAssetPath(selectedObj);

        // Load all assets at the path to find the actual Mesh (FBXs often contain nested assets)
        Object[] assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);

        bool foundAnyErrors = false;

        foreach (Object asset in assets)
        {
            if (asset is Mesh mesh)
            {
                List<Vector2> uvs = new List<Vector2>();
                mesh.GetUVs(1, uvs); // Check primary UVs (UV0)

                if (uvs == null || uvs.Count == 0) continue;

                int outOfBoundsCount = 0;

                foreach (Vector2 uv in uvs)
                {
                    // Check if the UV coordinate is completely outside the [0, 1] range
                    if (uv.x < 0f || uv.x > 1f || uv.y < 0f || uv.y > 1f)
                    {
                        outOfBoundsCount++;
                    }
                }

                if (outOfBoundsCount > 0)
                {
                    Debug.LogError($"Mesh '{mesh.name}' has {outOfBoundsCount} out-of-bounds Light map coordinates out of {uvs.Count} total vertices.");
                    foundAnyErrors = true;
                }
                else
                {
                    Debug.Log($"Mesh '{mesh.name}' is fully within the [0,1] UV lightmap space.");
                }
            }
        }

        if (!foundAnyErrors)
        {
            Debug.Log("Check complete. No out-of-bounds lightmap UVs found in the selected FBX.");
        }

    }

    private void AddMeshColldier()
    {
        if (Selection.objects.Length == 0)
        {
            Debug.LogWarning("No objects selected. Please select at least one GameObject.");
            return;
        }

        int count = 0;

        foreach (Object obj in Selection.objects)
        {
            // Optional: Process children if you want to add colliders to the whole hierarchy 
            // of the prefab instead of just the root.
            MeshFilter[] meshFilters = obj.GetComponentsInChildren<MeshFilter>();

            foreach (MeshFilter mf in meshFilters)
            {
                // Avoid adding a collider if one already exists
                if (mf.GetComponent<MeshCollider>() == null)
                {
                    MeshCollider meshCollider = mf.gameObject.AddComponent<MeshCollider>();

                    // The sharedMesh must be assigned for it to bake accurately
                    meshCollider.sharedMesh = mf.sharedMesh;

                    // Mark prefab stage as dirty so Unity saves the change
                    EditorUtility.SetDirty(mf.gameObject);
                    count++;
                }
            }
        }

        Debug.Log($"Successfully added Mesh Colliders to {count} objects.");
    }

    private void RemoveAllColliders()
    {
        if (Selection.objects.Length == 0)
        {
            Debug.LogWarning("No prefabs selected. Please select at least one prefab in the Project view.");
            return;
        }

        int colliderCount = 0;
        int prefabCount = 0;

        foreach (Object obj in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(obj);

            // Ensure it's actually a valid prefab asset
            if (PrefabUtility.GetPrefabAssetType(obj) == PrefabAssetType.NotAPrefab) continue;

            // Load the prefab as a GameObject
            GameObject prefabRoot = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefabRoot == null) continue;

            // Find all colliders, including children
            Collider[] colliders = prefabRoot.GetComponentsInChildren<Collider>(true);
            Collider2D[] colliders2D = prefabRoot.GetComponentsInChildren<Collider2D>(true);

            if (colliders.Length > 0 || colliders2D.Length > 0)
            {
                // Delete 3D Colliders
                foreach (Collider col in colliders)
                {
                    Undo.DestroyObjectImmediate(col);
                    colliderCount++;
                }

                // Delete 2D Colliders
                foreach (Collider2D col2d in colliders2D)
                {
                    Undo.DestroyObjectImmediate(col2d);
                    colliderCount++;
                }

                prefabCount++;
            }
        }

        Debug.Log($"Successfully removed {colliderCount} colliders across {prefabCount} prefab(s).");
    }

    private void RemoveObjectFromPrefab()
    {
        if (Selection.objects.Length == 0)
        {
            Debug.LogWarning("No objects selected.");
            return;
        }

        foreach (Object GOobj in Selection.objects)
        {
            // 1. Open the prefab contents in memory
            string prefabPath = AssetDatabase.GetAssetPath(GOobj);
            GameObject rootInstance = PrefabUtility.LoadPrefabContents(prefabPath);

            // 2. Find and destroy objects containing the string
            int removedCount = 0;
            Transform[] allChildren = rootInstance.GetComponentsInChildren<Transform>(true);

            foreach (Transform child in allChildren)
            {
                if (child.gameObject.name.Contains(objectNameToRemove))
                {
                    // Destroy child immediately in editor mode
                    DestroyImmediate(child.gameObject);
                    removedCount++;
                }
            }

            if (removedCount > 0)
            {
                // 3. Save changes back to the original Prefab Asset
                PrefabUtility.SaveAsPrefabAsset(rootInstance, prefabPath);
                Debug.Log($"Successfully removed {removedCount} objects containing '{objectNameToRemove}' from {GOobj.name}.");
            }
            else
            {
                Debug.Log($"No objects found containing '{objectNameToRemove}'.");
            }

            // 4. Unload the prefab contents to prevent memory leaks
            PrefabUtility.UnloadPrefabContents(rootInstance);
        }
    }

    private void Rip_Anim()
    {
        foreach (Object obj in Selection.objects)
        {
            // Get the selected object
            Object selectedObject = obj;
            if (selectedObject == null) return;

            string path = AssetDatabase.GetAssetPath(selectedObject);
            string folder = Path.GetDirectoryName(path);
            string fbxName = Path.GetFileNameWithoutExtension(path);

            // Load all assets, specifically looking for AnimationClips (Type: AnimationClip)
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
            var clips = assets.OfType<AnimationClip>().ToArray();

            if (clips.Length == 0)
            {
                Debug.LogWarning("No animation clips found in " + fbxName);
                return;
            }

            foreach (var clip in clips)
            {
                // Skip humanoid avatar animations or default T-Poses if necessary
                if (clip.name.StartsWith("__")) continue;

                // Create a duplicate of the animation
                AnimationClip newClip = new AnimationClip();
                EditorUtility.CopySerialized(clip, newClip);

                // Create new asset name based on FBX name
                string newPath = $"{folder}/{fbxName}.anim";

                // Save the asset
                AssetDatabase.CreateAsset(newClip, AssetDatabase.GenerateUniqueAssetPath(newPath));
                Debug.Log($"Exported: {newPath}");
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    private void AssignConvexHull()
    {
        if (Selection.objects.Length == 0)
        {
            Debug.LogWarning("No objects selected.");
            return;
        }

        int count = 0;
        foreach (GameObject obj in Selection.gameObjects)
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
        if (Selection.objects.Length == 0)
        {
            Debug.LogWarning("No objects selected.");
            return;
        }

        foreach (GameObject obj in Selection.gameObjects)
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
        if (Selection.objects.Length == 0)
        {
            Debug.LogWarning("No objects selected.");
            return;
        }

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
        if (Selection.objects.Length == 0)
        {
            Debug.LogWarning("No objects selected.");
            return;
        }

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
        if (Selection.objects.Length == 0)
        {
            Debug.LogWarning("No objects selected.");
            return;
        }

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
                {
                    Debug.Log("Renamed to: " + newName);
                }
                else
                {
                    Debug.LogError("Rename failed: " + result);
                }
            }
        }
    }

    private void AddPrefix()
    {
        if (Selection.gameObjects.Length == 0)
        {
            Debug.LogWarning("No objects selected.");
            return;
        }

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
        if (Selection.gameObjects.Length == 0)
        {
            Debug.LogWarning("No objects selected.");
            return;
        }

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
        if (Selection.gameObjects.Length == 0)
        {
            Debug.LogWarning("No objects selected.");
            return;
        }

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
        if (Selection.gameObjects.Length == 0)
        {
            Debug.LogWarning("No objects selected.");
            return;
        }

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
