using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlashBANG.Utilities
{
    public class ShaderBatch
    {
        /*public static List<Texture2D> queuedTextures;
        public static List<Vector2> queuedPositions;
        public static List<Rectangle> queuedRects;
        public static List<float> queuedRotations;
        public static List<float> queuedScales;
        public static List<Effect> queuedEffects;*/
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
            /*queuedTextures = new List<Texture2D>();
            queuedPositions = new List<Vector2>();
            queuedRects = new List<Rectangle>();
            queuedRotations = new List<float>();
            queuedScales = new List<float>();
            queuedEffects = new List<Effect>();*/

            queuedDraws = new List<DrawData>();
        }

        public static void DrawQueuedShaderDraws()
        {
            DrawData[] rgbList = SortEffectBatch(EffectID.RGBDistortion);
            //List<DrawData> distortionList = SortEffectBatch(RGBDistortionEffect);
            /*int numberOfDraws = queuedTextures.Count;     //Heh, one could imagine how inefficient this turned out to be
            for (int i = 0; i < numberOfDraws; i++)
            {
                DrawShaderItem(queuedTextures[i], queuedPositions[i], queuedRects[i], queuedRotations[i], queuedScales[i], queuedEffects[i]);
            }*/

            DrawShaderItems(rgbList, RGBDistortionEffect);

            /*queuedTextures.Clear();
            queuedPositions.Clear();
            queuedRects.Clear();
            queuedRotations.Clear();
            queuedScales.Clear();
            queuedEffects.Clear();*/
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
            }
            Array.Clear(queuedDrawsClone, 0, queuedDrawsClone.Length);
            Array.Clear(effectMatches, 0, effectMatches.Length);

            return sortedDraws;
        }

        /// <summary>
        /// Queues an item for drawing. Draws at the end of the Draw step.
        /// </summary>
        /// <param name="effect">The effect to use while drawing</param>
        /// <param name="texture">The texture to draw.</param>
        public static void DrawScreen(Effect effect, RenderTarget2D texture)
        {
            Main._graphics.GraphicsDevice.SetRenderTarget(null);
            Main._graphics.GraphicsDevice.Clear(Color.Black);

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, effect, null);
            Main.spriteBatch.Draw(texture, Vector2.Zero, Color.White);
            Main.spriteBatch.End();
        }


        public static void QueueShaderDraw(Texture2D texture, Vector2 position, Rectangle? rectangle, EffectID effectID)
        {
            Rectangle rect;
            if (rectangle == null)
                rect = new Rectangle(0, 0, texture.Width, texture.Height);
            else
                rect = (Rectangle)rectangle;

            /*queuedTextures.Add(texture);
            queuedPositions.Add(position);
            queuedRects.Add(rect);
            queuedRotations.Add(0f);
            queuedScales.Add(1f);
            queuedEffects.Add(effect);*/

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

        private static void DrawShaderItems(Texture2D texture, Vector2 position, Rectangle rect, float rotation, float scale, Effect effect)
        {
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, effect, Main.mainCamera.cameraMatrix);
            Main.spriteBatch.Draw(texture, position, rect, Color.White, rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.End();
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
