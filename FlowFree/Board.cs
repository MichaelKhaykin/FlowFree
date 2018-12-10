using MichaelLibrary;
using Microsoft.Xna.Framework;
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

        public Board(int rows, int cols, int gridCellSize, Rectangle bounds)
            : base(rows, cols, gridCellSize, bounds, Game1.Pixel)
        {
            CellSize = bounds.Width / cols;

            InitBoard();
            Test();
        }

        private void Test()
        {
            Grid[(0, 0)] = new FlowPiece(Color.Red, PieceType.Dot, 0);
            Grid[(2, 2)] = new FlowPiece(Color.Red, PieceType.Dot, 0);
            /*
            Grid[(0, 0.5)] = new FlowPiece(Color.Red, PieceType.Line, 0);
            Grid[(0, 1.5)] = new FlowPiece(Color.Red, PieceType.Line, 0);
            Grid[(0.5, 2)] = new FlowPiece(Color.Red, PieceType.Line, 90);
            Grid[(1.5, 2)] = new FlowPiece(Color.Red, PieceType.Line, 90);
            */
        }

        private void InitBoard()
        {
            for (int i = 0; i < Bounds.Width / CellSize; i++)
            {
                for (int j = 0; j < Bounds.Height / CellSize; j++)
                {
                    Grid[(i, j)] = new FlowPiece(Color.Transparent, PieceType.Empty, 0f);
                }
            }
        }

        private (int, int) MouseCell()
        {
            return (Game1.MouseState.Position.X / CellSize, Game1.MouseState.Position.Y / CellSize);
        }

        private bool IsBoardFull()
        {
            foreach (var cell in Grid)
            {
                if (cell.Value.PieceType == PieceType.Empty)
                {
                    return false;
                }
            }
            return true;
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var cell in Grid)
            {
                if (cell.Value.PieceType != PieceType.Dot) continue;

                var hitBox = new Rectangle((int)(cell.Key.x * CellSize), (int)(cell.Key.y * CellSize), CellSize, CellSize);
                if (hitBox.Contains(Game1.MouseState.Position) && Game1.MouseState.LeftButton == ButtonState.Pressed)
                {
                    CurrStartPos = new Vector2((float)cell.Key.x, (float)cell.Key.y);
                    currColor = cell.Value.Color;
                    adding = true;
                }
            }

            if (adding)
            {
                adding = !(Game1.MouseState.LeftButton == ButtonState.Released);

                (int mX, int mY) = MouseCell();

                bool isFirstLineBeingAdded = true;
                foreach (var cell in Grid)
                {
                    if (cell.Value.PieceType != PieceType.Dot && cell.Value.Color == currColor)
                    {
                        isFirstLineBeingAdded = false;
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
                if (mX == vectorToCheck.X && mY == vectorToCheck.Y)
                {
                    return;
                }

                bool hasDirectionBeenChosen = false;

                Direction directionChosen = Direction.None;

                if (mX > vectorToCheck.X)
                {
                    CurrDirection = Direction.Right;
                    if(PrevDirection == CurrDirection)
                    {
                        var offSetX = mX - 0.5f;
                        if(offSetX > vectorToCheck.X)
                        {
                            hasDirectionBeenChosen = true;
                        }
                    }

                    directionChosen = CurrDirection;
                }
                else if (mX < vectorToCheck.X && !hasDirectionBeenChosen)
                {
                    CurrDirection = Direction.Left;
                    if (PrevDirection == CurrDirection)
                    {
                        var offSetX = mX + 0.5f;
                        if (offSetX < vectorToCheck.X)
                        {
                            hasDirectionBeenChosen = true;
                        }
                        else
                        {
                            CurrDirection = directionChosen;
                        }
                    }

                    directionChosen = CurrDirection;
                }
                Game1.Title = hasDirectionBeenChosen.ToString();
                if (mY > vectorToCheck.Y && !hasDirectionBeenChosen)
                {
                    CurrDirection = Direction.Down;
                    if(PrevDirection == CurrDirection)
                    {
                        var offsetY = mY - 0.5f;
                        if(offsetY > vectorToCheck.Y)
                        {
                            hasDirectionBeenChosen = true;
                        }
                        else
                        {
                            CurrDirection = directionChosen;
                        }
                    }

                    directionChosen = CurrDirection;
                }
                else if (mY < vectorToCheck.Y && !hasDirectionBeenChosen)
                {
                    CurrDirection = Direction.Up;
                    if(PrevDirection == CurrDirection)
                    {
                        var offsetY = mY + 0.5f;
                        if(offsetY < vectorToCheck.Y)
                        {
                            hasDirectionBeenChosen = true;
                        }
                        else
                        {
                            CurrDirection = directionChosen;
                        }
                    }
                }
                #endregion

                Vector2 addPos = Vector2.One * -1;
                float rotation = 0f;
                
                switch (CurrDirection)
                {
                    case Direction.Up:
                        addPos = new Vector2(mX, mY + 0.5f);
                        break;

                    case Direction.Down:
                        addPos = new Vector2(mX, mY - 0.5f);
                        break;

                    case Direction.Left:
                        addPos = new Vector2(mX + 0.5f, mY);
                        rotation = 90;
                        break;

                    case Direction.Right:
                        addPos = new Vector2(mX - 0.5f, mY);
                        rotation = 90;
                        break;
                }
                
                if (addPos.X >= 0 && addPos.Y >= 0)
                {
                    Grid[(addPos.X, addPos.Y)] = new FlowPiece(currColor, PieceType.Line, rotation);
                    PrevPos = addPos;
                    PrevDirection = CurrDirection;
                }
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            Game1.Title = $"{IsBoardFull()}";

            foreach (var cell in Grid)
            {
                var piece = cell.Value;
                if (piece.PieceType == PieceType.Empty) continue;

                float x = (float)cell.Key.x;
                float y = (float)cell.Key.y;

                var texture = piece.PieceTexture[piece.PieceType];

                Vector2 pos = Vector2.Zero;

                switch (piece.PieceType)
                {
                    case PieceType.Dot:
                        pos = new Vector2(x * CellSize + CellSize / 2, y * CellSize + CellSize / 2);
                        break;

                    case PieceType.Line:
                        switch (piece.Rotation)
                        {
                            case 0:
                                pos = new Vector2(x * CellSize + CellSize / 2, y * CellSize + CellSize / 2);
                                break;

                            case 90:
                                pos = new Vector2(x * CellSize + CellSize / 2, y * CellSize + CellSize - CellSize / 2);
                                break;
                        }
                        break;
                }
                sb.Draw(texture, pos, null, piece.Color, piece.Rotation.ToRadians(), new Vector2(texture.Width / 2, texture.Height / 2), Scale.ToVector2(), SpriteEffects.None, 0f);

            }

            base.Draw(sb);
        }
    }
}
