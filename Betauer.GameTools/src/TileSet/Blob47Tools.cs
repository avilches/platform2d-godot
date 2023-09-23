namespace Betauer.TileSet;

public static class Blob47Tools {
    // The 256 blob tileset has too many tiles, most of them can be converted to the blob 47
    // This array contains the mapping from the 256 blob to the blob 47 in each position, so the
    // tile 2 real mask (which is 0) is in the position 2 of the array
    public static int[] Blob256To47 = new int[256] { 0, 1, 0, 1, 4, 5, 4, 7, 0, 1, 0, 1, 4, 5, 4, 7, 16, 17, 16, 17, 20, 21, 20, 23, 16, 17, 16, 17, 28, 29, 28, 31, 0, 1, 0, 1, 4, 5, 4, 7, 0, 1, 0, 1, 4, 5, 4, 7, 16, 17, 16, 17, 20, 21, 20, 23, 16, 17, 16, 17, 28, 29, 28, 31, 64, 65, 64, 65, 68, 69, 68, 71, 64, 65, 64, 65, 68, 69, 68, 71, 80, 81, 80, 81, 84, 85, 84, 87, 80, 81, 80, 81, 92, 93, 92, 95, 64, 65, 64, 65, 68, 69, 68, 71, 64, 65, 64, 65, 68, 69, 68, 71, 112, 113, 112, 113, 116, 117, 116, 119, 112, 113, 112, 113, 124, 125, 124, 127, 0, 1, 0, 1, 4, 5, 4, 7, 0, 1, 0, 1, 4, 5, 4, 7, 16, 17, 16, 17, 20, 21, 20, 23, 16, 17, 16, 17, 28, 29, 28, 31, 0, 1, 0, 1, 4, 5, 4, 7, 0, 1, 0, 1, 4, 5, 4, 7, 16, 17, 16, 17, 20, 21, 20, 23, 16, 17, 16, 17, 28, 29, 28, 31, 64, 193, 64, 193, 68, 197, 68, 199, 64, 193, 64, 193, 68, 197, 68, 199, 80, 209, 80, 209, 84, 213, 84, 215, 80, 209, 80, 209, 92, 221, 92, 223, 64, 193, 64, 193, 68, 197, 68, 199, 64, 193, 64, 193, 68, 197, 68, 199, 112, 241, 112, 241, 116, 245, 116, 247, 112, 241, 112, 241, 124, 253, 124, 255};
}