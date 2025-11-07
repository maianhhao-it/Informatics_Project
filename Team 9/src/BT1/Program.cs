using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WarehousePathfinding
{
    public class PathResult
    {
        public bool Found { get; set; }
        public int Distance { get; set; }
        public List<(int x, int y)> Path { get; set; } = new List<(int x, int y)>();
    }

    internal class Program
    {
        private static readonly int[] DX = { -1, 1, 0, 0 };
        private static readonly int[] DY = { 0, 0, -1, 1 };

        
        // BFS: Tìm đường ngắn nhất giữa 2 điểm
        
        static PathResult BFS(int[,] map, int startX, int startY, int endX, int endY, int rows, int cols, bool needPath = false)
        {
            var result = new PathResult();
            if (startX < 0 || startX >= rows || startY < 0 || startY >= cols ||
                endX < 0 || endX >= rows || endY < 0 || endY >= cols)
            {
                Console.WriteLine($"Lỗi: Tọa độ nằm ngoài bản đồ! Start({startX},{startY}) End({endX},{endY})");
                return result;
            }

            if (map[startX, startY] == 1 || map[endX, endY] == 1)
            {
                Console.WriteLine($"Lỗi: Điểm start hoặc end nằm trên tường!");
                return result;
            }
            
            if (startX == endX && startY == endY)
            {
                result.Found = true;
                result.Distance = 0;
                result.Path = new List<(int, int)> { (startX, startY) };
                return result;
            }

            var queue = new Queue<(int x, int y)>();
            var visited = new bool[rows, cols];
            var distance = new int[rows, cols];
            var parent = new (int, int)?[rows, cols];

            queue.Enqueue((startX, startY));
            visited[startX, startY] = true;
            distance[startX, startY] = 0;

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                int x = current.x;
                int y = current.y;

                if (x == endX && y == endY)
                {
                    result.Found = true;
                    result.Distance = distance[x, y];
                    if (needPath)
                        result.Path = ReconstructPath(parent, startX, startY, endX, endY);
                    return result;
                }

                for (int dir = 0; dir < 4; dir++)
                {
                    int nx = x + DX[dir];
                    int ny = y + DY[dir];
                    if (nx >= 0 && nx < rows && ny >= 0 && ny < cols &&
                        map[nx, ny] == 0 && !visited[nx, ny])
                    {
                        queue.Enqueue((nx, ny));
                        visited[nx, ny] = true;
                        distance[nx, ny] = distance[x, y] + 1;
                        parent[nx, ny] = (x, y);
                    }
                }
            }

            return result;
        }

        // Truy vết đường đi từ mảng parent
        static List<(int, int)> ReconstructPath((int, int)?[,] parent, int startX, int startY, int endX, int endY)
        {
            var path = new List<(int, int)>();
            var current = (endX, endY);

            //Ngăn vòng lặp vô hạn
            int maxSteps = parent.GetLength(0) * parent.GetLength(1);
            int steps = 0;

            while (current != (startX, startY) && steps < maxSteps)
            {
                path.Add(current);
                var p = parent[current.Item1, current.Item2];
                if (!p.HasValue) break;
                current = p.Value;
                steps++;
            }

            path.Add((startX, startY));
            path.Reverse();
            return path;
        }

        // Đọc bản đồ từ file
        static bool ReadMap(string[] lines, ref int lineIndex, int rows, int cols, out int[,] map)
        {
            map = new int[rows, cols];

            if (lineIndex + rows > lines.Length)
            {
                Console.WriteLine("File thiếu dữ liệu bản đồ!");
                return false;
            }

            for (int i = 0; i < rows; i++)
            {
                var row = lines[lineIndex + i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (row.Length != cols)
                {
                    Console.WriteLine($"Dòng {i + 1} của bản đồ không đủ {cols} cột!");
                    return false;
                }
                for (int j = 0; j < cols; j++)
                {
                    if (!int.TryParse(row[j], out int val) || (val != 0 && val != 1))
                    {
                        Console.WriteLine($"Giá trị tại ({i},{j}) không hợp lệ! Phải là 0 hoặc 1.");
                        return false;
                    }
                    map[i, j] = val;
                }
            }

            return true;
        }

        // In bản đồ GỐC (chưa có đường đi)
        static void PrintOriginalMap(int[,] map, int rows, int cols, (int, int)? start = null, (int, int)? end = null, List<(int, int)> waypoints = null)
        {
            Console.WriteLine("\nBẢN ĐỒ KHO HÀNG GỐC");
            Console.WriteLine("S=Start | E=End | W=Waypoint | #=Wall | .=Free\n");

            var waypointSet = waypoints != null ? new HashSet<(int, int)>(waypoints) : new HashSet<(int, int)>();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (start.HasValue && (i, j) == start.Value) Console.Write("S ");
                    else if (end.HasValue && (i, j) == end.Value) Console.Write("E ");
                    else if (waypointSet.Contains((i, j))) Console.Write("W ");
                    else if (map[i, j] == 1) Console.Write("# ");
                    else Console.Write(". ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        // In bản đồ + đường đi
        static void PrintMap(int[,] map, int rows, int cols, List<(int, int)> path = null,
            (int, int)? start = null, (int, int)? end = null)
        {
            Console.WriteLine("\nBẢN ĐỒ KHO HÀNG SAU KHI TÌM ĐƯỜNG");
            Console.WriteLine("S=Start | E=End | *=Path | #=Wall | .=Free\n");

            var pathSet = path != null ? new HashSet<(int, int)>(path) : new HashSet<(int, int)>();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (start.HasValue && (i, j) == start.Value) Console.Write("S ");
                    else if (end.HasValue && (i, j) == end.Value) Console.Write("E ");
                    else if (pathSet.Contains((i, j))) Console.Write("* ");
                    else if (map[i, j] == 1) Console.Write("# ");
                    else Console.Write(". ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        // MultiPointPath: đi qua nhiều điểm theo thứ tự
        static PathResult MultiPointPath(int[,] map, int rows, int cols, (int x, int y) start, List<(int x, int y)> waypoints, (int x, int y) end)
        {
            var result = new PathResult();
            var fullPath = new List<(int, int)>();
            int totalSteps = 0;

            var points = new List<(int x, int y)> { start };
            points.AddRange(waypoints);
            points.Add(end);

            // Kiểm tra tất cả điểm có hợp lệ không
            foreach (var (x, y) in points)
            {
                if (x < 0 || x >= rows || y < 0 || y >= cols)
                {
                    Console.WriteLine($"Lỗi: Điểm ({x},{y}) nằm ngoài bản đồ!");
                    result.Found = false;
                    return result;
                }
                if (map[x, y] == 1)
                {
                    Console.WriteLine($"Lỗi: Điểm ({x},{y}) nằm trên tường!");
                    result.Found = false;
                    return result;
                }
            }

            for (int i = 0; i < points.Count - 1; i++)
            {
                var from = points[i];
                var to = points[i + 1];

                Console.WriteLine($"Đang tìm đường từ ({from.x},{from.y}) đến ({to.x},{to.y})...");

                var res = BFS(map, from.x, from.y, to.x, to.y, rows, cols, true);
                if (!res.Found)
                {
                    Console.WriteLine($"Không tìm thấy đường từ ({from.x},{from.y}) đến ({to.x},{to.y})!");
                    result.Found = false;
                    return result;
                }

                // Ghép đoạn: bỏ điểm đầu nếu không phải đoạn đầu
                var segment = i == 0 ? res.Path : res.Path.Skip(1).ToList();
                fullPath.AddRange(segment);
                totalSteps += res.Distance;
            }

            result.Found = true;
            result.Path = fullPath;
            result.Distance = totalSteps;
            return result;
        }

       
        // Validate và parse tọa độ
        static bool TryParseCoordinate(string[] parts, out int x, out int y)
        {
            x = y = -1;
            if (parts.Length != 2) return false;
            return int.TryParse(parts[0], out x) && int.TryParse(parts[1], out y);
        }

       
        // Main: đọc file input, xử lý 2 chế độ
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("HỆ THỐNG TÌM ĐƯỜNG KHO HÀNG");

            try
            {
                if (!File.Exists("input.txt"))
                {
                    Console.WriteLine("Không tìm thấy file input.txt!");
                    return;
                }

                var lines = File.ReadAllLines("input.txt")
                    .Where(l => !string.IsNullOrWhiteSpace(l))
                    .ToArray();

                if (lines.Length < 3)
                {
                    Console.WriteLine("File input không đủ dữ liệu!");
                    return;
                }

                var size = lines[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (size.Length != 2 || !int.TryParse(size[0], out int rows) || !int.TryParse(size[1], out int cols))
                {
                    Console.WriteLine("Dòng đầu tiên phải là kích thước bản đồ (rows cols)!");
                    return;
                }

                if (rows <= 0 || cols <= 0 || rows > 1000 || cols > 1000)
                {
                    Console.WriteLine("Kích thước bản đồ không hợp lệ! (phải từ 1-1000)");
                    return;
                }

                int lineIndex = 1;

                // Phát hiện mode dựa vào format dòng tiếp theo
                string modeLine = lines[lineIndex].Trim();
                var testParts = modeLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // Mode 2: dòng có 1 số duy nhất (số waypoints)
                if (testParts.Length == 1 && int.TryParse(testParts[0], out int numWaypoints))
                {
                    lineIndex++;

                    if (numWaypoints < 2 || numWaypoints > 1000)
                    {
                        Console.WriteLine("Số lượng waypoints phải từ 2-1000!");
                        return;
                    }

                    if (lineIndex + numWaypoints >= lines.Length)
                    {
                        Console.WriteLine("File thiếu dữ liệu waypoints!");
                        return;
                    }

                    var waypoints = new List<(int, int)>();
                    for (int i = 0; i < numWaypoints; i++)
                    {
                        var wp = lines[lineIndex++].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (!TryParseCoordinate(wp, out int x, out int y))
                        {
                            Console.WriteLine($"Waypoint thứ {i + 1} có format không hợp lệ!");
                            return;
                        }
                        waypoints.Add((x, y));
                    }

                    // Đọc map
                    if (!ReadMap(lines, ref lineIndex, rows, cols, out int[,] map))
                        return;

                    var start = waypoints[0];
                    var end = waypoints[waypoints.Count - 1];
                    var midWaypoints = waypoints.Skip(1).Take(waypoints.Count - 2).ToList();

                    // IN BẢN ĐỒ GỐC
                    PrintOriginalMap(map, rows, cols, start, end, midWaypoints);

                    var result = MultiPointPath(map, rows, cols, start, midWaypoints, end);

                    Console.WriteLine("\nKẾT QUẢ");
                    if (result.Found)
                    {
                        Console.WriteLine($"✓ Tìm thấy đường đi!");
                        Console.WriteLine($"✓ Tổng quãng đường: {result.Distance} bước");
                        PrintMap(map, rows, cols, result.Path, start, end);

                        var sb = new StringBuilder();
                        sb.AppendLine("ĐƯỜNG ĐI:");
                        foreach (var p in result.Path)
                            sb.AppendLine($"{p.Item1} {p.Item2}");
                        sb.AppendLine($"Tổng quãng đường: {result.Distance}");
                        File.WriteAllText("output.txt", sb.ToString());
                        Console.WriteLine("✓ Đã ghi kết quả vào output.txt");
                    }
                    else
                    {
                        Console.WriteLine("✗ Không tìm thấy đường đi!");
                        File.WriteAllText("output.txt", "Không tìm thấy đường đi.");
                    }
                }
                // Mode 1: dòng có 2 số (start position)
                else if (testParts.Length == 2)
                {
                    if (!TryParseCoordinate(testParts, out int startX, out int startY))
                    {
                        Console.WriteLine("Tọa độ start không hợp lệ!");
                        return;
                    }
                    lineIndex++;

                    var endPos = lines[lineIndex++].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (!TryParseCoordinate(endPos, out int endX, out int endY))
                    {
                        Console.WriteLine("Tọa độ end không hợp lệ!");
                        return;
                    }

                    // Đọc map
                    if (!ReadMap(lines, ref lineIndex, rows, cols, out int[,] map))
                        return;

                    // IN BẢN ĐỒ GỐC
                    PrintOriginalMap(map, rows, cols, (startX, startY), (endX, endY));

                    var result = BFS(map, startX, startY, endX, endY, rows, cols, true);

                    Console.WriteLine("\nKẾT QUẢ");
                    if (result.Found)
                    {
                        Console.WriteLine($"✓ Tìm thấy đường đi!");
                        Console.WriteLine($"✓ Khoảng cách: {result.Distance} bước");
                        PrintMap(map, rows, cols, result.Path, (startX, startY), (endX, endY));

                        var sb = new StringBuilder();
                        sb.AppendLine("ĐƯỜNG ĐI:");
                        foreach (var p in result.Path)
                            sb.AppendLine($"{p.Item1} {p.Item2}");
                        sb.AppendLine($"Tổng quãng đường: {result.Distance}");
                        File.WriteAllText("output.txt", sb.ToString());
                        Console.WriteLine("✓ Đã ghi kết quả vào output.txt");
                    }
                    else
                    {
                        Console.WriteLine("✗ Không tìm thấy đường đi!");
                        File.WriteAllText("output.txt", "Không tìm thấy đường đi.");
                    }
                }
                else
                {
                    Console.WriteLine("Format input không hợp lệ!");
                    Console.WriteLine("Mode 1: Dòng 2 phải là start (2 số), dòng 3 là end (2 số)");
                    Console.WriteLine("Mode 2: Dòng 2 phải là số waypoints (1 số)");
                    File.WriteAllText("output.txt", "Format input không hợp lệ!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                File.WriteAllText("output.txt", $"Lỗi: {ex.Message}");
            }

            Console.WriteLine("\nNhấn Enter để thoát...");
            Console.ReadLine();
        }
    }
}