using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;

namespace EventProject
{
    /// <summary>
    /// This binding list sorting implementation is taken from this link:
    /// http://msdn.microsoft.com/en-us/library/aa480736.aspx
    /// The code had to be altered, though, as sorting of items with keys
    /// being the same didn't work correctly. The change, though, enforces
    /// that the sorted values are strings. If you need some other datatype,
    /// please alter the code accordingly.
    /// </summary>    
    [Serializable]
    public class BindingListWithSort<T> : BindingList<T>
    {
        ListSortDirection sortDirectionValue;
        [NonSerialized]
        PropertyDescriptor sortPropertyValue;

        protected override PropertyDescriptor SortPropertyCore
        {
            get { return sortPropertyValue; }
        }

        protected override ListSortDirection SortDirectionCore
        {
            get { return sortDirectionValue; }
        }

        protected override bool SupportsSortingCore
        {
            get { return true; }
        }

        public void RemoveSort()
        {
            RemoveSortCore();
        }

        private List<object[]> sortedList;
        ArrayList unsortedItems;

        protected override void ApplySortCore(PropertyDescriptor prop,
            ListSortDirection direction)
        {
            sortedList = new List<object[]>();

            // Check to see if the property type we are sorting by implements
            // the IComparable interface.
            Type interfaceType = prop.PropertyType.GetInterface("IComparable");

            if (interfaceType != null)
            {
                // If so, set the SortPropertyValue and SortDirectionValue.
                sortPropertyValue = prop;
                sortDirectionValue = direction;

                unsortedItems = new ArrayList(this.Count);

                // Loop through each item, adding it the the sortedItems ArrayList.
                //have to add not only sorted key, but also the original object
                //otherwise, when multiple items have multiple keys, we won't get
                //proper sorting (or, more specifically, may get it or may not get it)
                foreach (Object item in this.Items)
                {
                    sortedList.Add(new object[] {prop.GetValue(item), item});
                    unsortedItems.Add(item);
                }

                // Call Sort on the ArrayList.
                //sortedList.Sort();
                sortedList.Sort(delegate(object[] t1, object[] t2)
                { 
                    return (t1[0] as String).CompareTo(t2[0] as String); 
                });
                //T temp;

                // Check the sort direction and then copy the sorted items
                // back into the list.
                if (direction == ListSortDirection.Descending)
                    sortedList.Reverse();

                for (int i = 0; i < this.Count; i++)
                {
                    this[i] = (T)(sortedList[i] as object[])[1];
                }
/*
                for (int i = 0; i < this.Count; i++)
                {
                    int position = Find(prop.Name, sortedList[i]);
                    if (position != i)
                    {
                        temp = this[i];
                        this[i] = this[position];
                        this[position] = temp;
                    }
                }
*/
                // Raise the ListChanged event so bound controls refresh their
                // values.
                OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
            }
            else
                // If the property type does not implement IComparable, let the user
                // know.
                throw new NotSupportedException("Cannot sort by " + prop.Name + ". This" +
                    prop.PropertyType.ToString() + " does not implement IComparable");
        }

        protected override void RemoveSortCore()
        {
            int position;
            object temp;
            // Ensure the list has been sorted.
            if (unsortedItems != null)
            {
                // Loop through the unsorted items and reorder the
                // list per the unsorted list.
                for (int i = 0; i < unsortedItems.Count; )
                {
                    position = this.Find(SortPropertyCore.Name,
                        unsortedItems[i].GetType().
                        GetProperty(SortPropertyCore.Name).
                        GetValue(unsortedItems[i], null));
                    if (position >= 0 && position != i)
                    {
                        temp = this[i];
                        this[i] = this[position];
                        this[position] = (T)temp;
                        i++;
                    }
                    else if (position == i)
                        i++;
                    else
                        // If an item in the unsorted list no longer exists, delete it.
                        unsortedItems.RemoveAt(i);
                }
                OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
            }
        }

        protected override bool SupportsSearchingCore
        {
            get
            {
                return true;
            }
        }

        protected override int FindCore(PropertyDescriptor prop, object key)
        {
            // Get the property info for the specified property.
            PropertyInfo propInfo = typeof(T).GetProperty(prop.Name);
            T item;

            if (key != null)
            {
                // Loop through the the items to see if the key
                // value matches the property value.
                for (int i = 0; i < Count; ++i)
                {
                    item = (T)Items[i];
                    if (propInfo.GetValue(item, null).Equals(key))
                        return i;
                }
            }
            return -1;
        }

        public int Find(string property, object key)
        {
            // Check the properties for a property with the specified name.
            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(T));
            PropertyDescriptor prop = properties.Find(property, true);

            // If there is not a match, return -1 otherwise pass search to
            // FindCore method.
            if (prop == null)
                return -1;
            else
                return FindCore(prop, key);
        }
    }
}
