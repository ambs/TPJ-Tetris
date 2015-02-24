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

        public Piece()
        {
            selectedPiece = (new Random()).Next(models.Length);
            width  = models[selectedPiece].GetLength(1);
            height = models[selectedPiece].GetLength(0);
        }

        public byte GetBlock(int y, int x)
        {
            return models[selectedPiece][y, x];
        }
    }
}
