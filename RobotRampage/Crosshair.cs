using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobotRampage
{
    public class Crosshair : IGameObject
    {
        public bool MarkedForRemoval { get; set; }

        Vector2 position;
        Texture2D texture;
        MainGame parent;
        Rectangle srcRect;

        public Crosshair(Texture2D t, Vector2 pos, MainGame game)
        {
            texture = t;
            position = pos;
            parent = game;
            srcRect = new Rectangle(0, 0, texture.Width, texture.Height);
            this.MarkedForRemoval = false;
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            position = new Vector2((Mouse.GetState().X - texture.Width / 2) - parent.CameraOffset.X, (Mouse.GetState().Y - texture.Height / 2) - parent.CameraOffset.Y);
        }

        public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            sb.Draw(this.texture, position, srcRect, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 1.0f);
        }
    }
}
