using Npgsql;
using System;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KomponenetService.Admin
{
    public partial class Categories : System.Web.UI.Page
    {
        private NpgsqlConnection con;
        private NpgsqlCommand cmd;
        private NpgsqlDataAdapter sda;
        private DataTable dt;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblMsg.Visible = false;
                GetCategories();
            }
        }

        private void GetCategories(string searchQuery = "")
        {
            try
            {
                using (con = new NpgsqlConnection(Utils.getConnection()))
                {
                    con.Open();
                    string query = @"SELECT * FROM Category_Crud(@p_Action::text, @p_CategoryId::integer, @p_CategoryName::varchar, @p_CategoryImageUrl::varchar, @p_IsActive::boolean)";
                    cmd = new NpgsqlCommand(query, con);

                    cmd.Parameters.AddWithValue("p_Action", string.IsNullOrEmpty(searchQuery) ? "GETALL" : "SEARCH");
                    cmd.Parameters.AddWithValue("p_CategoryId", DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CategoryName", string.IsNullOrEmpty(searchQuery) ? DBNull.Value : (object)searchQuery);
                    cmd.Parameters.AddWithValue("p_CategoryImageUrl", DBNull.Value);
                    cmd.Parameters.AddWithValue("p_IsActive", DBNull.Value);

                    using (sda = new NpgsqlDataAdapter(cmd))
                    {
                        dt = new DataTable();
                        sda.Fill(dt);
                        rCategory.DataSource = dt;
                        rCategory.DataBind();

                        if (dt.Rows.Count == 0 && !string.IsNullOrEmpty(searchQuery))
                        {
                            ShowMessage("No categories found matching your search.", "alert alert-info");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading categories: " + ex.Message, "alert alert-danger");
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string searchQuery = txtSearch.Text.Trim();
            GetCategories(searchQuery);
        }

        protected void btnAddOrUpdate_Click(object sender, EventArgs e)
        {
            string imagePath = string.Empty;
            bool isValidToExecute = true;
            int categoryId = Convert.ToInt32(hfCategoryId.Value);

            if (string.IsNullOrEmpty(txtCategoryName.Text.Trim()))
            {
                ShowMessage("Category name is required!", "alert alert-danger");
                return;
            }

            using (con = new NpgsqlConnection(Utils.getConnection()))
            {
                cmd = new NpgsqlCommand("SELECT * FROM Category_Crud(@p_Action::text, @p_CategoryId::integer, @p_CategoryName::varchar, @p_CategoryImageUrl::varchar, @p_IsActive::boolean)", con);

                cmd.Parameters.AddWithValue("p_Action", categoryId == 0 ? "INSERT" : "UPDATE");
                cmd.Parameters.AddWithValue("p_CategoryId", categoryId);
                cmd.Parameters.AddWithValue("p_CategoryName", txtCategoryName.Text.Trim());
                cmd.Parameters.AddWithValue("p_IsActive", cbIsActive.Checked);

                if (fuCategoryImage.HasFile)
                {
                    if (Utils.isValidExtension(fuCategoryImage.FileName))
                    {
                        string newImageName = Utils.getUniqueId();
                        string fileExtension = Path.GetExtension(fuCategoryImage.FileName);
                        imagePath = "Images/Category/" + newImageName + fileExtension;
                        string physicalPath = Server.MapPath("~/Images/Category/") + newImageName + fileExtension;

                        try
                        {
                            fuCategoryImage.PostedFile.SaveAs(physicalPath);
                            cmd.Parameters.AddWithValue("p_CategoryImageUrl", imagePath);
                        }
                        catch (Exception ex)
                        {
                            ShowMessage("Error saving image: " + ex.Message, "alert alert-danger");
                            isValidToExecute = false;
                        }
                    }
                    else
                    {
                        ShowMessage("Please select .jpg, .png or .jpeg", "alert alert-danger");
                        isValidToExecute = false;
                    }
                }
                else
                {
                    cmd.Parameters.AddWithValue("p_CategoryImageUrl", DBNull.Value);
                }

                if (isValidToExecute)
                {
                    try
                    {
                        con.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                string actionName = categoryId == 0 ? "inserted" : "updated";
                                ShowMessage($"Category {actionName} successfully!", "alert alert-success");
                                Clear();
                                GetCategories();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowMessage("Database Error: " + ex.Message, "alert alert-danger");
                    }
                }
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void Clear()
        {
            txtCategoryName.Text = string.Empty;
            cbIsActive.Checked = false;
            hfCategoryId.Value = "0";
            btnAddOrUpdate.Text = "Add";
            imagePreview.ImageUrl = "~/Images/No_image.png";
            imagePreview.Height = 200;
            imagePreview.Width = 200;
            lblMsg.Visible = false;
        }

        private void ShowMessage(string message, string cssClass)
        {
            lblMsg.Visible = true;
            lblMsg.Text = message;
            lblMsg.CssClass = cssClass;
        }

        protected void rCategory_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            lblMsg.Visible = false;
            if (e.CommandName == "edit")
            {
                try
                {
                    using (con = new NpgsqlConnection(Utils.getConnection()))
                    {
                        con.Open();
                        cmd = new NpgsqlCommand("SELECT * FROM Category_Crud(@p_Action::text, @p_CategoryId::integer, @p_CategoryName::varchar, @p_CategoryImageUrl::varchar, @p_IsActive::boolean)", con);
                        cmd.Parameters.AddWithValue("p_Action", "GETBYID");
                        cmd.Parameters.AddWithValue("p_CategoryId", Convert.ToInt32(e.CommandArgument));
                        cmd.Parameters.AddWithValue("p_CategoryName", DBNull.Value);
                        cmd.Parameters.AddWithValue("p_CategoryImageUrl", DBNull.Value);
                        cmd.Parameters.AddWithValue("p_IsActive", DBNull.Value);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtCategoryName.Text = reader["out_CategoryName"].ToString();
                                cbIsActive.Checked = Convert.ToBoolean(reader["out_IsActive"]);
                                imagePreview.ImageUrl = string.IsNullOrEmpty(reader["out_CategoryImageUrl"].ToString()) ?
                                    "~/Images/No_image.png" : "~/" + reader["out_CategoryImageUrl"].ToString();
                                imagePreview.Height = 200;
                                imagePreview.Width = 200;
                                hfCategoryId.Value = reader["out_CategoryId"].ToString();
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
                        cmd = new NpgsqlCommand("SELECT * FROM Category_Crud(@p_Action::text, @p_CategoryId::integer, @p_CategoryName::varchar, @p_CategoryImageUrl::varchar, @p_IsActive::boolean)", con);
                        cmd.Parameters.AddWithValue("p_Action", "DELETE");
                        cmd.Parameters.AddWithValue("p_CategoryId", Convert.ToInt32(e.CommandArgument));
                        cmd.Parameters.AddWithValue("p_CategoryName", DBNull.Value);
                        cmd.Parameters.AddWithValue("p_CategoryImageUrl", DBNull.Value);
                        cmd.Parameters.AddWithValue("p_IsActive", DBNull.Value);

                        cmd.ExecuteNonQuery();
                        GetCategories();
                        ShowMessage("Category deleted successfully!", "alert alert-success");
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("Error: " + ex.Message, "alert alert-danger");
                }
            }
        }
    }
}