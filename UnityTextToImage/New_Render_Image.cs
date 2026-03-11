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

        //GUILayout.Space(20);

        //if (GUILayout.Button("Turn Background Black"))
        //{
        //    updatecamBG();
        //}

        GUILayout.Space(20);

        if (GUILayout.Button("Render The Image"))
        {
            RenderImage();
        }
    }

    //void updatecamBG()
    //{
    //    HDAdditionalCameraData hdData = previewCamera.GetComponent<HDAdditionalCameraData>();
    //    hdData.clearColorMode = HDAdditionalCameraData.ClearColorMode.Color;
    //    hdData.backgroundColorHDR = bgColor;
    //    hdData.volumeLayerMask = 0;
    //    Repaint();
        
    //}

    void RenderImage()
    {
        {
            ProjectPath = Application.dataPath;

            RenderTexture.active = PreviewRenderTexture;
            PreviewCamera.Render();

            RenderPNGImage = new Texture2D(PreviewRenderTexture.width, PreviewRenderTexture.height, TextureFormat.RGB24, false);
            RenderPNGImage.ReadPixels(new Rect(0, 0, PreviewRenderTexture.width, PreviewRenderTexture.height), 0, 0);
            RenderPNGImage.Apply();

            Bytes = RenderPNGImage.EncodeToPNG();

            if (string.IsNullOrEmpty(ImageName))
            {
                string filePath = Path.Combine(ProjectPath + "/" + "Empty" + ".png");

                File.WriteAllBytes(filePath, Bytes);
            }
            else
            {
                string filePath = Path.Combine(ProjectPath + "/" + ImageName + ".png");

                File.WriteAllBytes(filePath, Bytes);
            }

            AssetDatabase.Refresh();
            DestroyImmediate(RenderPNGImage);
        }
    }

    void OnDisable()
    {
        PreviewRenderTexture.Release();
        EditorSceneManager.OpenScene(PreviousScenePath, OpenSceneMode.Single);
    }
}

#region

[ExecuteInEditMode]
public abstract class TextProOnACurve : MonoBehaviour
{
    private TMP_Text m_TextComponent;

    private bool m_forceUpdate;

    private void Awake()
    {
        m_TextComponent = gameObject.GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        //every time the object gets enabled, we have to force a re-creation of the text mesh
        m_forceUpdate = true;
    }

    protected void Update()
    {
        //if the text and the parameters are the same of the old frame, don't waste time in re-computing everything
        if (!m_forceUpdate && !m_TextComponent.havePropertiesChanged && !ParametersHaveChanged())
        {
            return;
        }

        m_forceUpdate = false;

        //during the loop, vertices represents the 4 vertices of a single character we're analyzing, 
        //while matrix is the roto-translation matrix that will rotate and scale the characters so that they will
        //follow the curve
        Vector3[] vertices;
        Matrix4x4 matrix;

        //Generate the mesh and get information about the text and the characters
        m_TextComponent.ForceMeshUpdate();

        TMP_TextInfo textInfo = m_TextComponent.textInfo;
        int characterCount = textInfo.characterCount;

        //if the string is empty, no need to waste time
        if (characterCount == 0)
            return;

        //gets the bounds of the rectangle that contains the text 
        float boundsMinX = m_TextComponent.bounds.min.x;
        float boundsMaxX = m_TextComponent.bounds.max.x;

        //for each character
        for (int i = 0; i < characterCount; i++)
        {
            //skip if it is invisible
            if (!textInfo.characterInfo[i].isVisible)
                continue;

            //Get the index of the mesh used by this character, then the one of the material... and use all this data to get
            //the 4 vertices of the rect that encloses this character. Store them in vertices
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
            vertices = textInfo.meshInfo[materialIndex].vertices;

            //Compute the baseline mid point for each character. This is the central point of the character.
            //we will use this as the point representing this character for the geometry transformations
            Vector3 charMidBaselinePos = new Vector2((vertices[vertexIndex + 0].x + vertices[vertexIndex + 2].x) / 2, textInfo.characterInfo[i].baseLine);

            //remove the central point from the vertices point. After this operation, every one of the four vertices 
            //will just have as coordinates the offset from the central position. This will come handy when will deal with the rotations
            vertices[vertexIndex + 0] += -charMidBaselinePos;
            vertices[vertexIndex + 1] += -charMidBaselinePos;
            vertices[vertexIndex + 2] += -charMidBaselinePos;
            vertices[vertexIndex + 3] += -charMidBaselinePos;

            //compute the horizontal position of the character relative to the bounds of the box, in a range [0, 1]
            //where 0 is the left border of the text and 1 is the right border
            float zeroToOnePos = (charMidBaselinePos.x - boundsMinX) / (boundsMaxX - boundsMinX);

            //get the transformation matrix, that maps the vertices, seen as offset from the central character point, to their final
            //position that follows the curve
            matrix = ComputeTransformationMatrix(charMidBaselinePos, zeroToOnePos, textInfo, i);

            //apply the transformation, and obtain the final position and orientation of the 4 vertices representing this char
            vertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 0]);
            vertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 1]);
            vertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 2]);
            vertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 3]);
        }

        //Upload the mesh with the revised information
        m_TextComponent.UpdateVertexData();
    }

    protected abstract bool ParametersHaveChanged();

    protected abstract Matrix4x4 ComputeTransformationMatrix(Vector3 charMidBaselinePos, float zeroToOnePos, TMP_TextInfo textInfo, int charIdx);
}

