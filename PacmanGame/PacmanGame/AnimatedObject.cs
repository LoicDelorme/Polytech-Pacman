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
        private Vector2 position;
        private Vector2 size;
        private byte correspondingValue;
        private Vector2 spawnPoint;
        private byte oldValue;

        public AnimatedObject(Texture2D texture, Vector2 position, Vector2 size, byte correspondingValue, Vector2 spawnPoint)
        {
            this.texture = texture;
            this.position = position;
            this.size = size;
            this.correspondingValue = correspondingValue;
            this.spawnPoint = spawnPoint;
            this.oldValue = PacmanGame.EMPTY;
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

        public Vector2 Size
        {
            get
            {
                return size;
            }
        }

        public byte CorrespondingValue
        {
            get
            {
                return correspondingValue;
            }
        }

        public Vector2 SpawnPoint
        {
            get
            {
                return spawnPoint;
            }
        }

        public byte OldValue
        {
            get
            {
                return oldValue;
            }

            set
            {
                oldValue = value;
            }
        }
    }
}
