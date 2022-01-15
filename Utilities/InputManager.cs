using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using FlashBANG.Entities;

namespace FlashBANG.Utilities
{
    public class InputManager
    {
        public static KeyboardState currentKeyboardState;
        public static KeyboardState previousKeyboardState;
        public static MouseState currentMouseState;
        public static MouseState previousMouseState;

        public static Keys upKey = Keys.W;
        public static Keys downKey = Keys.S;
        public static Keys leftKey = Keys.A;
        public static Keys rightKey = Keys.D;
        public static Keys dashKey = Keys.E;
        public static Keys potionKey = Keys.H;
        public static Keys vialKey = Keys.G;

        public static bool IsKeyPressed(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key);
        }

        public static bool IsKeyJustPressed(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key) && !previousKeyboardState.IsKeyDown(key);
        }

        public static bool IsMouseLeftHeld()
        {
            return currentMouseState.LeftButton == ButtonState.Pressed;
        }

        public static bool IsMouseLeftJustClicked()
        {
            return currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton != ButtonState.Pressed;
        }

        public static bool IsMouseLeftJustReleased()
        {
            return currentMouseState.LeftButton != ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Pressed;
        }

        public static bool IsMouseRightJustClicked()
        {
            return currentMouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton != ButtonState.Pressed;
        }

        public static bool IsMouseRightJustReleased()
        {
            return currentMouseState.RightButton != ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Pressed;
        }

        public static void UpdateInputStates()
        {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
            Main.mouseScreenPos = (currentMouseState.Position.ToVector2() / 3f) - (Main.mainCamera.cameraOrigin / 3f);
            Main.mouseWorldPos = Player.player.position + Main.mouseScreenPos;
        }
    }
}