[ExecuteInEditMode]
public class TextProOnACircle : TextProOnACurve
{
    public float m_radius = 10.0f;

    public float m_arcDegrees = 90.0f;

    public float m_angularOffset = -90;

    private int m_maxDegreesPerLetter = 360;

    private float m_oldRadius = float.MaxValue;

    private float m_oldArcDegrees = float.MaxValue;

    private float m_oldAngularOffset = float.MaxValue;

    private float m_oldMaxDegreesPerLetter = float.MaxValue;

    protected override bool ParametersHaveChanged()
    {
        //check if paramters have changed and update the old values for next frame iteration
        bool retVal = m_radius != m_oldRadius || m_arcDegrees != m_oldArcDegrees || m_angularOffset != m_oldAngularOffset || m_oldMaxDegreesPerLetter != m_maxDegreesPerLetter;

        m_oldRadius = m_radius;
        m_oldArcDegrees = m_arcDegrees;
        m_oldAngularOffset = m_angularOffset;
        m_oldMaxDegreesPerLetter = m_maxDegreesPerLetter;

        return retVal;
    }

    protected override Matrix4x4 ComputeTransformationMatrix(Vector3 charMidBaselinePos, float zeroToOnePos, TMP_TextInfo textInfo, int charIdx)
    {
        //calculate the actual degrees of the arc considering the maximum distance between letters
        float actualArcDegrees = Mathf.Min(m_arcDegrees, textInfo.characterCount / textInfo.lineCount * m_maxDegreesPerLetter);

        //compute the angle at which to show this character.
        //We want the string to be centered at the top point of the circle, so we first convert the position from a range [0, 1]
        //to a [-0.5, 0.5] one and then add m_angularOffset degrees, to make it centered on the desired point
        float angle = ((zeroToOnePos - 0.5f) * actualArcDegrees + m_angularOffset) * Mathf.Deg2Rad; //we need radians for sin and cos

        //compute the coordinates of the new position of the central point of the character. Use sin and cos since we are on a circle.
        //Notice that we have to do some extra calculations because we have to take in count that text may be on multiple lines
        float x0 = Mathf.Cos(angle);
        float y0 = Mathf.Sin(angle);
        float radiusForThisLine = m_radius - textInfo.lineInfo[0].lineExtents.max.y * textInfo.characterInfo[charIdx].lineNumber;
        Vector2 newMideBaselinePos = new Vector2(x0 * radiusForThisLine, -y0 * radiusForThisLine); //actual new position of the character

        //compute the trasformation matrix: move the points to the just found position, then rotate the character to fit the angle of the curve 
        //(-90 is because the text is already vertical, it is as if it were already rotated 90 degrees)
        return Matrix4x4.TRS(new Vector3(newMideBaselinePos.x, newMideBaselinePos.y, 0), Quaternion.AngleAxis(-Mathf.Atan2(y0, x0) * Mathf.Rad2Deg - 90, Vector3.forward), Vector3.one);
    }
}
#endregion
