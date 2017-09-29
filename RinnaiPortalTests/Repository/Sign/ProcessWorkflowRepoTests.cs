using DBTools;
using RinnaiPortal.FactoryMethod;
using RinnaiPortal.Repository;
using RinnaiPortal.Tools;
using RinnaiPortalTests.Tools.Compare;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RinnaiPortalTests.Repository.Sign
{
	public class ProcessWorkflowRepoTests
	{
		private ProcessWorkflowRepository _repo { get; set; }
		public ProcessWorkflowRepoTests()
		{
			var dc = new DB(@"Data Source=.\SQLEXPRESS;Initial Catalog=RinnaiPortal;Persist Security Info=True;User ID=sa;Password=p@ssw0rd");
			_repo = new ProcessWorkflowRepository(dc, new RootRepository(dc));
		}

		public static IEnumerable<object[]> FindUpperDeptSample
		{
			get
			{
				yield return new object[]
				{
					"00591",
					"943200",
					new Dictionary<string, string>()
					{
						{"00099","3200"}
					}
				};

				yield return new object[]
				{
					"00467",
					"3410",
					new Dictionary<string, string>()
					{
						{"00484","923002"}
					}
				};

				yield return new object[]
				{
					"00094",
					"3910",
					new Dictionary<string, string>()
					{
						{"00412","923001"}
					}
				};

				yield return new object[]
				{
					"01466",
					"933961",
					new Dictionary<string, string>()
					{
						{"00919","923961"}
					}
				};

				yield return new object[]
				{
					"00915",
					"2600",
					new Dictionary<string, string>()
					{
						{"00484","2200"}
					}
				};
			}
		}

		[Theory]
		[MemberData("FindUpperDeptSample")]
		public void FindUpperDeptDataTest(string empID, string deptID, Dictionary<string,string> expect)
		{

			var actual = _repo.FindUpperDeptData(deptID, empID);
			Assert.Equal(expect, actual, new DicCompare<Dictionary<string, string>>());
		}
	}
}
