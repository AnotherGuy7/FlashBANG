using FlashBANG.World;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlashBANG.Entities.Enemies
{
    public class Enemy : CollisionBody
    {
        public int health = 0;

        public Vector2 DetectTileCollisionsWithVelocity(Vector2 velocity)
        {
            Vector2 modifiedVelocity = velocity;
            Rectangle modifiedHitbox = hitbox;
            modifiedHitbox.X += (int)velocity.X;
            modifiedHitbox.Y += (int)velocity.Y;
            for (int x = 0; x < 3; x++)
            {
                if (modifiedVelocity == Vector2.Zero)
                    break;

                for (int y = 0; y < 3; y++)
                {
                    int pointX = hitbox.X / 16;
                    int pointY = hitbox.Y / 16;
                    Point tilePos = new Point(pointX - 1 + x, pointY - 1 + y);
                    tilePos.X = Math.Clamp(tilePos.X, 0, Map.MapWidth - 1);
                    tilePos.Y = Math.Clamp(tilePos.Y, 0, Map.MapHeight - 1);

                    Rectangle collidingRect = Map.activeMapChunk[tilePos.X, tilePos.Y].tileRect;
                    if (collidingRect.Intersects(modifiedHitbox))
                    {
                        int hitboxReductionFactor = 2;
                        bool withinXBoundaries = modifiedHitbox.Left + hitboxReductionFactor < collidingRect.Right && modifiedHitbox.Right - hitboxReductionFactor > collidingRect.X;
                        bool withinYBoundaries = modifiedHitbox.Top + hitboxReductionFactor < collidingRect.Bottom && modifiedHitbox.Bottom - hitboxReductionFactor > collidingRect.Top;

                        if (withinXBoundaries)
                        {
                            if (modifiedHitbox.Top < collidingRect.Bottom && modifiedHitbox.Center.Y - hitboxReductionFactor > collidingRect.Center.Y)      //Top
                            {
                                modifiedVelocity.Y = 0.05f;
                            }
                            else if (modifiedHitbox.Bottom > collidingRect.Top && modifiedHitbox.Center.Y + hitboxReductionFactor < collidingRect.Center.Y)     //Bottom
                            {
                                modifiedVelocity.Y = -0.05f;
                            }
                        }

                        if (withinYBoundaries)
                        {
                            if (modifiedHitbox.Left < collidingRect.Right && modifiedHitbox.Center.X + hitboxReductionFactor < collidingRect.Center.X)      //Left
                            {
                                modifiedVelocity.X = -0.05f;
                            }
                            else if (modifiedHitbox.Right > collidingRect.Left && modifiedHitbox.Center.X - hitboxReductionFactor > collidingRect.Center.X)     //Right
                            {
                                modifiedVelocity.X = 0.05f;
                            }
                        }
                    }
                }
            }
            return modifiedVelocity;
        }
    }
}
