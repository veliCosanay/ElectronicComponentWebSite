using Npgsql;
using System;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KomponenetService.Admin
{
    public partial class Products : System.Web.UI.Page
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
                GetProducts();
                BindCategoryDropDownList();
            }
        }

        private void GetProducts(string searchQuery = "")
        {
            try
            {
                using (con = new NpgsqlConnection(Utils.getConnection()))
                {
                    con.Open();
                    string query;
                    
                    if (!string.IsNullOrEmpty(searchQuery))
                    {
                        query = @"SELECT productid as out_productid, 
                                productname as out_productname,
                                price as out_price,
                                stock as out_stock,
                                isactive as out_isactive,
                                productimageurl as out_productimageurl,
                                categoryid as out_categoryid,
                                (SELECT categoryname FROM category WHERE categoryid = p.categoryid) as out_categoryname
                                FROM product p
                                WHERE LOWER(productname) = LOWER(@SearchQuery)";
                        
                        cmd = new NpgsqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@SearchQuery", searchQuery);
                    }
                    else
                    {
                        query = @"SELECT * FROM Product_Crud(@c_Action::text, @c_ProductId::integer, @c_ProductName::varchar, 
                            @c_ShortDescription::varchar, @c_LongDescription::text, @c_Price::numeric, @c_CompanyName::varchar, 
                            @c_CategoryId::integer, @c_Stock::integer, @c_IsActive::boolean, @c_ProductImageUrl::varchar)";
                        
                        cmd = new NpgsqlCommand(query, con);
                        cmd.Parameters.AddWithValue("c_Action", "GETALL");
                        cmd.Parameters.AddWithValue("c_ProductId", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_ProductName", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_ShortDescription", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_LongDescription", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_Price", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_CompanyName", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_CategoryId", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_Stock", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_IsActive", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_ProductImageUrl", DBNull.Value);
                    }

                    using (sda = new NpgsqlDataAdapter(cmd))
                    {
                        dt = new DataTable();
                        sda.Fill(dt);
                        rProduct.DataSource = dt;
                        rProduct.DataBind();

                        if (dt.Rows.Count == 0 && !string.IsNullOrEmpty(searchQuery))
                        {
                            ShowMessage($"No products found matching: {searchQuery}", "alert alert-warning");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading products: " + ex.Message, "alert alert-danger");
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string searchQuery = txtSearch.Text.Trim();
            GetProducts(searchQuery);
        }

        protected void btnAddOrUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtProductName.Text.Trim()))
            {
                ShowMessage("Product name is required!", "alert alert-danger");
                return;
            }

            if (string.IsNullOrEmpty(hfCategoryId.Value) || hfCategoryId.Value == "0")
            {
                ShowMessage("Please select a valid category.", "alert alert-danger");
                return;
            }

            string imagePath = string.Empty;
            int productId = Convert.ToInt32(hfProductId.Value);
            int categoryId = Convert.ToInt32(hfCategoryId.Value);

            try
            {
                using (con = new NpgsqlConnection(Utils.getConnection()))
                {
                    con.Open();
                    string query = @"SELECT * FROM Product_Crud(@c_Action::text, @c_ProductId::integer, @c_ProductName::varchar, 
                        @c_ShortDescription::varchar, @c_LongDescription::text, @c_Price::numeric, @c_CompanyName::varchar, 
                        @c_CategoryId::integer, @c_Stock::integer, @c_IsActive::boolean, @c_ProductImageUrl::varchar)";

                    cmd = new NpgsqlCommand(query, con);

                    cmd.Parameters.AddWithValue("c_Action", productId == 0 ? "INSERT" : "UPDATE");
                    cmd.Parameters.AddWithValue("c_ProductId", productId == 0 ? DBNull.Value : (object)productId);
                    cmd.Parameters.AddWithValue("c_ProductName", txtProductName.Text.Trim());
                    cmd.Parameters.AddWithValue("c_ShortDescription", txtShortDesc.Text.Trim());
                    cmd.Parameters.AddWithValue("c_LongDescription", txtLongDesc.Text.Trim());
                    cmd.Parameters.AddWithValue("c_Price", Convert.ToDecimal(txtPrice.Text.Trim()));
                    cmd.Parameters.AddWithValue("c_CompanyName", txtCompanyName.Text.Trim());
                    cmd.Parameters.AddWithValue("c_CategoryId", categoryId);
                    cmd.Parameters.AddWithValue("c_Stock", Convert.ToInt32(txtStock.Text.Trim()));
                    cmd.Parameters.AddWithValue("c_IsActive", cbIsActive.Checked);

                    if (fuProductImage.HasFile)
                    {
                        if (Utils.isValidExtension(fuProductImage.FileName))
                        {
                            string newImageName = Utils.getUniqueId();
                            string fileExtension = Path.GetExtension(fuProductImage.FileName);
                            imagePath = "Images/Product/" + newImageName + fileExtension;
                            string physicalPath = Server.MapPath("~/Images/Product/") + newImageName + fileExtension;

                            try
                            {
                                fuProductImage.PostedFile.SaveAs(physicalPath);
                                cmd.Parameters.AddWithValue("c_ProductImageUrl", imagePath);
                            }
                            catch (Exception ex)
                            {
                                ShowMessage("Error saving image: " + ex.Message, "alert alert-danger");
                                return;
                            }
                        }
                        else
                        {
                            ShowMessage("Please select .jpg, .png or .jpeg", "alert alert-danger");
                            return;
                        }
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("c_ProductImageUrl", DBNull.Value);
                    }

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            string actionName = productId == 0 ? "inserted" : "updated";
                            ShowMessage($"Product {actionName} successfully!", "alert alert-success");
                            Clear();
                            GetProducts();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error saving product: " + ex.Message, "alert alert-danger");
            }
        }

        protected void rProduct_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            lblMsg.Visible = false;
            if (e.CommandName == "edit")
            {
                try
                {
                    using (con = new NpgsqlConnection(Utils.getConnection()))
                    {
                        con.Open();
                        string query = @"SELECT * FROM Product_Crud(@c_Action::text, @c_ProductId::integer, @c_ProductName::varchar, 
                            @c_ShortDescription::varchar, @c_LongDescription::text, @c_Price::numeric, @c_CompanyName::varchar, 
                            @c_CategoryId::integer, @c_Stock::integer, @c_IsActive::boolean, @c_ProductImageUrl::varchar)";

                        cmd = new NpgsqlCommand(query, con);

                        cmd.Parameters.AddWithValue("c_Action", "GETBYID");
                        cmd.Parameters.AddWithValue("c_ProductId", Convert.ToInt32(e.CommandArgument));
                        cmd.Parameters.AddWithValue("c_ProductName", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_ShortDescription", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_LongDescription", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_Price", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_CompanyName", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_CategoryId", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_Stock", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_IsActive", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_ProductImageUrl", DBNull.Value);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                hfProductId.Value = reader["out_productid"].ToString();
                                txtProductName.Text = reader["out_productname"].ToString();
                                txtShortDesc.Text = reader["out_shortdescription"].ToString();
                                txtLongDesc.Text = reader["out_longdescription"].ToString();
                                txtPrice.Text = reader["out_price"].ToString();
                                txtCompanyName.Text = reader["out_companyname"].ToString();
                                hfCategoryId.Value = reader["out_categoryid"].ToString();
                                ddlCategory.SelectedValue = reader["out_categoryid"].ToString();
                                txtStock.Text = reader["out_stock"].ToString();
                                cbIsActive.Checked = Convert.ToBoolean(reader["out_isactive"]);
                                imgProduct.ImageUrl = string.IsNullOrEmpty(reader["out_productimageurl"].ToString()) ?
                                    "~/Images/No_image.png" : "~/" + reader["out_productimageurl"].ToString();
                                imgProduct.Height = 200;
                                imgProduct.Width = 200;
                                btnAddOrUpdate.Text = "Update";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("Error loading product: " + ex.Message, "alert alert-danger");
                }
            }
            else if (e.CommandName == "delete")
            {
                try
                {
                    using (con = new NpgsqlConnection(Utils.getConnection()))
                    {
                        con.Open();
                        string query = @"SELECT * FROM Product_Crud(@c_Action::text, @c_ProductId::integer, @c_ProductName::varchar, 
                            @c_ShortDescription::varchar, @c_LongDescription::text, @c_Price::numeric, @c_CompanyName::varchar, 
                            @c_CategoryId::integer, @c_Stock::integer, @c_IsActive::boolean, @c_ProductImageUrl::varchar)";

                        cmd = new NpgsqlCommand(query, con);

                        cmd.Parameters.AddWithValue("c_Action", "DELETE");
                        cmd.Parameters.AddWithValue("c_ProductId", Convert.ToInt32(e.CommandArgument));
                        cmd.Parameters.AddWithValue("c_ProductName", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_ShortDescription", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_LongDescription", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_Price", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_CompanyName", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_CategoryId", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_Stock", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_IsActive", DBNull.Value);
                        cmd.Parameters.AddWithValue("c_ProductImageUrl", DBNull.Value);

                        cmd.ExecuteNonQuery();
                        GetProducts();
                        ShowMessage("Product deleted successfully!", "alert alert-success");
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("Error deleting product: " + ex.Message, "alert alert-danger");
                }
            }
        }

        private void BindCategoryDropDownList()
        {
            try
            {
                using (con = new NpgsqlConnection(Utils.getConnection()))
                {
                    con.Open();
                    string query = @"SELECT * FROM Category_Crud(@p_Action::text, @p_CategoryId::integer, @p_CategoryName::varchar, @p_CategoryImageUrl::varchar, @p_IsActive::boolean)";
                    cmd = new NpgsqlCommand(query, con);

                    cmd.Parameters.AddWithValue("p_Action", "GETALL");
                    cmd.Parameters.AddWithValue("p_CategoryId", DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CategoryName", DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CategoryImageUrl", DBNull.Value);
                    cmd.Parameters.AddWithValue("p_IsActive", DBNull.Value);

                    using (sda = new NpgsqlDataAdapter(cmd))
                    {
                        dt = new DataTable();
                        sda.Fill(dt);
                        ddlCategory.DataSource = dt;
                        ddlCategory.DataTextField = "out_CategoryName";
                        ddlCategory.DataValueField = "out_CategoryId";
                        ddlCategory.DataBind();
                        ddlCategory.Items.Insert(0, new ListItem("Select Category", "0"));
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading categories: " + ex.Message, "alert alert-danger");
            }
        }

        protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlCategory.SelectedValue != "0" && !string.IsNullOrEmpty(ddlCategory.SelectedValue))
            {
                hfCategoryId.Value = ddlCategory.SelectedValue;
            }
            else
            {
                ShowMessage("Please select a valid category", "alert alert-warning");
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void Clear()
        {
            txtProductName.Text = string.Empty;
            txtShortDesc.Text = string.Empty;
            txtLongDesc.Text = string.Empty;
            txtPrice.Text = string.Empty;
            txtCompanyName.Text = string.Empty;
            hfProductId.Value = "0";
            hfCategoryId.Value = "0";
            ddlCategory.SelectedValue = "0";
            cbIsActive.Checked = false;
            txtStock.Text = string.Empty;
            btnAddOrUpdate.Text = "Add";
            lblMsg.Visible = false;
            imgProduct.ImageUrl = "~/Images/No_image.png";
            imgProduct.Height = 200;
            imgProduct.Width = 200;
        }

        private void ShowMessage(string message, string cssClass)
        {
            lblMsg.Visible = true;
            lblMsg.Text = message;
            lblMsg.CssClass = cssClass;
        }
    }
}