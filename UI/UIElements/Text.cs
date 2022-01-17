using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FlashBANG.UI.UIElements
{
    public class Text : UIObject
    {
        public string text;
        public Vector2 position;
        public Color drawColor;
        public float scale;
        public Vector2 origin;
        public readonly Vector2 size;

        //Perhaps text alignment support may be useful in the future.

        public Text(string text, Vector2 textPosition, Color textColor, float textScale = 0.5f, bool centerOrigin = false)
        {
            this.text = text;
            position = textPosition;
            drawColor = textColor;
            scale = textScale;

            Vector2 fontSize = Main.mainFont.MeasureString(text);
            if (centerOrigin)
                origin = fontSize / 2f;
            else
                origin = Vector2.Zero;

            size = fontSize * scale;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Main.mainFont, text, position, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
        }
    }
}
