namespace TestPlugin;

public class ApiServerError: Exception {
    public string Id { get; set; }
    public string Name { get; set; }
    public string Message { get; set; }

    public ApiServerError(string id, string name, string? message) : base(message) {
        Id = id;
        Name = name;
        Message = message;
    }
}