using JworkzNeosMod.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JworkzNeosMod.Events.Watchers
{
    internal static class Utf8JsonFileProgressWatcher
    {
        internal static ConcurrentDictionary<FileId, int> FileToConsumedBytesSizeDictionary = new ConcurrentDictionary<FileId, int>();
        public static bool IsWatchingFile(FileId fileId) => FileToConsumedBytesSizeDictionary.ContainsKey(fileId);

        public static void StartCache(FileId id, int startingConsumedByteSize = 0)
        {
            if (!FileToConsumedBytesSizeDictionary.ContainsKey(id))
            {
                FileToConsumedBytesSizeDictionary[id] = startingConsumedByteSize;
            }
        }

        public static int GetConsumedBytesFromId(FileId id)
        {
            return !IsWatchingFile(id) ? 0 : FileToConsumedBytesSizeDictionary[id];
        }

        public static void UpdateCacheValue(FileId id, int consumedByteSize)
        {
            if (FileToConsumedBytesSizeDictionary.ContainsKey(id))
            {
                FileToConsumedBytesSizeDictionary[id] = consumedByteSize;
            }
        }

        public static void StopCache(FileId id)
        {
            if (FileToConsumedBytesSizeDictionary.ContainsKey(id))
            {
                FileToConsumedBytesSizeDictionary.TryRemove(id, out int __);
            }
        }
    }
}
