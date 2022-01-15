using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FlashBANG.Effects
{
    public class Smoke
    {
        public static Texture2D[] smokePixelTextures;
        public static readonly int[] smokeTextureSize = new int[3] { 2, 3, 5 };

        public const int WhitePixelTexture = 0;
        public const int StarPixelTexture = 1;
        public const int CirclePixelTexture = 2;

        private int smokeTextureType;
        private Vector2 position;
        private Vector2 velocity = Vector2.Zero;
        private Color smokeColor = Color.White;
        private float rotation;
        private float rotationToAdd;
        private float scale;
        private float drawAlpha = 1f;
        private Vector2 smokeOrigin;

        private int colorChangeTimerStart = 0;
        private int colorChangeTimer = 0;
        private int lifeTimer = 0;
        private int fadeTime = 0;
        private Color startColor;
        private Color endColor;

        /// <summary>
        /// Spawns a new smoke particle with the given information.
        /// </summary>
        /// <param name="position">The position the smoke will spawn in.</param>
        /// <param name="velocity">The velocity the smoke will retain (The velocity doesn't dampen!)</param>
        /// <param name="startColor">The color of the smoke particle before its life timer surpasses it's color change timer.</param>
        /// <param name="endColor">The color the smoke will fade into once the life timer surpasses it's color change timer.</param>
        /// <param name="colorChangeTime">The amount of time it will take for the color change effect to take place</param>
        /// <param name="lifeTime">The amount of time (in frames) that the smoke will remain in-game for.</param>
        /// <param name="fadeTime">The time it takes for the smoke to start fading away.</param>
        /// <param name="scale">The scale of the smoke.</param>
        /// <param name="rotation">The rotation of the smoke, in Degrees, that will continually be added to the smoke.</param>
        /// <param name="textureType">The type of texture the smoke will use.</param>
        public static void NewSmokeParticle(Vector2 position, Vector2 velocity, Color startColor, Color endColor, int colorChangeTime, int lifeTime, int fadeTime = 60, float scale = 0.4f, float rotation = 0f, int textureType = 0)
        {
            Smoke newInstance = new Smoke();
            newInstance.position = position;
            newInstance.velocity = velocity;
            newInstance.rotationToAdd = MathHelper.ToRadians(rotation);
            newInstance.scale = scale;
            newInstance.smokeColor = startColor;
            newInstance.startColor = startColor;
            newInstance.endColor = endColor;
            newInstance.lifeTimer = lifeTime;
            newInstance.fadeTime = fadeTime;
            newInstance.colorChangeTimer = colorChangeTime;
            newInstance.colorChangeTimerStart = colorChangeTime;
            newInstance.smokeTextureType = textureType;
            if (rotation != 0f)
                newInstance.smokeOrigin = new Vector2(smokeTextureSize[textureType] / 2f, smokeTextureSize[textureType] / 2f);

            Main.activeSmoke.Add(newInstance);
        }

        public void Update()
        {
            lifeTimer--;
            if (lifeTimer <= fadeTime)
                drawAlpha = (float)lifeTimer / (float)fadeTime;

            if (lifeTimer <= 0)
                DestroyInstance();

            position += velocity;
            rotation += rotationToAdd;

            if (colorChangeTimer > 0)
            {
                colorChangeTimer--;
                smokeColor.R = (byte)MathHelper.Lerp(startColor.R, endColor.R, 1f - ((float)colorChangeTimer / (float)colorChangeTimerStart));
                smokeColor.G = (byte)MathHelper.Lerp(startColor.G, endColor.G, 1f - ((float)colorChangeTimer / (float)colorChangeTimerStart));
                smokeColor.B = (byte)MathHelper.Lerp(startColor.B, endColor.B, 1f - ((float)colorChangeTimer / (float)colorChangeTimerStart));
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(smokePixelTextures[smokeTextureType], position, null, smokeColor * drawAlpha, rotation, smokeOrigin, scale, SpriteEffects.None, 0f);
        }

        private void DestroyInstance()
        {
            Main.activeSmoke.Remove(this);
        }
    }
}