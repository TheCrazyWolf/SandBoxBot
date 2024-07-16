namespace SandBox.Advanced.Abstract;

public interface IExecutable<T>
{
    Task<T> Execute();
}