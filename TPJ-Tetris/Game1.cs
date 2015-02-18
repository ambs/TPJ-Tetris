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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D box;
        byte[,] board = new byte[22, 10];
        byte[,] piece = { { 0, 1, 0 }, { 1, 1, 1 } };
        int pX = 4, pY = 0;
        float lastMove = 0f;

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 300;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

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

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            lastMove += (float) gameTime.ElapsedGameTime.TotalSeconds;


            if (lastMove >= 1f && canGoDown())
            {
                pY++;
                lastMove = 0;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
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
                            Color.White);
                    if (y >= pY && x >= pX && 
                        y < pY + piece.GetLength(0) &&
                        x < pX + piece.GetLength(1))
                    {
                        if (piece[y - pY, x - pX] != 0)
                        {
                            spriteBatch.Draw(box,
                                new Vector2(x*30, (y-2)*30),
                                Color.White);
                        }
                    }
                }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private bool canGoDown()
        {
            if (pY + piece.GetLength(0) >= 22)
                return false;
            else
                return true;

        }
    }
}
