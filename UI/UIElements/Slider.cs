using FlashBANG.Effects;
using FlashBANG.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FlashBANG.UI.UIElements
{
    public class Slider : UIObject
    {
        public static Texture2D sliderButtonTexture;

        private const float TextScale = 0.3f;
        private const int SliderButtonWidth = 5;
        private const int SliderButtonHeight = 7;

        private float sliderValue = 0f;
        private bool clickedOn = true;

        public string sliderName = "";
        public Color drawColor;
        public Vector2 sliderPosition;      //Position of the actual slider button
        public Vector2 position;      //Position of the whole bar
        public Rectangle rect;

        private Vector2 textOrigin;
        private Vector2 textSize;

        public Slider(Vector2 position, int width, int height, Color color, string label = "", float defaultValue = 0f)
        {
            this.position = position;
            rect = new Rectangle((int)position.X, (int)position.Y, width, height);
            drawColor = color;
            sliderPosition.Y = position.Y + (height / 2f);
            sliderName = label;
            textSize = Main.mainFont.MeasureString(sliderName);
            textOrigin = textSize - new Vector2(0f, textSize.Y / 2f);
            SetValue(defaultValue);
        }

        public override void Update()
        {
            rect.X = (int)position.X;
            rect.Y = (int)position.Y;

            if (rect.Contains(Main.mouseUIPos.ToPoint()))
            {
                Main.mouseOverUI = true;
                if (InputManager.IsMouseLeftJustClicked())
                {
                    clickedOn = true;
                }
            }
            if (clickedOn)
            {
                if (InputManager.IsMouseLeftJustReleased())
                {
                    clickedOn = false;
                    return;
                }

                sliderPosition.X = MathHelper.Clamp(Main.mouseUIPos.X, position.X, position.X + rect.Width);
                sliderValue = (sliderPosition.X - position.X) / (float)rect.Width;
            }
        }

        public void SetValue(float sliderValue)
        {
            sliderPosition.X = position.X + (rect.Width * sliderValue);
            this.sliderValue = sliderValue;
        }

        public float GetValue()
        {
            return sliderValue;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2(sliderButtonTexture.Width / 2f, sliderButtonTexture.Height / 2f);
            spriteBatch.Draw(sliderButtonTexture, sliderPosition, null, drawColor, 0f, origin, 1f, SpriteEffects.None, 0f);

            //Top line
            Vector2 topLinePosition = position + new Vector2(1f, 0f);
            Vector2 topLineScale = new Vector2(rect.Width - 2f, 1f);
            spriteBatch.Draw(Smoke.smokePixelTextures[Smoke.WhitePixelTexture], topLinePosition, null, drawColor, 0f, Vector2.Zero, topLineScale / 2f, SpriteEffects.None, 0f);

            //Left line
            Vector2 leftLinePosition = position + new Vector2(0f, 1f);
            Vector2 leftLineScale = new Vector2(1f, rect.Height - 2f);
            spriteBatch.Draw(Smoke.smokePixelTextures[Smoke.WhitePixelTexture], leftLinePosition, null, drawColor, 0f, Vector2.Zero, leftLineScale / 2f, SpriteEffects.None, 0f);

            //Right line
            Vector2 rightLinePosition = position + new Vector2(rect.Width - 1f, 1f);
            Vector2 rightLineScale = new Vector2(1f, rect.Height - 2f);
            spriteBatch.Draw(Smoke.smokePixelTextures[Smoke.WhitePixelTexture], rightLinePosition, null, drawColor, 0f, Vector2.Zero, rightLineScale / 2f, SpriteEffects.None, 0f);

            //Left line
            Vector2 bottomLinePosition = position + new Vector2(1f, rect.Height - 1f);
            Vector2 bottomLineScale = new Vector2(rect.Width - 2f, 1f);
            spriteBatch.Draw(Smoke.smokePixelTextures[Smoke.WhitePixelTexture], bottomLinePosition, null, drawColor, 0f, Vector2.Zero, bottomLineScale / 2f, SpriteEffects.None, 0f);

            if (sliderName != "")
            {
                spriteBatch.DrawString(Main.mainFont, sliderName, position - new Vector2(5f, -rect.Height / 2f), drawColor, 0f, textOrigin, TextScale, SpriteEffects.None, 0f);
            }
        }
    }
}
