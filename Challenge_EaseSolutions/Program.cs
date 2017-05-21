using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Challenge_EaseSolutions
{
    enum Direction
    {
        Up,
        Down,
        Right,
        Left
    }

    public class Position
    {
        public int x { get; set; }
        public int y { get; set; }

        public Position(int row, int col)
        {
            x = row;
            y = col;
        }
    }

    public class Node
    {
        public int Id { get; set; }
        public Position Location { get; set; }
        public int NodeValue { get; set; }
        public int ParentID { get; set; }
        public int Level { get; set; }

        public Node(int id, Position location, int nodeValue, int parentId, int level)
        {
            Id = id;
            Location = location;
            NodeValue = nodeValue;
            ParentID = parentId;
            Level = level;
        }
    }

    class Program
    {
        static List<Node> allNodes = new List<Node>();
        static int[][] mapGrid;
        static int currentValueID = 1;
        static int currentLevel = 0;

        static void Main(string[] args)
        {
            try
            {
                // read the file and load it to jagged array
                mapGrid = File.ReadAllLines("C:\\Users\\Rey\\Downloads\\Challenge\\map.txt").Skip(1).Select(x => x.Split(' ').Select(y => int.Parse(y)).ToArray()).ToArray();

                // load base level nodes
                for (int i = 0; i < mapGrid.Length; i++)
                {
                    for (int j = 0; j < mapGrid[i].Length; j++)
                    {
                        AddBaseLevelNode(new Position(i, j), mapGrid[i][j]);
                    }
                }

                //loop until there is no nodes to traverse 
                while (AddSubLevelNode() > 0) { }

                var longestPath = allNodes.OrderByDescending(x => x.Level).FirstOrDefault().Level;
                var steepestPathNodes = new List<int[]>();
                allNodes.Where(y => y.Level == longestPath).OrderByDescending(x => x.Id).ToList().ForEach(y => steepestPathNodes.Add(GetNodeValues(y)));
                var steepestPath = steepestPathNodes.OrderByDescending(x => x.Max() - x.Min()).FirstOrDefault();

                Console.WriteLine("Length Of Calculated Path: {0}", longestPath + 1);
                Console.WriteLine("Drop Of Calculated Path: {0}", steepestPath.Max() - steepestPath.Min());
                Console.WriteLine("Calculated Path: ");
                steepestPath.Reverse().ToList().ForEach(x => Console.Write(x + " "));
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Error: {0}", ex.Message);
                Console.ReadLine();
            }
        }

        static int[] GetNodeValues(Node node)
        {
            var nodePath = new List<int>() { node.NodeValue };
            Node parentNode = null;

            do
            {
                parentNode = allNodes.Where(x => x.Id == node.ParentID).SingleOrDefault();

                if (parentNode != null)
                    nodePath.Add(parentNode.NodeValue);

                node = parentNode;
            } while (parentNode != null);

            return nodePath.ToArray();
        }

        static void AddBaseLevelNode(Position node, int nodeValue)
        {
            allNodes.Add(new Node(currentValueID++, node, nodeValue, 0, 0));
        }

        static int AddSubLevelNode()
        {
            int count = 0;
            allNodes.Where(x => x.Level == currentLevel).ToArray().ToList().ForEach(x =>
            {
                count += AddAdjacentNode(Direction.Up, x) +
                         AddAdjacentNode(Direction.Down, x) +
                         AddAdjacentNode(Direction.Right, x) +
                         AddAdjacentNode(Direction.Left, x);
            });
            currentLevel++;
            return count;
        }

        static int AddAdjacentNode(Direction direction, Node node)
        {
            Position adjacentNode = GetAdjacentNode(direction, node.Location);
            if (adjacentNode != null && mapGrid[adjacentNode.x][adjacentNode.y] < node.NodeValue)
            {
                allNodes.Add(new Node(currentValueID++, adjacentNode, mapGrid[adjacentNode.x][adjacentNode.y], node.Id, currentLevel + 1));
                return 1;
            }

            return 0;
        }

        static Position GetAdjacentNode(Direction direction, Position node)
        {
            Position adjacentNode = null;

            switch (direction)
            {
                case Direction.Up:
                    if (node.x - 1 >= 0)
                        adjacentNode = new Position(node.x - 1, node.y);
                    break;
                case Direction.Down:
                    if (node.x + 1 < mapGrid.GetLength(0))
                        adjacentNode = new Position(node.x + 1, node.y);
                    break;
                case Direction.Right:
                    if (node.y + 1 < mapGrid.GetLength(0))
                        adjacentNode = new Position(node.x, node.y + 1);
                    break;
                case Direction.Left:
                    if (node.y - 1 >= 0)
                        adjacentNode = new Position(node.x, node.y - 1);
                    break;
                default:
                    break;
            }

            return adjacentNode;
        }
    }
}
