using FlashBANG.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FlashBANG.Utilities
{
    public class TextureGenerator
    {

        public static Texture2D CreateTexture(int width, int height)
        {
            return new Texture2D(Main._graphics.GraphicsDevice, width, height);
        }

        public static Texture2D CreatePanelTexture(int width, int height, int outlineSize, Color backgroundColor, Color outlineColor, bool cutCorners = true)
        {
            Texture2D texture = CreateTexture(width, height);
            Color[] colorArray = new Color[width * height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color chosenColor = backgroundColor;

                    if (y < outlineSize || y > height - outlineSize - 1 || x < outlineSize || x > width - outlineSize - 1)
                        chosenColor = outlineColor;

                    if (cutCorners && ((x == 0 && y == 0) || (x == 0 && y == height - 1) || (x == width - 1 && y == 0) || (x == width - 1 && y == height - 1)))
                        chosenColor = new Color(0, 0, 0, 0);

                    colorArray[x + y * width] = chosenColor;
                }
            }
            texture.SetData(colorArray);
            return texture;
        }

        public static Texture2D CreateTextureOutline(Texture2D sampleTexture)
        {
            int sampleTextureWidth = sampleTexture.Width;
            int sampleTextureHeight = sampleTexture.Height;
            Color[] sampleTextureColorArray = new Color[sampleTextureWidth * sampleTextureHeight];
            sampleTexture.GetData(sampleTextureColorArray);

            Texture2D newOutlineTexture = CreateTexture(sampleTextureWidth, sampleTextureHeight);
            Color[] newTextureColorArray = new Color[sampleTextureWidth * sampleTextureHeight];

            for (int x = 0; x < sampleTextureWidth; x++)
            {
                for (int y = 0; y < sampleTextureHeight; y++)
                {
                    Color outlineColor = Color.Transparent;

                    if (sampleTextureColorArray[x + y * sampleTextureWidth] != Color.Transparent)
                        continue;

                    int topIndex = x + ((y - 1) * sampleTextureWidth);
                    if (topIndex >= 0 && topIndex < sampleTextureColorArray.Length)
                    {
                        if (sampleTextureColorArray[topIndex] != Color.Transparent)
                            outlineColor = Color.White;
                    }

                    int rightIndex = (x + 1) + (y * sampleTextureWidth);
                    if (rightIndex >= 0 && rightIndex < sampleTextureColorArray.Length)
                    {
                        if (sampleTextureColorArray[rightIndex] != Color.Transparent)
                            outlineColor = Color.White;
                    }

                    int bottomIndex = x + ((y + 1) * sampleTextureWidth);
                    if (bottomIndex >= 0 && bottomIndex < sampleTextureColorArray.Length)
                    {
                        if (sampleTextureColorArray[bottomIndex] != Color.Transparent)
                            outlineColor = Color.White;
                    }

                    int leftIndex = (x - 1) + (y * sampleTextureWidth);
                    if (leftIndex >= 0 && leftIndex < sampleTextureColorArray.Length)
                    {
                        if (sampleTextureColorArray[leftIndex] != Color.Transparent)
                            outlineColor = Color.White;
                    }

                    newTextureColorArray[x + y * sampleTextureWidth] = outlineColor;
                }
            }

            newOutlineTexture.SetData(newTextureColorArray);
            return newOutlineTexture;
        }

        public static Texture2D MergeTextures(Texture2D[] texturesToMerge)
        {
            int textureWidth = texturesToMerge[0].Width;
            int textureHeight = texturesToMerge[0].Height;
            Texture2D newTexture = texturesToMerge[0];
            Color[] newMergedTextureData = new Color[textureWidth * textureHeight];
            newTexture.GetData(newMergedTextureData);
            for (int i = 1; i < texturesToMerge.Length; i++)
            {
                Color[] textureData = new Color[textureWidth * textureHeight];
                texturesToMerge[i].GetData(textureData);
                for (int c = 0; c < textureData.Length; c++)
                {
                    if (textureData[c].A == 0)
                        continue;

                    newMergedTextureData[c] = textureData[c];
                }
            }
            newTexture.SetData(newMergedTextureData);
            return newTexture;
        }

        public static void DrawDebugRectangle(SpriteBatch spriteBatch, Rectangle rectangle, Color color)
        {
            spriteBatch.Draw(Smoke.smokePixelTextures[Smoke.WhitePixelTexture], rectangle, color);
        }
    }
}
