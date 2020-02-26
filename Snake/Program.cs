using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Diagnostics;

namespace Snake
{
    class Program
    {
        static System.Timers.Timer myTimer;
        static ArrayList grid = new ArrayList();
        static PlayerSnake player;
        static bool isGameRunning = true, bFoodOnScreen = false;


        static int ticksRun;

        static void Main(string[] args)
        {
            //can't write a welcome message at the start because it messes up the collision of the perimeter of the game
            //Console.WriteLine("Welcome to G-Snake!");

            Console.Clear();
            Init();

            //handles movement input for moving up, down, left and right
            while (isGameRunning)
            {
                var ch = Console.ReadKey(false).Key;                                              
                switch (ch)
                {
                    case ConsoleKey.Escape:
                        Console.ReadLine();
                        myTimer.Stop();
                        myTimer.Dispose();
                        return;
                    case ConsoleKey.RightArrow:
                        if (player.getCurrentMovementDirection() != 2) //player can't do a flip
                        {
                            player.setCurrentMovementDirection(1); 
                        }
                        break;
                    case ConsoleKey.LeftArrow:
                        if (player.getCurrentMovementDirection() != 1) //player can't do a flip
                        {
                            player.setCurrentMovementDirection(2);
                        }
                        break;
                    case ConsoleKey.UpArrow:
                        if (player.getCurrentMovementDirection() != 4) //player can't do a flip
                        {
                            player.setCurrentMovementDirection(3);
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        if (player.getCurrentMovementDirection() != 3) //player can't do a flip
                        {
                            player.setCurrentMovementDirection(4);
                        }
                        break;
                }
            }

            //restart game feature
            //Console.WriteLine("Restart game? (y/n)");
            //string userChoice = Console.ReadLine();
            //if (userChoice == "y")
            //{
            //    //string[] args = null;
            //    Main(args);
            //}
            //else
            //{
            //    Environment.Exit(0);
            //}
        }

        //this is called only once at the beginning
        static void Init()
        {
            Console.SetWindowSize(100, 40);
            //Console.CursorVisible = false;
            InitGrid();
            DrawGrid();
            player = PlayerSnake.InitSnake();
            InitTimer();
        }

        static void InitGrid()                                  //the grid size and outline is fixed for now
        {
            for (int i = 0; i < 3600; i++)
            {
                //make a boundary wall
                if (i <= 99 || i >= 3500 || i % 100 == 0 || i % 100 == 99)
                {
                    Tile tile = new Tile(true);
                    grid.Add(tile);
                }
                else
                {
                    Tile tile = new Tile();
                    grid.Add(tile);
                }
            }
        }
        static void InitTimer()
        {
            //Initalizing the timer and the delegate stuff
            myTimer = new System.Timers.Timer(50);
            myTimer.Elapsed += Tick;
            myTimer.AutoReset = true;
            myTimer.Enabled = true;
        }

        //executes every 0.05 seconds
        static void Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            /*
             * THE ORDER IS VERY IMPORTANT!
             * First delete snake beacuse delete function uses coordinates 
             * then Update new coordinates according to movement 
             * then draw the updated coordinates on screen
             */

            //This condition is essential to check if the snake collided with itself
            if (!isGameRunning)
                return;

            //Deletes the whole snake of the previous tick
            DeletePreviousSnake();

            //Calculates new cordinates of snake according to the direction of movement
            player.UpdateSnakeCoordinates(CheckIfFoodIsEaten());
            
            //Check to see if the head's new position has collided with a wall or itself
            DetectCollision();

            //Updates the tile status(new coordinates for blocking) after it has successfully determined it is not colliding with anything
            UpdateSnakeBlockTiles();

            //Draws the snake again for this tick using updated coordinates
            DrawSnake();

            
            //generate food if not already present on screen
            if (!bFoodOnScreen)
            {
                GenerateFood();
                bFoodOnScreen = true;
            }

        }

        //returns true if the snake's head overlaps with a food tile
        static bool CheckIfFoodIsEaten()
        {
            //fetch snake's head tile
            Tile t = GetTile(player.Head.X, player.Head.Y);

            //if the overlapping  tile is a food tile (meaning the player ate food, generate more food
            if(t.getBIsFood())
            {
                t.setBIsFood(false);    //set the current tile to a non food tile
                bFoodOnScreen = false;  //so that food generation is automatically triggered on tick

                //increase snake size 
                player.setCurrentLength(player.getCurrentLength() + 1);                
                return true;
            }            
            return false;
        }

