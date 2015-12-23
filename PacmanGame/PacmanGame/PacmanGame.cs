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
    public class PacmanGame : Game
    {
        public static int WIDTH = 850;
        public static int HEIGHT = 620;

        public const byte WALL = 0;
        public const byte BEAN = 1;
        public const byte BOOSTER = 2;
        public const byte EMPTY = 3;
        public const byte PINK_GHOST = 4;
        public const byte RED_GHOST = 5;
        public const byte ORANGE_GHOST = 6;
        public const byte CYAN_GHOST = 7;
        public const byte PACMAN = 8;

        public static byte DEFAULT_SCORE = 0;
        public static byte DEFAULT_NUMBER_OF_LIFE = 3;
        public static String DEFAULT_PACMAN_DIRECTION = "left";
        public static String DEFAULT_GHOST_DIRECTION = "up";

        public static int PACMAN_REFRESH_RATE = 160;
        public static int GHOSTS_REFRESH_RATE = 140;
        public static int INVICIBLE_GHOSTS_THREAD_DURATION = 8000;

        public static Vector2 IMAGE_SIZE = new Vector2(20, 20);
        public static Vector2 SCORE_LABEL_TEXT_POSITION = new Vector2(580, 20);
        public static Vector2 SCORE_VALUE_TEXT_POSITION = new Vector2(580, 40);
        public static Vector2 LIFE_LABEL_TEXT_POSITION = new Vector2(580, 80);
        public static Vector2 LIFE_VALUE_TEXT_POSITION = new Vector2(580, 100);
        public static Vector2 RESULT_LABEL_TEXT_POSITION = new Vector2(580, 140);

        public const byte VX = 31;
        public const byte VY = 28;

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
        private AnimatedObject pacman;
        private AnimatedObject cyanGhost;
        private AnimatedObject orangeGhost;
        private AnimatedObject pinkGhost;
        private AnimatedObject redGhost;
        
        private byte[,] map;
        private int nbLifeRemaining;
        private int currentScore;
        private bool isPacmanInvicible;
        private int nbBeanRemaining;

        private String pacmanDirection;
        private String cyanGhostDirection;
        private String orangeGhostDirection;
        private String pinkGhostDirection;
        private String redGhostDirection;

        private bool pacmanTextureState;
        private bool eatableGhostTextureState;

        private Stopwatch pacmanRefreshRateElapsedTime;
        private Stopwatch ghostsRefreshRateElapsedTime;
        private Stopwatch invicibleThreadElapsedTime;
        
        private Thread invincibleThread;

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
            
            nbLifeRemaining = DEFAULT_NUMBER_OF_LIFE;
            currentScore = DEFAULT_SCORE;
            isPacmanInvicible = false;
            nbBeanRemaining = countNumberOfInitialBeans();

            pacmanDirection = DEFAULT_PACMAN_DIRECTION;
            cyanGhostDirection = DEFAULT_GHOST_DIRECTION;
            orangeGhostDirection = DEFAULT_GHOST_DIRECTION;
            pinkGhostDirection = DEFAULT_GHOST_DIRECTION;
            redGhostDirection = DEFAULT_GHOST_DIRECTION;

            pacmanTextureState = false;
            eatableGhostTextureState = false;

            pacmanRefreshRateElapsedTime = Stopwatch.StartNew();
            ghostsRefreshRateElapsedTime = Stopwatch.StartNew();
            invicibleThreadElapsedTime = Stopwatch.StartNew();
            
            invincibleThread = null;

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
                    if (map[x, y] == BEAN)
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

            graphics.PreferredBackBufferWidth = WIDTH;
            graphics.PreferredBackBufferHeight = HEIGHT;
            graphics.ApplyChanges();

            font = Content.Load<SpriteFont>(@"resources\font\defaultFont");

            deadPacmanSound = Content.Load<SoundEffect>(@"resources\sounds\dead_pacman");
            eatBeanSound = Content.Load<SoundEffect>(@"resources\sounds\eat_bean");
            invicibleSound = Content.Load<SoundEffect>(@"resources\sounds\invicible");
            sirenSound = Content.Load<SoundEffect>(@"resources\sounds\siren");

            wall = new AnimatedObject(Content.Load<Texture2D>(@"resources\images\environment\wall"), new Vector2(0, 0), IMAGE_SIZE, WALL, new Vector2(0, 0));
            bean = new AnimatedObject(Content.Load<Texture2D>(@"resources\images\environment\bean"), new Vector2(0, 0), IMAGE_SIZE, BEAN, new Vector2(0, 0));
            booster = new AnimatedObject(Content.Load<Texture2D>(@"resources\images\environment\booster"), new Vector2(0, 0), IMAGE_SIZE, BOOSTER, new Vector2(0, 0));
            pacman = new AnimatedObject(Content.Load<Texture2D>(computePacmanTexture()), new Vector2(17, 13), IMAGE_SIZE, PACMAN, new Vector2(17, 13));
            cyanGhost = new AnimatedObject(Content.Load<Texture2D>(@"resources\images\ghosts\cyan_ghost"), new Vector2(14, 15), IMAGE_SIZE, CYAN_GHOST, new Vector2(14, 15));
            orangeGhost = new AnimatedObject(Content.Load<Texture2D>(@"resources\images\ghosts\orange_ghost"), new Vector2(14, 14), IMAGE_SIZE, ORANGE_GHOST, new Vector2(14, 14));
            redGhost = new AnimatedObject(Content.Load<Texture2D>(@"resources\images\ghosts\red_ghost"), new Vector2(14, 13), IMAGE_SIZE, RED_GHOST, new Vector2(14, 13));
            pinkGhost = new AnimatedObject(Content.Load<Texture2D>(@"resources\images\ghosts\pink_ghost"), new Vector2(14, 12), IMAGE_SIZE, PINK_GHOST, new Vector2(14, 12));
        }

        private string computePacmanTexture()
        {
            string computedTexturePath = @"resources\images\pacman\pacman_" + pacmanDirection + "_" + (pacmanTextureState ? "1" : "0");
            pacmanTextureState = !pacmanTextureState;

            return computedTexturePath;
        }

        private string computeEatableGhostTexture()
        {
            string computedTexturePath = @"resources\images\eatable_ghost\eatable_ghost_" + (eatableGhostTextureState ? "1" : "0");
            eatableGhostTextureState = !eatableGhostTextureState;

            return computedTexturePath;
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

            nbBeanRemaining = countNumberOfInitialBeans();

            if ((nbLifeRemaining > 0) && (nbBeanRemaining > 0))
            {
                updateDirection();

                if (isPacmanInvicible && (ghostsRefreshRateElapsedTime.ElapsedMilliseconds >= GHOSTS_REFRESH_RATE))
                {
                    updatePacman();
                    pacmanRefreshRateElapsedTime.Restart();
                }

                if (ghostsRefreshRateElapsedTime.ElapsedMilliseconds >= GHOSTS_REFRESH_RATE)
                {
                    updateGhosts();
                    ghostsRefreshRateElapsedTime.Restart();
                }

                if (pacmanRefreshRateElapsedTime.ElapsedMilliseconds >= PACMAN_REFRESH_RATE)
                {
                    updatePacman();
                    pacmanRefreshRateElapsedTime.Restart();
                }

                if (invicibleThreadElapsedTime.ElapsedMilliseconds >= INVICIBLE_GHOSTS_THREAD_DURATION)
                {
                    isPacmanInvicible = false;
                    invicibleThreadElapsedTime.Reset();
                    invincibleThread = null;
                }
            }

            base.Update(gameTime);
        }

        private void updateDirection()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                pacmanDirection = "left";
            }

            if (keyboardState.IsKeyDown(Keys.Right))
            {
                pacmanDirection = "right";
            }

            if (keyboardState.IsKeyDown(Keys.Up))
            {
                pacmanDirection = "up";
            }

            if (keyboardState.IsKeyDown(Keys.Down))
            {
                pacmanDirection = "down";
            }
        }

        private void updateGhosts()
        {
            updateGhost(cyanGhost, ref cyanGhostDirection);
            updateGhost(orangeGhost, ref orangeGhostDirection);
            updateGhost(redGhost, ref redGhostDirection);
            updateGhost(pinkGhost, ref pinkGhostDirection);
        }

        private void updateGhost(AnimatedObject ghost, ref string ghostDirection)
        {
            byte targetValue;
            foreach (string currentDirection in getDirections(ghostDirection))
            {
                targetValue = getTargetPositionValue(ghost, currentDirection);

                if (targetValue == PACMAN)
                {
                    if (isPacmanInvicible)
                    {
                        map[(int) ghost.Position.X, (int) ghost.Position.Y] = ghost.OldValue;
                        eatGhost(ghost);
                    }
                    else
                    {
                        map[(int) ghost.Position.X, (int) ghost.Position.Y] = ghost.OldValue;
                        eatByGhost();
                    }

                    ghost.OldValue = ((targetValue == EMPTY) || (targetValue == BEAN) || (targetValue == BOOSTER) ? targetValue : EMPTY);
                    break;
                }

                if (targetValue != WALL)
                {
                    map[(int) ghost.Position.X, (int) ghost.Position.Y] = ghost.OldValue;

                    ghost.Position = updateAnimatedObjectPosition(ghost, currentDirection);
                    ghostDirection = currentDirection;
                    ghost.OldValue = ((targetValue == EMPTY) || (targetValue == BEAN) || (targetValue == BOOSTER) ? targetValue : EMPTY);
                    break;
                }
            }
        }

        private string[] getDirections(string direction)
        {
            List<string> directions = new List<string>();
            directions.Add("left");
            directions.Add("right");
            directions.Add("up");
            directions.Add("down");
            directions.Remove(direction);
            directions.Shuffle();
            directions.Add(direction);
            directions.Reverse();

            return directions.ToArray();
        }

        private void updatePacman()
        {
            switch (getTargetPositionValue(pacman, pacmanDirection))
            {
                case WALL:
                    break;
                case CYAN_GHOST:
                    tryEatGhost(cyanGhost);
                    break;
                case ORANGE_GHOST:
                    tryEatGhost(orangeGhost);
                    break;
                case RED_GHOST:
                    tryEatGhost(redGhost);
                    break;
                case PINK_GHOST:
                    tryEatGhost(pinkGhost);
                    break;
                case BEAN:
                    map[(int) pacman.Position.X, (int) pacman.Position.Y] = EMPTY;
                    pacman.Position = updateAnimatedObjectPosition(pacman, pacmanDirection);
                    eatBean();
                    break;
                case BOOSTER:
                    map[(int) pacman.Position.X, (int) pacman.Position.Y] = EMPTY;
                    pacman.Position = updateAnimatedObjectPosition(pacman, pacmanDirection);
                    eatBooster();
                    break;
                case EMPTY:
                    map[(int) pacman.Position.X, (int) pacman.Position.Y] = EMPTY;
                    pacman.Position = updateAnimatedObjectPosition(pacman, pacmanDirection);
                    break;
                default:
                    break;
            }

            pacman.Texture = Content.Load<Texture2D>(computePacmanTexture());
        }

        private byte getTargetPositionValue(AnimatedObject animatedObject, String direction)
        {
            int xPosition = (int) animatedObject.Position.X;
            int yPosition = (int) animatedObject.Position.Y;

            switch (direction)
            {
                case "left":
                    return map[xPosition, yPosition - 1];
                case "right":
                    return map[xPosition, yPosition + 1];
                case "up":
                    return map[xPosition - 1, yPosition];
                case "down":
                    return map[xPosition + 1, yPosition];
                default:
                    return WALL;
            }
        }

        private Vector2 updateAnimatedObjectPosition(AnimatedObject animatedObject, String direction)
        {
            int xPosition = (int) animatedObject.Position.X;
            int yPosition = (int) animatedObject.Position.Y;
            int xToAdd = 0;
            int yToAdd = 0;

            switch (direction)
            {
                case "left":
                    yToAdd = -1;
                    break;
                case "right":
                    yToAdd = 1;
                    break;
                case "up":
                    xToAdd = -1;
                    break;
                case "down":
                    xToAdd = 1;
                    break;
                default:
                    break;
            }
            
            map[xPosition + xToAdd, yPosition + yToAdd] = animatedObject.CorrespondingValue;
            return new Vector2(xPosition + xToAdd, yPosition + yToAdd);
        }

        private void eatBean()
        {
            playSound(eatBeanSound);
            currentScore += 100;
        }

        private void eatBooster()
        {
            playSound(eatBeanSound);
            isPacmanInvicible = true;

            if (invincibleThread == null)
            {
                invincibleThread = new Thread(playInvincibleSound);
                invincibleThread.Start();
            }

            invicibleThreadElapsedTime.Restart();
        }

        private void tryEatGhost(AnimatedObject possibleEatenGhost)
        {
            if (isPacmanInvicible)
            {
                map[(int) pacman.Position.X, (int) pacman.Position.Y] = EMPTY;
                pacman.Position = updateAnimatedObjectPosition(pacman, pacmanDirection);
                eatGhost(possibleEatenGhost);
            }
            else
            {
                eatByGhost();
            }
        }

        private void eatGhost(AnimatedObject eatenGhost)
        {
            currentScore += 1000;
            eatenGhost.OldValue = EMPTY;
            eatenGhost.Position = eatenGhost.SpawnPoint;
            map[(int) eatenGhost.Position.X, (int) eatenGhost.Position.Y] = eatenGhost.CorrespondingValue;
        }

        private void eatByGhost()
        {
            map[(int) pacman.Position.X, (int) pacman.Position.Y] = EMPTY;

            if (--nbLifeRemaining > 0)
            {
                pacman.Position = pacman.SpawnPoint;
                map[(int) pacman.Position.X, (int) pacman.Position.Y] = pacman.CorrespondingValue;
            }

            playSound(deadPacmanSound);
        }

        private void playInvincibleSound()
        {
            while (isPacmanInvicible)
            {
                playSound(invicibleSound);

                string computedEatableGhostTexture = computeEatableGhostTexture();
                cyanGhost.Texture = Content.Load<Texture2D>(computedEatableGhostTexture);
                orangeGhost.Texture = Content.Load<Texture2D>(computedEatableGhostTexture);
                redGhost.Texture = Content.Load<Texture2D>(computedEatableGhostTexture);
                pinkGhost.Texture = Content.Load<Texture2D>(computedEatableGhostTexture);

                Thread.Sleep(GHOSTS_REFRESH_RATE);
            }

            cyanGhost.Texture = Content.Load<Texture2D>(@"resources\images\ghosts\cyan_ghost");
            orangeGhost.Texture = Content.Load<Texture2D>(@"resources\images\ghosts\orange_ghost");
            redGhost.Texture = Content.Load<Texture2D>(@"resources\images\ghosts\red_ghost");
            pinkGhost.Texture = Content.Load<Texture2D>(@"resources\images\ghosts\pink_ghost");
        }

        private void playSound(SoundEffect soundEffectToPlay)
        {
            SoundEffectInstance soundToPlayInstance = soundEffectToPlay.CreateInstance();
            soundToPlayInstance.Volume = 1f;
            soundToPlayInstance.Play();
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            if (invincibleThread != null)
            {
                invincibleThread.Abort();
            }

            base.OnExiting(sender, args);
        }

        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            spriteBatch.DrawString(font, "Score :", SCORE_LABEL_TEXT_POSITION, Color.White);
            spriteBatch.DrawString(font, currentScore.ToString(), SCORE_VALUE_TEXT_POSITION, Color.White);
            spriteBatch.DrawString(font, "Vies :", LIFE_LABEL_TEXT_POSITION, Color.White);
            spriteBatch.DrawString(font, nbLifeRemaining.ToString(), LIFE_VALUE_TEXT_POSITION, Color.White);

            if (nbBeanRemaining == 0)
            {
                spriteBatch.DrawString(font, "Felicitations, vous avez gagne !", RESULT_LABEL_TEXT_POSITION, Color.White);
            }

            if (nbLifeRemaining == 0)
            {
                spriteBatch.DrawString(font, "Dommage, c'est perdu...", RESULT_LABEL_TEXT_POSITION, Color.White);
            }

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

public static class MyExtensions
{
    private static Random random = new Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
