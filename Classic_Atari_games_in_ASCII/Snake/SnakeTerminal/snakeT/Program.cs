using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleSnake
{
    class Program
    {
        // Grid dimensions
        private static int width = 40;
        private static int height = 20;

        // Snake data structures
        private static List<(int x, int y)> snakeBody = new List<(int x, int y)>();
        private static (int x, int y) snakeDirection = (1, 0); // Initially moving to the right

        // Food location
        private static (int x, int y) food;

        // Game states
        private static bool gameOver = false;
        private static int score = 0;

        static void Main(string[] args)
        {
            // Hide cursor for a cleaner look
            Console.CursorVisible = false;

            // Initialize the game
            InitializeGame();

            // Main game loop
            while (!gameOver)
            {
                // Read user input if available
                ProcessInput();
                // Update snake position
                UpdateSnake();
                // Render the new state
                DrawFrame();
                // Control the speed
                Thread.Sleep(150); // Lower value => faster game
            }

            // End-of-game message
            Console.SetCursorPosition(0, height + 2);
            Console.WriteLine($"Game Over! Final Score: {score}");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Sets up the initial game state.
        /// </summary>
        private static void InitializeGame()
        {
            // Place the snake in the center
            int startX = width / 2;
            int startY = height / 2;
            snakeBody.Clear();
            snakeBody.Add((startX, startY));  // Initially just a single segment

            score = 0;
            GenerateFood();
        }

        /// <summary>
        /// Generates food at a random location that is not on the snake.
        /// </summary>
        private static void GenerateFood()
        {
            Random rand = new Random();
            while (true)
            {
                int x = rand.Next(1, width - 1);
                int y = rand.Next(1, height - 1);

                // Check if the random (x, y) collides with any part of the snake
                bool isOnSnake = false;
                foreach (var segment in snakeBody)
                {
                    if (segment.x == x && segment.y == y)
                    {
                        isOnSnake = true;
                        break;
                    }
                }
                if (!isOnSnake)
                {
                    food = (x, y);
                    break;
                }
            }
        }

        /// <summary>
        /// Reads user input and changes snake direction accordingly.
        /// </summary>
        private static void ProcessInput()
        {
            if (!Console.KeyAvailable) return; // No input to process

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    if (snakeDirection != (0, 1))  // Prevent 180-degree turn
                        snakeDirection = (0, -1);
                    break;

                case ConsoleKey.DownArrow:
                    if (snakeDirection != (0, -1))
                        snakeDirection = (0, 1);
                    break;

                case ConsoleKey.LeftArrow:
                    if (snakeDirection != (1, 0))
                        snakeDirection = (-1, 0);
                    break;

                case ConsoleKey.RightArrow:
                    if (snakeDirection != (-1, 0))
                        snakeDirection = (1, 0);
                    break;
            }
        }

        /// <summary>
        /// Moves the snake by one step in the current direction.
        /// Checks for collisions with walls, self, or food.
        /// </summary>
        private static void UpdateSnake()
        {
            // Calculate new head position
            var head = snakeBody[0];
            var newHead = (x: head.x + snakeDirection.x, y: head.y + snakeDirection.y);

            // Check for wall collision
            if (newHead.x <= 0 || newHead.x >= width
                || newHead.y <= 0 || newHead.y >= height)
            {
                gameOver = true;
                return;
            }

            // Check for self collision
            foreach (var segment in snakeBody)
            {
                if (segment.x == newHead.x && segment.y == newHead.y)
                {
                    gameOver = true;
                    return;
                }
            }

            // Insert new head
            snakeBody.Insert(0, newHead);

            // Check if food is eaten
            if (newHead.x == food.x && newHead.y == food.y)
            {
                score++;
                GenerateFood(); // Create new food
            }
            else
            {
                // Remove the tail
                snakeBody.RemoveAt(snakeBody.Count - 1);
            }
        }

        /// <summary>
        /// Draws the game board, snake, and food in the console.
        /// </summary>
        private static void DrawFrame()
        {
            Console.SetCursorPosition(0, 0);

            // Top boundary
            Console.WriteLine(new string('#', width));

            // Middle rows
            for (int y = 1; y < height; y++)
            {
                Console.Write("#"); // Left boundary
                for (int x = 1; x < width - 1; x++)
                {
                    if (x == food.x && y == food.y)
                    {
                        // Draw food
                        Console.Write("F");
                    }
                    else
                    {
                        // Check if snake occupies this position
                        bool isSnakeSegment = false;
                        for (int i = 0; i < snakeBody.Count; i++)
                        {
                            if (snakeBody[i].x == x && snakeBody[i].y == y)
                            {
                                // Draw head or body
                                Console.Write(i == 0 ? "O" : "o");
                                isSnakeSegment = true;
                                break;
                            }
                        }
                        if (!isSnakeSegment)
                        {
                            Console.Write(" ");
                        }
                    }
                }
                Console.WriteLine("#"); // Right boundary
            }

            // Bottom boundary
            Console.WriteLine(new string('#', width));

            // Display score
            Console.WriteLine($"Score: {score}");
        }
    }
}
