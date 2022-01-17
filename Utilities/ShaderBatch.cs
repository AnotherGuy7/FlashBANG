using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlashBANG.Utilities
{
    public class ShaderBatch
    {
        public static List<DrawData> queuedDraws;

        public struct DrawData
        {
            public Texture2D texture;
            public Vector2 position;
            public Rectangle rect;
            public float rotation;
            public Vector2 origin;
            public float scale;
            public EffectID effectID;
        }

        public enum EffectID
        {
            BasicDistortion,
            RGBDistortion
        }

        public static Effect BasicDistortionEffect;
        public static Effect RGBDistortionEffect;
        public static Effect LightingEffect;

        public static void InitializeShaderBatchLists()
        {
            queuedDraws = new List<DrawData>();
        }

        public static void DrawQueuedShaderDraws()
        {
            if (queuedDraws.Count <= 0)
                return;

            DrawData[] distortionList = SortEffectBatch(EffectID.BasicDistortion);
            DrawData[] rgbList = SortEffectBatch(EffectID.RGBDistortion);
            DrawShaderItems(distortionList, BasicDistortionEffect);
            DrawShaderItems(rgbList, RGBDistortionEffect);

            queuedDraws.Clear();
        }

        /// <summary>
        /// Creates a list of draw datas using this effect.
        /// </summary>
        /// <param name="effect">The effect to be used as a sorting requirement.</param>
        /// <returns></returns>
        private static DrawData[] SortEffectBatch(EffectID effect)
        {
            int amountOfMatches = 0;
            bool[] effectMatches = new bool[queuedDraws.Count];
            for (int i = 0; i < queuedDraws.Count; i++)
            {
                if (queuedDraws[i].effectID == effect)
                {
                    amountOfMatches++;
                    effectMatches[i] = true;
                }
            }

            int sortIndex = -1;
            DrawData[] sortedDraws = new DrawData[amountOfMatches];
            DrawData[] queuedDrawsClone = queuedDraws.ToArray();
            for (int i = 0; i < queuedDraws.Count; i++)
            {
                if (!effectMatches[i])
                    continue;

                sortIndex++;
                sortedDraws[sortIndex] = queuedDrawsClone[i];
                queuedDraws.RemoveAt(i);
                i--;
            }
            Array.Clear(queuedDrawsClone, 0, queuedDrawsClone.Length);
            Array.Clear(effectMatches, 0, effectMatches.Length);

            return sortedDraws;
        }


        public static void QueueShaderDraw(Texture2D texture, Vector2 position, Rectangle? rectangle, EffectID effectID)
        {
            Rectangle rect;
            if (rectangle == null)
                rect = new Rectangle(0, 0, texture.Width, texture.Height);
            else
                rect = (Rectangle)rectangle;

            DrawData drawData = new DrawData();
            drawData.texture = texture;
            drawData.position = position;
            drawData.rect = rect;
            drawData.rotation = 0f;
            drawData.origin = Vector2.Zero;
            drawData.scale = 0f;
            drawData.effectID = effectID;
            queuedDraws.Add(drawData);
        }

        public static void QueueShaderDraw(Texture2D texture, Vector2 position, Rectangle? rectangle, float rotation, Vector2 origin, float scale, EffectID effectID)
        {
            Rectangle rect;
            if (rectangle == null)
                rect = new Rectangle(0, 0, texture.Width, texture.Height);
            else
                rect = (Rectangle)rectangle;

            DrawData drawData = new DrawData();
            drawData.texture = texture;
            drawData.position = position;
            drawData.rect = rect;
            drawData.rotation = rotation;
            drawData.origin = origin;
            drawData.scale = scale;
            drawData.effectID = effectID;
            queuedDraws.Add(drawData);
        }

        private static void DrawShaderItems(DrawData[] drawData, Effect effect)
        {
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, effect, Main.mainCamera.cameraMatrix);
            
            for (int i = 0; i < drawData.Length; i++)
            {
                DrawData data = drawData[i];
                Main.spriteBatch.Draw(data.texture, data.position, data.rect, Color.White, data.rotation, data.origin, data.scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.End();
        }
    }
}
