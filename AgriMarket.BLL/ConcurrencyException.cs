namespace AgriMarket.BLL;

public sealed class ConcurrencyException(string message) : Exception(message);
