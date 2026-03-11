using ntw.CurvedTextMeshPro;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
//using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;

public class Text_To_RenderImage_Script : EditorWindow
{
    private Camera PreviewCamera;
    private GameObject CameraGameObject;
    private string PreviewCameraName = "Preview_Camera";
    private int PreviewCameraDepth = -1;
    private Color BackGroundColor = Color.black;

    private string PreviousScenePath;
    private Scene PreviousScene;
    private Scene TextScene;
    private string TextSceneName = "Text_Scene";

    private RenderTexture PreviewRenderTexture;
    private string RenderTextureName = "Render_Texture_Asset";
    private int RenderTexturetWidth = 1024;
    private int RenderTextureHeight = 1024;
    private int RenderTextureDepth = 24;

    private string ImageName = "Filename";

    private GameObject Straight_RenderCanvasGameObject;
    private Canvas Straight_RenderCanvas;
    private string Straight_RenderCanvasName = "Straight_Canvas";

    private TextMeshProUGUI Straight_RenderText;
    private Color Straight_RenderTextColor = Color.white;
    private float Straight_RenderTextFontSize = 200;
    private string Straight_RenderTextInput = "Hello World";

    private TextAlignmentOptions Straight_RenderTextAlignment;
    private int SelectedAlignemntIndex = 1;
    private string[] AlignmentOptions = new string[] {"Left", "Center", "Right"};


    private bool CirculizeText = true;


    private TextProOnACircle Top_CirculizeTextScript;

    private GameObject Top_RenderCanvasGameObject;
    private Canvas Top_RenderCanvas;
    private string Top_RenderCanvasName = "Top_Circle_Canvas";

    private TextMeshProUGUI Top_RenderText;
    private Color Top_RenderTextColor = Color.white;
    private float Top_RenderTextFontSize = 200;
    private string Top_RenderTextInput = "Top Text";

    private float Top_tmpC_Radius = 270;
    private float Top_tmpC_Arc_Degrees = 100;
    private float Top_tmpC_Angular_Offset = -90;


    private TextProOnACircle Bottom_CirculizeTextScript;

    private GameObject Bottom_RenderCanvasGameObject;
    private Canvas Bottom_RenderCanvas;
    private string Bottom_RenderCanvasName = "Bottom_Circle_Canvas";

    private TextMeshProUGUI Bottom_RenderText;
    private Color Bottom_RenderTextColor = Color.white;
    private float Bottom_RenderTextFontSize = 200;
    private string Bottom_RenderTextInput = "Bottom Text";

    private float Bottom_tmpC_Radius = -270;
    private float Bottom_tmpC_Arc_Degrees = -100;
    private float Bottom_tmpC_Angular_Offset = -90;


    private Texture2D RenderPNGImage;
    private byte[] Bytes;
    private string ProjectPath;

    [MenuItem("Tools/Text_To_Image")]
    public static void ShowWindow()
    {
        GetWindow<Text_To_RenderImage_Script>("Text To Image");
    }

    void creatScene()
    {
        PreviousScene = EditorSceneManager.GetActiveScene();
        PreviousScenePath = PreviousScene.path;

        TextScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        TextScene.name = TextSceneName;
    }

    void createRT()
    {
        PreviewRenderTexture = new RenderTexture(RenderTexturetWidth, RenderTextureHeight, RenderTextureDepth, RenderTextureFormat.ARGB32);
        PreviewRenderTexture.Create();
        PreviewRenderTexture.name = RenderTextureName;
    }

    void createCameraObject()
    {
        CameraGameObject = new GameObject(PreviewCameraName);
        PreviewCamera = CameraGameObject.AddComponent<Camera>();
        PreviewCamera.clearFlags = CameraClearFlags.SolidColor;
        PreviewCamera.backgroundColor = BackGroundColor;


        PreviewCamera.targetTexture = PreviewRenderTexture;
        PreviewCamera.depth = PreviewCameraDepth;
        PreviewCamera.tag = "MainCamera";
    }

    void CreateCanvasObjectStraight()
    {
        Straight_RenderCanvasGameObject = new GameObject(Straight_RenderCanvasName);
        Straight_RenderCanvas = Straight_RenderCanvasGameObject.AddComponent<Canvas>();
        Straight_RenderCanvas.worldCamera = PreviewCamera;
        Straight_RenderCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        Straight_RenderCanvas.vertexColorAlwaysGammaSpace = true;

        Straight_RenderText = Straight_RenderCanvasGameObject.AddComponent<TextMeshProUGUI>();
        Straight_RenderText.text = Straight_RenderTextInput;
        Straight_RenderText.color = Straight_RenderTextColor;
        Straight_RenderText.fontSize = Straight_RenderTextFontSize;
        Straight_RenderText.alignment = Straight_RenderTextAlignment;
    }

