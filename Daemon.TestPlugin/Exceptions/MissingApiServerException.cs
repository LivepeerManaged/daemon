namespace Testing.Exceptions; 

public class MissingApiServerException: Exception {
    public MissingApiServerException(): base("ApiServer is missing in Config!") {
    }
}