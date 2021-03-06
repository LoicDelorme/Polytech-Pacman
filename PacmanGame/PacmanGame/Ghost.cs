﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PacmanGame
{
    class Ghost : AnimatedObject
    {
        public static Vector2 DEFAULT_GHOST_SIZE = new Vector2(20, 20);
        public static Directions.Direction DEFAULT_GHOST_DIRECTION = Directions.Direction.up;

        private ContentManager contentManager;
        private string initialTexture;
        private bool eatableGhostState;

        public Ghost(ContentManager contentManager, string ghostTexture, Vector2 position, Vector2 spawnPoint) : base(contentManager.Load<Texture2D>(ghostTexture), DEFAULT_GHOST_SIZE, DEFAULT_GHOST_DIRECTION, position, spawnPoint)
        {
            this.contentManager = contentManager;
            this.initialTexture = ghostTexture;
            this.eatableGhostState = false;
        }

        public void updateTexture()
        {
            Texture = contentManager.Load<Texture2D>(@"resources\images\eatable_ghost\eatable_ghost_" + (eatableGhostState ? "1" : "0"));
            eatableGhostState = !eatableGhostState;
        }

        public void resetTexture()
        {
            Texture = contentManager.Load<Texture2D>(initialTexture);
        }
    }
}
