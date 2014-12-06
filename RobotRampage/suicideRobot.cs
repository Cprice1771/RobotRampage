using FarseerPhysics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
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
        public bool MarkedForRemoval { get; set; }

        Texture2D texture;
        Texture2D emssionSpriteSheet;
        Rectangle srcRect;
        Rectangle destRect;
        Rectangle srcRectEmission;
        Rectangle destRectEmission;
        MainGame parent;
        RobotState state;
        PlayerDirection direction;
        Gun weapon;
        World myWorld;


        int frameSize;
        int frameCount;
        int frameIndex;
        int width;
        int height;
        float timeCounter;

        public SuicideRobot(Texture2D t, Texture2D et, MainGame game, World w)
            : base(w)
        {
            frameSize = 125;
            frameCount = 2;
            frameIndex = 0;
            width = 70;
            height = 60;
            timeCounter = 0.0f;
            texture = t;
            parent = game;
            Health = 100;
            state = RobotState.IDLE;
            direction = PlayerDirection.RIGHT;
            //frameCounter = 0;
            //frameRate = 1.0f / 24.0f;
            srcRect = new Rectangle(0, 0, width, height);
            destRect = new Rectangle((int)Position.X, (int)Position.Y, width, height);
            this.CreateFixture(new PolygonShape(PolygonTools.CreateRectangle(ConvertUnits.ToSimUnits(width / 2), ConvertUnits.ToSimUnits(height / 2)), 1.0f));
            this.BodyType = BodyType.Dynamic;
            this.Restitution = 0.3f;
            this.Friction = 1.0f;
            srcRectEmission = new Rectangle(0, 0, 30 , 50);
            destRectEmission = new Rectangle((int)Position.X + width, (int)Position.Y + height, 30, 50);
            myWorld = w;
            this.IgnoreGravity = true;
            emssionSpriteSheet = et;
            this.MarkedForRemoval = false;
            
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (this.LinearVelocity.X >= 0)
                direction = PlayerDirection.RIGHT;
            else
                direction = PlayerDirection.LEFT;

            //If on screen. This is bad, should do within range of player?
            if ((ConvertUnits.ToDisplayUnits(Position.Y)) + parent.CameraOffset.Y < MainGame.ScreenHeight && (ConvertUnits.ToDisplayUnits(Position.Y)) + parent.CameraOffset.Y > 0 &&
                (ConvertUnits.ToDisplayUnits(Position.X)) + parent.CameraOffset.X < MainGame.ScreenWidth && (ConvertUnits.ToDisplayUnits(Position.X)) + parent.CameraOffset.X > 0)
            {
                RunAtPlayerPlayer();
                state = RobotState.AGGRESIVE;
            }
            else
            {
                state = RobotState.IDLE;
                LinearVelocity = new Vector2(0, 0);
            }

            timeCounter += gameTime.ElapsedGameTime.Milliseconds/1000.0f;

            if (timeCounter > .04167f)
            {
                if (frameIndex < frameCount)
                    frameIndex++;
                else
                    frameIndex = 0;

                timeCounter -= .04167f;
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
            Vector2 offset = new Vector2(ConvertUnits.ToDisplayUnits(Position.X) - width / 2, ConvertUnits.ToDisplayUnits(Position.Y) - height / 2);

            int xpos = 30 + (frameIndex * frameSize);
            int ypos;
            if (state == RobotState.IDLE)
                ypos = 285;
            else
                ypos = 410;

            srcRect = new Rectangle(xpos, ypos, width, height);

            if (direction == PlayerDirection.LEFT)
                sb.Draw(this.texture, offset, srcRect, Color.White, Rotation, new Vector2(0, 0), 1.0f, SpriteEffects.None, 1.0f);
            else
                sb.Draw(this.texture, offset, srcRect, Color.White, Rotation, new Vector2(0, 0), 1.0f, SpriteEffects.FlipHorizontally, 1.0f);
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
