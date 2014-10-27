using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobotRampage
{
    public interface IGameObject
    {
        bool MarkedForRemoval {get; set;}
        void Update(GameTime gameTime);
        void Draw(SpriteBatch sb);
    }
}
