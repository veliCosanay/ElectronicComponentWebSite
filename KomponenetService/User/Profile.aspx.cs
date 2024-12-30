using System;
using System.IO;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Npgsql;

namespace KomponenetService.User
{
    public partial class Profile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["userId"] == null)
                {
                    Response.Redirect("Login.aspx");
                }
                LoadUserProfile();
                GetOrders();
            }
        }

        private void LoadUserProfile()
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(Utils.getConnection()))
                {
                    conn.Open();
                    string query = @"SELECT name, email, mobile, address, imageurl 
                                   FROM users WHERE userid = @userid";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("userid", Convert.ToInt32(Session["userId"]));
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtName.Text = reader["name"].ToString();
                                txtEmail.Text = reader["email"].ToString();
                                txtMobile.Text = reader["mobile"].ToString();
                                txtAddress.Text = reader["address"].ToString();

                                if (reader["imageurl"] != DBNull.Value)
                                {
                                    string imageUrl = reader["imageurl"].ToString();
                                    imgProfile.ImageUrl = "~/" + imageUrl;
                                }
                                else
                                {
                                    imgProfile.ImageUrl = "~/Images/No_image.png";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error loading profile: " + ex.Message);
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                string imagePath = string.Empty;
                bool hasNewImage = false;

                if (fuProfilePicture.HasFile)
                {
                    string fileExtension = Path.GetExtension(fuProfilePicture.FileName).ToLower();
                    if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png" || fileExtension == ".gif")
                    {
                        string fileName = Guid.NewGuid().ToString() + fileExtension;
                        imagePath = "Images/User/" + fileName;
                        string fullPath = Server.MapPath("~/") + imagePath;

                        string directory = Path.GetDirectoryName(fullPath);
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        fuProfilePicture.SaveAs(fullPath);
                        hasNewImage = true;
                    }
                    else
                    {
                        ShowErrorMessage("Please select .jpg, .jpeg, .png or .gif file only!");
                        return;
                    }
                }

                using (NpgsqlConnection conn = new NpgsqlConnection(Utils.getConnection()))
                {
                    conn.Open();
                    string updateQuery;
                    if (!string.IsNullOrEmpty(txtPassword.Text))
                    {
                        updateQuery = hasNewImage ?
                            @"UPDATE users SET name = @name, mobile = @mobile, address = @address, 
                              password = @password, imageurl = @imageurl WHERE userid = @userid" :
                            @"UPDATE users SET name = @name, mobile = @mobile, address = @address, 
                              password = @password WHERE userid = @userid";
                    }
                    else
                    {
                        updateQuery = hasNewImage ?
                            @"UPDATE users SET name = @name, mobile = @mobile, address = @address, 
                              imageurl = @imageurl WHERE userid = @userid" :
                            @"UPDATE users SET name = @name, mobile = @mobile, address = @address 
                              WHERE userid = @userid";
                    }

                    using (NpgsqlCommand cmd = new NpgsqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("name", txtName.Text.Trim());
                        cmd.Parameters.AddWithValue("mobile", txtMobile.Text.Trim());
                        cmd.Parameters.AddWithValue("address", txtAddress.Text.Trim());
                        cmd.Parameters.AddWithValue("userid", Convert.ToInt32(Session["userId"]));

                        if (!string.IsNullOrEmpty(txtPassword.Text))
                        {
                            cmd.Parameters.AddWithValue("password", txtPassword.Text.Trim());
                        }

                        if (hasNewImage)
                        {
                            cmd.Parameters.AddWithValue("imageurl", imagePath);
                        }

                        int result = cmd.ExecuteNonQuery();
                        if (result > 0)
                        {
                            Session["username"] = txtName.Text.Trim();
                            ShowSuccessMessage("Profile updated successfully!");
                            if (hasNewImage)
                            {
                                imgProfile.ImageUrl = "~/" + imagePath;
                            }
                            txtPassword.Text = string.Empty;
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

        private void GetOrders()
        {
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
                        cmd.Parameters.AddWithValue("userid", Convert.ToInt32(Session["userId"]));
                        using (NpgsqlDataAdapter sda = new NpgsqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            sda.Fill(dt);
                            rOrders.DataSource = dt;
                            rOrders.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error loading orders: " + ex.Message);
            }
        }

        private void ShowErrorMessage(string message)
        {
            lblMsg.Visible = true;
            lblMsg.Text = message;
            lblMsg.CssClass = "alert alert-danger";
        }

        private void ShowSuccessMessage(string message)
        {
            lblMsg.Visible = true;
            lblMsg.Text = message;
            lblMsg.CssClass = "alert alert-success";
        }
    }
} 