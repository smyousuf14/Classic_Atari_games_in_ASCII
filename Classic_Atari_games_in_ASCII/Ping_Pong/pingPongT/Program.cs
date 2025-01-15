using System;
using System.Threading;

class Program
{
    // Console size (you can adjust these)
    static int width = 60;
    static int height = 20;

    // Ball position and direction
    static int ballX;
    static int ballY;
    static int ballDX = 1;
    static int ballDY = 1;

    // Paddles
    static int paddleSize = 4;

    // Left paddle position
    static int leftPaddleY;

    // Right paddle position
    static int rightPaddleY;

    // Scores
    static int scoreLeft = 0;
    static int scoreRight = 0;

    // Game running
    static bool gameOver = false;

    static void Main()
    {
        // Hide the blinking cursor for aesthetics
        Console.CursorVisible = false;

        // Initialize positions
        Setup();

        // Main game loop
        while (!gameOver)
        {
            // Handle input
            ProcessInput();

            // Update game state
            Update();

            // Render
            Draw();

            // Control the speed
            Thread.Sleep(50);
        }

        // End of game screen
        Console.Clear();
        Console.WriteLine("Game Over!");
        Console.WriteLine($"Final Score:  Left {scoreLeft} : {scoreRight} Right");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey(true);
    }

    static void Setup()
    {
        // Set ball start position to center
        ballX = width / 2;
        ballY = height / 2;

        // Set paddles to center
        leftPaddleY = height / 2 - paddleSize / 2;
        rightPaddleY = height / 2 - paddleSize / 2;
    }

    static void ProcessInput()
    {
        // Non-blocking check if a key is pressed
        while (Console.KeyAvailable)
        {
            ConsoleKeyInfo key = Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.W:
                    if (leftPaddleY > 0)
                        leftPaddleY--;
                    break;
                case ConsoleKey.S:
                    if (leftPaddleY < height - paddleSize)
                        leftPaddleY++;
                    break;
                case ConsoleKey.UpArrow:
                    if (rightPaddleY > 0)
                        rightPaddleY--;
                    break;
                case ConsoleKey.DownArrow:
                    if (rightPaddleY < height - paddleSize)
                        rightPaddleY++;
                    break;
                case ConsoleKey.Escape:
                    gameOver = true;
                    break;
            }
        }
    }

    static void Update()
    {
        // Move the ball
        ballX += ballDX;
        ballY += ballDY;

        // Collision with top/bottom
        if (ballY <= 0 || ballY >= height - 1)
        {
            ballDY = -ballDY;
        }

        // Collision with left paddle
        if (ballX == 1)
        {
            // If the ball is in range of the left paddle
            if (ballY >= leftPaddleY && ballY < leftPaddleY + paddleSize)
            {
                ballDX = -ballDX; // bounce
            }
            else
            {
                // Score for right
                scoreRight++;
                ResetBall();
            }
        }

        // Collision with right paddle
        if (ballX == width - 2)
        {
            // If the ball is in range of the right paddle
            if (ballY >= rightPaddleY && ballY < rightPaddleY + paddleSize)
            {
                ballDX = -ballDX; // bounce
            }
            else
            {
                // Score for left
                scoreLeft++;
                ResetBall();
            }
        }

        // Check if the game is over
        if (scoreLeft >= 5 || scoreRight >= 5)
        {
            gameOver = true;
        }
    }

    static void Draw()
    {
        Console.Clear();

        // Draw top boundary
        for (int i = 0; i < width; i++)
            Console.Write("-");

        Console.WriteLine();

        // Draw the playing field
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (x == 0 || x == width - 1)
                {
                    Console.Write("|"); // Left or right boundary
                }
                else if (x == ballX && y == ballY)
                {
                    Console.Write("O"); // Ball
                }
                else if (x == 1 && y >= leftPaddleY && y < leftPaddleY + paddleSize)
                {
                    Console.Write("|"); // Left paddle
                }
                else if (x == width - 2 && y >= rightPaddleY && y < rightPaddleY + paddleSize)
                {
                    Console.Write("|"); // Right paddle
                }
                else
                {
                    Console.Write(" ");
                }
            }
            Console.WriteLine();
        }

        // Draw bottom boundary
        for (int i = 0; i < width; i++)
            Console.Write("-");
        Console.WriteLine();

        // Draw scores
        Console.WriteLine($"Left: {scoreLeft}  |  Right: {scoreRight}");
        Console.WriteLine("Press ESC to exit.");
    }

    static void ResetBall()
    {
        ballX = width / 2;
        ballY = height / 2;
        // Random direction for fun
        ballDX = new Random().Next(0, 2) == 0 ? -1 : 1;
        ballDY = new Random().Next(0, 2) == 0 ? -1 : 1;
    }
}
