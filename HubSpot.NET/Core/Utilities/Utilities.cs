using System;
using System.Collections;
using System.Collections.Generic;
using HubSpot.NET.Api.Contact.Dto;

namespace HubSpot.NET.Core.Utilities
{
    public class Utilities
    {
        /// <summary>
        /// UnitOfTime enum. This is intended to be used with the Sleep method, below.
        /// </summary>
        public enum UnitOfTime
        {
            Milliseconds = 1,
            Seconds = 1000,
            Minutes = 1000 * 60,
            Hours = 1000 * 60 * 60
        }
        
        /// <summary>
        /// Sleep for n units of time.
        /// </summary>
        /// <param name="duration">
        /// Specifies a duration of n units of time; defaults to 1.
        /// </param>
        /// <param name="unitOfTime">
        /// Specifies the time unit; defaults to seconds. 
        /// </param>
        /// <remarks>
        /// When called with no parameters, this method will sleep for one second by default.
        /// </remarks>
        public static void Sleep(int duration = 1, UnitOfTime unitOfTime = UnitOfTime.Seconds)
        {
            System.Threading.Thread.Sleep(duration * (int)unitOfTime);
        }
    }
   
    /// <summary>
    /// Represents a collection of objects that can be individually accessed by index. The size of the
    /// collection can be limited via the `maxItems` parameter.
    /// </summary>
    /// <param name="maxItems">The maximum number of items this list is allowed to contain.</param>
    public class LimitedList<T> : IList<T>
    {
        public LimitedList(int maxItems = 0)
        {
            if (maxItems < 0)
                throw new ArgumentOutOfRangeException($"Maximum items cannot be a negative integer.");
            _maxItems = maxItems;
        }

        private readonly int _maxItems;

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
    
    // TODO - Equality comparers for all other models
    
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