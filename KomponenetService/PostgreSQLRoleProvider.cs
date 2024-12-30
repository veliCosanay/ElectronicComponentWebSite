using System;
using System.Configuration;
using System.Configuration.Provider;
using System.Web.Security;
using Npgsql;

namespace KomponenetService.Models
{
    public class PostgreSQLRoleProvider : RoleProvider
    {
        private string _applicationName;
        public override string ApplicationName
        {
            get { return _applicationName; }
            set { _applicationName = value; }
        }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = "PostgreSQLRoleProvider";

            if (String.IsNullOrEmpty(config["connectionStringName"]))
                throw new ProviderException("Connection string cannot be blank.");

            if (String.IsNullOrEmpty(config["applicationName"]))
            {
                _applicationName = "/";
            }
            else
            {
                _applicationName = config["applicationName"];
            }

            base.Initialize(name, config);
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["cs"].ConnectionString))
            {
                conn.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT RoleName FROM Roles", conn))
                {
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        var roles = new System.Collections.Generic.List<string>();
                        while (reader.Read())
                        {
                            roles.Add(reader["RoleName"].ToString());
                        }
                        return roles.ToArray();
                    }
                }
            }
        }

        public override string[] GetRolesForUser(string username)
        {
            if (string.IsNullOrEmpty(username))
                return new string[] { };

            using (NpgsqlConnection conn = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["cs"].ConnectionString))
            {
                conn.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand(@"
                    SELECT r.RoleName 
                    FROM Users u 
                    JOIN Roles r ON u.RoleId = r.RoleId 
                    WHERE LOWER(u.Email) = LOWER(@Email)", conn))
                {
                    cmd.Parameters.AddWithValue("@Email", username);
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new string[] { reader["RoleName"].ToString() };
                        }
                    }
                }
            }
            return new string[] { };
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(roleName))
                return false;

            using (NpgsqlConnection conn = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["cs"].ConnectionString))
            {
                conn.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand(@"
                    SELECT COUNT(*) 
                    FROM Users u 
                    JOIN Roles r ON u.RoleId = r.RoleId 
                    WHERE LOWER(u.Email) = LOWER(@Email) AND LOWER(r.RoleName) = LOWER(@RoleName)", conn))
                {
                    cmd.Parameters.AddWithValue("@Email", username);
                    cmd.Parameters.AddWithValue("@RoleName", roleName);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
                return false;

            using (NpgsqlConnection conn = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["cs"].ConnectionString))
            {
                conn.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT COUNT(*) FROM Roles WHERE LOWER(RoleName) = LOWER(@RoleName)", conn))
                {
                    cmd.Parameters.AddWithValue("@RoleName", roleName);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }
    }
} 