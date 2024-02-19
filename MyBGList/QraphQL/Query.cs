using MyBGList.Models;

namespace MyBGList.QraphQL;

public class Query
{
    [Serial]
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<BoardGame> GetBoardGames(
        [Service] ApplicationDbContext context)
    {
        return context.BoardGames;
    }

    [Serial]
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Domain> GetDomains(
        [Service] ApplicationDbContext context)
    {
        return context.Domains;
    }

    [Serial]
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Mechanic> GetMechanics(
        [Service] ApplicationDbContext context)
    {
        return context.Mechanics;
    }
}