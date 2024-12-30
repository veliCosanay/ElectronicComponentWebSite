using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Npgsql;

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

        protected void btnUpdateCart_Click(object sender, EventArgs e)
        {
            bool success = true;
            string errorMessage = "";

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(Utils.getConnection()))
                {
                    conn.Open();
                    foreach (RepeaterItem item in rCartItems.Items)
                    {
                        if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                        {
                            TextBox txtQuantity = (TextBox)item.FindControl("txtQuantity");
                            if (txtQuantity != null)
                            {
                                int productId = Convert.ToInt32(txtQuantity.Attributes["CommandArgument"]);
                                int quantity;
                                if (int.TryParse(txtQuantity.Text, out quantity) && quantity > 0)
                                {
                                    string query = "UPDATE cart SET quantity = @quantity WHERE userid = @userid AND productid = @productid";
                                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                                    {
                                        cmd.Parameters.AddWithValue("quantity", quantity);
                                        cmd.Parameters.AddWithValue("userid", Convert.ToInt32(Session["userId"]));
                                        cmd.Parameters.AddWithValue("productid", productId);
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                                else
                                {
                                    success = false;
                                    errorMessage = "Miktar geçerli bir sayı olmalıdır ve 0'dan büyük olmalıdır.";
                                    break;
                                }
                            }
                        }
                    }
                }

                if (success)
                {
                    GetCartItems();
                    ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage", 
                        "alert('Sepet başarıyla güncellendi.');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage", 
                        "alert('" + errorMessage.Replace("'", "\\'") + "');", true);
                }
            }
            catch (PostgresException pgEx)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage", 
                    "alert('PostgreSQL Hatası: " + pgEx.MessageText.Replace("'", "\\'") + "');", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage", 
                    "alert('Sepet güncellenirken hata oluştu: " + ex.Message.Replace("'", "\\'") + "');", true);
            }
        }

        protected void lbCheckout_Click(object sender, EventArgs e)
        {
            Response.Redirect("Payment.aspx");
        }
    }
} 