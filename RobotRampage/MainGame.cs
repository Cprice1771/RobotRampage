#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework.Media;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Collision;
using System.Diagnostics;
#endregion

namespace RobotRampage
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainGame : Game
    {
        #region constants
        const int X_CAMERA_THRESHOLD = 200;
        public const int ScreenWidth = 1024;
        public const int ScreenHeight = 768;
        #endregion


        #region public members
        public Vector2 CameraOffset;
        public GameState State;
        public Camera gameCamera;
        public delegate void CreateProjectile(int x, float y, float z);
        #endregion

        #region private members
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont font;
        Song levelMusic;
        Song menuMusic;
        World physicsWorld;
        int mouseWheelLoc;
        List<Level> singlePlayerLevels;
        int currentLevel;
        MainMenu menu;

        #region Textures
        Texture2D playerSpriteSheet;
        Texture2D floorTexture;
        Texture2D crosshairTexture;
        Texture2D defaultGunTexture;
        Texture2D bulletTexture;
        Texture2D robotTexture;
        Texture2D suicideRobotTexture;
        Texture2D emissionSpriteSheet;
        Texture2D laserTexture;
        Texture2D wallTexture;
        Texture2D winPointTexture;
        Texture2D rocketTexture;
        Texture2D hudTexture;
        Texture2D Title;
        Texture2D PlayGameTexture;
        Texture2D OptionsMenuTexture;
        Texture2D spawnPointTexture;
        Texture2D handGunTexture;
        Texture2D shotGunTexture;
        #endregion

        #region world objects
        Player player;
        List<IGameObject> gameObjects;
        Crosshair crosshair;
        Gun assualtRifle;
        Gun handGun;
        Gun shotGun;
        Gun RocketLauncher;
        HUD hud;
        #endregion
        #endregion

        #region Constructor
        public MainGame()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            physicsWorld = new World(new Vector2(0.0f, 9.82f));
            
            gameObjects = new List<IGameObject>();
            State = GameState.MAIN_MENU;
            menu = new MainMenu(Title, PlayGameTexture, OptionsMenuTexture, this);
            currentLevel = 0;
            singlePlayerLevels = CreateLevels();
            
            hud = new HUD(hudTexture, player.Inventory, player.EquipedWeaponSlot, new Vector2(0, 0), font, this);
            gameCamera = new Camera();
            MediaPlayer.Volume = 1.0f;
            IsMouseVisible = true;
            MediaPlayer.IsRepeating = true;
            TryPlay(menuMusic);
        }

        private List<Level> CreateLevels()
        {
            //TODO create or load all our levels for the game
            CreateEnemies();
            CreateFloors();
            CreateWalls();
            CreatePlayer();
            CreateWalls();
            CreateWinPoint(ConvertUnits.ToSimUnits(1500, 300));
            List<Level> levels = new List<Level>();
            Dictionary<Type, List<Vector2>> levelObjects = GetLevelDictFromGameObjects(gameObjects);
            Level level1 = new Level(levelObjects, new SpawnPoint(new Vector2(ConvertUnits.ToSimUnits(100), ConvertUnits.ToSimUnits(150)), spawnPointTexture), levelMusic, "Level 1");
            levels.Add(level1);
            Rocket r = new Rocket(rocketTexture, this, 50, physicsWorld);
            r.Position = ConvertUnits.ToSimUnits(100, 100);
            Level level2 = new Level(levelObjects, new SpawnPoint(new Vector2(ConvertUnits.ToSimUnits(100), ConvertUnits.ToSimUnits(150)), spawnPointTexture), levelMusic, "Level 2");
            levels.Add(level2);

            return levels;
        }

        

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            LoadTextures();

            levelMusic = Content.Load<Song>("FailingDefense");
            menuMusic = Content.Load<Song>("In a Heartbeat");
            //TODO: uncomment for music
            //TryPlay(backgroundMusic);

            font = Content.Load<SpriteFont>("myFont");
            

            graphics.PreferredBackBufferHeight = ScreenHeight;
            graphics.PreferredBackBufferWidth = ScreenWidth;
            graphics.ApplyChanges();

            Vector2 initialPlayerPosition = new Vector2(ScreenWidth / 2, 150);
            
            crosshair = new Crosshair(crosshairTexture, new Vector2(ScreenWidth / 2, ScreenHeight / 2), this);
            assualtRifle = new Gun(defaultGunTexture, initialPlayerPosition, this, 25, 30, 5.0f, 100, 100.0, 1000.0, CreateBullet);
            shotGun = new Gun(shotGunTexture, initialPlayerPosition, this, 50, 15, 5.0f, 20, 1000.0, 1500.0, CreateShotgunBullet);
            handGun = new Gun(handGunTexture, initialPlayerPosition, this, 40, 10, 5.0f, 200, 500.0, 800.0, CreateBullet);
            RocketLauncher = new Gun(handGunTexture, initialPlayerPosition, this, 100, 4, 5.0f, 20, 1000.0, 1500.0, CreateRocket);
            mouseWheelLoc = Mouse.GetState().ScrollWheelValue;
            
        }

       

        

        private void LoadTextures()
        {
            crosshairTexture = Content.Load<Texture2D>("crosshair");
            playerSpriteSheet = Content.Load<Texture2D>("Player");
            floorTexture = Content.Load<Texture2D>("floor");
            defaultGunTexture = Content.Load<Texture2D>("gun");
            bulletTexture = Content.Load<Texture2D>("bullet");
            robotTexture = Content.Load<Texture2D>("robot");
            suicideRobotTexture = Content.Load<Texture2D>("suicideRobot");
            emissionSpriteSheet = Content.Load<Texture2D>("emissionSpriteSheet");
            laserTexture = Content.Load<Texture2D>("laser");
            wallTexture = Content.Load<Texture2D>("wall");
            winPointTexture = Content.Load<Texture2D>("WinPoint");
            rocketTexture = Content.Load<Texture2D>("rocket.png");
            hudTexture = Content.Load<Texture2D>("HUD.png");
            spawnPointTexture = Content.Load<Texture2D>("SpawnPoint");
            Title = Content.Load<Texture2D>("Title.png");
            PlayGameTexture = Content.Load<Texture2D>("PlayGame");
            OptionsMenuTexture = Content.Load<Texture2D>("Options");
            handGunTexture = Content.Load<Texture2D>("handgun");
            shotGunTexture = Content.Load<Texture2D>("shotgun");
        }
        #endregion

        #region Update
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            switch (State)
            {
                case GameState.LEVEL:
                    physicsWorld.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

                    #region Mouse Inputs
                    if (Mouse.GetState().ScrollWheelValue < mouseWheelLoc)
                        player.CycleNextWeapon();
                    else if (Mouse.GetState().ScrollWheelValue > mouseWheelLoc)
                        player.CyclePreviousWeapon();

                    mouseWheelLoc = Mouse.GetState().ScrollWheelValue;
                    #endregion

                    #region Camera
                    int Y_CAMERA_THRESHOLD = (ScreenHeight / 2) + 100;
                    //X movement
                    if (ConvertUnits.ToDisplayUnits(player.Position.X) > (-1 * (gameCamera.Position.X)) + ScreenWidth - X_CAMERA_THRESHOLD)
                        gameCamera.Move(new Vector2(-1 * (ConvertUnits.ToDisplayUnits(player.Position.X) - (-1 * (gameCamera.Position.X) + ScreenWidth - X_CAMERA_THRESHOLD)), 0));

                    else if (ConvertUnits.ToDisplayUnits(player.Position.X) < (-1 * (gameCamera.Position.X)) + X_CAMERA_THRESHOLD)
                        gameCamera.Move(new Vector2(-1 * (ConvertUnits.ToDisplayUnits(player.Position.X) - (-1 * (gameCamera.Position.X) + X_CAMERA_THRESHOLD)), 0));

                    //Y movement
                    float playerY = ConvertUnits.ToDisplayUnits(player.Position.Y);
                    //if (playerY > cam.Position.Y + 400 && cam.Position.Y > 0)
                     //   cam.Move(new Vector2(0, -1 * (ConvertUnits.ToDisplayUnits(player.Position.Y) - (-1 * (cam.Position.Y) + 400))));

                    if (playerY < gameCamera.Position.Y + Y_CAMERA_THRESHOLD + 100)
                        gameCamera.Move(new Vector2(0, -1 * (ConvertUnits.ToDisplayUnits(player.Position.Y) - (-1 * (gameCamera.Position.Y) + Y_CAMERA_THRESHOLD))));
           
                    CameraOffset = gameCamera.Position;
                    #endregion

                    #region Keyboard inputs
                    //TODO: fix ground colloision detection
                    if (Keyboard.GetState().IsKeyDown(Keys.Space) && player.LinearVelocity.Y == 0)
                        player.ApplyForce(new Vector2(0, -200.0f));

                    if (Keyboard.GetState().IsKeyDown(Keys.D) && player.LinearVelocity.X < 5.0f)
                        player.ApplyForce(new Vector2(20.0f, 0));
                    else if (Keyboard.GetState().IsKeyDown(Keys.A) && player.LinearVelocity.X > -5.0f)
                        player.ApplyForce(new Vector2(-20.0f, 0));

                    if (Keyboard.GetState().IsKeyDown(Keys.R))
                        player.EquipedWeapon.Reload();
                    #endregion

                    #region Update HUD
                    hud.Health = player.Health;
                    hud.AmmoLoaded = player.EquipedWeapon.LoadedAmmo;
                    hud.AmmoReserve = player.EquipedWeapon.ReserveAmmo;
                    hud.Reloading = player.EquipedWeapon.Reloading;
                    hud.selection = player.EquipedWeaponSlot;
                    #endregion

                    #region world cleanup
                    for (int i = gameObjects.Count - 1; i >= 0; i--)
                    {
                        if (gameObjects[i] is ILivingThing)
                        {
                            ILivingThing deadObject = gameObjects[i] as ILivingThing;
                            if (deadObject.Health <= 0)
                            {
                                physicsWorld.RemoveBody(deadObject as Body);
                                gameObjects.RemoveAt(i);
                            }
                        }
                        else if (gameObjects[i] is Bullet)
                        {
                            Bullet bullet = gameObjects[i] as Bullet;
                            if (bullet.OffScreen())
                            {
                                physicsWorld.RemoveBody(bullet);
                                gameObjects.RemoveAt(i);
                            }
                        }
                    }
                    if (player.Health <= 0)
                        respawnPlayer();
                    #endregion

                    #region Misc Updates
                    player.Update(gameTime);
                    crosshair.Update(gameTime);

                    for (int i = gameObjects.Count - 1; i >= 0; i--)
                        gameObjects[i].Update(gameTime);

                    #endregion
                    break;
                case GameState.MAIN_MENU:
                    menu.Update(gameTime);
                    gameCamera.Position = Vector2.Zero;
                    break;
                case GameState.OPTIONS_MENU:
                    throw new NotImplementedException();
                    break;

            }

            base.Update(gameTime);
        }

        private void respawnPlayer()
        {
            physicsWorld = new World(new Vector2(0.0f, 9.82f));
            gameObjects = new List<IGameObject>();
            CreateGameObjectsFromLevel(singlePlayerLevels[currentLevel].Objects);
            CreatePlayer();
            //player.Position = singlePlayerLevels[currentLevel].Spawn.location;
            //player.LinearVelocity = Vector2.Zero;
            //player.Reset();
        }

        
        #endregion

        #region Draw
        /// <summary>
        /// This is called when the game should draw it
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            var viewMaxtrix = gameCamera.GetTransform();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied,
                                    null, null, null, null, viewMaxtrix * Matrix.CreateScale(GetScreenScale()));
            switch (State)
            {
                case GameState.LEVEL:
                    foreach (IGameObject go in gameObjects)
                        go.Draw(spriteBatch);

                    player.Draw(spriteBatch);
                    hud.Draw(spriteBatch);
                    crosshair.Draw(spriteBatch);
                    break;
                case GameState.MAIN_MENU:
                    menu.Draw(spriteBatch);
                    break;
                case GameState.OPTIONS_MENU:
                    throw new NotImplementedException();
                    break;
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
        #endregion

        #region Create Methods
        private void CreatePlayer()
        {
            player = new Player(playerSpriteSheet, this, physicsWorld);
            player.Position = ConvertUnits.ToSimUnits(100, 150);
            player.GiveGun(RocketLauncher);
            //player.GiveGun(handGun);
            player.GiveGun(shotGun);
            player.GiveGun(assualtRifle);
            
        }

        private void CreateWinPoint(Vector2 pos)
        {
            WinPoint winPoint = new WinPoint(winPointTexture, physicsWorld);
            winPoint.OnCollision += new OnCollisionEventHandler(WinPoint_OnCollision);
            winPoint.Position = pos;
            gameObjects.Add(winPoint);
        }

        private void CreateFloor(Vector2 pos)
        {
            Floor f = new Floor(floorTexture, this, physicsWorld);
            f.Position = pos;
            gameObjects.Add(f);   
        }

        private void CreateFloors()
        {
            for (int i = 0; i < 5; i++)
            {
                Floor f = new Floor(floorTexture, this, physicsWorld);
                
                f.Position = ConvertUnits.ToSimUnits((ScreenWidth / 2) + (i * floorTexture.Width) - 100, ScreenHeight - 10);
                gameObjects.Add(f);
            }

            Floor foo = new Floor(floorTexture, this, physicsWorld);
            
            foo.Position = ConvertUnits.ToSimUnits(900, 100);
            gameObjects.Add(foo);
        }

        private void CreateWall(Vector2 pos)
        {
            Wall w = new Wall(wallTexture, this, physicsWorld);

            w.Position = pos;
            gameObjects.Add(w);
        }

        private void CreateWalls()
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Wall w = new Wall(wallTexture, this, physicsWorld);
                    
                    w.Position = ConvertUnits.ToSimUnits(i * (floorTexture.Width * 5), ScreenHeight - (j * wallTexture.Height));
                    gameObjects.Add(w);
                }
            }
        }

        public void CreateBullet(int damage, float rotation, float muzzleVelocity)
        {
            Bullet bulletBody = new Bullet(bulletTexture, this, damage, physicsWorld);

            bulletBody.Position = ConvertUnits.ToSimUnits(player.GunLocation());
            bulletBody.Rotation = rotation;
            bulletBody.OnCollision += new OnCollisionEventHandler(Bullet_OnCollision);
            bulletBody.ApplyForce(new Vector2(-(float)Math.Cos(rotation) * muzzleVelocity, -(float)Math.Sin(rotation) * muzzleVelocity));
            bulletBody.IgnoreGravity = true;
            gameObjects.Add(bulletBody);
        }

        private void CreateShotgunBullet(int damage, float rotation, float muzzleVelocity)
        {
            for (int i = 0; i < 5; i++)
            {
                Bullet bulletBody = new Bullet(bulletTexture, this, damage, physicsWorld);

                float rotDiff = (i - 2) * 0.1f;
                bulletBody.Position = ConvertUnits.ToSimUnits(player.GunLocation());
                bulletBody.Rotation = rotation + rotDiff;
                bulletBody.OnCollision += new OnCollisionEventHandler(Bullet_OnCollision);
                bulletBody.ApplyForce(new Vector2(-(float)Math.Cos(rotation + rotDiff) * muzzleVelocity, -(float)Math.Sin(rotation + rotDiff) * muzzleVelocity));
                bulletBody.IgnoreGravity = true;
                gameObjects.Add(bulletBody);
            }
        }

        internal void CreateRocket(int damage, float rotation, float muzzleVelocity)
        {
            Rocket rocketBody = new Rocket(rocketTexture, this, damage, physicsWorld);
            
            rocketBody.Position = ConvertUnits.ToSimUnits(player.GunLocation());
            rocketBody.Rotation = rotation;
            rocketBody.OnCollision += new OnCollisionEventHandler(Rocket_OnCollision);
            rocketBody.LinearVelocity = new Vector2(-(float)Math.Cos(rotation) * muzzleVelocity, -(float)Math.Sin(rotation) * muzzleVelocity);
            
            gameObjects.Add(rocketBody);
        }

        internal void CreateEnemies()
        {
            Random r = new Random();
            for (int i = 0; i < 8; i++)
            {
                Robot robot = new Robot(robotTexture, this, physicsWorld);
                robot.Position = ConvertUnits.ToSimUnits(((i + 1) * 400) + r.NextDouble() * 200, 100 + r.NextDouble() * 200);
                gameObjects.Add(robot);
            }

            for (int i = 0; i < 8; i++)
            {
                SuicideRobot robot = new SuicideRobot(suicideRobotTexture, emissionSpriteSheet, this, physicsWorld);
                robot.Position = ConvertUnits.ToSimUnits(((i + 1) * 500) + r.NextDouble() * 200, 400 + r.NextDouble() * 100);
                robot.OnCollision += new OnCollisionEventHandler(SuicideRobot_OnCollision);
                gameObjects.Add(robot);
            }
        }

        internal void CreateRobot(Vector2 pos)
        {
            Robot robot = new Robot(robotTexture, this, physicsWorld);
            robot.Position = pos;
            gameObjects.Add(robot);
        }

        internal void CreateSuicideRobot(Vector2 pos)
        {
            SuicideRobot robot = new SuicideRobot(suicideRobotTexture, emissionSpriteSheet, this, physicsWorld);
            robot.Position = pos;
            robot.OnCollision += new OnCollisionEventHandler(SuicideRobot_OnCollision);
            gameObjects.Add(robot);
        }

        internal void CreateLaser(int damage, float rotation, float muzzleVelocity, Vector2 location)
        {
            Bullet bulletBody = new Bullet(laserTexture, this, damage, physicsWorld);
            bulletBody.Position = location;
            bulletBody.Rotation = rotation;
            bulletBody.OnCollision += new OnCollisionEventHandler(Laser_OnCollision);
            bulletBody.ApplyForce(new Vector2(-(float)Math.Cos(rotation) * muzzleVelocity, -(float)Math.Sin(rotation) * muzzleVelocity));
            
            gameObjects.Add(bulletBody);
        }
        #endregion

        #region event handlers
        private bool Bullet_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            if (fixtureB.Body.GetType() == typeof(Bullet) || fixtureB.Body.GetType() == typeof(Rocket))
                return false;

            if (fixtureA.Body.GetType() == typeof(Bullet) && gameObjects.Contains((Bullet)fixtureA.Body))
            {
                physicsWorld.RemoveBody(fixtureA.Body);
                gameObjects.Remove((Bullet)fixtureA.Body);
                if (fixtureB.Body is ILivingThing)
                {
                    ILivingThing hitObject = fixtureB.Body as ILivingThing;
                    Bullet bull = fixtureA.Body as Bullet;
                    hitObject.DealDamage(bull.Damage);
                }
                return true;
            }
            return false;
        }

        private bool Laser_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            //Pass through other robots
            if(fixtureB.Body is Robot)
                return false;

            if (fixtureA.Body.GetType() == typeof(Bullet) && gameObjects.Contains((Bullet)fixtureA.Body))
            {
                physicsWorld.RemoveBody(fixtureA.Body);
                gameObjects.Remove((Bullet)fixtureA.Body);
                if (fixtureB.Body is Player)
                {
                    ILivingThing hitObject = fixtureB.Body as ILivingThing;
                    Bullet bull = fixtureA.Body as Bullet;
                    hitObject.DealDamage(bull.Damage);
                }
                return true;
            }
            return true;
        }

        private bool SuicideRobot_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (fixtureB.Body is Player)
            {
                physicsWorld.RemoveBody(fixtureA.Body);
                gameObjects.Remove((SuicideRobot)fixtureA.Body);
                Player hitObject = fixtureB.Body as Player;
                hitObject.DealDamage(15); 
                return true;
            }

            return false;
        }

        private bool WinPoint_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            if (fixtureB.Body is Player)
            {
                 ClearWorld();
                currentLevel++;
                if (currentLevel == singlePlayerLevels.Count) //TODO: win game screen
                {
                    GoToMainMenu();
                    return true;
                }
                gameObjects = new List<IGameObject>();
                //ClearWorld();
                //CreateGameObjectsFromLevel(singlePlayerLevels[currentLevel].Objects);
                //singlePlayerLevels[currentLevel].PlayMusic();
                respawnPlayer();
            }

            return true;
        }

        private void ClearWorld()
        {
            for (int i = gameObjects.Count - 1; i > 0; i--)
            {
                physicsWorld.RemoveBody((Body)gameObjects[i]);
            }
        }

        private bool Rocket_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (fixtureB.Body is Player)
                return false;

            if (fixtureB.Body is Bullet)
                return false;

            if (fixtureB.Body is Rocket)
                return false;

            //if (fixtureB.Body is Wall)
                //return true;

            Vector2 bodyPos = fixtureA.Body.Position;

            Vector2 min = bodyPos - new Vector2(1, 1);
            Vector2 max = bodyPos + new Vector2(1, 1);

            AABB aabb = new AABB(ref min, ref max);
            List<Fixture> fixtures = new List<Fixture>();

            physicsWorld.QueryAABB(fixture =>
            {
                fixtures.Add(fixture);
                return true;
            }, ref aabb);

            foreach (Fixture f in fixtures)
            {
                //Dont apply forces to static objects, bullets, or other rockets
                if (!f.Body.IsStatic && f.Body.GetType() != typeof(Bullet) && f.Body.GetType() != typeof(Rocket))
                {
                    Vector2 fv = f.Body.Position - bodyPos;
                    //fv.Normalize();
                    fv *= 5;
                    f.Body.ApplyLinearImpulse(ref fv);
                }
            }

            IGameObject obj = fixtureA.Body as IGameObject;
            if (!obj.MarkedForRemoval)
            {
                obj.MarkedForRemoval = true;
                physicsWorld.RemoveBody(fixtureA.Body);
                gameObjects.Remove((Rocket)fixtureA.Body);
            }

            //foreach (Body body in physicsWorld.QueryAABB(new FarseerPhysics.Collision.AABB(fixtureA.Body.Position, )
            //{
            //    Vector2 vectorFromExplosion = body.Position - explosion.Position;

            //    // Note that here we're using a linear falloff. If you want the explosion force on this body to fall off more sharply with distance, you can use vectorFromExplosion.LengthSquared() instead.
            //    float explosionForce = (explosion.Radius - vectorFromExplosion.Length()) * explosion.Force;

            //    Vector2 forceOnBody = Vector2.Normalize(vectorFromExplosion);
            //    forceOnBody *= explosionForce;

            //    body.ApplyForce(forceOnBody);
            //}
            return false;
        }


        #endregion

        #region private Helpers
        private Vector3 GetScreenScale()
        {
            var scaleX = (float)GraphicsDevice.Viewport.Width / (float)ScreenWidth;
            var scaleY = (float)GraphicsDevice.Viewport.Height / (float)ScreenHeight;
            return new Vector3(scaleX, scaleY, 1.0f);
        }

        private void TryPlay(Song backgroundMusic)
        {
            try
            {
                MediaPlayer.Play(backgroundMusic);
            }
            catch (Exception)
            {
                MediaPlayer.Play(backgroundMusic);
            }
        }

        private Dictionary<Type, List<Vector2>> GetLevelDictFromGameObjects(List<IGameObject> gameObjects)
        {
            Dictionary<Type, List<Vector2>> objects = new Dictionary<Type, List<Vector2>>();
            foreach (IGameObject obj in gameObjects)
            {
                if (obj is Body)
                {
                    Body b = obj as Body;
                    if (b is Floor)
                    {
                        Floor f = b as Floor;
                        if (objects.ContainsKey(f.GetType()))
                            objects[f.GetType()].Add(b.Position);
                        else
                        {
                            objects.Add(f.GetType(), new List<Vector2>());
                            objects[f.GetType()].Add(b.Position);
                        }
                        
                    }
                    else if (b is Wall)
                    {
                        Wall w = b as Wall;
                        if (objects.ContainsKey(w.GetType()))
                            objects[w.GetType()].Add(b.Position);
                        else
                        {
                            objects.Add(w.GetType(), new List<Vector2>());
                            objects[w.GetType()].Add(b.Position);
                        }
                    }
                    else if (b is Robot)
                    {
                        Robot r = b as Robot;
                        if (objects.ContainsKey(r.GetType()))
                            objects[r.GetType()].Add(b.Position);
                        else
                        {
                            objects.Add(r.GetType(), new List<Vector2>());
                            objects[r.GetType()].Add(b.Position);
                        };
                    }
                    else if (b is SuicideRobot)
                    {
                        SuicideRobot sr = b as SuicideRobot;
                        if (objects.ContainsKey(sr.GetType()))
                            objects[sr.GetType()].Add(b.Position);
                        else
                        {
                            objects.Add(sr.GetType(), new List<Vector2>());
                            objects[sr.GetType()].Add(b.Position);
                        }
                    }
                    else if (b is WinPoint)
                    {
                        WinPoint wp = b as WinPoint;
                        if (objects.ContainsKey(wp.GetType()))
                            objects[wp.GetType()].Add(b.Position);
                        else
                        {
                            objects.Add(wp.GetType(), new List<Vector2>());
                            objects[wp.GetType()].Add(b.Position);
                        }
                    }

                }
            }

            return objects;
        }

        private void CreateGameObjectsFromLevel(Dictionary<Type, List<Vector2>> dictionary)
        {
            foreach (KeyValuePair<Type, List<Vector2>> kvp in dictionary)
            {
                if (kvp.Key == typeof(Floor))
                {
                    foreach(Vector2 pos in kvp.Value)
                        CreateFloor(pos);
                }
                else if (kvp.Key == typeof(Wall))
                {
                    foreach (Vector2 pos in kvp.Value)
                        CreateWall(pos);
                }
                else if (kvp.Key == typeof(Robot))
                {
                    foreach (Vector2 pos in kvp.Value)
                        CreateRobot(pos);
                }
                else if (kvp.Key == typeof(SuicideRobot))
                {
                    foreach (Vector2 pos in kvp.Value)
                        CreateSuicideRobot(pos);
                }
                else if(kvp.Key == typeof(WinPoint))
                {
                    foreach (Vector2 pos in kvp.Value)
                        CreateWinPoint(pos);
                }
            }
        }
        #endregion

        #region State transitions
        public void GoToMainMenu()
        {
            gameCamera.Position = ConvertUnits.ToSimUnits(0f, 0f);
            CameraOffset = gameCamera.Position;
            State = GameState.MAIN_MENU;
            IsMouseVisible = true;
            TryPlay(menuMusic);
        }

        public void StartGame()
        {
            State = GameState.LEVEL;
            currentLevel = 0;
            MediaPlayer.Stop();
            singlePlayerLevels[currentLevel].PlayMusic();
            IsMouseVisible = false;
        }

        internal void GoToOptionsMenu()
        {
            State = GameState.OPTIONS_MENU;
            IsMouseVisible = true;
        }
        #endregion

        #region Cleanup
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        #endregion

        #region public helpers
        public Vector2 GetPlayerLocation()
        {
            return player.Position;
        }
        #endregion




        

        
    }
}
