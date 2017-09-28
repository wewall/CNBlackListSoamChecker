using ReimuAPI.ReimuBase.Interfaces;
using System;
using ReimuAPI.ReimuBase.TgData;
using ReimuAPI.ReimuBase;
using CNBlackListSoamChecker.DbManager;
using System.Collections.Generic;
using System.Diagnostics;
using CNBlackListSoamChecker.CommandObject;

namespace CNBlackListSoamChecker
{
    class CommandListener : ICommandReceiver
    {
        public CommandListener()
        {
            new DatabaseManager().checkdb();
        }

        public CallbackMessage OnGroupCommandReceive(TgMessage RawMessage, string JsonMessage, string Command)
        {
            return OnSupergroupCommandReceive(RawMessage, JsonMessage, Command);
        }

        public CallbackMessage OnPrivateCommandReceive(TgMessage RawMessage, string JsonMessage, string Command)
        {
            try
            {
                if (SharedCommand(RawMessage, JsonMessage, Command)) return new CallbackMessage();
                return new CallbackMessage();
            }
            catch (StopProcessException) { return new CallbackMessage() { StopProcess = true }; }
            catch (Exception e)
            {
                RAPI.GetExceptionListener().OnException(e, JsonMessage);
                throw e;
            }
        }

        public CallbackMessage OnSupergroupCommandReceive(TgMessage RawMessage, string JsonMessage, string Command)
        {
            try
            {
                GroupCfg cfg = Temp.GetDatabaseManager().GetGroupConfig(RawMessage.chat.id);
                if (cfg.AdminOnly == 0 && TgApi.getDefaultApiConnection().checkIsAdmin(RawMessage.chat.id, RawMessage.from.id) == false)
                {
                    return new CallbackMessage() {  };
                }
                if (SharedCommand(RawMessage, JsonMessage, Command)) return new CallbackMessage();
                switch (Command)
                {
                    case "/soamenable":
                        if (cfg.AdminOnly == 0 && TgApi.getDefaultApiConnection().checkIsAdmin(RawMessage.chat.id, RawMessage.from.id) == false)
                            return new CallbackMessage() { StopProcess = true };
                        new SoamManager().SoamEnable(RawMessage);
                        break;
                    case "/soamdisable":
                        if (cfg.AdminOnly == 0 && TgApi.getDefaultApiConnection().checkIsAdmin(RawMessage.chat.id, RawMessage.from.id) == false)
                            return new CallbackMessage() { StopProcess = true };
                        new SoamManager().SoamDisable(RawMessage);
                        break;
                    case "/__get_exception":
                        throw new Exception();
                    case "/soamstat":
                    case "/soamstatus":
                        if (cfg.AdminOnly == 0 && TgApi.getDefaultApiConnection().checkIsAdmin(RawMessage.chat.id, RawMessage.from.id) == false)
                            return new CallbackMessage() { StopProcess = true };
                        new SoamManager().SoamStatus(RawMessage);
                        break;
                    case "/bkick":
                        if (Temp.DisableBanList)
                        {
                            TgApi.getDefaultApiConnection().sendMessage(
                                RawMessage.chat.id,
                                "非常抱歉，当前的编译已经禁用了封禁用户的功能，请您重新下载源码并编译以启用此功能。",
                                RawMessage.message_id
                                );
                            break;
                        }
                        if (RawMessage.reply_to_message == null)
                        {
                            TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "请回复一条消息", RawMessage.message_id);
                            return new CallbackMessage();
                        }
                        BanUser ban = Temp.GetDatabaseManager().GetUserBanStatus(RawMessage.reply_to_message.from.id);
                        if (ban.Ban == 0)
                        {
                            if (ban.Level == 0)
                            {
                                SetActionResult bkick_result = TgApi.getDefaultApiConnection().kickChatMember(
                                    RawMessage.chat.id,
                                    RawMessage.reply_to_message.from.id,
                                    GetTime.GetUnixTime() + 86400
                                    );
                                if (bkick_result.ok)
                                {
                                    TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "已移除", RawMessage.message_id);
                                    return new CallbackMessage();
                                }
                                else
                                {
                                    TgApi.getDefaultApiConnection().sendMessage(
                                        RawMessage.chat.id,
                                        "无法移除，可能是机器人没有适当的管理员权限。",
                                        RawMessage.message_id
                                        );
                                    return new CallbackMessage();
                                }
                            }
                            else
                            {
                                TgApi.getDefaultApiConnection().sendMessage(
                                    RawMessage.chat.id,
                                    "无法移除，因为此用户的封禁级别没有达到要求，请您联系群组的管理员来处理。" +
                                    "如果您认为这位用户将会影响大量群组，您亦可联系 @" + Temp.MainChannelName + " 提供的群组。",
                                    RawMessage.message_id
                                    );
                                return new CallbackMessage();
                            }
                        }
                        else
                        {
                            TgApi.getDefaultApiConnection().sendMessage(
                                RawMessage.chat.id,
                                "无法移除，因为此用户没有被机器人列入全局封禁列表中，请您联系群组的管理员来处理。" +
                                "如果您认为这位用户将会影响大量群组，您亦可联系 @" + Temp.MainChannelName + " 提供的群组。",
                                RawMessage.message_id
                                );
                            return new CallbackMessage();
                        }
                }
                return new CallbackMessage();
            }
            catch (StopProcessException) { return new CallbackMessage() { StopProcess = true }; }
            catch (Exception e)
            {
                RAPI.GetExceptionListener().OnException(e, JsonMessage);
                throw e;
            }
        }

        private bool SharedCommand(TgMessage RawMessage, string JsonMessage, string Command)
        {
            switch (Command)
            {
                case "/banstat":
                case "/banstatus":
                    if (Temp.DisableBanList)
                    {
                        TgApi.getDefaultApiConnection().sendMessage(
                            RawMessage.chat.id,
                            "非常抱歉，当前的编译已经禁用了封禁用户的功能，请您重新下载源码并编译以启用此功能。",
                            RawMessage.message_id
                            );
                        break;
                    }
                    return new BanStatus().banstatus(RawMessage);
                case "/clickmetobesb":
                    TgApi.getDefaultApiConnection().sendMessage(
                        RawMessage.chat.id,
                        "Success, now you are SB.",
                        RawMessage.message_id
                        );
                    break;
            }
            return new AdminCommand().AdminCommands(RawMessage, JsonMessage, Command);
        }
    }
}
