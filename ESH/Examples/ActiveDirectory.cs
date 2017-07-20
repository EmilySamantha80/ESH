using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESH.Utility.ActiveDirectory;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;

namespace ESH.Examples
{
    public class ActiveDirectory
    {
        /// <summary>
        /// Test the functions
        /// </summary>
        public static void Test()
        {
            string ldapServer = "contoso.internal";
            string ldapContainer = "OU=Users,DC=contoso,DC=internal";
            string ldapUser = null;
            string ldapPassword = null;

            string user = "fooUser";
            string firstName = "Foo";
            string lastName = "User";
            string password = "fooPassword";
            string group = "fooGroup";

            var context = Connect(ldapServer, ldapContainer, ldapUser, ldapPassword);

            Console.WriteLine("CreateUser: " + CreateUser(user, firstName, lastName, context));

            Console.WriteLine("AuthenticateUser: " + AuthenticateUser(user, password, context).ToString());

            Console.WriteLine("IsUserInGroup: " + IsUserInGroup(user, group, context).ToString());

            Console.WriteLine("GetAllGroups:");
            foreach (var principal in GetAllGroups(context))
            {
                Console.WriteLine(principal.DistinguishedName);
            }

            Console.WriteLine("GetGroupMembers:");
            foreach (var principal in GetGroupMembers(group, context))
            {
                Console.WriteLine(principal.DistinguishedName);
            }

            Console.WriteLine("GetUserGroupMembership:");
            foreach (var principal in GetUserGroupMembership(user, context))
            {
                Console.WriteLine(principal.DistinguishedName);
            }

            Console.WriteLine("GetAllContainers (Recursive):");
            foreach (var container in GetAllContainers(context, true))
            {
                Console.WriteLine(container.Path.Substring(container.Path.LastIndexOf('/') + 1));
            }

            Console.Write("SetPassword: ");
            SetPassword(user, password, context);
            Console.WriteLine("OK");
        }

        /// <summary>
        /// Connect to an Active Directory server
        /// </summary>
        /// <param name="ldapServer">Server hostname/IP</param>
        /// <param name="ldapContainer">Base container to connect to</param>
        /// <param name="ldapUser">Username to connect with. Null if connecting as the current user</param>
        /// <param name="ldapPassword">Password to connect with</param>
        /// <returns>ADContext object representing the connection</returns>
        public static ADContext Connect(string ldapServer, string ldapContainer, string ldapUser = null, string ldapPassword = null)
        {
            var context = new ADContext();
            try
            {
                if (String.IsNullOrWhiteSpace(ldapUser))
                {
                    context.Connect(ldapServer, ldapContainer);
                }
                else
                {
                    context.Connect(ldapServer, ldapContainer, ldapUser, ldapPassword);
                }
            }
            catch
            {
                throw new Exception("Failed to connect to AD server: " + ldapServer);
            }

            return context;
        }

