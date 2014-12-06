using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobotRampage
{
    public class Explosion : IGameObject
    {
        Texture2D texture;
        MainGame parent;
        Rectangle srcRect;
        Rectangle destRect;
        float timeCounter;
        int frameCounter;
        Vector2 position;

        const int FRAME_COUNT = 5;

        public bool IsAlive { get; set; }
        public bool MarkedForRemoval { get; set; }

        public Explosion(Texture2D tex, MainGame game, Vector2 pos)
        {
            texture = tex;
            parent = game;
            position = pos;
            timeCounter = 0.0f;
            frameCounter = 0;
            srcRect = new Rectangle(0, 0, texture.Width / FRAME_COUNT, texture.Height);
            IsAlive = true;
            MarkedForRemoval = false;
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            timeCounter += gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            if (timeCounter > .04167f)
            {
                if (frameCounter < FRAME_COUNT - 1)
                    frameCounter++;
                else
                {
                    IsAlive = false;
                }
                timeCounter -= .04167f;
            }

            srcRect = new Rectangle(frameCounter * (texture.Width / FRAME_COUNT), 0, texture.Width / FRAME_COUNT, texture.Height);
        }

        public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            Vector2 offset = new Vector2(ConvertUnits.ToDisplayUnits(position.X) + 75, ConvertUnits.ToDisplayUnits(position.Y) - srcRect.Height / 2);
            sb.Draw(this.texture, offset, srcRect, Color.White, 0.0f, new Vector2(texture.Width / 2, texture.Height / 2), 1.0f, SpriteEffects.None, 1.0f);
        }

    }
}
