using System.Web;
using System.Web.Mvc;
using DemoChat.Models;

namespace DemoChat.Controllers
{
public class HomeController : Controller
{
    public ActionResult Index()
    {
        var viewHome = new Home()
        {
            Mensagens = ChatServer.GetHistorico()
        };

        return View(viewHome);
    }
}




}