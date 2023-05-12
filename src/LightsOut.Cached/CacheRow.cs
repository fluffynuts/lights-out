using System;
using System.Text.Json;

namespace LightsOut.Cached
{
    internal class CacheRow<T> : CacheRow where T : class
    {
        private T _item;

        public T Item
        {
            get => _item ??= JsonSerializer.Deserialize<T>(Data);
            set
            {
                Data = JsonSerializer.Serialize(value);
                _item = null;
            }
        }

        public DateTime LastModifiedOn
        {
            get => DateTime.Parse(LastModified);
            set => LastModified = value.ToString("o");
        }

        public CacheRow()
        {
        }

        public CacheRow(
            string context,
            T item,
            string id
        )
        {
            Context = context;
            Item = item;
            Id = id;
            LastModifiedOn = DateTime.Now;
        }
    }

    internal class CacheRow
    {
        public string Context { get; set; }
        public string Id { get; set; }
        public string Data { get; set; }
        public string LastModified { get; set; }

        internal static CacheRow<T> For<T>(
            string context,
            T item,
            string id
        ) where T : class
        {
            return new CacheRow<T>(
                context,
                item,
                id
            );
        }
    }
}