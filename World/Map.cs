using FlashBANG.Entities;
using FlashBANG.World.MapObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FlashBANG.World
{
    public class Map
    {
        public static Tile[,] map;
        public static Tile[,] activeMapChunk;
        public static List<MapObject> mapObjects;
        public static List<MapObject> activeMapObjects;
        private static Random worldRand;
        public static Point playerSpawnPoint;

        public const int MapWidth = 250;
        public const int MapHeight = 250;
        private const int MinimumRoomSize = 12;
        private const int MaximumRoomSize = 20;
        private const int MaxConnectedHallways = 2;
        private const int MinimumRoomSpreadDistance = MaximumRoomSize + 4;

        private const int MinRooms = 22;
        private const int MaxRooms = 32;

        public static int ChunkSize = 48;

        public static void UpdateActiveChunk(Vector2 position)
        {
            int chunkSize = 48;     //Make this change later on
            activeMapChunk = new Tile[chunkSize, chunkSize];
            Point pointPosition = new Point((int)position.X / 16, (int)position.Y / 16);
            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    int xCoord = MathHelper.Clamp(pointPosition.X + x - (chunkSize / 2), 0, MapWidth - 1);
                    int yCoord = MathHelper.Clamp(pointPosition.Y + y - (chunkSize / 2), 0, MapHeight - 1);

                    activeMapChunk[x, y] = map[xCoord, yCoord];
                }
            }

            activeMapObjects.Clear();
            for (int i = 0; i < mapObjects.Count; i++)
            {
                if (Vector2.Distance(mapObjects[i].position, position) <= 24 * 16f)
                    activeMapObjects.Add(mapObjects[i]);
            }
        }

        private static void ManageTileVisibility()
        {
            if (Tile.visibiltyAlphas == null)
                return;

            for (int i = 0; i < Tile.visibiltyAlphas.Length; i++)
            {
                if (i == Player.player.tileVisiblityID && Tile.visibiltyAlphas[i] < 1f)
                    Tile.visibiltyAlphas[i] += 0.05f;
                if (Tile.visibiltyAlphas[i] > 0f && i != Player.player.tileVisiblityID)
                    Tile.visibiltyAlphas[i] -= 0.05f;
            }
        }


        //Maps will be 32x32 ROOMS
        //Rooms will be 26x26 TILES

        //Instead, have rooms that you're not in black out.

        public static void UpdateMap()
        {
            ManageTileVisibility();
            foreach (MapObject mapObject in activeMapObjects)
                mapObject.Update();
        }

        public static void DrawMap(SpriteBatch spriteBatch)
        {
            if (activeMapChunk != null)
            foreach (Tile tile in activeMapChunk)
            {
                tile.Draw(spriteBatch);
            }
            foreach (MapObject mapObject in mapObjects)
            {
                mapObject.Draw(spriteBatch);
            }
        }

        public static void CreateWorld()
        {
            worldRand = new Random();
            map = new Tile[MapWidth, MapHeight];
            mapObjects = new List<MapObject>();
            activeMapObjects = new List<MapObject>();
            activeMapChunk = new Tile[48, 48];

            int visibilityID = 1;
            for (int x = 0; x < MapWidth; x++)      //Creates all the tile instances
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    Point tilePoint = new Point(x, y);
                    map[x, y] = Tile.CreateTile(Tile.Tile_Void, tilePoint);
                }
            }

            int amountOfRooms = worldRand.Next(MinRooms, MaxRooms + 1);
            Rectangle[] rooms = new Rectangle[amountOfRooms];
            Point[] roomCenters = new Point[amountOfRooms];
            for (int i = 0; i < amountOfRooms; i++)
            {
                int roomCenterX = worldRand.Next(1 + (MinimumRoomSize / 2), MapWidth - (MinimumRoomSize / 2) - 1);
                int roomCenterY = worldRand.Next(1 + (MaximumRoomSize / 2), MapHeight - (MaximumRoomSize / 2) - 1);
                roomCenters[i] = new Point(roomCenterX, roomCenterY);
                if (i == 0)
                    playerSpawnPoint = roomCenters[i];

                int roomSize = worldRand.Next(MinimumRoomSize, MaximumRoomSize + 1);

                bool stopRoomGeneration = false;
                rooms[i] = new Rectangle(roomCenters[i], new Point(roomSize));
                for (int j = 0; j < i; j++)
                {
                    if (i != j && rooms[i].Intersects(rooms[j]))
                    {
                        stopRoomGeneration = true;
                        break;       //just abort creation, let's not deal with overlaps for the funnies.
                    }

                    if (Vector2.Distance(roomCenters[i].ToVector2(), roomCenters[j].ToVector2()) < MinimumRoomSpreadDistance)
                    {
                        stopRoomGeneration = true;
                        break;
                    }
                }
                if (stopRoomGeneration)
                {
                    roomCenters[i] = Point.Zero;
                    rooms[i] = new Rectangle(0, 0, 1, 1);
                    continue;
                }

                CreateRoom(roomSize, roomCenters[i], visibilityID);
                visibilityID++;
            }
            ScreenshotMap("_Rooms");


            int[] roomHallways = new int[amountOfRooms];        //The amount of hallways a room has connected to it.
            for (int i = 0; i < amountOfRooms; i++)     //Base hallway generation
            {
                //0 for Horizontal
                //1 for Vertical
                int connectionStyle = worldRand.Next(0, 1 + 1);
                connectionStyle = 0;

                int hallwayWidth = 3;
                //if (worldRand.Next(0, 1 + 1) == 0)
                    //hallwayWidth = 5;

                if (roomCenters[i] == Point.Zero)
                    continue;

                if (roomHallways[i] >= MaxConnectedHallways)
                    continue;

                if (connectionStyle == 0)
                {
                    int otherRoom = FindClosestRoom(roomCenters[i], roomCenters, roomHallways);
                    Point closestRoomPoint = roomCenters[otherRoom];
                    Point closestRoomDimensions = new Point(rooms[otherRoom].Width, rooms[otherRoom].Height);

                    int xDist = roomCenters[i].X - closestRoomPoint.X;
                    int direction = 1;
                    if (xDist > 0)
                        direction = -1;
                    xDist = Math.Abs(xDist);
                    xDist -= (int)Math.Ceiling(rooms[i].Width / 2f) - 2;

                    int hallwayStartX = roomCenters[i].X + ((int)Math.Ceiling(rooms[i].Width / 2f) * direction);
                    for (int x = 0; x < xDist; x++)
                    {
                        //for (int y = -2 - (hallwayWidth / 2); y < (hallwayWidth / 2) + 2; y++)        //The hallway will connect with the room's center.
                        for (int y = -3; y < 2; y++)
                        {
                            int tileType = Tile.Tile_WoodenFloor;
                            int tileGenerationID = Tile.Generation_Hallway;
                            Point tilePoint = new Point(hallwayStartX + (x * direction), roomCenters[i].Y + y);
                            if (y == -3)
                                tileType = Tile.Tile_Wall_Top;
                            else if (y == -2)
                                tileType = Tile.Tile_Wall_Bottom;
                            //if (x >= xDist - 3 && roomCenters[i].Y - closestRoomPoint.Y > 0)
                                //tileType = Tile.Tile_WoodenFloor;
                            if (tileType != Tile.Tile_WoodenFloor)
                                tileGenerationID = Tile.Generation_Wall;

                            if (tileType != Tile.Tile_WoodenFloor && map[tilePoint.X, tilePoint.Y].tileType != Tile.Tile_Void)
                                continue;

                            if (CheckForOOB(tilePoint))
                                return;

                            map[tilePoint.X, tilePoint.Y] = Tile.CreateTile(tileType, tilePoint, generationID: tileGenerationID);
                        }
                    }

                    int yDist = roomCenters[i].Y - closestRoomPoint.Y;
                    direction = 1;
                    if (yDist > 0)
                        direction = -1;
                    yDist = Math.Abs(yDist);
                    yDist += 1;
                    //yDist -= closestRoomDimensions.Y / 2;

                    for (int y = 0; y < yDist; y++)
                    {
                        //for (int x = -(hallwayWidth / 2) - 1; x < (hallwayWidth / 2) + 1; x++)        //The hallway will connect with the room's center.
                        for (int x = -1; x < 2; x++)
                        {
                            int tileType = Tile.Tile_WoodenFloor;
                            Point tilePoint = new Point(closestRoomPoint.X + x, roomCenters[i].Y + (y * direction));
                            if (CheckForOOB(tilePoint))
                                return;

                            map[tilePoint.X, tilePoint.Y] = Tile.CreateTile(tileType, tilePoint, generationID: Tile.Generation_Hallway);
                        }
                    }
                    roomHallways[i]++;
                    roomHallways[otherRoom]++;
                }
                /*else
                {
                    int otherRoom = FindClosestRoom(roomCenters[i], roomCenters, roomHallways);
                    Point closestRoomPoint = roomCenters[otherRoom];
                    Point closestRoomDimensions = new Point(rooms[otherRoom].Width, rooms[otherRoom].Height);
                    
                    int yDist = roomCenters[i].Y - closestRoomPoint.Y;
                    int direction = 1;
                    if (yDist > 0)
                        direction = -1;
                    yDist = Math.Abs(yDist);
                    yDist -= closestRoomDimensions.Y / 2;

                    int hallwayStartY = roomCenters[i].Y + ((rooms[i].Height / 2) * direction);
                    for (int y = 0; y < yDist; y++)
                    {
                        //for (int x = -(hallwayWidth / 2); x < (hallwayWidth / 2); x++)        //The hallway will connect with the room's center.
                        for (int x = -1; x < 2; x++)
                        {
                            int tileType = Tile.Tile_WoodenFloor;
                            Point tilePoint = new Point(roomCenters[i].X + x, hallwayStartY + (y * direction));

                            map[tilePoint.X, tilePoint.Y] = Tile.CreateTile(tileType, tilePoint, generationID: Tile.Generation_Hallway);
                        }
                    }

                    int xDist = roomCenters[i].X - closestRoomPoint.X;
                    direction = 1;
                    if (xDist > 0)
                        direction = -1;
                    xDist = Math.Abs(xDist);
                    xDist -= closestRoomDimensions.X / 2;

                    for (int x = 0; x < xDist; x++)
                    {
                        //for (int y = -2 - (hallwayWidth / 2); y < (hallwayWidth / 2) + 2; y++)        //The hallway will connect with the room's center.
                        for (int y = -3; y < 2; y++)
                        {
                            int tileType = Tile.Tile_WoodenFloor;
                            Point tilePoint = new Point(roomCenters[i].X + (x * direction), closestRoomPoint.Y + y);
                            if (y == -3)
                                tileType = Tile.Tile_Wall_Top;
                            else if (y == -2)
                                tileType = Tile.Tile_Wall_Bottom;

                            if (tileType != Tile.Tile_WoodenFloor && map[tilePoint.X, tilePoint.Y].tileType != Tile.Tile_Void)
                                continue;

                            map[tilePoint.X, tilePoint.Y] = Tile.CreateTile(tileType, tilePoint, generationID: Tile.Generation_Hallway);
                        }
                    }
                    roomHallways[i]++;
                    roomHallways[otherRoom]++;
                }*/
            }

            int amountOfExtraHallways = 0;//worldRand.Next(0, 5 + 1);
            for (int i = 0; i < amountOfExtraHallways; i++)     //Extra hallway generation. These types of hallways will just connect to already existing tiles.
            {
                int chosenRoomIndex = worldRand.Next(0, amountOfRooms);
                Point chosenRoom = roomCenters[chosenRoomIndex];
                if (chosenRoom == Point.Zero)
                    continue;

                Point chosenRoomDimensions = new Point(rooms[chosenRoomIndex].Width, rooms[chosenRoomIndex].Height);

                //0 for Horizontal
                //1 for Vertical
                int connectionStyle = worldRand.Next(0, 1 + 1);

                if (connectionStyle == 0)
                {
                    int direction = 1;
                    bool noHallwayConnections = true;
                    Point connectionPoint = Point.Zero;
                    for (int j = 0; j < MapWidth; i++)
                    {
                        direction = -1;
                        int x = chosenRoom.X + ((chosenRoomDimensions.X / 2) * direction);
                        if (x < 0)
                            break;

                        if (map[x, chosenRoom.Y].tileType != Tile.Tile_Void)
                        {
                            noHallwayConnections = false;
                            connectionPoint = new Point(x, chosenRoom.Y);
                            break;
                        }
                    }

                    if (noHallwayConnections)
                    {
                        for (int j = 0; j < MapWidth; i++)
                        {
                            direction = 1;
                            int x = chosenRoom.X + ((chosenRoomDimensions.X / 2) * direction);
                            if (x >= MapWidth)
                                break;

                            if (map[x, chosenRoom.Y].tileType != Tile.Tile_Void)
                            {
                                noHallwayConnections = false;
                                connectionPoint = new Point(x, chosenRoom.Y);
                                break;
                            }
                        }
                    }

                    if (noHallwayConnections)
                        continue;

                    int xDist = Math.Abs(chosenRoom.X - connectionPoint.X);
                    for (int x = 0; x < xDist; x++)
                    {
                        for (int y = -3; y < 2; y++)        //2 walls and 3 floor tiles
                        {
                            int tileType = Tile.Tile_WoodenFloor;
                            Point tilePoint = new Point(chosenRoom.X + ((chosenRoomDimensions.X / 2) * direction), chosenRoom.Y + y);
                            if (y == -3)
                                tileType = Tile.Tile_Wall_Top;
                            else if (y == -2)
                                tileType = Tile.Tile_Wall_Bottom;

                            if (tileType != Tile.Tile_WoodenFloor && map[tilePoint.X, tilePoint.Y].tileType != Tile.Tile_Void)
                                continue;

                            if (CheckForOOB(tilePoint))
                                return;

                            map[tilePoint.X, tilePoint.Y] = Tile.CreateTile(tileType, tilePoint, generationID: Tile.Generation_Hallway);
                        }
                    }
                }
                else
                {
                    int direction = 1;
                    bool noHallwayConnections = true;
                    Point connectionPoint = Point.Zero;
                    for (int j = 0; j < MapWidth; i++)
                    {
                        direction = -1;
                        int y = chosenRoom.Y + ((chosenRoomDimensions.Y / 2) * direction);
                        if (y < 0)
                            break;

                        if (map[chosenRoom.X, y].tileType != Tile.Tile_Void)
                        {
                            noHallwayConnections = false;
                            connectionPoint = new Point(chosenRoom.X, y);
                            break;
                        }
                    }

                    if (noHallwayConnections)
                    {
                        for (int j = 0; j < MapWidth; i++)
                        {
                            direction = 1;
                            int y = chosenRoom.Y + ((chosenRoomDimensions.Y / 2) * direction);
                            if (y >= MapHeight)
                                break;

                            if (map[chosenRoom.X, y].tileType != Tile.Tile_Void)
                            {
                                noHallwayConnections = false;
                                connectionPoint = new Point(chosenRoom.X, y);
                                break;
                            }
                        }
                    }

                    if (noHallwayConnections)
                        continue;

                    int yDist = Math.Abs(chosenRoom.Y - connectionPoint.Y);
                    for (int x = -1; x < 2; x++)
                    {
                        for (int y = 0; y < yDist; y++)        //2 walls and 5 floor tiles
                        {
                            int tileType = Tile.Tile_WoodenFloor;
                            Point tilePoint = new Point(chosenRoom.X + x, chosenRoom.Y + ((chosenRoomDimensions.Y / 2) * direction));
                            if (CheckForOOB(tilePoint))
                                return;

                            map[tilePoint.X, tilePoint.Y] = Tile.CreateTile(tileType, tilePoint, generationID: Tile.Generation_Hallway);
                        }
                    }
                }
            }


            /*int amountOfExtraHallways = worldRand.Next(15, 24 + 1);
            for (int i = 0; i < amountOfExtraHallways; i++)     //Extra hallway generation. These types of hallways will stop generating if it hits an existing tile, which would make interconnecting hallways (probably)
            {
                int chosenRoomIndex = worldRand.Next(0, amountOfRooms);
                int targetRoomIndex = worldRand.Next(0, amountOfRooms);
                Point chosenRoom = roomCenters[chosenRoomIndex];
                Point targetRoom = roomCenters[targetRoomIndex];
                if (chosenRoomIndex == targetRoomIndex || chosenRoom == Point.Zero || targetRoom == Point.Zero)
                    continue;

                Point chosenRoomDimensions = new Point(rooms[chosenRoomIndex].Width, rooms[chosenRoomIndex].Height);
                Point targetRoomDimensions = new Point(rooms[targetRoomIndex].Width, rooms[targetRoomIndex].Height);

                while (Math.Abs(chosenRoom.X - targetRoom.X) < MaximumRoomSize || Math.Abs(chosenRoom.Y - targetRoom.Y) < MaximumRoomSize || targetRoom == Point.Zero)
                {
                    targetRoomIndex = worldRand.Next(0, amountOfRooms);
                    targetRoom = roomCenters[targetRoomIndex];
                    targetRoomDimensions = new Point(rooms[targetRoomIndex].Width, rooms[targetRoomIndex].Height);
                }

                //0 for Horizontal
                //1 for Vertical
                int connectionStyle = worldRand.Next(0, 1 + 1);

                int hallwayWidth = 3;
                if (worldRand.Next(0, 1 + 1) == 0)
                    hallwayWidth = 5;

                if (connectionStyle == 0)
                {
                    int xDist = chosenRoom.X - targetRoom.X;
                    int direction = 1;
                    if (xDist > 0)
                        direction = -1;
                    xDist = Math.Abs(xDist);
                    xDist -= chosenRoomDimensions.X + (targetRoomDimensions.X / 2);

                    bool stopHallwayGeneration = false;
                    int hallwayStartX = chosenRoom.X + (rooms[i].Width * direction);
                    for (int x = 0; x < xDist; x++)
                    {
                        for (int y = -2 - (hallwayWidth / 2); y < (hallwayWidth / 2) + 2; y++)        //The hallway will connect with the room's center.
                        {
                            if (map[hallwayStartX + (x * direction), chosenRoom.Y + y].tileType != Tile.Tile_Void)
                            {
                                stopHallwayGeneration = true;
                                break;
                            }

                            int tileType = Tile.Tile_WoodenFloor;
                            Point tilePoint = new Point(hallwayStartX + (x * direction), chosenRoom.Y + y);
                            Vector2 tilePoint = tilePoint.ToVector2() * 16f;
                            if (y == -(hallwayWidth / 2) - 2)
                                tileType = Tile.Tile_Wall_Top;
                            else if (y == -(hallwayWidth / 2) - 1)
                                tileType = Tile.Tile_Wall_Bottom;

                            map[tilePoint.X, tilePoint.Y] = Tile.CreateTile(tileType, tilePoint);
                        }
                        if (stopHallwayGeneration)
                            break;
                    }

                    if (stopHallwayGeneration)
                        continue;

                    int yDist = chosenRoom.Y - targetRoom.Y;
                    direction = 1;
                    if (yDist > 0)
                        direction = -1;
                    yDist = Math.Abs(yDist);
                    //yDist -= chosenRoomDimensions.Y + targetRoomDimensions.Y;

                    for (int y = 0; y < yDist; y++)
                    {
                        for (int x = -(hallwayWidth / 2); x < (hallwayWidth / 2); x++)        //The hallway will connect with the room's center.
                        {
                            if (map[targetRoom.X + x, chosenRoom.Y + (y * direction)].tileType != Tile.Tile_Void)
                            {
                                stopHallwayGeneration = true;
                                break;
                            }

                            int tileType = Tile.Tile_WoodenFloor;
                            Point tilePoint = new Point(targetRoom.X + x, chosenRoom.Y + (y * direction));
                            Vector2 tilePoint = tilePoint.ToVector2() * 16f;

                            map[tilePoint.X, tilePoint.Y] = Tile.CreateTile(tileType, tilePoint);
                        }
                        if (stopHallwayGeneration)
                            break;
                    }
                }
                else
                {
                    int yDist = chosenRoom.Y - targetRoom.Y;
                    int direction = 1;
                    if (yDist > 0)
                        direction = -1;
                    yDist = Math.Abs(yDist);
                    //yDist -= chosenRoomDimensions.Y + targetRoomDimensions.Y;

                    bool stopHallwayGeneration = false;
                    int hallwayStartY = chosenRoom.Y + (rooms[i].Height * direction);
                    for (int y = 0; y < yDist; y++)
                    {
                        for (int x = -(hallwayWidth / 2); x < (hallwayWidth / 2); x++)        //The hallway will connect with the room's center.
                        {
                            if (map[chosenRoom.X + x, hallwayStartY + y * direction].tileType != Tile.Tile_Void)
                            {
                                stopHallwayGeneration = true;
                                break;
                            }

                            int tileType = Tile.Tile_WoodenFloor;
                            Point tilePoint = new Point(chosenRoom.X + x, hallwayStartY + (y * direction));
                            Vector2 tilePoint = tilePoint.ToVector2() * 16f;

                            map[tilePoint.X, tilePoint.Y] = Tile.CreateTile(tileType, tilePoint);
                        }
                        if (stopHallwayGeneration)
                            break;
                    }

                    if (stopHallwayGeneration)
                        continue;

                    int xDist = chosenRoom.X - targetRoom.X;
                    direction = 1;
                    if (xDist > 0)
                        direction = -1;
                    xDist = Math.Abs(xDist);
                    xDist -= chosenRoomDimensions.X + (targetRoomDimensions.X / 2);

                    for (int x = 0; x < xDist; x++)
                    {
                        for (int y = -2 - (hallwayWidth / 2); y < (hallwayWidth / 2) + 2; y++)        //The hallway will connect with the room's center.
                        {
                            if (map[chosenRoom.X + (x * direction), targetRoom.Y + y].tileType != Tile.Tile_Void)
                            {
                                stopHallwayGeneration = true;
                                break;
                            }

                            int tileType = Tile.Tile_WoodenFloor;
                            Point tilePoint = new Point(chosenRoom.X + (x * direction), targetRoom.Y + y);
                            Vector2 tilePoint = tilePoint.ToVector2() * 16f;
                            if (y == -(hallwayWidth / 2) - 2)
                                tileType = Tile.Tile_Wall_Top;
                            else if (y == -(hallwayWidth / 2) - 1)
                                tileType = Tile.Tile_Wall_Bottom;

                            map[tilePoint.X, tilePoint.Y] = Tile.CreateTile(tileType, tilePoint);
                        }
                        if (stopHallwayGeneration)
                            break;
                    }
                }
                //roomHallways[chosenRoomIndex]++;
                //roomHallways[targetRoomIndex]++;
            }*/
            CreateMapObjects();
            CleanUpMap();
            ScreenshotMap("_Finished");
            Tile.visibiltyAlphas = new float[visibilityID];
            UpdateActiveChunk(playerSpawnPoint.ToVector2() * 16f);
        }

        private static bool CheckForOOB(Point point)
        {
            bool outOfBounds = point.X < 0 || point.X >= MapWidth || point.Y < 0 || point.Y >= MapHeight;
            if (outOfBounds)
                CreateWorld();

            return outOfBounds;
        }

        public static void CreateRoom(int size, Point roomCenter, int visibilityID)
        {
            Point roomTopLeft = new Point(roomCenter.X - (size / 2), roomCenter.Y - (size / 2));
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    int tileType = worldRand.Next(Tile.Tile_RoomTile_1, Tile.Tile_RoomTile_3 + 1);
                    Point tilePoint = new Point(roomTopLeft.X + x, roomTopLeft.Y + y);
                    if (y == 0)
                        tileType = Tile.Tile_Wall_Top;
                    else if (y == 1)
                        tileType = Tile.Tile_Wall_Bottom;

                    if (CheckForOOB(tilePoint))
                        return;

                    map[tilePoint.X, tilePoint.Y] = Tile.CreateTile(tileType, tilePoint, visibilityID, Tile.Generation_Room);
                }
            }
        }

        /// <summary>
        /// Returns the index of the closest room.
        /// </summary>
        /// <returns></returns>
        public static int FindClosestRoom(Point roomCenter, Point[] roomCenters, int[] roomHallwayCounts)
        {
            int closestRoomIndex = 0;

            int closestDistance = MapWidth;
            for (int i = 0; i < roomCenters.Length; i++)
            {
                if (roomCenters[i] == Point.Zero || roomCenters[i] == roomCenter)
                    continue;

                //if (Math.Abs(roomCenter.X - roomCenters[i].X) < MaximumRoomSize / 2 || Math.Abs(roomCenter.Y - roomCenters[i].Y) < MaximumRoomSize / 2)
                    //continue;

                int roomDist = (int)Vector2.Distance(roomCenters[i].ToVector2(), roomCenters[i].ToVector2());
                int amountOfHallwaysConnected = roomHallwayCounts[i];
                if (amountOfHallwaysConnected == 0 && roomDist < closestDistance)
                {
                    closestDistance = roomDist;
                    closestRoomIndex = i;
                }
            }
            
            return closestRoomIndex;
        }

        public static void CreateMapObjects()
        {
            int amountOfMetalCreated = 0;
            while (amountOfMetalCreated < 82)
            {
                int randX = worldRand.Next(0, MapWidth);
                int randY = worldRand.Next(0, MapWidth);

                if (map[randX, randY].generationID == Tile.Generation_Room)
                {
                    Vector2 metalPos = new Vector2(randX, randY) * 16f;
                    mapObjects.Add(Metal.CreateMetal(metalPos, map[randX, randY].visiblityID));
                    amountOfMetalCreated++;
                }
            }

            int amountOfBulbsCreated = 0;
            while (amountOfBulbsCreated < 56)
            {
                int randX = worldRand.Next(0, MapWidth);
                int randY = worldRand.Next(0, MapWidth);

                if (map[randX, randY].generationID == Tile.Generation_Room)
                {
                    Vector2 bulbPos = new Vector2(randX, randY) * 16f;
                    mapObjects.Add(Bulb.CreateBulb(bulbPos, map[randX, randY].visiblityID));
                    amountOfBulbsCreated++;
                }
            }

            /*int amountOfMapBoxes = worldRand.Next(50, 85 + 1);
            while (amountOfMapBoxes > 0)
            {
                int randX = worldRand.Next(0, MapWidth);
                int randY = worldRand.Next(0, MapWidth);

                if (map[randX, randY].generationID == Tile.Generation_Room)
                {
                    Vector2 boxPos = new Vector2(randX, randY) * 16f;
                    mapObjects.Add(Box.CreateBox(boxPos));
                    amountOfMapBoxes--;
                }
            }*/
        }

        public static void CleanUpMap()
        {
            EncloseRooms();

            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    Point tilePoint = new Point(x, y);
                    int currentTileType = map[tilePoint.X, tilePoint.Y].tileType;
                    int currentGenerationID = map[tilePoint.X, tilePoint.Y].generationID;

                    int newTileType = -1;
                    int newGenerationID = -1;

                    if (currentTileType == Tile.Tile_WoodenFloor)
                    {
                        if (CheckTileAbove(tilePoint).tileType == Tile.Tile_Void)
                        {
                            tilePoint.Y -= 1;
                            newTileType = Tile.Tile_BorderBottom;
                            newGenerationID = Tile.Generation_Border;
                        }
                        else if (CheckTileUnder(tilePoint).tileType == Tile.Tile_Void)
                        {
                            tilePoint.Y += 1;
                            newTileType = Tile.Tile_BorderTop;
                            newGenerationID = Tile.Generation_Border;
                        }
                        else if (CheckTileLeft(tilePoint).tileType == Tile.Tile_Void)
                        {
                            tilePoint.X -= 1;
                            newTileType = Tile.Tile_BorderRight;
                            newGenerationID = Tile.Generation_Border;
                        }
                        else if (CheckTileRight(tilePoint).tileType == Tile.Tile_Void)
                        {
                            tilePoint.X += 1;
                            newTileType = Tile.Tile_BorderLeft;
                            newGenerationID = Tile.Generation_Border;
                        }
                    }
                    if (currentGenerationID == Tile.Generation_Wall || currentTileType == Tile.Tile_Wall_Top || currentTileType == Tile.Tile_Wall_Bottom)
                    {
                        if (CheckTileAbove(tilePoint).tileType == Tile.Tile_WoodenFloor)
                        {
                            newTileType = Tile.Tile_WoodenFloor;
                            newGenerationID = Tile.Generation_BypassAll;
                            if (CheckForOOB(tilePoint))
                                return;

                            map[tilePoint.X, tilePoint.Y + 1] = Tile.CreateTile(newTileType, tilePoint + new Point(0, 1), generationID: Tile.Generation_BypassAll);
                        }
                    }
                    /*if (currentTileType == Tile.Tile_WoodenFloor)
                    {
                        if (CheckTileUnder(tilePoint).tileType == Tile.Tile_Wall_Top)
                        {
                            newTileType = Tile.Tile_WoodenFloor;
                            newGenerationID = Tile.Generation_Hallway;
                            map[tilePoint.X, tilePoint.Y + 1] = Tile.CreateTile(newTileType, tilePoint + new Point(0, 1), generationID: Tile.Generation_BypassAll);
                            map[tilePoint.X, tilePoint.Y + 1] = Tile.CreateTile(newTileType, tilePoint + new Point(0, 1), generationID: Tile.Generation_BypassAll);
                        }
                    }*/

                    if (newTileType != -1)
                        map[tilePoint.X, tilePoint.Y] = Tile.CreateTile(newTileType, tilePoint, 0, newGenerationID);
                }
            }
        }

        private static void EncloseRooms()
        {
            /*for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    Point tilePoint = new Point(x, y);
                    int currentTileType = map[tilePoint.X, tilePoint.Y].tileType;

                    if (map[tilePoint.X, tilePoint.Y].generationID == Tile.Generation_Room && map[tilePoint.X, tilePoint.Y].tileType == Tile.Tile_WoodenFloor)
                    {
                        if (CheckTileAbove(tilePoint) == Tile.Tile_WoodenFloor && map[tilePoint.X, tilePoint.Y - 1].generationID == Tile.Generation_Hallway)     //If it's the center of a hallway
                        {
                            if (CheckTileAbove(new Point(tilePoint.X - 1, tilePoint.Y)) == Tile.Tile_WoodenFloor && CheckTileAbove(new Point(tilePoint.X + 1, tilePoint.Y)) == Tile.Tile_WoodenFloor)
                            {
                                for (int i = -1; i < 1; i++)
                                {
                                    for (int j = -2; j < 0; j++)
                                    {
                                        Point targetPoint = new Point(x + i, y + j);
                                        int newTileType = -1;

                                        if (j == -2)        //This sucks.
                                        {
                                            if (i == -1)
                                                newTileType = Tile.Tile_BorderTopRight;
                                            if (i == 1)
                                                newTileType = Tile.Tile_BorderTopLeft;
                                        }
                                        if (j == -1)
                                        {
                                            if (i == -1)
                                                newTileType = Tile.Tile_BorderBottomRight;
                                            if (i == 1)
                                                newTileType = Tile.Tile_BorderBottomLeft;
                                        }
                                        if (i == 0 && j == -1)
                                            newTileType = Tile.Tile_DoorHorizontal;


                                        if (newTileType != -1)
                                            map[targetPoint.X, targetPoint.Y] = Tile.CreateTile(newTileType, targetPoint);
                                    }
                                }
                            }
                        }
                        if (CheckTileUnder(tilePoint) == Tile.Tile_WoodenFloor && map[tilePoint.X, tilePoint.Y + 1].generationID == Tile.Generation_Hallway)     //If it's the center of a hallway
                        {
                            if (CheckTileUnder(new Point(tilePoint.X - 1, tilePoint.Y)) == Tile.Tile_WoodenFloor && CheckTileUnder(new Point(tilePoint.X + 1, tilePoint.Y)) == Tile.Tile_WoodenFloor)
                            {
                                for (int i = -1; i < 1; i++)
                                {
                                    for (int j = 1; j < 3; j++)
                                    {
                                        Point targetPoint = new Point(x + i, y + j);
                                        int newTileType = -1;

                                        if (j == 1)        //This sucks.
                                        {
                                            if (i == -1)
                                                newTileType = Tile.Tile_BorderTopRight;
                                            if (i == 1)
                                                newTileType = Tile.Tile_BorderTopLeft;
                                        }
                                        if (j == 2)
                                        {
                                            if (i == -1)
                                                newTileType = Tile.Tile_BorderBottomRight;
                                            if (i == 1)
                                                newTileType = Tile.Tile_BorderBottomLeft;
                                        }
                                        if (i == 0 && j == 1)
                                            newTileType = Tile.Tile_DoorHorizontal;

                                        if (newTileType != -1)
                                            map[targetPoint.X, targetPoint.Y] = Tile.CreateTile(newTileType, targetPoint);
                                    }
                                }
                            }
                        }
                        if (CheckTileLeft(tilePoint) == Tile.Tile_WoodenFloor && map[tilePoint.X - 1, tilePoint.Y].generationID == Tile.Generation_Hallway)     //If it's the center of a hallway
                        {
                            if (CheckTileLeft(new Point(tilePoint.X, tilePoint.Y - 1)) == Tile.Tile_WoodenFloor && CheckTileLeft(new Point(tilePoint.X, tilePoint.Y + 1)) == Tile.Tile_WoodenFloor)
                            {
                                for (int i = -2; i < 0; i++)
                                {
                                    for (int j = -1; j < 1; j++)
                                    {
                                        Point targetPoint = new Point(x + i, y + j);
                                        int newTileType = -1;

                                        if (i == -2)        //This sucks.
                                        {
                                            if (j == -1)
                                                newTileType = Tile.Tile_BorderBottomLeft;
                                            if (j == 1)
                                                newTileType = Tile.Tile_BorderTopLeft;
                                        }
                                        if (i == -1)
                                        {
                                            if (j == -1)
                                                newTileType = Tile.Tile_BorderBottomRight;
                                            if (j == 1)
                                                newTileType = Tile.Tile_BorderBottomRight;
                                        }
                                        if (j == 0 && i == -1)
                                            newTileType = Tile.Tile_Door;

                                        if (newTileType != -1)
                                            map[targetPoint.X, targetPoint.Y] = Tile.CreateTile(newTileType, targetPoint);
                                    }
                                }
                            }
                        }
                        if (CheckTileRight(tilePoint) == Tile.Tile_WoodenFloor && map[tilePoint.X + 1, tilePoint.Y].generationID == Tile.Generation_Hallway)     //If it's the center of a hallway
                        {
                            if (CheckTileRight(new Point(tilePoint.X, tilePoint.Y - 1)) == Tile.Tile_WoodenFloor && CheckTileRight(new Point(tilePoint.X, tilePoint.Y + 1)) == Tile.Tile_WoodenFloor)
                            {
                                for (int i = 1; i < 3; i++)
                                {
                                    for (int j = -1; j < 1; j++)
                                    {
                                        Point targetPoint = new Point(x + i, y + j);
                                        int newTileType = -1;

                                        if (i == 1)        //This sucks.
                                        {
                                            if (j == -1)
                                                newTileType = Tile.Tile_BorderBottomLeft;
                                            if (j == 1)
                                                newTileType = Tile.Tile_BorderTopLeft;
                                        }
                                        if (i == 2)
                                        {
                                            if (j == -1)
                                                newTileType = Tile.Tile_BorderBottomRight;
                                            if (j == 1)
                                                newTileType = Tile.Tile_BorderBottomRight;
                                        }
                                        if (j == 0 && i == 1)
                                            newTileType = Tile.Tile_Door;

                                        if (newTileType != -1)
                                            map[targetPoint.X, targetPoint.Y] = Tile.CreateTile(newTileType, targetPoint);
                                    }
                                }
                            }
                        }
                    }
                }
            }*/

            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    Point tilePoint = new Point(x, y);
                    int currentTileType = map[tilePoint.X, tilePoint.Y].tileType;
                    int currentGenerationID = map[tilePoint.X, tilePoint.Y].generationID;

                    if (currentGenerationID == Tile.Generation_Hallway)
                    {
                        if (CheckTileAbove(tilePoint).generationID == Tile.Generation_Room)
                        {
                            if (CheckTileLeft(tilePoint).tileType == Tile.Tile_WoodenFloor && CheckTileRight(tilePoint).tileType == Tile.Tile_WoodenFloor)
                            {
                                map[tilePoint.X, tilePoint.Y] = Tile.CreateTile(Tile.Tile_DoorHorizontal, tilePoint, CheckTileAbove(tilePoint).visiblityID);
                                map[tilePoint.X - 1, tilePoint.Y] = Tile.CreateTile(Tile.Tile_BorderTopRight, tilePoint + new Point(-1, 0), CheckTileAbove(tilePoint).visiblityID);
                                map[tilePoint.X + 1, tilePoint.Y] = Tile.CreateTile(Tile.Tile_BorderTopLeft, tilePoint + new Point(1, 0), CheckTileAbove(tilePoint).visiblityID);
                            }
                        }
                        if (CheckTileUnder(tilePoint).generationID == Tile.Generation_Wall && CheckTileUnder(tilePoint + new Point(0, 2)).generationID == Tile.Generation_Room)
                        {
                            if (CheckTileLeft(tilePoint).tileType == Tile.Tile_WoodenFloor && CheckTileRight(tilePoint).tileType == Tile.Tile_WoodenFloor)
                            {
                                for (int i = -1; i < 2; i++)
                                {
                                    Point newTilePoint = new Point(tilePoint.X + i, tilePoint.Y + 1);
                                    map[newTilePoint.X, newTilePoint.Y] = Tile.CreateTile(Tile.Tile_WoodenFloor, newTilePoint);
                                }


                                map[tilePoint.X, tilePoint.Y + 2] = Tile.CreateTile(Tile.Tile_DoorHorizontal, tilePoint + new Point(0, 2), CheckTileUnder(tilePoint + new Point(0, 2)).visiblityID);
                                map[tilePoint.X - 1, tilePoint.Y + 2] = Tile.CreateTile(Tile.Tile_BorderBottomRight, tilePoint + new Point(-1, 2), CheckTileUnder(tilePoint + new Point(0, 2)).visiblityID);
                                map[tilePoint.X + 1, tilePoint.Y + 2] = Tile.CreateTile(Tile.Tile_BorderBottomLeft, tilePoint + new Point(1, 2), CheckTileUnder(tilePoint + new Point(0, 2)).visiblityID);
                            }
                        }
                        if (CheckTileRight(tilePoint).generationID == Tile.Generation_Room)
                        {
                            if (CheckTileAbove(tilePoint).tileType == Tile.Tile_WoodenFloor && CheckTileUnder(tilePoint).tileType == Tile.Tile_WoodenFloor)
                            {
                                map[tilePoint.X, tilePoint.Y] = Tile.CreateTile(Tile.Tile_Door, tilePoint, CheckTileRight(tilePoint).visiblityID);
                                map[tilePoint.X, tilePoint.Y - 1] = Tile.CreateTile(Tile.Tile_BorderBottomRight, tilePoint + new Point(0, -1), CheckTileRight(tilePoint).visiblityID);
                                map[tilePoint.X, tilePoint.Y + 1] = Tile.CreateTile(Tile.Tile_BorderTopRight, tilePoint + new Point(0, 1), CheckTileRight(tilePoint).visiblityID);
                            }
                        }
                        if (CheckTileLeft(tilePoint).generationID == Tile.Generation_Room)
                        {
                            if (CheckTileAbove(tilePoint).tileType == Tile.Tile_WoodenFloor && CheckTileUnder(tilePoint).tileType == Tile.Tile_WoodenFloor)
                            {
                                map[tilePoint.X, tilePoint.Y] = Tile.CreateTile(Tile.Tile_Door, tilePoint, CheckTileLeft(tilePoint).visiblityID);
                                map[tilePoint.X, tilePoint.Y - 1] = Tile.CreateTile(Tile.Tile_BorderBottomLeft, tilePoint + new Point(0, -1), CheckTileLeft(tilePoint).visiblityID);
                                map[tilePoint.X, tilePoint.Y + 1] = Tile.CreateTile(Tile.Tile_BorderTopLeft, tilePoint + new Point(0, 1), CheckTileLeft(tilePoint).visiblityID);
                            }
                        }
                    }
                    /*if (currentTileType == Tile.Tile_Wall_Top)
                        if (worldRand.Next(0, 35 + 1) == 0)
                            map[tilePoint.X, tilePoint.Y] = Tile.CreateTile(Tile.Tile_WallCandle, tilePoint, map[tilePoint.X, tilePoint.Y].visiblityID, map[tilePoint.X, tilePoint.Y].generationID);*/
                }
            }
        }

        private static Tile CheckTileAbove(Point coordinates)
        {
            if (coordinates.Y - 1 < 0)
                return null;

            return map[coordinates.X, coordinates.Y - 1];
        }

        private static Tile CheckTileUnder(Point coordinates)
        {
            if (coordinates.Y + 1 >= MapHeight)
                return null;

            return map[coordinates.X, coordinates.Y + 1];
        }

        private static Tile CheckTileLeft(Point coordinates)
        {
            if (coordinates.X - 1 < 0)
                return null;

            return map[coordinates.X - 1, coordinates.Y];
        }

        private static Tile CheckTileRight(Point coordinates)
        {
            if (coordinates.X + 1 >= MapWidth)
                return null;

            return map[coordinates.X + 1, coordinates.Y];
        }


        public static void ScreenshotMap(string fileNameEnd)
        {
            return;

            Texture2D dungeonResultOverviewTexture = new Texture2D(Main._graphics.GraphicsDevice, MapWidth, MapHeight);
            Color[] resultData = new Color[MapWidth * MapHeight];
            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    int tileType = map[x, y].tileType;
                    Color pixelColor = Color.White;
                    if (tileType == Tile.Tile_Void)
                    {
                        pixelColor = Color.Black;
                    }
                    else if (tileType == Tile.Tile_WoodenFloor)
                    {
                        pixelColor = Color.SaddleBrown;
                    }
                    else if (tileType == Tile.Tile_Wall_Bottom)
                    {
                        pixelColor = Color.Pink;
                    }
                    else if (tileType == Tile.Tile_Wall_Top)
                    {
                        pixelColor = Color.LightPink;
                    }
                    else if (tileType == Tile.Tile_Door)
                    {
                        pixelColor = Color.Red;
                    }
                    else if (tileType == Tile.Tile_BorderRight || tileType == Tile.Tile_BorderLeft || tileType == Tile.Tile_BorderTop || tileType == Tile.Tile_BorderBottom)
                    {
                        pixelColor = Color.DeepPink;
                    }
                    else if (tileType == Tile.Tile_BorderTopLeft || tileType == Tile.Tile_BorderTopRight || tileType == Tile.Tile_BorderBottomLeft || tileType == Tile.Tile_BorderBottomRight)
                    {
                        pixelColor = Color.LightPink;
                    }
                    else if (tileType == Tile.Tile_RoomTile_1 || tileType == Tile.Tile_RoomTile_2 || tileType == Tile.Tile_RoomTile_3)
                    {
                        pixelColor = Color.LightSlateGray;
                    }

                    if (new Point(x, y) == playerSpawnPoint)
                        pixelColor = Color.Yellow;

                    resultData[x + y * MapWidth] = pixelColor;
                }
            }
            dungeonResultOverviewTexture.SetData(resultData);
            SaveTexture2DAsPNG(dungeonResultOverviewTexture, fileNameEnd);
            dungeonResultOverviewTexture.Dispose();
        }

        public static void SaveTexture2DAsPNG(Texture2D texture, string pathAddition)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\FlashBANG\\MapImage_" + pathAddition + ".png";
            if (!File.Exists(path))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }

            FileStream fileStream = File.OpenWrite(path);
            StreamWriter writer = new StreamWriter(fileStream);
            texture.SaveAsPng(fileStream, texture.Width, texture.Height);
            writer.Close();
            fileStream.Close();
            texture.Dispose();
        }
    }
}
