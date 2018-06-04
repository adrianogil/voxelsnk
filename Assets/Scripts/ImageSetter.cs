using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ImageSetter : MonoBehaviour
{
    public int imageSizeX, imageSizeY;

    public RenderTexture image;
    private Texture2D imageTexture2D;

    [HideInInspector]
    public int pixelPosX;

    [HideInInspector]
    public int pixelPosY;

    [HideInInspector]
    public Color newColor;

    [HideInInspector]
    public int currentGridPosX;

    [HideInInspector]
    public int currentGridPosY;

    void Awake()
    {
        UpdateInternalTexture();
    }
    
    public void UpdateInternalTexture()
    {
        RenderTexture.active = image;
        imageSizeX = image.width;
        imageSizeY = image.height;

        imageTexture2D = new Texture2D(imageSizeX, imageSizeY);
        imageTexture2D.ReadPixels(new Rect(0, 0, imageSizeX, imageSizeY), 0, 0);
        imageTexture2D.Apply();
    }

    // <summary>
    /// SetPixelColor
    ///  - Set a Color in a specific pixel
    ///  - For debug purposes
    /// </summary>
    public void SetPixelColor(int x, int y, Color color, bool update = true)
    {
        Debug.Log("Set Pixel Color in pos (" + x + "," + y + ") with color " + color);

        imageTexture2D.SetPixel(x, y, color);

        if (update)
        {
            imageTexture2D.Apply();

            Graphics.Blit(imageTexture2D, image);    
        }
    }

    public Color GetPixelColor(int x, int y)
    {
        if (imageTexture2D != null)
         return imageTexture2D.GetPixel(x, y);

        return Color.black;
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(ImageSetter))]
public class ImageSetterEditor : Editor {

    private const int maxInnerWidth = 250;
    private const int maxInnerHeight = 80;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    
        ImageSetter editorObj = target as ImageSetter;
    
        if (editorObj == null) return;
        
        if (GUILayout.Button("UpdateInternalTexture"))
        {
            editorObj.UpdateInternalTexture();
        }

        editorObj.pixelPosX = EditorGUILayout.IntField("Pixel Position X:", editorObj.pixelPosX);
        editorObj.pixelPosY = EditorGUILayout.IntField("Pixel Position Y:", editorObj.pixelPosY);
        editorObj.newColor = EditorGUILayout.ColorField("New Color", editorObj.newColor);

        if (GUILayout.Button("Set Pixel"))
        {
            editorObj.SetPixelColor(editorObj.pixelPosX,
                                        editorObj.pixelPosY,
                                        editorObj.newColor);
        }

        OnDebugGrid(editorObj, editorObj.imageSizeX, editorObj.imageSizeY);
    }

    public void OnDebugGrid(ImageSetter editorObj, int sizeX, int sizeY)
    {
        editorObj.currentGridPosX = EditorGUILayout.IntField("Grid Start Position X:", editorObj.currentGridPosX);
        editorObj.currentGridPosY = EditorGUILayout.IntField("Grid Start Position Y:", editorObj.currentGridPosY);

        // Number of Cells
        int cols = 25, rows = 15;

        float gridItemWidth = maxInnerWidth / (1.0f * cols);

        // GUI.Box(new Rect(5,5, 800, 800), "Colors");
        // GUILayout.BeginArea(new Rect(10, 10, 700, 700));
        GUILayout.BeginVertical();
        for (int y = 0; y < rows && y < sizeY - editorObj.currentGridPosY; y++)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < cols && x < sizeX - editorObj.currentGridPosX; x++)
            {
                Color lastColor = editorObj.GetPixelColor(x + editorObj.currentGridPosX,
                                                                   y + editorObj.currentGridPosY);

                Color newColor = EditorGUILayout.ColorField(GUIContent.none,
                                           // colorGrid.GetColor(x, y),
                                           lastColor,
                                           false, true, false, null, GUILayout.Width(gridItemWidth));

                if (lastColor != newColor)
                {
                    editorObj.SetPixelColor(x + editorObj.currentGridPosX,
                                        y + editorObj.currentGridPosY,
                                        newColor);
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
        // GUILayout.EndArea();
    } 

}
#endif