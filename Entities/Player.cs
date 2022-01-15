using FlashBANG.Entities.Enemies;
using FlashBANG.Utilities;
using FlashBANG.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace FlashBANG.Entities
{
    public class Player : CollisionBody
    {
        public static Texture2D playerIdle;
        public static Texture2D playerWalk;
        public static Texture2D flashlight;
        public static Player player;

        private const int PlayerWidth = 6;
        private const int PlayerHeight = 13;
        private const float MoveSpeed = 1.2f;
        private readonly Vector2 playerOrigin = new Vector2(3, 6.5f);
        private readonly Vector2 lightOrigin = new Vector2(3, 2);

        public Vector2 lightPosition;
        public bool hiding = false;
        public int tileVisiblityID = 0;
        public int heldMetal = 0;
        public int heldBulbs = 0;
        public int oldTileType = 0;

        private Rectangle animRect;
        private Rectangle flashlightHitbox;
        private Texture2D currentSheet;
        private Vector2 previousChunkUpdatePosition;
        private int frame = 0;
        private int frameCounter = 0;
        private float playerRotation = 0f;
        private float lightRotation = 0f;
        private int direction = 1;
        private int tileType = 0;        //The tile the player's currently standing on.
        private bool flashlightActive = false;
        private AnimationState oldAnimState;
        private AnimationState animState;

        private enum AnimationState
        {
            Idle,
            Walking
        }

        public override void Initialize()
        {
            animRect = new Rectangle(0, 0, PlayerWidth, PlayerHeight);
            currentSheet = playerIdle;
            Main.mainCamera.SetToPlayerCamera();

            hitboxOffset = new Point(-PlayerWidth / 2, (PlayerHeight / 2) - 4);
            hitbox = new Rectangle(0, 0, PlayerWidth + 1, PlayerHeight - hitboxOffset.Y - 4);
            flashlightHitbox = new Rectangle((int)position.X, (int)position.Y, 28, 28);
            player = this;
        }

        public override void Update()
        {
            if (hiding)
                return;

            Vector2 velocity = Vector2.Zero;
            if (InputManager.IsKeyPressed(Keys.W) && !tileCollisionDirection[CollisionDirection_Top])
                velocity.Y -= MoveSpeed;
            if (InputManager.IsKeyPressed(Keys.A) && !tileCollisionDirection[CollisionDirection_Left])
                velocity.X -= MoveSpeed;
            if (InputManager.IsKeyPressed(Keys.S) && !tileCollisionDirection[CollisionDirection_Bottom])
                velocity.Y += MoveSpeed;
            if (InputManager.IsKeyPressed(Keys.D) && !tileCollisionDirection[CollisionDirection_Right])
                velocity.X += MoveSpeed;
            if (InputManager.IsKeyPressed(Keys.LeftShift))
                velocity *= 1.6f;

            if (velocity == Vector2.Zero)
            {
                animState = AnimationState.Idle;
                playerRotation = 0f;
            }
            else
            {
                animState = AnimationState.Walking;
                playerRotation = MathHelper.ToRadians((float)Math.Sin(frameCounter * 0.4f) * 21f);

                direction = 1;
                if (velocity.X < 0)
                    direction = -1;
            }

            position += velocity;
            hitbox.X = (int)(position.X + hitboxOffset.X);
            hitbox.Y = (int)(position.Y + hitboxOffset.Y);
            DetectTileCollisions();
            AnimatePlayer();
            ControlFlashlight();
            Main.mainCamera.UpdateCamera(position);

            if (Vector2.Distance(previousChunkUpdatePosition, position) >= 5 * 16)
            {
                previousChunkUpdatePosition = position;
                Map.UpdateActiveChunk(position);
            }
            if (velocity != Vector2.Zero)
            {
                Point pointPosition = new Point((int)position.X / 16, (int)position.Y / 16);
                tileType = Map.map[pointPosition.X, pointPosition.Y].tileType;
                tileVisiblityID = Map.map[pointPosition.X, pointPosition.Y].visiblityID;
                if (oldTileType != tileType)
                    TileChanged();
            }
        }

        public void TileChanged()
        {
            if ((tileType == Tile.Tile_Door && oldTileType != Tile.Tile_Door) || (tileType == Tile.Tile_DoorHorizontal && oldTileType != Tile.Tile_DoorHorizontal))
                SoundPlayer.PlayLocalSound(SoundPlayer.Sounds_DoorOpen);
            if ((oldTileType == Tile.Tile_Door && tileType != Tile.Tile_Door) || (oldTileType == Tile.Tile_DoorHorizontal && tileType != Tile.Tile_DoorHorizontal))
                SoundPlayer.PlayLocalSound(SoundPlayer.Sounds_DoorClose);

            oldTileType = tileType;
        }

        private void ControlFlashlight()
        {
            Vector2 vectorToMouse = Main.mouseWorldPos - position;
            vectorToMouse.Normalize();
            lightPosition = position + (vectorToMouse * 8f);
            float angle = (float)Math.Atan2(vectorToMouse.Y, vectorToMouse.X);
            lightRotation = angle;

            if (InputManager.IsMouseLeftHeld())
            {
                if (InputManager.IsMouseLeftJustClicked())
                    SoundPlayer.PlayLocalSound(SoundPlayer.Sounds_FlashlightClick);

                flashlightHitbox.X = (int)position.X;
                flashlightHitbox.Y = (int)position.Y;
                flashlightHitbox.Width = (int)(38f * (float)Math.Cos(angle));
                flashlightHitbox.Height = (int)(38f * (float)Math.Sin(angle));
                DetectRectCollision(flashlightHitbox, Main.activeEntities);
                flashlightActive = true;
            }
            if (InputManager.IsMouseLeftJustReleased())
            {
                SoundPlayer.PlayLocalSound(SoundPlayer.Sounds_FlashlightClick, -0.3f);
                flashlightActive = false;
            }
        }

        public override void HandleRectCollisions(CollisionBody collider, CollisionType colliderType)
        {
            if (colliderType != CollisionType.Enemies)
                return;

            Enemy enemy = collider as Enemy;
        }

        private void AnimatePlayer()
        {
            if (oldAnimState != animState)
            {
                frame = 0;
                oldAnimState = animState;
            }

            if (animState == AnimationState.Idle)
            {
                if (currentSheet != playerIdle)
                    currentSheet = playerIdle;

                frameCounter++;
                if (frameCounter >= 40)
                {
                    frame++;
                    frameCounter = 0;
                    if (frame >= 2)
                        frame = 0;
                }
            }
            else if (animState == AnimationState.Walking)
            {
                if (currentSheet != playerWalk)
                    currentSheet = playerWalk;

                frameCounter++;
                if (frameCounter >= 12)
                {
                    frame++;
                    frameCounter = 0;
                    if (frame >= 4)
                        frame = 0;
                }
            }

            animRect.Y = frame * PlayerHeight;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (hiding)
                return;

            SpriteEffects effect = SpriteEffects.None;
            if (direction == -1)
                effect = SpriteEffects.FlipHorizontally;

            if (lightPosition.Y - position.Y <= 0)
                spriteBatch.Draw(flashlight, lightPosition, null, Color.White, lightRotation, lightOrigin, 1f, SpriteEffects.None, 0f);

            spriteBatch.Draw(currentSheet, position, animRect, Color.White, playerRotation, playerOrigin, 1f, effect, 0f);
            if (lightPosition.Y - position.Y > 0)
                spriteBatch.Draw(flashlight, lightPosition, null, Color.White, lightRotation, lightOrigin, 1f, SpriteEffects.None, 0f);

            if (flashlightActive)
                Lighting.QueueLightData(Lighting.Texture_Flashlight_1, lightPosition, 12f, 1f, lightRotation);
        }
    }
}
