using CNBlackListSoamChecker.CommandObject;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace CNBlackListSoamChecker
{
    class AdminCommand
    {
        internal bool AdminCommands(TgMessage RawMessage, string JsonMessage, string Command)
        {
            if (!RAPI.getIsBotAdmin(RawMessage.GetSendUser().id))
            {
                return false;
            }
            switch (Command)
            {
                case "/ban":
                    new BanUserCommand().Ban(RawMessage, JsonMessage, Command);
                    throw new StopProcessException();
                case "/unban":
                    new UnbanUserCommand().Unban(RawMessage);
                    throw new StopProcessException();
                case "/getspamstr":
                    new SpamStringManager().GetName(RawMessage);
                    return true;
                case "/__getallspamstr":
                    new SpamStringManager().GetAllInfo(RawMessage);
                    return true;
                case "/__kick":
                    //new SpamStringManager().GetAllInfo(RawMessage);
                    return true;
                case "/addspamstr":
                    new SpamStringManager().Add(RawMessage);
                    throw new StopProcessException();
                case "/delspamstr":
                    new SpamStringManager().Remove(RawMessage);
                    throw new StopProcessException();
                case "/getspampoints":
                    new SpamStringManager().GetSpamPoints(RawMessage);
                    throw new StopProcessException();
                case "/jsonencode":
                    int spacePath = RawMessage.text.IndexOf(" ");
                    if (spacePath == -1)
                    {
                        TgApi.getDefaultApiConnection().sendMessage(
                            RawMessage.GetMessageChatInfo().id,
                            "您的输入有误",
                            RawMessage.message_id
                            );
                        throw new StopProcessException();
                    }
                    string jsonText = RawMessage.text.Substring(spacePath + 1);
                    TgApi.getDefaultApiConnection().sendMessage(
                        RawMessage.GetMessageChatInfo().id,
                        "<code>" + TgApi.getDefaultApiConnection().jsonEncode(jsonText) + "</code>",
                        RawMessage.message_id,
                        ParseMode: TgApi.PARSEMODE_HTML
                        );
                    throw new StopProcessException();
            }
            return false;
        }
    }
}
