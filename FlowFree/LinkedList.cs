using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowFree
{
    public class LinkedList
    {
        public LinkedListNode Head { get; }
        public LinkedListNode Tail { get; }

        public LinkedList(FlowPiece head, FlowPiece tail)
        {
            Head = new LinkedListNode(head);
            Tail = new LinkedListNode(tail);
        }

        public void AddLast(FlowPiece value)
        {
            var curr = Head;
            var prev = Head;
            while (curr != null)
            {
                if (curr.Value.ArrayPosition == value.ArrayPosition) return;

                prev = curr;
                curr = curr.Next;
            }

            if (value.ArrayPosition == Tail.Value.ArrayPosition)
            {
              //  if (prev.Prev == null) return;

                Tail.Prev = prev;
                prev.Next = Tail;
                return;
            }

            curr = new LinkedListNode(value)
            {
                Prev = prev
            };

            prev.Next = curr;
        }
        public void AddLastFromTail(FlowPiece value)
        {
            var curr = Tail;
            var next = Tail;
            while (curr != null)
            {
                if (curr.Value.ArrayPosition == value.ArrayPosition) return;

                next = curr;
                curr = curr.Prev;
            }

            if (value.ArrayPosition == Head.Value.ArrayPosition)
            {
               // if (next.Next == null) return;

                Head.Next = next;
                next.Prev = Head;
                return;
            }

            curr = new LinkedListNode(value)
            {
                Next = next
            };

            next.Prev = curr;
        }
        public void RipOut(Point pos)
        {
            var curr = Head;
            while(curr.Value.ArrayPosition != pos)
            {
                curr = curr.Next;
            }

            curr.Prev.Next = null;
            curr.Next.Prev = null;
        }
        public void RemoveLast(FlowPiece value)
        {
            var curr = Head;
            while(curr.Next != null)
            {
                curr = curr.Next;
            }
            curr.Prev.Next = null;
        }
        public void RemoveLastFromTail(FlowPiece value)
        {
            var curr = Tail;
            while(curr.Prev != null)
            {
                curr = curr.Prev;
            }
            curr.Next.Prev = null;
        }
        

        public void Reset()
        {
            var curr = Head.Next;
            while(curr != null)
            {
                Board.Grid[curr.Value.ArrayPosition.X, curr.Value.ArrayPosition.Y] = (PieceType.SmallDot, Color.White);
                curr = curr.Next;
            }

            curr = Tail.Prev;
            while(curr != null)
            {
                Board.Grid[curr.Value.ArrayPosition.X, curr.Value.ArrayPosition.Y] = (PieceType.SmallDot, Color.White);
                curr = curr.Prev;
            }

            #region Reset Head And Tail
            Head.Value.PieceType = PieceType.Dot;
            Tail.Value.PieceType = PieceType.Dot;

            Head.Value.Rotation = 0f;
            Tail.Value.Rotation = 0f;

            Head.Next = null;
            Tail.Prev = null;
            #endregion
        }
    }
}
