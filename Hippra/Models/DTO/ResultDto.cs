using Hippra.Models.Enums;
using System;
using System.Collections.Generic;

namespace Hippra.Models.ViewModel
{
    public class Result
    {
        private Result(bool isSuccess, long id, List<string> errors)
        {
            IsSuccess = isSuccess;
            EntityId = id;
            if (errors != null)
            {
                Errors = errors;
            }
        }
        public long EntityId { get; set; }
        public bool IsSuccess { get; set; }
        public IList<string> Errors { get; set; }
        public static Result Success(long id=0) => new(true, id,null);

        public static Result Failure(List<string> errors) => new(false, 0, errors);
    }
}
