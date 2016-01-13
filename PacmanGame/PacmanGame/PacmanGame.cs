using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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

        public static Vector2 DEFAULT_IMAGE_SIZE = new Vector2(20, 20);
        public static Vector2 DEFAULT_POSITION = new Vector2(0, 0);
        public static Vector2 DEFAULT_SPAWN_POINT = new Vector2(0, 0);

        public static byte DEFAULT_SCORE = 0;
        public static byte DEFAULT_NUMBER_OF_LIFE = 3;

        public static int PACMAN_REFRESH_RATE = 160;
        public static int GHOSTS_REFRESH_RATE = 140;
        public static int INVICIBLE_GHOSTS_THREAD_DURATION = 8000;

        public const byte VX = 31;
        public const byte VY = 28;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private SpriteFont font;

        private SoundEffect deadPacmanSound;
        private SoundEffect eatBeanSound;
        private SoundEffect invicibleSound;

        private AnimatedObject wall;
        private AnimatedObject bean;
        private AnimatedObject booster;
        private Pacman pacman;
        private Ghost cyanGhost;
        private Ghost orangeGhost;
        private Ghost pinkGhost;
        private Ghost redGhost;
        
        private byte[,] map;
        private int nbLifeRemaining;
        private int currentScore;
        private int nbBeanRemaining;

        private Random generator;

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
                {WALL, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, WALL, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, WALL, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, BEAN, WALL},
                {WALL, WALL, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, BEAN, WALL, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, WALL, BEAN, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, WALL, WALL},
                {WALL, WALL, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, WALL, WALL},
                {WALL, WALL, WALL, WALL, WALL, WALL, BEAN, WALL, WALL, BEAN, BEAN, BEAN, BEAN, EMPTY, BEAN, BEAN, BEAN, BEAN, BEAN, WALL, WALL, BEAN, WALL, WALL, WALL, WALL, WALL, WALL},
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
            nbBeanRemaining = countInitialBeans();

            generator = new Random();

            pacmanRefreshRateElapsedTime = Stopwatch.StartNew();
            ghostsRefreshRateElapsedTime = Stopwatch.StartNew();
            invicibleThreadElapsedTime = Stopwatch.StartNew();
            
            invincibleThread = null;

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        private int countInitialBeans()
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

            graphics.PreferredBackBufferWidth = WIDTH;
            graphics.PreferredBackBufferHeight = HEIGHT;
            graphics.ApplyChanges();

            font = Content.Load<SpriteFont>(@"resources\font\defaultFont");

            deadPacmanSound = Content.Load<SoundEffect>(@"resources\sounds\dead_pacman");
            eatBeanSound = Content.Load<SoundEffect>(@"resources\sounds\eat_bean");
            invicibleSound = Content.Load<SoundEffect>(@"resources\sounds\invicible");

            wall = new AnimatedObject(Content.Load<Texture2D>(@"resources\images\environment\wall"), DEFAULT_IMAGE_SIZE, Directions.Direction.up, DEFAULT_POSITION, DEFAULT_SPAWN_POINT);
            bean = new AnimatedObject(Content.Load<Texture2D>(@"resources\images\environment\bean"), DEFAULT_IMAGE_SIZE, Directions.Direction.up, DEFAULT_POSITION, DEFAULT_SPAWN_POINT);
            booster = new AnimatedObject(Content.Load<Texture2D>(@"resources\images\environment\booster"), DEFAULT_IMAGE_SIZE, Directions.Direction.up, DEFAULT_POSITION, DEFAULT_SPAWN_POINT);

            pacman = new Pacman(Content);
            cyanGhost = new CyanGhost(Content);
            orangeGhost = new OrangeGhost(Content);
            pinkGhost = new PinkGhost(Content);
            redGhost = new RedGhost(Content);
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

            if (invicibleThreadElapsedTime.ElapsedMilliseconds >= INVICIBLE_GHOSTS_THREAD_DURATION)
            {
                pacman.IsInvincible = false;
                invicibleThreadElapsedTime.Reset();
                invincibleThread = null;
            }

            if ((nbLifeRemaining > 0) && (nbBeanRemaining > 0))
            {
                updatePacmanDirection();

                if (pacman.IsInvincible && (ghostsRefreshRateElapsedTime.ElapsedMilliseconds >= GHOSTS_REFRESH_RATE))
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
            }

            base.Update(gameTime);
        }

        private void updatePacmanDirection()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                pacman.Direction = Directions.Direction.left;
            }

            if (keyboardState.IsKeyDown(Keys.Right))
            {
                pacman.Direction = Directions.Direction.right;
            }

            if (keyboardState.IsKeyDown(Keys.Up))
            {
                pacman.Direction = Directions.Direction.up;
            }

            if (keyboardState.IsKeyDown(Keys.Down))
            {
                pacman.Direction = Directions.Direction.down;
            }
        }

        private void updateGhosts()
        {
            updateGhost(cyanGhost);
            updateGhost(orangeGhost);
            updateGhost(redGhost);
            updateGhost(pinkGhost);
        }

        private void updateGhost(Ghost ghost)
        {
            ghost.Position = getRandomNewPosition(ghost);

            //if (generator.NextDouble() > 0.6)
            //{
            //    ghost.Position = getNewPosition(ghost, Dijkstra.getDirection(new Coordinate(ghost.Position), new Coordinate(pacman.Position), VX, VY, map));
            //}
            //else
            //{
            //    ghost.Position = getRandomNewPosition(ghost);
            //}

            processPossibleCollision(ghost);
        }

        private Vector2 getRandomNewPosition(AnimatedObject animatedObject)
        {
            List<Directions.Direction> directions = new List<Directions.Direction>();
            directions.Add(Directions.Direction.up);
            directions.Add(Directions.Direction.down);
            directions.Add(Directions.Direction.left);
            directions.Add(Directions.Direction.right);

            directions.Shuffle();

            foreach (Directions.Direction currentDirection in directions)
            {
                if (getTargetValue(animatedObject, currentDirection) != WALL)
                {
                    return getNewPosition(animatedObject, currentDirection);
                }
            }

            return animatedObject.SpawnPoint;
        }

        private void processPossibleCollision(Ghost ghost)
        {
            Vector3 ghostUpperLeftCorner = new Vector3(ghost.Position.Y * 20, ghost.Position.X * 20, 0);
            Vector3 ghostBottomRightCorner = new Vector3((ghost.Position.Y * 20) + ghost.Size.Y, (ghost.Position.X * 20) + ghost.Size.X, 0);

            Vector3 pacmanUpperLeftCorner = new Vector3(pacman.Position.Y * 20, pacman.Position.X * 20, 0);
            Vector3 pacmanBottomRightCorner = new Vector3((pacman.Position.Y * 20) + pacman.Size.Y, (pacman.Position.X * 20) + pacman.Size.X, 0);

            BoundingBox ghostBoundingBox = new BoundingBox(ghostUpperLeftCorner, ghostBottomRightCorner);
            BoundingBox pacmanBoundingBox = new BoundingBox(pacmanUpperLeftCorner, pacmanBottomRightCorner);

            if (ghostBoundingBox.Intersects(pacmanBoundingBox))
            {
                if (pacman.IsInvincible)
                {
                    eatGhost(ghost);
                }
                else
                {
                    eatByGhost();
                }
            }
        }

        private void eatGhost(Ghost eatenGhost)
        {
            currentScore += 1000;
            eatenGhost.Position = eatenGhost.SpawnPoint;
        }

        private void eatByGhost()
        {
            nbLifeRemaining--;
            pacman.Position = pacman.SpawnPoint;

            playSound(deadPacmanSound);
        }

        private void updatePacman()
        {
            switch (getTargetValue(pacman, pacman.Direction))
            {
                case WALL:
                    break;
                case BEAN:
                    pacman.Position = getNewPosition(pacman, pacman.Direction);
                    map[(int) pacman.Position.X, (int) pacman.Position.Y] = EMPTY;
                    eatBean();
                    break;
                case BOOSTER:
                    pacman.Position = getNewPosition(pacman, pacman.Direction);
                    map[(int) pacman.Position.X, (int) pacman.Position.Y] = EMPTY;
                    eatBooster();
                    break;
                case EMPTY:
                    pacman.Position = getNewPosition(pacman, pacman.Direction);
                    map[(int) pacman.Position.X, (int) pacman.Position.Y] = EMPTY;
                    break;
                default:
                    break;
            }

            processPossibleCollision(cyanGhost);
            processPossibleCollision(orangeGhost);
            processPossibleCollision(redGhost);
            processPossibleCollision(pinkGhost);

            pacman.updateTexture();
        }

        private byte getTargetValue(AnimatedObject animatedObject, Directions.Direction direction)
        {
            int xPosition = (int) animatedObject.Position.X;
            int yPosition = (int) animatedObject.Position.Y;

            switch (direction)
            {
                case Directions.Direction.left:
                    return map[xPosition, yPosition - 1];
                case Directions.Direction.right:
                    return map[xPosition, yPosition + 1];
                case Directions.Direction.up:
                    return map[xPosition - 1, yPosition];
                case Directions.Direction.down:
                    return map[xPosition + 1, yPosition];
                default:
                    return WALL;
            }
        }

        private Vector2 getNewPosition(AnimatedObject animatedObject, Directions.Direction direction)
        {
            int xPosition = (int) animatedObject.Position.X;
            int yPosition = (int) animatedObject.Position.Y;
            int xToAdd = 0;
            int yToAdd = 0;

            switch (direction)
            {
                case Directions.Direction.left:
                    yToAdd = -1;
                    break;
                case Directions.Direction.right:
                    yToAdd = 1;
                    break;
                case Directions.Direction.up:
                    xToAdd = -1;
                    break;
                case Directions.Direction.down:
                    xToAdd = 1;
                    break;
                default:
                    break;
            }

            return new Vector2(xPosition + xToAdd, yPosition + yToAdd);
        }

        private void eatBean()
        {
            nbBeanRemaining--;
            currentScore += 100;
            playSound(eatBeanSound);
        }

        private void eatBooster()
        {
            eatBean();
            pacman.IsInvincible = true;

            if (invincibleThread == null)
            {
                invincibleThread = new Thread(playInvincibleSound);
                invincibleThread.Start();
            }

            invicibleThreadElapsedTime.Restart();
        }

        private void playInvincibleSound()
        {
            while (pacman.IsInvincible)
            {
                playSound(invicibleSound);

                cyanGhost.updateTexture();
                orangeGhost.updateTexture();
                redGhost.updateTexture();
                pinkGhost.updateTexture();

                Thread.Sleep(GHOSTS_REFRESH_RATE);
            }

            cyanGhost.resetTexture();
            orangeGhost.resetTexture();
            redGhost.resetTexture();
            pinkGhost.resetTexture();
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

            Vector2 position;
            for (int x = 0; x < VX; x++)
            {
                for (int y = 0; y < VY; y++)
                {
                    position = new Vector2(y * 20, x * 20);

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
                    }
                }
            }

            spriteBatch.Draw(pacman.Texture, new Vector2(pacman.Position.Y * 20, pacman.Position.X * 20), Color.White);
            spriteBatch.Draw(pinkGhost.Texture, new Vector2(pinkGhost.Position.Y * 20, pinkGhost.Position.X * 20), Color.White);
            spriteBatch.Draw(redGhost.Texture, new Vector2(redGhost.Position.Y * 20, redGhost.Position.X * 20), Color.White);
            spriteBatch.Draw(orangeGhost.Texture, new Vector2(orangeGhost.Position.Y * 20, orangeGhost.Position.X * 20), Color.White);
            spriteBatch.Draw(cyanGhost.Texture, new Vector2(cyanGhost.Position.Y * 20, cyanGhost.Position.X * 20), Color.White);

            spriteBatch.DrawString(font, "Score :", new Vector2(580, 20), Color.White);
            spriteBatch.DrawString(font, currentScore.ToString(), new Vector2(580, 40), Color.White);
            spriteBatch.DrawString(font, "Vies :", new Vector2(580, 80), Color.White);
            spriteBatch.DrawString(font, nbLifeRemaining.ToString(), new Vector2(580, 100), Color.White);

            if (nbBeanRemaining == 0)
            {
                spriteBatch.DrawString(font, "Felicitations, vous avez gagne !", new Vector2(580, 140), Color.White);
            }

            if (nbLifeRemaining == 0)
            {
                spriteBatch.DrawString(font, "Dommage, c'est perdu...", new Vector2(580, 140), Color.White);
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