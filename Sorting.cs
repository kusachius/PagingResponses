using System;
namespace Kusachius.PagingResponses
{
    public class Sorting
    {
        public string Column { get; }
        public string Direction { get; }

        public Sorting(string column, string direction = "asc")
        {
            if (string.IsNullOrEmpty(direction))
            {
                direction = "asc";
            }

            direction = direction.ToLower();

            if (direction != "asc" && direction != "desc")
            {
                throw new Exception("Invalid direction 'direction', must be asc or desc");
            }

            this.Column = column;
            this.Direction = direction;
        }
    }
}
