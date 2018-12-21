using MichaelLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace FlowFree
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        #region ScreenStuff
        public static Dictionary<ScreenStates, Screen> Screens = new Dictionary<ScreenStates, Screen>();
        public static ScreenStates CurrentScreen;
        #endregion

        #region Static
        public static MouseState MouseState;
        public static MouseState OldMouseState;

        public static string Title;

        public static Texture2D Pixel;

        public static Dictionary<PieceType, Texture2D> PieceTexture;

        public static float Scale = 1f;
        #endregion


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            graphics.PreferredBackBufferWidth = 600;
            graphics.PreferredBackBufferHeight = 600;
            graphics.ApplyChanges();
            
            IsMouseVisible = true;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Pixel = new Texture2D(GraphicsDevice, 1, 1);
            Pixel.SetData(new[] { Color.White });

            PieceTexture = new Dictionary<PieceType, Texture2D>()
            {
                [PieceType.Dot] = Content.Load<Texture2D>("dot"),
                [PieceType.Line] = Content.Load<Texture2D>("flowline"),
                [PieceType.Turn] = Content.Load<Texture2D>("FlowCorner"),
                [PieceType.DotWithHalf] = Content.Load<Texture2D>("DotHalf"),
                [PieceType.SmallDot] = Content.Load<Texture2D>("smalldot")
            };

            CurrentScreen = ScreenStates.Game;

            Screens.Add(ScreenStates.Game, new GameScreen(GraphicsDevice, Content));

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
           
            MouseState = Mouse.GetState();

            // TODO: Add your update logic here

            Screens[CurrentScreen].Update(gameTime);

            OldMouseState = MouseState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            Window.Title = Title;

            spriteBatch.Begin();

            Screens[CurrentScreen].Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
