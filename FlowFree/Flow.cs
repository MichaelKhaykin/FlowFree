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

        Rectangle StartHitBox;
        Rectangle EndHitBox;

        bool shouldAddFromHead;

        public Flow(Color color, Point start, Point end, int cellsize, float scale)
        {
            var head = new FlowPiece(color, PieceType.Dot, 0, start.X, start.Y);
            var tail = new FlowPiece(color, PieceType.Dot, 0, end.X, end.Y);
            Pieces = new LinkedList(head, tail);

            StartHitBox = new Rectangle(start.X * cellsize, start.Y * cellsize, cellsize, cellsize);
            EndHitBox = new Rectangle(end.X * cellsize, end.Y * cellsize, cellsize, cellsize);

            CellSize = cellsize;

            Color = color;

            Board.Grid[start.X, start.Y] = (PieceType.Dot, color);
            Board.Grid[end.X, end.Y] = (PieceType.Dot, color);
            /*
            if (Color == Color.Red)
            {
                Pieces.AddLast(new FlowPiece(Color.Red, PieceType.SmallDot, 0f, 0, 1));
                Pieces.AddLast(new FlowPiece(Color.Red, PieceType.SmallDot, 0f, 0, 2));
                Pieces.AddLast(new FlowPiece(Color.Red, PieceType.SmallDot, 0f, 0, 3));
                Pieces.AddLast(new FlowPiece(Color.Red, PieceType.SmallDot, 0f, 0, 4));
                Pieces.AddLast(new FlowPiece(Color.Red, PieceType.SmallDot, 0f, 1, 4));

                Pieces.RipOut(FindPiece(new Point(0, 2)));
            }
            else if(Color == Color.Green)
            {
                Pieces.AddLast(new FlowPiece(Color.Green, PieceType.SmallDot, 0f, 3, 0));
                Pieces.AddLast(new FlowPiece(Color.Green, PieceType.SmallDot, 0f, 3, 1));
                Pieces.AddLast(new FlowPiece(Color.Green, PieceType.SmallDot, 0f, 3, 2));
                Pieces.AddLast(new FlowPiece(Color.Green, PieceType.SmallDot, 0f, 2, 2));
                Pieces.AddLast(new FlowPiece(Color.Green, PieceType.SmallDot, 0f, 1, 2));
                Pieces.AddLast(new FlowPiece(Color.Green, PieceType.SmallDot, 0f, 1, 3));

                Pieces.RipOut(FindPiece(new Point(2, 2)));
            }
            */
            Scale = scale;
        }

        public LinkedListNode FindPiece(Point pos)
        {
            var curr = Pieces.Head;
            while (curr != null)
            {
                if (curr.Value.ArrayPosition == pos)
                {
                    return curr;
                }
                curr = curr.Next;
            }

            curr = Pieces.Tail;
            while(curr != null)
            {
                if(curr.Value.ArrayPosition == pos)
                {
                    return curr;
                }
                curr = curr.Prev;
            }

            return null;
        }
        
        public void Update(GameTime gameTime)
        {
            (int x, int y) = MouseCell(CellSize);
          

            if (Game1.MouseState.LeftButton == ButtonState.Pressed && !shouldAdd)
            {
                if (StartHitBox.Contains(Game1.MouseState.Position))
                {
                    Pieces.Reset();

                    shouldAddFromHead = true;
                    if (Game1.OldMouseState.LeftButton == ButtonState.Released)
                    {
                        Board.CurrentColorActivated = Color;
                        shouldAdd = true;
                    }
                }
                else if (EndHitBox.Contains(Game1.MouseState.Position))
                {
                    Pieces.Reset();

                    shouldAddFromHead = false;
                    if (Game1.OldMouseState.LeftButton == ButtonState.Released)
                    {
                        Board.CurrentColorActivated = Color;
                        shouldAdd = true;
                    }
                }
                //logic
                //check if some flow piece of that color was pressed
                //if it was, check which direction it was going.
                //i.e check if it came from head or tail, by checking
                //which path is not null
                else if (Board.CurrentColorActivated == Color || Board.CurrentColorActivated == Color.White)
                {
                    var piece = FindPiece(new Point(x, y));
                    if (piece == null) return;

                    var curr = Pieces.Head;
                    while(curr != piece)
                    {
                        curr = curr.Next;
                        if (curr == null) break;
                    }

                    if(curr == null)
                    {
                        curr = Pieces.Tail;
                        while(curr != piece)
                        {
                            curr = curr.Prev;
                        }

                        shouldAddFromHead = false;
                        shouldAdd = true;
                    }
                    else
                    {
                        shouldAddFromHead = true;
                        shouldAdd = true;
                    }
                }
            }
            if (Game1.MouseState.LeftButton == ButtonState.Released)
            {
                shouldAdd = false;
            }

            if (shouldAdd)
            {
                if(x < 0 || x >= Board.Grid.GetLength(0)
                    || y < 0 || y >= Board.Grid.GetLength(1))
                {
                    return;
                }

                bool isNotMyColor = Board.Grid[x, y].color != Color;
                bool isDot = Board.Grid[x, y].type == PieceType.Dot;

                if (isNotMyColor && isDot)
                {
                    shouldAdd = false;
                    return;
                }

                if(Board.Grid[x, y].color != Color && Board.Grid[x, y].color != Color.White)
                {
                    //find flow and rip out at this spot
                    foreach(var flow in Board.Flows)
                    {
                        if(flow.Color == Board.Grid[x, y].color)
                        {
                            flow.Pieces.RipOut(new Point(x, y));
                        }
                    }
                }

                var flowPiece = new FlowPiece(Color, PieceType.SmallDot, 0f, x, y);
                bool isSuccess = false;

                if (shouldAddFromHead)
                {
                    isSuccess = Pieces.AddLast(flowPiece);
                }
                else
                {
                    isSuccess = Pieces.AddLastFromTail(flowPiece);
                }

                bool onPrevious = !isSuccess;
                if(onPrevious && !IsCompleted)
                {
                    if(shouldAddFromHead)
                    {
                        Pieces.RemoveLast();
                    }
                    else
                    {
                        Pieces.RemoveLastFromTail();
                    }
                }
            }
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
                if (i == 1)
                {
                    curr = Pieces.Tail;
                    while (curr.Prev != null)
                    {
                        curr = curr.Prev;
                    }
                }

                for (; curr != null; curr = curr.Next)
                {
                    //image                
                    if (curr == Pieces.Head && curr.Next != null)
                    {
                        curr.Value.PieceType = PieceType.DotWithHalf;

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
                        curr.Value.PieceType = PieceType.DotWithHalf;
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
                    else if (curr.Next != null && curr.Prev != null)
                    {
                        curr.Value.PieceType = PieceType.Turn;
                        //add in a bunch of checks for checking 
                        //the direction
                        var previousPos = new Vector2(curr.Prev.Value.ArrayPosition.X, curr.Prev.Value.ArrayPosition.Y);
                        var nextPos = new Vector2(curr.Next.Value.ArrayPosition.X, curr.Next.Value.ArrayPosition.Y);

                        var m = MidPoint(nextPos, previousPos);
                        var slope = m - curr.Value.ArrayPosition.ToVector2();
                        if (slope.X > 0 && slope.Y < 0)
                        {
                            curr.Value.Rotation = 0f;
                        }
                        else if (slope.X > 0 && slope.Y > 0)
                        {
                            curr.Value.Rotation = 90f;
                        }
                        else if (slope.X < 0 && slope.Y < 0)
                        {
                            curr.Value.Rotation = 270f;
                        }
                        else if (slope.X < 0 && slope.Y > 0)
                        {
                            curr.Value.Rotation = 180f;
                        }
                        else
                        {
                            curr.Value.PieceType = PieceType.Line;
                            bool isHorizontal = curr.Next.Value.ArrayPosition.Y == curr.Value.ArrayPosition.Y;
                            curr.Value.Rotation = isHorizontal ? 90 : 0;
                        }
                    }
                    else if (curr.Next == null && curr.Prev != null)
                    {
                        curr.Value.PieceType = PieceType.Line;
                        bool isHorizontal = curr.Prev.Value.ArrayPosition.Y == curr.Value.ArrayPosition.Y;
                        curr.Value.Rotation = isHorizontal ? 90 : 0;
                    }
                    else if (curr.Next != null && curr.Prev == null)
                    {
                        curr.Value.PieceType = PieceType.Line;
                        bool isHorizontal = curr.Next.Value.ArrayPosition.Y == curr.Value.ArrayPosition.Y;
                        curr.Value.Rotation = isHorizontal ? 90 : 0;
                    }

                    image = Game1.PieceTexture[curr.Value.PieceType];

                    Vector2 pos = new Vector2(curr.Value.ArrayPosition.X * CellSize + CellSize / 2, curr.Value.ArrayPosition.Y * CellSize + CellSize / 2);

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

                    Board.Grid[curr.Value.ArrayPosition.X, curr.Value.ArrayPosition.Y] = (curr.Value.PieceType, curr.Value.Color);
                    sb.Draw(image, pos, null, curr.Value.Color, curr.Value.Rotation.ToRadians(), new Vector2(image.Width / 2, image.Height / 2), Scale, SpriteEffects.None, 0);
                }
            }
        }
    }
}
