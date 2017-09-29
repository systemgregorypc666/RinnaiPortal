using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RinnaiPortalTests.Tools.Compare
{
	public class DicCompare<T> : IEqualityComparer<T>
	{
		public bool Equals(T actual, T expect)
		{
			var result = false;
			if (actual is Dictionary<string, string> && expect is Dictionary<string, string>)
			{
				var a = actual as Dictionary<string, string>;
				var b = expect as Dictionary<string, string>;
				if (a.Count != b.Count) { return false; }

				result = a.All(aDic =>
				{
					return a[aDic.Key].Equals(b[aDic.Key]);
				});
			}

			return result;
		}

		public int GetHashCode(T obj)
		{
			throw new NotImplementedException();
		}
	}
}
