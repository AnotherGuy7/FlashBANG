using FlashBANG.Entities;
using FlashBANG.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace FlashBANG.World.MapObjects
{
    public class Bulb : MapObject
    {
        public static Texture2D bulbTexture;
        public static Texture2D textureOutline;

        public static Bulb CreateBulb(Vector2 position, int visibilityID = 0)
        {
            Bulb bulb = new Bulb();
            bulb.position = position;
            bulb.visibiltyID = visibilityID;
            bulb.Initialize();
            return bulb;
        }

        public override void Initialize()
        {
            hitbox = new Rectangle((int)position.X, (int)position.Y, 14, 14);
        }

        public override void Update()
        {
            CheckForInteractions(24f);
        }

        public override void OnObjectInteraction()
        {
            Player.player.heldBulbs += 1;
            SoundPlayer.PlayLocalSound(SoundPlayer.Sounds_BulbPickUp);
            DestroyInstance();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Player.player.tileVisiblityID != visibiltyID)
                return;

            spriteBatch.Draw(bulbTexture, position, null, Color.White, 0, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);
            if (interactionAvailable)
                spriteBatch.Draw(textureOutline, position, null, Color.White, 0, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);
        }
    }
}
