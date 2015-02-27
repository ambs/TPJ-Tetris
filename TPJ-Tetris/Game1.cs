#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace TPJ_Tetris
{
    public class Game1 : Game
    {
        enum GameStatus
        {
            gameplay,
            freeze,
            highlight
        };
        GameStatus status;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D box;
        byte[,] board = new byte[22, 10];
        int pX = 4, pY = 0;
        float lastAutomaticMove = 0f;
        float lastHumanMove = 0f;
        bool spacePressed = false;
        Piece piece;
        Color[] colors = { Color.Wheat, Color.YellowGreen, Color.Violet,
                           Color.Navy, Color.LavenderBlush, Color.IndianRed,
                           Color.HotPink, Color.Black};

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 300;
            Content.RootDirectory = "Content";
        }
        protected override void Initialize()
        {
            piece = new Piece();
            status = GameStatus.gameplay;
            base.Initialize();
        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            box = Content.Load<Texture2D>("box");
        }
        protected override void UnloadContent()
        {
            box.Dispose();
        }
        protected override void Update(GameTime gameTime)
        {
            // para sair do jogo
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            // conta tempo que decorreu desde ultimo movimento automatico e manual
            lastAutomaticMove += (float) gameTime.ElapsedGameTime.TotalSeconds;
            lastHumanMove     += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (status == GameStatus.gameplay)
            {
                // movimento automatico para baixo
                if (lastAutomaticMove >= 1f)
                {
                    if (canGoDown())
                    {
                        pY++;
                        lastAutomaticMove = 0;
                    }
                    else newPiece();
                }

                if (lastHumanMove >= 1f / 20f)
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    {
                        piece.Rotate();
                        if (!canGo(pX, pY))
                            piece.Unrotate();
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Down) && canGoDown())
                        pY++;
                    if (Keyboard.GetState().IsKeyDown(Keys.Left) && canGoLeft())
                        pX--;
                    if (Keyboard.GetState().IsKeyDown(Keys.Right) && canGoRight())
                        pX++;

                    if (Keyboard.GetState().IsKeyUp(Keys.Space))
                        spacePressed = false;

                    if (spacePressed == false &&
                        Keyboard.GetState().IsKeyDown(Keys.Space))
                    {
                        while (canGoDown()) pY++;
                        newPiece();
                        spacePressed = true;
                    }
                    lastHumanMove = 0f;
                }
            }

            if (status == GameStatus.highlight)
            {
                if (highlightTime >= .2f)
                {
                    RemoveLine(completeLine);
                    DetectCompleteLine();
                    if (completeLine != 0)
                    {
                        HighlightLine(completeLine);
                        highlightTime = 0f;
                    }
                    else
                    {
                        piece = new Piece();
                        pY = 0;
                        pX = (10 - piece.width) / 2;
                        status = GameStatus.gameplay;
                    }
                }
                highlightTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            base.Update(gameTime);
        }

        private void RemoveLine(int line)
        {
            for (int y = line; y >= 1; y--)
                for (int x = 0; x < 10; x++)
                    board[y, x] = board[y - 1, x];
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            for (int x = 0; x < 10; x++)
                for (int y = 2; y < 22; y++)
                {
                    if (board[y , x] != 0)
                        spriteBatch.Draw(box,
                            new Vector2(x * 30, (y-2) * 30),
                            colors[board[y,x]-1] );
                    if (status == GameStatus.gameplay &&
                        y >= pY && x >= pX && 
                        y < pY + piece.height &&
                        x < pX + piece.width)
                    {
                        if (piece.GetBlock(y - pY, x - pX) != 0)
                        {

                            spriteBatch.Draw(box,
                                new Vector2(x*30, (y-2)*30),
                                colors[piece.GetBlock(y-pY,x-pX)-1]);
                        }
                    }
                }
            spriteBatch.End();

            base.Draw(gameTime);
        }
        private bool canGoDown()
        {
            if (pY + piece.height >= 22)
                return false;
            else
                return canGo(pX, pY+1);
        }
        
        private bool canGoLeft()
        {
            if (pX == 0) return false;
            else return canGo(pX - 1, pY);
        }
        private bool canGoRight()
        {
            if (pX + piece.width == 10) return false;
            else return canGo(pX + 1, pY);
        }
        private bool canGo(int dX, int dY)
        {
            if (dX < 0)                 return false;
            if (dX + piece.width > 10)  return false;
            if (dY + piece.height > 22) return false;

            // Vamos supor que é possível
            // e procurar um contra exemplo
            for (int x = 0; x < piece.width; x++)
            {
                for (int y = 0; y < piece.height; y++)
                {
                    if (piece.GetBlock(y, x) != 0 && board[dY + y, dX + x] != 0)
                    {
                        return false;
                    }        
                }
            }
            return true;
        }
        private void newPiece()
        {
            freeze();
            if (completeLine == 0)
            {
                status = GameStatus.gameplay;
                pY = 0;
                piece = new Piece();
                pX = (10 - piece.width) / 2;
            }
            else
            {
                status = GameStatus.highlight;
                HighlightLine(completeLine);
                highlightTime = 0f;
            }
        }

        float highlightTime;

        private void HighlightLine(int l)
        {
            for (int x = 0; x < 10; x++)
                board[l, x] = 8; // COR PARA HIGHLIGHT
        }
        private void freeze()
        {
            for (int x = 0; x < piece.width; x++)
            {
                for (int y = 0; y < piece.height; y++)
                {
                    if (piece.GetBlock(y, x) != 0)
                    {
                        board[pY+y, pX+x] = piece.GetBlock(y,x);
                    }
                }
            }
            status = GameStatus.freeze;
            DetectCompleteLine();
        }

        int completeLine;

        private void DetectCompleteLine()
        {
            completeLine = 0;
            for (int y = 21; y >= 2 && completeLine == 0; y--)
            {
                bool complete = true;
                for (int x = 0; x < 10 && complete; x++)
                {
                    if (board[y, x] == 0) complete = false;
                }
                if (complete) completeLine = y;
            }
        }
    }
}
