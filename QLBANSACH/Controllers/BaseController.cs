using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using QLBANSACH.Models;

namespace QLBANSACH.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Lấy Session user từ UserLogin
            
            
            // Kiểm tra đã có tài khoản Admin đăng nhập hay chưa?
            if (Session["Taikhoanadein"] == null )
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "LoginAdmin", action = "Login" }));
            }
            else
            {
                Admin userAdmin = (Admin)Session["Taikhoanadein"];
                string userAdminValue = userAdmin.UserAdmin;
                if (userAdminValue != "admin")
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "LoginAdmin", action = "Login" }));
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}