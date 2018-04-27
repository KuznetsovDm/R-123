namespace RadioTask.Model
{
    public interface IObserver<T>
    {
        void Handle(T obj);
    }
}
