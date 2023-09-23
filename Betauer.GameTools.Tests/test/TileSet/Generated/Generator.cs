using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Betauer.TestRunner;
using Betauer.TileSet;
using Betauer.TileSet.Image;
using Betauer.TileSet.Terrain;
using Godot;
using NUnit.Framework;

namespace Betauer.GameTools.Tests.TileSet.Generated;

[Betauer.TestRunner.Test]
[Betauer.TestRunner.Ignore("Only use it to generate the test and the Blob47Tools.cs file")]
public class GeneratorTests {

    [Betauer.TestRunner.Test(Description = "Ensure the Tilemap.tscn has all the possible values for the minimal 3x3 tileset blob 47")]
    public void VerifyTest() {
        var scene = ResourceLoader.Load<PackedScene>("res://test-resources/tileset/Tilemap.tscn");
        var godotTileMap = scene.Instantiate<Godot.TileMap>();
        var layer = 0;
        // var godotTileSet = ResourceLoader.Load<Godot.TileSet>("res://test-resources/tileset/Tileset.tres");
        var tiles = godotTileMap.GetTerrainMasksGrid(layer);
        var values = tiles.Cast<int>().Distinct().ToList();
        values.Remove(-1);
        Assert.That(values.Count, Is.EqualTo(47));
        CollectionAssert.AreEquivalent(values, TileSetLayouts.Minimal3X3Godot.GetTileIds());
    }
    
    /*
     * How the Blob47Test.cs is generated
     *
     * 1) Run the Generate256Combinations() test only to generate the Tilemap-256-no-connected.tscn scene
     * 2) Open the Tilemap-256-no-connected.tscn in the Godot editor, connect all the tiles manually and save the file as Tilemap-256.tscn
     * 3) Run the GenerateBlob47Tests() test to generate the Blob47Tests.cs and the Blob47Tools.cs files
     * 4) Now you can run the Blob47Tests.cs test to verify that all the 256 combinations are correctly mapped to the blob 47
     */

    [Betauer.TestRunner.Test(Description = "Generate a no connected patterns in a Godot Tilemap with all the 256 combinations")]
    [Betauer.TestRunner.Ignore(
    "This test is only to generate a file that it will need to manually in Godot and save it as Tilemap-256.tscn with all the connections")]
    public void Generate256Combinations() {
        var scene = ResourceLoader.Load<PackedScene>("res://test-resources/tileset/Tilemap.tscn");

        var godotTileMap = scene.Instantiate<TileMap>();
        godotTileMap.Clear();

        var terrain = new SingleTerrain(16 * 4, 16 * 4);
        var x = 0;
        var y = 0;
        for (var i = 0; i < 256; i++) {
            var neighbours = TerrainTools.CreateNeighboursGrid(i);
            terrain.SetCells(x, y, neighbours);
            x += 4;
            if (x >= terrain.Width) {
                x = 0;
                y += 4;
            }
        }
        var layer = 0;
        var sourceId = 2;
        var atlasCoords = new Vector2I(0, 3);
        for (var yy = 0; yy < terrain.Height; yy++) {
            for (var xx = 0; xx < terrain.Width; xx++) {
                var tileId = terrain.Grid[yy, xx];
                if (tileId >= 0) {
                    godotTileMap.SetCell(layer, new Vector2I(xx, yy), sourceId, atlasCoords);
                }
            }
        }
        var packedScene = new PackedScene();
        packedScene.Pack(godotTileMap);
        ResourceSaver.Save(packedScene, "res://test-resources/tileset/Tilemap-256-no-connected.tscn");
    }

