using System.Collections.Generic;
using UnityEngine;

namespace System.Collections.GenericEx
{
    public static class ListEx
    {
        public static void Shuffle(this IList<int> array, int first, int count)
        {
            for (int n = count; n > 1;)
            {
                int k = UnityEngine.Random.Range(0, count);
                --n;
                int temp = array[first+n];
                array[first+n] = array[first+k];
                array[first+k] = temp;
            }
        }
        
        public static IList<T> Swap<T>(this IList<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
            return list;
        }

        public static List<string> CreateStringIndxList(int beginvalue, int endvalue)
        {
            List<string> list = new List<string>();
            for(int i= beginvalue; i<=endvalue; ++i)
                list.Add(i.ToString());
            return list;
        }

        public static List<string> CreateStringIndxSqrList(int endvalue)
        {
            List<string> list = new List<string>();
            int sqr = 2;
            for (int i = 1; i <= endvalue; ++i)
            {
                sqr += 7;
                list.Add(sqr.ToString());
            }
            return list;
        }

        public static void CopyOf<T>(this List<T> t, List<T> t2) where T : class
        {
            t.Clear();
            for (int i = 0; i < t2.Count; i++)
                t.Add(t2[i]);
        }
    }

    public static class QueueEx
    {
        #region Int
        public static void Remove(this Queue<int> q, int v_)
        {
            int num = q.Count;
            for (int i = 0; i < num; ++i)
            {
                int v = q.Dequeue();
                if (v == v_)
                    return;
                q.Enqueue(v);
            }
        }

        public static bool Has(this Queue<int> q, int v_)
        {
            int num = q.Count;
            for (int i = 0; i < num; ++i)
            {
                int v = q.Dequeue();
                q.Enqueue(v);
                if (v == v_) return true;
            }
            return false;
        }

        public static void Enque_NoDupli(this Queue<int> q, int v_)
        {
            int num = q.Count;
            for (int i = 0; i < num; ++i)
            {
                int v = q.Dequeue();
                q.Enqueue(v);
                if (v == v_)
                    return;
            }
            q.Enqueue(v_);
        }

        public static void Enque_NoDupli(this Queue<short> q, short v_)
        {
            int num = q.Count;
            for (int i = 0; i < num; ++i)
            {
                short v = q.Dequeue();
                q.Enqueue(v);
                if (v == v_)
                    return;
            }
            q.Enqueue(v_);
        }

        public static int DequeAndEnque(this Queue<int> q)
        {
            int v = q.Dequeue();
            q.Enqueue(v);
            return v;
        }

        public static void Enque(this Queue<int> q, ref List<int> lst_)
        {
            int num = lst_.Count;
            for (int i = 0; i < num; ++i)
                q.Enqueue(lst_[i]);
        }
        #endregion

        public static void ClearExceptOne(this Queue<i2> queue, int value)
        {
            int num = queue.Count;
            for (int i = 0; i < num; ++i)
            {
                i2 existValue = queue.Dequeue();
                if (existValue.v2 == value)
                    queue.Enqueue(existValue);
            }
        }

        public static void ClearExceptOne(this Queue<short> queue, short value)
        {
            int num = queue.Count;
            for (int i = 0; i < num; ++i)
            {
                short existValue = queue.Dequeue();
                if (existValue == value)
                    queue.Enqueue(existValue);
            }
        }

        public static short DequeAndEnque(this Queue<short> queue)
        {
            short cdx = queue.Dequeue();
            queue.Enqueue(cdx);
            return cdx;
        }

        public static void Deq(this Queue<int> queue, int value)
        {
            int num = queue.Count;
            for (int i = 0; i < num; ++i)
            {
                int existValue = queue.Dequeue();
                if (existValue == value)
                    return;
                queue.Enqueue(existValue);
            }
        }

        public static bool DequeueOne(this Queue<int> queue, int value)
        {
            int num = queue.Count;
            for (int i = 0; i < num; ++i)
            {
                int existValue = queue.Dequeue();
                if (existValue == value)
                    return true;
                queue.Enqueue(existValue);
            }
            return false;
        }

        public static void Deq(this Queue<short> queue, short value)
        {
            int num = queue.Count;
            for (int i = 0; i < num; ++i)
            {
                short existValue = queue.Dequeue();
                if (existValue == value)
                    return;
                queue.Enqueue(existValue);
            }
        }

        public static bool DequeueOne(this Queue<short> queue, short value)
        {
            int num = queue.Count;
            for (int i = 0; i < num; ++i)
            {
                short existValue = queue.Dequeue();
                if (existValue == value)
                    return true;
                queue.Enqueue(existValue);
            }
            return false;
        }

        public static bool DequeuesAfterOne(this Queue<short> queue, short value, bool removeValue = true)
        {
            bool found = false;
            int num = queue.Count;
            for (int i = 0; i < num; ++i)
            {
                short existValue = queue.Dequeue();
                if (found)
                    continue;
                if (existValue == value)
                {
                    found = true;
                    if (removeValue)
                        continue;
                }
                queue.Enqueue(existValue);
            }
            return found;
        }
    }

    public static class HashTableEx
    {
        public static T Value<T>(this Hashtable hashTable) where T : struct
        {
            T a = new T();
            return a;
        }

    }

    public static class DicEx
    {

        public static void AddOrReplace<T,U>(this Dictionary<T,U> dic, T key, U value) 
            where T : struct
        {
            if (dic.ContainsKey(key))
                dic[key] = value;
            else
                dic.Add(key, value);
        }
        
        public static void RemoveContain<T, U>(this Dictionary<T, U> dic, T key)
            where T : struct
        {
            if (dic.ContainsKey(key))
                dic.Remove(key);
        }


    }
}
