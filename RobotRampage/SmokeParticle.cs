using FarseerPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobotRampage
{
    public class SmokeParticle : IGameObject
    {
        public bool MarkedForRemoval { get; set; }
        public bool IsAlive { get; set; }

        Texture2D texture;
        Rectangle rectangle;
        Vector2 position, midpoint;
        float scale;
        MainGame parent;
        int age;
        Color color;

        public SmokeParticle(MainGame g, Texture2D tex, Vector2 pos)
        {
            parent = g;
            texture = tex;
            position = pos;

            rectangle = new Rectangle((int)pos.X, (int)pos.Y, texture.Width / 4, texture.Height / 4);
            midpoint = new Vector2(rectangle.Width / 2, rectangle.Height / 2);
            scale = 0.5f;
            IsAlive = true;

            age = 255;
            
        }

        public void Draw(SpriteBatch sb)
        {
            //Vector2 offset = new Vector2(ConvertUnits.ToDisplayUnits(position.X), ConvertUnits.ToDisplayUnits(position.Y));
            color = new Color(255, 255, 255, age);
            sb.Draw(texture, position, null, color, 0.0f, midpoint, scale + ((0.5f) * ((float)(255 - age) / 255)), SpriteEffects.None, 1.0f);

        }

        public void Update(GameTime gameTime)
        {
            //kill it if its age is less than 0
            if (age <= 0)
            {
                IsAlive = false;
                return;
            }

            //make the particle only last 255/9 frames
            age -= 9;

        }
    }
}

