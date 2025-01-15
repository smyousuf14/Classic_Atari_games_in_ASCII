using System;
using System.Collections.Generic;
using System.Threading;

namespace AsciiAsteroids
{
    class Program
    {
        // ------------------------------------------------------
        // 1) Screen & Timing
        // ------------------------------------------------------
        private static int ScreenWidth = 80;
        private static int ScreenHeight = 24;
        private static int FrameTime = 100;  // ms per frame

        // ------------------------------------------------------
        // 2) Ship Properties
        // ------------------------------------------------------
        private static double shipX = ScreenWidth / 2.0;
        private static double shipY = ScreenHeight / 2.0;
        private static double shipAngle = 0.0;  // in degrees
        private static double shipSpeed = 0.0;
        private static double maxShipSpeed = 1.5;

        // ------------------------------------------------------
        // 3) Bullet Properties
        // ------------------------------------------------------
        private static List<Bullet> bullets = new List<Bullet>();
        private static double bulletSpeed = 2.0;
        private static int bulletLifetime = 40; // frames

        // ------------------------------------------------------
        // 4) Asteroid Properties
        // ------------------------------------------------------
        private static Random rand = new Random();
        private static List<Asteroid> asteroids = new List<Asteroid>();
        private static int initialAsteroidCount = 5;
        private static double maxAsteroidSpeed = 0.5;
        private static int asteroidSpawnRadius = 5;

        // ------------------------------------------------------
        // 5) Multi-line ASCII Ship Sprites
        //    Index 0: Up, 1: Right, 2: Down, 3: Left
        // ------------------------------------------------------
        private static readonly string[][] ShipSprites = new string[][]
        {
            // Facing Up
            new string[]
            {
                "  ^  ",
                " /#\\ ",
                "  |  "
            },
            // Facing Right
            new string[]
            {
                "   > ",
                "===#=",
                "   > "
            },
            // Facing Down
            new string[]
            {
                "  |  ",
                " \\#/ ",
                "  v  "
            },
            // Facing Left
            new string[]
            {
                " <   ",
                "=#===",
                " <   "
            }
        };

        // ------------------------------------------------------
        // 6) Game Running Flag
        // ------------------------------------------------------
        private static bool gameRunning = true;

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.Clear();

            // Create initial asteroids
            for (int i = 0; i < initialAsteroidCount; i++)
            {
                asteroids.Add(CreateRandomAsteroid());
            }

            // Main game loop
            while (gameRunning)
            {
                HandleInput();
                UpdateGame();
                DrawGame();

                Thread.Sleep(FrameTime);
            }

            // Game Over
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Game Over!");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }

        // ------------------------------------------------------
        // 7) Handle Input
        // ------------------------------------------------------
        private static void HandleInput()
        {
            while (Console.KeyAvailable)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                switch (keyInfo.Key)
                {
                    case ConsoleKey.LeftArrow:
                        // Rotate left
                        shipAngle -= 15;
                        break;
                    case ConsoleKey.RightArrow:
                        // Rotate right
                        shipAngle += 15;
                        break;
                    case ConsoleKey.UpArrow:
                        // Accelerate
                        shipSpeed += 0.3;
                        if (shipSpeed > maxShipSpeed) shipSpeed = maxShipSpeed;
                        break;
                    case ConsoleKey.Spacebar:
                        // Fire bullet
                        FireBullet();
                        break;
                }
            }
        }

        private static void FireBullet()
        {
            double radians = shipAngle * Math.PI / 180.0;

            // Bullet starts at ship tip
            double bulletStartX = shipX + Math.Cos(radians);
            double bulletStartY = shipY + Math.Sin(radians);

            var newBullet = new Bullet()
            {
                X = bulletStartX,
                Y = bulletStartY,
                VX = bulletSpeed * Math.Cos(radians),
                VY = bulletSpeed * Math.Sin(radians),
                Life = bulletLifetime
            };
            bullets.Add(newBullet);
        }

        // ------------------------------------------------------
        // 8) Update Game Logic
        // ------------------------------------------------------
        private static void UpdateGame()
        {
            // Move ship
            double radAngle = shipAngle * Math.PI / 180.0;
            shipX += shipSpeed * Math.Cos(radAngle);
            shipY += shipSpeed * Math.Sin(radAngle);

            Wrap(ref shipX, ScreenWidth);
            Wrap(ref shipY, ScreenHeight);

            // Update bullets
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                bullets[i].X += bullets[i].VX;
                bullets[i].Y += bullets[i].VY;
                Wrap(ref bullets[i].X, ScreenWidth);
                Wrap(ref bullets[i].Y, ScreenHeight);

                bullets[i].Life--;
                if (bullets[i].Life <= 0)
                {
                    bullets.RemoveAt(i);
                }
            }

            // Update asteroids
            for (int i = 0; i < asteroids.Count; i++)
            {
                var a = asteroids[i];
                a.X += a.VX;
                a.Y += a.VY;
                Wrap(ref a.X, ScreenWidth);
                Wrap(ref a.Y, ScreenHeight);
            }

            // Check bullet-asteroid collisions
            for (int i = asteroids.Count - 1; i >= 0; i--)
            {
                var a = asteroids[i];
                bool asteroidDestroyed = false;
                for (int j = bullets.Count - 1; j >= 0; j--)
                {
                    var b = bullets[j];
                    if (Distance(a.X, a.Y, b.X, b.Y) < 1.0)
                    {
                        // Destroy asteroid
                        asteroids.RemoveAt(i);
                        // Remove bullet
                        bullets.RemoveAt(j);
                        asteroidDestroyed = true;
                        break;
                    }
                }
                if (asteroidDestroyed) break;
            }

