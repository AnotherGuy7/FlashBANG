using FlashBANG.Effects;
using FlashBANG.Entities;
using FlashBANG.World;
using FlashBANG.World.MapObjects;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlashBANG.Utilities
{
    public class ContentLoader
    {
        public static ContentManager contentManager;

        public static void LoadContent(ContentManager manager)
        {
            contentManager = manager;

            LoadAllTextures();
            LoadAllSounds();
            LoadMisc();
        }

        private static void LoadAllTextures()
        {
            Player.playerIdle = LoadTex("Player/Player_Idle");
            Player.playerWalk = LoadTex("Player/Player_Walk");
            Player.flashlight = LoadTex("Player/Flashlight");

            Tile.tileTextures = new Texture2D[17];
            Tile.tileTextures[Tile.Tile_Void] = LoadTex("Tiles/Void");
            Tile.tileTextures[Tile.Tile_WoodenFloor] = LoadTex("Tiles/WoodTile");
            Tile.tileTextures[Tile.Tile_Wall_Bottom] = LoadTex("Tiles/WallBottom");
            Tile.tileTextures[Tile.Tile_Wall_Top] = LoadTex("Tiles/WallTop");
            Tile.tileTextures[Tile.Tile_BorderTop] = LoadTex("Tiles/Border_Top");
            Tile.tileTextures[Tile.Tile_BorderBottom] = LoadTex("Tiles/Border_Bottom");
            Tile.tileTextures[Tile.Tile_BorderLeft] = LoadTex("Tiles/Border_Left");
            Tile.tileTextures[Tile.Tile_BorderRight] = LoadTex("Tiles/Border_Right");
            Tile.tileTextures[Tile.Tile_BorderTopLeft] = LoadTex("Tiles/Border_TopLeft");
            Tile.tileTextures[Tile.Tile_BorderTopRight] = LoadTex("Tiles/Border_TopRight");
            Tile.tileTextures[Tile.Tile_BorderBottomLeft] = LoadTex("Tiles/Border_BottomLeft");
            Tile.tileTextures[Tile.Tile_BorderBottomRight] = LoadTex("Tiles/Border_BottomRight");
            Tile.tileTextures[Tile.Tile_Door] = LoadTex("Tiles/Door");
            Tile.tileTextures[Tile.Tile_DoorHorizontal] = LoadTex("Tiles/Door_Horizontal");
            Tile.tileTextures[Tile.Tile_RoomTile_1] = LoadTex("Tiles/RoomTile_1");
            Tile.tileTextures[Tile.Tile_RoomTile_2] = LoadTex("Tiles/RoomTile_2");
            Tile.tileTextures[Tile.Tile_RoomTile_3] = LoadTex("Tiles/RoomTile_3");

            Metal.metalTexture = LoadTex("Tiles/Interactables/Metal");
            Box.boxTexture = LoadTex("Tiles/Interactables/Box");
            Bulb.bulbTexture = LoadTex("Tiles/Interactables/Bulb");

            Lighting.lightTextures = new Texture2D[3];
            Lighting.lightTextures[Lighting.Texture_Blocker] = LoadTex("Misc/LightBlocker_1");
            Lighting.lightTextures[Lighting.Texture_LightRing] = LoadTex("Misc/Lightmask_1");
            Lighting.lightTextures[Lighting.Texture_Flashlight_1] = LoadTex("Misc/Lightmask_2");

            ShadowBall.shadowBallTexture = LoadTex("Enemies/ShadowBall");
            ShadowCube.shadowCubeTexture = LoadTex("Enemies/ShadowCube");

            Smoke.smokePixelTextures = new Texture2D[2];
            Smoke.smokePixelTextures[Smoke.WhitePixelTexture] = LoadTex("Misc/Pixel_1");
            Smoke.smokePixelTextures[Smoke.StarPixelTexture] = LoadTex("Misc/Pixel_2");
        }

        private static void LoadAllSounds()
        {
            SoundPlayer.sounds = new SoundEffect[3];
            SoundPlayer.sounds[SoundPlayer.Sounds_DoorOpen] = LoadSFX("Environment/DoorOpen");
            SoundPlayer.sounds[SoundPlayer.Sounds_DoorClose] = LoadSFX("Environment/DoorOpen");
            SoundPlayer.sounds[SoundPlayer.Sounds_FlashlightClick] = LoadSFX("FlashlightClick");

            MusicPlayer.gameMusic = new Song[6];
            //MusicPlayer.gameMusic[MusicPlayer.Music_TitleMusic] = LoadMusic("Music_Title");
            MusicPlayer.gameMusic[MusicPlayer.Music_Stage1] = LoadMusic("Music_Stage1");
            MusicPlayer.gameMusic[MusicPlayer.Music_Stage2] = LoadMusic("Music_Stage2_Slow");
            MusicPlayer.gameMusic[MusicPlayer.Music_Stage3] = LoadMusic("Music_Stage3_Slow");
            MusicPlayer.gameMusic[MusicPlayer.Music_Stage4] = LoadMusic("Music_Stage4_Slow");
            MusicPlayer.gameMusic[MusicPlayer.Music_Stage5] = LoadMusic("Music_Stage5_Slow");
        }

        private static void LoadMisc()
        {
            ShaderBatch.RGBDistortionEffect = LoadEffect("RGBDistortion");
            ShaderBatch.LightingEffect = LoadEffect("Lighting");
        }

        private static Texture2D LoadTex(string path)
        {
            return contentManager.Load<Texture2D>("Textures/" + path);
        }

        private static Effect LoadEffect(string path)
        {
            return contentManager.Load<Effect>("Effects/" + path);
        }

        private static SoundEffect LoadSFX(string path)
        {
            return contentManager.Load<SoundEffect>("Sounds/" + path);
        }

        private static Song LoadMusic(string name)
        {
            return contentManager.Load<Song>("Sounds/Music/" + name);
        }
    }
}
