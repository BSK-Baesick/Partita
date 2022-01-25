using UnityEditor;
using UnityEngine;

public class NormalMapGenerator : EditorWindow
{
    Texture2D sprite;

    [MenuItem("Tools/Normal Map Generator")]
    public static void ShowWindow()
    {
        GetWindow(typeof(NormalMapGenerator));
    }

    private void OnGUI()
    {
        GUILayout.Label("Generate Normal Map", EditorStyles.boldLabel);

        sprite = (Texture2D)EditorGUILayout.ObjectField("Sprite", sprite, typeof(Texture2D), false);

        if (GUILayout.Button("Generate"))
        {
            if (sprite == null)
            {
                ShowNotification(new GUIContent("No sprite has been added"));
            } else
            {
                NormalMap(sprite);
                ShowNotification(new GUIContent("Normal map has been generated"));
            }
        }

        GUILayout.Space(15f);

        GUILayout.Label("Click this after generating", EditorStyles.boldLabel);

        if (GUILayout.Button("Refresh Project Window"))
        {
            AssetDatabase.Refresh();
            TextureImport(sprite);
        }
    }

    private void NormalMap(Texture2D source)
    {
        string path = AssetDatabase.GetAssetPath(source);

        System.Diagnostics.Process.Start(Application.dataPath + "/Editor/LaigterPortable/laigter.exe", "--no-gui -d " + path + " -n");
    }

    private void TextureImport(Texture2D source)
    {
        string path = AssetDatabase.GetAssetPath(source);
        string filename = path.Replace(source.name, source.name + "_n");

        TextureImporter importer = AssetImporter.GetAtPath(filename) as TextureImporter;

        if (importer == null)
        {
            ShowNotification(new GUIContent("Try Again"));
        } else
        {
            importer.textureType = TextureImporterType.NormalMap;
            importer.SaveAndReimport();
        }
    }
}
