namespace SandBox.Advanced.Interfaces;

public interface IExecutable<T>
{
    Task<T> Execute();
}