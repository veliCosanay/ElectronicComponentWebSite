using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Npgsql;
using System.Web.Services;
using System.Web;

namespace KomponenetService.User
{
    public partial class Cart : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["userId"] == null)
                {
                    Response.Redirect("Login.aspx");
                }
                GetCartItems();
            }
        }

        private void GetCartItems()
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(Utils.getConnection()))
                {
                    conn.Open();
                    string query = @"SELECT c.cartid, c.productid, c.quantity, 
                                   p.productname as name, p.price, p.productimageurl as imageurl 
                                   FROM cart c 
                                   INNER JOIN product p ON c.productid = p.productid 
                                   WHERE c.userid = @userid";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("userid", Convert.ToInt32(Session["userId"]));
                        using (NpgsqlDataAdapter sda = new NpgsqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            sda.Fill(dt);
                            rCartItems.DataSource = dt;
                            rCartItems.DataBind();

                            if (dt.Rows.Count > 0)
                            {
                                decimal subTotal = 0;
                                foreach (DataRow row in dt.Rows)
                                {
                                    subTotal += Convert.ToDecimal(row["price"]) * Convert.ToInt32(row["quantity"]);
                                }

                                decimal shipping = subTotal > 0 ? 50 : 0;
                                decimal total = subTotal + shipping;

                                lblSubTotal.Text = subTotal.ToString("0.00");
                                lblShipping.Text = shipping.ToString("0.00");
                                lblTotal.Text = total.ToString("0.00");
                            }
                            else
                            {
                                lblSubTotal.Text = "0.00";
                                lblShipping.Text = "0.00";
                                lblTotal.Text = "0.00";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage", 
                    "alert('Sepet yüklenirken hata oluştu: " + ex.Message.Replace("'", "\\'") + "');", true);
            }
        }

        protected void rCartItems_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "remove")
            {
                int productId = Convert.ToInt32(e.CommandArgument);
                try
                {
                    using (NpgsqlConnection conn = new NpgsqlConnection(Utils.getConnection()))
                    {
                        conn.Open();
                        string query = "DELETE FROM cart WHERE userid = @userid AND productid = @productid";
                        using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("userid", Convert.ToInt32(Session["userId"]));
                            cmd.Parameters.AddWithValue("productid", productId);
                            cmd.ExecuteNonQuery();
                            GetCartItems();
                            ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage", 
                                "alert('Ürün sepetten kaldırıldı.');", true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage", 
                        "alert('Ürün kaldırılırken hata oluştu: " + ex.Message.Replace("'", "\\'") + "');", true);
                }
            }
        }

        protected void lbCheckout_Click(object sender, EventArgs e)
        {
            Response.Redirect("Payment.aspx");
        }

        [WebMethod]
        public static object GetCartSummary()
        {
            decimal subTotal = 0;
            decimal shipping = 50.00M; // Sabit kargo ücreti

            using (NpgsqlConnection conn = new NpgsqlConnection(Utils.getConnection()))
            {
                conn.Open();
                string query = @"SELECT SUM(p.price * c.quantity) as total
                               FROM cart c
                               JOIN product p ON c.productid = p.productid
                               WHERE c.userid = @userid";
                
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("userid", Convert.ToInt32(HttpContext.Current.Session["userId"]));
                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value && result != null)
                    {
                        subTotal = Convert.ToDecimal(result);
                    }
                }
            }

            return new { 
                subTotal = subTotal, 
                shipping = shipping, 
                total = subTotal + shipping 
            };
        }

        [WebMethod]
        public static void UpdateCartQuantity(int productId, int quantity)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(Utils.getConnection()))
            {
                conn.Open();
                string query = @"UPDATE cart 
                               SET quantity = @quantity 
                               WHERE userid = @userid AND productid = @productid";
                
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("quantity", quantity);
                    cmd.Parameters.AddWithValue("userid", Convert.ToInt32(HttpContext.Current.Session["userId"]));
                    cmd.Parameters.AddWithValue("productid", productId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        [WebMethod]
        public static object RemoveFromCart(int productId)
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(Utils.getConnection()))
                {
                    conn.Open();
                    string query = "DELETE FROM cart WHERE userid = @userid AND productid = @productid";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("userid", Convert.ToInt32(HttpContext.Current.Session["userId"]));
                        cmd.Parameters.AddWithValue("productid", productId);
                        cmd.ExecuteNonQuery();
                    }
                }
                return new { success = true };
            }
            catch (Exception ex)
            {
                return new { success = false, message = ex.Message };
            }
        }
    }
} 