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
    public class WinPoint : Body, IGameObject
    {
        Texture2D texture;
        public bool MarkedForRemoval { get; set; }


        public WinPoint(Texture2D text, World w)
            :base(w)
        {
            texture = text;
            this.CreateFixture(new PolygonShape(PolygonTools.CreateRectangle(ConvertUnits.ToSimUnits(text.Width / 2), ConvertUnits.ToSimUnits(text.Height / 2)), 1.0f));
            this.BodyType = BodyType.Kinematic;
            this.FixedRotation = true;
            this.Restitution = 0.0f;
            this.Friction = 1.0f;
            this.MarkedForRemoval = false;
        }

        public void Update(GameTime gameTime)
        {
            //Do nothing
        }

        public void Draw(SpriteBatch sb)
        {
            Vector2 offset = new Vector2(ConvertUnits.ToDisplayUnits(Position.X) - texture.Width / 2, ConvertUnits.ToDisplayUnits(Position.Y) - texture.Height / 2);
            sb.Draw(texture, offset, new Rectangle(0, 0, texture.Width, texture.Height), Color.White, Rotation, new Vector2(0, 0), 1.0f, SpriteEffects.None, 1.0f);
        }
    }
}
