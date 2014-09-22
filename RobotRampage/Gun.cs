using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobotRampage
{
    public class Gun : IGameObject
    {
        public Vector2 position;
        public Texture2D texture;
        MainGame parent;
        Rectangle srcRect;
        bool equipped;
        int magazineSize;
        int damage;
        bool Reloading;
        public PlayerDirection Direction;
        float muzzleVelocity;
        double fireRate = 100.0;
        double lastFire = -1.0;

        public int TotalAmmo { get; set; }
        public int LoadedAmmo { get; set; }
        public float Width { get { return texture.Width; } }
        public float Height { get { return texture.Height; } }
       
         public float Rotation
        {
            get
            {
                float x = (position.X - Mouse.GetState().X) + parent.CameraOffset.X;
                float y = (position.Y - Mouse.GetState().Y) + parent.CameraOffset.Y;
                double angle = Math.Atan2(y, x);
                return (float)angle;
            }
         }

        public Gun(Texture2D t, Vector2 pos, MainGame game, int d, int magSize, float mv)
        {
            texture = t;
            position = pos;
            parent = game;
            srcRect = new Rectangle(0, 0, texture.Width, texture.Height);
            damage = d;
            magazineSize = magSize;
            muzzleVelocity = mv;
            LoadedAmmo = 30;
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            lastFire += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                
                if(lastFire == -1.0f || 
                    lastFire  > fireRate)
                {
                    lastFire = gameTime.ElapsedGameTime.TotalMilliseconds;
                    Shoot();
                }
            }
        }

        public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            Vector2 offset;

            if(Direction == PlayerDirection.RIGHT)
            {
                offset = new Vector2(position.X + 10, position.Y);
                sb.Draw(this.texture, offset, srcRect, Color.White, Rotation, new Vector2(texture.Width, texture.Height / 2), 1.0f, SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally, 1.0f);
            }
            else
            {
                offset = new Vector2(position.X - 100, position.Y);
                sb.Draw(this.texture, position, srcRect, Color.White, Rotation, new Vector2(texture.Width, texture.Height / 2), 1.0f, SpriteEffects.FlipHorizontally, 1.0f);
            }
        }

        public void Shoot()
        {
            //If we can shoot
            if (!Reloading && LoadedAmmo > 0)
            {
                parent.CreateBullet(damage, Rotation, muzzleVelocity);
            }
        }

        public void Reload()
        {
            //If were already full on ammo do nothing
            if (magazineSize == LoadedAmmo)
                return;

            //If we have more than a magazine
            if (TotalAmmo - magazineSize > 0)
            {
                Reloading = true;
                LoadedAmmo = magazineSize;
                TotalAmmo -= magazineSize;
            }
            //Else just set the loaded ammo to all we got
            else if(TotalAmmo > 0) 
            {
                Reloading = true;
                LoadedAmmo = TotalAmmo;
            }
            //If we have no bullets do nothing
        }

        
    }
}
