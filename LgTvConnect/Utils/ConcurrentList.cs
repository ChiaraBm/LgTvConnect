using System.Collections;

namespace LgTvConnect.Utils;

/// <summary>
/// Threadsafe implementation of <see cref="IList{T}"/>
/// </summary>
/// <typeparam name="T">Type to store in the list</typeparam>
public class ConcurrentList<T> : IList<T>
{
    private readonly List<T> InnerList = new List<T>();
    private readonly ReaderWriterLockSlim Lock = new ReaderWriterLockSlim();

    /// <inheritdoc />
    public T this[int index]
    {
        get
        {
            Lock.EnterReadLock();
            try { return InnerList[index]; }
            finally { Lock.ExitReadLock(); }
        }
        set
        {
            Lock.EnterWriteLock();
            try { InnerList[index] = value; }
            finally { Lock.ExitWriteLock(); }
        }
    }

    /// <inheritdoc />
    public int Count
    {
        get
        {
            Lock.EnterReadLock();
            try { return InnerList.Count; }
            finally { Lock.ExitReadLock(); }
        }
    }

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <inheritdoc />
    public void Add(T item)
    {
        Lock.EnterWriteLock();
        try { InnerList.Add(item); }
        finally { Lock.ExitWriteLock(); }
    }

    /// <inheritdoc />
    public void Clear()
    {
        Lock.EnterWriteLock();
        try { InnerList.Clear(); }
        finally { Lock.ExitWriteLock(); }
    }

    /// <inheritdoc />
    public bool Contains(T item)
    {
        Lock.EnterReadLock();
        try { return InnerList.Contains(item); }
        finally { Lock.ExitReadLock(); }
    }

    /// <inheritdoc />
    public void CopyTo(T[] array, int arrayIndex)
    {
        Lock.EnterReadLock();
        try { InnerList.CopyTo(array, arrayIndex); }
        finally { Lock.ExitReadLock(); }
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
    {
        List<T> snapshot;
        Lock.EnterReadLock();
        try { snapshot = new List<T>(InnerList); }
        finally { Lock.ExitReadLock(); }

        return snapshot.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public int IndexOf(T item)
    {
        Lock.EnterReadLock();
        try { return InnerList.IndexOf(item); }
        finally { Lock.ExitReadLock(); }
    }

    /// <inheritdoc />
    public void Insert(int index, T item)
    {
        Lock.EnterWriteLock();
        try { InnerList.Insert(index, item); }
        finally { Lock.ExitWriteLock(); }
    }

    /// <inheritdoc />
    public bool Remove(T item)
    {
        Lock.EnterWriteLock();
        try { return InnerList.Remove(item); }
        finally { Lock.ExitWriteLock(); }
    }

    /// <inheritdoc />
    public void RemoveAt(int index)
    {
        Lock.EnterWriteLock();
        try { InnerList.RemoveAt(index); }
        finally { Lock.ExitWriteLock(); }
    }

    /// <summary>
    /// Removes a range of elements from the list
    /// </summary>
    /// <param name="index">Index to start deleting elements</param>
    /// <param name="count">Amount of elements to remove</param>
    public void RemoveRange(int index, int count)
    {
        Lock.EnterWriteLock();
        try { InnerList.RemoveRange(index, count); }
        finally { Lock.ExitWriteLock(); }
    }
}