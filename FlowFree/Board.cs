using MichaelLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowFree
{
    public class Board : GameBoard<FlowPiece>
    {
        int CellSize = 0;

        public static List<Flow> Flows = new List<Flow>();

        public static Color CurrentColorActivated;

        public static (PieceType type, Color color)[,] Grid;

        public Board(int rows, int cols, int gridCellSize, Rectangle bounds, ContentManager content)
            : base(rows, cols, gridCellSize, bounds, Game1.Pixel)
        {
            CellSize = bounds.Width / cols;


            Grid = new(PieceType, Color)[rows, cols];
            Test();
        }

        private void Test()
        {
            Flows.Add(new Flow(Color.Red, new Point(0, 0), new Point(1, 4), CellSize, Scale));
            Flows.Add(new Flow(Color.Green, new Point(2, 0), new Point(1, 3), CellSize, Scale));
            Flows.Add(new Flow(Color.Blue, new Point(2, 1), new Point(2, 4), CellSize, Scale));
            Flows.Add(new Flow(Color.Yellow, new Point(4, 0), new Point(3, 3), CellSize, Scale));
            Flows.Add(new Flow(Color.Orange, new Point(4, 1), new Point(3, 4), CellSize, Scale));
        }

        public override void Update(GameTime gameTime)
        {
            if(Game1.MouseState.LeftButton == ButtonState.Released)
            {
                CurrentColorActivated = Color.White;
            }

            bool isAllCompleted = true;
            foreach (var flow in Flows)
            {
                flow.Update(gameTime);

                isAllCompleted &= flow.IsCompleted;
            }

            Game1.Title = $"{isAllCompleted}";
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            foreach (var flow in Flows)
            {
                flow.Draw(sb);
            }
        }
    }
}
