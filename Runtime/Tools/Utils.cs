//@vadym udod

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace hootybird.Tools
{
    /// <summary>
    /// Utilieis class
    /// </summary>
    public static class Utils
    {
        private static System.Random rng = new System.Random();

        public static int ElementIndex<T>(this T[] array, T element)
        {
            for (int i = 0; i < array.Length; i++)
                if (array[i].Equals(element))
                    return i;

            return -1;
        }

        /// <summary>
        /// String to enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        //https://stackoverflow.com/questions/16100/how-should-i-convert-a-string-to-an-enum-in-c
        public static T ToEnum<T>(this string value) => (T)Enum.Parse(typeof(T), value, true);

        /// <summary>
        /// Angle to vector
        /// </summary>
        /// <param name="vec1"></param>
        /// <param name="vec2"></param>
        /// <returns></returns>
        public static float AngleTo(this Vector2 vec1, Vector2 vec2) => Mathf.DeltaAngle(Mathf.Atan2(vec1.y, vec1.x) * Mathf.Rad2Deg, Mathf.Atan2(vec2.y, vec2.x) * Mathf.Rad2Deg);

        public static Vector2 VectorFromAngle(this float angle) => new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle));

        public static T Random<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null) return default(T);

            var list = enumerable as IList<T> ?? enumerable.ToList();
            return list.Count == 0 ? default(T) : list[UnityEngine.Random.Range(0, list.Count)];
        }

        public static T Next<T>(this IEnumerable<T> enumerable, T current)
        {
            if (enumerable == null) return default(T);

            var list = enumerable as IList<T> ?? enumerable.ToList();

            if (list.Count == 0)
                return default(T);
            else
            {
                if (list.IndexOf(current) == list.Count - 1)
                    return list[0];
                else
                    return list[list.IndexOf(current) + 1];
            }
        }

        public static IList<T> InsertList<T>(this IEnumerable<T> enumerable, int index, IEnumerable<T> toAdd)
        {
            if (enumerable == null) return null;

            var list = enumerable as IList<T> ?? enumerable.ToList();
            var toAddList = toAdd as IList<T> ?? toAdd.ToList();

            for (int _index = index; _index < index + toAddList.Count; _index++)
                list.Insert(_index, toAddList[_index - index]);

            return list;
        }

        /// <summary>
        /// Add element to array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T[] AddElementToEnd<T>(this T[] values, T value)
        {
            T[] result = new T[values.Length + 1];

            Array.Copy(values, result, values.Length);
            result[values.Length] = value;

            return result;
        }

        /// <summary>
        /// Add element to array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T[] AddElementToStart<T>(this T[] values, T value)
        {
            T[] result = new T[values.Length + 1];

            Array.Copy(values, 0, result, 1, values.Length);
            result[0] = value;

            return result;
        }

        /// <summary>
        /// Removes last element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static T[] RemoveElement<T>(this T[] values)
        {
            T[] result = new T[values.Length - 1];

            Array.Copy(values, result, result.Length);

            return result;
        }

        /// <summary>
        /// List shuffle
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void MoveItem<T>(this List<T> collection, int oldIndex, int newIndex)
        {
            T removedItem = collection[oldIndex];

            collection.RemoveAt(oldIndex);
            collection.Insert(newIndex, removedItem);
        }

        public static bool IsSet<T>(T flags, T flag) where T : struct
        {
            int flagsValue = (int)(object)flags;
            int flagValue = (int)(object)flag;

            return (flagsValue & flagValue) != 0;
        }

        public static AnimationCurve CreateStraightCurve()
        {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0, 1));
            curve.AddKey(new Keyframe(1, 1));
            return curve;
        }

        public static AnimationCurve CreateLinearCurve()
        {
            float tan45 = Mathf.Tan(Mathf.Deg2Rad * 45);

            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0, 0, tan45, tan45));
            curve.AddKey(new Keyframe(1, 1, tan45, tan45));
            return curve;
        }

        public static AnimationCurve CreateEaseInEaseOutCurve()
        {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0, 0, 0, 0));
            curve.AddKey(new Keyframe(1, 1, 0, 0));
            return curve;
        }

        public static AnimationCurve CreateSteepCurve()
        {
            float tan45 = Mathf.Tan(Mathf.Deg2Rad * 45);

            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0, 0, 0, 0));
            curve.AddKey(new Keyframe(1, 1, tan45, tan45));
            return curve;
        }
    }

    public class StackModified<T>
    {
        private List<T> items = new List<T>();

        public int Count => items.Count;

        public T Pop()
        {
            if (items.Count > 0)
            {
                T temp = items[items.Count - 1];
                items.RemoveAt(items.Count - 1);
                return temp;
            }
            else
                return default(T);
        }

        public T Peek()
        {
            if (items.Count > 0)
                return items[items.Count - 1];
            else
                return default(T);
        }

        public void Push(T item) => items.Add(item);

        public void RemoveAt(int itemAtPosition) => items.RemoveAt(itemAtPosition);

        public void Remove(T element) => items.Remove(element);

        public bool Contains(T element) => items.Contains(element);
    }
}