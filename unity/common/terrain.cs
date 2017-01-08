using UnityEditor;
using UnityEngine;

using Ts.Grayscale;

namespace Ts.Unity
{
    public static class UnityTerrain
    {
        public static void DebugControls(this Terrain t, Generator g)
        {
            if (GUILayout.Button("Generate Heightmap")) {
                t.ApplyHeightmap(g);
            }

            if (GUILayout.Button("Reset Heightmap")) {
                t.ResetHeightmap();
            }

            if (GUILayout.Button("Generate Splatmap")) {
                t.ApplySplatmap(g);
            }

            if (GUILayout.Button("Reset Splatmap")) {
                t.ResetSplatmap();
            }
        }

        public static void ApplyHeightmap(this Terrain t, Generator g)
        {
            GrayscaleImage heightmap = g.GenerateImage(
                t.terrainData.heightmapHeight, t.terrainData.heightmapWidth);
            t.terrainData.SetHeights(0, 0, heightmap.ToArray());
        }

        public static void ResetHeightmap(this Terrain t)
        {
            float[,] heightmap =
                new float[t.terrainData.heightmapHeight, t.terrainData.heightmapWidth];
            Ts.Arrays.ForEach2D(heightmap, (int row, int col, ref float val) => {
                val = 0;
            });
            t.terrainData.SetHeights(0, 0, heightmap);
        }

        public static void ApplySplatmap(this Terrain t, Generator g)
        {
            // Set textures to black and white
            SplatPrototype black = new SplatPrototype();
            black.texture = Texture2D.blackTexture;
            SplatPrototype white = new SplatPrototype();
            white.texture = Texture2D.whiteTexture;
            t.terrainData.splatPrototypes = new SplatPrototype[]{ black, white };

            // Update the splatmap
            float[,,] splatmap =
                new float[t.terrainData.alphamapHeight, t.terrainData.alphamapWidth, 2];
            GrayscaleImage noise = g.GenerateImage(
                t.terrainData.alphamapHeight, t.terrainData.alphamapWidth);
            for (int x = 0; x < splatmap.GetLength(1); x++) {
                for (int y = 0; y < splatmap.GetLength(0); y++) {
                    float val = noise.GetValue(x, y);
                    splatmap[y, x, 0] = 1 - val;
                    splatmap[y, x, 1] = val;
                }
            }
            t.terrainData.SetAlphamaps(0, 0, splatmap);
        }

        public static void ResetSplatmap(this Terrain t)
        {
            float[,,] splatmap =
                new float[t.terrainData.alphamapHeight, t.terrainData.alphamapWidth, 2];
            for (int x = 0; x < splatmap.GetLength(1); x++) {
                for (int y = 0; y < splatmap.GetLength(0); y++) {
                    splatmap[y, x, 0] = 0;
                    splatmap[y, x, 1] = 1;
                }
            }
            t.terrainData.SetAlphamaps(0, 0, splatmap);
        }
    }
}
