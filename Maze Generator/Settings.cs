using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Maze_Generator
{
    class Settings
    {
        public static readonly int CellSize = 15;
        public static readonly int[] FormSize = new int[]{1200, 750};
        public static readonly int WallThickness = 1;

        public static readonly object[] IncreasedPathways = new object[] {true, 2};

        public static readonly Color BackgroundColor = ColorTranslator.FromHtml("#E2E2E2");
        public static readonly Color GridColor = BackgroundColor;
        public static readonly Color WallColor = Color.Black;
        public static readonly Color CellColor = SystemColors.Control;

        public static readonly int MillisecondsRefresh = 50;
        public static readonly double SecondsSpan = 4;
    }
}
