using System;
using System.Web.UI;
using System.Data;
using Npgsql;

namespace KomponenetService.Admin
{
    public partial class Dashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["userId"] == null || Session["roleId"].ToString() != "1")
                {
                    Response.Redirect("~/User/Login.aspx");
                }
                GetAdmins();
            }
        }

        private void GetAdmins()
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(Utils.getConnection()))
                {
                    conn.Open();
                    string query = @"SELECT ROW_NUMBER() OVER(ORDER BY userid) AS SrNo, 
                                   userid, name, email, mobile, address 
                                   FROM users 
                                   WHERE roleid = 1";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            rAdmins.DataSource = dt;
                            rAdmins.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error loading admin list: " + ex.Message);
            }
        }

        private void GetAdminDetails(int adminId)
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(Utils.getConnection()))
                {
                    conn.Open();
                    string query = @"SELECT name, email, mobile, address, password 
                                   FROM users 
                                   WHERE userid = @userId::integer AND roleid = 1";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("userId", adminId);
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtName.Text = reader["name"].ToString();
                                txtEmail.Text = reader["email"].ToString();
                                txtMobile.Text = reader["mobile"].ToString();
                                txtAddress.Text = reader["address"].ToString();
                                txtPassword.Text = reader["password"].ToString();
                                ViewState["AdminId"] = adminId;
                                adminProfile.Visible = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error loading admin details: " + ex.Message);
            }
        }

        protected void rAdmins_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "edit")
            {
                int adminId = Convert.ToInt32(e.CommandArgument);
                GetAdminDetails(adminId);
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(Utils.getConnection()))
                {
                    conn.Open();
                    string query = @"UPDATE users 
                                   SET name = @name, email = @email, mobile = @mobile, 
                                       address = @address, password = @password 
                                   WHERE userid = @userId::integer AND roleid = 1";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("name", txtName.Text.Trim());
                        cmd.Parameters.AddWithValue("email", txtEmail.Text.Trim());
                        cmd.Parameters.AddWithValue("mobile", txtMobile.Text.Trim());
                        cmd.Parameters.AddWithValue("address", txtAddress.Text.Trim());
                        cmd.Parameters.AddWithValue("password", txtPassword.Text.Trim());
                        cmd.Parameters.AddWithValue("userId", ViewState["AdminId"]);

                        int result = cmd.ExecuteNonQuery();
                        if (result > 0)
                        {
                            ShowSuccessMessage("Profile updated successfully!");
                            GetAdmins();
                            adminProfile.Visible = false;
                        }
                        else
                        {
                            ShowErrorMessage("Failed to update profile.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error updating profile: " + ex.Message);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            adminProfile.Visible = false;
            ClearForm();
        }

        private void ClearForm()
        {
            txtName.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtMobile.Text = string.Empty;
            txtAddress.Text = string.Empty;
            txtPassword.Text = string.Empty;
            ViewState["AdminId"] = null;
        }

        private void ShowErrorMessage(string message)
        {
            lblMsg.Visible = true;
            lblMsg.Text = "<div class='alert alert-danger'>" + message + "</div>";
        }

        private void ShowSuccessMessage(string message)
        {
            lblMsg.Visible = true;
            lblMsg.Text = "<div class='alert alert-success'>" + message + "</div>";
        }
    }
}