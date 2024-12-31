using Npgsql;
using System;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KomponenetService.Admin
{
    public partial class Users : System.Web.UI.Page
    {
        private NpgsqlConnection con;
        private NpgsqlCommand cmd;
        private NpgsqlDataAdapter sda;
        private DataTable dt;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["userId"] == null || Session["roleId"].ToString() != "1")
                {
                    Response.Redirect("../User/Login.aspx");
                }
                GetUsers();
            }
        }

        private void GetUsers(string searchQuery = "")
        {
            try
            {
                using (con = new NpgsqlConnection(Utils.getConnection()))
                {
                    con.Open();
                    if (string.IsNullOrEmpty(searchQuery))
                    {
                        string query = @"SELECT * FROM User_Crud(@c_Action::text, @c_UserId::integer, @c_Name::varchar, 
                            @c_Mobile::varchar, @c_Email::varchar, @c_Address::text, @c_Password::varchar, 
                            @c_ImageUrl::text, @c_RoleId::integer) WHERE out_roleid != 1";
                        cmd = new NpgsqlCommand(query, con);
                        cmd.Parameters.AddWithValue("c_Action", "GETALL");
                        cmd.Parameters.AddWithValue("c_UserId", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_Name", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_Mobile", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_Email", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_Address", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_Password", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_ImageUrl", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_RoleId", DBNull.Value);
                    }
                    else
                    {
                        string query = @"SELECT * FROM User_Crud(@c_Action::text, @c_UserId::integer, @c_Name::varchar, 
                            @c_Mobile::varchar, @c_Email::varchar, @c_Address::text, @c_Password::varchar, 
                            @c_ImageUrl::text, @c_RoleId::integer) 
                            WHERE out_roleid != 1 AND out_name ILIKE @SearchQuery";
                        cmd = new NpgsqlCommand(query, con);
                        cmd.Parameters.AddWithValue("c_Action", "GETALL");
                        cmd.Parameters.AddWithValue("c_UserId", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_Name", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_Mobile", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_Email", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_Address", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_Password", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_ImageUrl", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_RoleId", DBNull.Value);
                        cmd.Parameters.AddWithValue("SearchQuery", "%" + searchQuery + "%");
                    }

                    using (sda = new NpgsqlDataAdapter(cmd))
                    {
                        dt = new DataTable();
                        sda.Fill(dt);
                        rUsers.DataSource = dt;
                        rUsers.DataBind();

                        if (dt.Rows.Count == 0 && !string.IsNullOrEmpty(searchQuery))
                        {
                            ShowMessage("No users found matching your search.", "alert alert-info");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading users: " + ex.Message, "alert alert-danger");
            }
        }

        protected DataTable GetOrderHistory(int userId)
        {
            DataTable dtOrders = new DataTable();
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(Utils.getConnection()))
                {
                    conn.Open();
                    string query = @"SELECT paymentid, orderdetails, amount, paymentmode, paymentstatus, 
                                          address, createddate 
                                   FROM payment 
                                   WHERE userid = @userid 
                                   ORDER BY createddate DESC";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("userid", userId);
                        using (NpgsqlDataAdapter sda = new NpgsqlDataAdapter(cmd))
                        {
                            sda.Fill(dtOrders);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading order history: " + ex.Message, "alert alert-danger");
            }
            return dtOrders;
        }

        protected void btnAddOrUpdate_Click(object sender, EventArgs e)
        {
            string imagePath = string.Empty;
            bool hasNewImage = false;
            int userId = Convert.ToInt32(hdnUserId.Value);

            if (string.IsNullOrEmpty(txtName.Text.Trim()))
            {
                ShowMessage("Name is required!", "alert alert-danger");
                return;
            }

            try
            {
                if (fuUserImage.HasFile)
                {
                    if (Utils.isValidExtension(fuUserImage.FileName))
                    {
                        string newImageName = Utils.getUniqueId();
                        string fileExtension = Path.GetExtension(fuUserImage.FileName);
                        imagePath = "Images/User/" + newImageName + fileExtension;
                        string physicalPath = Server.MapPath("~/Images/User/") + newImageName + fileExtension;
                        fuUserImage.PostedFile.SaveAs(physicalPath);
                        hasNewImage = true;
                    }
                    else
                    {
                        ShowMessage("Please select .jpg, .png or .jpeg", "alert alert-danger");
                        return;
                    }
                }

                using (con = new NpgsqlConnection(Utils.getConnection()))
                {
                    con.Open();
                    string query = @"SELECT * FROM User_Crud(@c_Action::text, @c_UserId::integer, @c_Name::varchar, 
                        @c_Mobile::varchar, @c_Email::varchar, @c_Address::text, @c_Password::varchar, 
                        @c_ImageUrl::text, @c_RoleId::integer)";

                    cmd = new NpgsqlCommand(query, con);

                    cmd.Parameters.AddWithValue("c_Action", userId == 0 ? "INSERT" : "UPDATE");
                    cmd.Parameters.AddWithValue("c_UserId", userId == 0 ? DBNull.Value : (object)userId);
                    cmd.Parameters.AddWithValue("c_Name", txtName.Text.Trim());
                    cmd.Parameters.AddWithValue("c_Mobile", txtMobile.Text.Trim());
                    cmd.Parameters.AddWithValue("c_Email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("c_Address", txtAddress.Text.Trim());
                    cmd.Parameters.AddWithValue("c_Password", txtPassword.Text.Trim());
                    cmd.Parameters.AddWithValue("c_ImageUrl", hasNewImage ? (object)imagePath : DBNull.Value);
                    cmd.Parameters.AddWithValue("c_RoleId", 2); // Normal user role

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            string actionName = userId == 0 ? "added" : "updated";
                            ShowMessage($"User {actionName} successfully!", "alert alert-success");
                            Clear();
                            GetUsers();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error: " + ex.Message, "alert alert-danger");
            }
        }

        protected void rUsers_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "edit")
            {
                try
                {
                    using (con = new NpgsqlConnection(Utils.getConnection()))
                    {
                        con.Open();
                        string query = @"SELECT * FROM User_Crud(@c_Action::text, @c_UserId::integer, @c_Name::varchar, 
                            @c_Mobile::varchar, @c_Email::varchar, @c_Address::text, @c_Password::varchar, 
                            @c_ImageUrl::text, @c_RoleId::integer)";

                        cmd = new NpgsqlCommand(query, con);
                        cmd.Parameters.AddWithValue("c_Action", "GETBYID");
                        cmd.Parameters.AddWithValue("c_UserId", Convert.ToInt32(e.CommandArgument));
                        cmd.Parameters.AddWithValue("c_Name", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_Mobile", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_Email", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_Address", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_Password", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_ImageUrl", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_RoleId", DBNull.Value);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                hdnUserId.Value = reader["out_userid"].ToString();
                                txtName.Text = reader["out_name"].ToString();
                                txtEmail.Text = reader["out_email"].ToString();
                                txtMobile.Text = reader["out_mobile"].ToString();
                                txtAddress.Text = reader["out_address"].ToString();
                                txtPassword.Text = reader["out_password"].ToString();
                                
                                if (Convert.IsDBNull(reader["out_imageurl"]))
                                {
                                    imgUser.ImageUrl = "~/Images/No_image.png";
                                }
                                else
                                {
                                    imgUser.ImageUrl = "~/" + reader["out_imageurl"].ToString();
                                }
                                
                                btnAddOrUpdate.Text = "Update";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("Error: " + ex.Message, "alert alert-danger");
                }
            }
            else if (e.CommandName == "delete")
            {
                try
                {
                    using (con = new NpgsqlConnection(Utils.getConnection()))
                    {
                        con.Open();
                        string query = @"SELECT * FROM User_Crud(@c_Action::text, @c_UserId::integer, @c_Name::varchar, 
                            @c_Mobile::varchar, @c_Email::varchar, @c_Address::text, @c_Password::varchar, 
                            @c_ImageUrl::text, @c_RoleId::integer)";

                        cmd = new NpgsqlCommand(query, con);
                        cmd.Parameters.AddWithValue("c_Action", "DELETE");
                        cmd.Parameters.AddWithValue("c_UserId", Convert.ToInt32(e.CommandArgument));
                        cmd.Parameters.AddWithValue("c_Name", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_Mobile", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_Email", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_Address", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_Password", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_ImageUrl", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_RoleId", DBNull.Value);

                        cmd.ExecuteNonQuery();
                        GetUsers();
                        ShowMessage("User deleted successfully!", "alert alert-success");
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("Error: " + ex.Message, "alert alert-danger");
                }
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string searchQuery = txtSearch.Text.Trim();
            GetUsers(searchQuery);
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void Clear()
        {
            txtName.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtMobile.Text = string.Empty;
            txtAddress.Text = string.Empty;
            txtPassword.Text = string.Empty;
            imgUser.ImageUrl = "~/Images/No_image.png";
            hdnUserId.Value = "0";
            btnAddOrUpdate.Text = "Add";
            lblMsg.Visible = false;
        }

        private void ShowMessage(string message, string cssClass)
        {
            lblMsg.Visible = true;
            lblMsg.Text = message;
            lblMsg.CssClass = cssClass;
        }

        protected void btnDeleteAllUsers_Click(object sender, EventArgs e)
        {
            try
            {
                using (con = new NpgsqlConnection(Utils.getConnection()))
                {
                    con.Open();
                    string query = "CALL delete_users_by_roleid(@roleId)";
                    cmd = new NpgsqlCommand(query, con);
                    cmd.Parameters.AddWithValue("roleId", 2); // Normal user role ID

                    cmd.ExecuteNonQuery();
                    GetUsers();
                    ShowMessage("All users have been deleted successfully!", "alert alert-success");
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error deleting users: " + ex.Message, "alert alert-danger");
            }
        }
    }
} 