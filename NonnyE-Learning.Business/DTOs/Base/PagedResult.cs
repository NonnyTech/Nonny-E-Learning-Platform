using System.Collections.Generic;

namespace NonnyE_Learning.Business.DTOs.Base
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
    }
}
