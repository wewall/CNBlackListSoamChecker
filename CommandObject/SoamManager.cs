﻿using CNBlackListSoamChecker.DbManager;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace CNBlackListSoamChecker.CommandObject
{
    class SoamManager
    {
        public void SoamEnable(TgMessage message)
        {
            if (!TgApi.getDefaultApiConnection().checkIsAdmin(message.chat.id, message.from.id))
            {
                TgApi.getDefaultApiConnection().sendMessage(message.chat.id, "您不是这个群组的管理员，无法执行此操作。", message.message_id);
                return;
            }
            string enabled = "";
            int AdminOnly = 3;
            int Blacklist = 3;
            int AutoKick = 3;
            int AutoDeleteSpamMessage = 3;
            int AutoDeleteCommand = 3;
            int SubscribeBanList = 3;
            string text = message.text.ToLower();
            if (text.IndexOf(" adminonly") != -1)
            {
                AdminOnly = 0;
                enabled += " AdminOnly";
            }
            if (text.IndexOf(" blacklist") != -1)
            {
                Blacklist = 0;
                enabled += " Blacklist";
            }
            if (text.IndexOf(" autokick") != -1)
            {
                AutoKick = 0;
                enabled += " AutoKick";
            }
            if (text.IndexOf(" autodeletespammessage") != -1)
            {
                AutoDeleteSpamMessage = 0;
                enabled += " AutoDeleteSpamMessage";
            }
            if (text.IndexOf(" autodeletecommand") != -1)
            {
                AutoDeleteCommand = 0;
                enabled += " AutoDeleteCommand";
            }
            if (text.IndexOf(" subscribebanlist") != -1)
            {
                SubscribeBanList = 0;
                enabled += " SubscribeBanList";
            }
            Temp.GetDatabaseManager().SetGroupConfig(
                message.chat.id,
                AdminOnly: AdminOnly,
                BlackList: Blacklist,
                AutoKick: AutoKick,
                AutoDeleteSpamMessage: AutoDeleteSpamMessage,
                AutoDeleteCommand: AutoDeleteCommand,
                SubscribeBanList: SubscribeBanList
                );
            if (enabled == "")
            {
                enabled = "[[null]]\n\n请您使用 /soamenable [所需的功能] 来启用您需要的功能。\n" +
                    "例如: \"/soamenable BlackList\" (不包含引号) 则可以启用由 @CNBlackListR 提供的黑名单列表警告。\n" +
                    "您也可以使用多个选项，例如: \"/soamenable BlackList AutoKick\" (不包含引号) " +
                    "则可以启用由 @CNBlackListR 提供的黑名单列表警告，在警告后还会将成员移出群组。\n\n" +
                    "您可以使用 /soamstatus 查看当前群组开启或关闭了的功能。";
            }
            TgApi.getDefaultApiConnection().sendMessage(message.chat.id, "成功，开启了的功能有: " + enabled, message.message_id);
            return;
        }

        public void SoamDisable(TgMessage message)
        {
            if (!TgApi.getDefaultApiConnection().checkIsAdmin(message.chat.id, message.from.id))
            {
                TgApi.getDefaultApiConnection().sendMessage(message.chat.id, "您不是这个群组的管理员，无法执行此操作。", message.message_id);
                return;
            }
            string enabled = "";
            int AdminOnly = 3;
            int Blacklist = 3;
            int AutoKick = 3;
            int AutoDeleteSpamMessage = 3;
            int AutoDeleteCommand = 3;
            int SubscribeBanList = 3;
            string text = message.text.ToLower();
            if (text.IndexOf(" adminonly") != -1)
            {
                AdminOnly = 1;
                enabled += " AdminOnly";
            }
            if (text.IndexOf(" blacklist") != -1)
            {
                Blacklist = 1;
                enabled += " Blacklist";
            }
            if (text.IndexOf(" autokick") != -1)
            {
                AutoKick = 1;
                enabled += " AutoKick";
            }
            if (text.IndexOf(" autodeletespammessage") != -1)
            {
                AutoDeleteSpamMessage = 1;
                enabled += " AutoDeleteSpamMessage";
            }
            if (text.IndexOf(" autodeletecommand") != -1)
            {
                AutoDeleteCommand = 1;
                enabled += " AutoDeleteCommand";
            }
            if (text.IndexOf(" subcribebanlist") != -1)
            {
                SubscribeBanList = 1;
                enabled += " SubscribeBanList";
            }
            Temp.GetDatabaseManager().SetGroupConfig(
                message.chat.id,
                AdminOnly: AdminOnly,
                BlackList: Blacklist,
                AutoKick: AutoKick,
                AutoDeleteSpamMessage: AutoDeleteSpamMessage,
                AutoDeleteCommand: AutoDeleteCommand,
                SubscribeBanList: SubscribeBanList
                );
            if (enabled == "")
            {
                enabled = "[[null]]\n\n请您使用 /soamdisable [所需的功能] 来禁用您需要的功能。\n" +
                    "例如: \"/soamdisable BlackList\" (不包含引号) 则可以禁用由 @CNBlackListR 提供的黑名单列表警告。\n" +
                    "您也可以使用多个选项，例如: \"/soamdisable BlackList AutoKick\" (不包含引号) " +
                    "则可以禁用由 @CNBlackListR 提供的黑名单列表警告，并禁用在警告后将成员移出群组的功能。" +
                    "您可以使用 /soamstatus 查看当前群组开启或关闭了的功能。";
            }
            TgApi.getDefaultApiConnection().sendMessage(message.chat.id, "成功，关闭了的功能有: " + enabled, message.message_id);
            return;
        }

        public void SoamStatus(TgMessage message)
        {
            GroupCfg gc = Temp.GetDatabaseManager().GetGroupConfig(message.chat.id);
            TgApi.getDefaultApiConnection().sendMessage(
                message.chat.id,
                "BlackList (by @CNBlackListR ): " + (gc.BlackList == 0) + "\n" +
                "AutoKick: " + (gc.AutoKick == 0) + "\n" +
                "AutoDeleteSpamMessage: " + (gc.AutoDeleteSpamMessage == 0) + "\n" +
                "AutoDeleteCommand: " + (gc.AutoDeleteCommand == 0) + "\n" +
                "AdminOnly: " + (gc.AdminOnly == 0) + "\n" +
                "SubscribeBanList: " + (gc.SubscribeBanList == 0),
                message.message_id
                );
            return;
        }
    }
}
