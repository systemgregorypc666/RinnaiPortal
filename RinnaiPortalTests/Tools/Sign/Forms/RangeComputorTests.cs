using Xunit;
using RinnaiPortal.Tools.Sign.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RinnaiPortal.ViewModel.Sign.Forms;
using RinnaiPortalTests.Tools.Compare;

namespace RinnaiPortal.Tools.Sign.Forms.Tests
{
	public class RangeComputorTests
	{
		public static IEnumerable<object[]> BeforeOnWorkRangeData
		{
			get
			{
				yield return new object[]
				{
					new OvertimeVerifyViewModel()
					{
						startTime = new DateTime(2016, 5, 10, 6, 0, 0 ),
						endTime = new DateTime(2016, 5, 10, 7, 0, 0 ),
						onWorkTime = new DateTime(2016, 5, 10, 8, 0, 0 ),
					},
					new TimeSpan(1,0,0),
				};

				yield return new object[]
				{
					new OvertimeVerifyViewModel()
					{
						startTime = new DateTime(2016, 5, 10, 6, 0, 1 ),
						endTime = new DateTime(2016, 5, 10, 8, 0, 0 ),
						onWorkTime = new DateTime(2016, 5, 10, 8, 0, 0 ),
					},
					new TimeSpan(1,59,59),

				};
			}
		}

		[Theory]
		[MemberData("BeforeOnWorkRangeData")]
		public void BeforeOnWorkRangeTest(OvertimeVerifyViewModel model, TimeSpan expectResult)
		{
			var actualResult = RangeComputor.BeforeOnWorkRange(model);
			Assert.Equal(expectResult, actualResult);
		}

		public static IEnumerable<object[]> AfterAddWorkRangeData
		{
			get
			{
				yield return new object[]
				{
					new OvertimeVerifyViewModel()
					{
						startTime = new DateTime(2016, 5, 10, 17, 20, 0 ),
						endTime = new DateTime(2016, 5, 10, 18, 20, 0 ),
						addWorkTime = new DateTime(2016, 5, 10, 17, 20, 0 ),
					},
					new TimeSpan(1,0,0),
				};

				yield return new object[]
				{
					new OvertimeVerifyViewModel()
					{
						startTime = new DateTime(2016, 5, 10, 17, 21, 0 ),
						endTime = new DateTime(2016, 5, 10, 18, 20, 0 ),
						addWorkTime = new DateTime(2016, 5, 10, 17, 20, 0 ),
					},
					new TimeSpan(0,0,0),

				};

				yield return new object[]
				{
					new OvertimeVerifyViewModel()
					{
						startTime = new DateTime(2016, 5, 10, 17, 19, 0 ),
						endTime = new DateTime(2016, 5, 10, 18, 20, 0 ),
						addWorkTime = new DateTime(2016, 5, 10, 17, 20, 0 ),
					},
					new TimeSpan(1,1,0),
				};
			}
		}

		[Theory]
		[MemberData("AfterAddWorkRangeData")]
		public void AfterAddWorkRangeTest(OvertimeVerifyViewModel model, TimeSpan expectResult)
		{
			var actualResult = RangeComputor.AfterAddWorkRange(model);
			Assert.Equal(expectResult, actualResult);
		}

