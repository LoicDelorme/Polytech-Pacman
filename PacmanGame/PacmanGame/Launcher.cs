using System;

namespace PacmanGame
{
#if WINDOWS || XBOX
    static class Launcher
    {
        static void Main(string[] args)
        {
            PacmanGame pacmanGame = new PacmanGame();
            pacmanGame.Run();
        }
    }
#endif
}

