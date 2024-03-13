using FormsNet.Common;
using FormsNet.DB;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FormsNet.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class TestController : ControllerBase
	{

		IMyService _myService;
		IDB_Test _db_Test;

		public TestController(IMyService myService, IDB_Test dB_Test)
		{
			_myService = myService;
			_db_Test = dB_Test;
		}

		public async Task<IActionResult> Test()
		{
			string config = "Release";

#if DEBUG
			config = "Debug";
#endif

			return Ok(new
			{
				config = config,
				appsettingName = _myService.AppsettingName(),
				backEndV2DB = await _db_Test.BackEndV2DBTest()
			});
		}

	}
}
