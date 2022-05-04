using UnityEngine;
using System.Runtime.CompilerServices;

namespace UnityEngineEx
{
    public static class ParticleSystemEx
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBeginColor(this ParticleSystem ps, Color color)
        {
            var mainModule = ps.main;
            mainModule.startColor = color;
        }
    }
}