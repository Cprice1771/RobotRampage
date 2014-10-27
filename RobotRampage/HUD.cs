using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobotRampage
{
    public class HUD : IGameObject
    {
        public Vector2 position { get; set; }
        public GunSelected selection { get; set; }
        public int Health { get; set; }
        public int AmmoLoaded { get; set; }
        public int AmmoReserve { get; set; }
        public bool Reloading { get; set; }
        public bool MarkedForRemoval { get; set; }
        Texture2D hudTexture;
        MainGame parent;
        SpriteFont font;

        Vector2 HEALTH_LOCATION = new Vector2(750, 40);
        Vector2 AMMO_LOCATION = new Vector2(500, 40);

        List<Gun> inventory;

        public HUD(Texture2D hud, List<Gun> inven, GunSelected s, Vector2 pos, SpriteFont f, MainGame g)
        {
            hudTexture = hud;
            inventory = inven;
            position = pos;
            font = f;
            parent = g;
            selection = s;
            Health = 100;
            AmmoLoaded = 30;
            AmmoReserve = 150;
            MarkedForRemoval = true;
        }

        public void Update(GameTime gameTime)
        {
            //Do nothing
        }

        public void Draw(SpriteBatch sb)
        {
            Color myColor = Color.White;
            myColor.A = 200;
            sb.Draw(hudTexture, position - parent.CameraOffset, myColor);
            sb.DrawString(font, "Heath: " + Health, HEALTH_LOCATION - parent.CameraOffset, Color.Black, 0.0f, new Vector2(0, 0), 1.2f, SpriteEffects.None, 1.0f);
            if(!Reloading)
                sb.DrawString(font, "Ammo: " + AmmoLoaded +" / " + AmmoReserve, AMMO_LOCATION - parent.CameraOffset, Color.Black, 0.0f, new Vector2(0, 0), 1.2f, SpriteEffects.None, 1.0f);
            else
                sb.DrawString(font, "Reloading...", AMMO_LOCATION - parent.CameraOffset, Color.Black, 0.0f, new Vector2(0, 0), 1.2f, SpriteEffects.None, 1.0f);

            for (int i = 0; i < inventory.Count; i++)
            {
                myColor.A = ((int)selection == i) ? (byte)255 : (byte)50;
                sb.Draw(inventory[i].Texture, new Vector2(i * 100 + 100, 40) - parent.CameraOffset, new Rectangle(0, 0, inventory[i].Texture.Width, inventory[i].Texture.Height), myColor, -.5f, new Vector2(inventory[i].Texture.Width, inventory[i].Texture.Height / 2), 1.0f, SpriteEffects.None, 1.0f);
            }
        }
    }
}
