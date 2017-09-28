using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;
using System.Collections.Generic;

namespace CNBlackListSoamChecker.CommandObject
{
    class UnbanUserCommand
    {
        internal bool Unban(TgMessage RawMessage)
        {
            int banSpace = RawMessage.text.IndexOf(" ");
            if (banSpace == -1)
            {
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    "/unban [i|id=1] [f|from=f|fwd|r|reply]" +
                    " r|reason=\"asdfsadf asdfadsf\"\n\n" +
                    "from 选项仅在 id 未被定义时起作用\n" +
                    "ID 选择优先级: 手动输入的 ID > 回复的被转发消息 > 回复的消息\n" +
                    "选项优先级: 简写 > 全名\n" +
                    "Example:\n" +
                    "/unban id=1 reason=\"aaa bbb\\n\\\"ccc\\\" ddd\"\n" +
                    "/unban",
                    RawMessage.message_id
                    );
                return true;
            }
            int BanUserId = 0;
            string Reason;
            UserInfo BanUserInfo = null;
            try
            {
                Dictionary<string, string> banValues = CommandDecoder.cutKeyIsValue(RawMessage.text.Substring(banSpace + 1));

                // 获取用户信息
                UserInfo tmpUinfo = new GetValues().GetByTgMessage(banValues, RawMessage);
                if (tmpUinfo == null) return true; // 如果没拿到用户信息则代表出现了异常
                else
                {
                    BanUserId = tmpUinfo.id;
                    if (tmpUinfo.language_code != null)
                    {
                        if (tmpUinfo.language_code != "__CAN_NOT_GET_USERINFO__")
                        {
                            BanUserInfo = tmpUinfo;
                        }
                    }
                    else
                    {
                        BanUserInfo = tmpUinfo;
                    }
                }

                // 获取 Reason
                Reason = new GetValues().GetReason(banValues, RawMessage);
                if (Reason == null) return true; // 如果 Reason 是 null 则代表出现了异常
            }
            catch (DecodeException)
            {
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    "您的输入有误，请检查您的输入，请使用 /ban 来查看帮助 err10",
                    RawMessage.message_id
                    );
                return true;
            }
            bool status;
            try
            {
                if (BanUserInfo == null)
                {
                    status = Temp.GetDatabaseManager().UnbanUser(
                        RawMessage.GetSendUser().id,
                        BanUserId,
                        Reason
                        );
                }
                else
                {
                    status = Temp.GetDatabaseManager().UnbanUser(
                        RawMessage.GetSendUser().id,
                        BanUserId,
                        Reason,
                        BanUserInfo
                        );
                }
            }
            catch (System.InvalidOperationException)
            {
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    "操作失败，这位用户目前可能没有被 Ban。",
                    RawMessage.message_id
                    );
                return true;
            }
            if (status)
            {
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    "操作成功。",
                    RawMessage.message_id
                    );
                return true;
            }
            else
            {
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    "操作成功。\n\n请注意：转发用户消息到频道或发送用户信息到频道失败，请您手动发送至 @" + Temp.MainChannelName + " 。 err11",
                    RawMessage.message_id
                    );
                return true;
            }
            //return false;
        }

        private UserInfo GetUserInfo(TgMessage RawMessage, string from)
        {
            if (RawMessage.reply_to_message == null)
            {
                return null;
            }
            if (from == "r" || from == "reply")
            {
                return RawMessage.GetReplyMessage().GetSendUser();
            }
            else if (from == "f" || from == "fwd")
            {
                return RawMessage.GetForwardedFromUser();
            }
            return null;
        }
    }
}