            // Check ship-asteroid collisions
            foreach (var a in asteroids)
            {
                if (Distance(shipX, shipY, a.X, a.Y) < 1.0)
                {
                    gameRunning = false;
                    break;
                }
            }

            // Respawn asteroids if fewer than initial
            if (asteroids.Count < initialAsteroidCount)
            {
                asteroids.Add(CreateRandomAsteroid());
            }
        }

        // ------------------------------------------------------
        // 9) Draw the Game to Console
        // ------------------------------------------------------
        private static void DrawGame()
        {
            // Prepare a char buffer
            Console.SetCursorPosition(0, 0);
            char[,] buffer = new char[ScreenHeight, ScreenWidth];
            for (int y = 0; y < ScreenHeight; y++)
            {
                for (int x = 0; x < ScreenWidth; x++)
                {
                    buffer[y, x] = ' ';
                }
            }

            // Draw asteroids
            foreach (var a in asteroids)
            {
                int ax = (int)Math.Round(a.X) % ScreenWidth;
                int ay = (int)Math.Round(a.Y) % ScreenHeight;
                if (ax < 0) ax += ScreenWidth;
                if (ay < 0) ay += ScreenHeight;
                buffer[ay, ax] = 'O';
            }

            // Draw bullets
            foreach (var b in bullets)
            {
                int bx = (int)Math.Round(b.X) % ScreenWidth;
                int by = (int)Math.Round(b.Y) % ScreenHeight;
                if (bx < 0) bx += ScreenWidth;
                if (by < 0) by += ScreenHeight;
                buffer[by, bx] = '*';
            }

            // Draw the ship (multi-line ASCII art)
            DrawShip(buffer);

            // Render buffer to the console
            for (int y = 0; y < ScreenHeight; y++)
            {
                for (int x = 0; x < ScreenWidth; x++)
                {
                    Console.Write(buffer[y, x]);
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Draws the multi-line ship sprite into the buffer, choosing
        /// which sprite to use based on shipAngle (4 directions).
        /// </summary>
        private static void DrawShip(char[,] buffer)
        {
            int directionIndex = GetShipDirectionIndex(shipAngle);
            string[] sprite = ShipSprites[directionIndex];

            // Dimensions of the chosen sprite
            int spriteHeight = sprite.Length;
            int spriteWidth  = sprite[0].Length;

            // We'll consider the "center" of the sprite
            //   - row=1 for a 3-line sprite
            //   - col=2 for a 5-column sprite
            // (Adjust if your sprites are different sizes)
            int centerRow = 1;
            int centerCol = 2;

            int sx = (int)Math.Round(shipX);
            int sy = (int)Math.Round(shipY);

            for (int row = 0; row < spriteHeight; row++)
            {
                for (int col = 0; col < spriteWidth; col++)
                {
                    char c = sprite[row][col];
                    // Treat spaces as "transparent"
                    if (c == ' ') continue;

                    int drawX = sx - centerCol + col;
                    int drawY = sy - centerRow + row;
                    WrapCoordinate(ref drawX, ScreenWidth);
                    WrapCoordinate(ref drawY, ScreenHeight);

                    buffer[drawY, drawX] = c;
                }
            }
        }

        /// <summary>
        /// Chooses one of 4 directions (0..3) based on angle in degrees:
        /// 0 = Up, 1 = Right, 2 = Down, 3 = Left.
        /// </summary>
        private static int GetShipDirectionIndex(double angleDeg)
        {
            // Normalize angle to 0..360
            angleDeg = angleDeg % 360;
            if (angleDeg < 0) angleDeg += 360;

            // Quadrant approach:
            //   0° -> Up, 90° -> Right, 180° -> Down, 270° -> Left
            // We'll "pivot" each quadrant by +45°, then do integer division by 90.
            return (int)((angleDeg + 45) / 90) % 4;
        }

        /// <summary>
        /// Wraps a coordinate around the screen boundaries.
        /// </summary>
        private static void WrapCoordinate(ref int coord, int max)
        {
            if (coord < 0) coord += max;
            else if (coord >= max) coord -= max;
        }

        // ------------------------------------------------------
        // 10) Utility Methods
        // ------------------------------------------------------
        private static void Wrap(ref double value, int max)
        {
            if (value < 0) value += max;
            else if (value >= max) value -= max;
        }

        private static double Distance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        }

        private static Asteroid CreateRandomAsteroid()
        {
            double x = rand.NextDouble() * ScreenWidth;
            double y = rand.NextDouble() * ScreenHeight;

            // Ensure it doesn't spawn too close to the ship
            while (Distance(x, y, shipX, shipY) < asteroidSpawnRadius)
            {
                x = rand.NextDouble() * ScreenWidth;
                y = rand.NextDouble() * ScreenHeight;
            }

            double angle = rand.NextDouble() * 2 * Math.PI;
            double speed = rand.NextDouble() * maxAsteroidSpeed + 0.1;

            return new Asteroid()
            {
                X = x,
                Y = y,
                VX = speed * Math.Cos(angle),
                VY = speed * Math.Sin(angle),
            };
        }
    }

    // Simple classes for bullets and asteroids
    class Asteroid
    {
        public double X;
        public double Y;
        public double VX;
        public double VY;
    }

    class Bullet
    {
        public double X;
        public double Y;
        public double VX;
        public double VY;
        public int Life;
    }
}
