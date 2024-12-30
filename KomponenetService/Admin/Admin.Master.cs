using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KomponenetService.Admin
{
    public partial class Admin : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["userId"] == null || Session["roleId"].ToString() != "1")
                {
                    Response.Redirect("~/User/Login.aspx");
                }
            }
        }

        protected void lbLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("~/User/Login.aspx");
        }
    }
}