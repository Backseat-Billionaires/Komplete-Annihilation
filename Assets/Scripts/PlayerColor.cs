public struct PlayerColor
{
    public PlayerColor(int r, int g, int b)
    {
        R = r;
        G = g;
        B = b;
    }

    public int R;
    public int G;
    public int B;

    public static PlayerColor Red = new(255, 102, 102);
    public static PlayerColor Blue = new(102, 102, 255);
    public static PlayerColor Green = new(102, 255, 102);
    public static PlayerColor Yellow = new(255, 255, 102);
    public static PlayerColor Lavender = new(153, 102, 204);
    public static PlayerColor Apricot = new(255, 153, 102);
    public static PlayerColor Teal = new(102, 204, 204);
    public static PlayerColor White = new(211, 211, 211);

}
