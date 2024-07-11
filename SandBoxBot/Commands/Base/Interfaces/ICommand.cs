namespace SandBoxBot.Commands.Base.Interfaces;

public interface ICommand
{
    Task Execute(CancellationToken cancellationToken);
}