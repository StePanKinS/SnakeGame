namespace SnakeGame
{
    /// <summary>
    /// Represents the snake haed
    /// </summary>
    public class Head
    {
        public int x { get; set; }
        public int y { get; set; }
        public Direction direction { get; set; }

        public Head(int x, int y, Direction direction)
        {
            this.x = x;
            this.y = y;
            this.direction = direction;
        }

    }

    /// <summary>
    /// Represents the snake tail
    /// </summary>
    public class Tail
    {
        public int x { get; set; }
        public int y { get; set; }
        public Direction direction { get; set; }
        public Tail(int x, int y, Direction direction)
        {
            this.x = x;
            this.y = y;
            this.direction = direction;
        }
    }

    /// <summary>
    /// Represents the snake body element
    /// </summary>
    public class Body
    {
        public int x { get; set; }
        public int y { get; set; }
        public Direction From { get; set; }
        public Direction To { get; set; }

        public Body(int x, int y, Direction from, Direction to)
        {
            this.x = x;
            this.y = y;
            From = from;
            To = to;
        }
    }
}
