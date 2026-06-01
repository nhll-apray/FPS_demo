using System.Collections.Generic;
using UnityEngine;

namespace FpsDemo.Game
{
    public static class GameResources
    {
        private const string Root = "FpsDemo/";
        private static readonly Dictionary<string, Object> Cache = new Dictionary<string, Object>();

        public static T Load<T>(string path) where T : Object
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            string fullPath = (Root + path).Replace('\\', '/').Trim('/');
            string cacheKey = $"{typeof(T).FullName}:{fullPath}";
            if (Cache.TryGetValue(cacheKey, out Object cachedAsset))
            {
                return cachedAsset as T;
            }

            T asset = Resources.Load<T>(fullPath);
            if (asset == null)
            {
                return null;
            }

            Cache[cacheKey] = asset;
            return asset;
        }

        public static GameObject LoadPrefab(string path)
        {
            return Load<GameObject>("Prefabs/" + path);
        }

        public static T LoadData<T>(string path) where T : ScriptableObject
        {
            return Load<T>("Data/" + path);
        }

        public static AudioClip LoadSfx(string path)
        {
            return Load<AudioClip>("Audio/Sfx/" + path);
        }
    }
}