		public static IEnumerable<object[]> MorningRangeData
		{
			get
			{
				yield return new object[]
				{
					new OvertimeVerifyViewModel()
					{
						endTime = new DateTime(2016, 5, 10, 12, 0, 0 ),
						startTime = new DateTime(2016, 5, 10, 7, 0, 0 ),
						onWorkTime = new DateTime(2016, 5, 10, 8, 0, 0 ),
						offWorkTime = new DateTime(2016, 5, 10, 17, 0, 0 ),
						afternoonWorkTime = new DateTime(2016, 5, 10, 12, 40, 0 ),
						addWorkTime = new DateTime(2016, 5, 10, 17, 20, 0 ),
						noonTime = new DateTime(2016, 5, 10, 12, 0, 0 ),
					},
					new TimeSpan(5,0,0),
				};

				yield return new object[]
				{
					new OvertimeVerifyViewModel()
					{
						endTime = new DateTime(2016, 5, 10, 13, 40, 0 ),
						startTime = new DateTime(2016, 5, 10, 7, 0, 0 ),
						onWorkTime = new DateTime(2016, 5, 10, 8, 0, 0 ),
						offWorkTime = new DateTime(2016, 5, 10, 17, 0, 0 ),
						afternoonWorkTime = new DateTime(2016, 5, 10, 12, 40, 0 ),
						addWorkTime = new DateTime(2016, 5, 10, 17, 20, 0 ),
						noonTime = new DateTime(2016, 5, 10, 12, 0, 0 ),
					},
					new TimeSpan(6,0,0),
				};

				yield return new object[]
				{
					new OvertimeVerifyViewModel()
					{
						endTime = new DateTime(2016, 5, 10, 14, 0, 0 ),
						startTime = new DateTime(2016, 5, 10, 7, 0, 0 ),
						onWorkTime = new DateTime(2016, 5, 10, 8, 0, 0 ),
						offWorkTime = new DateTime(2016, 5, 10, 17, 0, 0 ),
						afternoonWorkTime = new DateTime(2016, 5, 10, 12, 40, 0 ),
						addWorkTime = new DateTime(2016, 5, 10, 17, 20, 0 ),
						noonTime = new DateTime(2016, 5, 10, 12, 0, 0 ),
					},
					new TimeSpan(6,20,0),
				};

				yield return new object[]
				{
					new OvertimeVerifyViewModel()
					{
						endTime = new DateTime(2016, 5, 10, 18, 19, 0 ),
						startTime = new DateTime(2016, 5, 10, 8, 0, 0 ),
						onWorkTime = new DateTime(2016, 5, 10, 8, 0, 0 ),
						offWorkTime = new DateTime(2016, 5, 10, 17, 0, 0 ),
						afternoonWorkTime = new DateTime(2016, 5, 10, 12, 40, 0 ),
						addWorkTime = new DateTime(2016, 5, 10, 17, 20, 0 ),
						noonTime = new DateTime(2016, 5, 10, 12, 0, 0 ),
					},
					new TimeSpan(9,19,0),
				};
			}
		}

		[Theory]
		[MemberData("MorningRangeData")]
		public void MorningRangeTest(OvertimeVerifyViewModel model, TimeSpan expect)
		{
			var actual = RangeComputor.MorningRange(model);
			Assert.Equal(expect, actual);
		}

		public static IEnumerable<object[]> AfternoonRangeData
		{
			get
			{
				yield return new object[]
				{
					new OvertimeVerifyViewModel()
					{
						endTime = new DateTime(2016, 5, 10, 15, 40, 0 ),
						startTime = new DateTime(2016, 5, 10, 12, 39, 0 ),
						afternoonWorkTime = new DateTime(2016, 5, 10, 12, 40, 0 ),
						offWorkTime = new DateTime(2016, 5, 10, 17, 0, 0 ),
						addWorkTime = new DateTime(2016, 5, 10, 17, 20, 0 ),
						noonTime = new DateTime(2016, 5, 10, 12, 0, 0 ),
					},
					new TimeSpan(3,1,0),
				};

				yield return new object[]
				{
					new OvertimeVerifyViewModel()
					{
						endTime = new DateTime(2016, 5, 10, 18, 20, 0 ),
						startTime = new DateTime(2016, 5, 10, 12, 39, 0 ),
						afternoonWorkTime = new DateTime(2016, 5, 10, 12, 40, 0 ),
						offWorkTime = new DateTime(2016, 5, 10, 17, 0, 0 ),
						addWorkTime = new DateTime(2016, 5, 10, 17, 20, 0 ),
						noonTime = new DateTime(2016, 5, 10, 12, 0, 0 ),
					},
					new TimeSpan(5,21,0),
				};
			}
		}

