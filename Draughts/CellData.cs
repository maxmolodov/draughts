using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight;

namespace Draughts
{
    /// <summary>
    /// Board cell data
    /// Should be split into inheriance hierarchy here, but didn't find a quick solution to find out binding type.
    /// So, 'Highlights', 'Text' and 'Draught Pieces' in one class
    /// </summary>
    public class CellData : ViewModelBase
    {
        /// <summary>
        /// Highlight/Text constructor
        /// </summary>
        /// <param name="pos">Position</param>
        /// <param name="highlightColor">Highlight Color</param>
        /// <param name="text">Text</param>
        /// <param name="highlightOpacity">Highlight Opacity</param>
        public CellData(Point pos, Color highlightColor, string text, double highlightOpacity = 1)
        {
            _pos = pos;
            _highlightColor = highlightColor;
            _text = text;
            _highlightOpacity = highlightOpacity;

            OriginalPos = Pos;
        }

        /// <summary>
        /// Draught Piece constructor
        /// </summary>
        /// <param name="pos">Position</param>
        /// <param name="type">Piece Type</param>
        /// <param name="player">Player side</param>
        public CellData(Point pos, PieceType type, Player player)
        {
            _pos = pos;
            _type = type;
            Player = player;

            OriginalPos = Pos;
        }

        /// <summary>
        /// Remembers Original Position. Used when Dragging.
        /// </summary>
        public void CompleteMove()
        {
            OriginalPos = Pos;
        }

        /// <summary>
        /// Reverts Display Position. Used when Dragging.
        /// </summary>
        public void CancelMove()
        {
            Pos = OriginalPos;
        }

        private Point _pos;
        /// <summary>
        /// Drag/Render position
        /// </summary>
        public Point Pos
        {
            get { return _pos; }
            set { _pos = value; RaisePropertyChanged(() => Pos); }
        }

        /// <summary>
        /// Original Position before move/drag is consideted complete
        /// </summary>
        public Point OriginalPos { get; private set; }

        /// <summary>
        /// Is highlight cell
        /// </summary>
        public bool IsHighlight => Type == null && Player == null && HighlightColor != null;

        private Color? _highlightColor;
        /// <summary>
        /// Highlight color
        /// </summary>
        public Color? HighlightColor
        {
            get { return _highlightColor; }
            set { _highlightColor = value; RaisePropertyChanged(() => HighlightColor); }
        }

        private double? _highlightOpacity;
        /// <summary>
        /// Highlight opacity
        /// </summary>
        public double? HighlightOpacity
        {
            get { return _highlightOpacity; }
            set { _highlightOpacity = value; RaisePropertyChanged(() => HighlightOpacity); }
        }
        /// <summary>
        /// Highlight brush, calculated based on highlight color
        /// </summary>
        public Brush HighlightBrush { get { return HighlightColor.HasValue ? new SolidColorBrush(HighlightColor.Value) : new SolidColorBrush();} }

        private string _text;
        /// <summary>
        /// Text to be displayed
        /// </summary>
        public string Text
        {
            get { return _text; }
            set { _text = value; RaisePropertyChanged(() => Text); }
        }

        private PieceType? _type;
        /// <summary>
        /// Piece type: Man/King
        /// </summary>
        public PieceType? Type
        {
            get { return _type; }
            set { _type = value; RaisePropertyChanged(() => Type); }
        }
        /// <summary>
        /// Player side: White/Black
        /// </summary>
        public Player? Player { get; }

    }
}
