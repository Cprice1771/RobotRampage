using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobotRampage
{
    public class Bullet : Body, IGameObject
    {
        public int Damage { get; private set; }

        Texture2D texture;
        MainGame parent;
        Rectangle srcRect;
        public Vector2 rotation { get; set; }

        public Bullet(Texture2D t, MainGame game, int d, World w)
            : base(w)
        {
            texture = t;
            parent = game;
            srcRect = new Rectangle(0, 0, texture.Width, texture.Height);
            Damage = d;
        }

        

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
        }

        public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            Vector2 offset = new Vector2(ConvertUnits.ToDisplayUnits(Position.X) - texture.Width / 2, ConvertUnits.ToDisplayUnits(Position.Y) - texture.Height / 2);
            sb.Draw(this.texture, offset, srcRect, Color.White, Rotation, new Vector2(0, 0), 1.0f, SpriteEffects.None, 1.0f);
        }



        internal bool OffScreen()
        {
            if ((ConvertUnits.ToDisplayUnits(Position.Y) + parent.CameraOffset.Y) > MainGame.ScreenHeight || (ConvertUnits.ToDisplayUnits(Position.Y) + parent.CameraOffset.Y) < 0 ||
                (ConvertUnits.ToDisplayUnits(Position.X) + parent.CameraOffset.X) > MainGame.ScreenWidth || (ConvertUnits.ToDisplayUnits(Position.X) + parent.CameraOffset.X) < 0)
                return true;

            return false;
        }
    }
}
