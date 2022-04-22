using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace ESMT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly TriangulationEngine _triangulationEngine;

        private readonly SpanningTreeEngine _spanningTreeEngine;

        public int PointsAmount { get; set; }

        public double DiagramWidth => (int)Canvas.ActualWidth;

        public double DiagramHeight => (int)Canvas.ActualHeight;

        public ICommand RunCommand { get; set; }

        public ICommand ClearCommand { get; set; }

        public ICommand GeneratePointsCommand { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;
            _triangulationEngine = new TriangulationEngine();
            _spanningTreeEngine = new SpanningTreeEngine();
            _points = new List<Shapes.Point>();
            RunCommand = new Command(param => RunBuilder());
            ClearCommand = new Command(param => ClearCanvas());
            GeneratePointsCommand = new Command(param => DrawRandomPoints());
            Canvas.SizeChanged += (sender, args) =>
            {
                OnPropertyChanged(nameof(DiagramWidth));
                OnPropertyChanged(nameof(DiagramHeight));
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private List<Shapes.Point> _points;

        private void RunBuilder()
        {
            if(_points == null)
                return;
            Canvas.ClipToBounds = true;
            Canvas.Children.Clear();
            DrawPoints(_points);
            var timer = new Stopwatch();
            timer.Start();
            var triangulation = _triangulationEngine.GetTriangulation(_points.ToList(), DiagramWidth, DiagramHeight);
            List<Edge> edges = new List<Edge>();
            foreach(var triangle in triangulation)
            {
                edges.Add(new Edge(triangle.Vertices[0], triangle.Vertices[1]));
                edges.Add(new Edge(triangle.Vertices[1], triangle.Vertices[2]));
                edges.Add(new Edge(triangle.Vertices[2], triangle.Vertices[0]));
            }
            if(edges.Count == 0 && _points.Count == 2)
            {
                edges.Add(new Edge(_points[0], _points[1]));
            }
            var emst = _spanningTreeEngine.GetSpanningTree(edges);
            timer.Stop();
            DrawGraph(edges, Brushes.Gray, 1);
            DrawGraph(emst, Brushes.Red, 2);
            TimeEllapsed.Text = timer.Elapsed.ToString();
        }

        private void DrawRandomPoints()
        {
            _points = Shapes.Point.GenerateRandomPoints(PointsAmount, DiagramWidth, DiagramHeight).ToList();
            DrawPoints(_points);
        }

        private void DrawGraph(IEnumerable<Edge> edges, Brush color, double thickness)
        {
            foreach(var edge in edges)
            {
                DrawLine(edge.Point1, edge.Point2, color, thickness);
            }
        }

        private void DrawPoints(List<Shapes.Point> points)
        {
            foreach(var point in points)
            {
                DrawPoint(point.X, point.Y);
            }
        }

        private void ClearCanvas()
        {
            Canvas.Children.Clear();
            TimeEllapsed.Text = String.Empty;
            _points.Clear();
        }

        private void DrawPoint(double x, double y)
        {
            var ellipse = new Ellipse();
            ellipse.Fill = Brushes.Black;
            ellipse.HorizontalAlignment = HorizontalAlignment.Left;
            ellipse.VerticalAlignment = VerticalAlignment.Top;
            ellipse.Width = 4;
            ellipse.Height = 4;
            ellipse.Margin = new Thickness(x - 0.5 * ellipse.Height, y - 0.5 * ellipse.Width, 0, 0);
            Canvas.Children.Add(ellipse);
        }

        private void DrawLine(Shapes.Point p1, Shapes.Point p2, Brush color, double thickness)
        {
            Line line = new Line();
            line.Stroke = color;
            line.StrokeThickness = thickness;
            line.X1 = p1.X;
            line.X2 = p2.X;
            line.Y1 = p1.Y;
            line.Y2 = p2.Y;
            Canvas.Children.Add(line);
        }

        private void CanvasClick(object sender, MouseButtonEventArgs e)
        {
            var point = e.GetPosition((IInputElement)sender);
            Shapes.Point p = new Shapes.Point(Math.Round(point.X, 2), Math.Round(point.Y, 2));
            if(Contains(_points, p))
                return;
            _points.Add(new Shapes.Point(point.X, point.Y));
            DrawPoint(point.X, point.Y);
            RunCommand.Execute(this);
        }

        private bool Contains(IEnumerable<Shapes.Point> points, Shapes.Point p)
        {
            foreach(var point in points)
            {
                if(point.X == p.X && point.Y == p.Y)
                    return true;
            }
            return false;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if(!MouseCoord.IsOpen)
            {
                MouseCoord.IsOpen = true;
            }
            var point = e.GetPosition((IInputElement)sender);
            MouseCoord.HorizontalOffset = point.X + 20;
            MouseCoord.VerticalOffset = point.Y;
            MouseCoordText.Text = $"{Math.Round(point.X, 1)}; {Math.Round(point.Y, 1)}";
        }

        private void Canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            MouseCoord.IsOpen = false;
        }
    }

    public class Command : ICommand
    {
        private readonly Action<object> _execute;

        private readonly Func<object, bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        public Command(Action<object> execute) : this(execute, ParamArrayAttribute => true)
        {
        }

        public Command(Action<object> execute, Func<object, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return (_canExecute == null) || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
