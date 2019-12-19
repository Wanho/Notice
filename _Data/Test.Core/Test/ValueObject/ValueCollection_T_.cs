using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Test.Core
{
	public class ValueCollection<T> : ICollection<T>, IEnumerable<T>, IEnumerable
	{
		public int Count
		{
			get
			{
				return this.List.Count;
			}
		}

		protected Dictionary<string, T> Dic
		{
			get;
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public T this[int index]
		{
			get
			{
				return this.List[index];
			}
		}

		public T this[string name]
		{
			get
			{
				return this.Dic[name];
			}
		}

		protected List<T> List
		{
			get;
		}

		public ValueCollection()
		{
		}

		public void Add(T item)
		{
			this.List.Add(item);
		}

		public void Clear()
		{
			this.List.Clear();
		}

		public bool Contains(T item)
		{
			return this.List.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this.List.GetEnumerator();
		}

		public bool Remove(T item)
		{
			return this.List.Remove(item);
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)this.List).GetEnumerator();
		}
	}
}