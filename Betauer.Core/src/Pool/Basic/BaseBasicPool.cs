namespace Betauer.Core.Pool.Basic;

/// <summary>
/// A pool that uses a lifo/fifo collection to store the elements.
/// Get() will create (and return) a new element when the pool is empty
/// When an element is requested, it disappears from the pool.
/// Elements need to be returned to the pool to be reused.
///
/// This Pool acts like a collection where you put element you don't use, and
/// when you can extract elements to use them and return to them later.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BaseBasicPool<T> {
    public PoolCollection<T> Pool { get; }

    protected BaseBasicPool(PoolCollection<T>? pool = null) {
        Pool = pool ?? new PoolCollection.Stack<T>();
    }

    public T Get() {
        var element = Pool.Count == 0 ? Create() : Pool.Get();
        return OnGet(element);
    }

    public void Fill(int desiredSize) {
        while (Pool.Count < desiredSize) {
            Pool.Add(Create());
        }
    }

    public void Return(T element) {
        Pool.Add(OnReturn(element));
    }

    public void Clear() {
        Pool.Clear();
    }

    public abstract T Create();

    public virtual T OnGet(T element) => element;

    public virtual T OnReturn(T element) => element;
}