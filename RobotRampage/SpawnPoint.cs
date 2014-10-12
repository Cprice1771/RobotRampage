﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobotRampage
{
    public class SpawnPoint
    {
        public Vector2 location { get; set; }
        Texture2D texture;
        public SpawnPoint(Vector2 loc, Texture2D text)
        {
            location = loc;
            texture = text;
        }
    }
}
