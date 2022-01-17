using FlashBANG.Entities;
using FlashBANG.UI.UIElements;
using FlashBANG.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlashBANG.UI
{
    public class EndScreen : UIObject
    {
        private int endTimer = 0;
        private Text statusText;

        public static void NewEndScreen()
        {
            EndScreen endScreen = new EndScreen();
            endScreen.Initialize();
            Main.uiScreen = endScreen;
        }

        public override void Initialize()
        {
            string text = "You've overcome THEM.";
            if (Main.gameLost)
                text = "THEY'VE overpowered you.";

            statusText = new Text(text, ToUICoords(Screen.center), Color.White, 1f, true);
            MusicPlayer.FadeOutInto(MusicPlayer.Music_None, 8 * 60, 2);
            Lighting.applyLighting = false;
        }

        public override void Update()
        {
            endTimer++;
            if (endTimer >= 8 * 60)
            {
                Main.gameState = Main.GameState.Title;
                MusicPlayer.FadeOutInto(MusicPlayer.Music_TitleMusic, 2, 2);
                TitleScreen.NewTitleScreen();
            }
            statusText.position = ToUICoords(Screen.center);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (endTimer >= 3 * 60)
                statusText.Draw(spriteBatch);
        }
    }
}
