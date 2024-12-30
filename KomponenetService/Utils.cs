using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace KomponenetService
{
    public class Utils
    {
        public static string getConnection()
        {
            return ConfigurationManager.ConnectionStrings["cs"].ConnectionString;
        }

        public static bool isValidExtension(string fileName)
        {
            bool isValid = false;
            string[] fileExtension = { ".jpg", ".png", ".jpeg" };
            foreach (string extension in fileExtension)
            {
                if(fileName.Contains(extension)) { isValid = true; break; }
            }
            return isValid;
        }

        public static string getUniqueId()
        {
            return Guid.NewGuid().ToString();
        }

        public static string getImageUrl(Object url)
        {
            string url1 = string.Empty;
            if (string.IsNullOrEmpty(url.ToString()) || url == DBNull.Value || url == null)
            {
                url1 = "../Images/No_image.png";
            }
            else
            {
                url1 = string.Format("../{0}", url);
            }
            return url1;
        }
    }
}