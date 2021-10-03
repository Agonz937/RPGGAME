using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

using Comora;

namespace RPGGAME
{
    enum Dir
    {
        Down,
        Up,
        Left,
        Right
    }

    public static class MySound
    {
        public static SoundEffect projectileSound;
        public static Song bgMusic;
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Character Sprite
        Texture2D playerSprite;
        Texture2D walkDown;
        Texture2D walkUp;
        Texture2D walkLeft;
        Texture2D walkRight;

        // Background Sprite
        Texture2D background;
        Texture2D ball;
        Texture2D skull;

        // Intantiate player
        Player player = new Player();

        // Initialize Comora
        Camera camera;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Windown Size
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();

            // Initialize Camera
            camera = new Camera(_graphics.GraphicsDevice);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

          
            // player content
            playerSprite = Content.Load<Texture2D>("Player/player");
            walkDown = Content.Load<Texture2D>("Player/walkDown");
            walkUp = Content.Load<Texture2D>("Player/walkUp");
            walkLeft = Content.Load<Texture2D>("Player/walkLeft");
            walkRight = Content.Load<Texture2D>("Player/walkRight");

            // background, skull, and ball content
            background = Content.Load<Texture2D>("background");
            skull = Content.Load<Texture2D>("skull");
            ball = Content.Load<Texture2D>("ball");

            // Players animations
            player.animations[0] = new SpriteAnimation(walkDown, 4, 8);
            player.animations[1] = new SpriteAnimation(walkUp, 4, 8);
            player.animations[2] = new SpriteAnimation(walkLeft, 4, 8);
            player.animations[3] = new SpriteAnimation(walkRight, 4, 8);

            // Sounds content
            MySound.projectileSound = Content.Load<SoundEffect>("Sounds/blip");
            MySound.bgMusic = Content.Load<Song>("Sounds/nature");
            MediaPlayer.Play(MySound.bgMusic);

            player.anim = player.animations[0];


        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            // Update player
            player.Update(gameTime);

            // Controller update
            if (!player.dead) 
                Controller.Update(gameTime, skull);

            // Update Camera
            camera.Position = player.Position;
            camera.Update(gameTime);

            // Update Enemies
            foreach (Enemy e in Enemy.enemies)
            {
                e.Update(gameTime, player.Position, player.dead);
                int sum = 32 + e.radius;

                if(Vector2.Distance(player.Position, e.Position) < sum)
                {
                    player.dead = true;
                }
            }

            // Update Projectile
            foreach (Projectile proj in Projectile.projectiles)
            {
                proj.Update(gameTime);
            }

            // Checks if projectile collided with enemy
            foreach (Projectile proj in Projectile.projectiles)
            {
                foreach (Enemy enemy in Enemy.enemies)
                {
                    int sum = proj.radius + enemy.radius;
                    if (Vector2.Distance(proj.Position, enemy.Position) < sum)
                    {
                        proj.Collided = true;
                        enemy.Dead = true;
                    }
                }
            }

            // Removes projectile and Enemies if collided
            Projectile.projectiles.RemoveAll(p => p.Collided);
            Enemy.enemies.RemoveAll(e => e.Dead);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Draw images
            _spriteBatch.Begin(camera);

            // Draw background
            _spriteBatch.Draw(background, new Vector2(-500, -500), Color.White);


            // Draw Enemies
            foreach (Enemy e in Enemy.enemies)
            {
                e.anim.Draw(_spriteBatch);
            }

            // Draw Projectile
            foreach (Projectile proj in Projectile.projectiles)
            {
                _spriteBatch.Draw(ball, new Vector2(proj.Position.X - 48, proj.Position.Y - 48), Color.White);
            }

            if (!player.dead)
            {
                // Draw Player
                player.anim.Draw(_spriteBatch);
            }
            
            
            // End Draw
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
