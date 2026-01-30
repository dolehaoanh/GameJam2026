using System.Collections.Generic;
using UnityEngine;

namespace AC.Core
{
    public static class ListExtension 
    {
        public static List<T> Shuffle<T>(this IList<T> list)
        {
            var tmpList = new List<T>();
            tmpList.AddRange(list);
            int n = tmpList.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                T value = tmpList[k];
                tmpList[k] = tmpList[n];
                tmpList[n] = value;
            }
            return tmpList;
        }
    }
}

