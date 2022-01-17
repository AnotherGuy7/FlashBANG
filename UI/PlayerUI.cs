using FlashBANG.Entities;
using FlashBANG.UI.UIElements;
using FlashBANG.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FlashBANG.UI
{
    public class PlayerUI : UIObject
    {
        public static Texture2D bulbTexture;
        public static Texture2D metalTexture;
        public static Texture2D itemTexturePanel;
        public static Texture2D craftMenuTexturePanel;
        public static Texture2D[] itemTextures;

        private readonly Vector2 bulbTextureSize = new Vector2(14, 14);
        private readonly Vector2 metalTextureSize = new Vector2(14, 14);
        private readonly Vector2 itemTexturePanelOrigin = new Vector2(1f, 13f);
        private readonly Vector2 craftMenuTexturePanelSize = new Vector2(260f, 150f);

        private bool craftMenuActive = false;
        private Button[] craftMenuButtons;
        private Text[] craftMenuText;
        private string[] craftMenuNotes = new string[5] { 
            "-Basic-\nRange: 3\nSpead: 45\nStrength: 1",
            "-Enchanced-\nRange: 5\nSpead: 65\nStrength: 3",
            "-Tri-beam-\nRange: 6\nSpead: 50\nStrength: 7",
            "-Light Laser-\nRange: 18\nSpead: 6\nStrength: 15",
            "-Heavy Laser-\nRange: 18\nSpead: 18\nStrength: 30"
        };
        private int[] requiredBulbs = new int[5] { 0, 3, 8, 14, 24 };
        private int[] requiredMetal = new int[5] { 0, 4, 10, 18, 26 };

        public static void NewPlayerUI()
        {
            PlayerUI playerUI = new PlayerUI();
            playerUI.Initialize();
            Main.activeUI.Add(playerUI);
        }

        public override void Initialize()
        {
            craftMenuButtons = new Button[5];
            craftMenuText = new Text[5];
            Vector2 startingPos = ToUICoords(Screen.center) + new Vector2(-84f, -24f);
            for (int i = 0; i < 5; i++)
            {
                craftMenuButtons[i] = new Button(startingPos + new Vector2(52f * i, 0f), 26, 26, 0.8f, 1.0f, Color.LightGray, Color.White, true);
                string note = craftMenuNotes[i];
                note += "\nMetal: " + requiredMetal[i];
                note += "\nBulbs:" + requiredBulbs[i];
                craftMenuText[i] = new Text(note, craftMenuButtons[i].buttonPosition + new Vector2(0f, 38f), Color.White, 0.6f);
            }
        }

        public override void Update()
        {
            if (InputManager.IsKeyJustPressed(Microsoft.Xna.Framework.Input.Keys.C))
                craftMenuActive = !craftMenuActive;

            if (craftMenuActive)
            {
                for (int i = 0; i < 5; i++)
                {
                    craftMenuButtons[i].Update();
                    if (!craftMenuButtons[i].buttonHover && (Player.player.heldBulbs < requiredBulbs[i] || Player.player.heldMetal < requiredMetal[i]))
                        craftMenuButtons[i].drawColor = Color.Red;

                    if (craftMenuButtons[i].buttonPressed && Player.player.heldBulbs >= requiredBulbs[i] && Player.player.heldMetal >= requiredMetal[i])
                    {
                        Player.player.heldBulbs -= requiredBulbs[i];
                        Player.player.heldMetal -= requiredMetal[i];
                        Player.player.heldFlashlightType = i;
                        Main.SwitchStageTo(i + 1);
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(bulbTexture, new Vector2(-1, -1) + (bulbTextureSize / 2f), Color.White);
            spriteBatch.DrawString(Main.mainFont, ": " + Player.player.heldBulbs, new Vector2(-4 + (bulbTextureSize.X * 2f), -2 + (bulbTextureSize.Y / 2f)), Color.White);

            spriteBatch.Draw(metalTexture, new Vector2(1, bulbTextureSize.Y + 1) + (metalTextureSize / 2f), Color.White);
            spriteBatch.DrawString(Main.mainFont, ": " + Player.player.heldMetal, new Vector2(-4 + (metalTextureSize.X * 2f), -2 + (metalTextureSize.Y + bulbTextureSize.Y / 2f)), Color.White);

            spriteBatch.Draw(itemTexturePanel, ToUICoords(Screen.topLeft) + new Vector2(0f, ToUISpace(Screen.resolutionHeight) / 2f), null, Color.White, 0f, itemTexturePanelOrigin, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(itemTextures[Player.player.heldFlashlightType], ToUICoords(Screen.topLeft) + new Vector2(0f, ToUISpace(Screen.resolutionHeight) / 2f), null, Color.White, 0f, itemTexturePanelOrigin, 1f, SpriteEffects.None, 0f);

            if (craftMenuActive)
            {
                Vector2 screenCenter = ToUICoords(Screen.center);
                Vector2 panelTopLeft = screenCenter - (craftMenuTexturePanelSize / 2f);
                spriteBatch.Draw(craftMenuTexturePanel, panelTopLeft, Color.White);
                for (int i = 0; i < 5; i++)
                {
                    craftMenuButtons[i].buttonPosition = panelTopLeft + new Vector2(12f, 32f) + new Vector2(48f * i, 0f);
                    craftMenuText[i].position = craftMenuButtons[i].buttonPosition + new Vector2(0f, 38f);
                    craftMenuButtons[i].Draw(spriteBatch);
                    spriteBatch.Draw(itemTextures[i], craftMenuButtons[i].buttonPosition - Vector2.One, null, craftMenuButtons[i].drawColor, 0f, Vector2.Zero, craftMenuButtons[i].scale, SpriteEffects.None, 0f);

                    craftMenuText[i].Draw(spriteBatch);

                    spriteBatch.DrawString(Main.mainFont, "CRAFT", panelTopLeft + new Vector2((craftMenuTexturePanelSize.X / 2f) - 46f, 1f), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
                }
            }

        }
    }
}
