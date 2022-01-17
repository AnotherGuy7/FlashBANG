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

        public const int Texture_Blocker = 0;
        public const int Texture_LightRing = 1;
        public const int Texture_Flashlight_1 = 2;
        public const int Texture_Flashlight_2 = 3;
        public const int Texture_Flashlight_3 = 4;
        public const int Texture_Flashlight_4 = 5;
        public const int Texture_Flashlight_5 = 6;
        private static readonly Vector2 LightRingOrigin = new Vector2(75, 75);
        private static readonly Vector2 FlashlightBeamOrigin = new Vector2(150, 150);

        public struct LightData
        {
            public int textureType;
            public float lightStrength;
            public float lightRadius;
            public Vector2 lightScale;
            public Vector2 lightPosition;
            public Color lightColor;
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
            lightData.lightColor = Color.White;
            lightData.lightRadius = radius * (16f / 150f);
            lightData.lightStrength = strength;
            lightData.lightRotation = rotation;
            queuedLightData.Add(lightData);
        }
        public static void QueueLightData(int textureType, Vector2 position, Color color, Vector2 scale, float strength = 1f, float rotation = 0f)
        {
            LightData lightData = new LightData();
            lightData.textureType = textureType;
            lightData.lightPosition = position;
            lightData.lightColor = color;
            lightData.lightScale = scale;
            lightData.lightStrength = strength;
            lightData.lightRotation = rotation;
            queuedLightData.Add(lightData);
        }

        public static void DrawLightMask(SpriteBatch spriteBatch)
        {
            /*foreach(Tile tile in Map.activeMapChunk)
            {
                if (tile.hasLight)
                    spriteBatch.Draw(lightTextures[Texture_LightRing], tile.tilePosition, null, Color.White, 0f, LightRingOrigin, tile.lightData.lightRadius, SpriteEffects.None, 0f);
            }*/
            foreach (LightData lightData in queuedLightData)
            {
                Vector2 origin = LightRingOrigin;
                if (lightData.textureType != Texture_LightRing)
                    origin = FlashlightBeamOrigin;
                Vector2 scale = new Vector2(lightData.lightRadius);
                if (lightData.lightScale != Vector2.Zero)
                    scale = lightData.lightScale;
                spriteBatch.Draw(lightTextures[lightData.textureType], lightData.lightPosition, null, lightData.lightColor, lightData.lightRotation, origin, scale, SpriteEffects.None, 0f);
            }

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
