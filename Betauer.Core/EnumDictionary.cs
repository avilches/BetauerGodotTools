using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Betauer {
    public class FixedEnumDictionary<TKey, TValue> : EnumDictionary<TKey, TValue> 
        where TKey : Enum 
        where TValue : class {
        
        internal FixedEnumDictionary(TKey[] enums) : base(enums) {
        }

        public override int GetPos(TKey key) => key.ToInt();
    }

    public class VariableEnumDictionary<TKey, TValue> : EnumDictionary<TKey, TValue> 
        where TKey : Enum 
        where TValue : class {
        
        internal VariableEnumDictionary(TKey[] enums) : base(enums) {
        }

        public override int GetPos(TKey key) {
            for (var pos = 0; pos < Count; pos++) {
                if (EqualityComparer<TKey>.Default.Equals(KeyArray[pos], key)) return pos;
            }
            return -1; // this will never happen
        }
    }

    public abstract class EnumDictionary<TKey, TValue> : IDictionary<TKey, TValue>
        where TKey : Enum
        where TValue : class {
        
        public readonly TValue?[] ValueArray;
        public readonly TKey[] KeyArray;
        public int Count { get; }

        public static EnumDictionary<TKey, TValue> Create(Func<TKey, TValue>? filler = null) {
            var enums = (TKey[])Enum.GetValues(typeof(TKey));
            var isFixed = true;
            for (var count = 0; count < enums.Length; count++) {
                if (enums[count].ToInt() != count) {
                    isFixed = false;
                    break;
                }
            }
            
            EnumDictionary<TKey, TValue> enumDictionary = isFixed ? 
                new FixedEnumDictionary<TKey, TValue>(enums) : 
                new VariableEnumDictionary<TKey, TValue>(enums);
            
            if (filler != null) enumDictionary.Fill(filler);
            return enumDictionary;
        }

        public abstract int GetPos(TKey key);

        protected EnumDictionary(TKey[] enums) {
            KeyArray = enums;
            Count = enums.Length;
            ValueArray = new TValue[Count];
        }

        public void Fill(Func<TKey, TValue> filler) {
            foreach (var key in Keys) this[key] = filler(key);
        }

        public TValue? this[TKey key] {
            get => ValueArray[GetPos(key)];
            set => ValueArray[GetPos(key)] = value;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            return new Enumerator(this);
        }

        public ICollection<TKey> Keys => KeyArray;
        public ICollection<TValue> Values => ValueArray;

        public void Clear() {
            for (var i = 0; i < ValueArray.Length; i++) ValueArray[i] = null;
        }

        public void Add(KeyValuePair<TKey, TValue> item) {
            this[item.Key] = item.Value;
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) {
            return item.Value == null ? this[item.Key] == null : this[item.Key]?.Equals(item.Value) ?? false;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) {
            this[item.Key] = null;
            return true;
        }

        public bool IsReadOnly => false;

        public void Add(TKey key, TValue value) {
            this[key] = value;
        }

        public bool ContainsKey(TKey key) {
            return this[key] is not null;
        }

        public bool Remove(TKey key) {
            this[key] = null;
            return true;
        }

        public bool TryGetValue(TKey key, out TValue value) {
            value = this[key];
            return value is not null;
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
                    TKey key = _enumDictionary.KeyArray[_index];
                    TValue? value = _enumDictionary[key];
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