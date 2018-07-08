/********************************************************************
 *       CSPBase
 *       基本消息协议
 *********************************************************************/
#pragma warning disable 1591
using CoffeeBean;
using System.Collections.Generic;

namespace CoffeeBean
{
	/// <summary>
	/// 测试用结构3412
	/// </summary>
	public partial class STest : CPackageBase
	{
		/// <summary>
		/// 测试1
		/// </summary>
		public int Test_1;

	}
	/// <summary>
	/// 测试用结构ga
	/// </summary>
	public partial class SData : CPackageBase
	{
		/// <summary>
		/// 测试1adsf
		/// </summary>
		public byte Test_1;

		/// <summary>
		/// 测试2fads
		/// </summary>
		public short Test_2;

		/// <summary>
		/// 测试3
		/// </summary>
		public int Test_3;

		/// <summary>
		/// 测试4
		/// </summary>
		public long Test_4;

		/// <summary>
		/// 测试5
		/// </summary>
		public bool Test_5;

		/// <summary>
		/// 测试6
		/// </summary>
		public float Test_6;

		/// <summary>
		/// 测试7
		/// </summary>
		public string Test_7;

		/// <summary>
		/// 测试8
		/// </summary>
		public short[] Test_8;

		/// <summary>
		/// 测试9
		/// </summary>
		public List< int > Test_9;

	}
	/// <summary>
	/// 客户端Ping
	/// </summary>
	[CAttrPackage( OpCode = 6979, ClassComment = "客户端Ping" )]
	public class CSPPing : CPackageBase
	{
		/// <summary>
		/// 测试1
		/// </summary>
		public byte Test_1;

		/// <summary>
		/// 测试2
		/// </summary>
		public short Test_2;

		/// <summary>
		/// 测试3
		/// </summary>
		public int Test_3;

		/// <summary>
		/// 测试4
		/// </summary>
		public long Test_4;

		/// <summary>
		/// 测试5
		/// </summary>
		public bool Test_5;

		/// <summary>
		/// 测试6
		/// </summary>
		public float Test_6;

		/// <summary>
		/// 测试7
		/// </summary>
		public string Test_7;

		/// <summary>
		/// 测试8
		/// </summary>
		public int[] Test_8;

		/// <summary>
		/// 测试9
		/// </summary>
		public List< int > Test_9;

		/// <summary>
		/// 测试10
		/// </summary>
		public List< SData > Test_10;

		/// <summary>
		/// 测试11
		/// </summary>
		public SData Test_11;

		/// <summary>
		/// 测试12
		/// </summary>
		public STest Test_12;

	}
}
