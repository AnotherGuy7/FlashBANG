using FlashBANG.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlashBANG.Utilities
{
    public class Lighting
    {
        public static bool applyLighting = true;
        public static Texture2D[] lightTextures;
        private static List<LightData> queuedLightData;

        public static int Texture_Blocker = 0;
        public static int Texture_LightRing = 1;
        public static int Texture_Flashlight_1 = 2;
        private static readonly Vector2 LightRingOrigin = new Vector2(75, 75);

        public struct LightData
        {
            public int textureType;
            public float lightStrength;
            public float lightRadius;
            public Vector2 lightPosition;
            public float lightRotation;
        }

        public static void InitializeLighting()
        {
            queuedLightData = new List<LightData>();
        }

        public static void QueueLightData(int textureType, Vector2 position, float radius, float strength = 1f, float rotation = 0f)
        {
            LightData lightData = new LightData();
            lightData.textureType = textureType;
            lightData.lightPosition = position;
            lightData.lightRadius = radius * (16f / 150f);
            lightData.lightStrength = strength;
            lightData.lightRotation = rotation;
            queuedLightData.Add(lightData);
        }

        public static void DrawLightMask(SpriteBatch spriteBatch)
        {
            foreach(Tile tile in Map.activeMapChunk)
            {
                if (tile.hasLight)
                    spriteBatch.Draw(lightTextures[Texture_LightRing], tile.tilePosition, null, Color.White, 0f, LightRingOrigin, tile.lightData.lightRadius, SpriteEffects.None, 0f);
            }
            foreach (LightData lightData in queuedLightData)
                spriteBatch.Draw(lightTextures[lightData.textureType], lightData.lightPosition, null, Color.White, lightData.lightRotation, LightRingOrigin, lightData.lightRadius, SpriteEffects.None, 0f);

            if (queuedLightData.Count > 0)
                queuedLightData.Clear();
        }

        public static void DrawBlockerMask(SpriteBatch spriteBatch)
        {
            foreach (Tile tile in Map.activeMapChunk)
                if (tile.tileType == Tile.Tile_Void)
                    spriteBatch.Draw(lightTextures[Texture_Blocker], tile.tilePosition, Color.White);
        }
    }
}
