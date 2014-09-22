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
    public class Robot : Body, IGameObject, ILivingThing
    {
        Texture2D texture;
        Rectangle srcRect;
        Rectangle destRect;
        MainGame parent;
        PlayerState state;
        PlayerDirection direction;
        Gun weapon;
        World myWorld;

        public Robot(Texture2D t, MainGame game, World w)
            : base(w)
        {
            texture = t;
            parent = game;
            Health = 100;
            state = PlayerState.IDLE;
            direction = PlayerDirection.RIGHT;
            //frameCounter = 0;
            //frameRate = 1.0f / 24.0f;
            srcRect = new Rectangle(0, 0, texture.Width, texture.Height);
            destRect = new Rectangle((int)Position.X, (int)Position.Y, 50, 50);
            myWorld = w;
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (this.LinearVelocity.X >= 0)
                direction = PlayerDirection.RIGHT;
            else
                direction = PlayerDirection.LEFT;
                
        }

        public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            Vector2 offset = new Vector2(ConvertUnits.ToDisplayUnits(Position.X) - texture.Width / 2, ConvertUnits.ToDisplayUnits(Position.Y) - texture.Height / 2);

            if (direction == PlayerDirection.LEFT)
                sb.Draw(this.texture, offset, srcRect, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.FlipHorizontally, 1.0f);
            else
                sb.Draw(this.texture, offset, srcRect, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 1.0f);
        }

        public int Health
        {
            get;
            private set;
        }

        public void DealDamage(int damage)
        {
            Health -= damage;
        }
    }
}
