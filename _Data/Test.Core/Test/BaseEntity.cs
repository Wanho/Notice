using System;
using System.Reflection;

namespace Test.Core
{
	[Serializable]
	public abstract class BaseEntity : ValueObject
	{
		private ValueRow data;

		[Ignore]
		public object this[string key]
		{
			get
			{
				return this.data[key];
			}
		}

		protected BaseEntity()
		{
		}

		public ValueRow GetValueRow()
		{
			return this.data;
		}

		internal void SetValueRow(ValueRow data)
		{
			this.data = data;
		}

		public T ToVo<T>() where T : ValueObject
		{
			return this.data.ToVo<T>(default(T), false);
		}

		public T ToVo<T>(T vo, bool useAlias = false) where T : ValueObject
		{
			return this.data.ToVo<T>(vo, useAlias);
		}
	}
}