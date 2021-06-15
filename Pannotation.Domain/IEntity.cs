using System;

namespace Pannotation.Domain
{
    public interface IEntity<TKey>
         where TKey : IComparable<TKey>, IComparable
    {
        TKey Id { get; set; }
    }

    public interface IEntity : IEntity<int>
    {
    }
}
