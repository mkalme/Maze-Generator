using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace Maze_Generator
{
    public partial class Form1 : Form
    {
        public static int[,,] cells;
        public static int[,,] cellWallsDrawn;
        public static List<int[]> path = new List<int[]>();

        public static int[] startingPoint = new int[] { 0, 0 };

        Bitmap bmp;
        Graphics bitmapGraphics;

        bool canBeUsed = true;

        //0 = if visited, 1 = north, 2 = east, 3 = south, 4 = west

        public Form1()
        {
            InitializeComponent();
        }


        //========================================== SETUP FORM ============================================
        private void Form1_Shown(object sender, EventArgs e)
        {
            setupForm();
            generateMaze();
        }

        public void setupForm() {
            BackColor = Settings.BackgroundColor;
            int cellSize = Settings.CellSize + Settings.WallThickness;

            int width = (int)Math.Round((double)Settings.FormSize[0] / (double)cellSize) * cellSize + 16 + Settings.WallThickness;
            int height = (int)Math.Round((double)Settings.FormSize[1] / (double)cellSize) * cellSize + 39 + Settings.WallThickness;

            Width = width;
            Height = height;
            CenterToScreen();

            cells = new int[(width - 16) / cellSize, (height - 39) / cellSize, 5];
            cellWallsDrawn = new int[(width - 16) / cellSize, (height - 39) / cellSize, 5];

            //bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            bmp = new Bitmap(width - 16, height - 39);
            bitmapGraphics = Graphics.FromImage(bmp);

            pictureBox1.Image = bmp;
        }


        //============================================== DRAW MAZE ===============================================
        public void drawGrid() {
            bitmapGraphics.Clear(Settings.BackgroundColor);

            int cellSize = Settings.CellSize + Settings.WallThickness;

            //vertical
            for (int i = 0; i < Width / cellSize + 1; i++) {
                drawLineBitmap(Settings.GridColor, i * cellSize + (Settings.WallThickness / 2), 0, i * cellSize + (Settings.WallThickness / 2), Height);
            }

            //horizontal
            for (int i = 0; i < Height / cellSize + 1; i++)
            {
                drawLineBitmap(Settings.GridColor, 0, i * cellSize + (Settings.WallThickness / 2), Width, i * cellSize + (Settings.WallThickness / 2));
            }
        }

        public void updateGrid()
        {
            drawGrid();
            drawMaze();

            pictureBox1.Image = bmp;
        }


        public void drawMaze()
        {
            int cellSize = Settings.CellSize + Settings.WallThickness;

            //draw end vertical line
            drawLineBitmap(Settings.WallColor, cells.GetLength(0) * cellSize + (Settings.WallThickness / 2), 0, cells.GetLength(0) * cellSize + (Settings.WallThickness / 2), Height);

            //draw horizontal line
            drawLineBitmap(Settings.WallColor, 0, cells.GetLength(1) * cellSize + (Settings.WallThickness / 2), Width, cells.GetLength(1) * cellSize + (Settings.WallThickness / 2));

            //draw all north
            for (int x = 0; x < cellWallsDrawn.GetLength(0); x++){
                for (int y = 0; y < cellWallsDrawn.GetLength(1); y++){
                    if (cellWallsDrawn[x, y, 0] == 1){//if has a wall
                        drawCellWallBitmap(Settings.WallColor, x, y, 0);
                    }
                }
            }

            //draw all west
            for (int x = 0; x < cellWallsDrawn.GetLength(0); x++){
                for (int y = 0; y < cellWallsDrawn.GetLength(1); y++){
                    if (cellWallsDrawn[x, y, 1] == 1){//if has a wall
                        drawCellWallBitmap(Settings.WallColor, x, y, 1);
                    }
                }
            }
        }

        public void drawCellWall(Color color, int x, int y, int type) {
            int cellSize = Settings.CellSize + Settings.WallThickness;

            if (type == 0) {
                drawLineBitmap(color, (cellSize * x), (cellSize * y) + (Settings.WallThickness / 2), (cellSize * x) + (cellSize - (Settings.WallThickness == 1 ? 1 : 0)), (cellSize * y) + (Settings.WallThickness / 2));
            } else if (type == 1) {
                drawLineBitmap(color, (cellSize * x) + (Settings.WallThickness / 2), (cellSize * y), (cellSize * x) + (Settings.WallThickness / 2), (cellSize * y) + (cellSize - (Settings.WallThickness == 1 ? 1 : 0)));
            }
        }

        public void drawCellWallBitmap(Color color, int x, int y, int type)
        {
            int cellSize = Settings.CellSize + Settings.WallThickness;

            if (type == 0)
            {
                drawLineBitmap(color, (cellSize * x), (cellSize * y) + (Settings.WallThickness / 2), (cellSize * x) + (cellSize - (Settings.WallThickness == 1 ? 1 : 0)), (cellSize * y) + (Settings.WallThickness / 2));
            }
            else if (type == 1)
            {
                drawLineBitmap(color, (cellSize * x) + (Settings.WallThickness / 2), (cellSize * y), (cellSize * x) + (Settings.WallThickness / 2), (cellSize * y) + (cellSize - (Settings.WallThickness == 1 ? 1 : 0)));
            }
        }

        public void eraseCellWall(int x, int y, int type) {//type 1 = west, 0 = north
            if (type == 0) {
                drawCellWall(Settings.CellColor, x, y, type);

                if (cellWallsDrawn[x, y, 1] == 1){//if has a west wall
                    drawCellWall(Settings.WallColor, x, y, 1);
                }
            } else if (type == 1) {
                drawCellWall(Settings.CellColor, x, y, type);

                if (cellWallsDrawn[x, y, 0] == 1){//if has a north wall
                    drawCellWall(Settings.WallColor, x, y, 0);
                }
            }
        }


        //========================================== DRAW FUNCTIONS ===============================================
        public void drawLineBitmap(Color color, int x1, int y1, int x2, int y2)
        {
            bitmapGraphics.DrawLine(new Pen(color, Settings.WallThickness), x1, y1, x2, y2);
        }

        public void drawSquare(Color color, int x, int y) {
            int cellSize = Settings.CellSize + Settings.WallThickness;
            bitmapGraphics.FillRectangle(new SolidBrush(color), new Rectangle(x * cellSize, y * cellSize, cellSize, cellSize));
        }


        //=========================================== ALGORITHM ===============================================
        public void generateMaze() {
            canBeUsed = false;

            Array.Clear(cells, 0, cells.Length);
            Array.Clear(cellWallsDrawn, 0, cellWallsDrawn.Length);
            path.Clear();

            int cellSize = Settings.CellSize + Settings.WallThickness;

            path.Add(startingPoint);
            //set as visited
            cells[startingPoint[0], startingPoint[1], 0] = 1;

            drawGrid();

            //draw end vertical line
            drawLineBitmap(Settings.WallColor, cells.GetLength(0) * cellSize + (Settings.WallThickness / 2), 0, cells.GetLength(0) * cellSize + (Settings.WallThickness / 2), bmp.Height);
            //draw horizontal line
            drawLineBitmap(Settings.WallColor, 0, cells.GetLength(1) * cellSize + (Settings.WallThickness / 2), bmp.Width, cells.GetLength(1) * cellSize + (Settings.WallThickness / 2));

            realTimeDrawing();

            Random rand = new Random();
            DateTime time = DateTime.Now;
            TimeSpan timeSpan;
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                double span = Settings.SecondsSpan * 1000; //seconds
                double timeForOneCell = 0.0000125;
                double delay = Math.Abs(span -((cells.GetLength(0) * cells.GetLength(1)) * timeForOneCell)) / ((cells.GetLength(0) * cells.GetLength(1)));

                double temp = (int)Math.Ceiling(1.0 / delay * 6.1) + ((cells.GetLength(0) * cells.GetLength(1) / 200000) * 600);

                for (int i = 1; i < cells.GetLength(0) * cells.GetLength(1); i++) {
                    List<int[]> Neighbors = getNeighbors(path[path.Count - 1][0], path[path.Count - 1][1], false);

                    //if deadend
                    if (Neighbors.Count == 0)
                    {
                        bool doesntHaveNeighbors = true;
                        while (doesntHaveNeighbors) {
                            //remove last item from path
                            path.RemoveAt(path.Count - 1);

                            //check if current item doesn't have neighbors
                            Neighbors = getNeighbors(path[path.Count - 1][0], path[path.Count - 1][1], false);
                            doesntHaveNeighbors = Neighbors.Count == 0;
                        }

                        i--;
                    }
                    else {
                        if (i % (int)temp == 0)
                            Thread.Sleep((int)Math.Ceiling(delay));

                        int indexN = 0;

                        //increase pathway length, if feature turned on
                        int indexOfRepetative = 0;
                        int rangeRandom = 0;
                        if ((bool)Settings.IncreasedPathways[0] && path.Count > 1)
                        {
                            bool Vertical = true; //false = horizontal, true = vertical
                            if (path[path.Count - 1][0] != path[path.Count - 2][0])
                            {//if horizontal changed
                                Vertical = false;
                            }
                            else if (path[path.Count - 1][1] != path[path.Count - 2][1])
                            {//if vertical changed
                                Vertical = true;
                            }

                            for (int b = 0; b < Neighbors.Count; b++)
                            {
                                if (Vertical)
                                {
                                    if (path[path.Count - 1][0] == Neighbors[b][0])
                                    {
                                        indexOfRepetative = b;
                                        goto after_loop;
                                    }
                                }
                                else
                                {
                                    if (path[path.Count - 1][1] == Neighbors[b][1])
                                    {
                                        indexOfRepetative = b;
                                        goto after_loop;
                                    }
                                }
                            }
                        after_loop:
                            rangeRandom = Neighbors.Count + (int)Settings.IncreasedPathways[1];

                            indexN = rand.Next(rangeRandom);
                            indexN = indexN > Neighbors.Count - 1 ? indexOfRepetative : indexN;
                        }
                        else {
                            indexN = rand.Next(Neighbors.Count);
                        }

                        //remove the wall
                        cells[path[path.Count - 1][0], path[path.Count - 1][1], Neighbors[indexN][2] + 1] = 1;

                        path.Add(new int[] { Neighbors[indexN][0], Neighbors[indexN][1] });

                        //remove the wall
                        int wallIndex = Neighbors[indexN][2];
                        if (wallIndex == 0) { wallIndex = 2; } else
                        if (wallIndex == 1) { wallIndex = 3; } else
                        if (wallIndex == 2) { wallIndex = 0; } else
                        if (wallIndex == 3) { wallIndex = 1; };
                        cells[path[path.Count - 1][0], path[path.Count - 1][1], wallIndex + 1] = 1;

                        //set as visited
                        cells[path[path.Count - 1][0], path[path.Count - 1][1], 0] = 1;

                        realTimeDrawing();

                        timeSpan = DateTime.Now - time;
                        if (timeSpan.TotalSeconds >= ((double)Settings.MillisecondsRefresh) / 1000) {
                            lock (pictureBox1)
                            {
                                pictureBox1.Image = bmp;
                            }
                            time = DateTime.Now;
                        }
                    }
                }

                Thread.Sleep(Settings.MillisecondsRefresh);
                pictureBox1.Image = bmp;

                canBeUsed = true;
            }).Start();
        }

        public void realTimeDrawing() {
            drawSquare(Settings.CellColor, path[path.Count - 1][0], path[path.Count - 1][1]);

            //if north is connected
            if (path[path.Count - 1][1] != 0)
            {
                if (cells[path[path.Count - 1][0], path[path.Count - 1][1], 1] == 0)
                {//if has a wall
                    cellWallsDrawn[path[path.Count - 1][0], path[path.Count - 1][1], 0] = 1;
                    drawCellWall(Settings.WallColor, path[path.Count - 1][0], path[path.Count - 1][1], 0);
                }else
                {
                    cellWallsDrawn[path[path.Count - 1][0], path[path.Count - 1][1], 0] = 0;
                    eraseCellWall(path[path.Count - 1][0], path[path.Count - 1][1], 0);
                }
            }else
            {
                cellWallsDrawn[path[path.Count - 1][0], path[path.Count - 1][1], 0] = 1;
                drawCellWall(Settings.WallColor, path[path.Count - 1][0], path[path.Count - 1][1], 0);
            }

            //if east is connected
            if (path[path.Count - 1][0] != cells.GetLength(0) - 1)
            {
                if (cells[path[path.Count - 1][0], path[path.Count - 1][1], 2] == 0)
                {//if has a wall
                    cellWallsDrawn[path[path.Count - 1][0] + 1, path[path.Count - 1][1], 1] = 1;
                    drawCellWall(Settings.WallColor, path[path.Count - 1][0] + 1, path[path.Count - 1][1], 1);
                }
                else
                {
                    cellWallsDrawn[path[path.Count - 1][0] + 1, path[path.Count - 1][1], 1] = 0;
                    eraseCellWall(path[path.Count - 1][0] + 1, path[path.Count - 1][1], 1);
                }
            }

            //if south is connected
            if (path[path.Count - 1][1] != cells.GetLength(1) - 1)
            {
                if (cells[path[path.Count - 1][0], path[path.Count - 1][1], 3] == 0)
                {//if has a wall
                    cellWallsDrawn[path[path.Count - 1][0], path[path.Count - 1][1] + 1, 0] = 1;
                    drawCellWall(Settings.WallColor, path[path.Count - 1][0], path[path.Count - 1][1] + 1, 0);
                }
                else
                {
                    cellWallsDrawn[path[path.Count - 1][0], path[path.Count - 1][1] + 1, 0] = 0;
                    eraseCellWall(path[path.Count - 1][0], path[path.Count - 1][1] + 1, 0);
                }
            }

            //if west is connected
            if (path[path.Count - 1][0] != 0)
            {
                if (cells[path[path.Count - 1][0], path[path.Count - 1][1], 4] == 0)
                {//if has a wall
                    cellWallsDrawn[path[path.Count - 1][0], path[path.Count - 1][1], 1] = 1;
                    drawCellWall(Settings.WallColor, path[path.Count - 1][0], path[path.Count - 1][1], 1);
                }
                else
                {
                    cellWallsDrawn[path[path.Count - 1][0], path[path.Count - 1][1], 1] = 0;
                    eraseCellWall(path[path.Count - 1][0], path[path.Count - 1][1], 1);
                }
            }
            else
            {
                cellWallsDrawn[path[path.Count - 1][0], path[path.Count - 1][1], 1] = 1;
                drawCellWall(Settings.WallColor, path[path.Count - 1][0], path[path.Count - 1][1], 1);
            }
        }

        public static List<int[]> getNeighbors(int x, int y, bool onlyWalls) {
            int[] neighbors = new int[] { 1, 1, 1, 1};

            List<int[]> allNeighbors = new List<int[]>();

            //if on north edge
            if (y == 0)
            {
                neighbors[0] = 0;
            }
            //if on east edge
            if (x == cells.GetLength(0) - 1) {
                neighbors[1] = 0;
            }
            //if on south edge
            if (y == cells.GetLength(1) - 1) {
                neighbors[2] = 0;
            }
            //if on west edge
            if (x == 0) {
                neighbors[3] = 0;
            }

            if (neighbors[0] == 1) {
                bool ifCan = cells[x, y - 1, 0] == 0;
                if (onlyWalls ? true : ifCan) {
                    neighbors[0] = 1;
                    allNeighbors.Add(new int[] { x, y - 1 , 0});
                }
            }

            if (neighbors[1] == 1)
            {
                bool ifCan = cells[x + 1, y, 0] == 0;
                if (onlyWalls ? true : ifCan)
                {
                    neighbors[1] = 1;
                    allNeighbors.Add(new int[] { x + 1, y , 1});
                }
            }

            if (neighbors[2] == 1)
            {
                bool ifCan = cells[x, y + 1, 0] == 0;
                if (onlyWalls ? true : ifCan)
                {
                    neighbors[2] = 1;
                    allNeighbors.Add(new int[] { x, y + 1 , 2});
                }
            }

            if (neighbors[3] == 1)
            {
                bool ifCan = cells[x - 1, y, 0] == 0;
                if (onlyWalls ? true : ifCan)
                {
                    neighbors[3] = 1;
                    allNeighbors.Add(new int[] { x - 1, y , 3});
                }
            }

            return allNeighbors;
        }

        //=========================================== UPDATE GRID ============================================
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (canBeUsed) {
                    generateMaze();
                }
            }
        }
    }
}
