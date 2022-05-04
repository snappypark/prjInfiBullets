using UnityEngine;
using System.Runtime.CompilerServices;

namespace UnityEngineEx
{
    public static class GamObjEx
    {
        public static GameObject Create(string name, Transform parent)
        {
            GameObject go = new GameObject(name + "(Clone)");
            go.transform.SetParent(parent);
            return go;
        }

        public static T Create<T>(string name, Transform parent) where T : Component
        {
            GameObject go = new GameObject(name + "(Clone)");
            go.transform.SetParent(parent);
            return go.AddComponent<T>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Create<T>(byte preload_idx, Transform preloads, Transform parent) where T : Component
        {
            return GameObject.Instantiate<T>(
               preloads.GetChild<T>(preload_idx),
               VectorEx.Huge, Quaternion.identity, parent);
        }
        
        public static void DestroyChildren(Transform rootTrans, int beginIdx = 0)
        {
            int numObj = rootTrans.childCount;
            for (int i = numObj-1; i > -1+ beginIdx; --i)
                GameObject.Destroy(rootTrans.GetChild(i).gameObject);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Active(this MonoBehaviour monoBevahiour)
        {
            monoBevahiour.gameObject.SetActive(true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Inactive(this MonoBehaviour monoBevahiour)
        {
            monoBevahiour.gameObject.SetActive(false);
        }
    }
}