namespace TWILite.Utilities.MenuItemAutogen
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class MenuItemAutogenCollection<T> : MenuItemAutogenCollection, IList<T> where T : MenuItemAutogenItem
    {
        [SerializeField] private List<T> items = new List<T>();

        public override int Count => items.Count;

        #region IList<T>

        public bool IsReadOnly => ((ICollection<T>)items).IsReadOnly;

        public T this[int index]
        { 
            get => items[index];
            set => items[index] = value;
        }

        public void Add(T item) => items.Add(item);

        public override void Clear() => items.Clear();
        public bool Contains(T item) => items.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) => items.CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator() => items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();

        public int IndexOf(T item) => items.IndexOf(item);
        public void Insert(int index, T item) => items.Insert(index, item);

        public bool Remove(T item) => items.Remove(item);
        public void RemoveAt(int index) => items.RemoveAt(index);

        #endregion
    }
}