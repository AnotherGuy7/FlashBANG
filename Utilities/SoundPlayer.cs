using Microsoft.Xna.Framework.Audio;

namespace FlashBANG.Utilities
{
    public class SoundPlayer
    {
        public static SoundEffect[] sounds;

        public const int Sounds_DoorOpen = 0;
        public const int Sounds_DoorClose = 1;
        public const int Sounds_FlashlightClick = 2;

        /// <summary>
        /// Plays a sound effect stored in the sounds array.
        /// </summary>
        /// <param name="soundType">Type of sound. Sounds types are available in SoundPlayer.</param>
        public static void PlayLocalSound(int soundType, float pitch = 0f)
        {
            sounds[soundType].Play(Main.SFXVolume, pitch, 0f);
        }
    }
}
