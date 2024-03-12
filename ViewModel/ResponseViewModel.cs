using FormsNet.Common;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace FormsNet.ViewModels
{
	public class ResponseViewModel
	{
		[JsonPropertyName("code")]
		public int Code { get; set; }

		[JsonPropertyName("message")]
		public string Message { get; set; }
		[JsonPropertyName("data")]
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public object Data { get; set; }
		public ResponseViewModel()
		{
			this.Code = -10001;
			this.Message = "不明錯誤";
		}
		public ResponseViewModel(int code, string msg = null)
		{
			this.Code = code;
			this.Message = msg;
		}
		public ResponseViewModel(ResponseCode code, string message = null)
		{
			this.Code = (int)code;
			this.Message = message;
			if (message == null)
				this.Message = EnumExtenstions.GetEnumDescription(code);
		}
	}
	public class OKResponse : ResponseViewModel
	{
		public OKResponse() : base(1, "Success")
		{
		}
	}

	public class DisableResponse : ResponseViewModel
	{
		public DisableResponse() : base(2)
		{
			this.Message = "Account Disable";
		}
	}
	public class FailResponse : ResponseViewModel
	{
		public FailResponse() : base(ResponseCode.fail)
		{
			this.Message = "失敗";
		}
		public FailResponse(string message) : base(ResponseCode.fail)
		{
			this.Message = message;
		}
	}
	public class MissParamResponse : ResponseViewModel
	{
		public MissParamResponse() : base(-2)
		{
		}
		public MissParamResponse(string paramName) : base(-2)
		{
			this.Message = "缺少參數:" + paramName;
		}
	}
	public class LoginFailResponse : ResponseViewModel
	{
		public LoginFailResponse(string message) : base(ResponseCode.login_fail)
		{
			this.Message = message;
		}
	}
	public class NoAuthResponse : ResponseViewModel
	{
		public NoAuthResponse() : base(ResponseCode.no_auth)
		{
			this.Message = "權限不足";
		}
		public NoAuthResponse(string message) : base(ResponseCode.no_auth)
		{
			this.Message = message;
		}
	}
	public class ExceptionResponse : ResponseViewModel
	{
		public ExceptionResponse() : base(ResponseCode.exception)
		{ }
		public ExceptionResponse(string msg) : base(ResponseCode.exception)
		{
			this.Message = msg;
		}
	}
	public enum ResponseCode
	{

		[Description("失敗")]
		fail = -1,

		[Description("參數錯誤")]
		param_error = -2,

		[Description("Login fail")]
		login_fail = -3,

		[Description("No auth")]
		no_auth = -4,

		[Description("重複送出")]
		repeat_submit = -5,


		[Description("Exception")]
		exception = -999,

		[Description("Success")]
		success = 1,

	}
}
