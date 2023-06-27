using System;
using System.Collections.Generic;

namespace SnakeGame
{
    public class WallCracked : Exception { }
    public class SelfCracked : Exception { }
    public class NoMorePlase : Exception { }

    /// <summary>
    /// Represents the state of the game and provides logic to go to the next step
    /// </summary>
    public class GameState
    {
        private static Random random = new Random();

        public Head head { get; protected set; }
        public List<Body> body { get; protected set; }
        public Tail tail { get; protected set; }

        public (int x, int y) apple { get; protected set; }
        public int score { get; protected set; }
        public int applesCount { get; protected set; }
        public int speedLevel { get; protected set; }
        public int[] changeSpeedLevelOn {  get; protected set; }

        public Action speedLevelChanged { get; set; }


        public int width { get; protected set; }
        public int height { get; protected set; }

        /// <summary>
        /// Creates the game
        /// </summary>
        /// <param name="settings"> Game settings </param>
        public GameState(GameSettings settings) : this(settings.MapWidth, settings.MapHeight, settings.HeadStartX, settings.HeadStartY, settings.StartDirection, settings.ChangeSpeedLevelOn) { }

        /// <summary>
        /// Creates the game
        /// </summary>
        /// <param name="mapWidth"> Map width </param>
        /// <param name="mapHeight"> Map height </param>
        /// <param name="xHeadPos"> Head position on start from left wall </param>
        /// <param name="yHeadPos"> Head position on start from top wall </param>
        /// <param name="direction"> Snake start direction </param>
        /// <param name="changeSpeedLevelOn"> How much apples needs to collect to increase speed on all speen levels </param>
        /// <exception cref="ArgumentException"/>
        public GameState(int mapWidth, int mapHeight, int xHeadPos, int yHeadPos, Direction direction, int[] changeSpeedLevelOn)
        {
            score = 0;
            speedLevel = 1;
            applesCount = 0;
            this.changeSpeedLevelOn = changeSpeedLevelOn;

            speedLevelChanged = () => { };

            head = new Head(xHeadPos, yHeadPos, direction);
            body = new List<Body>();

            height = mapHeight;
            width = mapWidth;

            switch (direction)
            {
                case Direction.Right:
                    {
                        if (xHeadPos < 3 || yHeadPos < 0 || xHeadPos >= mapWidth || yHeadPos >= mapHeight)
                            throw new ArgumentException("Can not place snake out of screen");


                        body.Add(new Body(xHeadPos - 1, yHeadPos, Direction.Left, direction));
                        body.Add(new Body(xHeadPos - 2, yHeadPos, Direction.Left, direction));

                        tail = new Tail(xHeadPos - 3, yHeadPos, direction);

                        break;
                    }
                case Direction.Left:
                    {
                        if (xHeadPos < 0 || yHeadPos < 0 || xHeadPos + 3 <= mapWidth || yHeadPos >= mapHeight)
                            throw new ArgumentException("Can not place snake out of screen");


                        body.Add(new Body(xHeadPos + 1, yHeadPos, Direction.Right, direction));
                        body.Add(new Body(xHeadPos + 2, yHeadPos, Direction.Right, direction));

                        tail = new Tail(xHeadPos + 3, yHeadPos, direction);

                        break;
                    }
                case Direction.Up:
                    {
                        if (xHeadPos < 0 || yHeadPos < 0 || xHeadPos >= mapWidth || yHeadPos + 3 >= mapHeight)
                            throw new ArgumentException("Can not place snake out of screen");


                        body.Add(new Body(xHeadPos, yHeadPos + 1, Direction.Down, direction));
                        body.Add(new Body(xHeadPos, yHeadPos + 2, Direction.Down, direction));

                        tail = new Tail(xHeadPos, yHeadPos + 3, direction);

                        break;
                    }
                case Direction.Down:
                    {
                        if (xHeadPos < 0 || yHeadPos < 3 || xHeadPos >= mapWidth || yHeadPos >= mapHeight)
                            throw new ArgumentException("Can not place snake out of screen");


                        body.Add(new Body(xHeadPos, yHeadPos - 1, Direction.Up, direction));
                        body.Add(new Body(xHeadPos, yHeadPos - 2, Direction.Up, direction));

                        tail = new Tail(xHeadPos, yHeadPos - 3, direction);

                        break;
                    }
                default:
                    {
                        throw new ArgumentException();
                    }
            }


            apple = NewApplePos();
        }

