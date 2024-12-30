using System;
using System.Data;
using Npgsql;
using System.Web.UI;
using System.Diagnostics;

namespace KomponenetService.User
{
    public partial class User : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Debug.WriteLine("Page_Load başladı");
                LoadCategories();
                if (Session["userId"] != null)
                {
                    UpdateCartCount();
                }
            }
            Page.DataBind();
        }

        private void LoadCategories()
        {
            Debug.WriteLine("LoadCategories başladı");
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(Utils.getConnection()))
                {
                    Debug.WriteLine("Veritabanı bağlantısı oluşturuluyor...");
                    conn.Open();
                    Debug.WriteLine("Veritabanı bağlantısı başarılı");

                    string query = @"SELECT categoryid as CategoryId, categoryname as Name 
                                   FROM category 
                                   WHERE isactive = true 
                                   ORDER BY categoryname";
                    Debug.WriteLine($"Sorgu: {query}");

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            rCategories.DataSource = dt;
                            rCategories.DataBind();
                            Debug.WriteLine($"Toplam {dt.Rows.Count} kategori bulundu");
                            Debug.WriteLine("Kategoriler Repeater'a bağlandı");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hata oluştu: {ex.Message}");
                Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                Response.Write("<script>alert('Error loading categories: " + ex.Message + "');</script>");
            }
        }

        protected void lbLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("Login.aspx");
        }

        public void UpdateCartCount()
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(Utils.getConnection()))
                {
                    conn.Open();
                    string query = @"SELECT COALESCE(SUM(quantity), 0) 
                                   FROM cart WHERE userid = @userid";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("userid", Convert.ToInt32(Session["userId"]));
                        int cartCount = Convert.ToInt32(cmd.ExecuteScalar());
                        lblCartCount.InnerText = cartCount.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error updating cart count: " + ex.Message + "');</script>");
            }
        }

        public void UpdateCartCount(int count)
        {
            lblCartCount.InnerText = count.ToString();
        }
    }
}