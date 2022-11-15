namespace Oxi
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    public class Environment : IDictionary<string, IValue>
    {
        private readonly IDictionary<string, IValue> vars =
            new Dictionary<string, IValue>();

        public Environment()
        {
        }

        public ICollection<string> Keys => this.vars.Keys;

        public ICollection<IValue> Values => this.vars.Values;

        public int Count => this.vars.Count;

        public bool IsReadOnly => false;

        public IValue this[string key]
        {
            get
            {
                if (this.ContainsKey(key))
                {
                    return this.vars[key];
                }

                return Value.Error.VARNF;
            }

            set
            {
                this.vars[key] = value;
            }
        }

        public void Add(string key, IValue value) =>
            this.vars.Add(key, value);

        public void Add(KeyValuePair<string, IValue> item) =>
            this.vars.Add(item);

        public void Clear() =>
            this.vars.Clear();

        public bool Contains(KeyValuePair<string, IValue> item) =>
            this.vars.Contains(item);

        public bool ContainsKey(string key) =>
            this.vars.ContainsKey(key);

        public void CopyTo(
            KeyValuePair<string, IValue>[] array,
            int arrayIndex) => this.vars.CopyTo(array, arrayIndex);

        public IEnumerator<KeyValuePair<string, IValue>> GetEnumerator() =>
            this.vars.GetEnumerator();

        public bool Remove(string key) =>
            this.vars.Remove(key);

        public bool Remove(KeyValuePair<string, IValue> item) =>
            this.vars.Remove(item);

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out IValue value) =>
            this.vars.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() =>
            this.vars.GetEnumerator();
    }
}