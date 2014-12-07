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
        public Dictionary<Type, List<List<float>>> Objects { get; set; }
        public string Name { get; set; }
        public Vector2 SpawnLocation { get; set; }

        public SongFile LevelMusic { get; set; }

        public Level(string fileName)
        {
            Objects = new Dictionary<Type, List<List<float>>>();
            Parse(fileName);
        }

        public Level(Dictionary<Type, List<List<float>>> objs, Vector2 spL, SongFile s, string n)
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

                foreach(KeyValuePair<Type, List<List<float>>> kvp in Objects)
                {
                    if (kvp.Key == typeof(Floor))
                    {
                        foreach (List<float> values in kvp.Value)
                            mapWriter.WriteLine("Floor," + ConvertUnits.ToDisplayUnits(values[0]) + "," + ConvertUnits.ToDisplayUnits(values[1]) + "," + ConvertUnits.ToDisplayUnits(values[2]));
                        
                    }
                    else if (kvp.Key == typeof(Wall))
                    {
                        foreach (List<float> values in kvp.Value)
                            mapWriter.WriteLine("Wall," + ConvertUnits.ToDisplayUnits(values[0]) + "," + ConvertUnits.ToDisplayUnits(values[1]) + "," + ConvertUnits.ToDisplayUnits(values[2]));
                    }
                    else if (kvp.Key == typeof(Robot))
                    {
                        foreach (List<float> values in kvp.Value)
                            mapWriter.WriteLine("Robot," + ConvertUnits.ToDisplayUnits(values[0]) + "," + ConvertUnits.ToDisplayUnits(values[1]) + "," + ConvertUnits.ToDisplayUnits(values[2]));
                    }
                    else if (kvp.Key == typeof(SuicideRobot))
                    {
                        foreach (List<float> values in kvp.Value)
                            mapWriter.WriteLine("SuicideRobot," + ConvertUnits.ToDisplayUnits(values[0]) + "," + ConvertUnits.ToDisplayUnits(values[1]) + "," + ConvertUnits.ToDisplayUnits(values[2]));
                    }
                    else if (kvp.Key == typeof(WinPoint))
                    {
                        foreach (List<float> values in kvp.Value)
                            mapWriter.WriteLine("WinPoint," + ConvertUnits.ToDisplayUnits(values[0]) + "," + ConvertUnits.ToDisplayUnits(values[1]) + "," + ConvertUnits.ToDisplayUnits(values[2]));
                    }
                    else if (kvp.Key == typeof(Spikes))
                    {
                        foreach (List<float> values in kvp.Value)
                            mapWriter.WriteLine("Spikes," + ConvertUnits.ToDisplayUnits(values[0]) + "," + ConvertUnits.ToDisplayUnits(values[1]) + "," + ConvertUnits.ToDisplayUnits(values[2]));
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

                    float rotation = 0.0f;

                    if (objects.Count() > 3)
                        rotation = float.Parse(objects[3]);

                    if (objects[0] == "Floor")
                    {
                        if(!Objects.ContainsKey(typeof(Floor)))
                            Objects.Add(typeof(Floor), new List<List<float>>());



                        Objects[typeof(Floor)].Add(new List<float>() { ConvertUnits.ToSimUnits(float.Parse(objects[1])), ConvertUnits.ToSimUnits(float.Parse(objects[2])), rotation });

                    }
                    else if (objects[0] == "Wall")
                    {
                        if (!Objects.ContainsKey(typeof(Wall)))
                            Objects.Add(typeof(Wall), new List<List<float>>());

                        Objects[typeof(Wall)].Add(new List<float>() { ConvertUnits.ToSimUnits(float.Parse(objects[1])), ConvertUnits.ToSimUnits(float.Parse(objects[2])), rotation });
                    }
                    else if (objects[0] == "Robot")
                    {
                        if (!Objects.ContainsKey(typeof(Robot)))
                            Objects.Add(typeof(Robot), new List<List<float>>());

                        Objects[typeof(Robot)].Add(new List<float>() { ConvertUnits.ToSimUnits(float.Parse(objects[1])), ConvertUnits.ToSimUnits(float.Parse(objects[2])), rotation });
                    }
                    else if (objects[0] == "SuicideRobot")
                    {
                        if (!Objects.ContainsKey(typeof(SuicideRobot)))
                            Objects.Add(typeof(SuicideRobot), new List<List<float>>());

                        Objects[typeof(SuicideRobot)].Add(new List<float>() { ConvertUnits.ToSimUnits(float.Parse(objects[1])), ConvertUnits.ToSimUnits(float.Parse(objects[2])), rotation });
                    }
                    else if (objects[0] == "WinPoint")
                    {
                        if (!Objects.ContainsKey(typeof(WinPoint)))
                            Objects.Add(typeof(WinPoint), new List<List<float>>());

                        Objects[typeof(WinPoint)].Add(new List<float>() { ConvertUnits.ToSimUnits(float.Parse(objects[1])), ConvertUnits.ToSimUnits(float.Parse(objects[2])), rotation });
                    }
                    else if (objects[0] == "SpawnPoint")
                    {
                        if (!Objects.ContainsKey(typeof(SpawnPoint)))
                            Objects.Add(typeof(SpawnPoint), new List<List<float>>());


                        SpawnLocation = new Vector2(ConvertUnits.ToSimUnits(float.Parse(objects[1])), ConvertUnits.ToSimUnits(float.Parse(objects[2])));

                        Objects[typeof(SpawnPoint)].Add(new List<float>() { ConvertUnits.ToSimUnits(float.Parse(objects[1])), ConvertUnits.ToSimUnits(float.Parse(objects[2])), rotation });
                    }
                    else if (objects[0] == "Spikes")
                    {
                        if (!Objects.ContainsKey(typeof(Spikes)))
                            Objects.Add(typeof(Spikes), new List<List<float>>());

                        Objects[typeof(Spikes)].Add(new List<float>() { ConvertUnits.ToSimUnits(float.Parse(objects[1])), ConvertUnits.ToSimUnits(float.Parse(objects[2])), rotation });
                    }
                    
                }
            }
        }

    }
}
