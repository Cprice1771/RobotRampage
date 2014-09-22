using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobotRampage
{
    public interface ILivingThing
    {
        int Health { get; }
        void DealDamage(int damage);
    }
}
