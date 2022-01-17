using FlashBANG.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FlashBANG.UI.UIElements
{
    public class TextBox : UIObject
    {
        public Texture2D texture;
        public Color drawColor;
        public Vector2 buttonPosition;
        public bool buttonHover = false;
        public int buttonWidth = 0;
        public int buttonHeight = 0;
        public bool focused = false;
        public string textBoxText;
        public bool enterPressed = false;
        public int characterLimit = 0;

        private Texture2D barTexture;
        private float scale;
        private float defaultScale;
        private float hoverScale;
        private Rectangle hitbox;
        private Color idleColor;
        private Color hoverColor;
        private Vector2 textScale;

        public TextBox(Vector2 position, int width, int height, float defaultScale, float hoverScale, Color idleColor, Color hoverColor, int maxCharacterLimit = 24)
        {
            buttonWidth = (int)(width * defaultScale);
            buttonHeight = (int)(height * defaultScale);
            hitbox = new Rectangle((int)position.X, (int)position.Y, buttonWidth, buttonHeight);
            this.defaultScale = defaultScale;
            this.hoverScale = hoverScale;
            buttonPosition = position;
            this.idleColor = idleColor;
            this.hoverColor = hoverColor;

            float textScaleX = (width - 4f) / (Main.mainFont.MeasureString("A").X * maxCharacterLimit);
            float textScaleY = (height - 3f) / Main.mainFont.MeasureString("A").Y;
            textScale = new Vector2(textScaleX, textScaleY) * defaultScale;
            characterLimit = maxCharacterLimit;
            texture = TextureGenerator.CreatePanelTexture(width, height, 1, Color.Black, Color.White);
            barTexture = TextureGenerator.CreatePanelTexture(1, buttonHeight - 4, 0, Color.White, Color.White, false);
            textBoxText = "";

            Main.gameScreen.Window.TextInput += HandleText;
        }

        public override void Update()
        {
            scale = defaultScale;
            drawColor = idleColor;
            buttonHover = false;
            if (focused)
            {
                scale = hoverScale;
                buttonHover = true;
                drawColor = hoverColor;
            }
            if (hitbox.Contains(Main.mouseScreenPos.ToPoint()))
            {
                scale = hoverScale;
                buttonHover = true;
                drawColor = hoverColor;
                Main.mouseOverUI = true;
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    focused = true;
                }
            }


            hitbox.X = (int)buttonPosition.X;
            hitbox.Y = (int)buttonPosition.Y;
        }

        public void ResetTextBox()
        {
            focused = false;
            textBoxText = "";
            enterPressed = false;
        }

        private void HandleText(object sender, TextInputEventArgs args)
        {
            if (focused)
            {
                if (args.Key == Keys.Back)
                {
                    if (textBoxText.Length > 0)
                        textBoxText = textBoxText.Remove(textBoxText.Length - 1);
                }
                else if (args.Key == Keys.Escape)
                    ResetTextBox();
                else if (args.Key == Keys.Tab)
                    textBoxText += "   ";
                else if (args.Key == Keys.Enter)
                    enterPressed = true;
                else
                {
                    if (textBoxText.Length < characterLimit)
                        textBoxText += args.Character;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, buttonPosition, null, drawColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            Vector2 textPos = buttonPosition + new Vector2(2f, (buttonHeight / 2f) - 1f);
            if (textBoxText != null)
            {
                Vector2 textOrigin = new Vector2(0f, Main.mainFont.MeasureString(textBoxText).Y / 2f);
                spriteBatch.DrawString(Main.mainFont, textBoxText, textPos, Color.White, 0f, textOrigin, textScale, SpriteEffects.None, 0f);
            }
            if (focused)
            {
                string textBoxString = textBoxText;
                if (textBoxText == null)
                    textBoxString = " ";

                Vector2 barPos = textPos + new Vector2((Main.mainFont.MeasureString(textBoxString).X * textScale.X) + 2f, -(buttonHeight / 2) + 3f);
                spriteBatch.Draw(barTexture, barPos, Color.White);
            }
        }
    }
}
