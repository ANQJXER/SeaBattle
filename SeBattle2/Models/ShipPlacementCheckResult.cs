namespace SeBattle2.Models
{
    public record struct ShipPlacementCheckResult(bool Allowed, List<ShipDirection> AllowedDirections);
}
