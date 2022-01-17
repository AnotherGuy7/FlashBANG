using FlashBANG.Entities.Enemies;
using FlashBANG.UI;
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

        public override CollisionType collisionType => CollisionType.Player;

        public const int Flashlight_Basic = 0;
        public const int Flashlight_Enchanced = 1;
        public const int Flashlight_Tribeam = 2;
        public const int Flashlight_Unilaser = 3;
        public const int Flashlight_FlashBANGCannon = 4;

        private const int PlayerWidth = 6;
        private const int PlayerHeight = 13;
        private const float MoveSpeed = 1.2f;
        private readonly Vector2 playerOrigin = new Vector2(3, 6.5f);
        private readonly Vector2 lightOrigin = new Vector2(3, 2);

        public Vector2 lightPosition;
        public bool hiding = false;
        public bool dead = false;
        public int tileVisiblityID = 0;
        public int heldMetal = 0;
        public int heldBulbs = 0;
        public int oldTileType = 0;
        public int heldFlashlightType = 0;

        private Rectangle animRect;
        private Texture2D currentSheet;
        private Vector2 previousChunkUpdatePosition;
        private int frame = 0;
        private int frameCounter = 0;
        private int deathTimer = 0;
        private int stage5Timer = 0;
        private float playerRotation = 0f;
        private float lightRotation = 0f;
        private int direction = 1;
        private int tileType = 0;        //The tile the player's currently standing on.
        private bool flashlightActive = false;
        private int[] flashlightRanges = new int[5] { 12, 20, 24, 108, 108 };
        private float[] flashlightHeightMult = new float[5] { 1f, 1f, 1f, 0.1f, 0.2f };
        private int[] flashlightSpreads = new int[5] { 45, 65, 50, 6, 18 };
        private int[] flashlightDamages = new int[5] { 1, 3, 6, 10, 16 };
        private int[] flashlightMaskIndexes = new int[5] {
            Lighting.Texture_Flashlight_1,
            Lighting.Texture_Flashlight_2,
            Lighting.Texture_Flashlight_3,
            Lighting.Texture_Flashlight_4,
            Lighting.Texture_Flashlight_5
        };
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
            player = this;
            PlayerUI.NewPlayerUI();
        }

        public override void Update()
        {
            if (dead)
            {
                deathTimer++;
                if (deathTimer >= 3 * 60)
                {
                    Main.gameLost = true;
                    Main.gameState = Main.GameState.End;
                    EndScreen.NewEndScreen();
                }
            }
            if (hiding || dead)
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
            ManageSoundscape();
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
            if (Main.gameStage == 5)
            {
                stage5Timer++;
                if (stage5Timer >= 60 * 60)
                {
                    Main.gameState = Main.GameState.End;
                    EndScreen.NewEndScreen();
                }
            }
        }

        public void ManageSoundscape()
        {
            SoundPlayer.LoopHum();
            if (Main.random.Next(0, 900 + 1) == 0)
                SoundPlayer.PlayRandomAmbienceSound();
        }

        public void TileChanged()
        {
            if ((tileType == Tile.Tile_Door && oldTileType != Tile.Tile_Door) || (tileType == Tile.Tile_DoorHorizontal && oldTileType != Tile.Tile_DoorHorizontal))
                SoundPlayer.PlayLocalSound(SoundPlayer.Sounds_DoorOpen, -0.2f);
            if ((oldTileType == Tile.Tile_Door && tileType != Tile.Tile_Door) || (oldTileType == Tile.Tile_DoorHorizontal && tileType != Tile.Tile_DoorHorizontal))
                SoundPlayer.PlayLocalSound(SoundPlayer.Sounds_DoorClose, -0.5f);

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

                //flashlightHitbox.X = (int)position.X;
                //flashlightHitbox.Y = (int)position.Y;
                //flashlightHitbox.Width = (int)(38f * (float)Math.Cos(angle));
                //flashlightHitbox.Height = (int)(38f * (float)Math.Sin(angle));
                flashlightActive = true;

                    for (int i = 0; i < Main.activeEntities.Count; i++)
                {
                    CollisionBody body = Main.activeEntities[i];
                    if (!(body is Enemy))
                        continue;

                    if (Vector2.Distance(position, body.position) <= flashlightRanges[heldFlashlightType] * 8f)
                    {
                        Vector2 vectorToEnemy = body.position - position;
                        float angleToEnemy = (float)(Math.Atan2(vectorToEnemy.Y, vectorToEnemy.X) + Math.PI);
                        float flashlightAngle = lightRotation + (float)Math.PI;
                        float higherAngle = flashlightAngle + (MathHelper.ToRadians(flashlightSpreads[heldFlashlightType] / 2));
                        float lowerAngle = flashlightAngle - (MathHelper.ToRadians(flashlightSpreads[heldFlashlightType] / 2));
                        bool alternateSearch = false;
                        /*if (higherAngle > Math.PI * 2)
                        {
                            alternateSearch = true;
                            higherAngle -= (float)Math.PI * 3;
                            higherAngle -= 0.1f;
                        }
                        if (lowerAngle < 0)
                        {
                            alternateSearch = true;
                            lowerAngle += (float)Math.PI * 3;
                            lowerAngle += 0.1f;
                        }*/
                        if (flashlightAngle < (float)Math.PI / 2f)
                        {
                            if (angleToEnemy < (float)Math.PI / 2f)
                                angleToEnemy += (float)Math.PI / 2f;
                            else if (angleToEnemy > (float)(3 * Math.PI) / 4f)
                                angleToEnemy -= (float)Math.PI / 2f;

                            lowerAngle += (float)Math.PI / 2f;
                            higherAngle += (float)Math.PI / 2f;
                        }
                        else if (flashlightAngle > (float)Math.PI * 1.5f)
                        {
                            if (angleToEnemy < (float)Math.PI / 2f)
                                angleToEnemy += (float)Math.PI / 2f;
                            else if (angleToEnemy > (float)(3 * Math.PI) / 4f)
                                angleToEnemy -= (float)Math.PI / 2f;

                            lowerAngle -= (float)Math.PI / 2f;
                            higherAngle -= (float)Math.PI / 2f;
                        }

                        if (angleToEnemy < higherAngle && angleToEnemy > lowerAngle)
                        {
                            Enemy enemy = body as Enemy;
                            enemy.health -= flashlightDamages[heldFlashlightType];
                        }
                        /*if (alternateSearch)
                        {
                            if (angleToEnemy > higherAngle && angleToEnemy < lowerAngle)
                            {
                                Enemy enemy = body as Enemy;
                                enemy.health -= 1;
                            }
                        }*/
                    }
                }
            }
            if (InputManager.IsMouseLeftJustReleased())
            {
                SoundPlayer.PlayLocalSound(SoundPlayer.Sounds_FlashlightClick, -0.3f);
                flashlightActive = false;
            }
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
            {
                float rangeScale = flashlightRanges[heldFlashlightType] * (16f / 300f);
                Vector2 scale = new Vector2(rangeScale, rangeScale * flashlightHeightMult[heldFlashlightType]);
                Lighting.QueueLightData(flashlightMaskIndexes[heldFlashlightType], lightPosition, Color.White, scale, 1f, lightRotation);
            }
        }
    }
}
