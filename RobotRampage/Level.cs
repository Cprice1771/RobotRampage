using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobotRampage
{
    public class Level
    {
        public Dictionary<Type, List<Vector2>> Objects { get; set; }
        public string Name { get; set; }
        public SpawnPoint Spawn { get; set; }

        Song levelMusic;

        public Level(string fileName)
        {
            Parse(fileName);
        }

        public Level(Dictionary<Type, List<Vector2>> objs, SpawnPoint sp, Song s, string n)
        {
            Objects = objs;
            Name = n;
            Spawn = sp;
            levelMusic = s;
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

        public void PlayMusic()
        {
            try
            {
                MediaPlayer.Play(levelMusic);
            }
            catch (Exception)
            {
                MediaPlayer.Play(levelMusic);
            }
        }
    }
}
