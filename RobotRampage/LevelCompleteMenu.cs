using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobotRampage
{
    public class LevelCompleteMenu
    {
        Texture2D title;
        Texture2D nextLevel;
        Texture2D backToMenu;
        MainGame parent;
        Rectangle titleRect;
        Rectangle nextLevelRect;
        Rectangle backToMenuRect;


        public LevelCompleteMenu(Texture2D tt, Texture2D nextt, Texture2D backt, MainGame mg)
        {
            title = tt;
            nextLevel = nextt;
            backToMenu = backt;
            parent = mg;
            titleRect = new Rectangle((MainGame.ScreenWidth / 2) - title.Width / 2, 75, title.Width, title.Height);
            nextLevelRect = new Rectangle(MainGame.ScreenWidth / 2 - nextLevel.Width / 2, (int)(1.4f * MainGame.ScreenHeight / 3), nextLevel.Width, nextLevel.Height);
            backToMenuRect = new Rectangle(MainGame.ScreenWidth / 2 - backToMenu.Width / 2, (int)(2.2f * MainGame.ScreenHeight / 3), backToMenu.Width, backToMenu.Height);
        }

        public void Update(GameTime gameTime)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if (nextLevelRect.Contains(Mouse.GetState().Position))
                    parent.NextLevel();
                else if (backToMenuRect.Contains(Mouse.GetState().Position))
                    parent.GoToMainMenu();
            }
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(title, titleRect, Color.White);
            sb.Draw(nextLevel, nextLevelRect, Color.White);
            sb.Draw(backToMenu, backToMenuRect, Color.White);
        }
    }
}
