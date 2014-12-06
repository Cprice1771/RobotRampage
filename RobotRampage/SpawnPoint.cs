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
    public class SpawnPoint : IGameObject
    {
        public Vector2 location { get; set; }
        Texture2D texture;
        public SpawnPoint(Vector2 loc, Texture2D text)
        {
            location = loc;
            texture = text;
        }

        public void Update(GameTime gameTime)
        {
            //Do nothing
        }

        public void Draw(SpriteBatch sb)
        {
            Vector2 offset = new Vector2(ConvertUnits.ToDisplayUnits(location.X) - texture.Width / 2, ConvertUnits.ToDisplayUnits(location.Y) - texture.Height / 2);
            sb.Draw(texture, offset, new Rectangle(0, 0, texture.Width, texture.Height), Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 1.0f);
        }

        public bool MarkedForRemoval
        {
            get;
            set;
        }
    }
}
