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
    public class ShadowBall : Enemy
    {
        public override CollisionType collisionType => CollisionType.Enemies;
        public override CollisionType[] colliderTypes => new CollisionType[1] { CollisionType.Player };

        public static Texture2D shadowBallTexture;

        private const int BallSize = 16;
        private const int MaxHealth = 350;
        private const float MoveSpeed = 0.3f;
        private readonly Vector2 ShadowBallOrigin = new Vector2(8f, 8f);

        public static ShadowBall NewShadowBall(Vector2 position)
        {
            ShadowBall shadowBall = new ShadowBall();
            shadowBall.position = position;
            shadowBall.Initialize();
            return shadowBall;
        }

        public override void Initialize()
        {
            health = MaxHealth;
            hitboxOffset = new Point(-BallSize / 2, BallSize / 2);
            hitbox = new Rectangle(0, 0, BallSize, BallSize);
            hitboxWidth = hitbox.Width;
            hitboxHeight = hitbox.Height;
        }

        public override void Update()
        {
            Vector2 velocity = Player.player.position - position;
            velocity.Normalize();
            velocity *= MoveSpeed * (health / (float)MaxHealth);

            velocity = DetectTileCollisionsWithVelocity(velocity);

            position += velocity;
            hitbox.X = (int)position.X + hitboxOffset.X;
            hitbox.Y = (int)position.Y + hitboxOffset.Y;
            DetectCollisions(Main.activeEntities);
            UpdateEffects();

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
                Vector2 pos = position + (new Vector2(Main.random.Next(-hitboxWidth, hitboxWidth), Main.random.Next(-hitboxHeight, hitboxHeight)) / 2f);
                int lifeTime = Main.random.Next(25, 60 + 1);
                float rotation = MathHelper.ToRadians(Main.random.Next(-3, 3 + 1));
                int textureType = Main.random.Next(Smoke.WhitePixelTexture, Smoke.StarPixelTexture + 1);
                Smoke.NewSmokeParticle(pos, Vector2.Zero, Color.Black, Color.Black, 180, lifeTime, lifeTime / 2, 0.6f, rotation, textureType);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            float scale = health / (float)MaxHealth;
            spriteBatch.Draw(shadowBallTexture, position, null, Color.White, 0f, ShadowBallOrigin, scale, SpriteEffects.None, 0f);
            //ShaderBatch.QueueShaderDraw(shadowBallTexture, position, null, ShaderBatch.EffectID.RGBDistortion);
        }
    }
}
