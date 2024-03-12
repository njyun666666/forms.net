using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Services.IServices
{
	public interface ISendEmailService
	{
		public Task<int> SendEmail(string[] tos, string subject, string body);
	}
}
