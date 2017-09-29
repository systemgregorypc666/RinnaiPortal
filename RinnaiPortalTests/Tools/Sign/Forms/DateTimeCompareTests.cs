using Xunit;
using RinnaiPortal.Tools.Sign.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RinnaiPortalTests.Tools.Compare;

namespace RinnaiPortalTests.Tools.Sign.Forms
{
	public class DateTimeCompareTests
	{
		public static IEnumerable<object[]> IsCoveredData
		{
			get
			{
				yield return new object[]
				{
					new DateTime(2016, 5, 10, 08, 00, 00 ),
					new DateTime(2016, 5, 10, 17, 00, 00 ),
					new DateTime(2016, 5, 10, 01, 00, 00 ),
					new DateTime(2016, 5, 10, 08, 00, 00 ),
					false
				};

				yield return new object[]
				{
					new DateTime(2016, 5, 10, 08, 00, 00 ),
					new DateTime(2016, 5, 10, 17, 00, 00 ),
					new DateTime(2016, 5, 10, 01, 00, 00 ),
					new DateTime(2016, 5, 10, 07, 59, 59 ),
					false
				};

				yield return new object[]
				{
					new DateTime(2016, 5, 10, 08, 00, 00 ),
					new DateTime(2016, 5, 10, 17, 00, 00 ),
					new DateTime(2016, 5, 10, 17, 00, 00 ),
					new DateTime(2016, 5, 10, 19, 00, 00 ),
					false
				};

				yield return new object[]
				{
					new DateTime(2016, 5, 10, 08, 00, 00 ),
					new DateTime(2016, 5, 10, 17, 00, 00 ),
					new DateTime(2016, 5, 10, 17, 00, 01 ),
					new DateTime(2016, 5, 10, 19, 00, 00 ),
					false
				};

				yield return new object[]
				{
					new DateTime(2016, 5, 10, 08, 00, 00 ),
					new DateTime(2016, 5, 10, 17, 00, 00 ),
					new DateTime(2016, 5, 10, 01, 00, 00 ),
					new DateTime(2016, 5, 10, 09, 00, 00 ),
					true
				};

				yield return new object[]
				{
					new DateTime(2016, 5, 10, 08, 00, 00 ),
					new DateTime(2016, 5, 10, 17, 00, 00 ),
					new DateTime(2016, 5, 10, 16, 00, 00 ),
					new DateTime(2016, 5, 10, 22, 00, 00 ),
					true
				};

				yield return new object[]
				{
					new DateTime(2016, 5, 10, 08, 00, 00 ),
					new DateTime(2016, 5, 10, 17, 00, 00 ),
					new DateTime(2016, 5, 10, 08, 00, 00 ),
					new DateTime(2016, 5, 10, 17, 00, 00 ),
					true
				};

				yield return new object[]
				{
					new DateTime(2016, 5, 10, 08, 00, 00 ),
					new DateTime(2016, 5, 10, 17, 00, 00 ),
					new DateTime(2016, 5, 10, 08, 00, 01 ),
					new DateTime(2016, 5, 10, 16, 59, 59 ),
					true
				};

				yield return new object[]
				{
					new DateTime(2016, 5, 10, 08, 00, 00 ),
					new DateTime(2016, 5, 10, 17, 00, 00 ),
					new DateTime(2016, 5, 10, 07, 00, 00 ),
					new DateTime(2016, 5, 10, 18, 00, 00 ),
					true
				};

				yield return new object[]
				{
					new DateTime(2016, 5, 10, 08, 00, 00 ),
					new DateTime(2016, 5, 10, 17, 00, 00 ),
					new DateTime(2016, 5, 08, 01, 00, 00 ),
					new DateTime(2016, 5, 12, 07, 00, 00 ),
					true
				};

			}
		}

		[Theory]
		[MemberData("IsCoveredData")]
		public void IsCoveredTest(DateTime baseStart, DateTime baseEnd, DateTime targetStart, DateTime targetEnd, bool expect)
		{
			var actual = DateTimeCompare.IsCovered(baseStart, baseEnd, targetStart, targetEnd);
			Assert.Equal(expect, actual);
		}
	}
}
