using System;
using Newtonsoft.Json;

namespace Hammer.Iiko.WsServer.Rpc;

internal class RpcRequestConstructor<T>
{
	private string _method { get; set; }

	public string ResultRequest { get; set; }

	public RpcRequestConstructor(string _method, T _params)
	{
		JsonRpcStruct<T> JsonToSerialize = new JsonRpcStruct<T>
		{
			jsonrpc = "2.0",
			method = _method,
			_params = _params,
			id = Guid.NewGuid().ToString()
		};
		ResultRequest = JsonConvert.SerializeObject(JsonToSerialize);
	}
}
