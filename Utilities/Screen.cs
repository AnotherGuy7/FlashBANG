using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlashBANG.Utilities
{
    public class Screen
    {
        public static Matrix screenMatrix;

        private readonly GameWindow Window;

        public static int actualResolutionWidth = 640;
        public static int actualResolutionHeight = 512;
        public static int desiredResolutionWidth = 320;
        public static int desiredResolutionHeight = 256;

        public static int halfScreenWidth;
        public static int halfScreenHeight;

        /// <summary>
        /// Initializes and sets up the Screen.
        /// </summary>
        /// <param name="window"></param>
        public Screen(GameWindow window)
        {
            Window = window;
            SetupScreen();
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += ResizeScreen;
        }

        public void SetupScreen()
        {
            Main._graphics.PreferredBackBufferWidth = actualResolutionWidth;
            Main._graphics.PreferredBackBufferHeight = actualResolutionHeight;
            Main._graphics.ApplyChanges();
        }

        public void ResizeScreen(object sender, EventArgs args)
        {
            actualResolutionWidth = Window.ClientBounds.Width;
            actualResolutionHeight = Window.ClientBounds.Height;

            float matrixX = actualResolutionWidth / (float)desiredResolutionWidth;
            float matrixY = actualResolutionHeight / (float)desiredResolutionHeight;

            halfScreenWidth = desiredResolutionWidth / 2;
            halfScreenHeight = desiredResolutionHeight / 2;

            screenMatrix = Matrix.CreateScale(matrixX, matrixY, 1f);
            Main.mouseScreenDivision = new Vector2(matrixX, matrixY);
        }
    }
}
