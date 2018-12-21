using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowFree
{
    [DebuggerDisplay("{PieceType}: {ArrayPosition}")]
    public class FlowPiece
    {
        public Color Color;
        public PieceType PieceType;
        public float Rotation;
     
        public Point ArrayPosition;

        
        //drawX and drawY are specifically for the turn piece
        public FlowPiece(Color color, PieceType type, float rotation, int x, int y)
        {
            Color = color;
            PieceType = type;
            Rotation = rotation;

            
            ArrayPosition = new Point(x, y);
        }
    }
}
