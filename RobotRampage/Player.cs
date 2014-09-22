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
    public class Player : Body, IGameObject, ILivingThing
    {
        Texture2D texture;
        Rectangle srcRect;
        Rectangle destRect;
        MainGame parent;
        PlayerState state;
        PlayerDirection direction;
        Gun weapon;
        public int Health { get; private set; }
        public float Width { get { return texture.Width; } }

        public float Height { get { return texture.Height; } }

        
        const float WALK_SPEED = 0.8f;
        const float RUN_SPEED = 4.0f;

        public Player(Texture2D t, MainGame game, World w)
            : base(w)
        {
            texture = t;
            parent = game;

            state = PlayerState.IDLE;
            direction = PlayerDirection.RIGHT;
            //frameCounter = 0;
            //frameRate = 1.0f / 24.0f;
            srcRect = new Rectangle(0, 0, texture.Width, texture.Height);
            destRect = new Rectangle((int)Position.X, (int)Position.Y, 50, 50);
            Health = 100;
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            bool shiftDown = Keyboard.GetState().IsKeyDown(Keys.LeftShift);
            bool aDown = Keyboard.GetState().IsKeyDown(Keys.A);
            bool dDown = Keyboard.GetState().IsKeyDown(Keys.D);
            bool spaceDown = Keyboard.GetState().IsKeyDown(Keys.Space);

            if ((Mouse.GetState().X - parent.CameraOffset.X) > ConvertUnits.ToDisplayUnits(Position.X))
            {
                direction = PlayerDirection.RIGHT;
                weapon.Direction = PlayerDirection.RIGHT;
            }
            else
            {
                direction = PlayerDirection.LEFT;
                weapon.Direction = PlayerDirection.LEFT;
            }
             
            weapon.Update(gameTime);
        }

        public void Draw(SpriteBatch sb)
        {
            Vector2 offset = new Vector2(ConvertUnits.ToDisplayUnits(Position.X) - texture.Width / 2, ConvertUnits.ToDisplayUnits(Position.Y) - texture.Height / 2);

            if (direction == PlayerDirection.LEFT)
                sb.Draw(this.texture, offset, srcRect, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.FlipHorizontally, 1.0f);
            else
            {
                Vector2 pos = ConvertUnits.ToDisplayUnits(Position);
                sb.Draw(this.texture, offset, srcRect, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 1.0f);
            }

            if (weapon != null)
            {
                Vector2 weaponOffset = new Vector2((ConvertUnits.ToDisplayUnits(Position.X)), ConvertUnits.ToDisplayUnits(Position.Y));
                weapon.position = weaponOffset;
                weapon.Draw(sb);
            }
        }

        public void EquipWeapon(Gun weap)
        {
            weapon = weap;
        }

        

        internal Vector2 GunLocation()
        {
            float x = ConvertUnits.ToDisplayUnits(Position.X) - (float)(Math.Cos(weapon.Rotation) * weapon.Width);
            float y = ConvertUnits.ToDisplayUnits(Position.Y) - (float)(Math.Sin(weapon.Rotation) * weapon.Width);
            return new Vector2(x, y);
        }


        public void DealDamage(int damage)
        {
            Health -= damage;
        }
    }
}
