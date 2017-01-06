using UnityEditor;
using UnityEngine;

using Ts.Grayscale;
using Ts.Grayscale.Generators;

public class PerlinDebugger : EditorWindow
{
    private static Terrain _terrain;

    private float _amplitude = 0;
    private float _frequency = 50;
    private int _octaves = 1;
    private float _persistence = 0.5f;

    [MenuItem("TerrainScript/Debug/Perlin")]
    public static void ShowWindow()
    {
        if (!Terrain.activeTerrain) {
            EditorUtility.DisplayDialog("No terrain detected.", "Shutting down TerainScript.", "OK");
            return;
        }

        _terrain = Terrain.activeTerrain;

        EditorWindow.GetWindow(typeof(PerlinDebugger));
    }

    public void OnGUI()
    {
        GUILayout.Label("Perlin");

        _amplitude = EditorGUILayout.Slider(_amplitude, 0, _terrain.terrainData.size.y);
        _frequency = EditorGUILayout.Slider(_frequency, 0, 100);
        _octaves = EditorGUILayout.IntSlider(_octaves, 1, 25);
        _persistence = EditorGUILayout.Slider(_persistence, 0, 1);

        if (GUILayout.Button("Generate Heightmap")) {
            perlinHeight();
        }

        if (GUILayout.Button("Reset Heightmap")) {
            resetHeight();
        }

        if (GUILayout.Button("Generate Splatmap")) {
            perlinSplat();
        }

        if (GUILayout.Button("Reset Splatmap")) {
            resetSplat();
        }
    }

    private GrayscaleImage perlin(int height, int width)
    {
        Generator p = new Perlin(
                _amplitude / _terrain.terrainData.size.y, _frequency, _octaves, _persistence);
        return p.GenerateImage(height, width);
    }

    private void perlinHeight()
    {
        GrayscaleImage heightmap = perlin(
            _terrain.terrainData.heightmapHeight, _terrain.terrainData.heightmapWidth);
        _terrain.terrainData.SetHeights(0, 0, heightmap.ToArray());
    }

    private void resetHeight()
    {
        float[,] heightmap =
            new float[_terrain.terrainData.heightmapHeight, _terrain.terrainData.heightmapWidth];
        Ts.Arrays.ForEach2D(heightmap, (int row, int col, ref float val) => {
            val = 0;
        });
        _terrain.terrainData.SetHeights(0, 0, heightmap);
    }

    private void perlinSplat()
    {
        // Set textures to black and white
        SplatPrototype black = new SplatPrototype();
        black.texture = Texture2D.blackTexture;
        SplatPrototype white = new SplatPrototype();
        white.texture = Texture2D.whiteTexture;
        _terrain.terrainData.splatPrototypes = new SplatPrototype[]{ black, white };

        // Update the splatmap
        float[,,] splatmap =
            new float[_terrain.terrainData.alphamapHeight, _terrain.terrainData.alphamapWidth, 2];
        GrayscaleImage noise = perlin(
            _terrain.terrainData.alphamapHeight, _terrain.terrainData.alphamapWidth);
        for (int x = 0; x < splatmap.GetLength(0); x++) {
            for (int y = 0; y < splatmap.GetLength(1); y++) {
                float val = noise.GetValue(x, y);
                splatmap[x, y, 0] = 1 - val;
                splatmap[x, y, 1] = val;
            }
        }
        _terrain.terrainData.SetAlphamaps(0, 0, splatmap);
    }

    private void resetSplat()
    {
        float[,,] splatmap =
            new float[_terrain.terrainData.alphamapHeight, _terrain.terrainData.alphamapWidth, 2];
        for (int x = 0; x < splatmap.GetLength(0); x++) {
            for (int y = 0; y < splatmap.GetLength(1); y++) {
                splatmap[x, y, 0] = 0;
                splatmap[x, y, 1] = 1;
            }
        }
        _terrain.terrainData.SetAlphamaps(0, 0, splatmap);
    }
}
