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
    public class Robot : Body, IGameObject, ILivingThing
    {
        public bool MarkedForRemoval { get; set; }

        Texture2D texture;
        Rectangle srcRect;
        Rectangle destRect;
        MainGame parent;
        PlayerState state;
        PlayerDirection direction;
        Gun weapon;
        World myWorld;
        double lastFire;
        double fireRate;

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
            lastFire = -1.0;
            fireRate = 1000;
            this.CreateFixture(new PolygonShape(PolygonTools.CreateRectangle(ConvertUnits.ToSimUnits(t.Width / 2), ConvertUnits.ToSimUnits(t.Height / 2)), 1.0f));
            this.BodyType = BodyType.Dynamic;
            this.Restitution = 0.3f;
            this.Friction = 1.0f;
            this.IgnoreGravity = true;
            this.MarkedForRemoval = false;
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            lastFire += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (this.LinearVelocity.X >= 0)
                direction = PlayerDirection.RIGHT;
            else
                direction = PlayerDirection.LEFT;

            if ((ConvertUnits.ToDisplayUnits(Position.Y) + parent.CameraOffset.Y) < MainGame.ScreenHeight && (ConvertUnits.ToDisplayUnits(Position.Y) + parent.CameraOffset.Y) > 0 &&
                (ConvertUnits.ToDisplayUnits(Position.X) + parent.CameraOffset.X) < MainGame.ScreenWidth && (ConvertUnits.ToDisplayUnits(Position.X) + parent.CameraOffset.X) > 0 &&
                (lastFire == -1.0f ||lastFire > fireRate))     
                {
                    lastFire = gameTime.ElapsedGameTime.TotalMilliseconds;
                    ShootPlayer();  
                }
        }

        private void ShootPlayer()
        {
            float x = (Position.X - parent.GetPlayerLocation().X);
            float y = (Position.Y - parent.GetPlayerLocation().Y);
            parent.CreateLaser(10, (float)Math.Atan2(y, x), 5f, Position);
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
