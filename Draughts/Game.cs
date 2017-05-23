using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GalaSoft.MvvmLight;

namespace Draughts
{
    /// <summary>
    /// Draught Game
    /// </summary>
    public class Game : ViewModelBase
    {
        private readonly List<Point> _darkCells = new List<Point>();
        /// <summary>
        /// Board Cell pieces and data collection
        /// </summary>
        public ObservableCollection<CellData> Pieces { get; }
        /// <summary>
        /// Possible Captures collection
        /// </summary>
        public ObservableCollection<CaptureData> BestCaptures { get; set; }


        public Game()
        {
            InitDarkCellsList();
            Pieces = GetPiecesForNewGame();
            BestCaptures = new ObservableCollection<CaptureData>();
        }

        /// <summary>
        /// Inits dark cells list (draught pieces are only allowed to be placed on dark cells)
        /// </summary>
        private void InitDarkCellsList()
        {
            for (int i = 0; i < Globals.BoardHeight; i++)
            {
                for (int j = 0; j < Globals.BoardWidth; j++)
                {
                    if ((i.IsEven() && !j.IsEven())
                        || (!i.IsEven() && j.IsEven()))
                    {
                        _darkCells.Add(new Point(i, j));
                    }
                }
            }
        }

        /// <summary>
        /// Returns standard pieces set for new game
        /// </summary>
        /// <returns></returns>
        private ObservableCollection<CellData> GetPiecesForNewGame()
        {
            ObservableCollection<CellData> pieces = new ObservableCollection<CellData>();

            //white pieces
            foreach (var pos in _darkCells.Where(x => x.Y >= 5))
            {
                pieces.Add(new CellData(pos, PieceType.Man, Player.White));
            }

            //dark pieces
            foreach (var pos in _darkCells.Where(x => x.Y <= 2))
            {
                pieces.Add(new CellData(pos, PieceType.Man, Player.Black));
            }

            //board letters
            for (int i = 0; i < Globals.BoardWidth; i++)
            {
                Point letterPoint = new Point(i, Globals.BoardHeight);
                pieces.Add(new CellData(letterPoint, Colors.AntiqueWhite, letterPoint.GetXAxisName()));
            }

            //board numbers
            for (int i = 0; i < Globals.BoardHeight; i++)
            {
                Point numberPoint = new Point(Globals.BoardWidth, i);
                pieces.Add(new CellData(numberPoint, Colors.AntiqueWhite, numberPoint.GetYAxisName()));
            }

            //empty cell between numbers and letters
            pieces.Add(new CellData(new Point(Globals.BoardWidth, Globals.BoardHeight), Colors.AntiqueWhite, null));

            return pieces;
        }

        /// <summary>
        /// Adds piece to the board (when dragged from options area)
        /// </summary>
        /// <param name="piece"></param>
        public void AddPiece(CellData piece)
        {
            if (IsValidArrangeMove(piece.Pos))
            {
                Pieces.Add(piece);
                piece.CompleteMove();
                UpdateCapturesList();
            }
        }
        /// <summary>
        /// Returns piece by coordinates
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private CellData GetPiece(Point point)
        {
            return Pieces.Single(x => x.OriginalPos.Equals(point) && !x.IsHighlight);
        }

        /// <summary>
        /// Returns Specific Player's pieces
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private List<CellData> GetPlayerPieces(Player player)
        {
            return Pieces.Where(x => x.Player == player).ToList();
        }

        /// <summary>
        /// Updates capture options
        /// </summary>
        private void UpdateCapturesList()
        {
            ClearHighlights();
            BestCaptures.Clear();

            //analyse white only according to task
            List<CellData> playerPieces = GetPlayerPieces(Player.White);

            foreach (var playerPiece in playerPieces)
            {
                List<Point> taken = new List<Point>();
                List<Point> pieceAvailableCaptures = new List<Point> { playerPiece.Pos };

                GetPieceDirectCaptureCellsRecursive(
                    pieceAvailableCaptures,
                    playerPiece.Pos,
                    playerPiece.Type == PieceType.Man,
                    Player.Black,
                    ref taken,
                    playerPiece.Pos);
            }
        }

