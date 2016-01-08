using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PacmanGame
{
    class AnimatedObject
    {
        private Texture2D texture;
        private Vector2 size;
        private Directions.Direction direction;
        private Vector2 position;
        private Vector2 spawnPoint;

        public AnimatedObject(Texture2D texture, Vector2 size, Directions.Direction direction, Vector2 position, Vector2 spawnPoint)
        {
            this.texture = texture;
            this.size = size;
            this.direction = direction;
            this.position = position;
            this.spawnPoint = spawnPoint;
        }

        public Texture2D Texture
        {
            get
            {
                return texture;
            }

            set
            {
                texture = value;
            }
        }

        public Vector2 Size
        {
            get
            {
                return size;
            }
        }

        public Directions.Direction Direction
        {
            get
            {
                return direction;
            }

            set
            {
                direction = value;
            }
        }

        public Vector2 Position
        {
            get
            {
                return position;
            }

            set
            {
                position = value;
            }
        }

        public Vector2 SpawnPoint
        {
            get
            {
                return spawnPoint;
            }
        }
    }
}
