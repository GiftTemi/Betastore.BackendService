using System.Collections.Generic;
using System.Linq;

namespace Domain.Common.Models
{
    public class Result
    {
        internal Result(bool succeeded, IEnumerable<string> messages)
        {
            Succeeded = succeeded;
            Messages = messages.ToArray();
        }

        internal Result(bool succeeded, string message)
        {
            Succeeded = succeeded;
            Message = message;
        }

        internal Result(bool succeeded, string message, object result)
        {
            Succeeded = succeeded;
            Entity = result;
            Message = message;
        }

        internal Result(bool succeeded, object result)
        {
            Succeeded = succeeded;
            Entity = result;
        }

        internal Result(bool succeeded, object result, object permissions)
        {
            Succeeded = succeeded;
            Entity = result;
            Permissions = permissions;
        }

        public bool Succeeded { get; set; }

        public object Entity { get; set; }

        public object Permissions { get; set; }

        public string Message { get; set; }

        public string[] Messages { get; set; }

        public static Result Success()
        {
            return new Result(true, new string[] { });
        }
        public static Result Success(string message)
        {
            return new Result(true, message);
        }

        public static Result Success(string message, object entity)
        {
            return new Result(true, message, entity);
        }

        public static Result Failure(string message, object entity)
        {
            return new Result(false, message, entity);
        }

        public static Result Success(string message, object entity, object Permissions)
        {
            return new Result(true, entity, Permissions);
        }

        public static Result Success(object entity)
        {
            return new Result(true, entity);
        }
        public static Result Failure(object entity)
        {
            return new Result(false, entity);
        }

        public static Result Failure(IEnumerable<string> errors)
        {
            return new Result(false, errors);
        }

        public static Result Failure(string error)
        {
            return new Result(false, error);
        }
    }
}
