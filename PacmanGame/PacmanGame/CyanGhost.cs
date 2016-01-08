using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PacmanGame
{
    class CyanGhost : Ghost
    {
        public static string DEFAULT_TEXTURE = @"resources\images\ghosts\cyan_ghost";
        public static Vector2 DEFAULT_POSITION = new Vector2(14, 15);
        public static Vector2 DEFAULT_SPAWN_POINT = new Vector2(14, 15);

        public CyanGhost(ContentManager contentManager) : base(contentManager, DEFAULT_TEXTURE, DEFAULT_POSITION, DEFAULT_SPAWN_POINT)
        {
        }
    }
}
