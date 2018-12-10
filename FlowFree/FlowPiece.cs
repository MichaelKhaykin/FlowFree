using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowFree
{
    public class FlowPiece
    {
        public Color Color;
        public PieceType PieceType;
        public float Rotation;
        public Dictionary<PieceType, Texture2D> PieceTexture;
        

        public FlowPiece(Color color, PieceType type, float rotation)
        {
            Color = color;
            PieceType = type;
            Rotation = rotation;

            PieceTexture = new Dictionary<PieceType, Texture2D>()
            {
                [PieceType.Dot] = Game1.DotTexture,
                [PieceType.Line] = Game1.LineTexture
            };
        }
    }
}
