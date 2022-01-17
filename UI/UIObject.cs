using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FlashBANG.UI
{
    public class UIObject
    {
        public virtual void Initialize()
        { }

        public virtual void Update()
        { }

        public virtual void Draw(SpriteBatch spriteBatch)
        { }

        public int ToUISpace(int num)
        {
            return num / 2;
        }

        public float ToUISpace(float num)
        {
            return num / 2f;
        }

        public Vector2 ToUICoords(Vector2 pos)
        {
            return pos / 2f;
        }
    }
}
