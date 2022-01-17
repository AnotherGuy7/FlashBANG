using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;

namespace FlashBANG.Utilities
{
    public class MusicPlayer
    {
        public const int AmountOfMusic = 3;
        public const int Music_None = -1;
        public const int Music_TitleMusic = 0;
        public const int Music_Stage1 = 1;
        public const int Music_Stage2 = 2;
        public const int Music_Stage3 = 3;
        public const int Music_Stage4 = 4;
        public const int Music_Stage5 = 5;
        public static Song[] gameMusic;

        private bool fadingOut = false;
        private int fadeOutDuration = 0;
        private int fadeOutTimer = 0;
        private int nextSongIndex = 0;
        private int activeSongIndex = 0;

        private bool fadingIn = false;
        private int fadeInDuration = 0;
        private int fadeInTimer = 0;

        public void Update()
        {
            if (fadingOut)
                FadeOut();

            if (fadingIn)
                FadeIn();

            if (!fadingOut && !fadingIn)
            {
                MediaPlayer.Volume = Main.MusicVolume;
            }

            if (MediaPlayer.Volume <= 0.01f)
                return;

            if (MediaPlayer.State == MediaState.Playing)
                return;

            MediaPlayer.Play(gameMusic[activeSongIndex]);
        }

        public static void FadeOutInto(int songIndex, int fadeOutTime, int fadeInTime = 180)
        {
            Main.musicPlayer.nextSongIndex = songIndex;
            Main.musicPlayer.fadingOut = true;
            Main.musicPlayer.fadeOutDuration = fadeOutTime;
            Main.musicPlayer.fadeInDuration = fadeInTime;
        }

        public void FadeOut()
        {
            if (fadeOutTimer < fadeOutDuration)
                fadeOutTimer++;

            MediaPlayer.Volume = (((float)fadeOutDuration - (float)fadeOutTimer) / (float)fadeOutDuration) * Main.MusicVolume;
            MediaPlayer.IsRepeating = false;

            if (fadeOutTimer >= fadeOutDuration)
            {
                fadingOut = false;
                fadingIn = true;
                fadeOutTimer = 0;
                fadeOutDuration = 0;

                if (nextSongIndex == Music_None)
                {
                    MediaPlayer.Stop();
                }
                else
                {
                    activeSongIndex = nextSongIndex;
                    MediaPlayer.Play(gameMusic[activeSongIndex]);
                }
            }
        }

        public void FadeIn()
        {
            if (fadeInTimer < fadeInDuration)
                fadeInTimer++;

            MediaPlayer.Volume = ((float)fadeInTimer / (float)fadeInDuration) * Main.MusicVolume;

            if (fadeInTimer >= fadeInDuration)
            {
                fadingIn = false;
                fadeInTimer = 0;
                fadeInDuration = 0;
            }
        }
    }
}