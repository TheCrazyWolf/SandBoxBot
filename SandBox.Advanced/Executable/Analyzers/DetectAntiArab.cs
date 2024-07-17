using SandBox.Advanced.Abstract;
using SandBox.Advanced.Executable.Common;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Analyzers;

public class DetectAntiArab : SandBoxHelpers, IExecutable<bool>
{
    private List<char> _arabicCharacters = new ();
    private static bool _isFirstRun = true;
    private bool _isCanOvveride;
    public Task<bool> Execute()
    {
        if (_isFirstRun)
        {
            PrepareCharacters();
            _isFirstRun = false;
        }
        
        _isCanOvveride = CanBeOverrideRestriction(Update.Message!.From!.Id, Update.Message.Chat.Id).Result;

        if (_isCanOvveride)
            return Task.FromResult(false);
        
        if (Update.Message?.NewChatMembers is not null)
        {
            
            foreach (var item in Update.Message.NewChatMembers)
            {
                if (item.FirstName.Any(c => _arabicCharacters.Contains(c)))
                    Proccess(item.Id);
                else if (item.LastName != null && item.LastName.Any(c => _arabicCharacters.Contains(c)))
                    Proccess(item.Id);
                else if (item.Username != null && item.Username.Any(c => _arabicCharacters.Contains(c)))
                    Proccess(item.Id);
            }
        }
        

        if(Update.Message?.From != null && Update.Message.From.FirstName.ToArray().Any(c => _arabicCharacters.Contains(c)))
            Proccess(Update.Message.From.Id);
        else if(Update.Message?.From?.LastName != null && Update.Message.From != null && Update.Message.From.LastName.ToArray().Any(c => _arabicCharacters.Contains(c)))
            Proccess(Update.Message.From.Id);
        else if(Update.Message?.From?.Username != null && Update.Message.From != null && Update.Message.From.Username.ToArray().Any(c => _arabicCharacters.Contains(c)))
            Proccess(Update.Message.From.Id);

        if(Update.Message?.Text != null && Update.Message.Text.ToArray().Any(c => _arabicCharacters.Contains(c)))
            Proccess(Update.Message.From!.Id);
        
        return Task.FromResult(false);
    }

    private Task PrepareCharacters()
    {
        for (char c = '\u0600'; c <= '\u06FF'; c++)
        {
            _arabicCharacters.Add(c);
        }

        // Дополнительные арабские символы: U+0750 - U+077F
        for (char c = '\u0750'; c <= '\u077F'; c++)
        {
            _arabicCharacters.Add(c);
        }

        // Арабские символы презентационных форм A: U+FB50 - U+FDFF
        for (char c = '\uFB50'; c <= '\uFDFF'; c++)
        {
            _arabicCharacters.Add(c);
        }

        // Арабские символы презентационных форм B: U+FE70 - U+FEFF
        for (char c = '\uFE70'; c <= '\uFEFF'; c++)
        {
            _arabicCharacters.Add(c);
        }

        return Task.CompletedTask;
    }

    private Task Proccess(long idUser)
    {
        BotClient.BanChatMemberAsync(chatId: Update.Message!.Chat.Id,
            userId: idUser);
        NotifyManagers();
        return Task.CompletedTask;
    }

    private Task NotifyManagers()
    {
        foreach (var id in Repository.Accounts.GetManagers().Result)
        {
            var message = BuildNotifyMessage();

            BotClient.SendTextMessageAsync(chatId: id.IdTelegram,
                text: message,
                disableNotification: true);
        }

        return Task.CompletedTask;
    }

    private string BuildNotifyMessage()
    {
        return
            $"\ud83d\udc7e Я забанил пользователя (@{Update.Message?.From?.Username}) # {Update.Message!.From?.Id} " +
            $"так как в у него в никнейме или его сообщении содержались арабские символы (возможено, что он бот)";
    }
}