using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace PathfindingVisualizer
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Grid<int> grid;
        private bool isSearching;
        private List<Node<int>> visitedNodes;
        private Node<int> currentNode;
        private List<Node<int>> frontier;
        private TimeSpan timer;
        private TimeSpan ticRate;
        private Button AStarButton;
        private Button DijstrasButton;
        private Func<List<Node<int>>, Node<int>> algo;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
            graphics.PreferredBackBufferHeight = 840;
            graphics.PreferredBackBufferWidth = 950;
            graphics.ApplyChanges();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            isSearching = false;

            Dictionary<NodeType, Color> colors = new Dictionary<NodeType, Color>
            {
                [NodeType.NotVisited] = Color.CornflowerBlue,
                [NodeType.Visited] = Color.Blue,
                [NodeType.Wall] = Color.Black,
                [NodeType.End] = Color.Green,
                [NodeType.Start] = Color.Red,
                [NodeType.Frontier] = Color.Purple,
                [NodeType.Path] = Color.Red,
            };

            grid = new Grid<int>(new Point(30, 30), 1, new Point(28, 28), Point.Zero, Color.Black, 2, colors);

            timer = TimeSpan.Zero;
            ticRate = TimeSpan.FromMilliseconds(10);

            SpriteFont font = Content.Load<SpriteFont>("ButtonFont");

            AStarButton = new Button(Color.Green, new Rectangle(870,28,56,28), "A*", font);
            DijstrasButton = new Button(Color.Green, new Rectangle(870, 84, 56, 28), "Dijstras", font);
            algo = DijstrasSelection;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            timer += gameTime.ElapsedGameTime;
            grid.Update();

            if(Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                isSearching = true;
                visitedNodes = new List<Node<int>>();
                currentNode = grid.Nodes[grid.Start.X, grid.Start.Y];
                currentNode.CumulitiveDistance = ManhattanHeuristic(currentNode, grid.Nodes[grid.End.X, grid.End.Y]);
                frontier = new List<Node<int>>();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                isSearching = false;
                grid.ResetNodes();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.C))
            {
                grid.ClearWalls();
            }

            if(DijstrasButton.IsClicked())
            {
                algo = DijstrasSelection;
            }
            else if (AStarButton.IsClicked())
            {
                algo = AStarSelection;
            }

            if (isSearching)
            {
                if(timer >= ticRate)
                {
                    if(Search(algo, ManhattanHeuristic))
                    {
                        GenerateRougt();
                        isSearching = false;
                    }
                    timer = TimeSpan.Zero;
                }
            }

            base.Update(gameTime);
        }

        private bool Search(Func<List<Node<int>>, Node<int>> selection, Func<Node<int>, Node<int>, float> heuristic)
        {
            for (int i = 0; i < currentNode.Neighbors.Count; i++)
            {
                float distanceFromStart = currentNode.DistanceFromStart + 1;
                Node<int> neighbor = currentNode.Neighbors[i].EndingNode;
                float cumulitiveDistance = distanceFromStart + heuristic(currentNode, grid.Nodes[grid.End.X, grid.End.Y]);
                neighbor.DistanceFromStart = distanceFromStart;
                neighbor.CumulitiveDistance = cumulitiveDistance;

                if (!Contains(visitedNodes, neighbor) && !Contains(frontier, neighbor) && neighbor.type != NodeType.Wall)
                {
                    frontier.Add(neighbor);
                    if (neighbor.type != NodeType.Start && neighbor.type != NodeType.End)
                    {
                        neighbor.type = NodeType.Frontier;
                    }
                    neighbor.Founder = currentNode;
                }
            }

            if(currentNode.type != NodeType.Start && currentNode.type != NodeType.End)
            {
                currentNode.type = NodeType.Visited;
            }

            visitedNodes.Add(currentNode);
            currentNode = selection(frontier);

            return currentNode.GridPos == grid.End;
        }

        static float ManhattanHeuristic(Node<int> startingNode, Node<int> endingNode)
        {
            float dx = Math.Abs(startingNode.GridPos.X - endingNode.GridPos.X);
            float dy = Math.Abs(startingNode.GridPos.Y - endingNode.GridPos.Y);
            return (float)((dx + dy) + (Math.Sqrt(2) - 2) * Math.Min(dx, dy));
        }

        static Node<int> DijstrasSelection(List<Node<int>> nodes)
        {
            Node<int> node = nodes[0];

            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].DistanceFromStart < node.DistanceFromStart)
                {
                    node = nodes[i];
                }
            }
            nodes.Remove(node);

            return node;
        }

        static Node<int> AStarSelection(List<Node<int>> nodes)
        {
            Node<int> node = nodes[0];

            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].CumulitiveDistance < node.CumulitiveDistance)
                {
                    node = nodes[i];
                }
            }
            nodes.Remove(node);

            return node;
        }

        private bool Contains(List<Node<int>> nodes, Node<int> node)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (node.GridPos == nodes[i].GridPos)
                {
                    return true;
                }
            }
            return false;
        }

        public void GenerateRougt()
        {
            List<Node<int>> path = new List<Node<int>>();
            while (currentNode.GridPos != grid.Nodes[grid.Start.X, grid.Start.Y].GridPos)
            {
                path.Add(currentNode);
                currentNode = currentNode.Founder;
            }
            path.Add(currentNode);
            path.Reverse();

            for (int i = 1; i < path.Count - 1; i++)
            {
                path[i].type = NodeType.Path;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            grid.Draw(spriteBatch);
            AStarButton.Draw(spriteBatch);
            DijstrasButton.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
