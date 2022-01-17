using FlashBANG.Effects;
using FlashBANG.Utilities;
using FlashBANG.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlashBANG.Entities.Enemies
{
    public class ShadowMan : Enemy
    {
        public override CollisionType collisionType => CollisionType.Enemies;
        public override CollisionType[] colliderTypes => new CollisionType[1] { CollisionType.Player };

        public static Texture2D shadowManTexture;

        private const int ManWidth = 14;
        private const int MaxHealth = 900;
        private const float MoveSpeed = 0.25f;
        private Rectangle animRect;
        private int frame = 0;
        private int frameCounter = 0;
        private int direction = 1;

        public static ShadowMan NewShadowMan(Vector2 position)
        {
            ShadowMan shadowMan = new ShadowMan();
            shadowMan.position = position;
            shadowMan.Initialize();
            return shadowMan;
        }

        public override void Initialize()
        {
            health = MaxHealth;
            hitboxOffset = new Point(0, ManWidth - 4);
            hitbox = new Rectangle(0, 0, ManWidth, 4);
            hitboxWidth = hitbox.Width;
            hitboxHeight = 4;

            animRect = new Rectangle(0, 0, 14, 27);
        }

        public override void Update()
        {
            Vector2 velocity = Player.player.position - position;
            velocity.Normalize();
            velocity *= MoveSpeed * (health / (float)MaxHealth);

            velocity = DetectTileCollisionsWithVelocity(velocity);

            direction = 1;
            if (velocity.X < 0)
                direction = -1;

            position += velocity;
            hitbox.X = (int)position.X + hitboxOffset.X;
            hitbox.Y = (int)position.Y + hitboxOffset.Y;
            DetectCollisions(Main.activeEntities);
            UpdateEffects();

            frameCounter++;
            if (frameCounter >= 11)
            {
                frame++;
                frameCounter = 0;
                if (frame >= 6)
                    frame = 0;

                animRect.Y = frame * 27;
            }

            if (health <= 0)
                DestroyInstance();
        }

        public override void HandleCollisions(CollisionBody collider, CollisionType colliderType)
        {
            if (collider == Player.player)
                Player.player.dead = true;
        }

        private void UpdateEffects()
        {
            for (int i = 0; i < Main.random.Next(1, 2 + 1); i++)
            {
                Vector2 pos = position + new Vector2(Main.random.Next(0, hitboxWidth), Main.random.Next(0, 27));
                int lifeTime = Main.random.Next(25, 60 + 1);
                float rotation = MathHelper.ToRadians(Main.random.Next(-3, 3 + 1));
                int textureType = Main.random.Next(Smoke.WhitePixelTexture, Smoke.StarPixelTexture + 1);
                Smoke.NewSmokeParticle(pos, Vector2.Zero, Color.Black, Color.Black, 180, lifeTime, lifeTime / 2, 0.6f, rotation, textureType);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            SpriteEffects effect = SpriteEffects.None;
            if (direction == -1)
                effect = SpriteEffects.FlipHorizontally;

            float scale = health / (float)MaxHealth;
            spriteBatch.Draw(shadowManTexture, position, animRect, Color.White, 0f, Vector2.Zero, scale, effect, 0f);
            //ShaderBatch.QueueShaderDraw(shadowManTexture, position, null, ShaderBatch.EffectID.RGBDistortion);
        }
    }
}
