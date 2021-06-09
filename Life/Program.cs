using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace cli_life
{
    public class Cell
    {
        public bool IsAlive;
        public int combination_s = 0;
        public readonly List<Cell> neighbors = new List<Cell>();
        public bool IsAliveNext;
        public Cell()
        {
        }
        public Cell(int val)
        {
            if (val == 1) IsAlive = true;
            else IsAlive = false;
        }
          public void Advance()
        {
            IsAlive = IsAliveNext;
        }
        public void DetermineNextLiveState()
        {
            int Neighbor_s = neighbors.Where(x => x.IsAlive).Count();
            if (IsAlive)
                IsAliveNext = Neighbor_s == 2 || Neighbor_s == 3;
            else
                IsAliveNext = Neighbor_s == 3;
        }
    }
    public class Board
    {
        public Cell[,] Cells;
        public int CellSize;
        public int F = 1;
        public int live_count = 0;
        public int Columns { get { return Cells.GetLength(0); } }
        public int Rows { get { return Cells.GetLength(1); } }
        public int Width { get { return Columns * CellSize; } }
        public int Height { get { return Rows * CellSize; } }
         readonly Random rand = new Random();
        public void Randomize(double Life)
        {
            foreach (var cell in Cells)
                cell.IsAlive = rand.NextDouble() < Life;
        }
        public void Advance()
        {
            live_count = 0;
            foreach (var cell in Cells)
                cell.DetermineNextLiveState();
            foreach (var cell in Cells)
            {
                if (cell.IsAlive != cell.IsAliveNext) F = 1;
                cell.Advance();
                if (cell.IsAlive) live_count++;
            }
        }
        public Board(int value, int width, int height, int cellSize, double Life = .1)
        {
            CellSize = cellSize;
            if (value == 0)
            {
                Cells = new Cell[width / cellSize, height / cellSize];
                for (int x = 0; x < Columns; x++)
                    for (int y = 0; y < Rows; y++)
                        Cells[x, y] = new Cell();
                ConnectNeighbors();
                Randomize(Life);
            }
            else
            {
                using (StreamReader r = new StreamReader("test.txt"))
                {
                    List<string> matr = new List<string>();
                    string curstr;
                    int max = 0;
                    while ((curstr = r.ReadLine()) != null)
                    {
                        matr.Add(curstr);
                        if (curstr.Length > max) max = curstr.Length;
                    }
                    if (max == 0) return;
                    Cells = new Cell[max / cellSize, matr.Count/ cellSize];
                    for (int x = 0; x < Columns; x++)
                        for (int y = 0; y < Rows; y++)
                        {
                            if (matr[y].Length <= x)
                                Cells[x, y] = new Cell(0);
                            else if (matr[y][x] == '*') Cells[x, y] = new Cell(1);
                            else Cells[x, y] = new Cell(0);
                        }
                    ConnectNeighbors();
                }
            }
        }
       
        private void ConnectNeighbors()
        {
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    int xL = (x > 0) ? x - 1 : Columns - 1;
                    int xR = (x < Columns - 1) ? x + 1 : 0;
                    int yT = (y > 0) ? y - 1 : Rows - 1;
                    int yB = (y < Rows - 1) ? y + 1 : 0;
                    Cells[x, y].neighbors.Add(Cells[xL, yT]);
                    Cells[x, y].neighbors.Add(Cells[x, yT]);
                    Cells[x, y].neighbors.Add(Cells[xR, yT]);
                    Cells[x, y].neighbors.Add(Cells[xL, y]);
                    Cells[x, y].neighbors.Add(Cells[xR, y]);
                    Cells[x, y].neighbors.Add(Cells[xL, yB]);
                    Cells[x, y].neighbors.Add(Cells[x, yB]);
                    Cells[x, y].neighbors.Add(Cells[xR, yB]);
                }
            }
        }
    }
    class Program
    {
        public class Item
        {
            public int width;
            public int height;
            public int cellSize;
            public double Life;
        }
        static Board board;
        static private void Reset(int val)
        {
            using (StreamReader r = new StreamReader("json_file.json"))
            {
                Item items = new Item();
                string str = r.ReadToEnd();
                string tmp_str = "";
                int i = 0;
                while (str[i] != ':') i++;
                i++;
                while (str[i] != ',')
                {
                    tmp_str = tmp_str + str[i];
                    i++;
                }
                items.width = Int32.Parse(tmp_str);
                while (str[i] != ':') i++;
                i++;
                tmp_str = "";
                while (str[i] != ',')
                {
                    tmp_str = tmp_str + str[i];
                    i++;
                }
                items.height = Int32.Parse(tmp_str);
                while (str[i] != ':') i++;
                i++;
                tmp_str = "";
                while (str[i] != ',')
                {
                    tmp_str = tmp_str + str[i];
                    i++;
                }
                items.cellSize = Int32.Parse(tmp_str);
                while (str[i] != ':') i++;
                i++;
                tmp_str = "";
                while (str[i] != '.')
                {
                    tmp_str = tmp_str + str[i];
                    i++;
                }
                i++;
                items.Life = Int32.Parse(tmp_str);
                tmp_str = "";
                while (str[i] != '\r')
                {
                    tmp_str = tmp_str + str[i];
                    i++;
                }
                int st = 1;
                for (int j = 0; j < tmp_str.Length; j++) st = st * 10;
                items.Life = items.Life + (float)Int32.Parse(tmp_str) / st;
                board = new Board(
                    val,
                    width: items.width,
                    height: items.height,
                    cellSize: items.cellSize,
                    Life: items.Life);
            }
        }
        static void Render()
        {
            using (StreamWriter r = new StreamWriter("test.txt"))
            {
                for (int row = 0; row < board.Rows; row++)
                {
                    for (int col = 0; col < board.Columns; col++)
                    {
                        var cell = board.Cells[col, row];
                        if (cell.IsAlive)
                        {
                            Console.Write('*');
                            r.Write('*');
                        }
                        else
                        {
                            Console.Write(' ');
                            r.Write(' ');
                        }
                    }
                    Console.Write('\n');
                    r.Write('\n');
                }
            }
        }
        static void Main(string[] args)
        {
            bool eq(int[,] arr1, int[,] arr2)
            {
                if (arr1.Length != arr2.Length) return false;
                int F = 1;
                for (int i = 0; i < arr1.Length / 2; i++) 
                {
                    int F2 = 0;
                    for (int j = 0; j < arr1.Length / 2; j++)
                    {
                        if (arr1[i, 0] == arr2[j, 0] && arr1[i, 1] == arr2[j, 1]) 
                        {
                            F2 = 1;
                            break;
                        }
                    }
                    if (F2 == 0)
                    {
                        F = 0;
                        break;
                    }
                }
                if (F == 0) return false;
                else return true;
            }
            int[,] sort(int[,] arr)
            {
                int F1 = 1;
                int F2 = 1;
                int F3 = 1;
                int F4 = 1;
                int minX = 10000;
                int minY = 10000;
                for (int i = 0; i < arr.Length / 2; i++) 
                {
                    if (arr[i, 0] == 0) F1 = 0;
                    if (arr[i, 0] == board.Columns - 1) F2 = 0;
                    if (arr[i, 0] < minX) minX = arr[i, 0];
                    if (arr[i, 1] == 0) F3 = 0;
                    if (arr[i, 1] == board.Rows - 1) F4 = 0;
                    if (arr[i, 1] < minY) minY = arr[i, 1];
                }
                if (F1 == 0 && F2 == 0)
                {
                    for (int i = 0; i < board.Columns; i++)
                    {
                        int F = 0;
                        for (int j = 0; j < arr.Length / 2; j++) 
                            if (arr[j, 0] == board.Columns - i - 1) 
                            {
                                F = 1;
                                break;
                            }
                        if (F == 0) 
                        {
                            for (int j = 0; j < arr.Length / 2; j++) 
                            {
                                if (arr[j, 0] < board.Columns - i - 1) arr[j, 0] = arr[j, 0] + i;
                                else arr[j, 0] = arr[j, 0] - board.Columns + i;
                            }
                            break;
                        }
                    }
                }
                if (F3 == 0 && F4 == 0)
                {
                    for (int i = 0; i < board.Columns; i++)
                    {
                        int F = 0;
                        for (int j = 0; j < arr.Length / 2; j++) 
                            if (arr[j, 1] == board.Rows - i - 1) 
                            {
                                F = 1;
                                break;
                            }
                        if (F == 0)
                        {
                            for (int j = 0; j < arr.Length / 2; j++) 
                            {
                                if (arr[j, 1] < board.Rows - i - 1) arr[j, 1] = arr[j, 1] + i;
                                else arr[j, 1] = arr[j, 1] - board.Rows + i;
                            }
                            break;
                        }
                    }
                }
                if (minX >= 1)
                    for (int i = 0; i < arr.Length / 2; i++) arr[i, 0] = arr[i, 0] - minX + 1;
                else for (int i = 0; i < arr.Length / 2; i++) arr[i, 0]++;
                if (minY >= 1)
                    for (int i = 0; i < arr.Length / 2; i++) arr[i, 1] = arr[i, 1] - minY + 1;
                else for (int i = 0; i < arr.Length / 2; i++) arr[i, 1]++;
                for (int i = 0; i < arr.Length / 2; i++) Console.WriteLine(arr[i, 0].ToString() + ' ' + arr[i, 1].ToString());
                Console.WriteLine("\n");
                return arr;
            }
            float count = 0;
            Reset(0);
            while (board.F == 1)
            {
                count++;
                board.F = 0;
                Console.Clear();
                Render();
                board.Advance();
                Thread.Sleep(1000);
                if (count > 1000) break;
            }
            Reset(1);
            while (board.F == 1)
            {
                count++;
                board.F = 0;
                Console.Clear();
                Render();
                board.Advance();
                Thread.Sleep(1000);
                if (count > 1000) break;
            }
            int n = 0;
            using (StreamReader r = new StreamReader("tres.txt"))
            {
                float k = float.Parse(r.ReadLine());
                n = Int32.Parse(r.ReadLine());
                count += k;
            }
            n++;
            using (StreamWriter r = new StreamWriter("tres.txt"))
            {
                r.Write(count/n);
                r.Write('\n');
                r.Write(n);
            }
            int combination_s = 0;
            for (int x = 0; x < board.Columns; x++)
                for (int y = 0; y < board.Rows; y++)
                {
                    if (board.Cells[x,y].IsAlive)
                    {
                        int min = 100000;
                        int F2 = 0;
                        for (int i = 0; i < 8; i++)
                            if (board.Cells[x, y].neighbors[i].combination_s > 0 && board.Cells[x, y].neighbors[i].combination_s < min)
                            {
                                F2 = 1;
                                min = board.Cells[x, y].neighbors[i].combination_s;
                            }
                        if (F2 == 0)
                        {
                            combination_s++;
                            board.Cells[x, y].combination_s = combination_s;
                        }
                        else board.Cells[x, y].combination_s = min;
                    }
                }
            int[,] hive = new int[6,2];
            int hive_count = 0;
            hive[0, 0] = 2; hive[0, 1] = 1; hive[1, 0] = 1; hive[1, 1] = 2; hive[2, 0] = 3; hive[2, 1] = 2; hive[3, 0] = 1; hive[3, 1] = 3; hive[4, 0] = 3; hive[4, 1] = 3; hive[5, 0] = 2; hive[5, 1] = 4;
            int[,] loaf = new int[7, 2];
            int loaf_count = 0;
            loaf[0, 0] = 2; loaf[0, 1] = 1; loaf[1, 0] = 3; loaf[1, 1] = 1; loaf[2, 0] = 1; loaf[2, 1] = 2; loaf[3, 0] = 4; loaf[3, 1] = 2; loaf[4, 0] = 2; loaf[4, 1] = 3; loaf[5, 0] = 4; loaf[5, 1] = 3; loaf[6, 0] = 3; loaf[6, 1] = 4;
            int[,] pool = new int[8, 2];
            int pool_count = 0;
            pool[0, 0] = 2; pool[0, 1] = 1; pool[1, 0] = 3; pool[1, 1] = 1; pool[2, 0] = 1; pool[2, 1] = 2; pool[3, 0] = 4; pool[3, 1] = 2; pool[4, 0] = 1; pool[4, 1] = 3; pool[5, 0] = 4; pool[5, 1] = 3; pool[6, 0] = 2; pool[6, 1] = 4; pool[7, 0] = 3; pool[7, 1] = 4;
            int[,] box = new int[4, 2];
            int box_count = 0;
            box[0, 0] = 2; box[0, 1] = 1; box[1, 0] = 1; box[1, 1] = 2; box[2, 0] = 3; box[2, 1] = 2; box[3, 0] = 2; box[3, 1] = 3;
            int[,] block = new int[4, 2];
            int block_count = 0;
            block[0, 0] = 1; block[0, 1] = 1; block[1, 0] = 2; block[1, 1] = 1; block[2, 0] = 1; block[2, 1] = 2; block[3, 0] = 2; block[3, 1] = 2;
            int[,] snake = new int[6, 2];
            int snake_count = 0;
            snake[0, 0] = 1; snake[0, 1] = 1; snake[1, 0] = 3; snake[1, 1] = 1; snake[2, 0] = 4; snake[2, 1] = 1; snake[3, 0] = 1; snake[3, 1] = 2; snake[4, 0] = 2; snake[4, 1] = 2; snake[5, 0] = 4; snake[5, 1] = 2;
            int[,] barge = new int[6, 2];
            int barge_count = 0;
            barge[0, 0] = 2; barge[0, 1] = 1; barge[1, 0] = 1; barge[1, 1] = 2; barge[2, 0] = 3; barge[2, 1] = 2; barge[3, 0] = 2; barge[3, 1] = 3; barge[4, 0] = 4; barge[4, 1] = 3; barge[5, 0] = 3; barge[5, 1] = 4;
            int[,] boat = new int[5, 2];
            int boat_count = 0;
            boat[0, 0] = 2; boat[0, 1] = 1; boat[1, 0] = 1; boat[1, 1] = 2; boat[2, 0] = 3; boat[2, 1] = 2; boat[3, 0] = 2; boat[3, 1] = 3; boat[4, 0] = 3; barge[4, 1] = 3;
            int[,] ship = new int[6, 2];
            int ship_count = 0;
            ship[0, 0] = 1; ship[0, 1] = 1; ship[1, 0] = 2; ship[1, 1] = 1; ship[2, 0] = 1; ship[2, 1] = 2; ship[3, 0] = 3; ship[3, 1] = 2; ship[4, 0] = 2; ship[4, 1] = 3; ship[5, 0] = 3; ship[5, 1] = 3;
            int unknown_count = 0;
            for (int i = 0; i < combination_s; i++)
            {
                List<int> arr1 = new List<int>();
                List<int> arr2 = new List<int>();
                for (int x = 0; x < board.Columns; x++)
                    for (int y = 0; y < board.Rows; y++)
                    {
                        if (board.Cells[x, y].combination_s == i + 1) 
                        {
                            arr1.Add(x);
                            arr2.Add(y);
                        }
                    }
                int[,] arr = new int[arr1.Count, 2];
                for (int j = 0; j < arr1.Count; j++)
                {
                    arr[j, 0] = arr1[j];
                    arr[j, 1] = arr2[j];
                }
                arr = sort(arr);
                if (eq(arr, hive)) hive_count++;
                else if (eq(arr, loaf)) loaf_count++;
                else if (eq(arr, pool)) pool_count++;
                else if (eq(arr, box)) box_count++;
                else if (eq(arr, block)) block_count++;
                else if (eq(arr, snake)) snake_count++;
                else if (eq(arr, barge)) barge_count++;
                else if (eq(arr, boat)) boat_count++;
                else if (eq(arr, ship)) ship_count++;
                else unknown_count++;
            }
            Console.WriteLine("\nSurvivors: " + board.live_count.ToString());
            Console.WriteLine("\nCombinations: " + combination_s.ToString());
            Console.WriteLine("\nHives: " + hive_count.ToString());
            Console.WriteLine("\nLoafs: " + loaf_count.ToString());
            Console.WriteLine("\nPools: " + pool_count.ToString());
            Console.WriteLine("\nBoxes: " + box_count.ToString());
            Console.WriteLine("\nBlocks: " + block_count.ToString());
            Console.WriteLine("\nSnakes: " + snake_count.ToString());
            Console.WriteLine("\nBarges: " + barge_count.ToString());
            Console.WriteLine("\nBoats: " + boat_count.ToString());
            Console.WriteLine("\nShips: " + ship_count.ToString());
            Console.WriteLine("\nUnknown: " + unknown_count.ToString());
            Console.WriteLine("\nAverage generation: " + (count/n).ToString());
            Console.ReadKey();
        }
    }
}