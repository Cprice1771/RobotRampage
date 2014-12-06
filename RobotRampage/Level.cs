using FarseerPhysics;
using MediaPlayerHelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RobotRampage
{
    public class Level
    {
        public Dictionary<Type, List<Vector2>> Objects { get; set; }
        public string Name { get; set; }
        public Vector2 SpawnLocation { get; set; }

        public SongFile LevelMusic { get; set; }

        public Level(string fileName)
        {
            Objects = new Dictionary<Type, List<Vector2>>();
            Parse(fileName);
        }

        public Level(Dictionary<Type, List<Vector2>> objs, Vector2 spL, SongFile s, string n)
        {
            Objects = objs;
            Name = n;
            SpawnLocation = spL;
            LevelMusic = s;
        }

        /// <summary>
        /// Writes the level to a file
        /// </summary>
        public void Write()
        {
            using(StreamWriter mapWriter = new StreamWriter(Directory.GetCurrentDirectory() + "\\" + Name + ".map"))
            {
                mapWriter.WriteLine("SpawnPoint," + SpawnLocation.X + "," + SpawnLocation.Y);

                foreach(KeyValuePair<Type, List<Vector2>> kvp in Objects)
                {
                    if (kvp.Key == typeof(Floor))
                    {
                        foreach (Vector2 pos in kvp.Value)
                            mapWriter.WriteLine("Floor," + ConvertUnits.ToDisplayUnits(pos.X) + "," + ConvertUnits.ToDisplayUnits(pos.Y));
                        
                    }
                    else if (kvp.Key == typeof(Wall))
                    {
                        foreach (Vector2 pos in kvp.Value)
                            mapWriter.WriteLine("Wall," + ConvertUnits.ToDisplayUnits(pos.X) + "," + ConvertUnits.ToDisplayUnits(pos.Y));
                    }
                    else if (kvp.Key == typeof(Robot))
                    {
                        foreach (Vector2 pos in kvp.Value)
                            mapWriter.WriteLine("Robot," + ConvertUnits.ToDisplayUnits(pos.X) + "," + ConvertUnits.ToDisplayUnits(pos.Y));
                    }
                    else if (kvp.Key == typeof(SuicideRobot))
                    {
                        foreach (Vector2 pos in kvp.Value)
                            mapWriter.WriteLine("SuicideRobot," + ConvertUnits.ToDisplayUnits(pos.X) + "," + ConvertUnits.ToDisplayUnits(pos.Y));
                    }
                    else if (kvp.Key == typeof(WinPoint))
                    {
                        foreach (Vector2 pos in kvp.Value)
                            mapWriter.WriteLine("WinPoint," + ConvertUnits.ToDisplayUnits(pos.X) + "," + ConvertUnits.ToDisplayUnits(pos.Y));
                    }
                    else if (kvp.Key == typeof(Spikes))
                    {
                        foreach (Vector2 pos in kvp.Value)
                            mapWriter.WriteLine("Spikes," + ConvertUnits.ToDisplayUnits(pos.X) + "," + ConvertUnits.ToDisplayUnits(pos.Y));
                    }
                }
            }
        }

        /// <summary>
        /// Parses a level file
        /// </summary>
        /// <param name="fileName">name of the file</param>
        private void Parse(string fileName)
        {
            using (StreamReader mapReader = new StreamReader(fileName))
            {
                Name = fileName;
                string line;

                while ((line = mapReader.ReadLine()) != null)
                {
                    string[] objects = line.Split(',');

                    if (objects[0] == "Floor")
                    {
                        if(!Objects.ContainsKey(typeof(Floor)))
                            Objects.Add(typeof(Floor), new List<Vector2>());
                            
                        Objects[typeof(Floor)].Add(new Vector2(ConvertUnits.ToSimUnits(float.Parse(objects[1])), ConvertUnits.ToSimUnits(float.Parse(objects[2]))));

                    }
                    else if (objects[0] == "Wall")
                    {
                        if (!Objects.ContainsKey(typeof(Wall)))
                            Objects.Add(typeof(Wall), new List<Vector2>());

                        Objects[typeof(Wall)].Add(new Vector2(ConvertUnits.ToSimUnits(float.Parse(objects[1])), ConvertUnits.ToSimUnits(float.Parse(objects[2]))));
                    }
                    else if (objects[0] == "Robot")
                    {
                        if (!Objects.ContainsKey(typeof(Robot)))
                            Objects.Add(typeof(Robot), new List<Vector2>());

                        Objects[typeof(Robot)].Add(new Vector2(ConvertUnits.ToSimUnits(float.Parse(objects[1])), ConvertUnits.ToSimUnits(float.Parse(objects[2]))));
                    }
                    else if (objects[0] == "SuicideRobot")
                    {
                        if (!Objects.ContainsKey(typeof(SuicideRobot)))
                            Objects.Add(typeof(SuicideRobot), new List<Vector2>());

                        Objects[typeof(SuicideRobot)].Add(new Vector2(ConvertUnits.ToSimUnits(float.Parse(objects[1])), ConvertUnits.ToSimUnits(float.Parse(objects[2]))));
                    }
                    else if (objects[0] == "WinPoint")
                    {
                        if (!Objects.ContainsKey(typeof(WinPoint)))
                            Objects.Add(typeof(WinPoint), new List<Vector2>());

                        Objects[typeof(WinPoint)].Add(new Vector2(ConvertUnits.ToSimUnits(float.Parse(objects[1])), ConvertUnits.ToSimUnits(float.Parse(objects[2]))));
                    }
                    else if (objects[0] == "SpawnPoint")
                    {
                        if (!Objects.ContainsKey(typeof(SpawnPoint)))
                            Objects.Add(typeof(SpawnPoint), new List<Vector2>());


                        SpawnLocation = new Vector2(ConvertUnits.ToSimUnits(float.Parse(objects[1])), ConvertUnits.ToSimUnits(float.Parse(objects[2])));
                        Objects[typeof(SpawnPoint)].Add(SpawnLocation);
                    }
                    else if (objects[0] == "Spikes")
                    {
                        if (!Objects.ContainsKey(typeof(Spikes)))
                            Objects.Add(typeof(Spikes), new List<Vector2>());

                        Objects[typeof(Spikes)].Add(new Vector2(ConvertUnits.ToSimUnits(float.Parse(objects[1])), ConvertUnits.ToSimUnits(float.Parse(objects[2]))));
                    }
                    
                }
            }
        }

    }
}
