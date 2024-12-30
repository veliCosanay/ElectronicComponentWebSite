using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Npgsql;

namespace KomponenetService.User
{
    public partial class Shop : System.Web.UI.Page
    {
        NpgsqlConnection conn = new NpgsqlConnection(Utils.getConnection());
        private const int PageSize = 8;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ViewState["CurrentPage"] = 1;
                ViewState["CategoryId"] = 0;
                ViewState["PriceRange"] = "all";
                PopulateCategories();
                PopulateProducts();
            }
        }

        private void PopulateCategories()
        {
            try
            {
                string query = @"SELECT c.CategoryId, c.CategoryName, COUNT(p.ProductId) as ProductCount 
                               FROM Category c 
                               LEFT JOIN Product p ON c.CategoryId = p.CategoryId 
                               GROUP BY c.CategoryId, c.CategoryName 
                               ORDER BY c.CategoryName";

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    DataTable dt = new DataTable();
                    conn.Open();
                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(cmd);
                    adapter.Fill(dt);

                    rCategories.DataSource = dt;
                    rCategories.DataBind();
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error: " + ex.Message + "');</script>");
            }
            finally
            {
                conn.Close();
            }
        }

        private void PopulateProducts(string searchQuery = "", string sortBy = "")
        {
            try
            {
                int currentPage = (int)ViewState["CurrentPage"];
                int categoryId = ViewState["CategoryId"] != null ? (int)ViewState["CategoryId"] : 0;
                string priceRange = ViewState["PriceRange"].ToString();

                string query = @"SELECT p.*, c.CategoryName 
                               FROM Product p 
                               INNER JOIN Category c ON p.CategoryId = c.CategoryId 
                               WHERE 1=1";

                if (!string.IsNullOrEmpty(searchQuery))
                {
                    query += " AND LOWER(p.ProductName) LIKE LOWER(@Search)";
                }

                if (categoryId > 0)
                {
                    query += " AND p.CategoryId = @CategoryId";
                }

                switch (priceRange.ToLower())
                {
                    case "0-100":
                        query += " AND p.Price BETWEEN 0 AND 100";
                        break;
                    case "100-500":
                        query += " AND p.Price BETWEEN 100 AND 500";
                        break;
                    case "500plus":
                        query += " AND p.Price > 500";
                        break;
                }

                switch (sortBy)
                {
                    case "latest":
                        query += " ORDER BY p.CreatedDate DESC";
                        break;
                    case "price_asc":
                        query += " ORDER BY p.Price ASC";
                        break;
                    case "price_desc":
                        query += " ORDER BY p.Price DESC";
                        break;
                    default:
                        query += " ORDER BY p.ProductId DESC";
                        break;
                }

                query += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    if (!string.IsNullOrEmpty(searchQuery))
                    {
                        cmd.Parameters.AddWithValue("@Search", "%" + searchQuery + "%");
                    }
                    if (categoryId > 0)
                    {
                        cmd.Parameters.AddWithValue("@CategoryId", categoryId);
                    }
                    cmd.Parameters.AddWithValue("@Offset", (currentPage - 1) * PageSize);
                    cmd.Parameters.AddWithValue("@PageSize", PageSize);

                    DataTable dt = new DataTable();
                    conn.Open();
                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(cmd);
                    adapter.Fill(dt);

                    rProducts.DataSource = dt;
                    rProducts.DataBind();

                    // Get total count for pagination
                    string countQuery = @"SELECT COUNT(*) FROM Product p WHERE 1=1";
                    if (!string.IsNullOrEmpty(searchQuery))
                    {
                        countQuery += " AND LOWER(p.ProductName) LIKE LOWER(@Search)";
                    }
                    if (categoryId > 0)
                    {
                        countQuery += " AND p.CategoryId = @CategoryId";
                    }
                    switch (priceRange.ToLower())
                    {
                        case "0-100":
                            countQuery += " AND p.Price BETWEEN 0 AND 100";
                            break;
                        case "100-500":
                            countQuery += " AND p.Price BETWEEN 100 AND 500";
                            break;
                        case "500plus":
                            countQuery += " AND p.Price > 500";
                            break;
                    }

                    cmd.CommandText = countQuery;
                    int totalRecords = Convert.ToInt32(cmd.ExecuteScalar());
                    int totalPages = (int)Math.Ceiling((double)totalRecords / PageSize);

                    // Bind pagination
                    rPaging.DataSource = Enumerable.Range(1, totalPages);
                    rPaging.DataBind();

                    // Enable/disable previous/next buttons
                    lbPrevious.Enabled = currentPage > 1;
                    lbNext.Enabled = currentPage < totalPages;
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error: " + ex.Message + "');</script>");
            }
            finally
            {
                conn.Close();
            }
        }

        protected void lbSearch_Click(object sender, EventArgs e)
        {
            ViewState["CurrentPage"] = 1;
            PopulateProducts(txtSearch.Text.Trim());
        }

        protected void ddlSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            ViewState["CurrentPage"] = 1;
            PopulateProducts(txtSearch.Text.Trim(), ddlSort.SelectedValue);
        }

        protected void rCategories_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "category")
            {
                ViewState["CurrentPage"] = 1;
                ViewState["CategoryId"] = Convert.ToInt32(e.CommandArgument);
                PopulateProducts(txtSearch.Text.Trim(), ddlSort.SelectedValue);
            }
        }

        protected void PriceFilter_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            ViewState["CurrentPage"] = 1;

            if (rb == rbAll)
                ViewState["PriceRange"] = "all";
            else if (rb == rb0_100)
                ViewState["PriceRange"] = "0-100";
            else if (rb == rb100_500)
                ViewState["PriceRange"] = "100-500";
            else if (rb == rb500plus)
                ViewState["PriceRange"] = "500plus";

            PopulateProducts(txtSearch.Text.Trim(), ddlSort.SelectedValue);
        }

        protected void rProducts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "addToCart")
            {
                if (Session["userId"] == null)
                {
                    Response.Redirect("Login.aspx");
                }
                else
                {
                    try
                    {
                        using (NpgsqlConnection conn = new NpgsqlConnection(Utils.getConnection()))
                        {
                            conn.Open();
                            // Önce stok kontrolü yap
                            string stockQuery = "SELECT stock FROM product WHERE productid = @productid";
                            using (NpgsqlCommand stockCmd = new NpgsqlCommand(stockQuery, conn))
                            {
                                stockCmd.Parameters.AddWithValue("productid", Convert.ToInt32(e.CommandArgument));
                                int stock = Convert.ToInt32(stockCmd.ExecuteScalar());
                                
                                if (stock < 1)
                                {
                                    Response.Write("<script>alert('Üzgünüz, ürün stokta yok.');</script>");
                                    return;
                                }
                            }

                            // Ürün sepette var mı kontrol et
                            string checkQuery = @"SELECT quantity FROM cart 
                                                WHERE userid = @userid AND productid = @productid";
                            using (NpgsqlCommand checkCmd = new NpgsqlCommand(checkQuery, conn))
                            {
                                checkCmd.Parameters.AddWithValue("userid", Convert.ToInt32(Session["userId"]));
                                checkCmd.Parameters.AddWithValue("productid", Convert.ToInt32(e.CommandArgument));
                                object result = checkCmd.ExecuteScalar();

                                if (result != null) // Ürün zaten sepette varsa
                                {
                                    int currentQty = Convert.ToInt32(result);
                                    string updateQuery = @"UPDATE cart 
                                                         SET quantity = @quantity 
                                                         WHERE userid = @userid AND productid = @productid";
                                    using (NpgsqlCommand updateCmd = new NpgsqlCommand(updateQuery, conn))
                                    {
                                        updateCmd.Parameters.AddWithValue("quantity", currentQty + 1);
                                        updateCmd.Parameters.AddWithValue("userid", Convert.ToInt32(Session["userId"]));
                                        updateCmd.Parameters.AddWithValue("productid", Convert.ToInt32(e.CommandArgument));
                                        updateCmd.ExecuteNonQuery();
                                    }
                                }
                                else // Ürün sepette yoksa yeni ekle
                                {
                                    string insertQuery = @"INSERT INTO cart (userid, productid, quantity) 
                                                         VALUES (@userid, @productid, @quantity)";
                                    using (NpgsqlCommand insertCmd = new NpgsqlCommand(insertQuery, conn))
                                    {
                                        insertCmd.Parameters.AddWithValue("userid", Convert.ToInt32(Session["userId"]));
                                        insertCmd.Parameters.AddWithValue("productid", Convert.ToInt32(e.CommandArgument));
                                        insertCmd.Parameters.AddWithValue("quantity", 1);
                                        insertCmd.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                        Response.Write("<script>alert('Ürün sepete eklendi.');</script>");
                        ((User)this.Master).UpdateCartCount();
                    }
                    catch (PostgresException pgEx)
                    {
                        Response.Write($"<script>alert('PostgreSQL Error: {pgEx.MessageText}\\nDetail: {pgEx.Detail}\\nHint: {pgEx.Hint}');</script>");
                    }
                    catch (Exception ex)
                    {
                        Response.Write($"<script>alert('Error: {ex.Message}');</script>");
                    }
                }
            }
            else if (e.CommandName == "viewDetail")
            {
                Response.Redirect($"ProductDetails.aspx?id={e.CommandArgument}");
            }
        }

        protected void lbPrevious_Click(object sender, EventArgs e)
        {
            int currentPage = (int)ViewState["CurrentPage"];
            if (currentPage > 1)
            {
                ViewState["CurrentPage"] = currentPage - 1;
                PopulateProducts(txtSearch.Text.Trim(), ddlSort.SelectedValue);
            }
        }

        protected void lbNext_Click(object sender, EventArgs e)
        {
            int currentPage = (int)ViewState["CurrentPage"];
            ViewState["CurrentPage"] = currentPage + 1;
            PopulateProducts(txtSearch.Text.Trim(), ddlSort.SelectedValue);
        }

        protected void rPaging_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "page")
            {
                ViewState["CurrentPage"] = Convert.ToInt32(e.CommandArgument);
                PopulateProducts(txtSearch.Text.Trim(), ddlSort.SelectedValue);
            }
        }
    }
}