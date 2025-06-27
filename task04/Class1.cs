namespace task04;

public interface ISpaceship
{
    void MoveForward();
    void Rotate(int angle);
    void Fire();
    int Speed { get; }
    int FirePower { get; }
}
public class Cruiser : ISpaceship
{
    public int Speed { get; set; } = 50;
    public int FirePower { get; set; } = 100;
    public void MoveForward()
    { 
        Console.WriteLine($"Крейсер плывёт со скоростью {Speed} единиц");
    }
    public void Rotate(int angle)
    {
         Console.WriteLine($"Крейсер повернул на {angle} градусов");
     }
    public void Fire()
    { 
        Console.WriteLine($"Крейсер стреляет ракетами мощностью {FirePower} единиц");
    }
}
public class Fighter : ISpaceship
{
    public int Speed { get; set; } = 100;
    public int FirePower { get; set; } = 50;
    public void MoveForward()
    {
        Console.WriteLine($"Истребитель летит со скоростью {Speed} единиц");
    }
    public void Rotate(int angle)
    {
        Console.WriteLine($"Истребитель повернул на {angle} градусов");
    }
    public void Fire()
    {
        Console.WriteLine($"Истребитель стреляет ракетами мощностью {FirePower} единиц");
    }
}
