using ReLogic.Threading;
using Terrapain.Content.Groups;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terrapain.Content.Functions;
using static Terrapain.Content.TUtilities.PathFinderSystem;

namespace Terrapain.Content.TUtilities
{
    public static class AIHelper
    {
        public static void CommonTerrapainFlyingMovement(Entity entity, Vector2 targetPosition, float rotatingSpeed, float MaxSpeed, float acceleration, float BreakingZone, bool instantBreak = true)
		{
			if (entity.Center == targetPosition)
			{
				if (BreakingZone > 0)
				{
					if (instantBreak)
						entity.velocity = Vector2.Zero;
                }
				return;
			}
            float maxVelocityMultyplier = 1;
            entity.velocity += entity.DirectionTo(targetPosition) * acceleration;
            if (entity.Distance(targetPosition) < BreakingZone)
            {
                maxVelocityMultyplier = 1 - (BreakingZone - entity.Distance(targetPosition)) / BreakingZone;
            }
            Vector2 vectorToTargetPosition = targetPosition - entity.Center;
            float positiveRotation = AngleBetweenVectors(vectorToTargetPosition, entity.velocity);
            positiveRotation = NormalizeRotation(positiveRotation);
            float negativeRotation = AngleBetweenVectors(entity.velocity, vectorToTargetPosition);
            negativeRotation = NormalizeRotation(negativeRotation);
            if (positiveRotation > negativeRotation)
            {
                entity.velocity.RotateBy(MathF.Max(-negativeRotation, -rotatingSpeed));
            }
            else
            {
                entity.velocity.RotateBy(MathF.Min(positiveRotation, rotatingSpeed));
            }
            if (entity.velocity.Length() > MaxSpeed * maxVelocityMultyplier)
            {
                if (instantBreak)
                    entity.velocity = entity.velocity.ToUnit() * MaxSpeed * maxVelocityMultyplier;
                else if (entity.velocity.Length() > 0)
                    entity.velocity = entity.velocity.Normalized() * MathF.Max(entity.velocity.Length() - acceleration * 2, MaxSpeed * maxVelocityMultyplier);
            }
        }
        public static void CommonTerrapainFlyingMovement(Vector2 position, ref Vector2 velocity, Vector2 targetPosition, float rotatingSpeed, float MaxSpeed, float acceleration, float BreakingZone)
        {
            float maxVelocityMultyplier = 1;
            if (targetPosition != position)
            {
                velocity += position.DirectionTo(targetPosition) * acceleration;
            }
            if (position.Distance(targetPosition) < BreakingZone)
            {
                maxVelocityMultyplier = 1 - (BreakingZone - position.Distance(targetPosition)) / BreakingZone;
            }
            Vector2 vectorToTargetPosition = targetPosition - position;
            float positiveRotation = AngleBetweenVectors(vectorToTargetPosition, velocity);
            positiveRotation = NormalizeRotation(positiveRotation);
            float negativeRotation = AngleBetweenVectors(velocity, vectorToTargetPosition);
            negativeRotation = NormalizeRotation(negativeRotation);
            if (positiveRotation > negativeRotation)
            {
                velocity.RotateBy(MathF.Max(-negativeRotation, -rotatingSpeed));
            }
            else
            {
                velocity.RotateBy(MathF.Min(positiveRotation, rotatingSpeed));
            }
            if (velocity.Length() > MaxSpeed * maxVelocityMultyplier)
            {
                velocity = velocity.ToUnit() * MaxSpeed * maxVelocityMultyplier;
            }
        }
        public static void OnlyRotationalMovement(Entity entity, Vector2 targetPosition, float rotatingSpeed)
        {
            if (targetPosition == entity.Center)
            {
                return;
            }
            Vector2 vectorToTargetPosition = targetPosition - entity.Center;
            float positiveRotation = AngleBetweenVectors(vectorToTargetPosition, entity.velocity);
            positiveRotation = NormalizeRotation(positiveRotation);
            float negativeRotation = AngleBetweenVectors(entity.velocity, vectorToTargetPosition);
            negativeRotation = NormalizeRotation(negativeRotation);
            if (positiveRotation > negativeRotation)
            {
                entity.velocity.RotateBy(MathF.Max(-negativeRotation, -rotatingSpeed));
            }
            else
            {
                entity.velocity.RotateBy(MathF.Min(positiveRotation, rotatingSpeed));
            }
        }
        public static bool AngularAcceleration(ref float angularVelocity, float acceleration, float maxAngularVelocity, float goalRotation, ref float rotation, bool Break = true)
		{
			bool rotateToTarget = false;
            goalRotation = NormalizeRotation(goalRotation, true);
            rotation = NormalizeRotation(rotation, true);

            if (rotation != goalRotation)
            {
                if (goalRotation < (float)Math.PI)
                {
                    if (rotation > goalRotation && rotation < goalRotation + Math.PI)
                    {
                        if (angularVelocity > -maxAngularVelocity)
                            angularVelocity -= acceleration;
                    }
                    else
                    {
                        if (angularVelocity < maxAngularVelocity)
                            angularVelocity += acceleration;
                    }
                }
                else
                {
                    if (rotation < goalRotation && rotation > goalRotation - Math.PI)
                    {
                        if (angularVelocity < maxAngularVelocity)
                            angularVelocity += acceleration;
                    }
                    else
                    {
                        if (angularVelocity > -maxAngularVelocity)
                            angularVelocity -= acceleration;
                    }
                }
                if ((rotation + angularVelocity > goalRotation && rotation < goalRotation) || (rotation + angularVelocity < goalRotation && rotation > goalRotation))
                {
                    rotation = goalRotation;
                    rotateToTarget = true;
					if (Break)
					{
						angularVelocity = 0;
					}
                }
                goalRotation += 2 * (float)Math.PI;
                if ((rotation + angularVelocity > goalRotation && rotation < goalRotation) || (rotation + angularVelocity < goalRotation && rotation > goalRotation))
                {
                    rotation = goalRotation;
                    rotateToTarget = true;
					if (Break)
					{
						angularVelocity = 0;
					}
                }
                goalRotation -= 4 * (float)Math.PI;
                if ((rotation + angularVelocity > goalRotation && rotation < goalRotation) || (rotation + angularVelocity < goalRotation && rotation > goalRotation))
                {
                    rotation = goalRotation;
                    rotateToTarget = true;
					if (Break)
                    { 
						angularVelocity = 0; 
					}
                }
                else
                {
                    rotation += angularVelocity;
                }
            }
			return rotateToTarget;
        }
        public static bool TryGetGroup<T>(this NPC npc, out T group) where T : Group
        {
            var t = npc.GetT();
            foreach(int g in t.MyGroups)
            {
                Group _group = Terrapain.group[g];
                if (_group is T)
                {
                    group = (T)_group;
                    return true;
                }
            }
            group = null;
            return false;
        }
        struct Graph
        {
            public List<Graph> hui;
            public Point origin;
            public bool reachTarget;
            public void SetReachTarget()
            {
                reachTarget = true;
            }
            public Graph()
            {
                hui = [];
            }
        }
        public static bool Include(this Rectangle rectangle, Point point)
        {
            return point.X >= rectangle.X && point.X <= rectangle.Right && point.Y >= rectangle.Y && point.Y <= rectangle.Bottom;
        }
        public static List<Vector2> FindPath(this Entity npc, Point target, int radius)
        {
            Point p = npc.Center.ToTileCoordinates();
            Rectangle area = new Rectangle(p.X - radius, p.Y - radius, radius * 2, radius * 2);
            return FindPath(npc, target, area);
        }
        public static List<Vector2> FindPath(this Entity npc, Point target, Rectangle area)
        {
            var _start = DateTime.Now;
            Point start = (npc.BottomRight - Vector2.One).ToTileCoordinates();
            if (!area.Include(target) || !area.Include(start))
            {
                return null;
            }
            int w = ((npc.width - 1) >> 4) + 1;
            int h = ((npc.height - 1) >> 4) + 1;
            bool halfBlock = (npc.height - 1) % 16 < 8;
            bool[,] map = GetMap(area, w, h, halfBlock);
            var getMapTime = DateTime.Now;
            Console.WriteLine("GetMap time: " + (getMapTime - _start).TotalMilliseconds + "ms");
            List<Node> nodes = GetArea(area);
            var getAreaTime = DateTime.Now;
            Console.WriteLine("GetArea time: " + (getAreaTime - getMapTime).TotalMilliseconds + "ms");
            // bool first = true;
            // if (Main.GameUpdateCount % 10 == 0)
            // {
            //     foreach (var node in nodes)
            //     {
            //         Dust.NewDust(node.ApplyOffset(new Point(w, h)).ToWorldCoordinates(), 0, 0, DustID.Torch);
            //         foreach (var link in node.links)
            //         {  
            //                 int d = Dust.NewDust(link.ApplyOffset(new Point(w, h)).ToWorldCoordinates() * 0.2f + node.ApplyOffset(new Point(w, h)).ToWorldCoordinates() * 0.8f, 0, 0, ModContent.DustType<PinkHeart>());
            //                 Main.dust[d].noGravity = true;
            //                 Main.dust[d].velocity = Vector2.Zero;
            //                 Main.dust[d].position = link.ApplyOffset(new Point(w, h)).ToWorldCoordinates() * 0.2f + node.ApplyOffset(new Point(w, h)).ToWorldCoordinates() * 0.8f;
            //                 d = Dust.NewDust(link.ApplyOffset(new Point(w, h)).ToWorldCoordinates() * 0.4f + node.ApplyOffset(new Point(w, h)).ToWorldCoordinates() * 0.6f, 0, 0, ModContent.DustType<PinkHeart>());
            //                 Main.dust[d].noGravity = true;
            //                 Main.dust[d].velocity = Vector2.Zero;
            //                 Main.dust[d].position = link.ApplyOffset(new Point(w, h)).ToWorldCoordinates() * 0.4f + node.ApplyOffset(new Point(w, h)).ToWorldCoordinates() * 0.6f;
            //         }
            //     }
            // }
            getAreaTime = DateTime.Now;
            Graph graph = new Graph() { origin = start};
            BuildGraph(ref graph, map, nodes, target, w, h, halfBlock, area);
            var buildGraphTime = DateTime.Now;
            Console.WriteLine("BuildGraph time: " + (buildGraphTime - getAreaTime).TotalMilliseconds + "ms");
            List<List<Point>> Pathes = BuildPathes(graph);
            var buildPathesTime = DateTime.Now;
            Console.WriteLine("BuildPathes time: " + (buildPathesTime - buildGraphTime).TotalMilliseconds + "ms");
            int shortest = -1;
            float length = -1;
            if (Pathes != null)
            {
                for (int i = 0; i < Pathes.Count; i++)
                {
                    float _length = 0;
                    List<Point> path = Pathes[i];
                    for (int j = 0; j < path.Count - 1; j++)
                    {
                        _length += path[j].ToVector2().Distance(path[j + 1].ToVector2());
                    }
                    if (shortest == -1 || _length < length)
                    {
                        length = _length;
                        shortest = i;
                    }
                }
            }
            //var buildPathesTime = DateTime.Now;
            Console.WriteLine("FindFastest time: " + (DateTime.Now - buildPathesTime).TotalMilliseconds + "ms");
            Console.WriteLine("Pathfinding time: " + (DateTime.Now - _start).TotalMilliseconds + "ms");
            if (shortest == -1)
            {
                return null;
            }
            else
            {
                List<Vector2> path = new List<Vector2>();
                Vector2 _offset = new Vector2((float)w * 8 - 16, (float)h * 8 - 16);
                foreach(var point in Pathes[shortest])
                {
                    path.Add(point.ToWorldCoordinates(-_offset.X, -_offset.Y));
                }
                return path;
            }
        }
        private static void BuildGraph(ref Graph graph, bool[,] map, List<Node> nodes, Point target, int width, int height, bool halfBlock, Rectangle area)
        {
            List<Point> newNodes = [];
            foreach(var n in nodes)
            {
                newNodes.Add(n.point);
            }
            Point size = new Point(width, height);
            if (MakeGraphLink(ref graph, map, target, true, area.Location))
            {
                Graph _ = graph.hui[0];
                _.reachTarget = true;
                graph.hui[0] = _;
            }
            else
            {
                List<Node> nodesToContinue = [];
                for (int i = 0; i < nodes.Count; i++)
                {    
                    Node node = nodes[i];
                    Point coordinates = node.ApplyOffset(size) - area.Location;
                    int w = map.GetLength(0);
                    int h = map.GetLength(1);
                    if (coordinates.X < map.GetLength(0) && coordinates.Y < map.GetLength(1))
                    {    
                        bool? check = node.Check(size, halfBlock);
                        if (!check.HasValue)
                        {
                            check = node.CheckOnMap(map, coordinates, size, halfBlock);
                        }

                        if ((check?? false) && MakeGraphLink(ref graph, map, coordinates + area.Location, false, area.Location))
                        {
                            newNodes.Remove(node.point);
                            nodesToContinue.Add(node);
                        }
                        SetNode(node);
                        //Node? test = TryGetNode(node.point);
                    }
                }

                for (int i = 0; i < graph.hui.Count; i++)
                {
                    Graph graf = graph.hui[i];
                    Node node = nodesToContinue[i];
                    BuildGraph(ref graf, node, map, target, size, halfBlock, area.Location, newNodes);
                    graph.hui[i] = graf;
                }
            }
        }
        private static void BuildGraph(ref Graph graph, Node node, bool[,] map, Point target, Point size, bool halfBlock, Point location, List<Point> nodes)
        {
            if (MakeGraphLink(ref graph, map, target, true, location))
            {
                Graph _ = graph.hui[0];
                _.reachTarget = true;
                graph.hui[0] = _;
            }
            else
            {
                List<Node> nodesToContinue = [];
                for (int i = 0; i < node.links.Count; i++)
                {
                    Link link = node.links[i];
                    bool? check = link.Check(size, halfBlock);
                    if ((check ?? true) && nodes.Contains(link.point))
                    {
                        if (link.point == node.point)
                        {
                            
                        }
                        // Point coordinates = link.ApplyOffset(size) - location;
                        if (!check.HasValue)
                        {
                            if (TryGetNode(link.point, out Node node1))
                            {
                                bool? check1 = node1.Check(size, halfBlock);
                                if (!check1.HasValue)
                                {
                                    check1 = node1.CheckOnMap(map, node1.ApplyOffset(size) - location, size, halfBlock);
                                }
                                if (check1?? false)
                                {    
                                    if (MakeGraphLink(ref graph, map, link.ApplyOffset(size), false, location))
                                    {
                                        link.sizeMin = size;
                                        link.minHalfBlock = halfBlock;
                                        nodesToContinue.Add(node1);
                                        nodes.Remove(link.point);
                                    }
                                    else
                                    {
                                        link.sizeMax = size;
                                        link.maxHalfBlock = halfBlock;
                                    }
                                    node.links[i] = link;
                                    if (node1.TryGetLink(node.point, out Link link1, out int j))
                                    {
                                        link1 = link;
                                        link1.point = node.point;
                                        link1.offset = node.offset;
                                        node1.links[j] = link1;
                                        SetNode(node1);
                                    }
                                    else
                                    {
                                        
                                    }
                                }
                                else
                                {
                                    nodes.Remove(link.point);
                                    link.sizeMax = size;
                                    link.maxHalfBlock = halfBlock;
                                }
                            }
                            else
                            {
                                node.links.RemoveAt(i);
                                i--;
                            }
                        }
                        else
                        {
                            if (check?? false)
                            {
                                nodes.Remove(link.point);
                                graph.hui.Add(new Graph() { origin = link.ApplyOffset(size) });
                                Node? node1 = TryGetNode(link.point);
                                if (node1.HasValue)
                                {
                                    nodesToContinue.Add(node1.Value);
                                    nodes.Remove(node1.Value.point);
                                }
                                else
                                {
                                    graph.hui.RemoveAt(graph.hui.Count - 1);
                                }
                            }
                        }
                    }
                }
                SetNode(node);

                for (int i = 0; i < graph.hui.Count; i++)
                {
                    Graph graf = graph.hui[i];
                    Node node2 = nodesToContinue[i];
                    BuildGraph(ref graf, node2, map, target, size, halfBlock, location, new List<Point> (nodes));
                    graph.hui[i] = graf;
                }
            }
        }
        private static bool MakeGraphLink(ref Graph graph, bool[,] map, Point node, bool target, Point offset)
        {
            Point left;
            Point right;
            if(graph.origin.X - node.X > 0)
            {
                left = node - offset;
                right = graph.origin - offset;
            }
            else
            {
                right = node - offset;
                left = graph.origin - offset;
            }
            int w = right.X - left.X;
            int h = right.Y - left.Y;
            if  (!target && w + Math.Abs(h) > 25)
            {
                return false;
            }
            if (w != 0)
            {
                float k = (float)h / (w + 1);
                float y = 0.5f + left.Y;
                for (int x = left.X; x <= right.X; x++)
                {
                    int oldY = (int)y;
                    y += k;
                    int ymin = Math.Min(oldY, (int)y);
                    int ymax = Math.Max(oldY, (int)y) + 1;
                    for (int _y = ymin; _y < ymax; _y++)
                    {
                        if (map[x, _y])
                        {
                            //Dust.NewDust((new Point(x, _y) + offset).ToWorldCoordinates(), 0, 0, DustID.RedTorch, Scale: 0.25f);
                            return false;
                        }
                        //Dust.NewDust((new Point(x, _y) + offset).ToWorldCoordinates(), 0, 0, DustID.GreenTorch, Scale: 0.25f);
                    }
                }
            }
            else
            {
                int ymin = Math.Min(left.Y, right.Y);
                int ymax = Math.Max(left.Y, right.Y) + 1;
                for (int y = ymin; y < ymax; y++)
                {
                    if (map[left.X, y])
                    {
                        //Dust.NewDust((new Point(left.X, y) + offset).ToWorldCoordinates(), 0, 0, DustID.RedTorch, Scale: 0.25f);
                        return false;
                    }
                }
            }
            graph.hui.Add(new Graph() { origin = node });
            return true;
        }
        private static List<List<Point>> BuildPathes(Graph graph)
        {
            if (graph.reachTarget)
            {
                return new ([[graph.origin]]);
            }
            List<List<Point>> pathes = [];
            foreach (var g in graph.hui)
            {
                List<List<Point>> _pathes = BuildPathes(g);
                if (_pathes != null)
                {
                    for (int i = 0; i < _pathes.Count; i++)
                    {
                        var path = _pathes[i];
                        path.Insert(0, graph.origin);
                        pathes.Add(path);
                    }
                }
            }
            return pathes;
        }
    }
    public class PathFinderSystem : ModSystem
    {
        public override void OnWorldLoad()
        {
            Nodes = new Dictionary<Point, Node>[Main.maxTilesX / squareSide + 1, Main.maxTilesY / squareSide + 1];
        }
        public override void OnWorldUnload()
        {
            Nodes = new Dictionary<Point, Node>[0, 0];
        }
        public const int squareSide = 25;
        public struct Node
        {
            public Point point;
            public Point offset;
            public Point sizeMin;
            public bool minHalfBlock;
            public Point sizeMax;
            public bool maxHalfBlock;
            public bool existToHalfBlock;
            public bool existToNotHalfBlock;
            public List<Link> links;
            public Node()
            {
                sizeMin = new Point(1, 1);
                minHalfBlock = true;
                links = [];
            }
            public bool? Check(Point size, bool halfBlock)
            {
                if (!((halfBlock && existToHalfBlock) || (!halfBlock && existToNotHalfBlock)))
                {
                    return false;
                }
                if (size.X <= sizeMin.X && size.Y <= sizeMin.Y && (halfBlock || !minHalfBlock))
                {
                    return true;    
                }
                if (sizeMax != Point.Zero && (size.X >= sizeMax.X || size.X > sizeMax.Y || (size.Y == sizeMax.Y && (!halfBlock || maxHalfBlock))))
                {
                    return false;
                }
                return null;
            }
            public bool? CheckOnMap(bool[,] map, Point coordinates, Point size, bool halfBlock)
            {
                int w = map.GetLength(0);
                int h = map.GetLength(1);
                if (offset == Point.Zero)
                {
                    if (coordinates.X < w - 1 && coordinates.Y < h - 1)
                    {
                        if (!map[coordinates.X + 1, coordinates.Y] && !map[coordinates.X, coordinates.Y + 1])
                        {   
                            sizeMin = size;
                            minHalfBlock = halfBlock; 
                            return true;
                        }
                        else
                        {
                            //Dust.NewDust(point.ToWorldCoordinates(), 0, 0, DustID.Torch);
                            sizeMax = size;
                            maxHalfBlock = halfBlock; 
                            return false;
                        }
                    }
                    return null;
                }
                else if (offset == new Point(0, 1))
                {
                    if (coordinates.X > 0 && coordinates.Y < h - 1)
                    {
                        if (!map[coordinates.X + 1, coordinates.Y] && !map[coordinates.X, coordinates.Y - 1])
                        {   
                            sizeMin = size;
                            minHalfBlock = halfBlock; 
                            return true;
                        }
                        else
                        {
                            //Dust.NewDust(point.ToWorldCoordinates(), 0, 0, DustID.Torch);
                            sizeMax = size;
                            maxHalfBlock = halfBlock; 
                            return false;
                        }
                    }
                    return null;
                }
                else if (offset == new Point(1, 0))
                {
                    if (coordinates.X < h - 1 && coordinates.Y > 0)
                    {
                        if (!map[coordinates.X - 1, coordinates.Y] && !map[coordinates.X, coordinates.Y + 1])
                        {   
                            sizeMin = size;
                            minHalfBlock = halfBlock; 
                            return true;
                        }
                        else
                        {
                            //Dust.NewDust(point.ToWorldCoordinates(), 0, 0, DustID.Torch);
                            sizeMax = size;
                            maxHalfBlock = halfBlock; 
                            return false;
                        }
                    }
                    return null;
                }
                else if (offset == new Point(1, 1))
                {
                    if (coordinates.X > 0 && coordinates.Y > 0)
                    {
                        if (!map[coordinates.X - 1, coordinates.Y] && !map[coordinates.X, coordinates.Y - 1])
                        {   
                            sizeMin = size;
                            minHalfBlock = halfBlock; 
                            return true;
                        }
                        else
                        {
                            //Dust.NewDust(point.ToWorldCoordinates(), 0, 0, DustID.Torch);
                            sizeMax = size;
                            maxHalfBlock = halfBlock; 
                            return false;
                        }
                    }
                    return null;
                }
                return null;
            }
            public Point ApplyOffset(Point Size)
            {
                Size.X -= 1;
                Size.Y -= 1;
                return point + offset * Size;
            }
            public bool TryGetLink(Point point, out Link link, out int i)
            {
                i = -1;
                link = new Link();
                lock (links)
                {
                    for (int j = 0; j < links.Count; j++)
                    {
                        if (links[j].point == point)
                        {
                            link = links[j];
                            i = j;
                            return true;
                        }
                    }
                }
                return false;
            }
        }
        public struct Link
        {
            public Point point;
            public Point offset;
            public Point sizeMin;
            public bool minHalfBlock;
            
