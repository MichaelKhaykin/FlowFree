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

        Color currColor;
        Vector2 CurrStartPos;
        bool adding = false;

        Direction CurrDirection;
        Direction PrevDirection;
        Vector2 PrevPos;

        float globalRot = 0f;
    
        public Board(int rows, int cols, int gridCellSize, Rectangle bounds, ContentManager content)
            : base(rows, cols, gridCellSize, bounds, Game1.Pixel)
        {
            CellSize = bounds.Width / cols;
    
            Test();
        }

        private void Test()
        {
            Grid[0, 0] = new FlowPiece(Color.Red, PieceType.Dot, 0);
            Grid[2, 2] = new FlowPiece(Color.Red, PieceType.Dot, 0);
            Grid[1, 1] = new FlowPiece(Color.Blue, PieceType.Dot, 0);
            Grid[4, 4] = new FlowPiece(Color.Blue, PieceType.Dot, 0);
            /*
            Grid[(0, 0.5)] = new FlowPiece(Color.Red, PieceType.Line, 0);
            Grid[(0, 1.5)] = new FlowPiece(Color.Red, PieceType.Line, 0);
            Grid[(0.5, 2)] = new FlowPiece(Color.Red, PieceType.Line, 90);
            Grid[(1.5, 2)] = new FlowPiece(Color.Red, PieceType.Line, 90);
            */
        }

        private (int, int) MouseCell()
        {
            return (Game1.MouseState.Position.X / CellSize, Game1.MouseState.Position.Y / CellSize);
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
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    var cell = Grid[row, col];
                    if (cell == null || cell.PieceType != PieceType.Dot) continue;

                    var hitBox = new Rectangle(col * CellSize, row * CellSize, CellSize, CellSize);
                    if (hitBox.Contains(Game1.MouseState.Position) && Game1.MouseState.LeftButton == ButtonState.Pressed && !adding)
                    {
                        CurrStartPos = new Vector2(col, row);
                        CurrDirection = Direction.None;

                        currColor = cell.Color;
                        adding = true;
                        
                        #region RemovePath and Circles
                        for (int i = 0; i < Rows; i++)
                        {
                            for (int j = 0; j < Cols; j++)
                            {
                                var currcell = Grid[i, j];
                                if (currcell == null) continue;
                                if (currcell.Color == currColor && currcell.PieceType != PieceType.Dot)
                                {
                                    Grid[i, j] = null;
                                }
                            }
                        }
                        #endregion
                    }
                }
            }

            if (!adding) return;

            adding = !(Game1.MouseState.LeftButton == ButtonState.Released);

            (int currCol, int currRow) = MouseCell();

            bool isFirstLineBeingAdded = true;

            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    var cell = Grid[row, col];
                    if (cell == null) continue;
                    if (cell.PieceType != PieceType.Dot && cell.Color == currColor)
                    {
                        isFirstLineBeingAdded = false;
                    }
                }
            }

            Vector2 vectorToCheck = Vector2.One;

            if (isFirstLineBeingAdded)
            {
                vectorToCheck = CurrStartPos;
            }
            else
            {
                vectorToCheck = PrevPos;
            }

            #region FigureOutDirection
            if (currCol == vectorToCheck.X && currRow == vectorToCheck.Y)
            {
                return;
            }

            if (currCol > vectorToCheck.X)
            {
                CurrDirection = Direction.Right;
            }
            else if (currCol < vectorToCheck.X)
            {
                CurrDirection = Direction.Left;
            }
            if (currRow > vectorToCheck.Y)
            {
                CurrDirection = Direction.Down;
            }
            else if (currRow < vectorToCheck.Y)
            {
                CurrDirection = Direction.Up;
            }
            #endregion

            Game1.Title = $"Prev: ({PrevPos.X},{PrevPos.Y}) Curr: ({currCol},{currRow})";
         
            if (PrevDirection != CurrDirection && PrevDirection != Direction.None)
            {
                Point pos = PrevPos.ToPoint();

                switch (CurrDirection)
                {
                    case Direction.Up:
                        globalRot = 0f;
                        break;
                    case Direction.Down:
                        globalRot = 0f;
                        break;
                    case Direction.Left:
                        globalRot = 90f;
                        break;
                    case Direction.Right:
                        globalRot = 90f;
                        break;
                }

                 Grid[pos.Y, pos.X] = new FlowPiece(currColor, PieceType.Turn, 0f);
            }

            Grid[currRow, currCol] = new FlowPiece(currColor, PieceType.Line, globalRot);
            PrevPos = new Vector2(currCol, currRow);
            PrevDirection = CurrDirection;
        }

        public override void Draw(SpriteBatch sb)
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    var piece = Grid[row, col];

                    if (piece == null) continue;
                    
                    var texture = piece.PieceTexture[piece.PieceType];

                    Vector2 pos = Vector2.Zero;

                    switch (piece.PieceType)
                    {
                        case PieceType.Dot:
                            pos = new Vector2(col * CellSize + CellSize / 2, row * CellSize + CellSize / 2);
                            break;
                            
                        case PieceType.Line:
                            switch (piece.Rotation)
                            {
                                case 0:
                                    pos = new Vector2(col * CellSize + CellSize / 2, row * CellSize + CellSize / 2);
                                    break;

                                case 90:
                                    pos = new Vector2(col * CellSize + CellSize / 2, row * CellSize + CellSize - CellSize / 2);
                                    break;
                            }
                            break;

                        case PieceType.Turn:
                            pos = new Vector2(col * CellSize + CellSize / 2 + Game1.TurnTexture.Width / 2, row * CellSize + CellSize / 2 - Game1.TurnTexture.Height / 2);
                            break;
                    }
                    sb.Draw(texture, pos, null, piece.Color, piece.Rotation.ToRadians(), new Vector2(texture.Width / 2, texture.Height / 2), Scale.ToVector2(), SpriteEffects.None, 0f);
                }
            }

       
            base.Draw(sb);
        }
    }
}
