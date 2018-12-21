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
        public int Count { get; set; }

        public LinkedList(FlowPiece head, FlowPiece tail)
        {
            Count = 2;

            Head = new LinkedListNode(head);
            Tail = new LinkedListNode(tail);
        }

        public bool AddLast(FlowPiece value)
        {
            var curr = Head;
            var prev = Head;
            while (curr != null)
            {
                if (curr.Value == value)
                {
                    // If we're over a piece that's last, return true;
                    // else, we went backwards in the flow

                    bool isNextNodeEmpty = curr.Next == null;
                    bool isAtEndOfFlow = curr.Value.PieceType == PieceType.Dot || curr.Value.PieceType == PieceType.DotWithHalf;
                    bool isThisLastPieceInFlow = isNextNodeEmpty || isAtEndOfFlow;

                    // Special case - we went back to the originating dot
                    if(isAtEndOfFlow && !isNextNodeEmpty)
                    {
                        return false;
                    }

                    return isThisLastPieceInFlow;
                }

                prev = curr;
                curr = curr.Next;
            }
            
            if (value == Tail.Value)
            {
                Tail.Prev = prev;
                prev.Next = Tail;
                return true;
            }

            curr = new LinkedListNode(value)
            {
                Prev = prev
            };

            prev.Next = curr;

            Count++;

            return true;
        }
        public bool AddLastFromTail(FlowPiece value)
        {
            var curr = Tail;
            var next = Tail;
            while (curr != null)
            {
                if (curr.Value == value)
                {
                    // If we're over a piece that's last, return true;
                    // else, we went backwards in the flow

                    bool isPrevNodeEmpty = curr.Prev == null;
                    bool isAtEndOfFlow = curr.Value.PieceType == PieceType.Dot || curr.Value.PieceType == PieceType.DotWithHalf;
                    bool isThisLastPieceInFlow = isPrevNodeEmpty || isAtEndOfFlow;

                    if(isThisLastPieceInFlow && !isPrevNodeEmpty)
                    {
                        return false;
                    }

                    return isThisLastPieceInFlow;
                }

                next = curr;
                curr = curr.Prev;
            }

            if (value == Head.Value)
            {
                Head.Next = next;
                next.Prev = Head;
                return true;
            }

            curr = new LinkedListNode(value)
            {
                Next = next
            };

            next.Prev = curr;

            Count++;

            return true;
        }
        public void RipOut(Point pos)
        {
            if (Count == 2) return;

            var curr = Head;
            while (curr.Value.ArrayPosition != pos)
            {
                curr = curr.Next;
                if (curr == null) break;
            }

            if (curr == null)
            {
                curr = Tail;
                while (curr.Value.ArrayPosition != pos)
                {
                    curr = curr.Prev;
                    //IF THIS IF STATEMENT HITS BIG PROBLEMS
                    if (curr == null) throw new Exception("Big problems");
                }
            }

            if (curr.Prev != null)
            {
                curr.Prev.Next = null;
            }
            if (curr.Next != null)
            { 
                curr.Next.Prev = null;
            }
            Count--;
        }
        public void RemoveLast()
        {
            var curr = Head;
            while (curr.Next != null)
            {
                curr = curr.Next;
            }
            curr.Prev.Next = null;
            Count--;
        }
        public void RemoveLastFromTail()
        {
            var curr = Tail;
            while (curr.Prev != null)
            {
                curr = curr.Prev;
            }
            curr.Next.Prev = null;

            Count--;
        }
        public void Reset()
        {
            var curr = Head.Next;
            while (curr != null)
            {
                Board.Grid[curr.Value.ArrayPosition.X, curr.Value.ArrayPosition.Y] = (PieceType.SmallDot, Color.White);
                curr = curr.Next;
            }

            curr = Tail.Prev;
            while (curr != null)
            {
                Board.Grid[curr.Value.ArrayPosition.X, curr.Value.ArrayPosition.Y] = (PieceType.SmallDot, Color.White);
                curr = curr.Prev;
            }

            Count = 2;

            #region Reset Head And Tail
            Head.Value.PieceType = PieceType.Dot;
            Tail.Value.PieceType = PieceType.Dot;

            Head.Value.Rotation = 0f;
            Tail.Value.Rotation = 0f;

            Head.Next = null;
            Tail.Prev = null;

            Board.Grid[Head.Value.ArrayPosition.X, Head.Value.ArrayPosition.Y] = (PieceType.Dot, Head.Value.Color);
            Board.Grid[Tail.Value.ArrayPosition.X, Tail.Value.ArrayPosition.Y] = (PieceType.Dot, Tail.Value.Color);


            #endregion
        }
    }
}
