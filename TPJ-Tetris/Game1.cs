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
                           Color.HotPink};

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
            // movimento automatico para baixo
            if (lastAutomaticMove >= 1f)
                if (canGoDown())
                {
                    pY++;
                    lastAutomaticMove = 0;
                }
                else newPiece();

            if (lastHumanMove >= 1f / 20f)
            {
                lastHumanMove = 0f;

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
                }            }


            base.Update(gameTime);
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
                    if (y >= pY && x >= pX && 
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
            pY = 0;
            piece = new Piece();
            pX = (10 - piece.width) / 2;
           
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
        }
    }
}
