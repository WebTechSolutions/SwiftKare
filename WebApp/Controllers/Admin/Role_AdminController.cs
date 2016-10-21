using DataAccess;
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

                return RedirectToAction("../Account/AdminLogin");
            }


        }

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            if (Session["LogedUserID"] != null)
            {
                var rolename = "";
                var roleid = "";
                ViewBag.successMessage = "";
                ViewBag.errorMessage = "";
                try
                {
                    var action = Request.Form["action"].ToString();
                    if (action == "create")
                    {
                        rolename = Request.Form["rolename"].ToString();
                        var role = (
                                       from p in db.Roles
                                       where (p.roleName == rolename && p.active == true)
                                       select p
                                   ).FirstOrDefault();
                        if (role != null)
                        {
                            ViewBag.successMessage = "";
                            ViewBag.errorMessage = "Role already exists";

                        }
                        if (role == null)
                        {
                            db.SP_AddRoles(rolename, Session["LogedUserID"].ToString());
                            db.SaveChanges();
                            ViewBag.successMessage = "Record has been saved successfully";
                            ViewBag.errorMessage = "";
                        }
                    }
                    if (action == "edit")
                    {
                        roleid = Request.Form["id"].ToString();
                        rolename = Request.Form["rolename"].ToString();
                        //var role = (
                        //               from p in db.Roles
                        //               where (p.roleName == rolename && p.active == true)
                        //               select p
                        //           ).FirstOrDefault();
                        //if (role != null)
                        //{
                        //    ViewBag.successMessage = "";
                        //    ViewBag.errorMessage = "Role already exists";

                        //}
                        //if (role == null)
                        //{
                        //db.sp_UpdateRole(Convert.ToInt64(roleid), rolename, Session["LogedUserID"].ToString(), System.DateTime.Now);
                        //db.SaveChanges();
                        ViewBag.successMessage = "Record has been saved successfully";
                        ViewBag.errorMessage = "";
                        // }
                    }
                    if (action == "delete")
                    {
                        roleid = Request.Form["id"].ToString();
                        db.sp_DeleteRole(Convert.ToInt64(roleid), Session["LogedUserID"].ToString(), System.DateTime.Now);
                        db.SaveChanges();
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
                return RedirectToAction("../Account/AdminLogin");
            }
        }

    }
}

