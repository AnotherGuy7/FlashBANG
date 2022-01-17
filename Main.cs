using FlashBANG.Effects;
using FlashBANG.Entities;
using FlashBANG.UI;
using FlashBANG.Utilities;
using FlashBANG.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace FlashBANG
{
    public class Main : Game
    {
        public static SpriteBatch spriteBatch;
        public static GraphicsDeviceManager _graphics;
        public static Random random;
        public static Screen gameScreen;
        public static SpriteFont mainFont;
        private static RenderTarget2D screenTarget;
        private static RenderTarget2D lightTarget;
        private static RenderTarget2D blockerTarget;

        public static bool mouseOverUI = false;
        public static Vector2 mouseWorldPos;
        public static Vector2 mouseScreenPos;
        public static Vector2 mouseUIPos;
        public static Vector2 mouseScreenDivision;
        public static Camera mainCamera;
        public static MusicPlayer musicPlayer;
        public static List<CollisionBody> activeEntities;
        public static List<Smoke> activeSmoke;
        public static List<UIObject> activeUI;
        public static UIObject uiScreen;

        public const float SFXVolume = 0.4f;
        public const float MusicVolume = 0.11f;

        public static int gameStage = 1;
        public static GameState gameState = GameState.Title;
        public static bool gameLost = false;
        public static bool queueExit = false;
        public static float fadeStrength = 1.8f;

        public enum GameState
        {
            Title,
            Playing,
            Paused,
            End
        }

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ContentLoader.LoadContent(Content);
            random = new Random();
            gameScreen = new Screen(Window);
            screenTarget = new RenderTarget2D(GraphicsDevice, Screen.resolutionWidth, Screen.resolutionHeight);
            lightTarget = new RenderTarget2D(GraphicsDevice, Screen.resolutionWidth, Screen.resolutionHeight);
            blockerTarget = new RenderTarget2D(GraphicsDevice, Screen.resolutionWidth, Screen.resolutionHeight);
            ShaderBatch.InitializeShaderBatchLists();
            Lighting.InitializeLighting();
            Lighting.applyLighting = false;

            mainCamera = new Camera();
            musicPlayer = new MusicPlayer();
            TitleScreen.NewTitleScreen();
            MediaPlayer.Play(MusicPlayer.gameMusic[MusicPlayer.Music_Stage1]);
            MediaPlayer.IsRepeating = true;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (gameState == GameState.Title)
            {
                uiScreen.Update();
            }
            else if (gameState == GameState.Playing)
            {
                CollisionBody[] entitiesClone = activeEntities.ToArray();
                Smoke[] smokeClone = activeSmoke.ToArray();
                UIObject[] uiClone = activeUI.ToArray();
                foreach (CollisionBody entity in entitiesClone)
                    entity.Update();
                foreach (Smoke smoke in smokeClone)
                    smoke.Update();
                foreach (UIObject ui in uiClone)
                    ui.Update();

                Map.UpdateMap();
                EntitySpawner.UpdateSpawner();
            }
            /*else if (gameState == GameState.Paused)
            {

            }*/
            else if (gameState == GameState.End)
            {
                uiScreen.Update();
            }

            mainCamera.Update();
            musicPlayer.Update();
            InputManager.UpdateInputStates();
            if (queueExit)
                Exit();
        }

        protected override void Draw(GameTime gameTime)
        {
            DrawGameScreenTarget();
            DrawLightingTargets();
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            ShaderBatch.LightingEffect.Parameters["lightMask"].SetValue(lightTarget);
            ShaderBatch.LightingEffect.Parameters["blockerMask"].SetValue(blockerTarget);
            ShaderBatch.LightingEffect.Parameters["applyLighting"].SetValue(Lighting.applyLighting);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, ShaderBatch.LightingEffect);
            spriteBatch.Draw(screenTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            ShaderBatch.DrawQueuedShaderDraws();

            DrawUI();
        }

        private void DrawGameScreenTarget()
        {
            GraphicsDevice.SetRenderTarget(screenTarget);
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, mainCamera.cameraMatrix);

            if (gameState == GameState.Playing)
            {
                Map.DrawMap(spriteBatch);

                CollisionBody[] entitiesClone = activeEntities.ToArray();
                Smoke[] smokeClone = activeSmoke.ToArray();
                foreach (CollisionBody entity in entitiesClone)
                    entity.Draw(spriteBatch);
                foreach (Smoke smoke in smokeClone)
                    smoke.Draw(spriteBatch);
            }
            /*else if (gameState == GameState.Paused)
            {

            }*/

            spriteBatch.End();
        }

        private void DrawLightingTargets()
        {
            if (!Lighting.applyLighting)
                return;

            GraphicsDevice.SetRenderTarget(blockerTarget);
            GraphicsDevice.Clear(Color.Red);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, mainCamera.cameraMatrix);
            Lighting.DrawBlockerMask(spriteBatch);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(lightTarget);
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, mainCamera.cameraMatrix);
            Lighting.DrawLightMask(spriteBatch);
            spriteBatch.End();
        }

        private void DrawUI()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Matrix.CreateScale(2f));

            if (gameState == GameState.Title || gameState == GameState.End)
            {
                uiScreen.Draw(spriteBatch);
            }
            else if (gameState == GameState.Playing)
            {
                UIObject[] uiClone = activeUI.ToArray();
                foreach (UIObject ui in uiClone)
                    ui.Draw(spriteBatch);
            }

            spriteBatch.End();
        }

        public static void RecreateRenderTargets()
        {
            screenTarget = new RenderTarget2D(_graphics.GraphicsDevice, Screen.resolutionWidth, Screen.resolutionHeight);
            lightTarget = new RenderTarget2D(_graphics.GraphicsDevice, Screen.resolutionWidth, Screen.resolutionHeight);
            blockerTarget = new RenderTarget2D(_graphics.GraphicsDevice, Screen.resolutionWidth, Screen.resolutionHeight);
        }

        public static void StartGame()
        {
            gameStage = 1;
            gameState = GameState.Playing;
            gameLost = false;
            activeEntities = new List<CollisionBody>();
            activeSmoke = new List<Smoke>();
            activeUI = new List<UIObject>();
            Map.CreateWorld();

            Player player = new Player();
            player.Initialize();
            Player.player.position = Map.playerSpawnPoint.ToVector2() * 16f;
            activeEntities.Add(player);
            MusicPlayer.FadeOutInto(MusicPlayer.Music_Stage1, 180);
            Lighting.applyLighting = true;
        }

        public static void ExitGame()
        {
            queueExit = true;
        }

        public static void SwitchStageTo(int stage)
        {
            gameStage = stage;
            if (stage == 1)
                MusicPlayer.FadeOutInto(MusicPlayer.Music_Stage1, 30, 30);
            else if (stage == 2)
                MusicPlayer.FadeOutInto(MusicPlayer.Music_Stage2, 30, 30);
            else if (stage == 3)
                MusicPlayer.FadeOutInto(MusicPlayer.Music_Stage3, 30, 30);
            else if (stage == 4)
                MusicPlayer.FadeOutInto(MusicPlayer.Music_Stage4, 30, 30);
            else if (stage == 5)
            {
                MusicPlayer.FadeOutInto(MusicPlayer.Music_Stage5, 30, 30);
                SoundPlayer.PlayLocalSound(SoundPlayer.Sounds_ThingScream, customVolume: 0.5f);
            }
        }
    }
}
