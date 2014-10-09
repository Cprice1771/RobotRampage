using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobotRampage
{
    public enum PlayerState : int
    {
        IDLE = 0,
        WALKING = 1,
        JUMPING = 2,
        RUNNING = 3
    }

    public enum PlayerDirection : int
    {
        LEFT = 0,
        RIGHT = 1
    }

    public enum GunSelected : int
    {
        PRIMARY = 0,
        SECONDARY = 1,
        TERTIARY = 2
    }

    public enum GameState : int
    {
        MAIN_MENU,
        OPTIONS_MENU,
        LEVEL
    }
}
