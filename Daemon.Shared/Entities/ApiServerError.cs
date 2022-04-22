namespace Daemon.Shared.Entities;

public class ApiServerError : Exception {
	public ApiServerError(string id, string name, string? message) : base(message) {
		Id = id;
		Name = name;
	}

	public string Id { get; set; }
	public string Name { get; set; }
}