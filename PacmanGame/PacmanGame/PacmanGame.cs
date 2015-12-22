using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace PacmanGame
{
    public class PacmanGame : Microsoft.Xna.Framework.Game
    {
        public const int WALL = 0;
        public const int BEAN = 1;
        public const int BOOSTER = 2;
        public const int EMPTY = 3;
        public const int PINK_GHOST = 4;
        public const int RED_GHOST = 5;
        public const int ORANGE_GHOST = 6;
        public const int CYAN_GHOST = 7;
        public const int PACMAN = 8;

        public const int VX = 31;
        public const int VY = 28;

        public const int DEFAULT_SCORE = 0;
        public const int DEFAULT_NB_LIFE = 3;
        public const String DEFAULT_DIRECTION = "Left";
        public const int PACMAN_REFRESH_LIMIT = 200;
        public const int INVICIBLE_SOUND_DURATION = 8000;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private SpriteFont font;

        private SoundEffect deadPacmanSound;
        private SoundEffect eatBeanSound;
        private SoundEffect invicibleSound;
        private SoundEffect sirenSound;

        private AnimatedObject wall;
        private AnimatedObject bean;
        private AnimatedObject booster;

        private AnimatedObject pacman; // TODO CREATE PACKMAN IN SEVERAL DIRECTION

        private AnimatedObject cyanGhost;
        private AnimatedObject orangeGhost;
        private AnimatedObject pinkGhost;
        private AnimatedObject redGhost; // TODO CREATE GHOSTS IN SEVERAL STATES

        private byte[,] map;
        private int nbLife;
        private int score;
        private bool isInvicible;
        private int nbBeanRemaining;
        private String direction;

        private Stopwatch pacmanRefreshElapsedTime;
        private Stopwatch invicibleSoundElapsedTime;

        public PacmanGame()
        {
            map = new byte[VX, VY] {
                {WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL},
                {WALL, BOOSTER, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, WALL, WALL, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BOOSTER, WALL},
                {WALL, BEAN, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, BEAN, WALL},
                {WALL, BEAN, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, BEAN, WALL},
                {WALL, BEAN, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, BEAN, WALL},
                {WALL, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, WALL},
                {WALL, BEAN, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, BEAN, WALL},
                {WALL, BEAN, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, BEAN, WALL},
                {WALL, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, WALL, WALL, BEAN, BEAN, BEAN, BEAN, WALL, WALL, BEAN, BEAN, BEAN, BEAN, WALL, WALL, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, WALL},
                {WALL, WALL, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, WALL, WALL},
                {WALL, WALL, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, WALL, WALL},
                {WALL, WALL, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, WALL, WALL},
                {WALL, WALL, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, BEAN, WALL, WALL, WALL, EMPTY, EMPTY, WALL, WALL, WALL, BEAN, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, WALL, WALL},
                {WALL, WALL, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, BEAN, WALL, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, WALL, BEAN, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, WALL, WALL},
                {WALL, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, WALL, EMPTY, PINK_GHOST, RED_GHOST, ORANGE_GHOST, CYAN_GHOST, EMPTY, WALL, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, WALL},
                {WALL, WALL, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, BEAN, WALL, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, WALL, BEAN, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, WALL, WALL},
                {WALL, WALL, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, WALL, WALL},
                {WALL, WALL, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, BEAN, BEAN, BEAN, BEAN, PACMAN, BEAN, BEAN, BEAN, BEAN, BEAN, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, WALL, WALL},
                {WALL, WALL, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, WALL, WALL},
                {WALL, WALL, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, WALL, WALL},
                {WALL, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, WALL, WALL, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, WALL},
                {WALL, BEAN, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, BEAN, WALL},
                {WALL, BEAN, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, BEAN, WALL},
                {WALL, BEAN, BEAN, BEAN, WALL, WALL, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, WALL, WALL, BEAN, BEAN, BEAN, WALL},
                {WALL, WALL, WALL, BEAN, WALL, WALL, BEAN, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, BEAN, WALL, WALL, BEAN, WALL, WALL, WALL},
                {WALL, WALL, WALL, BEAN, WALL, WALL, BEAN, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, BEAN, WALL, WALL, BEAN, WALL, WALL, WALL},
                {WALL, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, WALL, WALL, BEAN, BEAN, BEAN, BEAN, WALL, WALL, BEAN, BEAN, BEAN, BEAN, WALL, WALL, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, WALL},
                {WALL, BEAN, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, BEAN, WALL},
                {WALL, BEAN, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, BEAN, WALL},
                {WALL, BOOSTER, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BOOSTER, WALL},
                {WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL},
            };

            nbLife = DEFAULT_NB_LIFE;
            score = DEFAULT_SCORE;
            isInvicible = false;
            nbBeanRemaining = countNumberOfInitialBeans();
            direction = DEFAULT_DIRECTION;

            pacmanRefreshElapsedTime = Stopwatch.StartNew();
            invicibleSoundElapsedTime = Stopwatch.StartNew();
            invicibleSoundElapsedTime.Reset();

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        private int countNumberOfInitialBeans()
        {
            int nbBeans = 0;
            for(int x = 0; x < VX; x++)
            {
                for (int y = 0; y < VY; y++)
                {
                    if ((map[x, y] == BEAN) || (map[x, y] == BOOSTER))
                    {
                        nbBeans++;
                    }
                }
            }

            return nbBeans;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 620;
            graphics.ApplyChanges();

            font = Content.Load<SpriteFont>(@"resources\font\defaultFont");

            deadPacmanSound = Content.Load<SoundEffect>(@"resources\sounds\dead_pacman");
            eatBeanSound = Content.Load<SoundEffect>(@"resources\sounds\eat_bean");
            invicibleSound = Content.Load<SoundEffect>(@"resources\sounds\invicible");
            sirenSound = Content.Load<SoundEffect>(@"resources\sounds\siren");

            wall = new AnimatedObject(Content.Load<Texture2D>(@"resources\images\environment\wall"), new Vector2(0, 0), new Vector2(20, 20));
            bean = new AnimatedObject(Content.Load<Texture2D>(@"resources\images\environment\bean"), new Vector2(0, 0), new Vector2(20, 20));
            booster = new AnimatedObject(Content.Load<Texture2D>(@"resources\images\environment\booster"), new Vector2(0, 0), new Vector2(20, 20));

            pacman = new AnimatedObject(Content.Load<Texture2D>(@"resources\images\pacman\pacman_left_0"), new Vector2(17, 13), new Vector2(20, 20));
            cyanGhost = new AnimatedObject(Content.Load<Texture2D>(@"resources\images\ghosts\cyan_ghost"), new Vector2(15, 14), new Vector2(20, 20));
            orangeGhost = new AnimatedObject(Content.Load<Texture2D>(@"resources\images\ghosts\orange_ghost"), new Vector2(14, 14), new Vector2(20, 20));
            redGhost = new AnimatedObject(Content.Load<Texture2D>(@"resources\images\ghosts\red_ghost"), new Vector2(13, 14), new Vector2(20, 20));
            pinkGhost = new AnimatedObject(Content.Load<Texture2D>(@"resources\images\ghosts\pink_ghost"), new Vector2(12, 14), new Vector2(20, 20));
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                Exit();
            }

            updateDirection();

            if (invicibleSoundElapsedTime.ElapsedMilliseconds > INVICIBLE_SOUND_DURATION)
            {
                isInvicible = false;
                invicibleSoundElapsedTime.Reset();
            }

            if (pacmanRefreshElapsedTime.ElapsedMilliseconds > PACMAN_REFRESH_LIMIT)
            {
                updatePacman();
                pacmanRefreshElapsedTime.Restart();
            }

            base.Update(gameTime);
        }

        private void updateDirection()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                direction = "Left";
            }

            if (keyboardState.IsKeyDown(Keys.Right))
            {
                direction = "Right";
            }

            if (keyboardState.IsKeyDown(Keys.Up))
            {
                direction = "Up";
            }

            if (keyboardState.IsKeyDown(Keys.Down))
            {
                direction = "Down";
            }
        }

        private void updatePacman()
        {
            switch (getTargetPositionValue())
            {
                case BEAN:
                    updatePacmanPosition();
                    eatBean();
                    break;
                case BOOSTER:
                    updatePacmanPosition();
                    eatBooster();
                    break;
                case EMPTY:
                    updatePacmanPosition();
                    break;
                case WALL:
                default:
                    break;
            }

            // TODO UPDATE TEXTURE
        }

        private int getTargetPositionValue()
        {
            int xPosition = (int) pacman.Position.X;
            int yPosition = (int) pacman.Position.Y;

            switch (direction)
            {
                case "Left":
                    return map[xPosition, yPosition - 1];
                case "Right":
                    return map[xPosition, yPosition + 1];
                case "Up":
                    return map[xPosition - 1, yPosition];
                case "Down":
                    return map[xPosition + 1, yPosition];
                default:
                    return WALL;
            }
        }

        private void updatePacmanPosition()
        {
            int xPosition = (int) pacman.Position.X;
            int yPosition = (int) pacman.Position.Y;
            int xToAdd = 0;
            int yToAdd = 0;

            switch (direction)
            {
                case "Left":
                    yToAdd = -1;
                    break;
                case "Right":
                    yToAdd = 1;
                    break;
                case "Up":
                    xToAdd = -1;
                    break;
                case "Down":
                    xToAdd = 1;
                    break;
                default:
                    break;
            }

            map[xPosition, yPosition] = EMPTY;
            map[xPosition + xToAdd, yPosition + yToAdd] = PACMAN;
            pacman.Position = new Vector2(xPosition + xToAdd, yPosition + yToAdd);
        }

        private void eatBean()
        {
            playSound(eatBeanSound);
            nbBeanRemaining = nbBeanRemaining - 1;
            score = score + 100;
        }

        private void eatBooster()
        {
            eatBean();
            isInvicible = true;

            invicibleSoundElapsedTime.Start();
            Thread invincibleThread = new Thread(playInvincibleSound);
            invincibleThread.Start();
        }

        private void playInvincibleSound()
        {
            while (isInvicible)
            {
                playSound(invicibleSound);
                Thread.Sleep(150);
            }

            // TODO UPDATE GHOST TEXTURE
        }

        private void playSound(SoundEffect soundToPlay)
        {
            SoundEffectInstance soundToPlayInstance = soundToPlay.CreateInstance();
            soundToPlayInstance.Volume = 1f;
            soundToPlayInstance.Play();
        }

        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            int xPosition;
            int yPosition;
            Vector2 position;
            for (int x = 0; x < VX; x++)
            {
                for (int y = 0; y < VY; y++)
                {
                    xPosition = x * 20;
                    yPosition = y * 20;
                    position = new Vector2(yPosition, xPosition);

                    switch(map[x, y])
                    {
                        case WALL:
                            spriteBatch.Draw(wall.Texture, position, Color.White);
                            break;
                        case BEAN:
                            spriteBatch.Draw(bean.Texture, position, Color.White);
                            break;
                        case BOOSTER:
                            spriteBatch.Draw(booster.Texture, position, Color.White);
                            break;
                        case EMPTY:
                            break;
                        case PINK_GHOST:
                            spriteBatch.Draw(pinkGhost.Texture, position, Color.White);
                            break;
                        case RED_GHOST:
                            spriteBatch.Draw(redGhost.Texture, position, Color.White);
                            break;
                        case ORANGE_GHOST:
                            spriteBatch.Draw(orangeGhost.Texture, position, Color.White);
                            break;
                        case CYAN_GHOST:
                            spriteBatch.Draw(cyanGhost.Texture, position, Color.White);
                            break;
                        case PACMAN:
                            spriteBatch.Draw(pacman.Texture, position, Color.White);
                            break;
                        default:
                            break;
                    }
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
