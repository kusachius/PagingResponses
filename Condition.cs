using System;
using System.Linq;

namespace Kusachius.PagingResponses
{
    public class Condition
    {
        private string[] operations = { "=", ">", "<", ">=", "<=", "like" };

        public string Column { get; }
        public string Operation { get; }
        public string Value { get; }


        public Condition(string column, string operation, string value)
        {
            if (!operations.Contains(operation))
            {
                throw new Exception("Invalid operator " + operation);
            }

            Column = column;
            Operation = operation;
            Value = value;
        }

    }
}
