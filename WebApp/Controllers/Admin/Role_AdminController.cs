using DataAccess;
using Identity.Membership;
using Identity.Membership.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SwiftKare.Controllers
{
    public class Role_AdminController : Controller
    {
        SwiftKareDBEntities db = new SwiftKareDBEntities();
        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        public ActionResult Create()
        {
            if (Session["LogedUserID"] != null)
            {

                try
                {
                    var role = db.AspNetRoles.ToList();
                    return View(role);

                }
                catch (Exception ex)
                {
                    ViewBag.errorMessage = "Error occurred while loading data.";
                    return View();
                }
            }
            else
            {

                return RedirectToAction("../AdminLogin/AdminLogin");
            }


        }

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> Create(FormCollection collection)
        {
            if (Session["LogedUserID"] != null)
            {
                var rolename = "";
                var desc = "";
                var roleid = "";
                ViewBag.successMessage = "";
                ViewBag.errorMessage = "";
                try
                {
                    var action = Request.Form["action"].ToString();
                    if (action == "create")
                    {
                        rolename = Request.Form["rolename"].ToString();
                        desc = Request.Form["desc"].ToString();

                        
                        ViewBag.successMessage = "Record has been saved successfully";
                        ViewBag.errorMessage = "";

                    }
                    if (action == "edit")
                    {
                        roleid = Request.Form["id"].ToString();
                        rolename = Request.Form["rolename"].ToString();
                        
                       
                        ViewBag.successMessage = "Record has been saved successfully";
                        ViewBag.errorMessage = "";
                        var _existingroleList = db.AspNetRoles.ToList();
                        return View(_existingroleList);
                    }
                    if (action == "delete")
                    {
                        roleid = Request.Form["id"].ToString();
                       
                        ViewBag.successMessage = "Record has been deleted successfully";
                        ViewBag.errorMessage = "";
                    }
                    var __existingroleList = db.SP_SelectRole();
                    return View(__existingroleList);

                }
                catch (Exception ex)
                {
                    ViewBag.errorMessage = "Error occurred while processing your request.";
                    var _existingroleList = db.SP_SelectRole();
                    return View(_existingroleList);
                }
            }
            else
            {
                return RedirectToAction("../AdminLogin/AdminLogin");
            }
        }

    }
}

