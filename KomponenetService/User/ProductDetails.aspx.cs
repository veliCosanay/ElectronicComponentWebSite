using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Npgsql;

namespace KomponenetService.User
{
    public partial class ProductDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["id"] != null)
                {
                    LoadProductDetails();
                    LoadRelatedProducts();
                }
                else
                {
                    Response.Redirect("Shop.aspx");
                }
            }
        }

        private void LoadProductDetails()
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(Utils.getConnection()))
                {
                    conn.Open();
                    string query = @"SELECT p.productid, p.productname, p.shortdescription, p.longdescription, 
                                   p.price, p.productimageurl 
                                   FROM product p 
                                   WHERE p.productid = @productid";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("productid", Convert.ToInt32(Request.QueryString["id"]));
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblName.Text = reader["productname"].ToString();
                                lblDescription.Text = reader["longdescription"] != DBNull.Value ? 
                                    reader["longdescription"].ToString() : 
                                    reader["shortdescription"].ToString();
                                lblPrice.Text = Convert.ToDecimal(reader["price"]).ToString("0.00");
                                imgProduct.ImageUrl = Utils.getImageUrl(reader["productimageurl"]);
                            }
                            else
                            {
                                Response.Write("<script>alert('Ürün bulunamadı.');</script>");
                                Response.Redirect("Shop.aspx");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string errorMessage = "Error loading product details: ";
                if (ex is PostgresException pgEx)
                {
                    errorMessage += $"SQL Error: {pgEx.MessageText}";
                }
                else
                {
                    errorMessage += ex.Message;
                }
                Response.Write($"<script>alert('{errorMessage}');</script>");
                Response.Redirect("Shop.aspx");
            }
        }

        private void LoadRelatedProducts()
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(Utils.getConnection()))
                {
                    conn.Open();
                    string query = @"SELECT p.productid, p.productname as name, p.price, p.productimageurl as imageurl 
                                   FROM product p 
                                   WHERE p.categoryid = (
                                       SELECT categoryid FROM product 
                                       WHERE productid = @productid
                                   ) 
                                   AND p.productid != @productid 
                                   AND p.isactive = true
                                   LIMIT 4";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("productid", Convert.ToInt32(Request.QueryString["id"]));
                        using (NpgsqlDataAdapter sda = new NpgsqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            sda.Fill(dt);
                            rRelatedProducts.DataSource = dt;
                            rRelatedProducts.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error loading related products: " + ex.Message + "');</script>");
            }
        }

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            if (Session["userId"] == null)
            {
                Response.Redirect("Login.aspx");
            }
            else
            {
                try
                {
                    int quantity = Convert.ToInt32(txtQuantity.Text);
                    if (quantity <= 0)
                    {
                        Response.Write("<script>alert('Lütfen geçerli bir miktar girin.');</script>");
                        return;
                    }

                    using (NpgsqlConnection conn = new NpgsqlConnection(Utils.getConnection()))
                    {
                        conn.Open();
                        // Önce stok kontrolü yap
                        string stockQuery = "SELECT stock FROM product WHERE productid = @productid";
                        using (NpgsqlCommand stockCmd = new NpgsqlCommand(stockQuery, conn))
                        {
                            stockCmd.Parameters.AddWithValue("productid", Convert.ToInt32(Request.QueryString["id"]));
                            int stock = Convert.ToInt32(stockCmd.ExecuteScalar());
                            
                            if (stock < quantity)
                            {
                                Response.Write("<script>alert('Üzgünüz, yeterli stok yok. Mevcut stok: " + stock + "');</script>");
                                return;
                            }
                        }

                        // Ürün sepette var mı kontrol et
                        string checkQuery = @"SELECT quantity FROM cart 
                                            WHERE userid = @userid AND productid = @productid";
                        using (NpgsqlCommand checkCmd = new NpgsqlCommand(checkQuery, conn))
                        {
                            checkCmd.Parameters.AddWithValue("userid", Convert.ToInt32(Session["userId"]));
                            checkCmd.Parameters.AddWithValue("productid", Convert.ToInt32(Request.QueryString["id"]));
                            object result = checkCmd.ExecuteScalar();

                            if (result != null) // Ürün zaten sepette varsa
                            {
                                int currentQty = Convert.ToInt32(result);
                                string updateQuery = @"UPDATE cart 
                                                     SET quantity = @quantity 
                                                     WHERE userid = @userid AND productid = @productid";
                                using (NpgsqlCommand updateCmd = new NpgsqlCommand(updateQuery, conn))
                                {
                                    updateCmd.Parameters.AddWithValue("quantity", currentQty + quantity);
                                    updateCmd.Parameters.AddWithValue("userid", Convert.ToInt32(Session["userId"]));
                                    updateCmd.Parameters.AddWithValue("productid", Convert.ToInt32(Request.QueryString["id"]));
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
                                    insertCmd.Parameters.AddWithValue("productid", Convert.ToInt32(Request.QueryString["id"]));
                                    insertCmd.Parameters.AddWithValue("quantity", quantity);
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

        protected void rRelatedProducts_ItemCommand(object source, RepeaterCommandEventArgs e)
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
        }
    }
} 