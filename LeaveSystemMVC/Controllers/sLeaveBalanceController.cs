﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeaveSystemMVC.Controllers
{
    public class sLeaveBalanceController : Controller
    {
        // GET: sLeaveBalance
        public ActionResult Index()
        {

            var model = new List<Models.sleaveBalanceModel>();
            var claimsIdentity = User.Identity as System.Security.Claims.ClaimsIdentity;
            var c = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);   
            ViewBag.claim = c;
            string a = c.ToString();
            a = a.Substring(a.Length - 5);
            //System.Diagnostics.Debug.WriteLine("id is:"+a + ".");
            var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;          

            string queryString = "Select Balance,Leave_Name FROM dbo.Leave_Balance, dbo.Leave_Type where Leave_Balance.Employee_ID = '" + a+"' AND Leave_Balance.Leave_ID = Leave_Type.Leave_ID";

           using( var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(queryString, connection);

                connection.Open();
                int annual = 0;
                int sick = 0;
                int compassionate = 0;
                int maternity = 0;
                int shortHrs = 0;
                int unpaid = 0;
                int DIL = 0;



                using (var reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        var balance = new Models.sleaveBalanceModel();

                        //string leave = "";

                        string leave = (string)reader["Leave_Name"];
                        if (leave.Equals("Annual"))
                            annual =  (int)reader["Balance"];
                        ViewBag.annual = annual;    

                        if (leave.Equals("Sick"))
                            sick = (int)reader["Balance"];
                        ViewBag.sick = sick;

                        if (leave.Equals("Compassionate"))
                            compassionate = (int)reader["Balance"];
                        ViewBag.compassionate = compassionate;

                        if (leave.Equals("Maternity"))
                           maternity = (int)reader["Balance"];
                        ViewBag.maternity = maternity;

                        if (leave.Equals("Short_Hours"))
                           shortHrs = (int)reader["Balance"];
                        ViewBag.shortHrs = shortHrs;

                        if (leave.Equals("Unpaid"))
                            unpaid = (int)reader["Balance"];
                        ViewBag.unpaid = unpaid;

                        if (leave.Equals("DIL"))
                            DIL = (int)reader["Balance"];
                        ViewBag.DIL = DIL;

                        model.Add(balance);
                    }
                }
                connection.Close();
            }

            string queryString1 = "SELECT DATEDIFF(day,Emp_Start_Date,GETDATE()) AS DiffDate from dbo.Employee where Employee_ID = '" + a + "';";
            System.Diagnostics.Debug.WriteLine("entered diff query");
            using (var connection1 = new SqlConnection(connectionString))
            {
                var command1 = new SqlCommand(queryString1, connection1);
                connection1.Open();
                using (var reader1 = command1.ExecuteReader())
                {
                    while (reader1.Read())
                    {
                        System.Diagnostics.Debug.WriteLine("inside reader");
                        int dif = (int)reader1["DiffDate"];
                        System.Diagnostics.Debug.WriteLine("dif is:" +dif);
                        if (dif < 183) {
                            ViewBag.annual = 0;
                        }
                    }

                }
                connection1.Close();
            }



            return View(model);
        }
    }
}