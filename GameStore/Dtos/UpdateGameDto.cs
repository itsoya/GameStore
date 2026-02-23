using System.ComponentModel.DataAnnotations;

namespace GameStore.Dtos;

public record UpdateGameDto
(
    [Required][StringLength(50)] string Name,
    [Required][Range(1, 50)] int GenreId,
    [Required][Range(0.01, 100.00)]  decimal Price,
    DateOnly ReleaseDate
);
