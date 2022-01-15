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

        private static readonly int[] Stage1Enemies = new int[2] { Enemy_ShadowCube, Enemy_ShadowBall };
        private static readonly int[] Stage2Enemies = new int[1] { 0 };
        private static readonly int[] Stage3Enemies = new int[1] { 0 };
        private static readonly int[] Stage4Enemies = new int[1] { 0 };
        private static readonly int[] Stage5Enemies = new int[1] { 0 };

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
            }
            Main.activeEntities.Add(newInstance);
        }

        public static void UpdateSpawner()
        {
            if (Main.random.Next(0, 100 + 1) == 0)
            {
                Point playerPoint = (Player.player.position / 16f).ToPoint();
                Point chosenPoint = Point.Zero;
                while (chosenPoint == Point.Zero)
                {
                    int screenWidthMin = (Screen.actualResolutionWidth / 3) / 2;
                    int screenHeightMin = (Screen.actualResolutionHeight / 3) / 2;
                    Point checkPoint = playerPoint + new Point(Main.random.Next(screenWidthMin, screenWidthMin + 12), Main.random.Next(screenHeightMin, screenHeightMin + 12));
                    if (checkPoint.X < 0 || checkPoint.X >= Map.MapWidth || checkPoint.Y < 0 || checkPoint.Y >= Map.MapHeight)
                        continue;

                    if (Map.map[checkPoint.X, checkPoint.Y].tileRect != null)
                        continue;

                    chosenPoint = checkPoint;

                }
                int enemyType = 0;
                if (Main.gameStage == 1)
                    enemyType = Stage1Enemies[Main.random.Next(0, Stage1Enemies.Length)];
                else if (Main.gameStage == 2)
                    enemyType = Stage1Enemies[Main.random.Next(0, Stage2Enemies.Length)];
                else if (Main.gameStage == 3)
                    enemyType = Stage1Enemies[Main.random.Next(0, Stage3Enemies.Length)];
                else if (Main.gameStage == 4)
                    enemyType = Stage1Enemies[Main.random.Next(0, Stage4Enemies.Length)];
                else if (Main.gameStage == 5)
                    enemyType = Stage1Enemies[Main.random.Next(0, Stage5Enemies.Length)];

                SpawnEnemy(enemyType, chosenPoint.ToVector2() * 16f);
            }
        }
    }
}
