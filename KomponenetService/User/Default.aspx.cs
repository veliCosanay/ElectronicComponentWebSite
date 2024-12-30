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
    }
}