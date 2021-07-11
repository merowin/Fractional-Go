using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Media;

namespace Fractional_Go
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int BoardSize = 9;
        private const int distleft = 25;
        private const int disttop = 25;
        private const int PositionIndicatorRadius = 10;
        private const int linedist = 50;

        private Game Game1;

        
         // Scenario 4 Players, 2 Teams of 2 or free-for-all
          
        private List<int> Colours = new List<int>() { 1, 2, 3, 4 };

        private List<List<int>> ColourCombinations = new List<List<int>>()
            {
            new List<int>() { 1, 2 },
            new List<int>() { 3, 4 },
            new List<int>() { 1, 4 },
            new List<int>() { 3, 2 }
            };

        private List<Brush> Paintbrush = new List<Brush>()
        {
            Brushes.Black,
            Brushes.Blue,
            Brushes.White,
            Brushes.Red
        };

        //end


        /* // Scenario 3 Players, free for all
        
        private List<int> Colours = new List<int>() { 1, 2, 3 };

        private List<List<int>> ColourCombinations = new List<List<int>>()
            {
            new List<int>() { 1, 2 },
            new List<int>() { 2, 3 },
            new List<int>() { 3, 1 }
            };

        private List<Brush> Paintbrush = new List<Brush>()
        {
            Brushes.Blue,
            Brushes.Red,
            Brushes.Green
        };        
        */

        private int MouseX, MouseY;
        private List<List<List<Ellipse>>> EllipseField; 
        private Ellipse PositionIndicator;
        private List<Ellipse> NextStone;

        public MainWindow()
        {
            InitializeComponent();

            Game1 = new Game(BoardSize, Colours, ColourCombinations);

            EllipseField = new List<List<List<Ellipse>>>();
            for (int i = 0; i < BoardSize; i++)
            {
                EllipseField.Add(new List<List<Ellipse>>());
                for (int j = 0; j < BoardSize; j++)
                {
                    EllipseField[i].Add(new List<Ellipse>());
                }
            }

            PositionIndicator = new Ellipse();
            PositionIndicator.Stroke = Brushes.Black;
            PositionIndicator.StrokeThickness = 2;
            PositionIndicator.Width = 2 * PositionIndicatorRadius;
            PositionIndicator.Height = 2 * PositionIndicatorRadius;
            PositionIndicator.Visibility = Visibility.Hidden;
            Board.Children.Add(PositionIndicator);

            ResetNextStone(ColourCombinations[0]);

            Line line;
            for (int i = 0; i < BoardSize; i++)
            {
                //bottom-top
                line = new Line();
                line.Stroke = Brushes.Black;
                line.StrokeThickness = 2;
                line.X1 = distleft + i * linedist;
                line.Y1 = disttop;
                line.X2 = distleft + i * linedist;
                line.Y2 = disttop + linedist * (BoardSize - 1);
                Board.Children.Add(line);

                //left-right
                line = new Line();
                line.Stroke = Brushes.Black;
                line.StrokeThickness = 2;
                line.Y1 = disttop + i * linedist;
                line.X1 = distleft;
                line.Y2 = disttop + i * linedist;
                line.X2 = distleft + linedist * (BoardSize - 1);
                Board.Children.Add(line);
            }
        }

        private void ResetNextStone(List<int> _colourCombination)
        {
            int b1;
            double d;

            b1 = _colourCombination.Count();
            NextStoneCanvas.Children.Clear();
            NextStone = new List<Ellipse>();
            for (int ind = 0; ind < Game1.MaxNumberColours; ind++)
            {
                NextStone.Add(new Ellipse());
                NextStone[ind].Stroke = Brushes.Black;
                NextStone[ind].StrokeThickness = 2;
                if (ind < b1)
                {
                    NextStone[ind].Fill = Paintbrush[_colourCombination[ind] - 1];
                    d = linedist * (b1 - ind) / b1;
                    NextStone[ind].Width = d;
                    NextStone[ind].Height = d;
                    Canvas.SetLeft(NextStone[ind], (linedist - d) / 2);
                    Canvas.SetTop(NextStone[ind], (linedist - d) / 2);
                }
                else
                {
                    NextStone[ind].Visibility = Visibility.Hidden;
                }
                NextStoneCanvas.Children.Add(NextStone[ind]);
            }
        }

        private void UpdateNextStone(List<int> _colourCombination)
        {
            int c, b;
            double d;

            b = _colourCombination.Count();

            for (int ind = 0; ind < b; ind++)
            {
                c = _colourCombination[ind];
                d = linedist * (b - ind) / b;
                NextStone[ind].Width = d;
                NextStone[ind].Height = d;
                NextStone[ind].Fill = Paintbrush[c - 1];
                Canvas.SetLeft(NextStone[ind], (linedist - d) / 2);
                Canvas.SetTop(NextStone[ind], (linedist - d) / 2);
                NextStone[ind].Visibility = Visibility.Visible;
            }
            for (int ind = b; ind < Game1.MaxNumberColours; ind++)
            {
                NextStone[ind].Visibility = Visibility.Hidden;
            }
        }

        private Ellipse NewStone(int c)
        {
            Ellipse Stone = new Ellipse();
            Stone.Stroke = Brushes.Black;
            Stone.StrokeThickness = 2;
            if ((0 < c) && (c <= Paintbrush.Count())) Stone.Fill = Paintbrush[c - 1];
            return Stone;
        }

        private void PlaceStone(int x, int y, List<int> StoneColours)
        {
            int c;
            double d;
            int b = StoneColours.Count();
            Ellipse Stone;

            for (int ind = 0; ind < b; ind++)
            {
                c = StoneColours[ind];
                d = linedist * (b - ind) / b;
                Stone = NewStone(c);
                Stone.Width = d;
                Stone.Height = d;
                Canvas.SetTop(Stone, disttop + y * linedist - d / 2 );
                Canvas.SetLeft(Stone, distleft + x * linedist - d / 2);
                Board.Children.Add(Stone);
                EllipseField[x][y].Add(Stone);
            }
        }

        private void RemoveStone(int x, int y)
        {
            int b;
            b = EllipseField[x][y].Count();
            for (int i = 0; i < b; i++)
            {
                Board.Children.Remove(EllipseField[x][y][i]);
            }
            EllipseField[x][y] = new List<Ellipse>();
        }

        private void ClearBoard(object sender, RoutedEventArgs e)
        {
            Game1.MoveCycler = 0;
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    if (Game1.BoardPosition[i][j] != 0) RemoveStone(i, j);
                }
            }
        }

        private void BoardClick(object sender, RoutedEventArgs e)
        {
            List<List<coordinate>> DeadChains;
            if (Game1.BoardPosition[MouseX][MouseY] == 0)
            {
                PlaceStone(MouseX, MouseY, ColourCombinations[Game1.MoveCycler]);
                DeadChains = Game1.MakeMove(MouseX, MouseY);
                foreach (var list in DeadChains)
                {
                    foreach (coordinate p in list)
                    {
                        RemoveStone(p.X, p.Y);
                    }
                }
                UpdateNextStone(ColourCombinations[Game1.MoveCycler]);
            }
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            PositionIndicator.Visibility = Visibility.Visible;
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            PositionIndicator.Visibility = Visibility.Hidden;
        }

        private void Button_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(Board);
            MouseX = (int)((p.X - disttop + linedist / 2) / linedist);
            MouseY = (int)((p.Y - distleft + linedist / 2) / linedist);
            Canvas.SetLeft(PositionIndicator, distleft - PositionIndicatorRadius + MouseX * linedist);
            Canvas.SetTop(PositionIndicator, disttop - PositionIndicatorRadius + MouseY * linedist);
        }
    }
    public class coordinate
    {
        public int X;
        public int Y;
        public coordinate(int XCoordinate, int YCoordinate)
        {
            X = XCoordinate;
            Y = YCoordinate;
        }
    }

    public class Game
    {
        private List<coordinate> Queue;
        List<List<Boolean>> Flag;
        public readonly int BoardSize;
        public readonly List<int> Colours;
        private readonly int NumberofColourCombinations;
        private List<List<int>> ColourCombinations;
        public List<List<int>> BoardPosition; //0 = no stone
        public int MoveCycler;
        public int MaxNumberColours;

        public Game(int _boardsize, List<int> _colours, List<List<int>> _colourCombinations)
        {
            BoardSize = _boardsize;
            Colours = _colours;
            ColourCombinations = _colourCombinations;
            //Queue = new List<coordinate>();
            Flag = new List<List<Boolean>>();
            MoveCycler = 0;
            NumberofColourCombinations = ColourCombinations.Count();

            MaxNumberColours = 0;
            for (int z = 0; z < NumberofColourCombinations; z++) MaxNumberColours = Math.Max(MaxNumberColours, ColourCombinations[z].Count());

            for (int x = 0; x < BoardSize; x++)
            {
                Flag.Add(new List<Boolean>());
                for (int y = 0; y < BoardSize; y++)
                {
                    Flag[x].Add(false);
                }
            }

            BoardPosition = new List<List<int>>();
            for (int i = 0; i < BoardSize; i++)
            {
                BoardPosition.Add(new List<int>());
                for (int j = 0; j < BoardSize; j++)
                {
                    BoardPosition[i].Add(0);
                }
            }
        }

        public List<List<coordinate>> MakeMove(int x, int y)
        {
            List<List<coordinate>> DeadChains;

            BoardPosition[x][y] = MoveCycler + 1;
            DeadChains = RemoveStones(MoveCycler, new coordinate(x, y));
            MoveCycler = (MoveCycler + 1) % NumberofColourCombinations;

            return DeadChains;
        }

        private void ClearFlags()
        {
            for (int x = 0; x < BoardSize; x++)
            {
                for (int y = 0; y < BoardSize; y++)
                {
                    Flag[x][y] = false;
                }
            }
        }

        public (List<coordinate>, bool) SearchFrom(coordinate p, int c)
        {
            List<coordinate> Queue = new List<coordinate>();
            List<coordinate> Chain = new List<coordinate>();
            coordinate point;
            bool Hasliberty = false;

            Queue.Add(p);
            Flag[p.X][p.Y] = true;

            while (Queue.Count() > 0)
            {
                point = Queue[0];
                Queue.RemoveAt(0);

                if (ColourCombinations[BoardPosition[point.X][point.Y] - 1].Contains(c))
                {
                    Chain.Add(point);

                    if (point.X + 1 < BoardSize) if (!Flag[point.X + 1][point.Y])
                    {
                        if (BoardPosition[point.X + 1][point.Y] > 0)
                        {
                            Queue.Add(new coordinate(point.X + 1, point.Y));
                            Flag[point.X + 1][point.Y] = true;
                        }
                        else Hasliberty = true;
                    }
                    if (point.Y + 1 < BoardSize) if (!Flag[point.X][point.Y + 1])
                    {
                        if (BoardPosition[point.X][point.Y + 1] > 0)
                        {
                            Queue.Add(new coordinate(point.X, point.Y + 1));
                            Flag[point.X][point.Y + 1] = true;
                        }
                        else Hasliberty = true;
                    }
                    if (point.X - 1 >= 0) if (!Flag[point.X - 1][point.Y])
                    {
                        if (BoardPosition[point.X - 1][point.Y] > 0)
                        {
                            Queue.Add(new coordinate(point.X - 1, point.Y));
                            Flag[point.X - 1][point.Y] = true;
                        }
                        else Hasliberty = true;
                    }
                    if (point.Y - 1 >= 0) if (!Flag[point.X][point.Y - 1])
                    {
                        if (BoardPosition[point.X][point.Y - 1] > 0)
                        {
                            Queue.Add(new coordinate(point.X, point.Y - 1));
                            Flag[point.X][point.Y - 1] = true;
                        }
                        else Hasliberty = true;
                    }
                } 
            }
        return (Chain, Hasliberty);
        }

        public List<List<coordinate>> RemoveStones(int cind, coordinate startpoint)
        {
            List<List<coordinate>> DeadChains;
            List<coordinate> CurrentChain;
            coordinate point;
            bool HasLiberty = false;

            DeadChains = new List<List<coordinate>>();
            CurrentChain = new List<coordinate>();
            point = new coordinate(0, 0);

            foreach (int colour in Colours)
            {
                if (!ColourCombinations[cind].Contains(colour))
                {
                    if (startpoint.X + 1 < BoardSize) if (BoardPosition[startpoint.X + 1][startpoint.Y] > 0) if (ColourCombinations[BoardPosition[startpoint.X + 1][startpoint.Y] - 1].Contains(colour))
                        {
                            (CurrentChain, HasLiberty) = SearchFrom(new coordinate(startpoint.X + 1, startpoint.Y), colour);
                            if (!HasLiberty) DeadChains.Add(CurrentChain);
                            ClearFlags();
                            }

                    if (startpoint.Y + 1 < BoardSize) if (BoardPosition[startpoint.X][startpoint.Y + 1] > 0) if (ColourCombinations[BoardPosition[startpoint.X][startpoint.Y + 1] - 1].Contains(colour))
                        {
                            (CurrentChain, HasLiberty) = SearchFrom(new coordinate(startpoint.X, startpoint.Y + 1), colour);
                            if (!HasLiberty) DeadChains.Add(CurrentChain);
                            ClearFlags();
                            }

                    if (startpoint.X - 1 >= 0) if (BoardPosition[startpoint.X - 1][startpoint.Y] > 0) if (ColourCombinations[BoardPosition[startpoint.X - 1][startpoint.Y] - 1].Contains(colour))
                        {
                            (CurrentChain, HasLiberty) = SearchFrom(new coordinate(startpoint.X - 1, startpoint.Y), colour);
                            if (!HasLiberty) DeadChains.Add(CurrentChain);
                            ClearFlags();
                            }

                    if (startpoint.Y - 1 >= 0) if (BoardPosition[startpoint.X][startpoint.Y - 1] > 0) if (ColourCombinations[BoardPosition[startpoint.X][startpoint.Y - 1] - 1].Contains(colour))
                        {
                            (CurrentChain, HasLiberty) = SearchFrom(new coordinate(startpoint.X, startpoint.Y - 1), colour);
                            if (!HasLiberty) DeadChains.Add(CurrentChain);
                            ClearFlags();
                            }
                }
            }
            if (DeadChains.Count > 0)
            {
                foreach (List<coordinate> l in DeadChains)
                {
                    foreach (coordinate p in l)
                    {
                        BoardPosition[p.X][p.Y] = 0;
                    }
                }
            }
            else
            {
                foreach (int colour in Colours)
                {
                    if (ColourCombinations[cind].Contains(colour))
                    {
                        (CurrentChain, HasLiberty) = SearchFrom(new coordinate(startpoint.X, startpoint.Y), colour);
                        if (!HasLiberty) DeadChains.Add(CurrentChain);
                        ClearFlags();
                    }
                }

                foreach (List<coordinate> l in DeadChains)
                {
                    foreach (coordinate p in l)
                    {
                        BoardPosition[p.X][p.Y] = 0;
                    }
                }
            }
            return DeadChains;
        }
    }
}