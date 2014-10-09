using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobotRampage
{
    public class Level
    {
        public List<IGameObject> Objects { get; set; }
        public string Name { get; set; }
        public SpawnPoint Spawn { get; set; }


        public Level(string fileName)
        {
            Parse(fileName);
        }

        public Level(List<IGameObject> objs, SpawnPoint sp, string n)
        {
            Objects = objs;
            Name = n;
            Spawn = sp;
        }

        /// <summary>
        /// Writes the level to a file
        /// </summary>
        public void Write()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Parses a level file
        /// </summary>
        /// <param name="fileName">name of the file</param>
        private void Parse(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
