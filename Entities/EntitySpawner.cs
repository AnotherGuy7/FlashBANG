using FlashBANG.Entities.Enemies;
using FlashBANG.Utilities;
using FlashBANG.World;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlashBANG.Entities
{
    public class EntitySpawner
    {
        public const int AmountOfEnemies = 1;
        public const int Enemy_ShadowCube = 0;
        public const int Enemy_ShadowBall = 1;
        public const int Enemy_ShadowMan = 2;

        private static readonly int[] Stage1Enemies = new int[2] { Enemy_ShadowCube, Enemy_ShadowBall };
        private static readonly int[] Stage2Enemies = new int[2] { Enemy_ShadowCube, Enemy_ShadowBall };
        private static readonly int[] Stage3Enemies = new int[3] { Enemy_ShadowCube, Enemy_ShadowBall, Enemy_ShadowMan };
        private static readonly int[] Stage4Enemies = new int[3] { Enemy_ShadowCube, Enemy_ShadowBall, Enemy_ShadowMan };
        private static readonly int[] Stage5Enemies = new int[3] { Enemy_ShadowCube, Enemy_ShadowBall, Enemy_ShadowMan };

        private static int spawnCooldownTimer = 0;
        private static Vector2 previousPlayerPosition;

        public static void SpawnEnemy(int enemyType, Vector2 position)
        {
            CollisionBody newInstance = null;
            switch (enemyType)
            {
                case Enemy_ShadowCube:
                    newInstance = ShadowCube.NewShadowCube(position);
                    break;
                case Enemy_ShadowBall:
                    newInstance = ShadowBall.NewShadowBall(position);
                    break;
                case Enemy_ShadowMan:
                    newInstance = ShadowMan.NewShadowMan(position);
                    break;
            }
            Main.activeEntities.Add(newInstance);
        }

        public static void UpdateSpawner()
        {
            if (spawnCooldownTimer > 0)
            {
                spawnCooldownTimer--;
                return;
            }
            if (Main.gameLost)
                return;

            int highestChance = 50;
            int cooldownTime = (1 * 60) + 30;
            if (Main.gameStage == 5)
            {
                highestChance = 3;
                cooldownTime = 5;
            }

            if (Main.random.Next(0, highestChance + 1) == 0)
            {
                Vector2 playerPosition = Player.player.position;
                Vector2 chosenPosition = Vector2.Zero;
                while (chosenPosition == Vector2.Zero)
                {
                    /*int screenWidthMin = Screen.halfScreenWidth;
                    int screenHeightMin = Screen.halfScreenHeight;
                    Vector2 randomPos = new Vector2(Main.random.Next(-screenWidthMin - 32, -screenWidthMin), Main.random.Next(-screenHeightMin - 32, -screenHeightMin));
                    if (Main.random.Next(0, 1) == 0)
                        randomPos.X = Main.random.Next(screenWidthMin, screenWidthMin + 32);
                    if (Main.random.Next(0, 1) == 0)
                        randomPos.Y = Main.random.Next(screenHeightMin, screenHeightMin + 32);

                    Vector2 checkPos = playerPosition + randomPos;
                    if (checkPos.X < 0 || checkPos.X >= Map.MapWidth * 16 || checkPos.Y < 0 || checkPos.Y >= Map.MapHeight * 16)
                        continue;

                    chosenPosition = checkPos;*/

                    Vector2 playerDirection = playerPosition - previousPlayerPosition;
                    playerDirection.Normalize();
                    playerDirection *= Main.random.Next(Screen.resolutionWidth, Screen.resolutionWidth + 32) / 3f;
                    playerDirection += playerPosition;

                    bool spawnFound = false;
                    for (int x = -5; x < 5; x++)
                    {
                        for (int y = -5; y < 5; y++)
                        {
                            Point checkPoint = new Point((int)(playerDirection.X / 16) + x, (int)(playerDirection.Y / 16) + y);
                            if (checkPoint.X < 0 || checkPoint.X >= Map.MapWidth || checkPoint.Y < 0 || checkPoint.Y >= Map.MapHeight)
                                continue;

                            if (Map.map[checkPoint.X, checkPoint.Y].tileType == Tile.Tile_WoodenFloor || Map.map[checkPoint.X, checkPoint.Y].tileType == Tile.Tile_RoomTile_1)
                            {
                                if (Vector2.Distance(playerPosition, checkPoint.ToVector2() * 16f) < Screen.resolutionWidth / 3)
                                    continue;

                                if (Main.random.Next(0, 100 + 1) >= 60)     //For variation, most enemies spawn completely on the left side of any place.
                                    continue;

                                spawnFound = true;
                                chosenPosition = checkPoint.ToVector2() * 16f;
                            }
                        }
                        if (spawnFound)
                            break;
                    }
                    if (!spawnFound)
                        return;
                }
                previousPlayerPosition = playerPosition;
                /*int randDirX = Main.random.Next(0, 1 + 1);
                if (randDirX == 0)
                    randDirX = -1;

                int randDirY = Main.random.Next(0, 1 + 1);
                if (randDirY == 0)
                    randDirY = -1;

                bool spawnFailed = false;
                while (Map.map[(int)chosenPosition.X / 16, (int)chosenPosition.Y / 16].tileRect != null)
                {
                    chosenPosition.X += randDirX * 16f;
                    chosenPosition.Y += randDirY * 16f;
                    if (chosenPosition.X < 0 || chosenPosition.X >= Map.MapWidth * 16 || chosenPosition.Y < 0 || chosenPosition.Y >= Map.MapHeight * 16)
                    {
                        spawnFailed = true;
                        Lighting.QueueLightData(Lighting.Texture_LightRing, Player.player.position, 8f);
                        break;
                    }
                }
                if (spawnFailed)        //Always failed by the way
                    return;*/

                    /*Point playerPoint = (Player.player.position / 16f).ToPoint();
                    Point chosenPoint = Point.Zero;
                    while (chosenPoint == Point.Zero)
                    {
                        int screenWidthMin = (Screen.resolutionWidth / 16) / 6;
                        int screenHeightMin = (Screen.resolutionHeight / 16) / 6;
                        Point randomPoint = new Point(Main.random.Next(-screenWidthMin - 6, -screenWidthMin), Main.random.Next(-screenHeightMin - 6, -screenHeightMin));
                        if (Main.random.Next(0, 1) == 0)
                            randomPoint.X = Main.random.Next(screenWidthMin, screenWidthMin + 6);
                        if (Main.random.Next(0, 1) == 0)
                            randomPoint.Y = Main.random.Next(screenHeightMin, screenHeightMin + 6);

                        Point checkPoint = playerPoint + randomPoint;
                        if (checkPoint.X < 0 || checkPoint.X >= Map.MapWidth || checkPoint.Y < 0 || checkPoint.Y >= Map.MapHeight)
                            continue;

                        chosenPoint = checkPoint;
                    }
                     if (chosenPoint.X < 0 || chosenPoint.X >= Map.MapWidth || chosenPoint.Y < 0 || chosenPoint.Y >= Map.MapHeight)
                         return;

                     int randDirX = Main.random.Next(0, 1 + 1);
                    if (randDirX == 0)
                        randDirX = -1;

                    int randDirY = Main.random.Next(0, 1 + 1);
                    if (randDirY == 0)
                        randDirY = -1;

                    while (Map.map[chosenPoint.X, chosenPoint.Y].tileRect != null)
                    {
                         if (chosenPoint.X - 1 < 0 || chosenPoint.X + 1 >= Map.MapWidth || chosenPoint.Y - 1 < 0 || chosenPoint.Y + 1 >= Map.MapHeight)
                             break;

                         chosenPoint.X += randDirX;
                        chosenPoint.Y += randDirY;
                    }*/

                int enemyType = 0;
                /*if (Main.gameStage == 1)
                    enemyType = Stage1Enemies[Main.random.Next(0, Stage1Enemies.Length)];
                else if (Main.gameStage == 2)
                    enemyType = Stage1Enemies[Main.random.Next(0, Stage2Enemies.Length)];
                else if (Main.gameStage == 3)
                    enemyType = Stage1Enemies[Main.random.Next(0, Stage3Enemies.Length)];
                else if (Main.gameStage == 4)
                    enemyType = Stage1Enemies[Main.random.Next(0, Stage4Enemies.Length)];
                else if (Main.gameStage == 5)
                    enemyType = Stage1Enemies[Main.random.Next(0, Stage5Enemies.Length)];*/

                chosenPosition += new Vector2(Main.random.Next(0, 16 + 1), Main.random.Next(0, 16 + 1 - 4));
                if (Main.gameStage >= 3)
                    enemyType = Stage3Enemies[Main.random.Next(0, Stage3Enemies.Length)];
                else
                    enemyType = Stage1Enemies[Main.random.Next(0, Stage1Enemies.Length)];

                SpawnEnemy(enemyType, chosenPosition);
                //SpawnEnemy(enemyType, chosenPosition);
                spawnCooldownTimer += cooldownTime;

                for (int i = 0; i < Main.activeEntities.Count; i++)
                {
                    if (Main.activeEntities[i] == Player.player)
                        continue;

                    if (Vector2.Distance(Main.activeEntities[i].position, Player.player.position) >= Screen.resolutionWidth * 2f)
                    {
                        Main.activeEntities.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
    }
}
