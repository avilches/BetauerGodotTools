using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Betauer {
    public class FixedEnumDictionary<TEnumKey, TValue> : EnumDictionary<TEnumKey, TValue> 
        where TEnumKey : Enum 
        where TValue : class {
        
        internal FixedEnumDictionary(TEnumKey[] enums) : base(enums) {
        }

        public override int GetPos(TEnumKey key) => key.ToInt();
    }

    public class VariableEnumDictionary<TEnumKey, TValue> : EnumDictionary<TEnumKey, TValue> 
        where TEnumKey : Enum 
        where TValue : class {
        internal VariableEnumDictionary(TEnumKey[] enums) : base(enums) {
        }

        public override int GetPos(TEnumKey key) {
            Span<TEnumKey> span = KeyArray.AsSpan();
            for (var pos = 0; pos < Count; pos++) {
                if (EqualityComparer<TEnumKey>.Default.Equals(span[pos], key)) return pos;
            }
            return -1; // this will never happen
        }
    }

    public abstract class EnumDictionary<TEnumKey, TValue> : IDictionary<TEnumKey, TValue>
        where TEnumKey : Enum
        where TValue : class {
        
        public readonly TEnumKey[] KeyArray;
        public readonly TValue?[] ValueArray;
        public int Count { get; }

        public static EnumDictionary<TEnumKey, TValue> Create(Func<TEnumKey, TValue>? filler = null) {
            var enums = (TEnumKey[])Enum.GetValues(typeof(TEnumKey));
            var isFixed = true;
            for (var count = 0; count < enums.Length; count++) {
                if (enums[count].ToInt() != count) {
                    isFixed = false;
                    break;
                }
            }
            
            EnumDictionary<TEnumKey, TValue> enumDictionary = isFixed ? 
                new FixedEnumDictionary<TEnumKey, TValue>(enums) : 
                new VariableEnumDictionary<TEnumKey, TValue>(enums);
            
            if (filler != null) enumDictionary.Fill(filler);
            return enumDictionary;
        }

        public abstract int GetPos(TEnumKey key);

        protected EnumDictionary(TEnumKey[] enums) {
            KeyArray = enums;
            Count = enums.Length;
            ValueArray = new TValue[Count];
        }

        public void Fill(Func<TEnumKey, TValue> filler) {
            foreach (var key in Keys) this[key] = filler(key);
        }

        public TValue? this[TEnumKey key] {
            get => ValueArray[GetPos(key)];
            set => ValueArray[GetPos(key)] = value;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public IEnumerator<KeyValuePair<TEnumKey, TValue>> GetEnumerator() {
            return new Enumerator(this);
        }

        public ICollection<TEnumKey> Keys => KeyArray;
        public ICollection<TValue> Values => ValueArray;

        public void Clear() {
            for (var i = 0; i < ValueArray.Length; i++) ValueArray[i] = null;
        }

        public void Add(KeyValuePair<TEnumKey, TValue> item) {
            this[item.Key] = item.Value;
        }

        public bool Contains(KeyValuePair<TEnumKey, TValue> item) {
            return item.Value == null ? this[item.Key] == null : this[item.Key]?.Equals(item.Value) ?? false;
        }

        public void CopyTo(KeyValuePair<TEnumKey, TValue>[] array, int arrayIndex) {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<TEnumKey, TValue> item) {
            this[item.Key] = null;
            return true;
        }

        public bool IsReadOnly => false;

        public void Add(TEnumKey key, TValue value) {
            this[key] = value;
        }

        public bool ContainsKey(TEnumKey key) {
            return this[key] is not null;
        }

        public bool Remove(TEnumKey key) {
            this[key] = null;
            return true;
        }

        public bool TryGetValue(TEnumKey key, out TValue value) {
            value = this[key];
            return value is not null;
        }

        public class Enumerator : IEnumerator<KeyValuePair<TEnumKey, TValue>> {
            private readonly EnumDictionary<TEnumKey, TValue> _enumDictionary;
            private int _index;
            private KeyValuePair<TEnumKey, TValue> _current;

            internal Enumerator(EnumDictionary<TEnumKey, TValue> enumDictionary) {
                _enumDictionary = enumDictionary;
                _index = 0;
                _current = default;
            }

            public bool MoveNext() {
                // Use unsigned comparison since we set index to dictionary.count+1 when the enumeration ends.
                // dictionary.count+1 could be negative if dictionary.count is int.MaxValue
                if ((uint)_index < (uint)_enumDictionary.Count) {
                    TEnumKey key = _enumDictionary.KeyArray[_index];
                    TValue? value = _enumDictionary[key];
                    _current = new KeyValuePair<TEnumKey, TValue>(key, value);
                    _index++;
                    return true;
                }

                _index = _enumDictionary.Count + 1;
                _current = default;
                return false;
            }

            public KeyValuePair<TEnumKey, TValue> Current => _current;

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