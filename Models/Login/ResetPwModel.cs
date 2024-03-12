using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Models.Login
{
	public class ResetPwModel
	{
		public string Account { get; set; }
	}
	public class ResetPwMailModel
	{
		public string Account { get; set; }
		public DateTime ExpireTime { get; set; }
	}
}
