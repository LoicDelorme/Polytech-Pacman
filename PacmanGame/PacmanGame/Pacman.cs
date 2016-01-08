using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PacmanGame
{
    class Pacman : AnimatedObject
    {
        public static string DEFAULT_TEXTURE = @"resources\images\pacman\pacman_left_0";
        public static Vector2 DEFAULT_PACMAN_SIZE = new Vector2(20, 20);
        public static Directions.Direction DEFAULT_DIRECTION = Directions.Direction.left;
        public static Vector2 DEFAULT_POSITION = new Vector2(17, 13);
        public static Vector2 DEFAULT_SPAWN_POINT = new Vector2(17, 13);

        private ContentManager contentManager;
        private bool textureState;
        private bool isInvincible;

        public Pacman(ContentManager contentManager) : base(contentManager.Load<Texture2D>(DEFAULT_TEXTURE), DEFAULT_PACMAN_SIZE, DEFAULT_DIRECTION, DEFAULT_POSITION, DEFAULT_SPAWN_POINT)
        {
            this.contentManager = contentManager;
            this.textureState = true;
            this.isInvincible = false;
        }

        public void updateTexture()
        {
            Texture = contentManager.Load<Texture2D>(@"resources\images\pacman\pacman_" + Direction.ToString() + "_" + (textureState ? "1" : "0"));
            textureState = !textureState;
        }

        public bool IsInvincible
        {
            get
            {
                return isInvincible;
            }

            set
            {
                isInvincible = value;
            }
        }
    }
}
