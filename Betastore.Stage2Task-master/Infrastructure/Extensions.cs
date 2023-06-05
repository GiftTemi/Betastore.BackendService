using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure
{
    public static class Extensions
    {
        public static ICollection<T> SkipAndTake<T>(this List<T> list, out int count, int? skip = 0, int? take = 0)
        {
            List<T> items = new();
            count = 0;
            try
            {
                if (list != null)
                {
                    count = list.Count;
                    if (skip == 0 & take == 0)
                    {
                        items = list;
                    }
                    else if (skip > 0 & take == 0)
                    {
                        items = list.Skip(skip.Value).ToList();
                    }
                    else if (skip == 0 & take > 0)
                    {
                        items = list.Take(take.Value).ToList();
                    }
                    else
                    {
                        items = list.Skip(skip.Value).Take(take.Value).ToList();
                    }
                }
            }
            catch (Exception ex) { }
            return items;
        }

        public static ICollection<T> SkipAndTake<T>(this List<T> list, int? skip = 0, int? take = 0)
        {
            List<T> items = new();
            try
            {
                if (list != null)
                {
                    if (skip == 0 & take == 0)
                    {
                        items = list;
                    }
                    else if (skip > 0 & take == 0)
                    {
                        items = list.Skip(skip.Value).ToList();
                    }
                    else if (skip == 0 & take > 0)
                    {
                        items = list.Take(take.Value).ToList();
                    }
                    else
                    {
                        items = list.Skip(skip.Value).Take(take.Value).ToList();
                    }
                }
            }
            catch (Exception ex) { }
            return items;
        }
        public static string ToStringItems<T>(this IEnumerable<T> items, string separator = ",")
        {
            return items != null ? string.Join(separator, items) : null;
        }
    }
}
