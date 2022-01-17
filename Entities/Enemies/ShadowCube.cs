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
    public class ShadowCube : Enemy
    {
        public override CollisionType collisionType => CollisionType.Enemies;
        public override CollisionType[] colliderTypes => new CollisionType[1] { CollisionType.Player };

        public static Texture2D shadowCubeTexture;

        private const int CubeSize = 16;
        private const int MaxHealth = 150;
        private const float MoveSpeed = 0.3f;
        private readonly Vector2 ShadowCubeOrigin = new Vector2(8f, 8f);

        private float cubeRotation = 0f;

        public static ShadowCube NewShadowCube(Vector2 position)
        {
            ShadowCube shadowCube = new ShadowCube();
            shadowCube.position = position;
            shadowCube.Initialize();
            return shadowCube;
        }

        public override void Initialize()
        {
            health = MaxHealth;
            hitboxOffset = new Point(-CubeSize / 2, CubeSize / 2);
            hitbox = new Rectangle((int)position.X - (CubeSize / 2), (int)position.Y - (CubeSize / 2), CubeSize, CubeSize);
            hitboxWidth = hitbox.Width;
            hitboxHeight = hitbox.Height;
        }

        public override void Update()
        {
            cubeRotation += MathHelper.ToRadians(3f);
            UpdateEffects();
            DetectCollisions(Main.activeEntities);
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
                int lifeTime = Main.random.Next(45, 80 + 1);
                float rotation = MathHelper.ToRadians(Main.random.Next(-3, 3 + 1));
                int textureType = Main.random.Next(Smoke.WhitePixelTexture, Smoke.StarPixelTexture + 1);
                Smoke.NewSmokeParticle(pos, Vector2.Zero, Color.Black, Color.Black, 180, lifeTime, lifeTime / 2, 0.6f, rotation, textureType);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            float scale = health / (float)MaxHealth;
            spriteBatch.Draw(shadowCubeTexture, position, null, Color.White, cubeRotation, ShadowCubeOrigin, scale, SpriteEffects.None, 0f);
        }
    }
}
