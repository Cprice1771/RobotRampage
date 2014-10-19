using FarseerPhysics;
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
        #region public members
        public Vector2 position { get; set; }
        public Texture2D Texture { get; set; }
        public int magazineSize { get; set; }
        public bool Reloading { get; set; }
        public int LoadedAmmo { get; set; }
        public float Width { get { return Texture.Width; } }
        public float Height { get { return Texture.Height; } }
        #endregion

        #region Private members
        MainGame parent;
        Rectangle srcRect;
        bool equipped;
        
        int damage;
        public PlayerDirection Direction;
        float muzzleVelocity;
        double fireRate;
        double lastFire;
        double reloadSpeed;
        double reloadTime;
        RobotRampage.MainGame.CreateProjectile shoot;
        #endregion

        

        public int ReserveAmmo
        {
            get;
            set;
        }
       
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

        /// <summary>
        /// Contructor for Gun
        /// </summary>
        /// <param name="t">Texture</param>
        /// <param name="pos">Position</param>
        /// <param name="game">reference to MainGame</param>
        /// <param name="d">Damage of bullets</param>
        /// <param name="magSize">Size of magazine</param>
        /// <param name="mv">Muzzle velocity</param>
        /// <param name="ammo">Starting ammo</param>
        /// <param name="fr">Fire rate in ms</param>
        /// <param name="rs">Reload speed in ms</param>
        public Gun(Texture2D t, Vector2 pos, MainGame game, int d, int magSize, float mv, int ammo, double fr, double rs, RobotRampage.MainGame.CreateProjectile cp)
        {
            Texture = t;
            position = pos;
            parent = game;
            srcRect = new Rectangle(0, 0, Texture.Width, Texture.Height);
            damage = d;
            magazineSize = magSize;
            if (ammo > magSize)
            {
                LoadedAmmo = ammo % magSize;
                ReserveAmmo = ammo - LoadedAmmo;
            }
            else
            {
                LoadedAmmo = ammo;
                LoadedAmmo = 0;
            }

            muzzleVelocity = mv;
            LoadedAmmo = magSize;
            fireRate = fr;
            reloadSpeed = rs;
            lastFire = -1.0;
            reloadTime = 0.0;
            shoot = cp;
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            lastFire += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (Mouse.GetState().LeftButton == ButtonState.Pressed && !Reloading && LoadedAmmo > 0)
            {
                
                if(lastFire == -1.0f || 
                    lastFire  > fireRate)
                {
                    lastFire = gameTime.ElapsedGameTime.TotalMilliseconds;
                    Shoot();
                }
            }
            else if (Reloading)
            {
                reloadTime += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (reloadTime >= reloadSpeed)
                {
                    Reloading = false;
                    reloadTime = 0.0;
                }

            }
        }

        public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            Vector2 offset;

            if(Direction == PlayerDirection.RIGHT)
            {
                offset = new Vector2(position.X + 10, position.Y);
                sb.Draw(this.Texture, offset, srcRect, Color.White, Rotation, new Vector2(Texture.Width, Texture.Height / 2), 1.0f, SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally, 1.0f);
            }
            else
            {
                offset = new Vector2(position.X - 100, position.Y);
                sb.Draw(this.Texture, position, srcRect, Color.White, Rotation, new Vector2(Texture.Width, Texture.Height / 2), 1.0f, SpriteEffects.FlipHorizontally, 1.0f);
            }
        }

        public void Shoot()
        {
            //If we can shoot
            if (!Reloading && LoadedAmmo > 0)
            {
                float x = position.X - (float)(Math.Cos(Rotation) * (Width + 5));
                float y = position.Y - (float)(Math.Sin(Rotation) * (Width + 5));
                //parent.CreateBullet();
                //parent.CreateRocket(damage, Rotation, muzzleVelocity);
                shoot(damage, Rotation, muzzleVelocity);
                LoadedAmmo--;
                
            }
        }

        public void Reload()
        {
            //If were already full on ammo do nothing
            if (magazineSize == LoadedAmmo)
                return;

            //If we have more than a magazine
            if (ReserveAmmo - magazineSize > 0)
            {
                Reloading = true;
                ReserveAmmo -= magazineSize - LoadedAmmo;
                LoadedAmmo = magazineSize;
                
            }
            //Else just set the loaded ammo to all we got
            else if(ReserveAmmo > 0) 
            {
                Reloading = true;
                LoadedAmmo = ReserveAmmo;
                ReserveAmmo = 0;
            }
            //If we have no bullets do nothing
        }



        
    }
}
