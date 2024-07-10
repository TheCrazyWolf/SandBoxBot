﻿using Telegram.Bot.Types;

namespace SandBoxBot.Commands.Base;

public interface ICommand
{
    Task Execute(Message message, CancellationToken cancellationToken);
}