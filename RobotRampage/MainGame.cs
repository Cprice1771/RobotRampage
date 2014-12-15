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
using MediaPlayerHelper;
using System.IO;
#endregion

namespace RobotRampage
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainGame : Game
    {
        #region constants
        const int X_CAMERA_THRESHOLD = 500;
        public const int ScreenWidth = 1920;
        public const int ScreenHeight = 1080;
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
        
        World physicsWorld;
        int mouseWheelLoc;
        List<Level> singlePlayerLevels;
        int currentLevel;
        MainMenu mainMenu;
        LevelCompleteMenu levelCompleteMenu;
        string contentPath;
        Rectangle levelDimensions;

        #region Audio
        SongFile levelMusic;
        SongFile menuMusic;
        SongFile deathEffect;
        SongFile explosionEffect;
        SongFile JumpEffect;
        SongFile rifleShootSound;
        SongFile shotgunShootSound;
        SongFile emptyGunSound;
        SongFile laserGunSound;
        SongFile rocketLauncherSound;
        SongFile successSound;
        #endregion

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
        Texture2D Title;
        Texture2D PlayGameTexture;
        Texture2D OptionsMenuTexture;
        Texture2D spawnPointTexture;
        Texture2D handGunTexture;
        Texture2D shotGunTexture;
        Texture2D RobotSpriteSheet;
        Texture2D rocketLauncherTexture;
        Texture2D ExplosionSpriteSheet;
        Texture2D SpikeTexture;
        Texture2D healthTextureSheet;
        Texture2D smokeParticleTexture;
        Texture2D backgroundTexture;
        Texture2D nextLevelTexture;
        Texture2D mainMenuTexture;
        Texture2D levelCompleteTexture;
        Texture2D doodadTexture;
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
        Color _backgroundColor;
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
            mainMenu = new MainMenu(Title, PlayGameTexture, OptionsMenuTexture, this);
            levelCompleteMenu = new LevelCompleteMenu(levelCompleteTexture, nextLevelTexture, mainMenuTexture, this);
            currentLevel = 0;
            
            //singlePlayerLevels = CreateLevels();
            singlePlayerLevels = new List<Level>();
            singlePlayerLevels = ReadInLevels();
            levelDimensions = singlePlayerLevels[currentLevel].GetLevelDimensions();
            CreatePlayer();
            hud = new HUD(player.Inventory, player.EquipedWeaponSlot, new Vector2(0, 0), font, this, new HealthBar(healthTextureSheet, this));
            gameCamera = new Camera();
            MediaPlayerHelper.MediaPlayerHelper.Instance.Loop = true;
            MediaPlayerHelper.MediaPlayerHelper.Instance.Play(menuMusic);
            IsMouseVisible = true;
            _backgroundColor = Color.Black;
        }


        //private List<Level> CreateLevels()
        //{
        //    CreateEnemies();
        //    CreateFloors();
        //    CreateWalls();
        //    CreateWinPoint(ConvertUnits.ToSimUnits(1500, 700));
        //    List<Level> levels = new List<Level>();
        //    Dictionary<Type, List<List<float>>> levelObjects = GetLevelDictFromGameObjects(gameObjects);
        //    Level level1 = new Level(levelObjects, new Vector2(ConvertUnits.ToSimUnits(150), ConvertUnits.ToSimUnits(700)), levelMusic, "Level 1");
        //    levels.Add(level1);
        //    Rocket r = new Rocket(rocketTexture, this, 50, physicsWorld);
        //    r.Position = ConvertUnits.ToSimUnits(100, 100);
        //    Level level2 = new Level(levelObjects, new Vector2(ConvertUnits.ToSimUnits(150), ConvertUnits.ToSimUnits(700)), levelMusic, "Level 2");
        //    levels.Add(level2);

        //    return levels;
        //}

        

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            LoadTextures();
            contentPath = System.IO.Path.Combine(Environment.CurrentDirectory + "\\Content");

            levelMusic = new SongFile(System.IO.Path.Combine(contentPath + "\\FailingDefense.wma"));

            

            menuMusic = new SongFile(System.IO.Path.Combine(contentPath + "\\In a Heartbeat.mp3"));
            deathEffect = new SongFile(System.IO.Path.Combine(contentPath + "\\death.wav"));
            explosionEffect = new SongFile(System.IO.Path.Combine(contentPath + "\\explosion.wav"));
            JumpEffect = new SongFile(System.IO.Path.Combine(contentPath + "\\jump.wav"));
            rifleShootSound = new SongFile(System.IO.Path.Combine(contentPath + "\\smg.wav"));
            shotgunShootSound = new SongFile(System.IO.Path.Combine(contentPath + "\\shotgun.wav"));
            emptyGunSound = new SongFile(System.IO.Path.Combine(contentPath + "\\empty.wav"));
            laserGunSound = new SongFile(System.IO.Path.Combine(contentPath + "\\laser.wav"));
            rocketLauncherSound = new SongFile(System.IO.Path.Combine(contentPath + "\\rocketShoot.wav"));
            successSound = new SongFile(System.IO.Path.Combine(contentPath + "\\Success.mp3"));
            //TODO: uncomment for music
            //TryPlay(backgroundMusic);

            font = Content.Load<SpriteFont>("myFont");


            //var screen = Screen.AllScreens.First(e => e.Primary);
            //Window.IsBorderless = true;
            //Window.Position = new Point(screen.Bounds.X, screen.Bounds.Y);
            //graphics.PreferredBackBufferWidth = screen.Bounds.Width;
            //graphics.PreferredBackBufferHeight = screen.Bounds.Height;

            graphics.PreferredBackBufferHeight = ScreenHeight;
            graphics.PreferredBackBufferWidth = ScreenWidth;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            Vector2 initialPlayerPosition = new Vector2(ScreenWidth / 2, 150);
            
            crosshair = new Crosshair(crosshairTexture, new Vector2(ScreenWidth / 2, ScreenHeight / 2), this);
            assualtRifle = new Gun(defaultGunTexture, initialPlayerPosition, this, 35, 30, 7.5f, 200, 100.0, 1000.0, CreateBullet, rifleShootSound, emptyGunSound);
            shotGun = new Gun(shotGunTexture, initialPlayerPosition, this, 50, 5, 7.5f, 20, 1000.0, 1500.0, CreateShotgunBullet, shotgunShootSound, emptyGunSound);
            //handGun = new Gun(handGunTexture, initialPlayerPosition, this, 40, 10, 5.0f, 200, 500.0, 800.0, CreateBullet, rifleShootSound, emptyGunSound);
            RocketLauncher = new Gun(rocketLauncherTexture, initialPlayerPosition, this, 100, 4, 7.5f, 20, 800.0, 1500.0, CreateRocket, rocketLauncherSound, emptyGunSound);
            mouseWheelLoc = Mouse.GetState().ScrollWheelValue;
            
        }

       

        

        private void LoadTextures()
        {
            crosshairTexture = Content.Load<Texture2D>("crosshair");
            playerSpriteSheet = Content.Load<Texture2D>("PlayerSpriteSheet");
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
            spawnPointTexture = Content.Load<Texture2D>("SpawnPoint");
            Title = Content.Load<Texture2D>("Title.png");
            PlayGameTexture = Content.Load<Texture2D>("PlayGame");
            OptionsMenuTexture = Content.Load<Texture2D>("Options");
            handGunTexture = Content.Load<Texture2D>("handgun");
            shotGunTexture = Content.Load<Texture2D>("shotgun");
            RobotSpriteSheet = Content.Load<Texture2D>("RobotSpriteSheet");
            rocketLauncherTexture = Content.Load<Texture2D>("RocketLauncher");
            ExplosionSpriteSheet = Content.Load<Texture2D>("ExplosionSpriteSheet");
            SpikeTexture = Content.Load<Texture2D>("spikes");
            healthTextureSheet = Content.Load<Texture2D>("hpBars");
            smokeParticleTexture = Content.Load<Texture2D>("smoke");
            backgroundTexture = Content.Load<Texture2D>("background");
            nextLevelTexture = Content.Load<Texture2D>("nextlevel");
            mainMenuTexture = Content.Load<Texture2D>("mainmenu");
            levelCompleteTexture = Content.Load<Texture2D>("levelcomplete");
            doodadTexture = Content.Load<Texture2D>("doodad");
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

                    if (playerY > gameCamera.Position.Y + 700 && gameCamera.Position.Y > 0)
                        gameCamera.Move(new Vector2(0, -1 * (ConvertUnits.ToDisplayUnits(player.Position.Y) - (-1 * (gameCamera.Position.Y) + 400))));
                    else if (playerY < gameCamera.Position.Y + Y_CAMERA_THRESHOLD + 100)
                        gameCamera.Move(new Vector2(0, -1 * (ConvertUnits.ToDisplayUnits(player.Position.Y) - (-1 * (gameCamera.Position.Y) + Y_CAMERA_THRESHOLD))));
           
                    CameraOffset = gameCamera.Position;
                    #endregion

                    #region Keyboard inputs

                    if (player.LinearVelocity.Y != 0)
                        player.State = PlayerState.JUMPING;
                    else if (Keyboard.GetState().IsKeyDown(Keys.D) || Keyboard.GetState().IsKeyDown(Keys.A))
                        player.State = PlayerState.RUNNING;
                    else
                        player.State = PlayerState.IDLE;

                    //TODO: fix ground colloision detection
                    if (Keyboard.GetState().IsKeyDown(Keys.Space) && player.LinearVelocity.Y == 0)
                    {
                        MediaPlayerHelper.MediaPlayerHelper.Instance.PlaySound(JumpEffect);
                        player.ApplyForce(new Vector2(0, -200.0f));
                    }

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
                                if(!(deadObject is Player))
                                {
                                    Body deadBody = deadObject as Body;
                                    CreateExplosion(deadBody.Position);

                                }
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
                        else if (gameObjects[i] is Explosion)
                        {
                            Explosion explosion = gameObjects[i] as Explosion;
                            if(!explosion.IsAlive)
                                gameObjects.RemoveAt(i);
                        }
                        else if (gameObjects[i] is SmokeParticle)
                        {
                            SmokeParticle smoke = gameObjects[i] as SmokeParticle;
                            if (!smoke.IsAlive)
                                gameObjects.RemoveAt(i);
                        }
                    }
                    if (player.Health <= 0)
                    {
                        MediaPlayerHelper.MediaPlayerHelper.Instance.PlaySound(deathEffect);
                        respawnPlayer();
                    }
                    #endregion

                    #region Misc Updates
                    player.Update(gameTime);
                    crosshair.Update(gameTime);

                    for (int i = gameObjects.Count - 1; i >= 0; i--)
                        gameObjects[i].Update(gameTime);

                    #endregion
                    break;
                case GameState.MAIN_MENU:
                    mainMenu.Update(gameTime);
                    gameCamera.Position = Vector2.Zero;
                    break;
                case GameState.LEVEL_COMPLETE:
                    levelCompleteMenu.Update(gameTime);
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
            GraphicsDevice.Clear(_backgroundColor);
            var viewMaxtrix = gameCamera.GetTransform();

            if (State == GameState.LEVEL)
            {
                spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Opaque, SamplerState.LinearWrap,
    DepthStencilState.Default, RasterizerState.CullNone, null, viewMaxtrix * Matrix.CreateScale(new Vector3(GetScreenScale().X, GetScreenScale().Y, GetScreenScale().Z)));
                spriteBatch.Draw(backgroundTexture, levelDimensions, levelDimensions, Color.White);
                spriteBatch.End();
            }

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
                    mainMenu.Draw(spriteBatch);
                    break;
                case GameState.LEVEL_COMPLETE:
                    levelCompleteMenu.Draw(spriteBatch);
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
            //player.Position = new Vector2(singlePlayerLevels[currentLevel].Spawn.location.X + ConvertUnits.ToSimUnits(spawnPointTexture.Width / 2), singlePlayerLevels[currentLevel].Spawn.location.Y + ConvertUnits.ToSimUnits(spawnPointTexture.Height / 2));
            player.Position = new Vector2(singlePlayerLevels[currentLevel].SpawnLocation.X, singlePlayerLevels[currentLevel].SpawnLocation.Y);
            
            assualtRifle = new Gun(defaultGunTexture, player.Position, this, 35, 30, 7.5f, 200, 100.0, 1000.0, CreateBullet, rifleShootSound, emptyGunSound);
            shotGun = new Gun(shotGunTexture, player.Position, this, 50, 5, 7.5f, 20, 1000.0, 1500.0, CreateShotgunBullet, shotgunShootSound, emptyGunSound);
            //handGun = new Gun(handGunTexture, player.Position, this, 40, 10, 5.0f, 200, 500.0, 800.0, CreateBullet, rifleShootSound, emptyGunSound);
            RocketLauncher = new Gun(rocketLauncherTexture, player.Position, this, 100, 4, 7.5f, 20, 800.0, 1500.0, CreateRocket, rocketLauncherSound, emptyGunSound);

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

        private void CreateSpawnPoint(Vector2 pos)
        {
            SpawnPoint spawnPoint = new SpawnPoint(pos, spawnPointTexture);
            gameObjects.Add(spawnPoint);
        }

        private void CreateDoodad(Vector2 vector2)
        {
            Doodad f = new Doodad(vector2, doodadTexture);
            gameObjects.Add(f); 
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

                f.Position = ConvertUnits.ToSimUnits((floorTexture.Width / 2) + (i * floorTexture.Width), ScreenHeight - 10);
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
                Robot robot = new Robot(RobotSpriteSheet, this, physicsWorld);
                robot.Position = ConvertUnits.ToSimUnits(((i + 1) * 400) + r.NextDouble() * 200, 100 + r.NextDouble() * 200);
                gameObjects.Add(robot);
            }

            for (int i = 0; i < 8; i++)
            {
                SuicideRobot robot = new SuicideRobot(RobotSpriteSheet, emissionSpriteSheet, this, physicsWorld);
                robot.Position = ConvertUnits.ToSimUnits(((i + 1) * 500) + r.NextDouble() * 200, 400 + r.NextDouble() * 100);
                robot.OnCollision += new OnCollisionEventHandler(SuicideRobot_OnCollision);
                gameObjects.Add(robot);
            }
        }

        internal void CreateRobot(Vector2 pos)
        {
            Robot robot = new Robot(RobotSpriteSheet, this, physicsWorld);
            robot.Position = pos;
            gameObjects.Add(robot);
        }

        internal void CreateSuicideRobot(Vector2 pos)
        {
            SuicideRobot robot = new SuicideRobot(RobotSpriteSheet, emissionSpriteSheet, this, physicsWorld);
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

        internal void CreateSpike(Vector2 location, float rotation)
        {
            Spikes spikeBody = new Spikes(SpikeTexture, this, physicsWorld, rotation);
            spikeBody.Position = location;
            spikeBody.OnCollision += new OnCollisionEventHandler(Spike_OnCollision);

            gameObjects.Add(spikeBody);
        }

        private void CreateExplosion(Vector2 pos)
        {
            if(OnScreen(pos))
                MediaPlayerHelper.MediaPlayerHelper.Instance.PlaySound(explosionEffect);
            Explosion ex = new Explosion(ExplosionSpriteSheet, this, pos);
            gameObjects.Add(ex);
        }

        

        public void CreateSmoke(Vector2 pos)
        {
            SmokeParticle ex = new SmokeParticle(this, smokeParticleTexture, pos);
            gameObjects.Add(ex);
        }
        

        
        #endregion

        #region event handlers
        private bool Spike_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if ((typeof(ILivingThing).IsAssignableFrom(fixtureB.Body.GetType())))
            {
                ILivingThing hitObject = fixtureB.Body as ILivingThing;
                hitObject.DealDamage(100);
                return true;
            }
            return true;
        }

        private bool Bullet_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            if (fixtureB.Body.GetType() == typeof(Bullet) || fixtureB.Body.GetType() == typeof(Rocket) || fixtureB.Body.GetType() == typeof(Player))
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
                CreateExplosion(fixtureA.Body.Position);
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
                GoToWinLevelMenmu();
            }

            return true;
        }

        

        private void ClearWorld()
        {
            for (int i = gameObjects.Count - 1; i > 0; i--)
            {
                if(gameObjects[i] is Body)
                    physicsWorld.RemoveBody((Body)gameObjects[i]);
            }
        }

        private bool Rocket_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            IGameObject obj = fixtureA.Body as IGameObject;

            if (fixtureB.Body.GetType() == typeof(Bullet) || fixtureB.Body.GetType() == typeof(Rocket) || fixtureB.Body.GetType() == typeof(Player) || obj.MarkedForRemoval)
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
                    fv.Normalize();

                    //if (fv.X > 0)
                    //    fv.X = 1 - fv.X;
                    //else
                    //    fv.X = -1 - fv.X;

                    //if (fv.Y > 0)
                    //    fv.Y = 1 - fv.Y;
                    //else
                    //    fv.Y = -1 - fv.Y;


                    fv *= 5;
                    f.Body.ApplyLinearImpulse(ref fv);
                }
            }

            
            if (!obj.MarkedForRemoval)
            {
                obj.MarkedForRemoval = true;
                physicsWorld.RemoveBody(fixtureA.Body);
                gameObjects.Remove((Rocket)fixtureA.Body);
            }

            CreateExplosion(fixtureA.Body.Position);

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
        private bool OnScreen(Vector2 pos)
        {
            if ((ConvertUnits.ToDisplayUnits(pos.Y)) + CameraOffset.Y < MainGame.ScreenHeight && (ConvertUnits.ToDisplayUnits(pos.Y)) + CameraOffset.Y > 0 &&
                (ConvertUnits.ToDisplayUnits(pos.X)) + CameraOffset.X < MainGame.ScreenWidth && (ConvertUnits.ToDisplayUnits(pos.X)) + CameraOffset.X > 0)
                return true;

            return false;
        }

        private List<Level> ReadInLevels()
        {
            string[] levels = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\content");
            List<Level> tempLevels = new List<Level>();
            foreach (string file in levels)
            {
                if(file.Contains(".map"))
                {
                    tempLevels.Add(new Level(file));
                }

            }

            return tempLevels;
        }

        private Vector3 GetScreenScale()
        {
            var scaleX = (float)GraphicsDevice.Viewport.Width / (float)ScreenWidth;
            var scaleY = (float)GraphicsDevice.Viewport.Height / (float)ScreenHeight;
            return new Vector3(scaleX, scaleY, 1.0f);
        }

        //private void TryPlay(Song backgroundMusic)
        //{
        //    try
        //    {
        //        MediaPlayer.Play(backgroundMusic);
        //    }
        //    catch (Exception)
        //    {
        //        MediaPlayer.Play(backgroundMusic);
        //    }
        //}

        private Dictionary<Type, List<List<float>>> GetLevelDictFromGameObjects(List<IGameObject> gameObjects)
        {
            Dictionary<Type, List<List<float>>> objects = new Dictionary<Type, List<List<float>>>();
            foreach (IGameObject obj in gameObjects)
            {
                if (obj is Body)
                {
                    Body b = obj as Body;
                    if (b is Floor)
                    {
                        Floor f = b as Floor;
                        if (objects.ContainsKey(f.GetType()))
                            objects[f.GetType()].Add(new List<float>() { b.Position.X, b.Position.Y, b.Rotation });
                        else
                        {
                            objects.Add(f.GetType(), new List<List<float>>());
                            objects[f.GetType()].Add(new List<float>() { b.Position.X, b.Position.Y, b.Rotation });
                        }
                        
                    }
                    else if (b is Wall)
                    {
                        Wall w = b as Wall;
                        if (objects.ContainsKey(w.GetType()))
                            objects[w.GetType()].Add(new List<float>() { b.Position.X, b.Position.Y, b.Rotation });
                        else
                        {
                            objects.Add(w.GetType(), new List<List<float>>());
                            objects[w.GetType()].Add(new List<float>() { b.Position.X, b.Position.Y, b.Rotation });
                        }
                    }
                    else if (b is Robot)
                    {
                        Robot r = b as Robot;
                        if (objects.ContainsKey(r.GetType()))
                            objects[r.GetType()].Add(new List<float>() { b.Position.X, b.Position.Y, b.Rotation });
                        else
                        {
                            objects.Add(r.GetType(), new List<List<float>>());
                            objects[r.GetType()].Add(new List<float>() { b.Position.X, b.Position.Y, b.Rotation });
                        };
                    }
                    else if (b is SuicideRobot)
                    {
                        SuicideRobot sr = b as SuicideRobot;
                        if (objects.ContainsKey(sr.GetType()))
                            objects[sr.GetType()].Add(new List<float>() { b.Position.X, b.Position.Y, b.Rotation });
                        else
                        {
                            objects.Add(sr.GetType(), new List<List<float>>());
                            objects[sr.GetType()].Add(new List<float>() { b.Position.X, b.Position.Y, b.Rotation });
                        }
                    }
                    else if (b is WinPoint)
                    {
                        WinPoint wp = b as WinPoint;
                        if (objects.ContainsKey(wp.GetType()))
                            objects[wp.GetType()].Add(new List<float>() { b.Position.X, b.Position.Y, b.Rotation });
                        else
                        {
                            objects.Add(wp.GetType(), new List<List<float>>());
                            objects[wp.GetType()].Add(new List<float>() { b.Position.X, b.Position.Y, b.Rotation });
                        }
                    }

                }
            }

            return objects;
        }


        private void CreateGameObjectsFromLevel(Dictionary<Type, List<List<float>>> dictionary)
        {
            //IMPORTANT
            //Order matters here, the lower in the tree the object is the further towards the front of the screen it will be drawn
            //IE. the first object has the lowest priority, last one has highest
            foreach (KeyValuePair<Type, List<List<float>>> kvp in dictionary)
            {
                if (kvp.Key == typeof(Floor))
                {
                    foreach(List<float> data in kvp.Value)
                        CreateFloor(new Vector2(data[0], data[1]));
                }
                else if (kvp.Key == typeof(Doodad))
                {
                    foreach (List<float> data in kvp.Value)
                        CreateDoodad(new Vector2(data[0], data[1]));
                }
                else if (kvp.Key == typeof(Wall))
                {
                    foreach (List<float> data in kvp.Value)
                        CreateWall(new Vector2(data[0], data[1]));
                }
                else if (kvp.Key == typeof(Spikes))
                {
                    foreach (List<float> data in kvp.Value)
                        CreateSpike(new Vector2(data[0], data[1]), data[2]);
                }
                else if (kvp.Key == typeof(WinPoint))
                {
                    foreach (List<float> data in kvp.Value)
                        CreateWinPoint(new Vector2(data[0], data[1]));
                }
                else if (kvp.Key == typeof(SpawnPoint))
                {
                    foreach (List<float> data in kvp.Value)
                        CreateSpawnPoint(new Vector2(data[0], data[1]));
                }
                else if (kvp.Key == typeof(Robot))
                {
                    foreach (List<float> data in kvp.Value)
                        CreateRobot(new Vector2(data[0], data[1]));
                }
                else if (kvp.Key == typeof(SuicideRobot))
                {
                    foreach (List<float> data in kvp.Value)
                        CreateSuicideRobot(new Vector2(data[0], data[1]));
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
            MediaPlayerHelper.MediaPlayerHelper.Instance.Play(menuMusic);
            _backgroundColor = Color.Black;

            
        }

        public void StartGame()
        {
            currentLevel = 0;
            gameObjects = new List<IGameObject>();
            respawnPlayer();
            State = GameState.LEVEL;

            MediaPlayerHelper.MediaPlayerHelper.Instance.Play(levelMusic);
            IsMouseVisible = false;
            _backgroundColor = Color.Black;

            //foreach (Level l in singlePlayerLevels)
            //    l.Write();
        }

        internal void GoToOptionsMenu()
        {
            State = GameState.OPTIONS_MENU;
            IsMouseVisible = true;
            _backgroundColor = Color.White;
        }

        internal void NextLevel()
        {
            IsMouseVisible = false;
            ClearWorld();
            currentLevel++;
            if (currentLevel == singlePlayerLevels.Count) //TODO: win game screen
            {
                GoToMainMenu();
                return;
            }
            levelDimensions = singlePlayerLevels[currentLevel].GetLevelDimensions();
            gameObjects = new List<IGameObject>();
            //ClearWorld();
            //CreateGameObjectsFromLevel(singlePlayerLevels[currentLevel].Objects);
            //singlePlayerLevels[currentLevel].PlayMusic();
            respawnPlayer();
            State = GameState.LEVEL;
        }

        private void GoToWinLevelMenmu()
        {
            MediaPlayerHelper.MediaPlayerHelper.Instance.PlaySound(successSound);
            gameCamera.Position = ConvertUnits.ToSimUnits(0f, 0f);
            CameraOffset = gameCamera.Position;
            State = GameState.LEVEL_COMPLETE;
            IsMouseVisible = true;
            _backgroundColor = Color.Black;
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
