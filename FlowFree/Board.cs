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
      
        List<Flow> Flows = new List<Flow>();

        bool[,] Grid;

        public Board(int rows, int cols, int gridCellSize, Rectangle bounds, ContentManager content)
            : base(rows, cols, gridCellSize, bounds, Game1.Pixel)
        { 
            CellSize = bounds.Width / cols;
    
         
            Grid = new bool[rows, cols];
            Test();
        }

        private void Test()
        {
            Flows.Add(new Flow(Color.Red, new Point(0, 0), new Point(2, 2), CellSize, Scale));
            Grid[0, 0] = true;
            Grid[2, 2] = true;
        }


        private bool IsBoardFull()
        {
            foreach (var cell in Grid)
            {
                if (cell == null)
                {
                    return false;
                }
            }
            return true;
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var flow in Flows)
            {
                flow.Update(gameTime, Grid);
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            foreach(var flow in Flows)
            {
                flow.Draw(sb);
            }
            base.Draw(sb);
        }
    }
}
