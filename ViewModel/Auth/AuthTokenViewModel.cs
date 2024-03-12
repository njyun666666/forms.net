using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.ViewModel.Auth
{
    public class AuthTokenViewModel
    {
        public string UID { get; set; }
		public string TokenKey { get; set; }
		/// <summary>
		/// 過期時間
		/// </summary>
		public DateTime ExpiresDate { get; set; }
    }
}
