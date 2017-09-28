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
                    if (Temp.DisableBanList || Temp.DisableAdminTools)
                    {
                        TgApi.getDefaultApiConnection().sendMessage(
                            RawMessage.chat.id,
                            "非常抱歉，当前的编译已经禁用了封禁用户的功能，请您重新下载源码并编译以启用此功能。",
                            RawMessage.message_id
                            );
                        break;
                    }
                    new BanUserCommand().Ban(RawMessage, JsonMessage, Command);
                    throw new StopProcessException();
                case "/unban":
                    if (Temp.DisableBanList)
                    {
                        TgApi.getDefaultApiConnection().sendMessage(
                            RawMessage.chat.id,
                            "非常抱歉，当前的编译已经禁用了封禁用户的功能，请您重新下载源码并编译以启用此功能。",
                            RawMessage.message_id
                            );
                        break;
                    }
                    new UnbanUserCommand().Unban(RawMessage);
                    throw new StopProcessException();
                case "/getspamstr":
                    if (Temp.DisableBanList)
                    {
                        TgApi.getDefaultApiConnection().sendMessage(
                            RawMessage.chat.id,
                            "非常抱歉，当前的编译已经禁用了封禁用户的功能，请您重新下载源码并编译以启用此功能。",
                            RawMessage.message_id
                            );
                        break;
                    }
                    new SpamStringManager().GetName(RawMessage);
                    return true;
                case "/__getallspamstr":
                    if (Temp.DisableBanList)
                    {
                        TgApi.getDefaultApiConnection().sendMessage(
                            RawMessage.chat.id,
                            "非常抱歉，当前的编译已经禁用了封禁用户的功能，请您重新下载源码并编译以启用此功能。",
                            RawMessage.message_id
                            );
                        break;
                    }
                    new SpamStringManager().GetAllInfo(RawMessage);
                    return true;
                case "/__kick":
                    if (Temp.DisableBanList)
                    {
                        TgApi.getDefaultApiConnection().sendMessage(
                            RawMessage.chat.id,
                            "非常抱歉，当前的编译已经禁用了封禁用户的功能，请您重新下载源码并编译以启用此功能。",
                            RawMessage.message_id
                            );
                        break;
                    }
                    //new SpamStringManager().GetAllInfo(RawMessage);
                    return true;
                case "/addspamstr":
                    if (Temp.DisableBanList)
                    {
                        TgApi.getDefaultApiConnection().sendMessage(
                            RawMessage.chat.id,
                            "非常抱歉，当前的编译已经禁用了封禁用户的功能，请您重新下载源码并编译以启用此功能。",
                            RawMessage.message_id
                            );
                        break;
                    }
                    new SpamStringManager().Add(RawMessage);
                    throw new StopProcessException();
                case "/delspamstr":
                    if (Temp.DisableBanList)
                    {
                        TgApi.getDefaultApiConnection().sendMessage(
                            RawMessage.chat.id,
                            "非常抱歉，当前的编译已经禁用了封禁用户的功能，请您重新下载源码并编译以启用此功能。",
                            RawMessage.message_id
                            );
                        break;
                    }
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
