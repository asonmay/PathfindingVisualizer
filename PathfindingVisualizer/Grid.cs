using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Input;
using System.Net.Security;

namespace PathfindingVisualizer
{
    public class Grid<T>
    {
        public Node<T>[,] Nodes;
        private Point size;
        private Point position;
        private Point hovorPos;
        private bool isPressed;
        public Point Start {  get; private set; }
        public Point End { get; private set; }

        public Grid(Point size, T value, Point tileSize, Point position, Color tileOutlineColor, int outlineThickness, Dictionary<NodeType, Color> colors)
        {
            Nodes = new Node<T>[size.X, size.Y];
            this.size = size;
            this.position = position;
            isPressed = false;
            Start = new Point(0, 0);
            End = new Point(size.X - 1, size.Y - 1);

            InitializeNodes(value, tileSize, tileOutlineColor, outlineThickness, colors);
            GenerateNeighbors();
            ResetNodes();
        }

        public void InitializeNodes(T value, Point tileSize, Color tileOutlineColor, int outlineThickness, Dictionary<NodeType, Color> colors)
        {
            for (int x = 0; x < size.X; x++)
            {
                for (int y = 0; y < size.Y; y++)
                {
                    Point pos = new Point(position.X + x * tileSize.X, position.Y + y * tileSize.Y);
                    if(new Point(x,y) == Start)
                    {
                        Nodes[x, y] = new Node<T>(value, tileSize, pos, outlineThickness, colors, tileOutlineColor, NodeType.Start, new Point(x,y));
                    }
                    else if(new Point(x,y) == End)
                    {
                        Nodes[x, y] = new Node<T>(value, tileSize, pos, outlineThickness, colors, tileOutlineColor, NodeType.End, new Point(x, y));
                    }
                    else
                    {
                        Nodes[x, y] = new Node<T>(value, tileSize, pos, outlineThickness, colors, tileOutlineColor, NodeType.NotVisited, new Point(x, y));
                    }
                }
            }
        }

        public void ResetNodes()
        {
            for(int x = 0;x < size.X ;x++)
            {
                for (int y = 0; y < size.Y; y++)
                {
                    Nodes[x, y].Founder = null;
                    Nodes[x, y].DistanceFromStart = Nodes[x,y].type == NodeType.Start ? 0 : float.MaxValue;
                    Nodes[x, y].CumulitiveDistance = float.MaxValue;
                    if (Nodes[x,y].type == NodeType.Path || Nodes[x,y].type == NodeType.Frontier || Nodes[x, y].type == NodeType.Visited)
                    {
                        Nodes[x,y].type = NodeType.NotVisited;
                    }
                }
            }
        }

        public void ClearWalls()
        {
            for (int x = 0; x < size.X; x++)
            {
                for (int y = 0; y < size.Y; y++)
                {
                    if (Nodes[x, y].type != NodeType.Start && Nodes[x, y].type != NodeType.End)
                    {
                        Nodes[x, y].type = NodeType.NotVisited;
                    }
                }
            }
        }

        public void GenerateNeighbors()
        { 
            for (int x = 0; x < size.X; x++)
            {
                for (int y = 0; y < size.Y; y++)
                {
                    if (x + 1 < size.X)
                    {
                        Nodes[x, y].AddNeighbor(Nodes[x + 1, y], 1);
                        Nodes[x + 1, y].AddNeighbor(Nodes[x, y], 1);
                    }
                    if (y + 1 < size.Y)
                    {
                        Nodes[x, y + 1].AddNeighbor(Nodes[x, y], 1);
                        Nodes[x, y].AddNeighbor(Nodes[x, y + 1], 1);
                    }
                    if (x + 1 < size.X && y + 1 < size.Y)
                    {
                        Nodes[x + 1, y + 1].AddNeighbor(Nodes[x, y], (float)Math.Sqrt(2));
                        Nodes[x , y ].AddNeighbor(Nodes[x + 1, y + 1], (float)Math.Sqrt(2));
                    }
                }
            }
        }

        public void Update()
        {
            for(int x = 0; x < size.X; x++)
            {
                for(int y = 0; y < size.Y; y++)
                {
                    Nodes[x, y].Update();
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for(int x = 0; x < size.X; x++)
            {
                for(int y = 0; y < size.Y; y++)
                {
                    Nodes[x,y].Draw(spriteBatch);
                }
            }
        }
    }
}
