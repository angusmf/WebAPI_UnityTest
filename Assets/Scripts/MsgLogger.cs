using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

public class MsgLogger : MonoBehaviour
    {

    class MsgItem
    {
        public string msg;
        public bool error;
        public bool exception;
    }

    List<MsgItem> Msgs = new List<MsgItem>();
    private void Awake()
    {
        MessageBroker.Default.Receive<DebugMsg>().Subscribe(dbgMsg =>
        {
            Msgs.Add(new MsgItem() { msg = dbgMsg.msg });
        });

        MessageBroker.Default.Receive<ErrorMsg>().Subscribe(dbgMsg =>
        {
            Msgs.Add(new MsgItem() { msg = dbgMsg.msg, error = true });
        });

        MessageBroker.Default.Receive<ExceptionMsg>().Subscribe(dbgMsg =>
        {
            Msgs.Add(new MsgItem() { msg = dbgMsg.msg, exception = true });
        });
    }

    private void Start()
    {
        IngameDebugConsole.DebugLogConsole.AddCommandInstance("print", "print all debug messages", "PrintAll", this);
        IngameDebugConsole.DebugLogConsole.AddCommandInstance("clear", "clear all debug messages", "ClearAll", this);
    }

    public void PrintAll()
    {
        foreach (MsgItem msgItem in Msgs)
        {
            if (msgItem.error)
                Debug.LogError(msgItem.msg);
            else if (msgItem.exception)
                Debug.LogException(new Exception(msgItem.msg));
            else
                Debug.Log(msgItem.msg);
        }
    }

    public void ClearAll()
    {
        Msgs.Clear();
    }

    public static void LogMsg(string msg)
    {
        MessageBroker.Default.Publish(new DebugMsg(msg));
    }

    public static void LogMsgError(string msg)
    {
        MessageBroker.Default.Publish(new ErrorMsg(msg));
    }
}

