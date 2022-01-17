using FlashBANG.Utilities;
using FlashBANG.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlashBANG.World.MapObjects
{
    public abstract class MapObject
    {
        public Rectangle hitbox;
        public Vector2 position;
        public bool interactionAvailable = false;
        public bool noCollision = false;
        public int visibiltyID = 0;

        public virtual void Initialize()
        { }

        public virtual void Update()
        { }

        public void CheckForInteractions(float requiredDistance)
        {
            interactionAvailable = Vector2.Distance(position, Player.player.position) <= requiredDistance;
            if (!interactionAvailable)
                return;

            if (hitbox.Contains(Main.mouseWorldPos) && InputManager.IsMouseRightJustClicked())
            {
                OnObjectInteraction();
            }
        }

        public virtual void OnObjectInteraction()
        { }

        public virtual void Draw(SpriteBatch spriteBatch)
        { }

        public void DestroyInstance()
        {
            Map.mapObjects.Remove(this);
        }
    }
}
