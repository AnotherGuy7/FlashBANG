using FlashBANG.World;
using FlashBANG.World.MapObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace FlashBANG.Entities
{
    public abstract class CollisionBody
    {
        public Vector2 position;
        public Rectangle hitbox;
        public Point hitboxOffset;
        public int hitboxWidth = 0;
        public int hitboxHeight = 0;

        /// <summary>
        /// An array of what this object can collide with.
        /// </summary>
        public virtual CollisionType[] colliderTypes { get; }

        /// <summary>
        /// The type of the collision of this object.
        /// </summary>
        public virtual CollisionType collisionType { get; }

        public bool[] tileCollisionDirection = new bool[4];
        public readonly int CollisionDirection_Top = 0;        //To make it usable in all inheritants without needing to reference this class
        public readonly int CollisionDirection_Bottom = 1;
        public readonly int CollisionDirection_Left = 2;
        public readonly int CollisionDirection_Right = 3;

        public enum CollisionType
        {
            None,
            Player,
            Enemies,
        }


        public virtual void Initialize()
        { }

        public virtual void Update()
        { }

        public virtual void Draw(SpriteBatch spriteBatch)
        { }

        /// <summary>
        /// Detects collisions between the object this method is called on and the objects in the list.
        /// </summary>
        /// <param name="possibleIntersectors">The list of collision bodies to compare against.</param>
        public void DetectCollisions(List<CollisionBody> possibleIntersectors)
        {
            CollisionBody[] possibleIntersectorsCopy = possibleIntersectors.ToArray();
            foreach (CollisionBody intersector in possibleIntersectorsCopy)
            {
                if (hitbox.Intersects(intersector.hitbox))
                {
                    HandleAnyCollision();
                    HandleCollisions(intersector, intersector.collisionType);
                    break;
                }
            }
        }

        /// <summary>
        /// A method that gets called whenever a collision happens.
        /// </summary>
        /// <param name="collider"> The collider. </param>
        /// <param name="colliderType"> The collision type of the collider. </param>
        public virtual void HandleCollisions(CollisionBody collider, CollisionType colliderType)
        { }

        /// <summary>
        /// Detects collisions between the object this method is being called on and the active map chunk.
        /// </summary>
        /// <returns>Whether or not a collision happened. Direction is returnd in the CollisionDirection array.</returns>
        public bool DetectTileCollisions()
        {
            bool colliding = false;
            for (int i = 0; i < tileCollisionDirection.Length; i++)
                tileCollisionDirection[i] = false;

            if (Map.activeMapChunk == null)
                return false;

            for (int x = 0; x < Map.ChunkSize; x++)
            {
                for (int y = 0; y < Map.ChunkSize; y++)
                {
                    Tile possibleColliderTile = Map.activeMapChunk[x, y];

                    if (hitbox.Intersects(possibleColliderTile.tileRect))
                    {
                        colliding = true;
                        Rectangle collidingRect = possibleColliderTile.tileRect;
                        int hitboxReductionFactor = 2;
                        bool withinXBoundaries = hitbox.Left + hitboxReductionFactor < collidingRect.Right && hitbox.Right - hitboxReductionFactor > collidingRect.X;
                        bool withinYBoundaries = hitbox.Top + hitboxReductionFactor < collidingRect.Bottom && hitbox.Bottom - hitboxReductionFactor > collidingRect.Top;

                        if (withinXBoundaries)
                        {
                            if (hitbox.Top < collidingRect.Bottom && hitbox.Center.Y > collidingRect.Center.Y)
                            {
                                tileCollisionDirection[CollisionDirection_Top] = true;
                            }
                            if (hitbox.Bottom > collidingRect.Top && hitbox.Center.Y < collidingRect.Center.Y)
                            {
                                tileCollisionDirection[CollisionDirection_Bottom] = true;
                            }
                        }

                        if (withinYBoundaries)
                        {
                            if (hitbox.Left < collidingRect.Right && hitbox.Center.X < collidingRect.Center.X)
                            {
                                tileCollisionDirection[CollisionDirection_Right] = true;
                            }
                            if (hitbox.Right > collidingRect.Left && hitbox.Center.X > collidingRect.Center.X)
                            {
                                tileCollisionDirection[CollisionDirection_Left] = true;
                            }
                        }
                        HandleAnyCollision();
                    }
                }
            }
            return colliding;
        }

        /// <summary>
        /// Detects collisions with Map Objects in the active chunk.
        /// </summary>
        /// <returns>Whether or not a collision happened. Direction is returnd in the CollisionDirection array.</returns>
        public bool DetectMapObjectCollisions()
        {
            bool colliding = false;
            MapObject[] possibleIntersectorsCopy = Map.activeMapObjects.ToArray();
            for (int i = 0; i < possibleIntersectorsCopy.Length; i++)
            {
                MapObject possibleColliderObject = possibleIntersectorsCopy[i];
                if (possibleColliderObject.noCollision)
                    continue;

                if (hitbox.Intersects(possibleColliderObject.hitbox))
                {
                    colliding = true;
                    Rectangle collidingRect = possibleColliderObject.hitbox;
                    int hitboxReductionFactor = 2;
                    bool withinXBoundaries = hitbox.Left + hitboxReductionFactor < collidingRect.Right && hitbox.Right - hitboxReductionFactor > collidingRect.X;
                    bool withinYBoundaries = hitbox.Top + hitboxReductionFactor < collidingRect.Bottom && hitbox.Bottom - hitboxReductionFactor > collidingRect.Top;

                    if (withinXBoundaries)
                    {
                        if (hitbox.Top < collidingRect.Bottom && hitbox.Center.Y > collidingRect.Center.Y)
                        {
                            tileCollisionDirection[CollisionDirection_Top] = true;
                        }
                        if (hitbox.Bottom > collidingRect.Top && hitbox.Center.Y < collidingRect.Center.Y)
                        {
                            tileCollisionDirection[CollisionDirection_Bottom] = true;
                        }
                    }

                    if (withinYBoundaries)
                    {
                        if (hitbox.Left < collidingRect.Right && hitbox.Center.X < collidingRect.Center.X)
                        {
                            tileCollisionDirection[CollisionDirection_Right] = true;
                        }
                        if (hitbox.Right > collidingRect.Left && hitbox.Center.X > collidingRect.Center.X)
                        {
                            tileCollisionDirection[CollisionDirection_Left] = true;
                        }
                    }
                    HandleAnyCollision();
                }
            }
            return colliding;
        }

        /// <summary>
        /// Detects collisions with Map Objects in the active chunk and calls HandleMapCollisions() if a collision was detected.
        /// </summary>
        public void DetectHandledMapObjectCollisions()
        {
            MapObject[] possibleIntersectorsCopy = Map.activeMapObjects.ToArray();
            foreach (MapObject intersector in possibleIntersectorsCopy)
            {
                if (intersector.noCollision)
                    continue;

                if (hitbox.Intersects(intersector.hitbox))
                {
                    HandleAnyCollision();
                    HandleMapObjectCollision(intersector);
                    break;
                }
            }
        }

        /// <summary>
        /// Detects rectangle-rectangle collisions with the input rectangle and the rectangles of the input list.
        /// </summary>
        /// <param name="collidingRect">The rectangle to check collisions for.</param>
        /// <param name="possibleIntersectors">The list of collision bodies to compare against.</param>
        public void DetectRectCollision(Rectangle collidingRect, List<CollisionBody> possibleIntersectors)
        {
            CollisionBody[] possibleIntersectorsCopy = possibleIntersectors.ToArray();
            foreach (CollisionBody intersector in possibleIntersectorsCopy)
            {
                if (collidingRect.Intersects(intersector.hitbox))
                {
                    HandleAnyCollision();
                    HandleRectCollisions(intersector, intersector.collisionType);
                    break;
                }
            }
        }

        /// <summary>
        /// A method that gets called whenever any collision happens.
        /// </summary>
        /// <param name="collider"></param>
        public virtual void HandleAnyCollision()
        { }

        /// <summary>
        /// A method that gets called whenever a specfiic rectangle collision happens.
        /// </summary>
        /// <param name="collider"> The collider. </param>
        /// <param name="colliderType"> The collision type of the collider. </param>
        public virtual void HandleRectCollisions(CollisionBody collider, CollisionType colliderType)
        { }

        /// <summary>
        /// A method that gets called whenever a specific map object collision happens.
        /// </summary>
        /// <param name="collider">The collider.</param>
        public virtual void HandleMapObjectCollision(MapObject collider)
        { }
    }
}
