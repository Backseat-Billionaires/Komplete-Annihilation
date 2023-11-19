public class PlayerSpawnOptions
{
    public PlayerSpawnOptions(int spawnOrder, PlayerColor color)
    {
        SpawnOrder = spawnOrder;
        Color = color;
    }

    public int SpawnOrder { get; set; }

    public PlayerColor Color { get; set; }

    public bool IsActivePlayer { get; set; }
}
