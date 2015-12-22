using System;

namespace PacmanGame
{
#if WINDOWS || XBOX
    static class Launcher
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            PacmanGame pacmanGame = new PacmanGame();
            pacmanGame.Run();
        }
    }
#endif
}

