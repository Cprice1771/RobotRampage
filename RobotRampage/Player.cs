using FarseerPhysics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobotRampage
{
    public class Player : Body, IGameObject, ILivingThing
    {
        const int FRAME_COUNT = 7;
        const int FRAME_WIDTH = 60;
        const int FRAME_SEPERATION = 0;
        const int FRAME_HEIGHT = 105;
        const int FRAME_HEIGHT_SPERATION = 0;

        int frameCounter;
        float timeCounter;

        Texture2D texture;
        Rectangle srcRect;
        MainGame parent;
        PlayerDirection direction;

        public PlayerState State { get; set; }
        public List<Gun> Inventory { get; private set; }
        public bool MarkedForRemoval { get; set; }
        public GunSelected EquipedWeaponSlot { get; set; }
        public int Health { get; private set; }
        public Gun EquipedWeapon { get; set; }
        public float Width { get { return FRAME_WIDTH; } }

        public float Height { get { return FRAME_HEIGHT; } }
        
        
        const float WALK_SPEED = 0.8f;
        const float RUN_SPEED = 4.0f;

        public Player(Texture2D t, MainGame game, World w)
            : base(w)
        {
            texture = t;
            parent = game;

            State = PlayerState.IDLE;
            direction = PlayerDirection.RIGHT;
            frameCounter = 0;
            timeCounter = 0.0f;
            srcRect = new Rectangle(0, 0, FRAME_WIDTH, FRAME_HEIGHT);
            Health = 100;
            Inventory = new List<Gun>();
            EquipedWeaponSlot = GunSelected.PRIMARY;
            this.CreateFixture(new PolygonShape(PolygonTools.CreateRectangle(ConvertUnits.ToSimUnits(FRAME_WIDTH / 2), ConvertUnits.ToSimUnits(FRAME_HEIGHT / 2)), 1.0f));
            this.BodyType = BodyType.Dynamic;
            this.FixedRotation = true;
            this.Restitution = 0.0f;
            this.Friction = 1.0f;
            this.MarkedForRemoval = true;
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            bool shiftDown = Keyboard.GetState().IsKeyDown(Keys.LeftShift);
            bool aDown = Keyboard.GetState().IsKeyDown(Keys.A);
            bool dDown = Keyboard.GetState().IsKeyDown(Keys.D);
            bool spaceDown = Keyboard.GetState().IsKeyDown(Keys.Space);

            if ((Mouse.GetState().X - parent.CameraOffset.X) > ConvertUnits.ToDisplayUnits(Position.X))
            {
                direction = PlayerDirection.RIGHT;
                EquipedWeapon.Direction = PlayerDirection.RIGHT;
            }
            else
            {
                direction = PlayerDirection.LEFT;
                EquipedWeapon.Direction = PlayerDirection.LEFT;
            }

            EquipedWeapon.Update(gameTime);

            timeCounter += gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            if (timeCounter > .04167f)
            {
                if(State != PlayerState.JUMPING || frameCounter < 3)
                    frameCounter++;

                timeCounter -= .04167f;
            }

            if (frameCounter >= FRAME_COUNT)
                frameCounter = 0;
             
            
        }

        public void Draw(SpriteBatch sb)
        {
            Vector2 offset = new Vector2(ConvertUnits.ToDisplayUnits(Position.X) - FRAME_WIDTH / 2, ConvertUnits.ToDisplayUnits(Position.Y) - FRAME_HEIGHT / 2);

            srcRect = new Rectangle(frameCounter * (FRAME_WIDTH + FRAME_SEPERATION), (int)State * (FRAME_HEIGHT_SPERATION + FRAME_HEIGHT), FRAME_WIDTH, FRAME_HEIGHT);

            if (direction == PlayerDirection.LEFT)
                sb.Draw(this.texture, offset, srcRect, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.FlipHorizontally, 1.0f);
            else
            {
                Vector2 pos = ConvertUnits.ToDisplayUnits(Position);
                sb.Draw(this.texture, offset, srcRect, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 1.0f);
            }

            if (EquipedWeapon != null)
            {
                Vector2 weaponOffset = new Vector2((ConvertUnits.ToDisplayUnits(Position.X)), ConvertUnits.ToDisplayUnits(Position.Y));
                EquipedWeapon.position = weaponOffset;
                EquipedWeapon.Draw(sb);
            }
        }

        public void GiveGun(Gun weap)
        {
            EquipedWeapon = weap;
            EquipedWeaponSlot = (GunSelected)Inventory.Count;
            Inventory.Add(weap);
            
        }

        public GunSelected CycleNextWeapon()
        {
            if ((int)EquipedWeaponSlot == Inventory.Count - 1)
                EquipedWeaponSlot = GunSelected.PRIMARY;
            else
                EquipedWeaponSlot++;

            EquipedWeapon = Inventory[(int)EquipedWeaponSlot];

            return EquipedWeaponSlot;

        }

        public GunSelected CyclePreviousWeapon()
        {
            if ((int)EquipedWeaponSlot == 0)
                EquipedWeaponSlot = (GunSelected)Inventory.Count - 1;
            else
                EquipedWeaponSlot--;

            EquipedWeapon = Inventory[(int)EquipedWeaponSlot];

            return EquipedWeaponSlot;

        }

        internal Vector2 GunLocation()
        {
            float x = ConvertUnits.ToDisplayUnits(Position.X) - (float)(Math.Cos(EquipedWeapon.Rotation) * (EquipedWeapon.Width / 2));
            float y = ConvertUnits.ToDisplayUnits(Position.Y) - (float)(Math.Sin(EquipedWeapon.Rotation) * (EquipedWeapon.Width / 2));
            return new Vector2(x, y);
        }


        public void DealDamage(int damage)
        {
            Health -= damage;
        }

        internal void Reset()
        {
            Health = 100;
            foreach (Gun g in Inventory)
            {
                g.LoadedAmmo = g.magazineSize;
                g.ReserveAmmo = g.magazineSize * 3;
                g.Reloading = false;
            }
        }
    }
}
