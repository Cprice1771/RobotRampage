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
    public class Floor : Body, IGameObject
    {
        public float Width { get { return texture.Width; } }
        public float Height { get { return texture.Height; } }

        Texture2D texture;
        MainGame parent;
        Rectangle srcRect;
        

        public Floor(Texture2D t, MainGame game, World w)
            :base(w)
        {
            texture = t;
            parent = game;
            srcRect = new Rectangle(0, 0, texture.Width, texture.Height);
            this.CreateFixture(new PolygonShape(PolygonTools.CreateRectangle(ConvertUnits.ToSimUnits(t.Width / 2), ConvertUnits.ToSimUnits(t.Height / 2)), 1.0f));
            this.BodyType = BodyType.Static;
            this.Restitution = 0.0f;
            this.Friction = 0.6f;

        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
           //Do nothing 
        }

        public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            Vector2 offset = new Vector2(ConvertUnits.ToDisplayUnits(Position.X) - texture.Width / 2, ConvertUnits.ToDisplayUnits(Position.Y) - texture.Height / 2);
            sb.Draw(this.texture, offset, srcRect, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 1.0f);
        }

        
    }
}
