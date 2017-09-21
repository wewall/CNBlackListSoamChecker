using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;
using System.Collections.Generic;

namespace CNBlackListSoamChecker.CommandObject
{
    internal class BanUserCommand
    {
        internal bool Ban(TgMessage RawMessage, string JsonMessage, string Command)
        {
            int banSpace = RawMessage.text.IndexOf(" ");
            if (banSpace == -1)
            {
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    "/ban [i|id=1] [l|level=0] [m|minutes=0] [h|hours=0] [d|days=15] [f|from=f|fwd|r|reply] [halal [f|fwd|r|reply]]" +
                    " r|reason=\"asdfsadf asdfadsf\"\n\n" +
                    "m: 分钟, h: 小时, d: 天\n" +
                    "from 选项仅在 id 未被定义时起作用\n" +
                    "ID 选择优先级: 手动输入的 ID > 回复的被转发消息 > 回复的消息\n" +
                    "选项优先级: 简写 > 全名\n" +
                    "halal 选项只能单独使用，不能与其他选项共同使用，并且需要回复一条消息，否则将会触发异常。\n\n" +
                    "Example:\n" +
                    "/ban id=1 m=0 h=0 d=15 level=0 reason=\"aaa bbb\\n\\\"ccc\\\" ddd\"\n" +
                    "/ban halal\n" +
                    "/ban halal=reply",
                    RawMessage.message_id
                    );
                return true;
            }
            int BanUserId = 0;
            long ExpiresTime = 0;
            int Level = 0;
            string Reason = "";
            UserInfo BanUserInfo = null;
            string value = RawMessage.text.Substring(banSpace + 1);
            int valLen = value.Length;
            bool NotHalal = true;
            if (valLen >= 5)
            {
                if (value.Substring(0, 5) == "halal")
                {
                    NotHalal = false;
                    Reason = "Halal （中国人无法识别的语言）";
                    if (valLen > 6)
                    {
                        if (value[5] != ' ')
                        {
                            TgApi.getDefaultApiConnection().sendMessage(
                                RawMessage.GetMessageChatInfo().id,
                                "您的输入有误，请检查您的输入，或使用 /ban 查看帮助。 err_a1",
                                RawMessage.message_id
                                );
                            return true;
                        }
                        UserInfo tmpUinfo = new GetValues().GetByTgMessage(new Dictionary<string, string> { { "from" , value.Substring(6) } }, RawMessage);
                        if (tmpUinfo == null) return true; // 如果没拿到用户信息则代表出现了异常
                        else
                        {
                            BanUserId = tmpUinfo.id;
                            if (tmpUinfo.language_code != null && tmpUinfo.language_code != "__CAN_NOT_GET_USERINFO__")
                            {
                                BanUserInfo = tmpUinfo;
                            }
                        }
                    }
                    else
                    {
                        UserInfo tmpUinfo = new GetValues().GetByTgMessage(new Dictionary<string, string> {  }, RawMessage);
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
                    }
                }
            }
            if (NotHalal)
            {
                try
                {
                    Dictionary<string, string> banValues = CommandDecoder.cutKeyIsValue(value);
                    string tmpString = "";

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

                    // 获取 ExpiresTime
                    long tmpExpiresTime = new GetValues().GetBanUnixTime(banValues, RawMessage);
                    if (tmpExpiresTime == -1) return true; // 如果过期时间是 -1 则代表出现了异常
                    else ExpiresTime = tmpExpiresTime;

                    // 获取 Level
                    tmpString = banValues.GetValueOrDefault("l", "__invalid__");
                    if (tmpString == "__invalid__")
                    {
                        tmpString = banValues.GetValueOrDefault("level", "0");
                    }
                    if (!int.TryParse(tmpString, out Level))
                    {
                        TgApi.getDefaultApiConnection().sendMessage(
                            RawMessage.GetMessageChatInfo().id,
                            "您的输入有误，请检查您的输入，或使用 /ban 查看帮助。 err8",
                            RawMessage.message_id
                            );
                        return true;
                    }

                    // 获取 Reason
                    Reason = new GetValues().GetReason(banValues, RawMessage);
                    if (Reason == null) return true; // 如果 Reason 是 null 则代表出现了异常
                    if (Reason.ToLower() == "halal")
                    {
                        Reason = "Halal （中国人无法识别的语言）";
                    }
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
            }
            bool status;
            if (BanUserInfo == null)
            {
                status = Temp.GetDatabaseManager().BanUser(
                    RawMessage.GetSendUser().id,
                    BanUserId,
                    Level,
                    ExpiresTime,
                    Reason
                    );
            }
            else
            {
                status = Temp.GetDatabaseManager().BanUser(
                    RawMessage.GetSendUser().id,
                    BanUserId,
                    Level,
                    ExpiresTime,
                    Reason,
                    RawMessage.GetMessageChatInfo().id,
                    RawMessage.GetReplyMessage().message_id,
                    BanUserInfo
                    );
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
    }
}
