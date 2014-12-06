using FarseerPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobotRampage
{
    public class HealthBar : IGameObject
    {
        public bool MarkedForRemoval { get; set; }
        public int Health { get; set; }

        Texture2D texture;
        MainGame parent;
        Rectangle srcRect;
        public Vector2 Position {get; set;}

        const int FRAME_COUNT = 11;
        const int FRAME_SIZE = 37;

        

        public HealthBar(Texture2D t, MainGame game)
        {
            texture = t;
            parent = game;
            srcRect = new Rectangle(0, 0, texture.Width, texture.Height / FRAME_COUNT);
            this.MarkedForRemoval = false;
            Position = Vector2.Zero;
        }
        
       

        public void Update(GameTime gameTime)
        {
 	        //Do nothing
        }

        public void Draw(SpriteBatch sb)
        {

                //if(Health >= 100)
                //    srcRect = new Rectangle(0, 0 * FRAME_SIZE, texture.Width, texture.Height / FRAME_COUNT);
                //else if(Health > 90 )
                //    srcRect = new Rectangle(0, 1 * FRAME_SIZE, texture.Width, texture.Height / FRAME_COUNT);
                //else if(Health > 80)
                //    srcRect = new Rectangle(0, 2 * FRAME_SIZE, texture.Width, texture.Height / FRAME_COUNT);
                //else if(Health > 70)
                //    srcRect = new Rectangle(0, 3 * FRAME_SIZE, texture.Width, texture.Height / FRAME_COUNT);
                //else if(Health > 60)
                //    srcRect = new Rectangle(0, 4 * FRAME_SIZE, texture.Width, texture.Height / FRAME_COUNT);
                //else if(Health > 50)
                //    srcRect = new Rectangle(0, 5 * FRAME_SIZE, texture.Width, texture.Height / FRAME_COUNT);
                //else if(Health > 40)
                //    srcRect = new Rectangle(0, 6 * FRAME_SIZE, texture.Width, texture.Height / FRAME_COUNT);
                //else if(Health > 30)
                //    srcRect = new Rectangle(0, 7 * FRAME_SIZE, texture.Width, texture.Height / FRAME_COUNT);
                //else if(Health > 20)
                //    srcRect = new Rectangle(0, 8 * FRAME_SIZE, texture.Width, texture.Height / FRAME_COUNT);
                //else if(Health > 10)
                //    srcRect = new Rectangle(0, 9 * FRAME_SIZE, texture.Width, texture.Height / FRAME_COUNT);
                //else
                //    srcRect = new Rectangle(0, 10 * FRAME_SIZE, texture.Width, texture.Height / FRAME_COUNT);

            srcRect = new Rectangle(0, (10 - (Health / 10)) * FRAME_SIZE, texture.Width, FRAME_SIZE);
                    
            Vector2 offset = new Vector2(ConvertUnits.ToDisplayUnits(Position.X), ConvertUnits.ToDisplayUnits(Position.Y));
            sb.Draw(this.texture, Position, srcRect, Color.White, 0.0f, new Vector2(texture.Width / 2, FRAME_SIZE / 2), 1.0f, SpriteEffects.None, 1.0f);
        }
    }
}
