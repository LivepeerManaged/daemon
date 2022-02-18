namespace Testing.Exceptions;

public class MissingKeyPairException : Exception {
	public MissingKeyPairException() : base("Public or Private key is missing in Config!") {
	}
}