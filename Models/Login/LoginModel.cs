using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FormsNet.Models.Login
{
    public class LoginModel
    {
		[Required(ErrorMessage = "請輸入帳號")]
		public string Account { get; set; }
		[Required(ErrorMessage = "請輸入密碼")]
		public string Password { get; set; }
	}
	public class LoginInfoModel
	{
		public LoginInfoModel()
		{
			Date = DateTime.Now;
		}
		[JsonPropertyName("uid")]
		public string UID { get; set; }
		[JsonPropertyName("name")]
		public string Name { get; set; }
		[JsonPropertyName("photoURL")]
		public string PhotoURL { get; set; }
		/// <summary>
		/// 取得時間
		/// </summary>
		[JsonPropertyName("date")]
		public DateTime Date { get; set; }
	}
	public class LoginViewModel
	{
		public string Token { get; set; }
		public string InfoJsonString { get; set; }
	}
	public class ChangeUserModel
	{
		public string UID { get; set; }
	}
}
