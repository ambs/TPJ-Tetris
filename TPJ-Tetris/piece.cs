using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TPJ_Tetris
{
    class Piece
    {
        byte[][,] models = { new byte[,] { { 0, 1, 0 },
                                           { 1, 1, 1 } },
                             new byte[,] { { 2, 2, 2 },
                                           { 2, 0, 0 } },
                             new byte[,] { { 3, 3 },
                                           { 3, 3 } },
                             new byte[,] { { 4, 4, 4 },
                                           { 0, 0, 4 } },
                             new byte[,] { { 0, 5, 5 },
                                           { 5, 5, 0 } },
                             new byte[,] { { 6, 6, 0 },
                                           { 0, 6, 6 } },
                             new byte[,] { { 7, 7, 7, 7 } } };
        int selectedPiece;
        public int width { get; private set; }
        public int height { get; private set; }
        byte[,] instance;

        static Random rand = null; // ======================

        public Piece()
        {
            if (rand == null) rand = new Random(); //============
            selectedPiece = rand.Next(models.Length); //==========
            instance = models[selectedPiece];
            width  = instance.GetLength(1);
            height = instance.GetLength(0);
        }

        public byte GetBlock(int y, int x)
        {
            return instance[y, x];
        }

        public void Rotate()
        {
            byte[,] rotated = new byte[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    rotated[x, height - y - 1] = instance[y, x];
                }
            }
            instance = rotated;
            width = instance.GetLength(1);
            height = instance.GetLength(0);
        }

        public void Unrotate()
        {
            byte[,] rotated = new byte[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    rotated[x,y] = instance[y,width-1-x];
                }
            }
            instance = rotated;
            width = instance.GetLength(1);
            height = instance.GetLength(0);
        }
    
    }
}
