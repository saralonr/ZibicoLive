using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ZibicoLive.Socket.Controllers
{
    [EnableCors("*","*","*")]
    public class HomeController : ApiController
    {
    }
}
