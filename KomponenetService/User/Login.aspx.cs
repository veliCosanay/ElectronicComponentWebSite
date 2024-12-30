using System;
using System.Data;
using Npgsql;
using System.Configuration;
using System.Diagnostics;
using System.Web.Security;
using System.Web.UI;

namespace KomponenetService.User
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session.Clear();
                FormsAuthentication.SignOut();
                Debug.WriteLine("Sayfa yüklendi");
                ShowMessage("Sayfa yüklendi. Giriş yapabilirsiniz.", "alert alert-info");
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("Login butonu tıklandı");
            try
            {
                ShowMessage("Login butonu tıklandı.", "alert alert-info");

                string email = txtEmail.Text.Trim().ToLower();
                string password = txtPassword.Text.Trim();

                Debug.WriteLine($"Email: {email}, Password: {password}");
                ShowMessage($"Email: {email}, Password: {password}", "alert alert-info");

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    ShowErrorMessage("Lütfen email ve şifre alanlarını doldurun.");
                    return;
                }

                string connectionString = ConfigurationManager.ConnectionStrings["cs"].ConnectionString;
                Debug.WriteLine($"Bağlantı dizesi: {connectionString}");

                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    try
                    {
                        Debug.WriteLine("Veritabanı bağlantısı oluşturuluyor...");

                        conn.Open();
                        Debug.WriteLine("Veritabanına bağlantı başarılı.");

                        // Doğrudan login sorgusunu çalıştıralım
                        string loginQuery = @"
                            SELECT u.UserId, u.Name, u.Email, u.RoleId, r.RoleName 
                            FROM Users u
                            JOIN Roles r ON u.RoleId = r.RoleId
                            WHERE LOWER(u.Email) = LOWER(@Email) AND u.Password = @Password";

                        using (NpgsqlCommand cmd = new NpgsqlCommand(loginQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@Email", email);
                            cmd.Parameters.AddWithValue("@Password", password);

                            Debug.WriteLine($"Sorgu: {loginQuery}");
                            Debug.WriteLine($"Parametreler - Email: {email}, Password: {password}");

                            using (NpgsqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    int roleId = Convert.ToInt32(reader["RoleId"]);
                                    string roleName = reader["RoleName"].ToString();
                                    string userId = reader["UserId"].ToString();
                                    string userName = reader["Name"].ToString();

                                    Debug.WriteLine($"Kullanıcı bulundu - RoleId: {roleId}, RoleName: {roleName}");

                                    // Session'ları ayarla
                                    Session["userId"] = userId;
                                    Session["username"] = userName;
                                    Session["email"] = email;
                                    Session["roleId"] = roleId.ToString();
                                    Session["roleName"] = roleName;

                                    // Forms Authentication ticket oluştur
                                    FormsAuthentication.SetAuthCookie(email, false);

                                    // Debug bilgileri
                                    string debugInfo = $@"
                                        <strong>Giriş Bilgileri:</strong><br/>
                                        User ID: {userId}<br/>
                                        Email: {email}<br/>
                                        Kullanıcı Adı: {userName}<br/>
                                        Rol ID: {roleId}<br/>
                                        Rol Adı: {roleName}";
                                    
                                    Debug.WriteLine(debugInfo.Replace("<br/>", "\n"));
                                    ShowMessage(debugInfo, "alert alert-info");

                                    // Rol kontrolü ve yönlendirme
                                    Debug.WriteLine($"Rol kontrolü yapılıyor - RoleId: {roleId}");
                                    if (roleId == 1)
                                    {
                                        Debug.WriteLine("Admin rolü tespit edildi, yönlendirme yapılıyor...");
                                        ShowMessage("Admin rolü tespit edildi. Admin paneline yönlendiriliyorsunuz...", "alert alert-success");
                                        try
                                        {
                                            Response.Redirect("~/Admin/Dashboard.aspx", false);
                                            Context.ApplicationInstance.CompleteRequest();
                                        }
                                        catch (System.Threading.ThreadAbortException)
                                        {
                                            // Bu beklenen bir durum
                                        }
                                        catch (Exception ex)
                                        {
                                            Debug.WriteLine($"Yönlendirme hatası: {ex.Message}");
                                            ShowErrorMessage("Yönlendirme sırasında bir hata oluştu: " + ex.Message);
                                        }
                                    }
                                    else
                                    {
                                        Debug.WriteLine("Normal kullanıcı rolü tespit edildi, Default.aspx'e yönlendirme yapılıyor...");
                                        ShowMessage("Kullanıcı rolü tespit edildi. Ana sayfaya yönlendiriliyorsunuz...", "alert alert-success");
                                        try
                                        {
                                            Response.Redirect("Default.aspx", false);
                                            Context.ApplicationInstance.CompleteRequest();
                                        }
                                        catch (System.Threading.ThreadAbortException)
                                        {
                                            // Bu beklenen bir durum
                                        }
                                        catch (Exception ex)
                                        {
                                            Debug.WriteLine($"Yönlendirme hatası: {ex.Message}");
                                            ShowErrorMessage("Yönlendirme sırasında bir hata oluştu: " + ex.Message);
                                        }
                                    }
                                }
                                else
                                {
                                    Debug.WriteLine("Kullanıcı bulunamadı veya şifre hatalı");
                                    ShowErrorMessage("Email veya şifre hatalı!");
                                }
                            }
                        }
                    }
                    catch (NpgsqlException ex)
                    {
                        Debug.WriteLine($"PostgreSQL Hatası: {ex.Message}, Error Code: {ex.ErrorCode}");
                        ShowErrorMessage($"Veritabanı hatası: {ex.Message}<br/>Error Code: {ex.ErrorCode}");
                    }
                    catch (Exception ex)
                    {
                        if (!(ex is System.Threading.ThreadAbortException))
                        {
                            Debug.WriteLine($"Genel Hata: {ex.Message}\nStack Trace: {ex.StackTrace}");
                            ShowErrorMessage($"Hata: {ex.Message}<br/>Stack Trace: {ex.StackTrace}");
                        }
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                            Debug.WriteLine("Veritabanı bağlantısı kapatıldı");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"En Üst Seviye Hata: {ex.Message}");
                ShowErrorMessage($"Genel hata: {ex.Message}");
            }
        }

        private void ShowErrorMessage(string message)
        {
            ShowMessage(message, "alert alert-danger");
        }

        private void ShowMessage(string message, string cssClass)
        {
            lblMsg.Visible = true;
            lblMsg.Text = message;
            lblMsg.CssClass = cssClass;
            Debug.WriteLine($"Mesaj gösteriliyor: {message}");
        }
    }
}
