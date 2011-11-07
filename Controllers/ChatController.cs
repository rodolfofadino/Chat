using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoChat.Controllers
{
public class ChatController : AsyncController
{
    [AsyncTimeout(ChatServer.MaxTimetoutSegundos * 1000)]
    public void IndexAsync()
    {
        AsyncManager.OutstandingOperations.Increment();
        ChatServer.CheckForMensagensAsync(msgs =>
        {
            AsyncManager.Parameters["response"] = new ChatResposta
            {
                mensagens = msgs
            };
            AsyncManager.OutstandingOperations.Decrement();
        });
    }

    public ActionResult IndexCompleted(ChatResposta response)
    {
        return Json(response);
    }

    [HttpPost]
    public ActionResult New(string nome, string msg)
    {
        ChatServer.AddMensagem(nome, msg);
        return Json(new
        {
            d = 1
        });
    }
}
}
