using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using MonoGame.Extended;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Net;

namespace PathfindingVisualizer
{
    public enum NodeType
    {
        NotVisited,
        Wall,
        Visited,
        Frontier,
        Start,
        End,
        Path
    }
    public class Node<T>
    {
        public T Value { get; }

        public List<Edge<T>> Neighbors { get; }

        public NodeType type { get; set; }

        public float CumulitiveDistance { get; set; }
        public float DistanceFromStart { get; set; }

        public Point GridPos { get; set; }

        public Node<T> Founder { get; set; }
        
        public Rectangle Hitbox
        {
            get
            {
                return new Rectangle(position, tileSize);
            }
        }

        private Point tileSize;
        private Point position;
        private Color outlineColor;
        private Dictionary<NodeType, Color> colors;
        private int outlineThickness;
        private bool isPressed;

        public Node(T value, Point tileSize, Point position, int outlineThickness, Dictionary<NodeType, Color> colors, Color outlineColor, NodeType type, Point gridPos)
        {
            Value = value;
            Neighbors = new List<Edge<T>>();
            this.tileSize = tileSize;
            this.position = position;
            isPressed = false;
            this.colors = colors;
            this.outlineColor = outlineColor;
            this.outlineThickness = outlineThickness;
            this.type = type;
            GridPos = gridPos;
        }

        public void AddNeighbor(Node<T> node, float weight) => Neighbors.Add(new Edge<T>(this, node, weight));

        public void Update()
        {
            MouseState mouse = Mouse.GetState();

            if(new Rectangle(mouse.Position, new Point(1,1)).Intersects(Hitbox) && mouse.LeftButton == ButtonState.Pressed && !isPressed)
            {
                isPressed = true;
                type = type == NodeType.NotVisited ? NodeType.Wall : NodeType.NotVisited;
                
            }
            else if(mouse.LeftButton == ButtonState.Released)
            {
                isPressed = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.FillRectangle(new Rectangle(position, tileSize), colors[type]);       
            spriteBatch.DrawRectangle(new Rectangle(position, tileSize), outlineColor, outlineThickness);
        }
    }
}
