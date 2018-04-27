namespace RadioTask.Model
{
    public interface IObservable<T> 
    {
        void AddObserver(IObserver<T> observer);
        void RemoveObserver();
    }
}
