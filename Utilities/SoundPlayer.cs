using Microsoft.Xna.Framework.Audio;

namespace FlashBANG.Utilities
{
    public class SoundPlayer
    {
        public static SoundEffect[] sounds;

        public const int Sounds_DoorOpen = 0;
        public const int Sounds_DoorClose = 1;
        public const int Sounds_FlashlightClick = 2;
        public const int Sounds_MetalPickUp = 3;
        public const int Sounds_BulbPickUp = 4;
        public const int Sounds_HumAmbience = 5;
        public const int Sounds_RandomSound_1 = 6;
        public const int Sounds_RandomSound_2 = 7;
        public const int Sounds_RandomSound_3 = 8;
        public const int Sounds_RandomSound_4 = 9;
        public const int Sounds_ButtonHover = 10;
        public const int Sounds_ThingScream = 11;

        public static SoundEffectInstance humSound;

        /// <summary>
        /// Plays a sound effect stored in the sounds array.
        /// </summary>
        /// <param name="soundType">Type of sound. Sounds types are available in SoundPlayer.</param>
        public static void PlayLocalSound(int soundType, float pitch = 0f, float pan = 0f, float customVolume = Main.SFXVolume)
        {
            sounds[soundType].Play(customVolume, pitch, pan);
        }

        public static void PlayRandomAmbienceSound()
        {
            int type = Main.random.Next(Sounds_RandomSound_1, Sounds_RandomSound_4 + 1);
            float pitch = Main.random.Next(-5, 5 + 1) / 10f;
            float pan = Main.random.Next(-10, 10 + 1) / 10f;
            float volumeRand = Main.random.Next(-2, 2 + 1) / 10f;
            PlayLocalSound(type, pitch, pan, 0.5f + volumeRand);
        }

        public static void LoopHum()
        {
            if (humSound.State != SoundState.Playing)
                humSound.Play();
        }
    }
}
