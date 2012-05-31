/*  
 Copyright 2007 The NGenerics Team  
 (http://www.codeplex.com/NGenerics/Wiki/View.aspx?title=Team)

 This program is licensed under the Microsoft Permissive License (Ms-PL).  You should 
 have received a copy of the license along with the source code.  If not, an online copy
 of the license can be found at http://www.codeplex.com/NGenerics/Project/License.aspx.
*/


using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OhioState.Collections
{
    /// <summary>
    /// An implementation of a Heap data structure.
    /// </summary>
    /// <typeparam name="T">The type of item stored in the heap.</typeparam>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    [Serializable]
    public class Heap<T> : ICollection<T>
    {
        #region Globals

        private readonly List<T> data;
        private IComparer<T> comparerToUse;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="Heap{T}"/> class.
        /// </summary>
        /// <example>
        /// <code source="..\..\Examples\ExampleLibraryCSharp\DataStructures\General\HeapExamples.cs" region="Constructor" lang="cs" title="The following example shows how to use the default constructor."/>
        /// <code source="..\..\Examples\ExampleLibraryVB\DataStructures\General\HeapExamples.vb" region="Constructor" lang="vbnet" title="The following example shows how to use the default constructor."/>
        /// </example>
        public Heap() : this(Comparer<T>.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Heap{T}"/> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        /// <example>
        /// <code source="..\..\Examples\ExampleLibraryCSharp\DataStructures\General\HeapExamples.cs" region="ConstructorCapacity" lang="cs" title="The following example shows how to use the capacity constructor."/>
        /// <code source="..\..\Examples\ExampleLibraryVB\DataStructures\General\HeapExamples.vb" region="ConstructorCapacity" lang="vbnet" title="The following example shows how to use the capacity constructor."/>
        /// </example>
        public Heap( int capacity ) : this(capacity, Comparer<T>.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Heap{T}"/> class.
        /// </summary>
        /// <param name="comparer">The comparer to use.</param>
        /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is a null reference (<c>Nothing</c> in Visual Basic).</exception>
        /// <example>
        /// <code source="..\..\Examples\ExampleLibraryCSharp\DataStructures\General\HeapExamples.cs" region="ConstructorComparer" lang="cs" title="The following example shows how to use the comparer constructor."/>
        /// <code source="..\..\Examples\ExampleLibraryVB\DataStructures\General\HeapExamples.vb" region="ConstructorComparer" lang="vbnet" title="The following example shows how to use the comparer constructor."/>
        /// </example>
        public Heap( IComparer<T> comparer )
        {
            //Guard.ArgumentNotNull(comparer, "comparer");

            data = new List<T>();
            data.Add(default(T));  // Add a dummy item so our indexing starts at 1

            comparerToUse = comparer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Heap{T}"/> class.
        /// </summary>
        /// <param name="capacity">The initial capacity of the Heap.</param>
        /// <param name="comparer">The comparer to use.</param>
        /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is a null reference (<c>Nothing</c> in Visual Basic).</exception>
        public Heap( int capacity, IComparer<T> comparer )
        {
            //Guard.ArgumentNotNull(comparer, "comparer");

            data = new List<T>(capacity);
            data.Add(default(T));  // Add a dummy item so our indexing starts at 1

            comparerToUse = comparer;
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Gets the smallest item in the heap (located at the root).
        /// </summary>
        /// <returns>The value of the root of the Heap.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="Heap{T}"/> is empty.</exception>
        /// <example>
        /// <code source="..\..\Examples\ExampleLibraryCSharp\DataStructures\General\HeapExamples.cs" region="Root" lang="cs" title="The following example shows how to use the Root property."/>
        /// <code source="..\..\Examples\ExampleLibraryVB\DataStructures\General\HeapExamples.vb" region="Root" lang="vbnet" title="The following example shows how to use the Root property."/>
        /// </example>
        public T Root
        {
            get
            {
                #region Validation

                if (Count == 0)
                {
                    //throw new InvalidOperationException(Resources.HeapIsEmpty);
                    throw new InvalidOperationException("Heap is empty.");
                }

                #endregion

                return data[1];
            }
        }

        /// <summary>
        /// Removes the smallest item in the heap (located at the root).
        /// </summary>
        /// <returns>The value contained in the root of the Heap.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="Heap{T}"/> is empty.</exception>
        /// <example>
        /// <code source="..\..\Examples\ExampleLibraryCSharp\DataStructures\General\HeapExamples.cs" region="RemoveRoot" lang="cs" title="The following example shows how to use the RemoveRoot method."/>
        /// <code source="..\..\Examples\ExampleLibraryVB\DataStructures\General\HeapExamples.vb" region="RemoveRoot" lang="vbnet" title="The following example shows how to use the RemoveRoot method."/>
        /// </example>
        public T RemoveRoot()
        {
            #region Validation

            if (Count == 0)
            {
                //throw new InvalidOperationException(Resources.HeapIsEmpty);
                throw new InvalidOperationException("Heap is empty");
            }

            #endregion

            // The minimum item to return.
            T minimum = data[1];

            // The last item in the heap
            T last = data[Count];
            data.RemoveAt(Count);

            // If there's still items left in this heap, re-heapify it.
            if (Count > 0)
            {
                // Re-heapify the binary tree to conform to the heap property 
                int counter = 1;

                while ((counter * 2) < (data.Count))
                {
                    int child = counter * 2;

                    if (((child + 1) < (data.Count)) &&
                        (comparerToUse.Compare(data[child + 1], data[child]) < 0))
                    {
                        child++;
                    }

                    if (comparerToUse.Compare(last, data[child]) <= 0)
                    {
                        break;
                    }

                    data[counter] = data[child];
                    counter = child;
                }

                data[counter] = last;
            }

            return minimum;
        }

        /// <summary>
        /// Gets or sets the object used to compare to items on the heap.
        /// </summary>
        public IComparer<T> ComparerToUse
        {
            get { return comparerToUse; }
            set
            {
                if (comparerToUse != value)
                {
                    comparerToUse = value;
                    this.Clear();
                }
            }
        }

        #endregion

        #region ICollection<T>
        /// <summary>
        /// Determines whether the <see cref="ICollection{T}"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="ICollection{T}"/>.</param>
        /// <returns>
        /// true if item is found in the <see cref="ICollection{T}"/>; otherwise, false.
        /// </returns>
        /// <example>
        /// <code source="..\..\Examples\ExampleLibraryCSharp\DataStructures\General\HeapExamples.cs" region="Contains" lang="cs" title="The following example shows how to use the Contains method."/>
        /// <code source="..\..\Examples\ExampleLibraryVB\DataStructures\General\HeapExamples.vb" region="Contains" lang="vbnet" title="The following example shows how to use the Contains method."/>
        /// </example>
        public bool Contains( T item )
        {
            return data.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the <see cref="ICollection{T}"/> to an <see cref="Array"/>, starting at a particular <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="ICollection{T}"/>. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is a null reference (<c>Nothing</c> in Visual Basic).</exception>
        /// <exception cref="ArgumentException"><paramref name="array"/> is multidimensional.-or-<paramref name="arrayIndex"/> is equal to or greater than the length of array.-or-The number of elements in the source <see cref="ICollection{T}"/> is greater than the available space from arrayIndex to the end of the destination array.-or-Type T cannot be cast automatically to the type of the destination array.</exception>
        /// <example>
        /// <code source="..\..\Examples\ExampleLibraryCSharp\DataStructures\General\HeapExamples.cs" region="CopyTo" lang="cs" title="The following example shows how to use the CopyTo method."/>
        /// <code source="..\..\Examples\ExampleLibraryVB\DataStructures\General\HeapExamples.vb" region="CopyTo" lang="vbnet" title="The following example shows how to use the CopyTo method."/>
        /// </example>
        public void CopyTo( T[] array, int arrayIndex )
        {
            #region Validation

            //Guard.ArgumentNotNull(array, "array");

            if ((array.Length - arrayIndex) < Count)
            {
                //throw new ArgumentException(Resources.NotEnoughSpaceInTargetArray, "array");
                throw new ArgumentException("There is not enough space in the targeted array.");
            }

            #endregion

            for (int i = 1; i < data.Count; i++)
            {
                array[arrayIndex++] = data[i];
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="ICollection"/>.
        /// </summary>
        /// <returns>The number of elements contained in the <see cref="ICollection"/>.</returns>
        /// <example>
        /// <code source="..\..\Examples\ExampleLibraryCSharp\DataStructures\General\HeapExamples.cs" region="Count" lang="cs" title="The following example shows how to use the Count property."/>
        /// <code source="..\..\Examples\ExampleLibraryVB\DataStructures\General\HeapExamples.vb" region="Count" lang="vbnet" title="The following example shows how to use the Count property."/>
        /// </example>
        public int Count
        {
            get
            {
                return data.Count - 1;
            }
        }

        /// <summary>
        /// Adds an item to the <see cref="ICollection{T}"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="ICollection{T}"/>.</param>
        /// <exception cref="NotSupportedException">The <see cref="ICollection{T}"/> is read-only.</exception>
        /// <example>
        /// <code source="..\..\Examples\ExampleLibraryCSharp\DataStructures\General\HeapExamples.cs" region="Add" lang="cs" title="The following example shows how to use the Add method."/>
        /// <code source="..\..\Examples\ExampleLibraryVB\DataStructures\General\HeapExamples.vb" region="Add" lang="vbnet" title="The following example shows how to use the Add method."/>
        /// </example>
        public void Add( T item )
        {
            // Add a dummy to the end of the list (it will be replaced)
            data.Add(default(T));

            int counter = data.Count - 1;

            while ((counter > 1) && (comparerToUse.Compare(data[counter / 2], item) > 0))
            {
                data[counter] = data[counter / 2];
                counter = counter / 2;
            }

            data[counter] = item;
        }
        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ICollection{T}"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="ICollection{T}"/>.</param>
        /// <returns>
        /// true if item was successfully removed from the <see cref="ICollection{T}"/>; otherwise, false. This method also returns false if item is not found in the original <see cref="ICollection{T}"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">Always.</exception>
        bool ICollection<T>.Remove( T item )
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="IEnumerator{T}"/> that can be used to iterate through the collection.
        /// </returns>
        /// <example>
        /// <code source="..\..\Examples\ExampleLibraryCSharp\DataStructures\General\HeapExamples.cs" region="GetEnumerator" lang="cs" title="The following example shows how to use the GetEnumerator method."/>
        /// <code source="..\..\Examples\ExampleLibraryVB\DataStructures\General\HeapExamples.vb" region="GetEnumerator" lang="vbnet" title="The following example shows how to use the GetEnumerator method."/>
        /// </example>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 1; i < data.Count; i++)
            {
                yield return data[i];
            }
        }

        /// <summary>
        /// Clears all the objects in this instance.
        /// </summary>
        /// <example>
        /// <code source="..\..\Examples\ExampleLibraryCSharp\DataStructures\General\HeapExamples.cs" region="Clear" lang="cs" title="The following example shows how to use the Clear method."/>
        /// <code source="..\..\Examples\ExampleLibraryVB\DataStructures\General\HeapExamples.vb" region="Clear" lang="vbnet" title="The following example shows how to use the Clear method."/>
        /// </example>
        public void Clear()
        {
            data.RemoveRange(1, data.Count - 1); // Clears all objects in this instance except the first dummy one.

        }

        /// <summary>
        /// Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        /// <example>
        /// <code source="..\..\Examples\ExampleLibraryCSharp\DataStructures\General\HeapExamples.cs" region="IsReadOnly" lang="cs" title="The following example shows how to use the IsReadOnly property."/>
        /// <code source="..\..\Examples\ExampleLibraryVB\DataStructures\General\HeapExamples.vb" region="IsReadOnly" lang="vbnet" title="The following example shows how to use the IsReadOnly property."/>
        /// </example>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>An enumerator for enumerating though the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
