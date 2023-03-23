using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StarterTemplate.Core.Entities
{
    /// <summary>
    /// A Generic paginated list that can be used to help the front end navigate the data
    /// </summary>
    /// <remarks>
    /// Useful for large lists where the user needs to scroll through many pages
    /// </remarks>
    public class PagedListResult<T>
    {
        public PagedListResult()
        {
            Total = 0;
            Start = 0;
            Limit = 0;
            AmountReturned = 0;
            Items = new List<T>();
        }

        public PagedListResult(int capacity)
        {
            Total = 0;
            Start = 0;
            Limit = 0;
            AmountReturned = 0;
            Items = new List<T>(capacity);
        }

        //used to create a list with 1 item
        public PagedListResult(List<T> items)
        {
            Total = 1;
            Start = 0;
            Limit = 1;
            AmountReturned = 1;
            Items = items;
        }

        public PagedListResult(int total, int start, int limit, int amountReturned, List<T> items)
        {
            Total = total;
            Start = start;
            Limit = limit;
            AmountReturned = amountReturned;
            Items = items;
        }

        [Required]
        public int AmountReturned { get; set; }

        public List<T> Items { get; set; }

        [Required]
        public int Limit { get; set; }

        [Required]
        public int Start { get; set; }

        [Required]
        public int Total { get; set; }

        public void SetValues(int total, int start, int limit, int amountReturned, List<T> items)
        {
            Total = total;
            Start = start;
            Limit = limit;
            AmountReturned = amountReturned;
            Items = items;
        }
    }
}