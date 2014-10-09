using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobotRampage
{
    public class MainMenu
    {
        Texture2D title;
        Texture2D playGame;
        Texture2D options;
        MainGame parent;
        Rectangle titleRect;
        Rectangle playGameRect;
        Rectangle optionsRect;


        public MainMenu(Texture2D tt, Texture2D pg, Texture2D op, MainGame mg)
        {
            title = tt;
            playGame = pg;
            options = op;
            parent = mg;
            titleRect =  new Rectangle((MainGame.ScreenWidth / 2) - title.Width / 2, MainGame.ScreenHeight / 3, title.Width, title.Height);
            playGameRect = new Rectangle(MainGame.ScreenWidth / 2 - playGame.Width / 2, (int)(1.5f * MainGame.ScreenHeight / 3), playGame.Width, playGame.Height);
            optionsRect = new Rectangle(MainGame.ScreenWidth / 2 - options.Width / 2, 2 * MainGame.ScreenHeight / 3, options.Width, options.Height);
        }

        public void Update(GameTime gameTime)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if (playGameRect.Contains(Mouse.GetState().Position))
                    parent.StartGame();
                else if (optionsRect.Contains(Mouse.GetState().Position))
                    parent.GoToOptionsMenu();
            }
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(title,titleRect, Color.White);
            sb.Draw(playGame, playGameRect, Color.White);
            sb.Draw(options, optionsRect, Color.White);
        }
    }
}