    void OnEnable()
    {
        //opens a new scene that will be used
        creatScene();

        //creates the Render Texture
        createRT();

        //Create the Camera object in the scene
        createCameraObject();

        if (CirculizeText == false)
        {
            //Create objects in the scene
            CreateCanvasObjectStraight();
        }
        else
        {
            CreateCanvasObjectCircle();
        }

    }

    void CreateCanvasObjectCircle()
    {
        Top_RenderCanvasGameObject = new GameObject(Top_RenderCanvasName);
        Top_RenderCanvas = Top_RenderCanvasGameObject.AddComponent<Canvas>();
        Top_RenderCanvas.worldCamera = PreviewCamera;
        Top_RenderCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        Top_RenderCanvas.vertexColorAlwaysGammaSpace = true;

        Top_RenderText = Top_RenderCanvasGameObject.AddComponent<TextMeshProUGUI>();
        Top_RenderText.text = Top_RenderTextInput;
        Top_RenderText.color = Top_RenderTextColor;
        Top_RenderText.fontSize = Top_RenderTextFontSize;
        Top_RenderText.alignment = TextAlignmentOptions.Center;

        Top_CirculizeTextScript = Top_RenderCanvasGameObject.AddComponent<TextProOnACircle>();
        Top_CirculizeTextScript.m_radius = Top_tmpC_Radius;
        Top_CirculizeTextScript.m_arcDegrees = Top_tmpC_Arc_Degrees;
        Top_CirculizeTextScript.m_angularOffset = Top_tmpC_Angular_Offset;


        Bottom_RenderCanvasGameObject = new GameObject(Bottom_RenderCanvasName);
        Bottom_RenderCanvas = Bottom_RenderCanvasGameObject.AddComponent<Canvas>();
        Bottom_RenderCanvas.worldCamera = PreviewCamera;
        Bottom_RenderCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        Bottom_RenderCanvas.vertexColorAlwaysGammaSpace = true;

        Bottom_RenderText = Bottom_RenderCanvasGameObject.AddComponent<TextMeshProUGUI>();
        Bottom_RenderText.text = Bottom_RenderTextInput;
        Bottom_RenderText.color = Bottom_RenderTextColor;
        Bottom_RenderText.fontSize = Bottom_RenderTextFontSize;
        Bottom_RenderText.alignment = TextAlignmentOptions.Center;

        Bottom_CirculizeTextScript = Bottom_RenderCanvasGameObject.AddComponent<TextProOnACircle>();
        Bottom_CirculizeTextScript.m_radius = Bottom_tmpC_Radius;
        Bottom_CirculizeTextScript.m_arcDegrees = Bottom_tmpC_Arc_Degrees;
        Bottom_CirculizeTextScript.m_angularOffset = Bottom_tmpC_Angular_Offset;
    }

    void ForceUpdateBackground()
    {
        HDAdditionalCameraData HDData = previewCamera.GetComponent<HDAdditionalCameraData>();
        if (HDData != null)
        {
            hdData.clearColorMode = HDAdditionalCameraData.ClearColorMode.Color;
            hdData.backgroundColorHDR = bgColor;
            hdData.volumeLayerMask = 0;
            Repaint();
        }
    }

