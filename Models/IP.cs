using System.Net;
using System.Net.Sockets;

namespace API.Models {
	public struct IP {
		public IPAddress? IPAddress {
			set {
				_address = value;
			}
		}
		private IPAddress? _address = default;

        public IP() {}
		public IP(IPAddress? address) {
			_address = address;
        }

        public string? Address { get => _address?.ToString(); }
		public string? Family {
			get {
				return _address?.AddressFamily switch {
					AddressFamily.InterNetwork => "IPv4",
					AddressFamily.InterNetworkV6 => "IPv6",
					_ => _address?.AddressFamily.ToString()
				};
			}
		}
	}
}