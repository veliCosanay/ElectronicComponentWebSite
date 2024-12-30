using System;
using System.IO;
using System.Data;
using Npgsql;
using System.Web.UI;
using System.Diagnostics;
using System.Configuration;

namespace KomponenetService.User
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["userId"] != null)
                {
                    Response.Redirect("Default.aspx");
                }
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            lblMsg.Visible = true;

            try
            {
                // Bağlantı kontrolü
                string connectionString = ConfigurationManager.ConnectionStrings["cs"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    lblMsg.Text = "Database connection string is missing!";
                    lblMsg.CssClass = "alert alert-danger";
                    return;
                }

                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                    }
                    catch (Exception ex)
                    {
                        lblMsg.Text = "Database connection failed: " + ex.Message;
                        lblMsg.CssClass = "alert alert-danger";
                        return;
                    }

                    if (!Page.IsValid)
                    {
                        lblMsg.Text = "Please fill all required fields.";
                        lblMsg.CssClass = "alert alert-danger";
                        return;
                    }

                    lblMsg.Text = "Processing registration...";
                    lblMsg.CssClass = "alert alert-info";

                    // Email kontrolü
                    string checkQuery = @"SELECT COUNT(*) FROM Users WHERE Email = @Email";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(checkQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        if (count > 0)
                        {
                            lblMsg.Text = "Email already exists!";
                            lblMsg.CssClass = "alert alert-danger";
                            return;
                        }
                    }

                    string imagePath = string.Empty;
                    if (fuProfilePicture.HasFile)
                    {
                        string fileExtension = Path.GetExtension(fuProfilePicture.FileName);
                        if (fileExtension.ToLower() == ".jpg" || fileExtension.ToLower() == ".jpeg" ||
                            fileExtension.ToLower() == ".png" || fileExtension.ToLower() == ".gif")
                        {
                            string fileName = Path.GetFileName(fuProfilePicture.FileName);
                            imagePath = "Images/User/" + fileName;
                            string fullPath = Server.MapPath("~/") + imagePath;

                            string directory = Path.GetDirectoryName(fullPath);
                            if (!Directory.Exists(directory))
                            {
                                Directory.CreateDirectory(directory);
                            }

                            fuProfilePicture.SaveAs(fullPath);
                        }
                        else
                        {
                            lblMsg.Text = "Please select .jpg, .jpeg, .png or .gif file only!";
                            lblMsg.CssClass = "alert alert-danger";
                            return;
                        }
                    }

                    // Kullanıcı kaydı
                    string insertQuery = @"INSERT INTO Users (Name, Email, Mobile, Address, Password, ImageUrl, RoleId) 
                                         VALUES (@Name, @Email, @Mobile, @Address, @Password, @ImageUrl, @RoleId)";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", txtName.Text.Trim());
                        cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                        cmd.Parameters.AddWithValue("@Mobile", txtMobile.Text.Trim());
                        cmd.Parameters.AddWithValue("@Address", txtAddress.Text.Trim());
                        cmd.Parameters.AddWithValue("@Password", txtPassword.Text.Trim());
                        cmd.Parameters.AddWithValue("@ImageUrl", string.IsNullOrEmpty(imagePath) ? DBNull.Value : (object)imagePath);
                        cmd.Parameters.AddWithValue("@RoleId", 2); // 2 = Customer role

                        try
                        {
                            int result = cmd.ExecuteNonQuery();
                            if (result > 0)
                            {
                                // Form'u temizle
                                txtName.Text = string.Empty;
                                txtEmail.Text = string.Empty;
                                txtMobile.Text = string.Empty;
                                txtAddress.Text = string.Empty;
                                txtPassword.Text = string.Empty;

                                Response.Write("<script>alert('Registration successful! Please login to continue.');window.location.href='Login.aspx';</script>");
                            }
                            else
                            {
                                lblMsg.Text = "Registration failed. No rows were affected.";
                                lblMsg.CssClass = "alert alert-danger";
                            }
                        }
                        catch (NpgsqlException ex)
                        {
                            lblMsg.Text = "Database error: " + ex.Message;
                            lblMsg.CssClass = "alert alert-danger";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error: " + ex.Message;
                lblMsg.CssClass = "alert alert-danger";
            }
        }
    }
}