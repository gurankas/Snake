using System;
using System.Collections.Generic;
using System.Numerics;
using System.Collections;
using System.Text;

namespace Snake
{
    class PlayerSnake
    {
        private static PlayerSnake snakeObject = null;

        public Vector2 Head
        {
            get
            {
                return SnakeCellCoordinates[0];
            }
        }


        public int defaultLength = 6;
        //getter
        public int getDefaultLength()
        {
            return defaultLength;
        }

        //Defines the start place in the grid of 100x36 (length x height)
        Vector2 defaultStartPos = new Vector2(50, 18);

        int currentMovementDirection = 2;
        int defaultMovementDirection = 1;   // 1 for right  
                                            // 2 for left
                                            // 3 for up
                                            // 4 for down
        public int getDefaultMovementDirection()
        {
            return defaultMovementDirection;
        }
        public void setCurrentMovementDirection(int Direction)
        {
            if (Direction == 1 || Direction == 2 || Direction == 3 || Direction == 4)
            {
                currentMovementDirection = Direction;
            }
        }
        public int getCurrentMovementDirection()
        {
            return currentMovementDirection;
        }

        //how long is the snake currently
        int currentLength = 6;
        //getter
        public int getCurrentLength()
        {
            return currentLength;
        }

        //setter
        public void setCurrentLength(int length)
        {
             currentLength = length;
        }


        //contains snakes all past Vector
        public Dictionary<int, Vector2> SnakeCellCoordinates = new Dictionary<int, Vector2>();

        private PlayerSnake()
        {
            InitCoordinates();
        }

        //Sets the grid positions for each snake cell
        private void InitCoordinates()
        {
            Vector2 unitY = new Vector2(1, 0);
            for (int i = 0; i < currentLength; i++)                  
            {
                //
                if (i == 0)
                {
                    SnakeCellCoordinates.Add(i, defaultStartPos); 
                }
                else
                {
                    Vector2 nextIndex;
                    SnakeCellCoordinates.TryGetValue(i - 1, out nextIndex);
                    SnakeCellCoordinates.Add(i, Vector2.Add(nextIndex, unitY));
                }
            }
        }

        internal void UpdateSnakeCoordinates(bool bWasFoodEaten)      //according to the movement
        {
            int i = SnakeCellCoordinates.Count - 1;
            while(i >= 0)
            {
                if (i == SnakeCellCoordinates.Count - 1)                    //handling of adding length of snake when food is eaten
                {
                    if (bWasFoodEaten)
                    {
                        System.Diagnostics.Debug.WriteLine("food was eaten");
                        SnakeCellCoordinates.Add(currentLength - 1, SnakeCellCoordinates[SnakeCellCoordinates.Count - 1]);      //basically add another coordinate with the last cell'siinformation
                    }
                }

                if (i == 0)      //calculate the next head position here
                {               //head's index is 0
                    switch(currentMovementDirection)
                    {
                        case 1: //for right
                            {
                                SnakeCellCoordinates[0] += new Vector2(1, 0);
                                break;
                            }
                        case 2: //for left
                            {
                                SnakeCellCoordinates[0] += new Vector2(-1, 0);
                                break;
                            }
                        case 3: //for up
                            {
                                SnakeCellCoordinates[0] += new Vector2(0, -1);
                                break;
                            }
                        case 4: //for down
                            {
                                SnakeCellCoordinates[0] += new Vector2(0, 1);
                                break;
                            }
                    }
                }
                else            //previous cell data to the next one since the snake  will keep the shape it has made
                {
                    SnakeCellCoordinates[i] = SnakeCellCoordinates[i - 1];
                }
                i--;
            }
        }

        static public PlayerSnake InitSnake()               //Creating a singleton so that only one snake exists in the world
        {
            if(snakeObject != null)
            {
                return snakeObject;
            }
            else
            {
                snakeObject = new PlayerSnake();
                return snakeObject;
            }
        }
        
    }
}