    void OnGUI()
    {
        Rect rect = GUILayoutUtility.GetAspectRect(RenderTexturetWidth / RenderTextureHeight);
        EditorGUI.DrawPreviewTexture(rect, PreviewRenderTexture);

        //--------------------------------------------------------------------------------------------
        
        ImageName = EditorGUILayout.TextField("ImageName", ImageName);

        //--------------------------------------------------------------------------------------------

        CirculizeText = EditorGUILayout.Toggle("Circulize Text", CirculizeText);

        //--------------------------------------------------------------------------------------------

        if (CirculizeText == false)
        {
            if (Straight_RenderCanvasGameObject == null)
            {
                CreateCanvasObjectStraight();
            }

            if (Top_RenderCanvasGameObject != null)
            {
                DestroyImmediate(Top_RenderCanvasGameObject);
            }

            if (Bottom_RenderCanvasGameObject != null)
            {
                DestroyImmediate(Bottom_RenderCanvasGameObject);
            }

            Straight_RenderTextInput = EditorGUILayout.TextField("Input String", Straight_RenderTextInput);
            Straight_RenderTextFontSize = EditorGUILayout.FloatField("Text Font", Straight_RenderTextFontSize);

            Straight_RenderText.text = Straight_RenderTextInput;
            Straight_RenderText.fontSize = Straight_RenderTextFontSize;

            SelectedAlignemntIndex = EditorGUILayout.Popup("Alignment Options", SelectedAlignemntIndex, AlignmentOptions);

            switch (SelectedAlignemntIndex)
            {
                case 0:
                    Straight_RenderTextAlignment = TextAlignmentOptions.Left;
                    Straight_RenderText.alignment = Straight_RenderTextAlignment;
                    break;
                case 1:
                    Straight_RenderTextAlignment = TextAlignmentOptions.Center;
                    Straight_RenderText.alignment = Straight_RenderTextAlignment;
                    break;
                case 2:
                    Straight_RenderTextAlignment = TextAlignmentOptions.Right;
                    Straight_RenderText.alignment = Straight_RenderTextAlignment;
                    break;
            }

            Repaint();
        }
        else
        {
            if (Top_RenderCanvasGameObject == null & Bottom_RenderCanvasGameObject == null)
            {
                CreateCanvasObjectCircle();
            }

            if (Straight_RenderCanvasGameObject != null)
            {
                DestroyImmediate(Straight_RenderCanvasGameObject);
            }

            Top_RenderTextInput = EditorGUILayout.TextField("Input String", Top_RenderTextInput);
            Top_RenderTextFontSize = EditorGUILayout.FloatField("Text Font", Top_RenderTextFontSize);
            Top_tmpC_Radius = EditorGUILayout.FloatField("Radius", Top_tmpC_Radius);
            Top_tmpC_Arc_Degrees = EditorGUILayout.FloatField("Arc Degrees", Top_tmpC_Arc_Degrees);
            Top_tmpC_Angular_Offset = EditorGUILayout.FloatField("Angular Offset", Top_tmpC_Angular_Offset);

            Top_RenderText.text = Top_RenderTextInput;
            Top_RenderText.fontSize = Top_RenderTextFontSize;

            Top_CirculizeTextScript.m_radius = Top_tmpC_Radius;
            Top_CirculizeTextScript.m_arcDegrees = Top_tmpC_Arc_Degrees;
            Top_CirculizeTextScript.m_angularOffset = Top_tmpC_Angular_Offset;

            GUILayout.Space(20);

            Bottom_RenderTextInput = EditorGUILayout.TextField("Input String", Bottom_RenderTextInput);
            Bottom_RenderTextFontSize = EditorGUILayout.FloatField("Text Font", Bottom_RenderTextFontSize);
            Bottom_tmpC_Radius = EditorGUILayout.FloatField("Radius", Bottom_tmpC_Radius);
            Bottom_tmpC_Arc_Degrees = EditorGUILayout.FloatField("Arc Degrees", Bottom_tmpC_Arc_Degrees);
            Bottom_tmpC_Angular_Offset = EditorGUILayout.FloatField("Angular Offset", Bottom_tmpC_Angular_Offset);

            Bottom_RenderText.text = Bottom_RenderTextInput;
            Bottom_RenderText.fontSize = Bottom_RenderTextFontSize;

            Bottom_CirculizeTextScript.m_radius = Bottom_tmpC_Radius;
            Bottom_CirculizeTextScript.m_arcDegrees = Bottom_tmpC_Arc_Degrees;
            Bottom_CirculizeTextScript.m_angularOffset = Bottom_tmpC_Angular_Offset;

            Repaint();
        }

        
        if (GUILayout.Button("Render to Image"))
        {
            RenderTextImage()
        }
    
                
        ForceUpdateBackground()
    }

    void RenderTextImage()
    {
        ProjectPath = Application.dataPath;

        RenderTexture.active = PreviewRenderTexture;
        previewCamera.Render();

        RenderPNGImage = new Texture2D(PreviewRenderTexture.width, PreviewRenderTexture.height, TextureFormat.RGB24, false);
        RenderPNGImage.ReadPixels(new Rect(0, 0, PreviewRenderTexture.width, PreviewRenderTexture.height), 0, 0);
        RenderPNGImage.Apply();

        Bytes = RenderPNGImage.EncodeToPNG();

        if (string.IsNullOrEmpty(ImageName))
        {
            string filePath = Path.Combine(ProjectPAth + "/" + "Empty" + ".png");

            File.WriteAllBytes(filePath, Bytes);
        }
        else
        {
            string filePath = Path.Combine(ProjectPAth + "/" + ImageName + ".png");

            File.WriteAllBytes(filePath, Bytes
        }

        AssetDatabase.Refresh();
        DestroyImmediate(RenderPNGImage);
    }

    void OnDisable()
    {
        PreviewRenderTexture.Release();
        EditorSceneManager.OpenScene(PreviousScenePath, OpenSceneMode.Single);
    }
}
