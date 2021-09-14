using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using me.cqp.luohuaming.ZhouYi.Code.ZhouYi;
using me.cqp.luohuaming.ZhouYi.Sdk.Cqp.EventArgs;
using ZhouYi.PublicInfos;

namespace me.cqp.luohuaming.ZhouYi.Code.OrderFunctions
{
    public class GetBaGua : IOrderModel
    {
        public bool ImplementFlag { get; set; } = true;

        public string GetOrderStr() => "#周易";

        public bool Judge(string destStr) => destStr.Trim().Equals(GetOrderStr());

        static List<callInfo> callList = new List<callInfo>();
        class callInfo
        {
            public long Group { get; set; }
            public List<long> callList { get; set; } = new List<long>();
        }
        public FunctionResult Progress(CQGroupMessageEventArgs e)//群聊处理
        {
            FunctionResult result = new FunctionResult
            {
                Result = true,
                SendFlag = true,
            };
            SendText sendText = new SendText
            {
                SendID = e.FromGroup,
            };
            if (callList.Any(x => x.Group == e.FromGroup && x.callList.Any(o => o == e.FromQQ)))
            {
                sendText.MsgToSend.Add("请六小时后再来");
            }
            else
            {
                if(callList.Any(x=>x.Group==e.FromGroup) == false)
                {
                    var c = new callInfo { Group = e.FromGroup };
                    c.callList.Add(e.FromQQ);
                    callList.Add(c);
                }
                else
                {
                    callList.First(x => x.Group == e.FromGroup).callList.Add(e.FromQQ);
                }
                Thread thread = new Thread(() =>
                {
                    Thread.Sleep(6 * 60 * 60 * 1000);
                    callList.First(x=>x.Group == e.FromGroup).callList.Remove(e.FromQQ);
                });
                thread.Start();
                sendText.MsgToSend.Add($"下次可调用时间：{DateTime.Now.AddHours(6):T}");
                sendText.MsgToSend.Add(CalcBaGua.GetGuaText());
            }
            result.SendObject.Add(sendText);
            return result;
        }

        public FunctionResult Progress(CQPrivateMessageEventArgs e)//私聊处理
        {
            FunctionResult result = new FunctionResult
            {
                Result = true,
                SendFlag = true,
            };
            SendText sendText = new SendText
            {
                SendID = e.FromQQ,
            };
            sendText.MsgToSend.Add(CalcBaGua.GetGuaText());
            result.SendObject.Add(sendText);
            return result;
        }
    }
}
