using FlashBANG.Utilities;
using FlashBANG.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace FlashBANG.World.MapObjects
{
    public class Metal : MapObject
    {
        public static Texture2D metalTexture;
        public static SoundEffect pickUpSound;

        public static Metal CreateMetal(Vector2 position)
        {
            Metal metal = new Metal();
            metal.position = position;
            metal.Initialize();
            return metal;
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
            Player.player.heldMetal += 1;
            pickUpSound.Play(Main.SFXVolume, 0, 0);
            DestroyInstance();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(metalTexture, hitbox, Color.White);
        }
    }
}
