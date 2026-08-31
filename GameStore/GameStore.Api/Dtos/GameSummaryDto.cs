namespace GameStore.Api.Dtos;

//A DTO (Data Transfer Object) is a simple object that is used to transfer data between layers of an application. 
// In this case, the GameDto class is likely used to transfer data related to a game in the GameStore application. 
// The record class syntax in C# is used to define immutable data objects with value-based equality.
public record GameSummaryDto
(
    int Id,
    string Name,
    string Genre,
    decimal Price,
    DateOnly ReleaseDate
);
  