using MichaelLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowFree
{
    public class GameScreen : Screen
    {
        Board board;

        public GameScreen(GraphicsDevice graphics, ContentManager content)
            : base(graphics, content)
        {
            board = new Board(5, 5, 60, graphics.Viewport.Bounds, content);
        }

        public override void Update(GameTime gameTime)
        {
            board.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            board.Draw(spriteBatch);
            base.Draw(spriteBatch);
        }
    }
}