        /// <summary>
        /// Should a piece at the given point become King
        /// </summary>
        /// <param name="point">Piece Coordinate</param>
        /// <param name="opponent">Opponent (used to find out last line of the board)</param>
        /// <returns></returns>
        private bool ShouldCoronate(Point point, Player opponent)
        {
            return (point.Y == 0 && opponent == Player.Black) || (point.Y == Globals.BoardHeight - 1 && opponent == Player.White);
        }

        /// <summary>
        /// Recursive function to check for available captures
        /// </summary>
        /// <param name="pieceAvailableCaptures">Available capture option</param>
        /// <param name="pos">Current piece position</param>
        /// <param name="isMan">Is Piece Man or King</param>
        /// <param name="opponent">Opponent</param>
        /// <param name="taken">Already 'supposedly' taken pieces</param>
        /// <param name="originalPosition">Original piece position</param>
        private void GetPieceDirectCaptureCellsRecursive(List<Point> pieceAvailableCaptures,
            Point pos, bool isMan, Player opponent, ref List<Point> taken, Point originalPosition)
        {
            if (ShouldCoronate(pos, opponent))
                //let's do a Coronation
                isMan = false;

            List<List<Point>> diagonales = GetDiagonales(pos);

            foreach (var diagonale in diagonales)
            {
                if (isMan)
                {
                    Point? endPoint;
                    Point? takenPoint;
                    if (CanManCaptureDiagonale(diagonale, opponent, out endPoint, out takenPoint, taken, originalPosition))
                    {
                        if (endPoint != null && takenPoint != null)
                        {
                            List<Point> newPieceAvailableCaptures = new List<Point>(pieceAvailableCaptures) { endPoint.Value };

                            taken.Add(takenPoint.Value);

                            BestCaptures.Add(new CaptureData { Points = newPieceAvailableCaptures, TakenPoints = taken });
                            BestCaptures.Sort();

                            GetPieceDirectCaptureCellsRecursive(newPieceAvailableCaptures, endPoint.Value, isMan, opponent, ref taken, originalPosition);
                        }
                    }
                }
                else
                {
                    List<Point> endPoints;
                    Point? takenPoint;
                    if (CanKingCaptureDiagonale(diagonale, opponent, out endPoints, out takenPoint, taken, originalPosition))
                    {
                        foreach (var endPoint in endPoints)
                        {
                            List<Point> newPieceAvailableCaptures = new List<Point>(pieceAvailableCaptures) { endPoint };

                            if (takenPoint != null) taken.Add(takenPoint.Value);

                            BestCaptures.Add(new CaptureData { Points = newPieceAvailableCaptures, TakenPoints = taken });
                            BestCaptures.Sort();


                            GetPieceDirectCaptureCellsRecursive(newPieceAvailableCaptures, endPoint, isMan, opponent, ref taken, originalPosition);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Finds first available piece on diagonal
        /// </summary>
        /// <param name="diagonaleCells">Diagonal cells</param>
        /// <param name="diagonalIndex">Index of first found piece</param>
        /// <param name="cellData">First found piece</param>
        /// <returns></returns>
        private bool IsPieceFoundOnDiagonal(List<Point> diagonaleCells, out int diagonalIndex, out CellData cellData)
        {
            diagonalIndex = 0;
            cellData = null;

            for (int index = 0; index < diagonaleCells.Count; index++)
            {
                Point t = diagonaleCells[index];
                CellData data = Pieces.SingleOrDefault(x => x.Pos.Equals(t) && !x.IsHighlight);
                if (data != null)
                {
                    diagonalIndex = index;
                    cellData = data;

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if King can find a capture on the given diagonal
        /// </summary>
        /// <param name="diagonaleCells"></param>
        /// <param name="opponent"></param>
        /// <param name="endPoints"></param>
        /// <param name="takenPoint"></param>
        /// <param name="taken"></param>
        /// <param name="originalPosition"></param>
        /// <returns></returns>
        private bool CanKingCaptureDiagonale(List<Point> diagonaleCells, Player opponent,
            out List<Point> endPoints, out Point? takenPoint,
            List<Point> taken, Point originalPosition)
        {
            endPoints = new List<Point>();
            takenPoint = null;
            bool anyCaputeExist = false;
            bool nextPieceReached = false;
            bool diagonalEndReached = false;
            int moveEndPosition = 1;

            int diagonalIndex;
            CellData cellData;
            if (IsPieceFoundOnDiagonal(diagonaleCells, out diagonalIndex, out cellData))
            {
                moveEndPosition = diagonalIndex + 1;

                if (moveEndPosition < diagonaleCells.Count)
                    do
                    {

                        bool nearestIsOpponent = cellData.Player == opponent;

                        bool nextIsEmpty = !Pieces.Any(x => x.Pos.Equals(diagonaleCells[moveEndPosition]) && !x.IsHighlight) ||
                                           (diagonaleCells[moveEndPosition].Equals(originalPosition));
                        //original position shuold be empty at the time of move;
                        bool isNotTakenYet = !taken.Contains(diagonaleCells[0]);

                        diagonalEndReached = moveEndPosition == diagonaleCells.Count - 1;
                        nextPieceReached = !nextIsEmpty;

                        bool can = false;

                        if (nearestIsOpponent && nextIsEmpty && isNotTakenYet)
                        {
                            anyCaputeExist = true;
                            can = true;
                        }

                        if (can)
                        {
                            endPoints.Add(diagonaleCells[moveEndPosition]);
                            takenPoint = cellData.Pos;
                        }

                        moveEndPosition++;


                    } while (!diagonalEndReached && !nextPieceReached);
            }
            return anyCaputeExist;
        }

        /// <summary>
        /// Checks if Man can find a capture on the given diagonal
        /// </summary>
        /// <param name="diagonaleCells"></param>
        /// <param name="opponent"></param>
        /// <param name="endPoint"></param>
        /// <param name="takenPoint"></param>
        /// <param name="taken"></param>
        /// <param name="originalPosition"></param>
        /// <returns></returns>
        private bool CanManCaptureDiagonale(List<Point> diagonaleCells, Player opponent,
            out Point? endPoint, out Point? takenPoint,
            List<Point> taken, Point originalPosition)
        {
            endPoint = null;
            takenPoint = null;

            //nearest is opponent and next is free
            bool nearestIsOpponent = Pieces.Any(x => x.Pos.Equals(diagonaleCells[0]) && !x.IsHighlight && x.Player == opponent);
            bool nextIsEmpty = !Pieces.Any(x => x.Pos.Equals(diagonaleCells[1]) && !x.IsHighlight) ||
            (diagonaleCells[1].Equals(originalPosition));//original position shuold be empty at the time of move

            bool isNotTakenYet = !taken.Contains(diagonaleCells[0]);

            bool can = nearestIsOpponent && nextIsEmpty && isNotTakenYet;

            if (can)
            {
                endPoint = diagonaleCells[1];
                takenPoint = diagonaleCells[0];
            }

            return can;
        }

        /// <summary>
        /// Returns list of theoretically available for capture diagonales
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private List<List<Point>> GetDiagonales(Point pos)
        {
            List<List<Point>> diagonales = new List<List<Point>>();

            List<Point> rightBottomDiag = GetDiagonale(pos, 1, 1);
            List<Point> rightTopDiag = GetDiagonale(pos, 1, -1);
            List<Point> leftBottomDiag = GetDiagonale(pos, -1, 1);
            List<Point> leftTopDiag = GetDiagonale(pos, -1, -1);

            if (rightBottomDiag.Count > 1) //potential capture - at least 2 cells
                diagonales.Add(rightBottomDiag);

            if (rightTopDiag.Count > 1) //potential capture - at least 2 cells
                diagonales.Add(rightTopDiag);

            if (leftBottomDiag.Count > 1) //potential capture - at least 2 cells
                diagonales.Add(leftBottomDiag);

            if (leftTopDiag.Count > 1) //potential capture - at least 2 cells
                diagonales.Add(leftTopDiag);

            return diagonales;
        }

        /// <summary>
        /// Returns diagonal cells
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="xIncrement"></param>
        /// <param name="yIncrement"></param>
        /// <returns></returns>
        private List<Point> GetDiagonale(Point pos, int xIncrement, int yIncrement)
        {
            List<Point> diagonaleCells = new List<Point>();
            bool cellFound;
            do
            {
                pos.X += xIncrement;
                pos.Y += yIncrement;
                cellFound = _darkCells.Any(x => x.X == pos.X & x.Y == pos.Y);

                if (cellFound) diagonaleCells.Add(pos);

            } while (cellFound);

            return diagonaleCells;
        }
        /// <summary>
        /// Removes draught piece at given coordinates
        /// </summary>
        /// <param name="point"></param>
        private void RemovePiece(Point point)
        {
            var piece = Pieces.Single(x => x.OriginalPos.Equals(point) && !x.IsHighlight);
            Pieces.Remove(piece);
        }

        /// <summary>
        /// Checks if the place is valid to move piece
        /// </summary>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        private bool IsValidArrangeMove(Point endPoint)
        {
            //only dark cells
            if (!_darkCells.Contains(endPoint))
                return false;

            //only free cells
            if (Pieces.Any(x => x.Pos.Equals(endPoint) && !x.IsHighlight))
                return false;


            return true;
        }



        private bool IsValidPlayMove(Point startPoint, Point endPoint, CellData activePiece)
        {
            throw new NotImplementedException("IsValidPlayMove not implemented yet");

            //only dark cells
            if (!_darkCells.Contains(endPoint))
                return false;

            //only free cells
            if (Pieces.Any(x => x.Pos.Equals(endPoint) && !x.IsHighlight))
                return false;

            //men only move forward
            if (activePiece.Type == PieceType.Man)
            {
                //white - up
                if (activePiece.Player == Player.White)
                {
                    if (startPoint.Y <= endPoint.Y)
                        return false;
                }
                //black - down
                else
                {
                    if (startPoint.Y >= endPoint.Y)
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Swithces piece type from man to king and vice versa
        /// </summary>
        /// <param name="pos"></param>
        public void SwitchPieceType(Point pos)
        {
            CellData piece = GetPiece(pos);
            piece.Type = piece.Type == PieceType.Man ? PieceType.King : PieceType.Man;
            UpdateCapturesList();
        }

        /// <summary>
        /// Moves piece
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        public void MovePiece(Point startPoint, Point endPoint)
        {
            CellData activePiece = GetPiece(startPoint);
            
            if (IsValidArrangeMove(endPoint)) //if move is valid
            {
                activePiece.Pos = endPoint;
                activePiece.CompleteMove();

                if (ShouldCoronate(activePiece.OriginalPos, activePiece.Player.Value == Player.White ? Player.Black : Player.White))
                    //let's do a Coronation
                    activePiece.Type = PieceType.King;

                UpdateCapturesList();
            }
            else
            {
                activePiece.CancelMove();
            }
        }

        /// <summary>
        /// Performs the opponent pieces capture
        /// </summary>
        /// <param name="captureData"></param>
        public void Capture(CaptureData captureData)
        {
            if (captureData == null)
                return;

            //move piece to end position
            CellData activePiece = GetPiece(captureData.Points[0]);
            activePiece.Pos = captureData.Points.Last();
            activePiece.CompleteMove();

            foreach (var point in captureData.Points)
            {
                if (ShouldCoronate(point, Player.Black))
                    //let's do a Coronation
                    activePiece.Type = PieceType.King;
            }

            //remove captured pieces
            foreach (var takenPoint in captureData.TakenPoints)
            {
                RemovePiece(takenPoint);
            }

            UpdateCapturesList();
        }

        /// <summary>
        /// Adds highlight
        /// </summary>
        /// <param name="points"></param>
        private void AddHighlight(List<Point> points)
        {
            foreach (var point in points)
            {
                //add highlights to the beginning of the list so they do not overlap with pieces
                Pieces.Insert(0, new CellData(point, Colors.GreenYellow, null, 0.6));
            }
        }

        /// <summary>
        /// Clears all highlights
        /// </summary>
        private void ClearHighlights()
        {
            var highlights = Pieces.Where(x => x.OriginalPos.Y < Globals.BoardHeight && x.OriginalPos.X < Globals.BoardWidth && x.IsHighlight);
            foreach (var highlight in highlights.ToList())
            {
                Pieces.Remove(highlight);
            }
        }

        /// <summary>
        /// Highlights potential capture
        /// </summary>
        /// <param name="captureData"></param>
        public void HighlightCapture(CaptureData captureData)
        {
            ClearHighlights();
            if (captureData != null)
                AddHighlight(captureData.Points);

        }
    }
}
