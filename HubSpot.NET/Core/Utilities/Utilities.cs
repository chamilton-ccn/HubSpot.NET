using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using HubSpot.NET.Api.Contact.Dto;

namespace HubSpot.NET.Core.Utilities
{
    public class Utilities
    {
        /// <summary>
        ///     UnitOfTime enum. This is intended to be used with the Sleep method, below.
        /// </summary>
        public enum UnitOfTime
        {
            Milliseconds = 1,
            Seconds = 1000,
            Minutes = 1000 * 60,
            Hours = 1000 * 60 * 60
        }

        /// <summary>
        ///     Sleep for n units of time.
        /// </summary>
        /// <param name="duration">
        ///     Specifies a duration of n units of time; defaults to 1.
        /// </param>
        /// <param name="unitOfTime">
        ///     Specifies the time unit; defaults to seconds.
        /// </param>
        /// <remarks>
        ///     When called with no parameters, this method will sleep for one second by default.
        /// </remarks>
        public static void Sleep(int duration = 1, UnitOfTime unitOfTime = UnitOfTime.Seconds)
        {
            Thread.Sleep(duration * (int)unitOfTime);
        }

        /// <summary>
        ///     Invoke the delegate specified by the operation parameter on each item in the batch. Exponential backoff is
        ///     enabled by default (see parameters: attempts, retryDelay, unitOfTime, and jitter).
        /// </summary>
        /// <param name="operation">A delegate that will be invoked for each item in the batch</param>
        /// <param name="batch">An enumerable containing HubSpot objects</param>
        /// <param name="attempts">The number of times to attempt an operation</param>
        /// <param name="retryDelay">The delay between attempts</param>
        /// <param name="timeUnit">The unit of time of the delay</param>
        /// <param name="jitter">Enables/disables a random pad on the retry delay, between 1ms and 3s</param>
        /// <typeparam name="T">T is T</typeparam>
        /// <returns>
        ///     A tuple containing a list of objects of type T, for whom the operation was successful (Item1), and a tuple
        ///     (Item2) containing the failed objects (Item1) and exceptions (Item2) that were thrown during the process.
        /// </returns>
        /// TODO - This needs a unit test!
        public static Tuple<IList<T>, IList<Tuple<T, Exception>>> UnrollBatch<T>(
            Delegate operation,
            IEnumerable<T> batch,
            int attempts = 2,
            int retryDelay = 500,
            UnitOfTime timeUnit = UnitOfTime.Milliseconds,
            bool jitter = true)
        {
            var jitterValue = 0;
            if (jitter)
            {
                var random = new Random();
                jitterValue = random.Next(1, 3000);
            }

            var successfulResults = new List<T>();
            var unsuccessfulResults = new List<Tuple<T, Exception>>();
            foreach (var item in batch)
            {
                var attemptCount = 0;
                while (attemptCount < attempts)
                {
                    try
                    {
                        var result = (T)operation.DynamicInvoke(item);
                        successfulResults.Add(result);
                        break;
                    }
                    catch (Exception e)
                    {
                        unsuccessfulResults.Add(new Tuple<T, Exception>(item, e));
                        if (attempts > 1)
                        {
                            Sleep(retryDelay * (attemptCount + 1), timeUnit);
                            Sleep(jitterValue, UnitOfTime.Milliseconds); // Jitter should always be in Milliseconds
                        }
                    }

                    attemptCount++;
                }
            }

            return new Tuple<IList<T>, IList<Tuple<T, Exception>>>(successfulResults, unsuccessfulResults);
        }
    }


    /// <summary>
    ///     Represents a collection of objects that can be individually accessed by index. The size of the
    ///     collection can be limited via the `maxItems` parameter.
    /// </summary>
    /// <param name="maxItems">The maximum number of items this list is allowed to contain.</param>
    public class LimitedList<T> : IList<T>
    {
        private readonly IList<T> _list = new List<T>();

        private readonly int _maxItems;

        public LimitedList(int maxItems = 0)
        {
            if (maxItems < 0)
                throw new ArgumentOutOfRangeException(nameof(maxItems));
            _maxItems = maxItems;
        }

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
                _list.Add(item);
            else if (Count + 1 > _maxItems)
                throw new ArgumentOutOfRangeException(nameof(Add),
                    $"Maximum capacity of this list is: {_maxItems}");
            else
                _list.Add(item);
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
    ///     Determines whether two ContactHubSpotModel instances should be treated as equals by comparing both their <c>Id</c>
    ///     <i>and</i> <c>Email</c> properties. HubSpot treats the email address as a quasi-unique identifier. Let's say you
    ///     have a contact, with HubSpot ID #123456, and its <c>Email</c> property is <c>someuser@domain.tld</c>. At some
    ///     point, one of your HubSpot users merged this contact record with another contact record whose <c>Email</c> property
    ///     was <c>someusers-other-email-address@domain.tld</c>; the implication being these two contacts were merged because
    ///     they were merely two separate email addresses referring to the same <i>person</i>. A few things happen in this
    ///     scenario:
    ///     <ul>
    ///         <li>
    ///             The <c>hs_additional_emails</c> read-only property will be populated with the email address(-es) of every
    ///             contact that was merged into the initial contact.
    ///         </li>
    ///         <li>
    ///             The contact record associated with HubSpot ID #123456 can now be retrieved using <i>any</i> email address
    ///             belonging to this contact. I.e., both <c>someuser@domain.tld</c> and
    ///             <c>someusers-other-email-address@domain.tld</c>
    ///             refer to a contact record with the HubSpot ID #123456.
    ///         </li>
    ///     </ul>
    ///     So when comparing contact objects for equality, we must first check see if both <c>Id</c> and <c>Email</c>
    ///     properties are populated.
    ///     <ul>
    ///         <li>
    ///             If they are, and <i>both</i> <c>Id</c> and <c>Email</c> are identical on <i>both</i> contact objects, then
    ///             they shall be treated as equal.
    ///         </li>
    ///         <li>
    ///             However, if the <c>Id</c> properties match, but the <c>Email</c> properties <i>do not match</i> we may need
    ///             to treat these as separate contact records, even though they are <i>technically</i> identical, as far as
    ///             HubSpot is concerned.
    ///         </li>
    ///     </ul>
    /// </summary>
    public class ContactHubSpotModelCompareByIdAndEmail : IEqualityComparer<ContactHubSpotModel>
    {
        public bool Equals(ContactHubSpotModel thisContact, ContactHubSpotModel thatContact)
        {
            if ((thisContact?.Id != null) & (thatContact?.Id != null) &&
                (thisContact?.Email != null) & (thatContact?.Email != null))
                return (thisContact?.Id == thatContact?.Id) &
                       string.Equals(thisContact?.Email, thatContact?.Email, StringComparison.CurrentCultureIgnoreCase);
            throw new InvalidOperationException("'Id' and 'Email' properties are both required for comparison!");
        }

        public int GetHashCode(ContactHubSpotModel contact)
        {
            return $"{contact.Id}_{contact.Email}".GetHashCode();
        }
    }

    public class ContactHubSpotModelCompareByIdOrEmail : IEqualityComparer<ContactHubSpotModel>
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