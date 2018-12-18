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
                prev = curr;
                curr = curr.Next;
            }

            if (value == Tail.Value)
            {
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
                next = curr;
                curr = curr.Prev;
            }

            if (value == Head.Value)
            {
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
        public void RipOut(FlowPiece value)
        {
            var curr = Head;
            while(curr.Value != value)
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

        void Reset()
        {
            Head.Next = null;
            Tail.Prev = null;
        }
    }
}
