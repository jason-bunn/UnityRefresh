
using System.Numerics;

public enum BlockType
{
    None,
    Air,
    Grass,
    Dirt,
    Stone,
    Sand,
    Water,
    Wood,
    Leaves,
    IronOre,
    GoldOre,
    DiamondOre,
    CoalOre,
    Bedrock,
    Glass,
    Brick,
    Clay,
    Snow,
    Ice
}

public static class BlockUVs
{
    public static readonly int TileSize = 16; // Size of each tile in pixels
    public static readonly int AtlasSizeInTiles = 16; // Number of tiles in the texture atlas (4x4 for a 64x64 texture)
    public static readonly int atlasPixelSize = TileSize * AtlasSizeInTiles; // Total size of the texture atlas in pixels
    
    public static Vector2[] GetUVs(BlockType type, int face)
    {
        Vector2 uvOffset;

        switch (type)
        {
            case BlockType.Grass:
                if (face == 4) uvOffset = GetUVOffset(0, 0); // top
                else if (face == 5) uvOffset = GetUVOffset(2, 0); //bottom
                else uvOffset = GetUVOffset(1, 0); // Sides
                break;
            case BlockType.Dirt:
                uvOffset = GetUVOffset(2, 0);
                break;
            case BlockType.Stone:
                uvOffset = GetUVOffset(3, 0);
                break;
            default:
                uvOffset = Vector2.Zero;
                break;
        }

        float tileSize = 1.0f / AtlasSizeInTiles;
        float pixelPadding = 1.0f / atlasPixelSize;  // e.g. 1 / 256 if atlas is 256px
        float padding = pixelPadding * 0.5f;

        return new Vector2[]
        {
            uvOffset + new Vector2(padding, padding),
            uvOffset + new Vector2(padding, tileSize - padding),
            uvOffset + new Vector2(tileSize - padding, tileSize - padding),
            uvOffset + new Vector2(tileSize - padding, padding)
        };
    }

    private static Vector2 GetUVOffset(int x, int y)
    {
        float tileSize = 1.0f / AtlasSizeInTiles;
        return new Vector2(x * tileSize, (AtlasSizeInTiles - 1) - y * tileSize);
    }
}