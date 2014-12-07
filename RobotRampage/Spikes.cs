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
    public class Spikes : Body, IGameObject
    {
        Texture2D texture;
        MainGame parent;
        Rectangle srcRect;

        public Spikes(Texture2D t, MainGame game, World w, float rot)
            : base(w)
        {
            texture = t;
            parent = game;
            srcRect = new Rectangle(0, 0, texture.Width, texture.Height);
            this.CreateFixture(new PolygonShape(PolygonTools.CreateRectangle(ConvertUnits.ToSimUnits(t.Width / 2), ConvertUnits.ToSimUnits(t.Height / 2)), 1.0f));
            this.BodyType = BodyType.Static;
            this.Restitution = 0.3f;
            this.Friction = 1.0f;
            this.IgnoreGravity = true;
            this.MarkedForRemoval = false;
            Rotation = rot;
        }

        public bool MarkedForRemoval
        {
            get;
            set;
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            //Do nothing
        }

        public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            Vector2 offset = new Vector2(ConvertUnits.ToDisplayUnits(Position.X), ConvertUnits.ToDisplayUnits(Position.Y));
            sb.Draw(this.texture, offset, srcRect, Color.White, Rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1.0f, SpriteEffects.None, 1.0f);
        }
    }
}
