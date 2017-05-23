using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using GalaSoft.MvvmLight;

namespace Draughts
{
    /// <summary>
    /// Capture option
    /// </summary>
    public class CaptureData : ViewModelBase, IComparable
    {
        /// <summary>
        /// User-friendly display
        /// </summary>
        public string Text
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (Points.Any())
                {
                    sb.Append(Points[0].GetXAxisName());
                    sb.Append(Points[0].GetYAxisName());

                    for (int i = 1; i < Points.Count; i++)
                    {
                        sb.Append(" -> ");
                        sb.Append(Points[i].GetXAxisName());
                        sb.Append(Points[i].GetYAxisName());
                    }
                }

                return sb.ToString();
            }

        }

        /// <summary>
        /// Points to be moved on
        /// </summary>
        public List<Point> Points { get; set; }

        /// <summary>
        /// Points to be taken
        /// </summary>
        public List<Point> TakenPoints { get; set; } 

        /// <summary>
        /// Compare with other Capture options.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public int CompareTo(object o)
        {
            CaptureData a = this;
            CaptureData b = (CaptureData)o;
            return b.Points.Count - a.Points.Count;
        }
    }
}