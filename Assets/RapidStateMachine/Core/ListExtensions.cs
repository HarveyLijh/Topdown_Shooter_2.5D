using System.Collections.Generic;
using UnityEngine;

namespace RSM
{
    public static class ListExtensions
    {
        public static bool ExistsAndContains<T>(this List<T> thisList, T filter)
        {
            if (thisList == null) return false;
            if (thisList.Count == 0) return false;
            return thisList.Contains(filter);
        }

        public static T GetRandom<T>(this List<T> thisList)
            => thisList[Random.Range(0, thisList.Count)];

        public static void MoveTo<T>(this List<T> thisList, int newIndex, T element)
        {
            if (thisList == null) return;
            if (thisList.Count == 0) return;
            if (!thisList.Contains(element)) return;

            T storedElement = element;
            thisList.Remove(element);
            thisList.Insert(newIndex, storedElement);
        }

        public static void Shift<T>(this List<T> thisList, int shiftAmount, T element)
        {
            if (thisList == null) return;
            if (thisList.Count == 0) return;
            if (!thisList.Contains(element)) return;
            int elementIndex = thisList.IndexOf(element);
            int newIndex = elementIndex + shiftAmount;
            if (newIndex < 0 || newIndex > thisList.Count) return;
            thisList.MoveTo(newIndex, element);
        }
    }
}