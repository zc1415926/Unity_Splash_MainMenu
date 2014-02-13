using UnityEngine;
using UnityEditor;
using System.Collections;

public class GUITools : ScriptableObject {

    [MenuItem ("GUITools/Apply Uncompressed GUI Texture Settings")]
    static void ApplyHighTextureSettings () {
        Object[] textures = Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets); 
        Selection.objects = new Object[0];
        foreach (Texture2D texture in textures)  {
            string path = AssetDatabase.GetAssetPath(texture); 
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter; 
            textureImporter.npotScale = TextureImporterNPOTScale.None;  
            textureImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;
            textureImporter.mipmapEnabled = false;
            AssetDatabase.ImportAsset(path); 
        }
    }

    [MenuItem ("GUITools/Apply Compressed GUI Texture Settings")]
    static void ApplyLowTextureSettings () {
        Object[] textures = Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets); 
        Selection.objects = new Object[0];
        foreach (Texture2D texture in textures)  {
            string path = AssetDatabase.GetAssetPath(texture); 
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter; 
            textureImporter.npotScale = TextureImporterNPOTScale.ToNearest;  
            textureImporter.textureFormat = TextureImporterFormat.AutomaticCompressed;
            textureImporter.mipmapEnabled = false;
            AssetDatabase.ImportAsset(path); 
        }
    }

}
