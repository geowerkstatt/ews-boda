namespace EWS;

/// <summary>
/// Represents a response from the data service endpoint.
/// </summary>
/// <param name="Gemeinde">The Gemeinde, if any.</param>
/// <param name="Grundbuchnummer">The Grundbuchnummer(n), if any.</param>
public record DataServiceResponse(string? Gemeinde, string? Grundbuchnummer);
