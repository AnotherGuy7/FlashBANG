using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlashBANG.Utilities
{
    public class Screen
    {
        public readonly GameWindow Window;

        public static int resolutionWidth = 640;
        public static int resolutionHeight = 512;

        public static int halfScreenWidth;
        public static int halfScreenHeight;

        public static Vector2 topLeft;
        public static Vector2 topRight;
        public static Vector2 center;
        public static Vector2 bottomLeft;
        public static Vector2 bottomRight;

        /// <summary>
        /// Initializes and sets up the Screen.
        /// </summary>
        /// <param name="window"></param>
        public Screen(GameWindow window)
        {
            Window = window;
            SetupScreen();
            SetupScreenPositions();
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += ResizeScreen;
        }

        public void SetupScreen()
        {
            Main._graphics.PreferredBackBufferWidth = resolutionWidth;
            Main._graphics.PreferredBackBufferHeight = resolutionHeight;
            Main._graphics.ApplyChanges();
        }

        public void SetupScreenPositions()
        {
            topLeft = Vector2.Zero;
            topRight = new Vector2(resolutionWidth, 0f);
            center = new Vector2(resolutionWidth, resolutionHeight) / 2f;
            bottomLeft = new Vector2(0f, resolutionHeight);
            bottomRight = new Vector2(resolutionWidth, resolutionHeight);
        }

        public void ResizeScreen(object sender, EventArgs args)
        {
            resolutionWidth = Window.ClientBounds.Width;
            resolutionHeight = Window.ClientBounds.Height;
            //halfScreenWidth = desiredResolutionWidth / 2;
            //halfScreenHeight = desiredResolutionHeight / 2;

            Main.RecreateRenderTargets();
            SetupScreenPositions();
            //Main.mouseScreenDivision = new Vector2(matrixX, matrixY);

            Main.mainCamera.SetToPlayerCamera();
        }
    }
}
