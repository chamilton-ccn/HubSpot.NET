using System;
using System.Collections;
using System.Collections.Generic;
using HubSpot.NET.Api.Contact.Dto;

namespace HubSpot.NET.Core.Utilities
{
    public class Utilities
    {
        /// <summary>
        /// Sleep for n seconds
        /// </summary>
        /// <param name="duration">
        /// The number of seconds to sleep
        /// </param>
        public static void Sleep(int duration = 20)
        {
            System.Threading.Thread.Sleep(duration * 1000);
        }
    }
    
    /// <summary>
    /// Represents a collection of objects that can be individually accessed by index.
    /// </summary>
    /// <typeparam name="T">T is T</typeparam>
    public class LimitedList<T> : IList<T>
    {
        /// <summary>
        /// Represents a collection of objects that can be individually accessed by index. The size of the
        /// collection can be limited via the `maxItems` parameter.
        /// </summary>
        /// <param name="maxItems">The maximum number of items this list is allowed to contain.</param>
        public LimitedList(int maxItems = 0)
        {
            _maxItems = maxItems;
        }

        private int _maxItems;

        private readonly IList<T> _list = new List<T>();

        public T this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public int Count => _list.Count;

        public bool IsReadOnly => _list.IsReadOnly;

        public void Add(T item)
        {
            if (_maxItems == 0)
            {
                _list.Add(item);
            }
            else if ((Count + 1) > _maxItems)
            {
                throw new ArgumentOutOfRangeException(nameof(Add),
                    $"Maximum capacity of this list is: {_maxItems}");
            }
            else
            {
                _list.Add(item);
            }
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _list.Insert(index, item);
        }

        public bool Remove(T item)
        {
            return _list.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_list).GetEnumerator();
        }
    }
    
    // TODO - Equality comparer for all other models
    
    /// <summary>
    /// ContactHubSpotModelComparer - Determines whether or not two ContactHubSpotModel instances should be treated as
    /// equals.
    /// </summary>
    public class ContactHubSpotModelComparer : IEqualityComparer<ContactHubSpotModel>
    {
        public bool Equals(ContactHubSpotModel thisContact, ContactHubSpotModel thatContact)
        {
            if ((thisContact?.Id != null) & (thatContact?.Id != null))
                return thisContact?.Id == thatContact?.Id;
            if ((thisContact?.Email != null) & (thatContact?.Email != null))
                return string.Equals(thisContact?.Email, thatContact?.Email,
                    StringComparison.CurrentCultureIgnoreCase);
            throw new InvalidOperationException("'Id' or 'Email' property are required for comparison!");
        }

        public int GetHashCode(ContactHubSpotModel contact)
        {
            if (contact.Id != null)
                return contact.Id.GetHashCode();
            if (contact.Email != null)
                return contact.Email.GetHashCode();
            throw new InvalidOperationException("'Id' or 'Email' property are required to get hash code!");
        }
        
    }
    
}