        /// <summary>
        /// Moves snake to the next step
        /// </summary>
        /// <param name="direction"> In which direction snake goes </param>
        /// <exception cref="WallCracked"/>
        /// <exception cref="SelfCracked"/>
        /// <exception cref="ArgumentException"/>
        public void Go(Direction direction)
        {
            if (direction == head.direction.Opposit())
                direction = head.direction;

            (int x, int y) newHeadCords;

            switch (direction)
            {
                case Direction.Up:
                    {
                        newHeadCords = (head.x, head.y - 1);


                        if (newHeadCords.y == -1)
                            throw new WallCracked();


                        break;
                    }
                case Direction.Left:
                    {
                        newHeadCords = (head.x - 1, head.y);


                        if (newHeadCords.x == -1)
                            throw new WallCracked();


                        break;
                    }
                case Direction.Down:
                    {
                        newHeadCords = (head.x, head.y + 1);


                        if (newHeadCords.y == height)
                            throw new WallCracked();


                        break;
                    }
                case Direction.Right:
                    {
                        newHeadCords = (head.x + 1, head.y);


                        if (newHeadCords.x == width)
                            throw new WallCracked();


                        break;
                    }
                default:
                    {
                        throw new ArgumentException();
                    }
            }

            foreach (Body bodyEl in body)
            {
                if (newHeadCords == (bodyEl.x, bodyEl.y))
                    throw new SelfCracked();
            }
            if (newHeadCords == (tail.x, tail.y))
                throw new SelfCracked();

            body.Insert(0, new Body(head.x, head.y, head.direction.Opposit(), direction));

            head.x = newHeadCords.x;
            head.y = newHeadCords.y;
            head.direction = direction;

            if (newHeadCords == apple)
            {
                apple = NewApplePos();
                score += 5 * speedLevel * speedLevel;
                applesCount++;

                if(speedLevel != changeSpeedLevelOn.Length + 1 && applesCount == changeSpeedLevelOn[speedLevel - 1])
                {
                    speedLevel++;
                    speedLevelChanged();
                    applesCount = 0;
                }
            }
            else
            {
                tail.x = body[body.Count - 1].x;
                tail.y = body[body.Count - 1].y;
                tail.direction = body[body.Count - 1].To;
                body.RemoveAt(body.Count - 1);
            }
        }

        /// <summary>
        /// Finds the place to place apple
        /// </summary>
        /// <returns> Position </returns>
        /// <exception cref="NoMorePlase"/>
        public (int, int) NewApplePos()
        {
            (int x, int y) pos = (random.Next(width), random.Next(height));
            (int, int) startPos = pos;

            bool cont;

            while (true)
            {
                cont = false;

                if (pos == (head.x, head.y))
                    PositionPlusPlus();

                if (pos == (tail.x, tail.y))
                    PositionPlusPlus();



                foreach (Body body in body)
                {
                    if (pos == (body.x, body.y))
                    {
                        PositionPlusPlus();
                    }
                }

                if (cont)
                    continue;

                return pos;
            }


            void PositionPlusPlus()
            {
                pos.x++;
                if (pos.x == width)
                {
                    pos.x = 0;
                    pos.y++;

                    if (pos.y == height)
                    {
                        pos.y = 0;
                    }
                }

                cont = true;

                if (pos == startPos)
                    throw new NoMorePlase();
            }
        }
    }
}