        //draws food on screen based on random available  non-wall non-snake occupied tiles
        static void GenerateFood()
        {
            int generatedIndex;
            do
            {
                generatedIndex = GenerateRandomNumber(0, grid.Count - 1);
            }
            while (((Tile)grid[generatedIndex]).getBIsWall());              //keep generating numbers until the corresponding grid isn't a wall (or occupied by the snake

            //move the cursor to the generated index and draw food
            Console.SetCursorPosition(generatedIndex % 100, generatedIndex / 100);
            Console.Write("o");

            //Set the tile so that it is a food tile
            Tile t = GetTile(generatedIndex % 100, generatedIndex / 100);
            t.setBIsFood(true);
        }

        static int GenerateRandomNumber(int lowerRange, int upperBound)
        {
            Random r = new Random();
            return r.Next(lowerRange, upperBound);
        }

        //Called when collision is detected
        static void EndGame ()
        {
            isGameRunning = false;
            Console.SetCursorPosition(0, 40);
            Console.WriteLine($"Game Over! Your score is {player.getCurrentLength()-6}");
        }

        private static void DetectCollision()
        {
            try
            {
                Tile t = GetTile(player.Head.X, player.Head.Y);

                //Debug.WriteLine($"{t.getBIsWall()} and player is at {player.Head.X}, {player.Head.Y}");

                if (t.getBIsWall())
                {
                    EndGame();
                }
                     //check whether the snake's head grid point is wall/snake
            }
            catch (Exception)
            {
                Console.WriteLine("Incorrect grid access");
                throw;
            }
        }

        static Tile GetTile (float x, float y)
        {
            return (Tile)grid[(int)((y * 100) + x)];
        }

        //update the grids the snake is occupying to block
        private static void UpdateSnakeBlockTiles()
        {
            //We wait with checking for collision until full snake is moving
            if (ticksRun <= 20)
            {
                ticksRun++;
                return;
            }

            for (int i = 0; i < player.SnakeCellCoordinates.Count; i++)
            {
                try
                {                                                   //the snake's all cell's
                    Tile t = GetTile(player.SnakeCellCoordinates[i].X, player.SnakeCellCoordinates[i].Y); 
                    
                    //If the spot was already occupied, another part of the snake is there (= snake collided with itself)
                    if (t.getBIsWall())
                    {
                        EndGame();
                        return;
                    }
                    t.setBIsWall(true);      //sets the tiles occupied by the snake
                }
                catch (Exception)
                {
                    Console.WriteLine("The grid isn't a tile object");
                    throw;
                }
            }
        }

        private static void DeletePreviousSnake()
        {
            int i = 0;
            while (i < player.SnakeCellCoordinates.Count)
            {
                Vector2 startPos;
                player.SnakeCellCoordinates.TryGetValue(i, out startPos);
                Console.SetCursorPosition((int)startPos.X, (int)startPos.Y);
                Console.Write(" ");

                //update the these cells to all false
                Tile t = GetTile(player.SnakeCellCoordinates[i].X, player.SnakeCellCoordinates[i].Y);
                t.setBIsWall(false);
                i++;
            }
        }

        private static void DrawSnake()
        {            
            int i = 0;
            while (i < player.SnakeCellCoordinates.Count)
            {
                Vector2 startPos;
                player.SnakeCellCoordinates.TryGetValue(i, out startPos);
                Console.SetCursorPosition((int)startPos.X, (int)startPos.Y);
                //draw head
                if(i == 0)
                {
                    Console.Write("<");
                }
                ////draw tail                                                   will have to deal with rotation of tail  if we want to draw tail
                //else if(i == player.SnakeCellCoordinates.Count - 1)       
                //{
                //    Console.Write("-");
                //}
                else
                {
                    Console.Write("█"); 
                }
                i++;
            }
        }

        private static void DrawGrid()
        {
            for(int i = 0; i < grid.Count; i++)
            {
                if(i % 100 == 0)
                {
                    if (!(i==0))
                    {
                        Console.WriteLine(); 
                    }
                }
                if(((Tile)grid[i]).getBIsWall())
                {
                    Console.Write("█");
                }
                else
                {
                    Console.Write(" ");
                }
            }
            Console.WriteLine("\n\nPress arrow keys to control the snake and Esc to exit");
        }
    }
}
/*
 * TODOs
 * detection of collision               done
 * powerup generation                   done
 * increase length of snake             done
 * handling edge test cases
 */