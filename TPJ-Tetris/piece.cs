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
                             new byte[,] { { 1, 1, 1 },
                                           { 1, 0, 0 } },
                             new byte[,] { { 1, 1 },
                                           { 1, 1 } },
                             new byte[,] { { 1, 1, 1 },
                                           { 0, 0, 1 } },
                             new byte[,] { { 0, 1, 1 },
                                           { 1, 1, 0 } },
                             new byte[,] { { 1, 1, 0 },
                                           { 0, 1, 1 } },
                             new byte[,] { { 1, 1, 1, 1 } } };
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
