using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PacmanGame
{
    class PinkGhost : Ghost
    {
        public static string DEFAULT_TEXTURE = @"resources\images\ghosts\pink_ghost";
        public static Vector2 DEFAULT_POSITION = new Vector2(14, 12);
        public static Vector2 DEFAULT_SPAWN_POINT = new Vector2(14, 12);

        public PinkGhost(ContentManager contentManager) : base(contentManager, DEFAULT_TEXTURE, DEFAULT_POSITION, DEFAULT_SPAWN_POINT)
        {
        }
    }
}
