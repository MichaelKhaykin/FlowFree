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

        public bool IsCompleted => Pieces.Last().ArrayPosition == EndPosition;

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
        
            EndPosition = end;

            Scale = scale;
        }

        public void Update(GameTime gameTime, bool[,] Grid)
        {
            if (Game1.MouseState.LeftButton == ButtonState.Pressed)
            {
                foreach (var piece in Pieces)
                {
                    var hitbox = new Rectangle(piece.ArrayPosition.X * CellSize, piece.ArrayPosition.Y * CellSize, CellSize, CellSize);
                    if (hitbox.Contains(Game1.MouseState.Position))
                    {
                        shouldAdd = true;
                    }
                }
            }
            else
            {
                shouldAdd = false;
            }

            if (!shouldAdd) return;

            (int x, int y) = MouseCell(CellSize);
            if (Grid[x, y] == false)
            {
                Pieces.AddLast(new FlowPiece(Color, PieceType.Line, 0f, x, y));
                Grid[x, y] = true;
            }
        }
        
        private (int x, int y) MouseCell(int CellSize)
        {
            return (Game1.MouseState.Position.X / CellSize, Game1.MouseState.Position.Y / CellSize);
        }

        public void Draw(SpriteBatch sb)
        {
            for (var curr = Pieces.First; curr != null; curr = curr.Next)
            { 
                Texture2D image = null;
            
                //image
                if (Pieces.Count == 1)
                {
                    image = Game1.DotTexture;
                }
                else if(curr == Pieces.First)
                {
                    image = Game1.DotHalf;
                    if (curr.Value.ArrayPosition.Y > curr.Next.Value.ArrayPosition.Y)
                    {
                         curr.Value.Rotation = 0;
                    }
                    else if (curr.Value.ArrayPosition.X < curr.Next.Value.ArrayPosition.X
                        || curr.Value.ArrayPosition.X > curr.Next.Value.ArrayPosition.X)
                    {
                        curr.Value.Rotation = 90f;
                    }
                }
                else if(curr == Pieces.Last)
                {
                    image = Game1.DotHalf;
                    if (curr.Value.ArrayPosition.Y > curr.Previous.Value.ArrayPosition.Y)
                    {
                        curr.Value.Rotation = 180;
                    }
                    else if (curr.Value.ArrayPosition.X < curr.Previous.Value.ArrayPosition.X
                        || curr.Value.ArrayPosition.X > curr.Previous.Value.ArrayPosition.X)
                    {
                        curr.Value.Rotation = 90f;
                    }
                }
                else if(curr.Previous.Value.ArrayPosition.X == curr.Next.Value.ArrayPosition.X 
                    || curr.Previous.Value.ArrayPosition.Y == curr.Next.Value.ArrayPosition.Y)
                {
                    image = Game1.LineTexture;
                }
                else
                {
                    image = Game1.TurnTexture;
                    //add in a bunch of checks for checking 
                    //the direction
                    if(curr.Value.ArrayPosition.Y == curr.Next.Value.ArrayPosition.Y
                        && curr.Value.ArrayPosition.X < curr.Next.Value.ArrayPosition.X)
                    {
                        curr.Value.Rotation = 0f;
                    }
                    else if(curr.Value.ArrayPosition.Y == curr.Next.Value.ArrayPosition.Y
                        && curr.Value.ArrayPosition.X > curr.Next.Value.ArrayPosition.X)
                    {
                        curr.Value.Rotation = 270f;
                    }
                    else if(curr.Value.ArrayPosition.X == curr.Next.Value.ArrayPosition.X 
                        && curr.Value.ArrayPosition.Y < curr.Next.Value.ArrayPosition.Y)
                    {
                        curr.Value.Rotation = 180f;
                    }
                    else
                    {
                        curr.Value.Rotation = 90f;
                    }
                }


                Vector2 pos = new Vector2(curr.Value.ArrayPosition.X * CellSize + CellSize / 2, curr.Value.ArrayPosition.Y * CellSize + CellSize / 2);
                //rotation
                switch(curr.Value.Rotation)
                {
                    case 0:
                        switch (image.Name)
                        {
                            case "LineTexture":

                                break;

                            case "FlowCorner":
                                pos.X += image.Width / 2;
                                pos.Y -= image.Height / 2;
                                break;
                        }
                        break;

                    case 90:
                        switch(image.Name)
                        {
                            case "LineTexture":

                                break;

                            case "FlowCorner":

                                break;
                        }
                        break;

                    case 180:
                        switch(image.Name)
                        {
                            case "LineTexture":

                                break;

                            case "FlowCorner":
                                pos.X -= image.Width / 2;
                                pos.Y += image.Height / 2;
                                break;
                        }
                        break;

                    case 270:
                        switch(image.Name)
                        {
                            case "LineTexture":

                                break;

                            case "FlowCorner":
                                pos.X -= image.Width / 2;
                                pos.Y -= image.Height / 2;
                                break;
                        }
                        break;
                }

                //draw the flow
                sb.Draw(image, pos, null, curr.Value.Color, curr.Value.Rotation.ToRadians(), new Vector2(image.Width / 2, image.Height / 2), Scale, SpriteEffects.None, 0);

            }
            //draw the end point
        }
    }
}
