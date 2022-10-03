using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Betauer {
    public class EnumDictionary<TKey, TValue> : IDictionary<TKey, TValue> where TKey : struct, Enum {
        private readonly TValue[] _values;
        private readonly TKey[] _keys;
        public int Count { get; }

        public EnumDictionary(Func<TKey, TValue> filler) : this() {
            Fill(filler);
        }

        private int GetPos(TKey key) {
            for (var pos = 0; pos < Count; pos++) {
                if (EqualityComparer<TKey>.Default.Equals( _keys[pos], key)) return pos;
            }
            return -1; // this will never happen
        }

        public EnumDictionary() {
            var enums = Enum.GetValues(typeof(TKey));
            Count = enums.Length;
            _keys = enums.Cast<TKey>().ToArray();

            _values = new TValue[Count];
        }

        public void Fill(Func<TKey, TValue> filler) {
            foreach (var key in Keys) _values[GetPos(key)] = filler(key);
        }

        public TValue this[TKey key] {
            get => _values[GetPos(key)];
            set => _values[GetPos(key)] = value;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            return new Enumerator(this);
        }

        public ICollection<TKey> Keys => _keys;
        public ICollection<TValue> Values => _values;

        public void Clear() {
            Fill(e => default);
        }

        public void Add(KeyValuePair<TKey, TValue> item) {
            this[item.Key] = item.Value;
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) {
            return this[item.Key]?.Equals(item.Value) ?? false;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) {
            this[item.Key] = default;
            return true;
        }

        public bool IsReadOnly => false;
        public void Add(TKey key, TValue value) {
            this[key] = value;
        }

        public bool ContainsKey(TKey key) {
            return true;
        }

        public bool Remove(TKey key) {
            this[key] = default;
            return true;
        }

        public bool TryGetValue(TKey key, out TValue value) {
            value = this[key];
            return true;
        }

        public class Enumerator : IEnumerator<KeyValuePair<TKey, TValue>> {
            private readonly EnumDictionary<TKey, TValue> _enumDictionary;
            private int _index;
            private KeyValuePair<TKey, TValue> _current;

            internal Enumerator(EnumDictionary<TKey, TValue> enumDictionary) {
                _enumDictionary = enumDictionary;
                _index = 0;
                _current = default;
            }

            public bool MoveNext() {
                // Use unsigned comparison since we set index to dictionary.count+1 when the enumeration ends.
                // dictionary.count+1 could be negative if dictionary.count is int.MaxValue
                if ((uint)_index < (uint)_enumDictionary.Count) {
                    TKey key = _enumDictionary._keys[_index];
                    TValue value = _enumDictionary[key];
                    _current = new KeyValuePair<TKey, TValue>(key, value);
                    _index++;
                    return true;
                }

                _index = _enumDictionary.Count + 1;
                _current = default;
                return false;
            }

            public KeyValuePair<TKey, TValue> Current => _current;

            public void Dispose() {
            }

            void IEnumerator.Reset() {
                _index = 0;
                _current = default;
            }

            object IEnumerator.Current => Current;
        }
    }
}