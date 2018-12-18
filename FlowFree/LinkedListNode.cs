using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowFree
{
    public class LinkedListNode
    {
        public FlowPiece Value { get; set; }
        public LinkedListNode Next { get; set; }
        public LinkedListNode Prev { get; set; }

        public LinkedListNode(FlowPiece value)
        {
            Value = value;
        }
    }
}
