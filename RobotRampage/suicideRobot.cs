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
    public class SuicideRobot : Body, IGameObject, ILivingThing
    {
        Texture2D texture;
        Texture2D emssionSpriteSheet;
        Rectangle srcRect;
        Rectangle destRect;
        Rectangle srcRectEmission;
        Rectangle destRectEmission;
        MainGame parent;
        PlayerState state;
        PlayerDirection direction;
        Gun weapon;
        World myWorld;

        public SuicideRobot(Texture2D t, Texture2D et, MainGame game, World w)
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
            destRect = new Rectangle((int)Position.X, (int)Position.Y, texture.Width, texture.Height);

            srcRectEmission = new Rectangle(0, 0, 30 , 50);
            destRectEmission = new Rectangle((int)Position.X + texture.Width, (int)Position.Y + texture.Height, 30, 50);
            myWorld = w;

            emssionSpriteSheet = et;
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (this.LinearVelocity.X >= 0)
                direction = PlayerDirection.RIGHT;
            else
                direction = PlayerDirection.LEFT;

            //If on screen
            if ((ConvertUnits.ToDisplayUnits(Position.Y) + parent.CameraOffset.Y) < MainGame.ScreenHeight && (ConvertUnits.ToDisplayUnits(Position.Y) + parent.CameraOffset.Y) > 0 &&
                (ConvertUnits.ToDisplayUnits(Position.X) + parent.CameraOffset.X) < MainGame.ScreenWidth && (ConvertUnits.ToDisplayUnits(Position.X) + parent.CameraOffset.X) > 0)
            {
                RunAtPlayerPlayer();
            }
                
        }

        private void RunAtPlayerPlayer()
        {
            float x = (Position.X - parent.GetPlayerLocation().X);
            float y = (Position.Y - parent.GetPlayerLocation().Y);
            double angle = Math.Atan2(y, x);
            LinearVelocity = new Vector2((float)(-5 * Math.Cos(angle)), (float)(-5 * Math.Sin(angle)));
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