		[Theory]
		[MemberData("AfternoonRangeData")]
		public void AfternoonRangeTest(OvertimeVerifyViewModel model, TimeSpan expect)
		{
			var actual = RangeComputor.AfternoonRange(model);
			Assert.Equal(expect, actual);
		}

		public static IEnumerable<object[]> OvertimeRangeData
		{
			get
			{
				yield return new object[]
				{
					new OvertimeVerifyViewModel()
					{
						endTime = new DateTime(2016, 5, 10, 18, 20, 0 ),
						startTime = new DateTime(2016, 5, 10, 17, 10, 0 ),
						addWorkTime = new DateTime(2016, 5, 10, 17, 20, 0 ),
					},
					new TimeSpan(1,0,0),
				};
			}
		}

		[Theory]
		[MemberData("OvertimeRangeData")]
		public void OvertimeRangeTest(OvertimeVerifyViewModel model, TimeSpan expect)
		{
			var actual = RangeComputor.OvertimeRange(model);
			Assert.Equal(expect, actual);
		}

		public static IEnumerable<object[]> HMformaterData
		{
			get
			{
				yield return new object[]
				{
					new DateTime(2016, 5, 10, 8, 0, 0 ),
					new DateTime(2016, 5, 10, 7, 59, 0 ),
					new DateTime(2016, 5, 10, 7, 59, 0 ),
				};

				yield return new object[]
				{
					new DateTime(2016, 5, 10, 8, 0, 0 ),
					new DateTime(2016, 5, 10, 8, 1, 0 ),
					new DateTime(2016, 5, 10, 9, 0, 0 ),
				};

				yield return new object[]
				{
					new DateTime(2016, 5, 10, 8, 0, 0 ),
					new DateTime(2016, 5, 10, 8, 0, 0 ),
					new DateTime(2016, 5, 10, 8, 0, 0 ),
				};

				yield return new object[]
				{
					new DateTime(2016, 5, 10, 12, 40, 0 ),
					new DateTime(2016, 5, 10, 12, 36, 0 ),
					new DateTime(2016, 5, 10, 12, 36, 0 ),
				};

				yield return new object[]
				{
					new DateTime(2016, 5, 10, 12, 40, 0 ),
					new DateTime(2016, 5, 10, 12, 41, 0 ),
					new DateTime(2016, 5, 10, 13, 40, 0 ),
				};

				yield return new object[]
				{
					new DateTime(2016, 5, 10, 12, 40, 0 ),
					new DateTime(2016, 5, 10, 12, 40, 0 ),
					new DateTime(2016, 5, 10, 12, 40, 0 ),
				};

				yield return new object[]
				{
					new DateTime(2016, 5, 10, 17, 20, 0 ),
					new DateTime(2016, 5, 10, 17, 00, 0 ),
					new DateTime(2016, 5, 10, 17, 00, 0 ),
				};

				yield return new object[]
				{
					new DateTime(2016, 5, 10, 17, 20, 0 ),
					new DateTime(2016, 5, 10, 17, 30, 0 ),
					new DateTime(2016, 5, 10, 18, 20, 0 ),
				};

				yield return new object[]
				{
					new DateTime(2016, 5, 10, 17, 20, 0 ),
					new DateTime(2016, 5, 10, 17, 20, 0 ),
					new DateTime(2016, 5, 10, 17, 20, 0 ),
				};

				yield return new object[]
				{
					new DateTime(2016, 5, 10, 17, 01, 0 ),
					new DateTime(2016, 5, 10, 16, 59, 0 ),
					new DateTime(2016, 5, 10, 16, 59, 0 ),
				};

				yield return new object[]
				{
					new DateTime(2016, 5, 10, 17, 20, 0 ),
					new DateTime(2016, 5, 10, 18, 20, 0 ),
					new DateTime(2016, 5, 10, 18, 20, 0 ),
				};
			}
		}

		[Theory]
		[MemberData("HMformaterData")]
		public void HMformaterTest(DateTime standar, DateTime time, DateTime expect)
		{
			var actual = RangeComputor.HMformater(standar, time);
			Assert.Equal(expect, actual);
		}
	}
}