    [Betauer.TestRunner.Test(Description = "Generate Blob47Tests.cs, which tests all the 256 possible combinations and the Blob47Tools.cs with the blob 256 to blob 47 mapping")]
    public void GenerateBlob47Tests() {
        var scene = ResourceLoader.Load<PackedScene>("res://test-resources/tileset/Tilemap-256.tscn");

        var godotTileMap = scene.Instantiate<TileMap>();
        var x = 0;
        var y = 0;
        var tiles = new List<int>();
        var tileLegend = new StringWriter();
        for (var i = 0; i < 256; i++) {
            var tileData = godotTileMap.GetCellTileData(0, new Vector2I(x + 1, y + 1));
            var bitmask = tileData?.GetTerrainMask() ?? -1;
            tileLegend.Write(i.ToString().PadLeft(3) + ":" + bitmask.ToString().PadRight(3) + " | ");
            tiles.Add(bitmask);
            x += 4;
            if (x >= 16 * 4) {
                x = 0;
                tileLegend.WriteLine();
                y += 4;
            }
        }

        File.WriteAllText("Betauer.GameTools/src/TileSet/Blob47Tools.cs",
            $$"""
              namespace Betauer.TileSet;

              public static class Blob47Tools {
                  // The 256 blob tileset has too many tiles, most of them can be converted to the blob 47
                  // This array contains the mapping from the 256 blob to the blob 47 in each position, so the
                  // tile 2 real mask (which is 0) is in the position 2 of the array
                  public static int[] Blob256To47 = new int[256] { {{string.Join(", ", tiles)}}};
              }
              """);

        Console.WriteLine(tileLegend);
        var values = tiles.Distinct().ToList();
        Assert.That(values.Count, Is.EqualTo(47));
        CollectionAssert.AreEquivalent(values, TileSetLayouts.Minimal3X3Godot.GetTileIds());

        Dictionary<int, List<int>> shared = new();
        for (var i = 0; i < 256; i++) {
            var tile = tiles[i];
            if (!shared.ContainsKey(tile)) {
                shared[tile] = new List<int>() { i };
            } else {
                shared[tile].Add(i);
            }
        }
        var terrain = new SingleTerrain(3, 3);
        var testClass = new StringWriter();
        foreach (var (mainTileId, sharedList) in shared) {
            testClass.WriteLine($"    [Test(Description=\"{mainTileId} when {string.Join(",", sharedList)}\")]");
            testClass.WriteLine($"    public void TestTile{mainTileId}() {{");
            x = 0;
            foreach (var tileId in sharedList) {
                terrain.SetCells(0, 0, TerrainTools.CreateNeighboursGrid(tileId));
                x += 4;
                testClass.WriteLine($"        ");
                testClass.WriteLine($"        // Pattern where central tile with {tileId} mask is transformed to {mainTileId}");
                testClass.WriteLine($"        AssertExpandGrid(\"\"\"");
                for (y = 0; y < terrain.Grid.GetLength(0); y++) {
                    testClass.Write($"                         :");
                    for (x = 0; x < terrain.Grid.GetLength(1); x++) {
                        testClass.Write(terrain.Grid[y, x] >= 0 ? "*" : " ");
                    }
                    testClass.WriteLine(":");
                }
                testClass.WriteLine($"                         \"\"\", new[,] {{");
                var maskGrid = godotTileMap.GetTerrainMasksGrid(0, tileId % 16 * 4, tileId / 16 * 4, 3, 3);
                for (y = 0; y < maskGrid.GetLength(0); y++) {
                    testClass.Write($"                         {{");
                    for (x = 0; x < maskGrid.GetLength(1); x++) {
                        testClass.Write(maskGrid[y, x].ToString().PadLeft(4) + ", ");
                    }
                    testClass.WriteLine("}, ");
                }
                testClass.WriteLine($"                         }});");
            }
            testClass.WriteLine($"   }}");
        }
        // write testClass to a file
        File.WriteAllText("Betauer.GameTools.Tests/test/TileSet/Generated/Blob47Tests.cs",
            $$"""
              using Betauer.TestRunner;

              namespace Betauer.GameTools.Tests.TileSet.Generated;

              [Test]
              public class Blob47Tests : BaseBlobTests {
              {{testClass}}
              }
              """);
    }
}