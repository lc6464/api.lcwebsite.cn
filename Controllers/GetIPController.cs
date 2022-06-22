using System.Net.Sockets;

namespace API.Controllers {
	[ApiController]
	[Route("[controller]")]
	public class GetIPController : ControllerBase {
		private readonly ILogger<GetIPController> _logger;

		public GetIPController(ILogger<GetIPController> logger) {
			_logger = logger;
		}

		[HttpGet]
		public IP Get() {
			var connection = HttpContext.Connection;
			var address = connection.RemoteIpAddress;
			_logger.LogDebug("Hello! Client {}:{}", address?.AddressFamily == AddressFamily.InterNetworkV6 ? $"[{address}]" : address, connection.RemotePort);
			return new(address);
		}
	}
}