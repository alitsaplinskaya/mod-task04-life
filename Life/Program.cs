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
        public int perest = 0;
        public bool IsAlive;
        public readonly List<Cell> neighbors = new List<Cell>();
        private bool IsAliveNext;
        public void DetermineNextLiveState()
        {
            int liveNeighbors = neighbors.Where(x => x.IsAlive).Count();
            if (IsAlive)
                IsAliveNext = liveNeighbors == 2 || liveNeighbors == 3;
            else
                IsAliveNext = liveNeighbors == 3;
        }
        public void Advance()
        {
            IsAlive = IsAliveNext;
        }
         public Cell()
        {
        }
        public Cell(int val)
        {
            if (val == 1) IsAlive = true;
            else IsAlive = false;
        }
    }
    public class Board
    {
        public readonly Cell[,] Cells;
        public readonly int CellSize;
        public int F = 1;
        public int live_count = 0;
        public int columnst { get { return Cells.GetLength(0); } }
        public int rowss { get { return Cells.GetLength(1); } }
        public int Width { get { return columnst * CellSize; } }
        public int Height { get { return rowss * CellSize; } }

        public Board(int value,int width, int height, int cellSize, double Live = .1)
        {
            CellSize = cellSize;
            if (value == 0)
            {
            Cells = new Cell[width / cellSize, height / cellSize];
            for (int x = 0; x < columnst; x++)
                for (int y = 0; y < rowss; y++)
                    Cells[x, y] = new Cell();

            ConnectNeighbors();
            Randomize(Live);
            }
            else
                {
                using (StreamReader r = new StreamReader("test.txt"))
                {
                    List<string> matrix = new List<string>();
                    string str;
                    int max = 0;
                    while ((str = r.ReadLine()) != null)
                    {
                        matrix.Add(str);
                        if (str.Length > max) max = str.Length;
                    }
                    if (max == 0) return;
                    Cells = new Cell[max / cellSize, matrix.Count/ cellSize];
                    for (int x = 0; x < columnst; x++)
                    { 
                        for (int y = 0; y < rowss; y++)
                        {
                            if (matrix[y].Length <= x)
                                Cells[x, y] = new Cell(0);
                            else if (matrix[y][x] == '*') Cells[x, y] = new Cell(1);
                            else Cells[x, y] = new Cell(0);
                        }
                    }
                    ConnectNeighbors();
                }
            }


        }

        readonly Random rand = new Random();
        public void Randomize(double Live)
        {
            foreach (var cell in Cells)
                cell.IsAlive = rand.NextDouble() < Live;
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
        private void ConnectNeighbors()
        {
            for (int x = 0; x < columnst; x++)
            {
                for (int y = 0; y < rowss; y++)
                {
                    int xL = (x > 0) ? x - 1 : columnst - 1;
                    int xR = (x < columnst - 1) ? x + 1 : 0;

                    int yT = (y > 0) ? y - 1 : rowss - 1;
                    int yB = (y < rowss - 1) ? y + 1 : 0;

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
            public double Live;
        }
        static Board board;
        static private void Reset(int val)
        {
            using (StreamReader r = new StreamReader("json_file.json"))
            {
                Item items = new Item();
                string str = r.ReadToEnd();
                string strtmp = "";
                int i = 0;
                while (str[i] != ':') i++;
                i++;
                while (str[i] != ',')
                {
                    strtmp = strtmp + str[i];
                    i++;
                }
                items.width = Int32.Parse(strtmp);
                while (str[i] != ':') i++;
                i++;
                strtmp = "";
                while (str[i] != ',')
                     {
                    strtmp = strtmp + str[i];
                    i++;
                }
                items.height = Int32.Parse(strtmp);
                while (str[i] != ':') i++;
                i++;
                strtmp = "";
                while (str[i] != ',')
                {
                    strtmp = strtmp + str[i];
                    i++;
                }
                items.cellSize = Int32.Parse(strtmp);
                while (str[i] != ':') i++;
                i++;
                strtmp = "";
                while (str[i] != '.')
                {
                    strtmp = strtmp + str[i];
                    i++;
                }
                i++;
                items.Live = Int32.Parse(strtmp);
                strtmp = "";
                while (str[i] != '\r')
                     {
                    strtmp = strtmp + str[i];
                    i++;
                }
                int st = 1;
                for (int j = 0; j < strtmp.Length; j++) st = st * 10;
                items.Live = items.Live + (float)Int32.Parse(strtmp) / st;
                board = new Board(
                    val,
                    width: items.width,
                    height: items.height,
                    cellSize: items.cellSize,
                    Live: items.Live);
            }
            
        }
        static void Render()
        {
            using (StreamWriter r = new StreamWriter("test.txt"))
            for (int rows = 0; rows < board.rowss; rows++)
            {
                for (int column = 0; column < board.columnst; column++)   
                {
                    var cell = board.Cells[column, rows];
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
        static void Main(string[] ar)
        {
             bool eq(int[,] arrayay1, int[,] arrayay2)
            {
                if (arrayay1.Length != arrayay2.Length) return false;
                int F = 1;
                for (int i = 0; i < arrayay1.Length / 2; i++) 
                {
                    int F2 = 0;
                    for (int j = 0; j < arrayay1.Length / 2; j++)
                    {
                        if (arrayay1[i, 0] == arrayay2[j, 0] && arrayay1[i, 1] == arrayay2[j, 1]) 
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
            int[,] sort(int[,] array)
            {
                int F1 = 1;
                int F2 = 1;
                int F3 = 1;
                int F4 = 1;
                int minX = 10000;
                int minY = 10000;
                for (int i = 0; i < array.Length / 2; i++) 
                {
                    if (array[i, 0] == 0) F1 = 0;
                    if (array[i, 0] == board.columnst - 1) F2 = 0;
                    if (array[i, 0] < minX) minX = array[i, 0];
                    if (array[i, 1] == 0) F3 = 0;
                    if (array[i, 1] == board.rowss - 1) F4 = 0;
                    if (array[i, 1] < minY) minY = array[i, 1];
                }
                if (F1 == 0 && F2 == 0)
                {
                    for (int i = 0; i < board.columnst; i++)
                    {
                        int F = 0;
                        for (int j = 0; j < array.Length / 2; j++) 
                            if (array[j, 0] == board.columnst - i - 1) 
                            {
                                F = 1;
                                break;
                            }
                        if (F == 0) 
                        {
                            for (int j = 0; j < array.Length / 2; j++) 
                            {
                                if (array[j, 0] < board.columnst - i - 1) array[j, 0] = array[j, 0] + i;
                                else array[j, 0] = array[j, 0] - board.columnst + i;
                            }
                            break;
                        }
                    }
                }
                if (F3 == 0 && F4 == 0)
                {
                    for (int i = 0; i < board.columnst; i++)
                    {
                        int F = 0;
                        for (int j = 0; j < array.Length / 2; j++) 
                            if (array[j, 1] == board.rowss - i - 1) 
                            {
                                F = 1;
                                break;
                            }
                        if (F == 0)
                        {
                            for (int j = 0; j < array.Length / 2; j++) 
                            {
                                if (array[j, 1] < board.rowss - i - 1) array[j, 1] = array[j, 1] + i;
                                else array[j, 1] = array[j, 1] - board.rowss + i;
                            }
                            break;
                        }
                    }
                }
                if (minX >= 1)
                    for (int i = 0; i < array.Length / 2; i++) array[i, 0] = array[i, 0] - minX + 1;
                else for (int i = 0; i < array.Length / 2; i++) array[i, 0]++;
                if (minY >= 1)
                    for (int i = 0; i < array.Length / 2; i++) array[i, 1] = array[i, 1] - minY + 1;
                else for (int i = 0; i < array.Length / 2; i++) array[i, 1]++;
                for (int i = 0; i < array.Length / 2; i++) Console.WriteLine(array[i, 0].ToString() + ' ' + array[i, 1].ToString());
                Console.WriteLine("\n");
                return array;
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
            int perestnamb = 0;
            for (int x = 0; x < board.columnst; x++)
            {
                for (int y = 0; y < board.rowss; y++)
                {
                    if (board.Cells[x,y].IsAlive)
                    {
                        int min = 100000;
                        int F2 = 0;
                        for (int i = 0; i < 8; i++)
                            if (board.Cells[x, y].neighbors[i].perest > 0 && board.Cells[x, y].neighbors[i].perest < min)
                            {
                                F2 = 1;
                                min = board.Cells[x, y].neighbors[i].perest;
                            }
                        if (F2 == 0)
                        {
                            perestnamb++;
                            board.Cells[x, y].perest = perestnamb;
                        }
                        else board.Cells[x, y].perest = min;
                    }
                }
            }
            int[,] hive = new int[6,2];
            int hive_count = 0;
            hive[0, 0] = 2; hive[0, 1] = 1; hive[1, 0] = 1; hive[1, 1] = 2; hive[2, 0] = 3; hive[2, 1] = 2; hive[3, 0] = 1; hive[3, 1] = 3; hive[4, 0] = 3; hive[4, 1] = 3; hive[5, 0] = 2; hive[5, 1] = 4;
            int[,] loaf = new int[7, 2];
            int loaf_count = 0;
            loaf[0, 0] = 2; loaf[0, 1] = 1; loaf[1, 0] = 3; loaf[1, 1] = 1; loaf[2, 0] = 1; loaf[2, 1] = 2; loaf[3, 0] = 4; loaf[3, 1] = 2; loaf[4, 0] = 2; loaf[4, 1] = 3; loaf[5, 0] = 4; loaf[5, 1] = 3; loaf[6, 0] = 3; loaf[6, 1] = 4;
            int[,] pull = new int[8, 2];
            int pull_count = 0;
            pull[0, 0] = 2; pull[0, 1] = 1; pull[1, 0] = 3; pull[1, 1] = 1; pull[2, 0] = 1; pull[2, 1] = 2; pull[3, 0] = 4; pull[3, 1] = 2; pull[4, 0] = 1; pull[4, 1] = 3; pull[5, 0] = 4; pull[5, 1] = 3; pull[6, 0] = 2; pull[6, 1] = 4; pull[7, 0] = 3; pull[7, 1] = 4;
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
            for (int i = 0; i < perestnamb; i++)
            {
                List<int> array_1 = new List<int>();
                List<int> array_2 = new List<int>();
                for (int x = 0; x < board.columnst; x++)
                    for (int y = 0; y < board.rowss; y++)
                    {
                        if (board.Cells[x, y].perest == i + 1) 
                        {
                            array_1.Add(x);
                            array_2.Add(y);
                        }
                    }
                int[,] array = new int[array_1.Count, 2];
                for (int j = 0; j < array_1.Count; j++)
                {
                    array[j, 0] = array_1[j];
                    array[j, 1] = array_2[j];
                }
                array = sort(array);
                if (eq(array, hive)) hive_count++;
                else if (eq(array, loaf)) loaf_count++;
                else if (eq(array, pull)) pull_count++;
                else if (eq(array, box)) box_count++;
                else if (eq(array, block)) block_count++;
                else if (eq(array, snake)) snake_count++;
                else if (eq(array, barge)) barge_count++;
                else if (eq(array, boat)) boat_count++;
                else if (eq(array, ship)) ship_count++;
                else unknown_count++;
            }
            Console.WriteLine("\nSurvivors: " + board.live_count.ToString());
            Console.WriteLine("\nCombinations: " + perestnamb.ToString());
            Console.WriteLine("\nHives: " + hive_count.ToString());
            Console.WriteLine("\nLoafs: " + loaf_count.ToString());
            Console.WriteLine("\npulls: " + pull_count.ToString());
            Console.WriteLine("\nBoxes: " + box_count.ToString());
            Console.WriteLine("\nBlocks: " + block_count.ToString());
            Console.WriteLine("\nSnakes: " + snake_count.ToString());
            Console.WriteLine("\nBarges: " + barge_count.ToString());
            Console.WriteLine("\nBoats: " + boat_count.ToString());
            Console.WriteLine("\nShips: " + ship_count.ToString());
            Console.WriteLine("\nUnknown: " + unknown_count.ToString());
            Console.WriteLine("Average generation: " + (count/n).ToString());
            Console.ReadKey();
        
           
        }
    }
}