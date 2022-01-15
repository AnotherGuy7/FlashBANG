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
        public static SoundEffect pickUpSound;

        public static Bulb CreateBulb(Vector2 position)
        {
            Bulb bulb = new Bulb();
            bulb.position = position;
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
            pickUpSound.Play(Main.SFXVolume, 0, 0);
            DestroyInstance();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(bulbTexture, hitbox, Color.White);
            Lighting.QueueLightData(Lighting.Texture_LightRing, position, 8f);
        }
    }
}
