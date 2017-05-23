using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Draughts
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Game CurrentGame { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            InitBoard();
        }

        public void InitBoard()
        {
            CurrentGame = new Game();
 
            this.Board.ItemsSource = CurrentGame.Pieces;
            this.DataContext = this;
            this.BestCaptures.ItemsSource = CurrentGame.BestCaptures;
        }

        /// <summary>
        /// User starts dragging piece
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            Thumb thumb = (Thumb)sender;
            CellData piece = (CellData)thumb.DataContext;

            if (piece.IsHighlight)
                return;

            // Update the position of the rectangle in the data-model.
            piece.Pos = new Point(
                piece.Pos.X + e.HorizontalChange, 
                piece.Pos.Y + e.VerticalChange
                );
        }

        /// <summary>
        /// User ends dragging piece
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            Thumb thumb = (Thumb)sender;
            CellData piece = (CellData)thumb.DataContext;

            if (piece.IsHighlight)
                return;

            // use Math.Round to stick to cell
            CurrentGame.MovePiece(piece.OriginalPos, 
                new Point(
                    Math.Round(piece.Pos.X), 
                    Math.Round(piece.Pos.Y)
                ));
        }

        /// <summary>
        /// User adds new White piece (from Options pane)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewWhite_DragStarted(object sender, DragStartedEventArgs e)
        {
            Thumb thumb = (Thumb) sender;

            // Initialize the drag & drop operation
            DataObject dragData = new DataObject("PieceFormat", new CellData (new Point(), PieceType.Man, Player.White));
            DragDrop.DoDragDrop(thumb, dragData, DragDropEffects.Move);
        }

        /// <summary>
        /// User adds new Black piece (from Options pane)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewBlack_DragStarted(object sender, DragStartedEventArgs e)
        {
            Thumb thumb = (Thumb)sender;

            // Initialize the drag & drop operation
            DataObject dragData = new DataObject("PieceFormat", new CellData(new Point(), PieceType.Man, Player.Black));
            DragDrop.DoDragDrop(thumb, dragData, DragDropEffects.Move);
        }

        /// <summary>
        /// User drops new piece to the board
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Board_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("PieceFormat"))
            {
                CellData piece = e.Data.GetData("PieceFormat") as CellData;
                ItemsControl board = sender as ItemsControl;

                Point position = e.GetPosition(board);

                // Stick to cell
                piece.Pos = new Point(
                    Math.Round(position.X),
                    Math.Round(position.Y)
                    );

                CurrentGame.AddPiece(piece);
            }
        }
        
        /// <summary>
        /// Perform Capture
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CaptureBtn_Click(object sender, RoutedEventArgs e)
        {
            CurrentGame.Capture((CaptureData) BestCaptures.SelectedValue);
        }

        /// <summary>
        /// User selects capture option
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BestCaptures_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentGame.HighlightCapture((CaptureData)BestCaptures.SelectedValue);
        }

        /// <summary>
        /// User resets the board
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {
            InitBoard();
        }

        /// <summary>
        /// User switches piece type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Thumb_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            Thumb thumb = (Thumb)sender;
            CellData piece = (CellData)thumb.DataContext;

            if (piece.IsHighlight)
                return;
            
            CurrentGame.SwitchPieceType(piece.OriginalPos);
        }
    }
}
