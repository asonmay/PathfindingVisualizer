using System;
using System.Collections.Generic;
using System.Text;

namespace PathfindingVisualizer
{
    public class Edge<T>
    {
        public Node<T> StartingNode { get; }
        public Node<T> EndingNode { get; }
        public float Weight { get; }

        public Edge(Node<T> startingNode, Node<T> endingNode, float weight)
        {
            StartingNode = startingNode;
            EndingNode = endingNode;
            Weight = weight;
        }
    }
}
