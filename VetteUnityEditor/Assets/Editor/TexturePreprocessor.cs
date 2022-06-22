using UnityEditor;
using UnityEngine;

public class TexturePreprocessor : AssetPostprocessor
{
    private void OnPreprocessTexture()
    {
        var texturePreprocessor = (TextureImporter)assetImporter;
        texturePreprocessor.mipmapEnabled = false;
        texturePreprocessor.filterMode = FilterMode.Point;
        texturePreprocessor.textureCompression = TextureImporterCompression.Uncompressed;
    }
}
