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
        public LinkedList<FlowPiece> Pieces;

        public bool IsCompleted
        {
            get
            {
                if (Pieces.Count == 0) return false;
                return Pieces.Last().ArrayPosition == EndPosition;
            }
        }

        public Point EndPosition;

        bool shouldAdd = false;

        Color Color;

        int CellSize = 0;

        float Scale;

     
        public Flow(Color color, Point start, Point end, int cellsize, float scale)
        {
            Pieces = new LinkedList<FlowPiece>();

            CellSize = cellsize;

            Color = color;

            Pieces.AddLast(new FlowPiece(color, PieceType.Dot, 0, start.X, start.Y));

            Board.Grid[start.X, start.Y].isFilled = true;
            Board.Grid[start.X, start.Y].color = color;

            EndPosition = end;

            Scale = scale;
        }

        public void Update(GameTime gameTime)
        {
            if (Game1.MouseState.LeftButton == ButtonState.Pressed && !shouldAdd)
            {
                //if off flow, do nothing
                bool isOnFlow = false;
                foreach (var piece in Pieces)
                {
                    var hitbox = new Rectangle(piece.ArrayPosition.X * CellSize, piece.ArrayPosition.Y * CellSize, CellSize, CellSize);
                    if (hitbox.Contains(Game1.MouseState.Position))
                    {
                        isOnFlow = true;
                    }
                }
                var endHitBox = new Rectangle(EndPosition.X * CellSize, EndPosition.Y * CellSize, CellSize, CellSize);
                if (endHitBox.Contains(Game1.MouseState.Position))
                {
                    isOnFlow = true;
                }
                
                if (isOnFlow)
                {
                    var startHitBox = new Rectangle(Pieces.First().ArrayPosition.X * CellSize, Pieces.First().ArrayPosition.Y * CellSize, CellSize, CellSize);
                    endHitBox = new Rectangle(EndPosition.X * CellSize, EndPosition.Y * CellSize, CellSize, CellSize);
                    var lastPiece = new Rectangle(Pieces.Last().ArrayPosition.X * CellSize, Pieces.Last().ArrayPosition.Y * CellSize, CellSize, CellSize);

                    if (startHitBox.Contains(Game1.MouseState.Position))
                    {
                        Pieces.First().PieceType = PieceType.Dot;
                        Pieces.First().Rotation = 0f;

                        Clear(Pieces);
                    }
                    else if (endHitBox.Contains(Game1.MouseState.Position))
                    {
                        Board.Grid[Pieces.First().ArrayPosition.X, Pieces.First().ArrayPosition.Y].isFilled = false;
                        Board.Grid[Pieces.First().ArrayPosition.X, Pieces.First().ArrayPosition.Y].color = Color.White;
                        Board.Grid[EndPosition.X, EndPosition.Y].isFilled = true;
                        Board.Grid[EndPosition.X, EndPosition.Y].color = Color;

                        var temp = EndPosition;
                        EndPosition = Pieces.First().ArrayPosition;
                        Pieces.First().ArrayPosition = temp;

                        Pieces.First().PieceType = PieceType.Dot;
                        Pieces.First().Rotation = 0f;

                        Clear(Pieces);
                    }
                    if (lastPiece.Contains(Game1.MouseState.Position))
                    {
                        shouldAdd = true;
                    }
                }
            }

            if (Game1.MouseState.LeftButton == ButtonState.Released)
            {
                shouldAdd = false;
            }

            if ((!shouldAdd) || (IsCompleted)) return;

            (int x, int y) = MouseCell(CellSize);

            if(Board.Grid[x, y].isFilled && Board.Grid[x, y].color != Color)
            {
                //clear both paths
                var flowToClear = new LinkedList<FlowPiece>();
                foreach(var flow in Board.Flows)
                {
                    if (flow.Color == Board.Grid[x, y].color)
                    {
                        flowToClear = flow.Pieces;
                    }
                }

                Clear(Pieces);
                Clear(flowToClear);
            }

            var prevPos = Pieces.Last().ArrayPosition;

            if (Board.Grid[x, y].isFilled == false && (x == prevPos.X || y == prevPos.Y))
            {
                Pieces.AddLast(new FlowPiece(Color, PieceType.Line, 0f, x, y));
                Board.Grid[x, y].isFilled = true;
                Board.Grid[x, y].color = Color;
            }
            
        }

        private void Clear(LinkedList<FlowPiece> listToClear)
        {
            int count = listToClear.Count - 1;
            while (count > 0)
            {
                var piece = listToClear.ElementAt(count);
                Board.Grid[piece.ArrayPosition.X, piece.ArrayPosition.Y].isFilled = false;
                Board.Grid[piece.ArrayPosition.X, piece.ArrayPosition.Y].color = Color.White;
                listToClear.Remove(piece);
                count--;
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
            for (var curr = Pieces.First; curr != null; curr = curr.Next)
            {
                //image
                if (Pieces.Count == 1)
                {
                    image = Game1.DotTexture;
                }
                else if (curr == Pieces.First)
                {
                    image = Game1.DotHalf;
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
                else if (curr == Pieces.Last)
                {
                    image = Game1.DotHalf;
                    if (curr.Value.ArrayPosition.Y > curr.Previous.Value.ArrayPosition.Y)
                    {
                        curr.Value.Rotation = 180;
                    }
                    else if (curr.Value.ArrayPosition.X < curr.Previous.Value.ArrayPosition.X)
                    {
                        curr.Value.Rotation = 270;
                    }
                    else if (curr.Value.ArrayPosition.X > curr.Previous.Value.ArrayPosition.X)
                    {
                        curr.Value.Rotation = 90f;
                    }
                }
                else
                {
                    image = Game1.TurnTexture;
                    //add in a bunch of checks for checking 
                    //the direction
                    var previousPos = new Vector2(curr.Previous.Value.ArrayPosition.X, curr.Previous.Value.ArrayPosition.Y);
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
                        image = Game1.LineTexture;
                    }
                }

                Vector2 pos = new Vector2(curr.Value.ArrayPosition.X * CellSize + CellSize / 2, curr.Value.ArrayPosition.Y * CellSize + CellSize / 2);
                //rotation
                switch (curr.Value.Rotation)
                {
                    case 0:
                        switch (image.Name)
                        {
                            case "flowline":

                                break;

                            case "FlowCorner":
                                pos.X += image.Width / 2;
                                pos.Y -= image.Height / 2;
                                break;
                        }
                        break;

                    case 90:
                        switch (image.Name)
                        {
                            case "flowline":

                                break;

                            case "FlowCorner":
                                pos.X += image.Width / 2;
                                pos.Y += image.Height / 2;
                                break;
                        }
                        break;

                    case 180:
                        if (image.Name == "FlowCorner")
                        {
                            pos.X -= image.Width / 2;
                            pos.Y += image.Height / 2;
                        }
                        break;

                    case 270:
                        if (image.Name == "FlowCorner")
                        {
                            pos.X -= image.Width / 2;
                            pos.Y -= image.Height / 2;
                        }
                        break;
                }
                //draw the flow
                sb.Draw(image, pos, null, curr.Value.Color, curr.Value.Rotation.ToRadians(), new Vector2(image.Width / 2, image.Height / 2), Scale, SpriteEffects.None, 0);
            }

            var endImage = Game1.DotTexture;
            sb.Draw(endImage, new Vector2(EndPosition.X * CellSize + CellSize / 2, EndPosition.Y * CellSize + CellSize / 2), null, Color, 0f, new Vector2(endImage.Width / 2, endImage.Height / 2), Scale, SpriteEffects.None, 0f);
            //draw the end point
        }
    }
}
