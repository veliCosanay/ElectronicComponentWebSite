using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Npgsql;
using System.Data;

namespace KomponenetService.User
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetRandomProducts();
            }
        }

        private void GetRandomProducts()
        {
            try
            {
                using (var conn = new NpgsqlConnection(Utils.getConnection()))
                {
                    conn.Open();
                    string query = @"SELECT * FROM Product WHERE isactive = true ORDER BY RANDOM() LIMIT 8";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        using (var adapter = new NpgsqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                rProducts.DataSource = dt;
                                rProducts.DataBind();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error loading products: " + ex.Message + "');</script>");
            }
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

                                if (stock <= 0)
                                {
                                    Response.Write("<script>alert('Üzgünüz, bu ürün stokta yok.');</script>");
                                    return;
                                }

                                // Sepette bu üründen var mı kontrol et
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
                        }
                        Response.Write(@"<script>
                            alert('Ürün sepete eklendi.');
                            window.location.href = 'Cart.aspx';
                        </script>");
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