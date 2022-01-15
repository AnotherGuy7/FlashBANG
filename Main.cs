using FlashBANG.Effects;
using FlashBANG.Entities;
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
        public RenderTarget2D screenTarget;
        public RenderTarget2D lightTarget;
        public RenderTarget2D blockerTarget;

        public static Vector2 mouseWorldPos;
        public static Vector2 mouseScreenPos;
        public static Vector2 mouseScreenDivision;
        public static Camera mainCamera;
        public static MusicPlayer musicPlayer;
        public static List<CollisionBody> activeEntities;
        public static List<Smoke> activeSmoke;

        public const float SFXVolume = 0.5f;
        public const float MusicVolume = 0.21f;

        public static int gameStage = 0;
        public static GameState gameState = GameState.Playing;

        public enum GameState
        {
            Title,
            Playing,
            Paused,
            GameOver
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
            screenTarget = new RenderTarget2D(GraphicsDevice, Screen.actualResolutionWidth, Screen.actualResolutionHeight);
            lightTarget = new RenderTarget2D(GraphicsDevice, Screen.actualResolutionWidth, Screen.actualResolutionHeight);
            blockerTarget = new RenderTarget2D(GraphicsDevice, Screen.actualResolutionWidth, Screen.actualResolutionHeight);
            ShaderBatch.InitializeShaderBatchLists();
            Lighting.InitializeLighting();

            mainCamera = new Camera();
            musicPlayer = new MusicPlayer();
            activeEntities = new List<CollisionBody>();
            activeSmoke = new List<Smoke>();
            Player player = new Player();
            player.Initialize();
            activeEntities.Add(player);
            //musicPlayer.FadeOutInto(MusicPlayer.Music_Stage1, 180);
            MediaPlayer.Play(MusicPlayer.gameMusic[MusicPlayer.Music_Stage3]);
            MediaPlayer.IsRepeating = true;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (gameState == GameState.Title)
            {

            }
            else if (gameState == GameState.Playing)
            {
                InputManager.UpdateInputStates();

                if (Map.map == null)
                    Map.CreateWorld();

                CollisionBody[] entitiesClone = activeEntities.ToArray();
                Smoke[] smokeClone = activeSmoke.ToArray();
                foreach (CollisionBody entity in entitiesClone)
                    entity.Update();
                foreach (Smoke smoke in smokeClone)
                    smoke.Update();

                Map.UpdateMap();
                EntitySpawner.UpdateSpawner();
            }
            else if (gameState == GameState.Paused)
            {

            }
            else if (gameState == GameState.GameOver)
            {

            }
            mainCamera.Update();
            musicPlayer.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            DrawGameScreenTarget();
            DrawLightingTargets();
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            ShaderBatch.LightingEffect.Parameters["lightMask"].SetValue(lightTarget);
            //ShaderBatch.LightingEffect.Parameters["blockerMask"].SetValue(blockerTarget);
            ShaderBatch.LightingEffect.Parameters["applyLighting"].SetValue(Lighting.applyLighting);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, ShaderBatch.LightingEffect);
            spriteBatch.Draw(screenTarget, new Rectangle(0, 0, Screen.actualResolutionWidth, Screen.actualResolutionHeight), Color.White);
            spriteBatch.End();

            ShaderBatch.DrawQueuedShaderDraws();
        }

        private void DrawGameScreenTarget()
        {
            GraphicsDevice.SetRenderTarget(screenTarget);
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, mainCamera.cameraMatrix);

            if (gameState == GameState.Title)
            {

            }
            else if (gameState == GameState.Playing)
            {
                Map.DrawMap(spriteBatch);

                CollisionBody[] entitiesClone = activeEntities.ToArray();
                Smoke[] smokeClone = activeSmoke.ToArray();
                foreach (CollisionBody entity in entitiesClone)
                    entity.Draw(spriteBatch);
                foreach (Smoke smoke in smokeClone)
                    smoke.Draw(spriteBatch);
            }
            else if (gameState == GameState.Paused)
            {

            }
            else if (gameState == GameState.GameOver)
            {

            }

            spriteBatch.End();
        }

        private void DrawLightingTargets()
        {
            if (!Lighting.applyLighting)
                return;

            GraphicsDevice.SetRenderTarget(blockerTarget);
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, mainCamera.cameraMatrix);
            Lighting.DrawBlockerMask(spriteBatch);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(lightTarget);
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, mainCamera.cameraMatrix);
            Lighting.DrawLightMask(spriteBatch);
            spriteBatch.End();
        }
    }
}
