using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using FlashBANG.Entities;

namespace FlashBANG.World.MapObjects
{
    public class Box : MapObject
    {
        public static Texture2D boxTexture;

        private bool hiding = false;
        private int frame = 0;
        private Rectangle animRect;

        public static Box CreateBox(Vector2 position)
        {
            Box box = new Box();
            box.position = position;
            box.Initialize();
            return box;
        }

        public override void Initialize()
        {
            animRect = new Rectangle(0, 0, 16, 16);
            hitbox = new Rectangle((int)position.X, (int)position.Y, 16, 16);
        }

        public override void Update()
        {
            CheckForInteractions(16f);
            if (interactionAvailable && !hiding)
                frame = 1;
            else
                frame = 0;
            animRect.Y = frame * 16;
        }

        public override void OnObjectInteraction()
        {
            hiding = !hiding;
            Player.player.hiding = hiding;
            Player.player.position = position;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(boxTexture, position, animRect, Color.White);
        }
    }
}
