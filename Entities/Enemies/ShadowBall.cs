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
        private const int MaxHealth = 100;
        private const float MoveSpeed = 0.3f;

        public static ShadowBall NewShadowBall(Vector2 position)
        {
            ShadowBall shadowBall = new ShadowBall();
            shadowBall.position = position;
            shadowBall.Initialize();
            return shadowBall;
        }

        public override void Initialize()
        {
            hitboxOffset = new Point(-BallSize / 2, BallSize / 2);
            hitbox = new Rectangle(0, 0, BallSize, BallSize);
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
        }

        public override void HandleCollisions(CollisionBody collider, CollisionType colliderType)
        {
            Player player = Player.player;
        }

        private void UpdateEffects()
        {
            for (int i = 0; i < Main.random.Next(1, 4 + 1); i++)
            {
                Vector2 pos = position + (new Vector2(Main.random.Next(-hitboxWidth, hitboxWidth), Main.random.Next(-hitboxHeight, hitboxHeight)) / 2f);
                int lifeTime = Main.random.Next(45, 80 + 1);
                float rotation = MathHelper.ToRadians(Main.random.Next(-3, 3 + 1));
                int textureType = Main.random.Next(Smoke.WhitePixelTexture, Smoke.StarPixelTexture + 1);
                Smoke.NewSmokeParticle(pos, Vector2.Zero, Color.Black, Color.Black, 180, lifeTime, lifeTime / 2, 0.6f, rotation, textureType);
            }
        }

        

        public override void Draw(SpriteBatch spriteBatch)
        {
            ShaderBatch.QueueShaderDraw(shadowBallTexture, position, null, ShaderBatch.RGBDistortionEffect);
        }
    }
}
