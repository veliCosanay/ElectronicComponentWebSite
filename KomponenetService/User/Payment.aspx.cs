using System;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Npgsql;

namespace KomponenetService.User
{
    public partial class Payment : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["userId"] == null)
                {
                    Response.Redirect("Login.aspx");
                }
                GetCartTotal();
            }
        }

        private void GetCartTotal()
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(Utils.getConnection()))
                {
                    conn.Open();
                    string query = @"SELECT c.quantity, p.price 
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
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
                    "alert('Error loading cart total: " + ex.Message.Replace("'", "\\'") + "');", true);
            }
        }

        protected void btnPayNow_Click(object sender, EventArgs e)
        {
            try
            {
                // Basic validations
                if (string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(txtCardNo.Text) ||
                    string.IsNullOrEmpty(txtExpiryDate.Text) || string.IsNullOrEmpty(txtCvv.Text) ||
                    string.IsNullOrEmpty(txtAddress.Text))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
                        "alert('Please fill all required fields.');", true);
                    return;
                }

                // Validate card number (must be 16 digits)
                string cardNo = txtCardNo.Text.Trim();
                if (cardNo.Length != 16 || !cardNo.All(c => char.IsDigit(c)))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
                        "alert('Please enter a valid 16-digit card number.');", true);
                    return;
                }

                // Validate expiry date format (MM/YY)
                if (!txtExpiryDate.Text.Contains("/") || txtExpiryDate.Text.Length != 5)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
                        "alert('Please enter expiry date in MM/YY format.');", true);
                    return;
                }

                // Validate CVV (must be 3 digits)
                int cvv;
                if (!int.TryParse(txtCvv.Text, out cvv) || txtCvv.Text.Length != 3)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
                        "alert('Please enter a valid 3-digit CVV number.');", true);
                    return;
                }

                using (NpgsqlConnection conn = new NpgsqlConnection(Utils.getConnection()))
                {
                    conn.Open();
                    using (NpgsqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // 1. Get cart items for order details
                            string cartQuery = @"SELECT c.quantity, p.productname, p.price 
                                               FROM cart c 
                                               INNER JOIN product p ON c.productid = p.productid 
                                               WHERE c.userid = @userid";

                            string orderDetails = "";
                            using (NpgsqlCommand cartCmd = new NpgsqlCommand(cartQuery, conn))
                            {
                                cartCmd.Transaction = transaction;
                                cartCmd.Parameters.AddWithValue("userid", Convert.ToInt32(Session["userId"]));
                                using (NpgsqlDataReader reader = cartCmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        orderDetails += $"{reader["productname"]} x {reader["quantity"]} @ ${reader["price"]}, ";
                                    }
                                }
                            }

                            if (orderDetails.Length > 2)
                            {
                                orderDetails = orderDetails.Substring(0, orderDetails.Length - 2);
                            }

                            // 2. Insert payment record with order details
                            string paymentQuery = @"INSERT INTO payment (
                                                    name, cardno, expirydate, cvvno, address, 
                                                    paymentmode, paymentstatus, userid, amount, orderdetails) 
                                                VALUES (
                                                    @name, @cardno, @expirydate, @cvvno, @address, 
                                                    @paymentmode, 'Completed', @userid, @amount, @orderdetails)";

                            using (NpgsqlCommand cmd = new NpgsqlCommand(paymentQuery, conn))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("name", txtName.Text.Trim());
                                cmd.Parameters.AddWithValue("cardno", cardNo);
                                cmd.Parameters.AddWithValue("expirydate", txtExpiryDate.Text.Trim());
                                cmd.Parameters.AddWithValue("cvvno", txtCvv.Text.Trim());
                                cmd.Parameters.AddWithValue("address", txtAddress.Text.Trim());
                                cmd.Parameters.AddWithValue("paymentmode", ddlPaymentMode.SelectedValue);
                                cmd.Parameters.AddWithValue("userid", Convert.ToInt32(Session["userId"]));
                                cmd.Parameters.AddWithValue("amount", Convert.ToDecimal(lblTotal.Text));
                                cmd.Parameters.AddWithValue("orderdetails", orderDetails);
                                cmd.ExecuteNonQuery();
                            }

                            // 3. Clear the cart
                            string clearCartQuery = "DELETE FROM cart WHERE userid = @userid";
                            using (NpgsqlCommand clearCmd = new NpgsqlCommand(clearCartQuery, conn))
                            {
                                clearCmd.Transaction = transaction;
                                clearCmd.Parameters.AddWithValue("userid", Convert.ToInt32(Session["userId"]));
                                clearCmd.ExecuteNonQuery();
                            }

                            // 4. Commit transaction
                            transaction.Commit();

                            // 5. Show success message and redirect
                            string script = @"alert('Payment successful! Your order has been placed.');
                                           window.location.href = 'Default.aspx';";
                            ScriptManager.RegisterStartupScript(this, GetType(), "successMessage", script, true);
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
                    "alert('Error processing payment: " + ex.Message.Replace("'", "\\'") + "');", true);
            }
        }
    }
}