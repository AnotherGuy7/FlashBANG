using FlashBANG.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FlashBANG.World
{
    public class Tile
    {
        public static Texture2D[] tileTextures;
        public static float[] visibiltyAlphas;

        public int tileType = 0;
        public Vector2 tilePosition;
        public int visiblityID = 0;
        public int generationID = 0;
        public Rectangle tileRect;
        public Lighting.LightData lightData;
        public bool hasLight = false;

        public const int Tile_Void = 0;
        public const int Tile_WoodenFloor = 1;
        public const int Tile_Wall_Bottom = 2;
        public const int Tile_Wall_Top = 3;
        public const int Tile_BorderTop = 4;
        public const int Tile_BorderRight = 5;
        public const int Tile_BorderBottom = 6;
        public const int Tile_BorderLeft = 7;
        public const int Tile_BorderTopLeft = 8;
        public const int Tile_BorderTopRight = 9;
        public const int Tile_BorderBottomLeft = 10;
        public const int Tile_BorderBottomRight = 11;
        public const int Tile_Door = 12;
        public const int Tile_DoorHorizontal = 13;
        public const int Tile_RoomTile_1 = 14;
        public const int Tile_RoomTile_2 = 15;
        public const int Tile_RoomTile_3 = 16;
        public const int Tile_WallCandle = 17;


        public const int Generation_Room = 1;
        public const int Generation_Hallway = 2;
        public const int Generation_Wall = 3;
        public const int Generation_Border = 4;
        public const int Generation_BypassAll = 5;

        public static Tile CreateTile(int tileType, Point point, int visibiltyID = 0, int generationID = 0)
        {
            Tile newTile = new Tile();

            if (point.X < 0 || point.X >= Map.MapWidth || point.Y < 0 || point.Y >= Map.MapHeight)
                return null;
            /*switch (tileType)
            {
                case Tile_Void:
                    break;

                case Tile_WoodenFloor:
                    break;
            }*/
            if (tileType != Tile_RoomTile_1 && tileType != Tile_RoomTile_2 && tileType != Tile_RoomTile_3 && tileType != Tile_WoodenFloor && tileType != Tile_Door && tileType != Tile_DoorHorizontal)
            {
                newTile.tileRect = new Rectangle(new Point(point.X * 16, point.Y * 16), new Point(16, 16));
            }
            if (tileType == Tile_Wall_Bottom || tileType == Tile_Wall_Top)
            {
                generationID = Generation_Wall;
                if (Map.map[point.X, point.Y].tileType != Tile_Void)
                    return Map.map[point.X, point.Y];
            }
            if (generationID == Generation_Hallway || generationID == Generation_Wall)
            {
                if (Map.map[point.X, point.Y].generationID != 0)
                    return Map.map[point.X, point.Y];
            }
            if (generationID == Generation_Border && Map.map[point.X, point.Y].generationID == generationID)
            {
                int otherTileType = Map.map[point.X, point.Y].tileType;
                if (tileType == Tile_BorderTop)
                {
                    if (otherTileType == Tile_BorderLeft)
                        tileType = Tile_BorderTopLeft;
                    if (otherTileType == Tile_BorderRight)
                        tileType = Tile_BorderTopRight;
                }
                else if (tileType == Tile_BorderBottom)
                {
                    if (otherTileType == Tile_BorderLeft)
                        tileType = Tile_BorderBottomLeft;
                    if (otherTileType == Tile_BorderRight)
                        tileType = Tile_BorderBottomRight;
                }
                else if (tileType == Tile_BorderLeft)
                {
                    if (otherTileType == Tile_BorderTop)
                        tileType = Tile_BorderTopLeft;
                    if (otherTileType == Tile_BorderBottom)
                        tileType = Tile_BorderBottomLeft;
                }
                else if (tileType == Tile_BorderRight)
                {
                    if (otherTileType == Tile_BorderTop)
                        tileType = Tile_BorderTopRight;
                    if (otherTileType == Tile_BorderBottom)
                        tileType = Tile_BorderBottomRight;
                }
            }
            /*if (tileType == Tile_WallCandle)
            {
                newTile.hasLight = true;
                newTile.lightData = new Lighting.LightData();
                newTile.lightData.lightStrength = 1f;
            }*/

            newTile.tilePosition = point.ToVector2() * 16f;
            newTile.tileType = tileType;
            newTile.visiblityID = visibiltyID;
            newTile.generationID = generationID;
            return newTile;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (tileType == Tile_Door || tileType == Tile_DoorHorizontal)
                spriteBatch.Draw(tileTextures[tileType], tilePosition, Color.White);
            else
                spriteBatch.Draw(tileTextures[tileType], tilePosition, Color.White * visibiltyAlphas[visiblityID]);
        }
    }
}