        /// <summary>
        /// Authenticate a user
        /// </summary>
        /// <param name="user">Username to authenticate</param>
        /// <param name="password">User's password</param>
        /// <param name="context">Active context connection</param>
        /// <returns>Whether or not the user is authenticated successfully</returns>
        public static bool AuthenticateUser(string user, string password, ADContext context)
        {
            bool login_result = false;
            ADUserPrincipal userPrincipal = null;
            string resultString = String.Empty;
            login_result = context.ValidateCredentials(user, password, out userPrincipal, out resultString);

            return login_result;
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="samAccountName">SamAccountName (username) for the new user</param>
        /// <param name="firstName">New user's first name</param>
        /// <param name="lastName">New user's last name</param>
        /// <param name="context">Active context connection</param>
        /// <returns>The created ADUserPrincipal</returns>
        public static UserPrincipal CreateUser(string samAccountName, string firstName, string lastName, ADContext context)
        {
            ADUserPrincipal userPrincipal = ADUserPrincipal.FindOneByAttribute(context.PrincipalContext, "sAMAccountName", samAccountName, MatchType.Equals);
            if (userPrincipal != null)
                throw new Exception("User already exists: " + samAccountName);

            userPrincipal = new ADUserPrincipal(context.PrincipalContext);
            userPrincipal.SamAccountName = samAccountName;
            userPrincipal.GivenName = firstName;
            userPrincipal.Surname = lastName;
            userPrincipal.Name = firstName + " " + lastName;
            //Example of how to set attributes that aren't available as properties
            userPrincipal.SetAttribute("employeeId", "123456789");
            userPrincipal.Save();

            return userPrincipal;
        }

        /// <summary>
        /// Check if a user is in a group
        /// </summary>
        /// <param name="user">Username to check</param>
        /// <param name="group">Group to check</param>
        /// <param name="context">Active context connection</param>
        /// <returns>Whether or not the user is part of the group</returns>
        public static bool IsUserInGroup(string user, string group, ADContext context)
        {
            ADGroupPrincipal groupPrincipal = ADGroupPrincipal.FindOneByAttribute(context.PrincipalContext, "sAMAccountName", group, MatchType.Equals);
            ADUserPrincipal userPrincipal = ADUserPrincipal.FindOneByAttribute(context.PrincipalContext, "sAMAccountName", user, MatchType.Equals);
            if (groupPrincipal == null)
                throw new Exception("Failed to find group: " + group);

            if (userPrincipal == null)
                throw new Exception("Failed to find user: " + user);

            if (userPrincipal.IsMemberOf(groupPrincipal))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Gets all groups under the context's base OU
        /// </summary>
        /// <param name="context">Active context connection</param>
        /// <returns>List of all groups</returns>
        public static List<ADGroupPrincipal> GetAllGroups(ADContext context)
        {
            List<ADGroupPrincipal> principals = ADGroupPrincipal.FindByAttribute(context.PrincipalContext, "sAMAccountName", "*", MatchType.Equals);

            return principals;

        }

        /// <summary>
        /// Get all members of a group
        /// </summary>
        /// <param name="group">Group to check</param>
        /// <param name="context">Active context connection</param>
        /// <returns>List of group members</returns>
        public static PrincipalCollection GetGroupMembers(string group, ADContext context)
        {
            ADGroupPrincipal principal = ADGroupPrincipal.FindOneByAttribute(context.PrincipalContext, "sAMAccountName", group, MatchType.Equals);

            if (principal == null)
            {
                throw new Exception("Group not found: " + group);
            }

            return principal.Members;
        }

        /// <summary>
        /// Get a user's group membership
        /// </summary>
        /// <param name="user">User to check</param>
        /// <param name="context">Active context connection</param>
        /// <returns>List of groups</returns>
        public static PrincipalSearchResult<Principal> GetUserGroupMembership(string user, ADContext context)
        {
            ADUserPrincipal userPrincipal = ADUserPrincipal.FindOneByAttribute(context.PrincipalContext, "sAMAccountName", user, MatchType.Equals);
            if (userPrincipal == null)
                throw new Exception("User does not exist: " + user);

            return userPrincipal.GetGroups();
        }

        /// <summary>
        /// Gets all containers and OUs under the context's base OU
        /// </summary>
        /// <param name="context">Active context connection</param>
        /// <param name="recurseContainers">Whether or not to recurse containers</param>
        /// <param name="entries">Existing entries. Initially this should be null</param>
        /// <param name="currentEntry">Current entry that is being evaluated. Initially this should be null</param>
        /// <returns>List of containers</returns>
        public static List<DirectoryEntry> GetAllContainers(ADContext context, bool recurseContainers = false, List<DirectoryEntry> entries = null, DirectoryEntry currentEntry = null)
        {
            if (entries == null)
            {
                entries = new List<DirectoryEntry>();
            }

            var containers = new List<DirectoryEntry>();
            if (currentEntry == null)
            {
                containers = context.GetDirectoryEntries(SchemaClass.container | SchemaClass.organizationalUnit);
                if (!recurseContainers)
                {
                    return containers;
                }
            }
            else
            {
                containers = context.GetDirectoryEntries(SchemaClass.container | SchemaClass.organizationalUnit, currentEntry.Path.Substring(currentEntry.Path.LastIndexOf('/') + 1));
            }
            if (containers.Count() == 0)
            {
                return new List<DirectoryEntry>();
            }
            foreach (var container in containers)
            {
                entries.Add(container);
                entries.AddRange(GetAllContainers(context, true, entries, container));
            }
            return entries;
        }

        /// <summary>
        /// Set a user's password
        /// </summary>
        /// <param name="user">User to set password on</param>
        /// <param name="password">New password</param>
        /// <param name="context">Active context connection</param>
        public static void SetPassword(string user, string password, ADContext context)
        {
            ADUserPrincipal userPrincipal = ADUserPrincipal.FindOneByAttribute(context.PrincipalContext, "sAMAccountName", user, MatchType.Equals);
            if (userPrincipal == null)
                throw new Exception("User does not exist: " + user);

            userPrincipal.SetPassword(password);
        }
    }
}
