#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Audio;
#endregion

/*** TODO LIST ****
 * 
 * 4. Preview de Próxima peça
 * 5. Som
 */

namespace TPJ_Tetris
{
    public class Game1 : Game
    {
        enum GameStatus
        {
            gameplay,
            freeze,
            highlight,
            gameover,
        };
        GameStatus status;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SoundEffect slap;
        
        Texture2D box;
        Texture2D pixel;

        SpriteFont font;

        int score = 0;
        byte[,] board = new byte[22, 10];
        int pX = 4, pY = 0;
        float lastAutomaticMove = 0f;
        float lastHumanMove = 0f;
        bool spacePressed = false;

        Piece nextPiece, piece;
        Color[] colors = { Color.Wheat,   Color.YellowGreen,   Color.Violet,
                           Color.Navy,    Color.LavenderBlush, Color.IndianRed,
                           Color.HotPink, Color.Black};

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferHeight = 650;
            graphics.PreferredBackBufferWidth = 550;
            Content.RootDirectory = "Content";
        }
        protected override void Initialize()
        {
            piece = new Piece();
            nextPiece = new Piece();
            status = GameStatus.gameplay;
            base.Initialize();
        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            box = Content.Load<Texture2D>("box");
            pixel = Content.Load<Texture2D>("pixel");
            font = Content.Load<SpriteFont>("MyFont");
            slap = Content.Load<SoundEffect>("slap");
        }
        protected override void UnloadContent()
        {
            box.Dispose();
            pixel.Dispose();
            slap.Dispose();
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
                        piece = nextPiece;
                        nextPiece = new Piece();
                        pY = 0;
                        pX = (10 - piece.width) / 2;
                        if (canGo(pX, pY))
                            status = GameStatus.gameplay;
                        else
                            status = GameStatus.gameover;
                    }
                }
                highlightTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            base.Update(gameTime);
        }

        private void RemoveLine(int line)
        {
            score += 5;
            for (int y = line; y >= 1; y--)
                for (int x = 0; x < 10; x++)
                    board[y, x] = board[y - 1, x];
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            DrawRectangle(new Rectangle(25, 25, 302, 602), Color.DarkBlue);
            spriteBatch.DrawString(font, score + " pts",
                                new Vector2(350, 25), Color.Chartreuse);
            for (int x = 0; x < 10; x++)
                for (int y = 2; y < 22; y++)
                {
                    if (board[y , x] != 0)
                        spriteBatch.Draw(box,
                            new Vector2(x * 30 + 25, (y-2) * 30 + 25),
                            colors[board[y,x]-1] );
                    if (status == GameStatus.gameplay &&
                        y >= pY && x >= pX && 
                        y < pY + piece.height &&
                        x < pX + piece.width)
                    {
                        if (piece.GetBlock(y - pY, x - pX) != 0)
                        {

                            spriteBatch.Draw(box,
                                new Vector2(x*30 + 25, (y-2)*30 + 25),
                                colors[piece.GetBlock(y-pY,x-pX)-1]);
                        }
                    }
                }
            if (status == GameStatus.gameover)
            {
                spriteBatch.DrawString(font, "GameOver!",
                        new Vector2(10, 250), Color.Red);
            }
            DrawNextPiece();
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
                piece = nextPiece;
                nextPiece = new Piece();
                pY = 0;
                pX = (10 - piece.width) / 2;

                if (canGo(pX, pY))
                    status = GameStatus.gameplay;
                else
                    status = GameStatus.gameover;
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
            slap.Play();
            score++;
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

        // Desenha um rectângulo (linha, apenas)
        private void DrawRectangle(Rectangle r, Color c)
        {
            // linha horizontal de topo
            spriteBatch.Draw(pixel, new Rectangle(r.X, r.Y, r.Width, 1),  c);
            // linha vertical esquerda
            spriteBatch.Draw(pixel, new Rectangle(r.X, r.Y, 1, r.Height), c);
            // linha horizontal, fundo
            spriteBatch.Draw(pixel, new Rectangle(r.X, r.Y+r.Height-1, r.Width, 1), c);
            // linha vertical, direita
            spriteBatch.Draw(pixel, new Rectangle(r.X+r.Width-1, r.Y, 1, r.Height), c);
        }

        private void DrawNextPiece()
        {
            int posX = 350;
            int posY = 150;

            for (int x = 0; x < nextPiece.width; x++)
                for (int y = 0; y < nextPiece.height; y++)
                {
                    byte c = nextPiece.GetBlock(y,x);
                    if (c != 0)
                        spriteBatch.Draw(box,
                            new Vector2(posX + x*30,posY + y*30),
                            colors[c - 1]);
                         
                }
        }
    }
}
