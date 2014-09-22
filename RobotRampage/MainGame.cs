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
#endregion

namespace RobotRampage
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainGame : Game
    {
        #region public members
        public Vector2 CameraOffset;
        public int ScreenWidth = 800;
        public int ScreenHeight = 500;
        #endregion

        #region private members
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        #region Textures
        Texture2D playerSpriteSheet;
        Texture2D floorTexture;
        Texture2D crosshairTexture;
        Texture2D defaultGunTexture;
        Texture2D bulletTexture;
        Texture2D robotTexture;
        Texture2D wallTexture;
        #endregion

        Song backgroundMusic;
        Camera cam;

        World physicsWorld;

        const int X_CAMERA_THRESHOLD = 200;


        #region world objects
        List<Bullet> bullets;
        List<Robot> enemies;
        Player player;
        List<Floor> floors;
        List<Wall> walls;
        Crosshair crosshair;
        Gun defaultWeapon;
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
            bullets = new List<Bullet>();
            enemies = new List<Robot>();
            floors = new List<Floor>();
            walls = new List<Wall>();
            CreateEnemies();
            CreateFloor();
            CreateWalls();
            CreatePlayer();
            CreateWalls();
            cam = new Camera();
            MediaPlayer.Volume = 1.0f;
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

            //TryPlay(backgroundMusic);

            

            graphics.PreferredBackBufferHeight = ScreenHeight;
            graphics.PreferredBackBufferWidth = ScreenWidth;
            graphics.ApplyChanges();

            ScreenWidth = graphics.GraphicsDevice.Viewport.Width;
            ScreenHeight = graphics.GraphicsDevice.Viewport.Height;

            Vector2 initialPlayerPosition = new Vector2(ScreenWidth / 2, 150);
            
            crosshair = new Crosshair(crosshairTexture, new Vector2(ScreenWidth / 2, ScreenHeight / 2), this);
            defaultWeapon = new Gun(defaultGunTexture, initialPlayerPosition, this, 25, 30, 5.0f);
            
            
        }

        private void LoadTextures()
        {
            crosshairTexture = Content.Load<Texture2D>("crosshair");
            playerSpriteSheet = Content.Load<Texture2D>("Player");
            floorTexture = Content.Load<Texture2D>("floor");
            defaultGunTexture = Content.Load<Texture2D>("gun");
            bulletTexture = Content.Load<Texture2D>("bullet");
            robotTexture = Content.Load<Texture2D>("robot");
            wallTexture = Content.Load<Texture2D>("wall");
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

            physicsWorld.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);
            Vector2 pos = player.Position;
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                player.Position = ConvertUnits.ToSimUnits(ScreenWidth / 2, 150);
                player.LinearVelocity = new Vector2(0, 0);
            }


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

            //TODO: fix ground colloision detection
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && player.LinearVelocity.Y == 0)
                player.ApplyForce(new Vector2(0, -500.0f));
            if (Keyboard.GetState().IsKeyDown(Keys.D) && player.LinearVelocity.X < 5.0f)
                player.ApplyForce(new Vector2(20.0f, 0));
            else if (Keyboard.GetState().IsKeyDown(Keys.A) && player.LinearVelocity.X > -5.0f)
                player.ApplyForce(new Vector2(-20.0f, 0));


            foreach (Bullet b in bullets)
                b.Update(gameTime);

            foreach (Robot r in enemies)
                r.Update(gameTime);

            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                if (bullets[i].OffScreen())
                {
                    physicsWorld.RemoveBody(bullets[i]);
                    bullets.RemoveAt(i);
                }
            }

            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                if (enemies[i].Health <= 0)
                {
                    physicsWorld.RemoveBody(enemies[i]);
                    enemies.RemoveAt(i);
                }
            }

            player.Update(gameTime);
            //floor.Update(gameTime);
            crosshair.Update(gameTime);
            base.Update(gameTime);
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
            foreach (Bullet b in bullets)
                b.Draw(spriteBatch);
            player.Draw(spriteBatch);

            foreach(Floor f in floors)
                f.Draw(spriteBatch);
            

            
            foreach (Robot r in enemies)
                r.Draw(spriteBatch);
            foreach (Wall w in walls)
                w.Draw(spriteBatch);

            crosshair.Draw(spriteBatch);
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
            player.EquipWeapon(defaultWeapon);
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
                f.Position = ConvertUnits.ToSimUnits((ScreenWidth / 2) + (i * floorTexture.Width), ScreenHeight - 10);
                floors.Add(f);
            }

            Floor foo = new Floor(floorTexture, this, physicsWorld);
            foo.CreateFixture(new PolygonShape(PolygonTools.CreateRectangle(ConvertUnits.ToSimUnits(floorTexture.Width / 2), ConvertUnits.ToSimUnits(floorTexture.Height / 2)), 1.0f));
            foo.BodyType = BodyType.Static;
            foo.Restitution = 0.0f;
            foo.Friction = 0.6f;
            foo.Position = ConvertUnits.ToSimUnits(900, 100);
            floors.Add(foo);
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
                    walls.Add(w);
                }
            }
        }

        internal void CreateBullet(int damage, float rotation, float muzzleVelocity)
        {
            Bullet bulletBody = new Bullet(bulletTexture, this, damage, physicsWorld);
            bulletBody.CreateFixture(new PolygonShape(PolygonTools.CreateRectangle(ConvertUnits.ToSimUnits(bulletTexture.Width / 2),  ConvertUnits.ToSimUnits(bulletTexture.Height / 2)), 1.0f));
            bulletBody.BodyType = BodyType.Dynamic;
            bulletBody.Restitution = 0.3f;
            bulletBody.Friction = 1.0f;
            bulletBody.Position = ConvertUnits.ToSimUnits(player.GunLocation());
            bulletBody.Rotation = rotation;
            bulletBody.OnCollision += new OnCollisionEventHandler(Bullet_OnCollision);
            bulletBody.ApplyForce(new Vector2(-(float)Math.Cos(rotation) * muzzleVelocity, -(float)Math.Sin(rotation) * muzzleVelocity));
            bulletBody.IgnoreGravity = true;
            bullets.Add(bulletBody);
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
                enemies.Add(robot);
            }
        }
        #endregion

        #region event handlers
        private bool Bullet_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            if (fixtureA.Body.GetType() == typeof(Bullet))
            {
                //physicsWorld.RemoveBody(fixtureA.Body);
                bullets.Remove((Bullet)fixtureA.Body);
                if (fixtureB.Body is ILivingThing)
                {
                    ILivingThing hitObject = fixtureB.Body as ILivingThing;
                    Bullet bull = fixtureA.Body as Bullet;
                    hitObject.DealDamage(bull.Damage);
                }
            }
            else if (fixtureB.Body.GetType() == typeof(Bullet))
            {
                physicsWorld.RemoveBody(fixtureB.Body);
                bullets.Remove((Bullet)fixtureB.Body);

                if (fixtureA.Body is ILivingThing)
                {
                    ILivingThing hitObject = fixtureA.Body as ILivingThing;
                    Bullet bull = fixtureB.Body as Bullet;
                    hitObject.DealDamage(bull.Damage);
                }
            }
            return true;
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
            catch (Exception e)
            {
                MediaPlayer.Play(backgroundMusic);
            }
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
    }
}
