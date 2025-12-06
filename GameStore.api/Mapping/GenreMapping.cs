using System;
using GameStore.api.Dtos;
using GameStore.api.Entities;

namespace GameStore.api.Mapping;

public static class GenreMapping
{
    public static GenreDto toDto(this Genre genre)
    {
        return new GenreDto(genre.Id, genre.Name);
    }
}
