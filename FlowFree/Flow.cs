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
    public class Flow
    {
        public LinkedList Pieces;

        public bool IsCompleted
        {
            get
            {
                var curr = Pieces.Head;
                while (curr.Next != null)
                {
                    curr = curr.Next;
                }

                return curr == Pieces.Tail;
            }
        }

        public bool shouldAdd = false;

        Color Color;

        int CellSize = 0;

        float Scale;

        public Flow(Color color, Point start, Point end, int cellsize, float scale)
        {
            var head = new FlowPiece(color, PieceType.Dot, 0, start.X, start.Y);
            var tail = new FlowPiece(color, PieceType.Dot, 0, end.X, end.Y);
            Pieces = new LinkedList(head, tail);

            CellSize = cellsize;

            Color = color;

            Board.Grid[start.X, start.Y] = (PieceType.Dot, color);
            Board.Grid[end.X, end.Y] = (PieceType.Dot, color);

            Scale = scale;
        }

        public void Update(GameTime gameTime)
        {
            (int x, int y) = MouseCell(CellSize);

        }

        private (int x, int y) MouseCell(int CellSize)
        {
            return (Game1.MouseState.Position.X / CellSize, Game1.MouseState.Position.Y / CellSize);
        }

        private Vector2 MidPoint(Vector2 a, Vector2 b)
        {
            return new Vector2((a.X + b.X) / 2, (a.Y + b.Y) / 2);
        }

        public void Draw(SpriteBatch sb)
        {
            Texture2D image = null;
            var curr = Pieces.Head;
            for (int i = 0; i < 2; i++)
            {
                if(i == 1)
                {
                    curr = Pieces.Tail;
                    while(curr.Prev != null)
                    {
                        curr = curr.Prev;
                    }
                }

                for (; curr != null; curr = curr.Next)
                {
                    //image                
                    if (curr == Pieces.Head && curr.Next != null)
                    {
                        if (curr.Value.ArrayPosition.Y > curr.Next.Value.ArrayPosition.Y)
                        {
                            curr.Value.Rotation = 180f;
                        }
                        else if (curr.Value.ArrayPosition.X < curr.Next.Value.ArrayPosition.X)
                        {
                            curr.Value.Rotation = 270f;
                        }
                        else if (curr.Value.ArrayPosition.X > curr.Next.Value.ArrayPosition.X)
                        {
                            curr.Value.Rotation = 90;
                        }
                    }
                    else if (curr == Pieces.Tail && curr.Prev != null)
                    {
                        if (curr.Value.ArrayPosition.Y > curr.Prev.Value.ArrayPosition.Y)
                        {
                            curr.Value.Rotation = 180;
                        }
                        else if (curr.Value.ArrayPosition.X < curr.Prev.Value.ArrayPosition.X)
                        {
                            curr.Value.Rotation = 270;
                        }
                        else if (curr.Value.ArrayPosition.X > curr.Prev.Value.ArrayPosition.X)
                        {
                            curr.Value.Rotation = 90f;
                        }
                    }
                    else if (curr.Next != null)
                    {
                        curr.Value.PieceType = PieceType.Turn;
                        //add in a bunch of checks for checking 
                        //the direction
                        var previousPos = new Vector2(curr.Prev.Value.ArrayPosition.X, curr.Prev.Value.ArrayPosition.Y);
                        var nextPos = new Vector2(curr.Next.Value.ArrayPosition.X, curr.Next.Value.ArrayPosition.Y);

                        var m = MidPoint(nextPos, previousPos);
                        var slope = m - curr.Value.ArrayPosition.ToVector2();
                        if ((slope.X > 0 && slope.X < 2) && (slope.Y < 0 && slope.Y > -2))
                        {
                            curr.Value.Rotation = 0f;
                        }
                        else if ((slope.X > 0 && slope.X < 2) && (slope.Y > 0 && slope.Y < 2))
                        {
                            curr.Value.Rotation = 90f;
                        }
                        else if ((slope.X < 0 && slope.X > -2) && (slope.Y < 0 && slope.Y > -2))
                        {
                            curr.Value.Rotation = 270f;
                        }
                        else if ((slope.X < 0 && slope.X > -2) && (slope.Y > 0 && slope.Y < 2))
                        {
                            curr.Value.Rotation = 180f;
                        }
                        else if (slope.X == 0)
                        {
                            curr.Value.PieceType = PieceType.Line;
                        }
                    }

                    image = Game1.PieceTexture[curr.Value.PieceType];

                    Vector2 pos = new Vector2(curr.Value.ArrayPosition.X * CellSize + CellSize / 2, curr.Value.ArrayPosition.Y * CellSize + CellSize / 2);
                    //rotation

                    if (curr.Value.PieceType == PieceType.Turn)
                    {
                        switch (curr.Value.Rotation)
                        {
                            case 0:
                                pos.X += image.Width / 2;
                                pos.Y -= image.Height / 2;
                                break;

                            case 90:
                                pos.X += image.Width / 2;
                                pos.Y += image.Height / 2;
                                break;

                            case 180:
                                pos.X -= image.Width / 2;
                                pos.Y += image.Height / 2;
                                break;

                            case 270:
                                pos.X -= image.Width / 2;
                                pos.Y -= image.Height / 2;
                                break;
                        }
                    }
                    //draw the flow
                    sb.Draw(image, pos, null, curr.Value.Color, curr.Value.Rotation.ToRadians(), new Vector2(image.Width / 2, image.Height / 2), Scale, SpriteEffects.None, 0);
                }
            }
        }
    }
}
