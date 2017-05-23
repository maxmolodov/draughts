using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Draughts
{
    public static class Ext
    {
        /// <summary>
        /// Used to find out dark/light cells on the board
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool IsEven(this int number)
        {
            return number%2 == 0;
        }

        /// <summary>
        /// Returns user-friendly point axis name
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static string GetXAxisName(this Point point)
        {
            return Utils.Number2String((int)Math.Round(point.X+1), true);
        }

        /// <summary>
        /// Returns user-friendly point axis name
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static string GetYAxisName(this Point point)
        {
            return (Globals.BoardHeight - point.Y).ToString();
        }

        /// <summary>
        /// Used to order capture options by value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        public static void Sort<T>(this ObservableCollection<T> collection) where T : IComparable
        {
            List<T> sorted = collection.OrderBy(x => x).ToList();
            for (int i = 0; i < sorted.Count(); i++)
                collection.Move(collection.IndexOf(sorted[i]), i);
        }
    }
}
