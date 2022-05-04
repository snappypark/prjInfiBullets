using System;
using UnityEngine;

namespace UnityEngineEx
{
    public static class RandEx
    {
        public static void InitSeedWithTick()
        {
            int ticks = (int)System.DateTime.Now.Ticks % 1010101010;
            UnityEngine.Random.InitState(ticks >> 13);
        }

        public static bool TruePerCt(int percent) { return UnityEngine.Random.Range(1, 101) <= percent; }

        public static int GetN(int begin, int n) { return UnityEngine.Random.Range(begin, n); }
        public static int GetN(int n) { return UnityEngine.Random.Range(0, n); }
        public static int Get01() { return UnityEngine.Random.Range(0, 2); }
        public static int Get012() { return UnityEngine.Random.Range(0, 3); }
        public static int Get0123() { return UnityEngine.Random.Range(0, 4); }
        public static int Get01234() { return UnityEngine.Random.Range(0, 5); }

        public static int Get(params int[] nums)
        {
            return nums[UnityEngine.Random.Range(0, nums.Length)];
        }

        public static int GetHalfIdx(int value)
        {
            return ((value + 1 + Get01()) / 2) - 1;
        }

        public static bool IsTrueOrNot()
        {
            return UnityEngine.Random.Range(0, 2) == 0;
        }

        public static bool IsTrue_Probability(int chance, int max = 100)
        {
            return UnityEngine.Random.Range(0, max) < chance;
        }

        public static Vector2 GetBoundGapPos()
        {
            int random = UnityEngine.Random.Range(0, 4);
            switch (random)
            {
                case 0: return new Vector2(UnityEngine.Random.Range(-0.5f, 0.5f), -0.5f);
                case 1: return new Vector2(UnityEngine.Random.Range(-0.5f, 0.5f), 0.5f);
                case 2: return new Vector2(-0.5f, UnityEngine.Random.Range(-0.5f, 0.5f));
                default: return new Vector2(0.5f, UnityEngine.Random.Range(-0.5f, 0.5f));
            }
        }

        public static void GetCell3x3(int x, int y, out int oX, out int oZ)
        {
            int rand = UnityEngine.Random.Range(0, 9);
            switch (rand)
            {
                case 0: oX = x - 1; oZ = y - 1; return;
                case 1: oX = x; oZ = y - 1; return;
                case 2: oX = x + 1; oZ = y - 1; return;
                case 3: oX = x - 1; oZ = y; return;
                case 4: oX = x; oZ = y; return;
                case 5: oX = x + 1; oZ = y; return;
                case 6: oX = x - 1; oZ = y + 1; return;
                case 7: oX = x; oZ = y + 1; return;
                case 8: oX = x + 1; oZ = y + 1; return;
                default: oX = x; oZ = y; return;
            }
        }

        public static void GetCell5x5(int x, int y, out int oX, out int oZ)
        {
            int rand = UnityEngine.Random.Range(0, 25);
            switch (rand)
            {
                case 0: oX = x - 1; oZ = y - 1; return;
                case 1: oX = x; oZ = y - 1; return;
                case 2: oX = x + 1; oZ = y - 1; return;
                case 3: oX = x - 1; oZ = y; return;
                case 4: oX = x; oZ = y; return;
                case 5: oX = x + 1; oZ = y; return;
                case 6: oX = x - 1; oZ = y + 1; return;
                case 7: oX = x; oZ = y + 1; return;
                case 8: oX = x + 1; oZ = y + 1; return;

                case 9: oX = x - 2; oZ = y - 2; return;
                case 10: oX = x - 1; oZ = y - 2; return;
                case 11: oX = x; oZ = y - 2; return;
                case 12: oX = x + 1; oZ = y - 2; return;
                case 13: oX = x + 2; oZ = y - 2; return;

                case 14: oX = x - 2; oZ = y + 2; return;
                case 15: oX = x - 1; oZ = y + 2; return;
                case 16: oX = x; oZ = y + 2; return;
                case 17: oX = x + 1; oZ = y + 2; return;
                case 18: oX = x + 2; oZ = y + 2; return;

                case 19: oX = x - 2; oZ = y - 1; return;
                case 20: oX = x - 2; oZ = y; return;
                case 21: oX = x - 2; oZ = y + 1; return;
                case 22: oX = x + 2; oZ = y - 1; return;
                case 23: oX = x + 2; oZ = y; return;
                case 24: oX = x + 2; oZ = y + 1; return;

                default: oX = x; oZ = y; return;
            }
        }

        public static void GetCell5x5_NoCorner(int x, int y, out int oX, out int oZ)
        {
            int rand = UnityEngine.Random.Range(0, 21);
            switch (rand)
            {
                case 0: oX = x - 1; oZ = y - 1; return;
                case 1: oX = x; oZ = y - 1; return;
                case 2: oX = x + 1; oZ = y - 1; return;
                case 3: oX = x - 1; oZ = y; return;
                case 4: oX = x; oZ = y; return;
                case 5: oX = x + 1; oZ = y; return;
                case 6: oX = x - 1; oZ = y + 1; return;
                case 7: oX = x; oZ = y + 1; return;
                case 8: oX = x + 1; oZ = y + 1; return;

                //case 9:  oX = x - 2; oZ = y - 2; return;
                case 9: oX = x - 1; oZ = y - 2; return;
                case 10: oX = x; oZ = y - 2; return;
                case 11: oX = x + 1; oZ = y - 2; return;
                //case 13: oX = x + 2; oZ = y - 2; return;

                //case 14: oX = x - 2; oZ = y + 2; return;
                case 12: oX = x - 1; oZ = y + 2; return;
                case 13: oX = x; oZ = y + 2; return;
                case 14: oX = x + 1; oZ = y + 2; return;
                //case 18: oX = x + 2; oZ = y + 2; return;

                case 15: oX = x - 2; oZ = y - 1; return;
                case 16: oX = x - 2; oZ = y; return;
                case 17: oX = x - 2; oZ = y + 1; return;
                case 18: oX = x + 2; oZ = y - 1; return;
                case 19: oX = x + 2; oZ = y; return;
                case 20: oX = x + 2; oZ = y + 1; return;

                default: oX = x; oZ = y; return;
            }
        }



        public static void GetPt_OnCorner_Randomly(int x0, int z0, int szR, int szC, out int oX, out int oZ)
        {
            int rand = UnityEngine.Random.Range(0, 4);
            switch (rand)
            {
                case 0: oX = x0 + szR -1; oZ = z0; return;
                case 1: oX = x0; oZ = z0 + szC - 1; return;
                case 2: oX = x0 + szR - 1; oZ = z0 + szC - 1; return;
                default: oX = x0; oZ = z0; return;
            }
        }

        public static T RandomEnumValue<T>()
        {
            var values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(UnityEngine.Random.Range(0, values.Length));
        }


        public static Vector3 Dir8()
        {
            switch (UnityEngine.Random.Range(0, 8))
            {
                case 0: return new Vector3(0, 0, 1);
                case 1: return new Vector3(0, 0, -1);
                case 2: return new Vector3(1, 0, 0);
                case 3: return new Vector3(-1, 0, 0);

                case 4: return new Vector3(1, 0, 1).normalized;
                case 5: return new Vector3(-1, 0, -1).normalized;
                case 6: return new Vector3(1, 0, -1).normalized;
                case 7: return new Vector3(-1, 0, 1).normalized;
            }
            return Vector3.forward;
        }
    }
}

