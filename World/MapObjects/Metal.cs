using FlashBANG.Entities;
using FlashBANG.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace FlashBANG.World.MapObjects
{
    public class Metal : MapObject
    {
        public static Texture2D metalTexture;
        public static Texture2D textureOutline;

        public static Metal CreateMetal(Vector2 position, int visibilityID = 0)
        {
            Metal metal = new Metal();
            metal.position = position;
            metal.visibiltyID = visibilityID;
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
            SoundPlayer.PlayLocalSound(SoundPlayer.Sounds_MetalPickUp);
            DestroyInstance();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Player.player.tileVisiblityID != visibiltyID)
                return;

            spriteBatch.Draw(metalTexture, position, null, Color.White, 0, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);
            if (interactionAvailable)
                spriteBatch.Draw(textureOutline, position, null, Color.White, 0, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);
        }
    }
}
