using FlashBANG.UI.UIElements;
using FlashBANG.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlashBANG.UI
{
    public class TitleScreen : UIObject
    {
        public static Texture2D controlsPanel;

        public TextButton playButton;
        public TextButton controlButton;
        public TextButton quitButton;
        private Text controlPanelText;

        private bool controlsPanelShowing = false;
        private readonly Vector2 controlPanelSize = new Vector2(180f, 90f);

        public static void NewTitleScreen()
        {
            TitleScreen titleScreen = new TitleScreen();
            titleScreen.Initialize();
            Main.uiScreen = titleScreen;
        }

        public override void Initialize()
        {
            Main.gameState = Main.GameState.Title;

            playButton = new TextButton("Start", ToUICoords(Screen.topLeft) + new Vector2(6f, 42f), 0.8f, 1.0f, Color.Gray, Color.White);
            controlButton = new TextButton("Controls", playButton.buttonPosition + new Vector2(0f, 16f), 0.8f, 1.0f, Color.Gray, Color.White);
            quitButton = new TextButton("Quit", playButton.buttonPosition + new Vector2(0f, 32f), 0.8f, 1.0f, Color.DarkRed, Color.Red);

            string controlsText = 
                "W- Move Up\n" +
                "A- Move Left\n" +
                "S- Move Down\n" +
                "D- Move Right\n" +
                "C- Crafts Panel\n" +
                "Left-Click- Use Flashlight\n" +
                "Right-Click- Pick up objects\n" +
                "\n  *Press Controls again to leave this menu*";
            Vector2 controlsPanelPos = ToUICoords(Screen.center) + new Vector2(-controlPanelSize.X, -controlPanelSize.Y + 42f);
            controlPanelText = new Text(controlsText, controlsPanelPos, Color.White, 0.6f, true);
        }

        public override void Update()
        {
            playButton.Update();
            controlButton.Update();
            quitButton.Update();

            playButton.buttonPosition = ToUICoords(Screen.topLeft) + new Vector2(6f, + 42f);
            controlButton.buttonPosition = playButton.buttonPosition + new Vector2(0f, 14f);
            quitButton.buttonPosition = playButton.buttonPosition + new Vector2(0f, 28f);
            controlPanelText.position = ToUICoords(Screen.center);

            if (playButton.buttonPressed)
            {
                Main.StartGame();
            }

            if (controlButton.buttonPressed)
            {
                controlsPanelShowing = !controlsPanelShowing;
            }

            if (quitButton.buttonPressed)
            {
                Main.ExitGame();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 textPos = ToUICoords(Screen.topLeft) + new Vector2(Screen.resolutionWidth / 8f, 0f);
            spriteBatch.DrawString(Main.mainFont, "FlashBANG", textPos, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);

            playButton.Draw(spriteBatch);
            controlButton.Draw(spriteBatch);
            quitButton.Draw(spriteBatch);

            if (controlsPanelShowing)
            {
                spriteBatch.Draw(controlsPanel, ToUICoords(Screen.center) - (controlPanelSize / 2f), Color.White);
                controlPanelText.Draw(spriteBatch);
            }
        }
    }
}