            public Point sizeMax;
            public bool maxHalfBlock;
            public bool existToHalfBlock;
            public bool existToNotHalfBlock;
            public Link()
            {
                sizeMin = new Point(1, 1);
                minHalfBlock = true;
            }
            public Point ApplyOffset(Point Size)
            {
                Size.X -= 1;
                Size.Y -= 1;
                if (offset != Point.Zero)
                {
                }
                return point + offset * Size;
            }
            public bool? Check(Point size, bool halfBlock)
            {
                if (!((halfBlock && existToHalfBlock) || (!halfBlock && existToNotHalfBlock)))
                {
                    return false;
                }
                if (size.X <= sizeMin.X && size.Y <= sizeMin.Y && (halfBlock || !minHalfBlock))
                {
                    return true;    
                }
                if (sizeMax != Point.Zero && (size.X >= sizeMax.X || size.X > sizeMax.Y || (size.Y == sizeMax.Y && (!halfBlock || maxHalfBlock))))
                {
                    return false;
                }
                return null;
            }
        }
        public static Dictionary<Point, Node>[,] Nodes;
        public static void SetNode(Node node)
        {
            int x = node.point.X / squareSide;
            int y = node.point.Y / squareSide;
            lock (Nodes[x, y])
            {
                Nodes[x, y][node.point] = node;
            }
        }
        public static Node? TryGetNode(Point point)
        {
            if (Nodes[point.X / squareSide, point.Y / squareSide].TryGetValue(point, out Node node))
            {
                return node;
            }
            return null;
        }
        public static bool TryGetNode(Point point, out Node node)
        {
            node = new Node();
            if (Nodes[point.X / squareSide, point.Y / squareSide] != null && Nodes[point.X / squareSide, point.Y / squareSide].TryGetValue(point, out Node _node))
            {
                node = _node;
                return true;
            }
            return false;
        }
        public static List<Node> GetArea(Rectangle Area)
        {
            List<Node> result = [];
            for (int x = Area.X / squareSide; x <= Area.Right / squareSide; x++)
            {
                for (int y = Area.Y / squareSide; y <= Area.Bottom / squareSide; y++)
                {   
                    UnloadArea(x, y);
                    if (Nodes[x, y] == null)
                    {
                        LoadArea(x, y);
                    }
                    foreach(var node in Nodes[x, y])
                    {
                        if (Area.Include(node.Value.point))
                        {
                            result.Add(node.Value);
                        }
                    }
                }
            }
            return result;
        }
        public static void LoadArea(int x, int y)
        {
            Rectangle area = new Rectangle(x * squareSide - 1, y * squareSide - 1, squareSide + 1, squareSide + 1);

            bool[,] map = GetMap(area, 1, 1, true);
            bool[,] map1 = GetMap(area, 1, 1, false);
            List<Node> nodes = [];
            List<Node> nodes1 = [];
            List<Node> nodes2 = [];
            nodes.AddRange(GetNodes(map, area));
            nodes1.AddRange(GetNodes(map1, area));
            for (int i = 0; i < nodes1.Count; i++)
            {
                if (Contains(nodes, nodes1[i], out int index))
                {
                    nodes.RemoveAt(index);
                    nodes2.Add(nodes1[i]);
                    nodes1.RemoveAt(i);
                    i--;
                }
            }
            bool Contains(List<Node> items, Node item, out int index)
            {
                index = 0;
                for (; index < items.Count; index++)
                {
                    if (items[index].point == item.point)
                    {
                        return true;
                    }
                }
                return false;
            }
            List<Node> nodes3 = [];

            foreach (var node in nodes)
            {
                Node _node = node;
                _node.existToNotHalfBlock = true;
                nodes3.Add(_node);
            }
            foreach (var node in nodes1)
            {
                Node _node = node;
                _node.existToHalfBlock = true;
                nodes3.Add(_node);
            }
            foreach (var node in nodes2)
            {
                Node _node = node;
                _node.existToNotHalfBlock = true;
                _node.existToHalfBlock = true;
                nodes3.Add(_node);
            }

            if (nodes3.Count == 0)
            {
                Nodes[x, y] = [];
                return;
            }
            List<Node> nodes4 = [];
            int x1 = area.X + 1;
            int x2 = area.Right - 1;
            int y1 = area.Y + 1;
            int y2 = area.Bottom - 1;
            int w = Nodes.GetLength(0);
            int h = Nodes.GetLength(1);
            if (x > 0 && Nodes[x - 1, y] != null)
            {
                x1 = (x - 1) * squareSide;
                foreach (var node in Nodes[x - 1, y])
                {
                    nodes4.Add(node.Value);
                }
            }
            if (x > 0 && y > 0 && Nodes[x - 1, y - 1] != null)
            {
                x1 = (x - 1) * squareSide;
                y1 = (y - 1) * squareSide;
                foreach (var node in Nodes[x - 1, y - 1])
                {
                    nodes4.Add(node.Value);
                }
            }
            if (y > 0 && Nodes[x, y - 1] != null)
            {
                y1 = (y - 1) * squareSide;
                foreach (var node in Nodes[x, y - 1])
                {
                    nodes4.Add(node.Value);
                }
            }
            if (x < w && y > 0 && Nodes[x + 1, y - 1] != null)
            {
                y1 = (y - 1) * squareSide;
                x2 = (x + 2) * squareSide - 1;
                foreach (var node in Nodes[x + 1, y - 1])
                {
                    nodes4.Add(node.Value);
                }
            }
            if (x < w && Nodes[x + 1, y] != null)
            {
                x2 = (x + 2) * squareSide - 1;
                foreach (var node in Nodes[x + 1, y])
                {
                    nodes4.Add(node.Value);
                }
            }
            if (x < w && y < h && Nodes[x + 1, y + 1] != null)
            {
                y2 = (y + 2) * squareSide - 1;
                x2 = (x + 2) * squareSide - 1;
                foreach (var node in Nodes[x + 1, y + 1])
                {
                    nodes4.Add(node.Value);
                }
            }
            if (y < h && Nodes[x, y + 1] != null)
            {
                y2 = (y + 2) * squareSide - 1;
                foreach (var node in Nodes[x, y + 1])
                {
                    nodes4.Add(node.Value);
                }
            }
            if (x > 0 && y < h && Nodes[x - 1, y + 1] != null)
            {
                x1 = (x - 1) * squareSide;
                y2 = (y + 2) * squareSide - 1;
                foreach (var node in Nodes[x - 1, y + 1])
                {
                    nodes4.Add(node.Value);
                }
            }
            Node[] nodes5 = nodes4.ToArray();
            Rectangle newArea = new Rectangle(x1, y1, x2 - x1, y2 - y1);
            bool[,] map2 = GetMap(newArea, 1, 1, true);

            Nodes[x,y] = [];
            if (nodes3.Count * nodes4.Count > 50)
            {
                FastParallel.For(0, nodes3.Count, delegate(int start, int end, object context)
                {
                    for (int i = start; i < end; i++)
                    {
                        Node node1 = nodes3[i];
                        for(int j = 0; j < nodes4.Count; j++)
                        {
                            if ((nodes5[j].existToHalfBlock && node1.existToHalfBlock) || (nodes5[j].existToNotHalfBlock && node1.existToNotHalfBlock))
                            {
                                if (CheckLink(node1.point - newArea.Location, map2, nodes5[j].point - newArea.Location))
                                {
                                    Node node = nodes5[j];
                                    node1.links.Add(new Link() { point = node.point, offset = node.offset, existToHalfBlock = node.existToHalfBlock && node1.existToHalfBlock, existToNotHalfBlock = node.existToNotHalfBlock && node1.existToNotHalfBlock});
                                    AddLink(j, new Link() { point = node1.point, offset = node1.offset, existToHalfBlock = node.existToHalfBlock && node1.existToHalfBlock, existToNotHalfBlock = node.existToNotHalfBlock && node1.existToNotHalfBlock});
                                    //SetNode(node);
                                }
                            }
                        }
                        foreach(var node in nodes3)
                        {
                            if ((node.existToHalfBlock && node1.existToHalfBlock) || (node.existToNotHalfBlock && node1.existToNotHalfBlock))
                            {
                                if (CheckLink(node1.point - newArea.Location, map2, node.point - newArea.Location))
                                {
                                    node1.links.Add(new Link() { point = node.point, offset = node.offset, existToHalfBlock = node.existToHalfBlock && node1.existToHalfBlock, existToNotHalfBlock = node.existToNotHalfBlock && node1.existToNotHalfBlock});
                                    //SetNode(node);
                                }
                            }
                        }
                        SetNode(node1);
                    }
                });
                void AddLink(int i, Link link)
                {
                    lock (nodes5[i].links)
                    {
                        nodes5[i].links.Add(link);
                    }
                }
            }
            else
            {
                for (int i = 0; i < nodes3.Count; i++)
                {
                    Node node1 = nodes3[i];
                    for(int j = 0; j < nodes5.Length; j++)
                    {
                        Node node = nodes5[j]; 
                        if ((node.existToHalfBlock && node1.existToHalfBlock) || (node.existToNotHalfBlock && node1.existToNotHalfBlock))
                        {
                            if (CheckLink(node1.point - newArea.Location, map2, node.point - newArea.Location))
                            {
                                node1.links.Add(new Link() { point = node.point, offset = node.offset, existToHalfBlock = node.existToHalfBlock && node1.existToHalfBlock, existToNotHalfBlock = node.existToNotHalfBlock && node1.existToNotHalfBlock});
                                nodes5[j].links.Add(new Link() { point = node1.point, offset = node1.offset, existToHalfBlock = node.existToHalfBlock && node1.existToHalfBlock, existToNotHalfBlock = node.existToNotHalfBlock && node1.existToNotHalfBlock});
                                SetNode(node);
                            }
                        }
                    }
                    for(int j = i + 1; j < nodes3.Count; j++)
                    {
                        var node = nodes3[j];
                        if ((node.existToHalfBlock && node1.existToHalfBlock) || (node.existToNotHalfBlock && node1.existToNotHalfBlock))
                        {
                            if (CheckLink(node1.point - newArea.Location, map2, node.point - newArea.Location))
                            {
                                node1.links.Add(new Link() { point = node.point, offset = node.offset, existToHalfBlock = node.existToHalfBlock && node1.existToHalfBlock, existToNotHalfBlock = node.existToNotHalfBlock && node1.existToNotHalfBlock});
                                nodes3[j].links.Add(new Link() { point = node1.point, offset = node1.offset, existToHalfBlock = node.existToHalfBlock && node1.existToHalfBlock, existToNotHalfBlock = node.existToNotHalfBlock && node1.existToNotHalfBlock});
                                //SetNode(node);
                            }
                        }
                    }
                    SetNode(node1);
                }
            }
            foreach(var node in nodes5)
            {
                SetNode(node);
            }
        }
        public static void UnloadArea(int x, int y)
        {
            Dictionary<Point, Node> nodes = Nodes[x, y];
            if (nodes == null)
            {
                return;
            }
            foreach (var node in nodes)
            {
                foreach (var link in node.Value.links)
                {
                    if (TryGetNode(link.point, out Node node1))
                    {
                        if (node1.TryGetLink(node.Value.point, out _, out int i))
                        {
                            node1.links.RemoveAt(i);
                        }
                    }
                }
            }
            Nodes[x, y] = null;
        }
        private static bool CheckLink(Point origin, bool[,] map, Point node)
        {
            if (origin == node)
            {
                return false;
            }
            if (node.X < 0 || node.X >= map.GetLength(0) || node.Y < 0 || node.Y >= map.GetLength(1))
            {
                return false;
            }
            if (origin.X < 0 || origin.X >= map.GetLength(0) || origin.Y < 0 || origin.Y >= map.GetLength(1))
            {
                return false;
            }
            Point left;
            Point right;
            if(origin.X - node.X > 0)
            {
                left = node;
                right = origin;
            }
            else
            {
                right = node;
                left = origin;
            }
            int w = right.X - left.X;
            int h = right.Y - left.Y;
            if  (w + Math.Abs(h) > 25)
            {
                return false;
            }
            if (w != 0)
            {
                float k = (float)h / (w + 1);
                float y = 0.5f + left.Y;
                for (int x = left.X; x <= right.X; x++)
                {
                    int oldY = (int)y;
                    y += k;
                    int ymin = Math.Min(oldY, (int)y);
                    int ymax = Math.Max(oldY, (int)y) + 1;
                    for (int _y = ymin; _y < ymax; _y++)
                    {
                        if (map[x, _y])
                        {
                            return false;
                        }
                    }
                }
            }
            else
            {
                int ymin = Math.Min(left.Y, right.Y);
                int ymax = Math.Min(left.Y, right.Y) + 1;
                for (int y = ymin; y < ymax; y++)
                {
                    if (map[left.X, y])
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public static bool[,] GetMap(Rectangle area, int width, int height, bool halfBlocks)
        {
            int endX = area.Right + 1;
            int endY = area.Bottom + 1;
            bool[,] map = new bool[area.Width + 1, area.Height + 1];
            FastParallel.For(area.X, endX, delegate(int start, int end, object context)
            {
                for (int X = start; X < end; X++)
                {
                    for (int Y = area.Y; Y < endY; Y++)
                    {
                        if (Main.tile[X, Y].IsSolid())
                        {
                            int _y = 1;
                            if (!halfBlocks || !Main.tile[X, Y].IsHalfBlock)
                            {
                                map[X - area.X, Y - area.Y] = true;
                                _y = 0;
                            }
                            int y = 1;
                            for (int x = 0; x < width && X + x < endX; x++)
                            {
                                for (; y < height && Y + y < endY; y++)
                                {
                                    map[X - area.X + x, Y - area.Y + y] = true;
                                }
                                y = _y;
                            }
                        }
                    }
                }
            });
            return map;
        }
        public static List<Node> GetNodes(bool[,] map, Rectangle Area)
        {
            List<Node> nodes = new List<Node>();
            int w = map.GetLength(0);
            int h = map.GetLength(1);
            FastParallel.For(1, map.GetLength(0) - 1, delegate (int start, int end, object context)
            {
                for (int x = start; x < end; x++)
                {
                    for (int y = 1; y < map.GetLength(1) - 1; y++)
                    {
                        if (!map[x, y])
                        {
                            if(map[x + 1, y + 1] && !map[x + 1, y] && !map[x, y + 1])
                            {
                                Add(new Node() { point = new Point(x, y) + Area.Location, offset = Point.Zero });
                            }
                            else if(map[x + 1, y - 1] && !map[x + 1, y] && !map[x, y - 1])
                            {
                                Add(new Node() { point = new Point(x, y) + Area.Location, offset = new Point(0, 1) });
                            }
                            else if(map[x - 1, y + 1] && !map[x - 1, y] && !map[x, y + 1])
                            { 
                                Add(new Node() { point = new Point(x, y) + Area.Location, offset = new Point(1, 0) });
                            }
                            else if(map[x - 1, y - 1] && !map[x - 1, y] && !map[x, y - 1])
                            {
                                Add(new Node() { point = new Point(x, y) + Area.Location, offset = new Point(1, 1) });
                            }
                        }
                    }
                }
            });
            void Add(Node node)
            {
                lock (nodes)
                {
                    nodes.Add(node);
                }
            }
            return nodes;
        }
    }
}