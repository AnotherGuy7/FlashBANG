using FlashBANG.World;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlashBANG.Utilities
{
    public class Camera
    {
        public Vector2 position;
        public Vector2 cameraOrigin;
        public float cameraRotation;
        public Matrix cameraMatrix;
        public bool cameraLocked = false;

        public Vector2 cameraShakeOffset;
        public int cameraShakeStrength = 0;
        public int cameraShakeTimer = 0;

        /// <summary>
        /// Changes the settings of the Camera to allow it to properly show the Player and World.
        /// </summary>
        public void SetToPlayerCamera()
        {
            cameraOrigin = new Vector2(Screen.actualResolutionWidth, Screen.actualResolutionHeight) / 2f;
            position = cameraOrigin / 3f;       //3f because of the Screen matrix scale
        }

        public void Update()
        {
            if (cameraLocked)
                return;

            //ManageCameraShake();
            UpdateCameraView();
        }

        /*public void ShakeCamera(int shakeStrength, int duration)
        {
            cameraShakeTimer = duration;
            cameraShakeStrength = shakeStrength;
        }

        private void ManageCameraShake()
        {
            if (cameraShakeTimer > 0)
            {
                cameraShakeTimer--;
                float offsetX = Main.random.Next(-cameraShakeStrength, cameraShakeStrength + 1);
                float offsetY = Main.random.Next(-cameraShakeStrength, cameraShakeStrength + 1);
                position += new Vector2(offsetX, offsetY);
            }
        }*/

        public void UpdateCamera(Vector2 cameraPosition)
        {
            position = cameraPosition;

            position.X = MathHelper.Clamp(position.X, Screen.halfScreenWidth + 1, (Map.MapWidth * 16f) - Screen.halfScreenWidth - 1);
            position.Y = MathHelper.Clamp(position.Y, Screen.halfScreenHeight + 1, (Map.MapHeight * 16f) - Screen.halfScreenHeight - 1);

            Vector2 mouseOffset = Main.mouseWorldPos - position;
            mouseOffset.Normalize();
            mouseOffset *= (Vector2.Distance(Main.mouseWorldPos, position) * 12f) / Screen.actualResolutionHeight;
            //position += mouseOffset;
        }

        public void UpdateCameraView()
        {
            cameraMatrix = Matrix.CreateTranslation(new Vector3(-position.X, -position.Y, 0)) * Matrix.CreateRotationZ(cameraRotation) * Matrix.CreateScale(3f)
                * Matrix.CreateTranslation(new Vector3(cameraOrigin.X, cameraOrigin.Y, 0));
        }
    }
}
