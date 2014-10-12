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
        public Camera cam;
        #endregion

        #region private members
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont font;
        Song backgroundMusic;
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
        WinPoint winPoint;
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
            cam = new Camera();
            MediaPlayer.Volume = 1.0f;
            IsMouseVisible = true;
        }

        private List<Level> CreateLevels()
        {
            //TODO create or load all our levels for the game
            CreateEnemies();
            CreateFloor();
            CreateWalls();
            CreatePlayer();
            CreateWalls();
            CreateWinPoint();
            List<Level> levels = new List<Level>();

            Level l = new Level(gameObjects, new SpawnPoint(new Vector2(ConvertUnits.ToSimUnits(100), ConvertUnits.ToSimUnits(150)), spawnPointTexture), "Level 1");
            levels.Add(l);

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

            backgroundMusic = Content.Load<Song>("FailingDefense");

            //TODO: uncomment for music
            //TryPlay(backgroundMusic);

            font = Content.Load<SpriteFont>("myFont");
            

            graphics.PreferredBackBufferHeight = ScreenHeight;
            graphics.PreferredBackBufferWidth = ScreenWidth;
            graphics.ApplyChanges();

            Vector2 initialPlayerPosition = new Vector2(ScreenWidth / 2, 150);
            
            crosshair = new Crosshair(crosshairTexture, new Vector2(ScreenWidth / 2, ScreenHeight / 2), this);
            assualtRifle = new Gun(defaultGunTexture, initialPlayerPosition, this, 25, 30, 5.0f, 100, 100.0, 1000.0);
            shotGun = new Gun(shotGunTexture, initialPlayerPosition, this, 50, 15, 5.0f, 20, 1000.0, 1500.0);
            handGun = new Gun(handGunTexture, initialPlayerPosition, this, 40, 10, 5.0f, 200, 500.0, 800.0);
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
                    int Y_CAMERA_THRESHOLD = ScreenHeight / 2;
                    //X movement
                    if (ConvertUnits.ToDisplayUnits(player.Position.X) > (-1 * (cam.Position.X)) + ScreenWidth - X_CAMERA_THRESHOLD)
                        cam.Move(new Vector2(-1 * (ConvertUnits.ToDisplayUnits(player.Position.X) - (-1 * (cam.Position.X) + ScreenWidth - X_CAMERA_THRESHOLD)), 0));

                    else if (ConvertUnits.ToDisplayUnits(player.Position.X) < (-1 * (cam.Position.X)) + X_CAMERA_THRESHOLD)
                        cam.Move(new Vector2(-1 * (ConvertUnits.ToDisplayUnits(player.Position.X) - (-1 * (cam.Position.X) + X_CAMERA_THRESHOLD)), 0));

                    //Y movement
                    float playerY = ConvertUnits.ToDisplayUnits(player.Position.Y);
                    //if (playerY > cam.Position.Y + 400 && cam.Position.Y > 0)
                     //   cam.Move(new Vector2(0, -1 * (ConvertUnits.ToDisplayUnits(player.Position.Y) - (-1 * (cam.Position.Y) + 400))));

                    if (playerY < cam.Position.Y + Y_CAMERA_THRESHOLD)
                        cam.Move(new Vector2(0, -1 * (ConvertUnits.ToDisplayUnits(player.Position.Y) - (-1 * (cam.Position.Y) + Y_CAMERA_THRESHOLD))));
           
                    CameraOffset = cam.Position;
                    #endregion

                    #region Keyboard inputs
                    //TODO: fix ground colloision detection
                    if (Keyboard.GetState().IsKeyDown(Keys.Space) && player.LinearVelocity.Y == 0)
                        player.ApplyForce(new Vector2(0, -500.0f));

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
                    break;
                case GameState.OPTIONS_MENU:
                    throw new NotImplementedException();
                    break;

            }

            base.Update(gameTime);
        }

        private void respawnPlayer()
        {
            player.Position = singlePlayerLevels[currentLevel].Spawn.location;
            player.Reset();
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
            var viewMaxtrix = cam.GetTransform();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied,
                                    null, null, null, null, viewMaxtrix * Matrix.CreateScale(GetScreenScale()));
            switch (State)
            {
                case GameState.LEVEL:
                    foreach (IGameObject go in gameObjects)
                        go.Draw(spriteBatch);

                    winPoint.Draw(spriteBatch);
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
            player.CreateFixture(new PolygonShape(PolygonTools.CreateRectangle(ConvertUnits.ToSimUnits(playerSpriteSheet.Width / 2), ConvertUnits.ToSimUnits(playerSpriteSheet.Height / 2)), 1.0f));
            player.BodyType = BodyType.Dynamic;
            player.FixedRotation = true;
            player.Restitution = 0.0f;
            player.Friction = 1.0f;
            player.Position = ConvertUnits.ToSimUnits(100, 150);
            player.GiveGun(handGun);
            player.GiveGun(shotGun);
            player.GiveGun(assualtRifle);
        }

        private void CreateWinPoint()
        {
            winPoint = new WinPoint(winPointTexture, physicsWorld);
            winPoint.CreateFixture(new PolygonShape(PolygonTools.CreateRectangle(ConvertUnits.ToSimUnits(winPointTexture.Width / 2), ConvertUnits.ToSimUnits(winPointTexture.Height / 2)), 1.0f));
            winPoint.BodyType = BodyType.Kinematic;
            winPoint.FixedRotation = true;
            winPoint.Restitution = 0.0f;
            winPoint.Friction = 1.0f;
            winPoint.OnCollision += new OnCollisionEventHandler(WinPoint_OnCollision);
            winPoint.Position = ConvertUnits.ToSimUnits(1500, 300);
        }

        private void CreateFloor()
        {
            for (int i = 0; i < 5; i++)
            {
                Floor f = new Floor(floorTexture, this, physicsWorld);
                f.CreateFixture(new PolygonShape(PolygonTools.CreateRectangle(ConvertUnits.ToSimUnits(floorTexture.Width / 2), ConvertUnits.ToSimUnits(floorTexture.Height / 2)), 1.0f));
                f.BodyType = BodyType.Static;
                f.Restitution = 0.0f;
                f.Friction = 0.6f;
                f.Position = ConvertUnits.ToSimUnits((ScreenWidth / 2) + (i * floorTexture.Width) - 100, ScreenHeight - 10);
                gameObjects.Add(f);
            }

            Floor foo = new Floor(floorTexture, this, physicsWorld);
            foo.CreateFixture(new PolygonShape(PolygonTools.CreateRectangle(ConvertUnits.ToSimUnits(floorTexture.Width / 2), ConvertUnits.ToSimUnits(floorTexture.Height / 2)), 1.0f));
            foo.BodyType = BodyType.Static;
            foo.Restitution = 0.0f;
            foo.Friction = 0.6f;
            foo.Position = ConvertUnits.ToSimUnits(900, 100);
            gameObjects.Add(foo);
        }

        private void CreateWalls()
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Wall w = new Wall(wallTexture, this, physicsWorld);
                    w.CreateFixture(new PolygonShape(PolygonTools.CreateRectangle(ConvertUnits.ToSimUnits(wallTexture.Width / 2), ConvertUnits.ToSimUnits(wallTexture.Height / 2)), 1.0f));
                    w.BodyType = BodyType.Static;
                    w.Restitution = 0.0f;
                    w.Friction = 0.6f;
                    w.Position = ConvertUnits.ToSimUnits(i * (floorTexture.Width * 5), ScreenHeight - (j * wallTexture.Height));
                    gameObjects.Add(w);
                }
            }
        }

        internal void CreateBullet(int damage, float rotation, float muzzleVelocity, Vector2 location)
        {
            Bullet bulletBody = new Bullet(bulletTexture, this, damage, physicsWorld);
            bulletBody.CreateFixture(new PolygonShape(PolygonTools.CreateRectangle(ConvertUnits.ToSimUnits(bulletTexture.Width / 2),  ConvertUnits.ToSimUnits(bulletTexture.Height / 2)), 1.0f));
            bulletBody.BodyType = BodyType.Dynamic;
            bulletBody.Restitution = 0.3f;
            bulletBody.Friction = 1.0f;
            bulletBody.Position = location;
            bulletBody.Rotation = rotation;
            bulletBody.OnCollision += new OnCollisionEventHandler(Bullet_OnCollision);
            bulletBody.ApplyForce(new Vector2(-(float)Math.Cos(rotation) * muzzleVelocity, -(float)Math.Sin(rotation) * muzzleVelocity));
            bulletBody.IgnoreGravity = true;
            gameObjects.Add(bulletBody);
        }

        internal void CreateRocket(int damage, float rotation, float muzzleVelocity)
        {
            Rocket rocketBody = new Rocket(rocketTexture, this, damage, physicsWorld);
            rocketBody.CreateFixture(new PolygonShape(PolygonTools.CreateRectangle(ConvertUnits.ToSimUnits(rocketTexture.Width / 2), ConvertUnits.ToSimUnits(rocketTexture.Height / 2)), 1.0f));
            rocketBody.BodyType = BodyType.Dynamic;
            rocketBody.Restitution = 0.3f;
            rocketBody.Friction = 1.0f;
            rocketBody.Position = ConvertUnits.ToSimUnits(player.GunLocation());
            rocketBody.Rotation = rotation;
            rocketBody.OnCollision += new OnCollisionEventHandler(Rocket_OnCollision);
            rocketBody.ApplyForce(new Vector2(-(float)Math.Cos(rotation) * muzzleVelocity, -(float)Math.Sin(rotation) * muzzleVelocity));
            rocketBody.IgnoreGravity = true;
            gameObjects.Add(rocketBody);
        }

        internal void CreateEnemies()
        {
            Random r = new Random();
            for (int i = 0; i < 8; i++)
            {
                Robot robot = new Robot(robotTexture, this, physicsWorld);
                robot.CreateFixture(new PolygonShape(PolygonTools.CreateRectangle(ConvertUnits.ToSimUnits(robotTexture.Width / 2), ConvertUnits.ToSimUnits(robotTexture.Height / 2)), 1.0f));
                robot.BodyType = BodyType.Dynamic;
                robot.Restitution = 0.3f;
                robot.Friction = 1.0f;
                robot.Position = ConvertUnits.ToSimUnits(((i + 1) * 400) + r.NextDouble() * 200, 100 + r.NextDouble() * 200);
                robot.IgnoreGravity = true;
                gameObjects.Add(robot);
            }

            for (int i = 0; i < 8; i++)
            {
                SuicideRobot robot = new SuicideRobot(suicideRobotTexture, emissionSpriteSheet, this, physicsWorld);
                robot.CreateFixture(new PolygonShape(PolygonTools.CreateRectangle(ConvertUnits.ToSimUnits(robotTexture.Width / 2), ConvertUnits.ToSimUnits(robotTexture.Height / 2)), 1.0f));
                robot.BodyType = BodyType.Dynamic;
                robot.Restitution = 0.3f;
                robot.Friction = 1.0f;
                robot.Position = ConvertUnits.ToSimUnits(((i + 1) * 500) + r.NextDouble() * 200, 400 + r.NextDouble() * 100);
                robot.OnCollision += new OnCollisionEventHandler(SuicideRobot_OnCollision);
                robot.IgnoreGravity = true;
                gameObjects.Add(robot);
            }
        }

        

        internal void CreateLaser(int damage, float rotation, float muzzleVelocity, Vector2 location)
        {
            Bullet bulletBody = new Bullet(laserTexture, this, damage, physicsWorld);
            bulletBody.CreateFixture(new PolygonShape(PolygonTools.CreateRectangle(ConvertUnits.ToSimUnits(bulletTexture.Width / 2), ConvertUnits.ToSimUnits(bulletTexture.Height / 2)), 1.0f));
            bulletBody.BodyType = BodyType.Dynamic;
            bulletBody.Restitution = 0.3f;
            bulletBody.Friction = 1.0f;
            bulletBody.Position = location;
            bulletBody.Rotation = rotation;
            bulletBody.OnCollision += new OnCollisionEventHandler(Laser_OnCollision);
            bulletBody.ApplyForce(new Vector2(-(float)Math.Cos(rotation) * muzzleVelocity, -(float)Math.Sin(rotation) * muzzleVelocity));
            bulletBody.IgnoreGravity = true;
            gameObjects.Add(bulletBody);
        }
        #endregion

        #region event handlers
        private bool Bullet_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
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
            if (fixtureA.Body.GetType() == typeof(Bullet) && gameObjects.Contains((Bullet)fixtureA.Body) && !(fixtureB.Body is Robot))
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
            if (fixtureA.Body is Player)
            {
                //Win game
                currentLevel++;
            }

            return true;
        }

        private bool Rocket_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (fixtureB.Body is Player)
                return false;

            Vector2 bodyPos = fixtureA.Body.Position;

            Vector2 min = bodyPos - new Vector2(10, 10);
            Vector2 max = bodyPos + new Vector2(10, 10);

            AABB aabb = new AABB(ref min, ref max);

            physicsWorld.QueryAABB(fixture =>
            {
                Vector2 fv = fixture.Body.Position - bodyPos;
                fv.Normalize();
                fv *= 40;
                fixture.Body.ApplyLinearImpulse(ref fv);
                return true;
            }, ref aabb);

            physicsWorld.RemoveBody(fixtureA.Body);

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
        #endregion

        #region State transitions
        public void GoToMainMenu()
        {
            State = GameState.MAIN_MENU;
            IsMouseVisible = true;
        }

        public void StartGame()
        {
            State = GameState.LEVEL;
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
