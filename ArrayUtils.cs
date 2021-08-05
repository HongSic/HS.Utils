using System.Collections.Generic;

namespace HS.Utils
{
    public static class ArrayUtils
    {
        public static void PushAll<T>(this Stack<T> Stack, params T[] Params)
        {
            if (Params == null || Params.Length == 0) return;
            for (int i = 0; i < Params.Length; i++) Stack.Push(Params[i]);
        }
        public static void EnqueueAll<T>(this Queue<T> Queue, params T[] Params)
        {
            if (Params == null || Params.Length == 0) return;
            for (int i = 0; i < Params.Length; i++) Queue.Enqueue(Params[i]);
        }
    }
}
