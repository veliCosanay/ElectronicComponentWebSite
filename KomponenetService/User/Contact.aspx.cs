using System;
using System.Web.UI;
using Npgsql;

namespace KomponenetService.User
{
    public partial class Contact : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblMsg.Visible = false;
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(Utils.getConnection()))
                {
                    conn.Open();
                    string query = @"INSERT INTO Contact (""Name"", ""Email"", ""Subject"", ""Message"") 
                                   VALUES (@Name, @Email, @Subject, @Message)";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", txtName.Text.Trim());
                        cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                        cmd.Parameters.AddWithValue("@Subject", txtSubject.Text.Trim());
                        cmd.Parameters.AddWithValue("@Message", txtMessage.Text.Trim());

                        cmd.ExecuteNonQuery();
                        
                        ShowMessage("Message sent successfully!", "alert alert-success");
                        Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error sending message: " + ex.Message, "alert alert-danger");
            }
        }

        private void Clear()
        {
            txtName.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtSubject.Text = string.Empty;
            txtMessage.Text = string.Empty;
        }

        private void ShowMessage(string message, string cssClass)
        {
            lblMsg.Visible = true;
            lblMsg.Text = message;
            lblMsg.CssClass = cssClass;
        }
    }
}