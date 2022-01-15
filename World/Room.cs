using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlashBANG.World
{
    /*public class Room
    {
        public Tile[,] roomTiles;
        public RoomType roomType;

        private const int MaxRoomSize = 26;

        public enum RoomType
        {
            Room,
            Hallway
        }

        public void CreateRoom(RoomType roomType, int width, int height)
        {
            roomTiles = new Tile[MaxRoomSize, MaxRoomSize];
            if (roomType == RoomType.Room)
            {
                for (int x = 0; x < roomTiles.Length; x++)      //Creates all the tile instances
                {
                    for (int y = 0; y < roomTiles.Length; y++)
                    {
                        Vector2 tilePosition = new Vector2(x, y) * 16f;
                        roomTiles[x, y] = Tile.CreateTile(Tile.Tile_Void, tilePosition);
                    }
                }

                int startX = (MaxRoomSize - width + 1) / 2;
                int startY = (MaxRoomSize - height + 1) / 2;
                for (int x = startX; x < width; x++)
                {
                    for (int y = startY; y < height; y++)
                    {
                        int tileType = Tile.Tile_WoodenFloor;
                        Vector2 tilePosition = new Vector2(x, y) * 16f;
                        if (y == 0)
                            tileType = Tile.Tile_Wall_Top;
                        else if (y == 1)
                            tileType = Tile.Tile_Wall_Bottom;

                        roomTiles[x, y] = Tile.CreateTile(tileType, tilePosition);
                    }
                }
            }
            else if (roomType == RoomType.Hallway)
            {
                for (int x = 0; x < roomTiles.Length; x++)      //Creates all the tile instances
                {
                    for (int y = 0; y < roomTiles.Length; y++)
                    {
                        Vector2 tilePosition = new Vector2(x, y) * 16f;
                        roomTiles[x, y] = Tile.CreateTile(Tile.Tile_Void, tilePosition);
                    }
                }
            }
        }
    }*/
}
