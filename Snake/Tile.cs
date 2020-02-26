using System;
using System.Collections.Generic;
using System.Text;

namespace Snake
{
    class Tile
    {
        bool bIsWall = false;
        bool bIsFood = false;

        public Tile(bool bIsWall = false)
        {
            this.bIsWall = bIsWall;
        }

        //getters and setters
        public bool getBIsWall()
        {
            return bIsWall;
        }

        public void  setBIsWall(bool bIsOccupied)
        {
            bIsWall = bIsOccupied;
        }

        public bool getBIsFood()
        {
            return bIsFood;
        }

        public void setBIsFood(bool bIsFood)
        {
            this.bIsFood = bIsFood;
        }
    }
}
