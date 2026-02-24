using ntw.CurvedTextMeshPro;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class render_Image : EditorWindow
{
    //Text objects in the scene
    public string top_Text_Name = "Top_Text";
    public string bottom_Text_Name = "Bottom_Text";

    private GameObject top_Text_Object;
    private GameObject bottom_Text_Object;

    //Gets the Camera
    private Camera mainCamera;

    //Gets the render scene
    private string fullPathOfScene = null;

    //gets file paths
    private string fileNameWithExtension = null;
    private string fileNameWithoutExtension = null;
    private string pathOnly = null;

    //Grabs the Render Texture near the scene file
    private readonly string nameOfRenderTexture = "renderTextureAsset.renderTexture";
    private RenderTexture renderTexture;

    //top text attributes
    private string top_Text_String = "Top Text";
    private float top_m_radius = 360.0f;
    private float top_m_arcDegrees = 90.0f;
    private float top_m_angularOffset = -90.0f;
    private float top_Font = 60.0f;

    //top text attributes
    private string bottom_Text_String = "Bottom Text";
    private float bottom_m_radius = -360.0f;
    private float bottom_m_arcDegrees = -90.0f;
    private float bottom_m_angularOffset = -90.0f;
    private float bottom_Font = 60.0f;

    [MenuItem("Tools/Render_ImageUI")]
    public static void ShowWindow()
    {
        Rect windowRect = new Rect(100, 100, 400, 1000);
        render_Image window = GetWindowWithRect<render_Image>(windowRect, false, "Render Image UI");

        window.CenterOnMainWin();

        window.FindProperties();
    }

    public void CenterOnMainWin()
    {
        Rect main = EditorGUIUtility.GetMainWindowPosition();
        Rect pos = position;

        float centerWidth = (main.width - pos.width) * 0.5f;
        float centerHeight = (main.height - pos.height) * 0.5f;

        pos.x = main.x + centerWidth;
        pos.y = main.y + centerHeight;
        position = pos;
    }
    public void FindProperties()
    {
        top_Text_Object = GameObject.Find(top_Text_Name);
        bottom_Text_Object = GameObject.Find(bottom_Text_Name);

        mainCamera = Camera.main;

        //Gets the render scene
        fullPathOfScene = SceneManager.GetSceneByName("TextToImage").path;

        //gets file paths
        fileNameWithExtension = Path.GetFileName(fullPathOfScene);
        fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fullPathOfScene);
        pathOnly = Path.GetDirectoryName(fullPathOfScene);

        //Assigns the RenderTexture.
        renderTexture = AssetDatabase.LoadAssetAtPath<RenderTexture>(pathOnly + "/" + nameOfRenderTexture);
    }
    
    public void OnGUI()
    {
        //Draw the texture, stretching to fill the window
        Rect rect = GUILayoutUtility.GetAspectRect((float)renderTexture.width / renderTexture.height);
        EditorGUI.DrawPreviewTexture(rect, renderTexture);

        GUILayout.Space(20);

        //---------------------------------------------------------------------------------------------

        //Top Text attributes on GUI
        top_Text_String = EditorGUILayout.TextField("Top Text", top_Text_String);
        top_Font = EditorGUILayout.FloatField("Top Font", top_Font);

        top_m_radius = EditorGUILayout.FloatField("Top Radius", top_m_radius);
        top_m_arcDegrees = EditorGUILayout.FloatField("Top Arc Degrees", top_m_arcDegrees);
        top_m_angularOffset = EditorGUILayout.FloatField("Top Angular Offset", top_m_angularOffset);

        //---------------------------------------------------------------------------------------------
        GUILayout.Space(20);
        //---------------------------------------------------------------------------------------------

        //Bottom Text attributes on GUI
        bottom_Text_String = EditorGUILayout.TextField("Bottom Text", bottom_Text_String);
        bottom_Font = EditorGUILayout.FloatField("Bottom Font", bottom_Font);

        bottom_m_radius = EditorGUILayout.FloatField("Bottom Radius", bottom_m_radius);
        bottom_m_arcDegrees = EditorGUILayout.FloatField("Bottom Arc Degrees", bottom_m_arcDegrees);
        bottom_m_angularOffset = EditorGUILayout.FloatField("Bottom Angular Offset", bottom_m_angularOffset);

        //---------------------------------------------------------------------------------------------
        GUILayout.Space(20);
        //---------------------------------------------------------------------------------------------

        //Draws the button on the Window
        GUILayout.Label("Click the button to refresh the Image", EditorStyles.boldLabel);

        // Create a button
        if (GUILayout.Button("Update Image"))
        {
            //starts UpdateText
            UpdateText();

            //Waits till we start the image Render to give time to update the scene
            DelayUseAsync();
        }
    }

    async void DelayUseAsync()
    {
        //Refreshes all Scenes to be used
        SceneView.RepaintAll();

        //waits for one second
        await Task.Delay(1000);

        //Starts the Render
        Render_Image();
    }

    public void UpdateText()
    {
        //Gets the TMPro asset
        TMP_Text topTextInput = top_Text_Object.GetComponent<TextMeshProUGUI>();

        //Updates the Top Text
        topTextInput.text = top_Text_String;

        //Updates the Top Font
        topTextInput.fontSize = top_Font;

        //Gets the textMeshpro on a curve script
        TextProOnACircle top_textProOnACurve = top_Text_Object.GetComponent<TextProOnACircle>();

        //Updates the Top Radius
        top_textProOnACurve.m_radius = top_m_radius;

        //Updates the Arc Degrees
        top_textProOnACurve.m_arcDegrees = top_m_arcDegrees;

        //Updates the Top Angular Offset
        top_textProOnACurve.m_angularOffset = top_m_angularOffset;

        //Updates the Bottom Text;
        TMP_Text bottomTextInput = bottom_Text_Object.GetComponent<TextMeshProUGUI>();
        bottomTextInput.text = bottom_Text_String;

        //Updates the Bottom Font
        bottomTextInput.fontSize = bottom_Font;

        //Gets the textMeshpro on a curve script
        TextProOnACircle bottom_textProOnACurve = bottom_Text_Object.GetComponent<TextProOnACircle>();

        //Updates the Top Radius
        bottom_textProOnACurve.m_radius = bottom_m_radius;

        //Updates the Arc Degrees
        bottom_textProOnACurve.m_arcDegrees = bottom_m_arcDegrees;

        //Updates the Top Angular Offset
        bottom_textProOnACurve.m_angularOffset = bottom_m_angularOffset;
    }

    public void Render_Image()
    {
        //Renders Camera View
        mainCamera.Render();

        //Make sure there is a renderTexture on the camera
        RenderTexture rt = mainCamera.targetTexture;

        //Sets teh renterTexture to active from the main Camera renderTexure
        RenderTexture.active = rt;

        //Renders Camera View
        mainCamera.Render();

        //Read the pixels into a Texture2D
        Texture2D image = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        image.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        image.Apply();

        //Encode the TExture2d to PNG bytes
        byte[] bytes = image.EncodeToPNG();

        //Saves the file path out
        string filePath = Path.Combine(pathOnly + "/" + top_Text_String + "_" + bottom_Text_String + ".png");

        //Writes the Bytes to the file
        File.WriteAllBytes(filePath, bytes);

        //Refreshes the database
        AssetDatabase.Refresh();
    }
}
