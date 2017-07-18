// Title:  Full featured Active Directory client in C#
// Author: Emily Heiner
// Date:   2016-12-08
//
// MIT License
// Copyright(c) 2017 Emily Heiner (emilysamantha80@gmail.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.


using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.DirectoryServices.AccountManagement;
using System.Security;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Linq;

/// <summary>
/// Full featured C# Active Directory client
/// </summary>
namespace ESH.Utility.ActiveDirectory
{
    /// <summary>
    ///     Specifies the different schema classes of an Active Directory object.
    ///     The application can set multiple options that are linked with a bitwise OR operation.
    ///     SchemaClass.Any represents all of the schema classes.
    /// </summary>
    [Serializable]
    [Flags]
    public enum SchemaClass
    {
        //Any Schema Class
        Any = 1,
        //Organizational Unit
        organizationalUnit = 2,
        //Container
        container = 4,
        //Built-in Domain Container
        builtinDomain = 8,
        //Group
        group = 16,
        //User
        user = 32,
        //Computer
        computer = 64,
        //Printer
        printer = 128,
        //Contact
        contact = 256,
        //Shared Folder
        volume = 512,
        //InetOrgPerson
        inetOrgPerson = 1024,
        //Query-based distribution Group
        msExchDynamicDistributionList = 2048
    }

    /// <summary>
    /// This object represents the connection to the Active Directory server.
    /// It also contains methods to manipulate Active Directory objects.
    /// </summary>
    public class ADContext
    {
        /// <summary>
        /// Used for GetNewDirectoryEntry(). It is set when you connect to the server.
        /// </summary>
        private Principal _DefaultPrincipal;


        private ContextType _ContextType = ContextType.Domain;
        /// <summary>
        /// The ContextType of the current connection to the server.
        /// </summary>
        public ContextType ContextType
        {
            get
            {
                return _ContextType;
            }
        }

        private string _Container = String.Empty;
        /// <summary>
        /// The distinguished name of the container that the current connection is bound to
        /// </summary>
        public string Container
        {
            get
            {
                return _Container;
            }
        }

        private string _Server = String.Empty;
        /// <summary>
        /// The Active Directory server that is currently connected.
        /// </summary>
        public string Server
        {
            get
            {
                return _Server;
            }
        }

        private string _UserName = String.Empty;
        /// <summary>
        /// The username that was used to connect to the Active Directory Server.
        /// If connected using the applicatoin's credentials, this will be empty.
        /// </summary>
        public string UserName
        {
            get
            {
                return _UserName;
            }
        }

        private SecureString _Password = new SecureString();
        /// <summary>
        /// The password that was used to connect to the Active Directory Server.
        /// If connected using the applicatoin's credentials, this will be empty.
        /// </summary>
        public SecureString Password
        {
            get
            {
                return _Password;
            }
        }

        private bool _ConnectedUsingPassword = false;
        /// <summary>
        /// Returns true if connected to the server by specifying a username and password, false if connected using the application's credentials.
        /// </summary>
        public bool ConnectedUsingPassword
        {
            get
            {
                return _ConnectedUsingPassword;
            }
        }

        private ContextOptions _ContextOptions = (ContextOptions.ServerBind | ContextOptions.Negotiate);
        /// <summary>
        /// The ContextOptions that were used to connect to the server.
        /// </summary>
        public ContextOptions ContextOptions
        {
            get
            {
                return _ContextOptions;
            }
        }

        /// <summary>
        /// The PrincipalContext object that is used for Active Directory operations.
        /// </summary>
        private PrincipalContext _PrincipalContext;
        public PrincipalContext PrincipalContext
        {
            get
            {
                return _PrincipalContext;
            }
        }

        /// <summary>
        /// Creates a new ADContext instance.
        /// </summary>
        public ADContext()
        {
        }

        /// <summary>
        /// Connects to an Active Directory server using the application's credentials.
        /// </summary>
        /// <param name="server">The Active Directory server to connect to.</param>
        /// <param name="container">The distinguished name of the container to bind to</param>
        public void Connect(string server, string container)
        {
            Connect(server, container, this.ContextOptions);
        }

        /// <summary>
        /// Connects to an Active Directory server using the application's credentials.
        /// </summary>
        /// <param name="server">The Active Directory server to connect to.</param>
        /// <param name="container">The distinguished name of the container to bind to</param>
        /// <param name="contextOptions">The options that are used to bind to the server</param>
        public void Connect(string server, string container, ContextOptions contextOptions)
        {
            this._Server = server;
            this._Container = container;
            this._UserName = String.Empty;
            this._Password = new SecureString();
            this._ContextOptions = contextOptions;
            this._ConnectedUsingPassword = false;
            Match match = Regex.Match(container, @"(DC\=.*)", RegexOptions.CultureInvariant);
            if (match.Groups.Count > 0)
            {
                _PrincipalContext = new PrincipalContext(this._ContextType, this._Server, match.Groups[0].Value, this._ContextOptions);
                _DefaultPrincipal = ADUserPrincipal.FindOneByAttribute(_PrincipalContext, "sAMAccountName", "*");
                if (_DefaultPrincipal == null)
                {
                    _DefaultPrincipal = ADGroupPrincipal.FindOneByAttribute(_PrincipalContext, "sAMAccountName", "*");
                }
                ChangeContainer(container);
            }
            else
            {
                _PrincipalContext = new PrincipalContext(this._ContextType, this._Server, this._Container, this._ContextOptions);
                _DefaultPrincipal = ADUserPrincipal.FindOneByAttribute(_PrincipalContext, "sAMAccountName", "*");
                if (_DefaultPrincipal == null)
                {
                    _DefaultPrincipal = ADGroupPrincipal.FindOneByAttribute(_PrincipalContext, "sAMAccountName", "*");
                }
            }
        }

        /// <summary>
        /// Connects to an Active Directory server specifying the username and password.
        /// </summary>
        /// <param name="server">The Active Directory server to connect to.</param>
        /// <param name="container">The distinguished name of the container to bind to</param>
        /// <param name="userName">The username to connect using</param>
        /// <param name="password">The password to connect using</param>
        public void Connect(string server, string container, string userName, string password)
        {
            Connect(server, container, userName, ToSecureString(password));
        }

        /// <summary>
        /// Connects to an Active Directory server specifying the username and password.
        /// </summary>
        /// <param name="server">The Active Directory server to connect to.</param>
        /// <param name="container">The distinguished name of the container to bind to</param>
        /// <param name="userName">The username to connect using</param>
        /// <param name="password">The password to connect using</param>
        public void Connect(string server, string container, string userName, SecureString password)
        {
            Connect(server, container, userName, password, this.ContextOptions);
        }

        /// <summary>
        /// Connects to an Active Directory server specifying the username and password.
        /// </summary>
        /// <param name="server">The Active Directory server to connect to.</param>
        /// <param name="container">The distinguished name of the container to bind to</param>
        /// <param name="userName">The username to connect using</param>
        /// <param name="password">The password to connect using</param>
        /// <param name="contextOptions">The options that are used to bind to the server</param>
        public void Connect(string server, string container, string userName, string password, ContextOptions contextOptions)
        {
            Connect(server, container, userName, ToSecureString(password), contextOptions);
        }

        /// <summary>
        /// Connects to an Active Directory server specifying the username and password.
        /// </summary>
        /// <param name="server">The Active Directory server to connect to.</param>
        /// <param name="container">The distinguished name of the container to bind to</param>
        /// <param name="userName">The username to connect using</param>
        /// <param name="password">The password to connect using</param>
        /// <param name="contextOptions">The options that are used to bind to the server</param>
        public void Connect(string server, string container, string userName, SecureString password, ContextOptions contextOptions)
        {
            this._Server = server;
            this._Container = container;
            this._UserName = userName;
            this._Password = password;
            this._ContextOptions = contextOptions;
            this._ConnectedUsingPassword = true;
            if (_ContextType != ContextType.Machine)
            {
                Match match = Regex.Match(container, @"(DC\=.*)", RegexOptions.CultureInvariant);
                if (match.Groups.Count > 0)
                {
                    _PrincipalContext = new PrincipalContext(this._ContextType, this._Server, match.Groups[0].Value, this._ContextOptions, this._UserName, ToInsecureString(this._Password));
                    _DefaultPrincipal = ADUserPrincipal.FindOneByAttribute(_PrincipalContext, "sAMAccountName", "*");
                    if (_DefaultPrincipal == null)
                    {
                        _DefaultPrincipal = ADGroupPrincipal.FindOneByAttribute(_PrincipalContext, "sAMAccountName", "*");
                    }
                    ChangeContainer(container);
                }
                else
                {
                    _PrincipalContext = new PrincipalContext(this._ContextType, this._Server, this._Container, this._ContextOptions, this._UserName, ToInsecureString(this._Password));
                    _DefaultPrincipal = ADUserPrincipal.FindOneByAttribute(_PrincipalContext, "sAMAccountName", "*");
                    if (_DefaultPrincipal == null)
                    {
                        _DefaultPrincipal = ADGroupPrincipal.FindOneByAttribute(_PrincipalContext, "sAMAccountName", "*");
                    }
                }
            }
            else
            {
                _PrincipalContext = new PrincipalContext(this._ContextType, this._Server);
            }

        }

        /// <summary>
        /// Boolean value representing the connection state. Returns true if connected, false if not connected.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                bool result = false;
                if (_PrincipalContext != null && !String.IsNullOrEmpty(_PrincipalContext.ConnectedServer))
                {
                    result = true;
                }
                return result;
            }
        }

        /// <summary>
        /// Gets all of the DirectoryEntry objects in the current container.
        /// </summary>
        /// <returns>A List object containing the returned DirectoryEntry objects.</returns>
        public List<DirectoryEntry> GetDirectoryEntries()
        {
            return GetDirectoryEntries(SchemaClass.Any, this._Container);
        }

        /// <summary>
        /// Gets all of the DirectoryEntry objects in the specified container.
        /// </summary>
        /// <param name="container">The distinguished name of the container to bind to</param>
        /// <returns>A List object containing the returned DirectoryEntry objects.</returns>
        public List<DirectoryEntry> GetDirectoryEntries(string container)
        {
            return GetDirectoryEntries(SchemaClass.Any, container);
        }

        /// <summary>
        /// Gets all of the DirectoryEntry objects in the current container that are in a particular SchemaClass.
        /// </summary>
        /// <param name="schemaClass">The SchemaClass to retrieve. The application can set multiple options that are linked with a bitwise OR operation.</param>
        /// <returns>A List object containing the returned DirectoryEntry objects.</returns>
        public List<DirectoryEntry> GetDirectoryEntries(SchemaClass schemaClass)
        {
            return GetDirectoryEntries(schemaClass, this._Container);
        }

        /// <summary>
        /// Gets all of the DirectoryEntry objects in the specified container that are in a particular SchemaClass.
        /// </summary>
        /// <param name="schemaClass">The SchemaClass to retrieve. The application can set multiple options that are linked with a bitwise OR operation.</param>
        /// <param name="container">The distinguished name of the container to bind to</param>
        /// <returns>A List object containing the returned DirectoryEntry objects.</returns>
        public List<DirectoryEntry> GetDirectoryEntries(SchemaClass schemaClass, string container)
        {
            var entries = new List<DirectoryEntry>();
            var principal = new UserPrincipal(_PrincipalContext);
            var baseEntry = GetNewDirectoryEntry(container);

            foreach (DirectoryEntry entry in baseEntry.Children)
            {
                if (schemaClass.HasFlag(SchemaClass.Any))
                    entries.Add(entry);
                else if (schemaClass.HasFlag(SchemaClass.organizationalUnit) && entry.SchemaClassName == SchemaClass.organizationalUnit.ToString())
                    entries.Add(entry);
                else if (schemaClass.HasFlag(SchemaClass.container) && entry.SchemaClassName == SchemaClass.container.ToString())
                    entries.Add(entry);
                else if (schemaClass.HasFlag(SchemaClass.builtinDomain) && entry.SchemaClassName == SchemaClass.builtinDomain.ToString())
                    entries.Add(entry);
                else if (schemaClass.HasFlag(SchemaClass.group) && entry.SchemaClassName == SchemaClass.group.ToString())
                    entries.Add(entry);
                else if (schemaClass.HasFlag(SchemaClass.user) && entry.SchemaClassName == SchemaClass.user.ToString())
                    entries.Add(entry);
                else if (schemaClass.HasFlag(SchemaClass.computer) && entry.SchemaClassName == SchemaClass.computer.ToString())
                    entries.Add(entry);
                else if (schemaClass.HasFlag(SchemaClass.printer) && entry.SchemaClassName == SchemaClass.printer.ToString())
                    entries.Add(entry);
                else if (schemaClass.HasFlag(SchemaClass.contact) && entry.SchemaClassName == SchemaClass.contact.ToString())
                    entries.Add(entry);
                else if (schemaClass.HasFlag(SchemaClass.volume) && entry.SchemaClassName == SchemaClass.volume.ToString())
                    entries.Add(entry);
                else if (schemaClass.HasFlag(SchemaClass.inetOrgPerson) && entry.SchemaClassName == SchemaClass.inetOrgPerson.ToString())
                    entries.Add(entry);
                else if (schemaClass.HasFlag(SchemaClass.msExchDynamicDistributionList) && entry.SchemaClassName == SchemaClass.msExchDynamicDistributionList.ToString())
                    entries.Add(entry);
            }
            return entries;
        }

        /// <summary>
        /// Change the current connection's container.
        /// This will disconnect and reconnect to the Active Directory server.
        /// </summary>
        /// <param name="container">The distinguished name of the container to bind to</param>
        public void ChangeContainer(string container)
        {
            if (this._ConnectedUsingPassword)
            {
                _PrincipalContext = new PrincipalContext(this._ContextType, this._Server, container, this._ContextOptions, this._UserName, ToInsecureString(this._Password));
            }
            else
            {
                _PrincipalContext = new PrincipalContext(this._ContextType, this._Server, container, this._ContextOptions);
            }
        }

        /// <summary>
        /// Gets a new DirectoryEntry object with the connection information from the current connection.
        /// The DirectoryEntry object will be bound to the current connection's container.
        /// If there is no current connection to an Active Directory server, an ActiveDirectoryOperationException will be thrown.
        /// NOTE: There must be at least one user or group object on the Active Directory server for this function to work.
        /// </summary>
        /// <param name="container">The distinguished name of the container to bind to</param>
        /// <returns>A new DirectoryEntry object with connection information and container set</returns>
        public DirectoryEntry GetNewDirectoryEntry()
        {
            return GetNewDirectoryEntry(_Container);
        }

        /// <summary>
        /// Gets a new DirectoryEntry object with the connection information from the current connection.
        /// The DirectoryEntry object will be bound to the container specified.
        /// If there is no current connection to an Active Directory server, an ActiveDirectoryOperationException will be thrown.
        /// NOTE: There must be at least one user or group object on the Active Directory server for this function to work.
        /// </summary>
        /// <param name="container">The distinguished name of the container to bind to</param>
        /// <returns>A new DirectoryEntry object with connection information and container set</returns>
        public DirectoryEntry GetNewDirectoryEntry(string container)
        {
            if (!IsConnected)
            {
                throw new ActiveDirectoryOperationException("There is no current connection to an Active Directory server.");
            }

            if (_DefaultPrincipal == null)
            {
                throw new ActiveDirectoryOperationException("The default principal object does not exist.");
            }
            var sourceEntry = (DirectoryEntry)_DefaultPrincipal.GetUnderlyingObject();

            DirectoryEntry destinationEntry = new DirectoryEntry();
            if (_ConnectedUsingPassword)
            {
                destinationEntry.AuthenticationType = sourceEntry.AuthenticationType;
                destinationEntry.Username = this._UserName;
                destinationEntry.Password = ToInsecureString(this._Password);
            }
            else
            {
                destinationEntry.AuthenticationType = sourceEntry.AuthenticationType;
            }
            string pathPrefix = String.Empty;
            if (this._ContextType == ContextType.Domain || this._ContextType == ContextType.ApplicationDirectory)
            {
                pathPrefix = "LDAP://";
            }
            else if (this._ContextType == ContextType.Machine)
            {
                pathPrefix = "WinNT://";
            }
            string separator = "/";
            if (String.IsNullOrEmpty(container))
            {
                separator = "";
            }

            destinationEntry.Path = pathPrefix + this._Server + separator + container;

            return destinationEntry;
        }

        /// <summary>
        /// Moves a Principal to a different container.
        /// If there is no current connection to an Active Directory server, an ActiveDirectoryOperationException will be thrown.
        /// </summary>
        /// <param name="principal">The Principal to move.</param>
        /// <param name="container">The distinguished name of the new container.</param>
        public void MovePrincipal(Principal principal, string container)
        {
            if (!IsConnected)
            {
                throw new ActiveDirectoryOperationException("There is no current connection to an Active Directory server.");
            }
            DirectoryEntry sourceEntry = (DirectoryEntry)principal.GetUnderlyingObject();
            DirectoryEntry destinationEntry = GetNewDirectoryEntry(container);
            sourceEntry.MoveTo(destinationEntry);
        }

        /// <summary>
        /// Returns the NetBIOS name of the Active Directory domain that is currently connected.
        /// If there is no current connection to an Active Directory server, an ActiveDirectoryOperationException will be thrown.
        /// </summary>
        public string NetBiosName
        {
            get
            {
                if (!IsConnected)
                {
                    throw new ActiveDirectoryOperationException("There is no current connection to an Active Directory server.");
                }
                DirectoryEntry rootDse = GetNewDirectoryEntry("RootDSE");

                string configurationNamingContext = rootDse.Properties["configurationNamingContext"][0].ToString();
                string rootDomainName = rootDse.Properties["ldapServiceName"][0].ToString().Split(':')[0];

                DirectoryEntry searchRoot = GetNewDirectoryEntry("cn=Partitions," + configurationNamingContext);

                DirectorySearcher searcher = new DirectorySearcher(searchRoot);
                searcher.SearchScope = SearchScope.OneLevel;
                searcher.PropertiesToLoad.Add("nETBIOSName");
                searcher.Filter = string.Format("(&(objectcategory=Crossref)(dnsRoot={0})(nETBIOSName=*))", rootDomainName);

                SearchResult result = searcher.FindOne();

                if (result != null)
                    return result.Properties["nETBIOSName"][0].ToString();
                else
                    return null;
            }
        }

        /// <summary>
        /// Returns the root DNS domain name of the Active Directory domain that is currently connected.
        /// If there is no current connection to an Active Directory server, an ActiveDirectoryOperationException will be thrown.
        /// </summary>
        public string DnsDomainName
        {
            get
            {
                if (!IsConnected)
                {
                    throw new ActiveDirectoryOperationException("There is no current connection to an Active Directory server.");
                }
                DirectoryEntry rootDse = GetNewDirectoryEntry("RootDSE");

                string rootDomainName = rootDse.Properties["ldapServiceName"][0].ToString().Split(':')[0];

                return rootDomainName;
            }
        }

        /// <summary>
        /// Creates the connections to the server and returns a Boolean value that specifies
        /// whether the specified username and password are valid.
        /// </summary>
        /// <param name="userName">The username that is validated on the server.</param>
        /// <param name="password">The password that is validated on the server.</param>
        /// <returns>true if the credentials are valid; otherwise false.</returns>
        public bool ValidateCredentials(string userName, string password)
        {
            return _PrincipalContext.ValidateCredentials(userName, password);
        }

        /// <summary>
        /// Creates the connections to the server and returns a Boolean value that specifies
        /// whether the specified username and password are valid.
        /// </summary>
        /// <param name="userName">The username that is validated on the server.</param>
        /// <param name="password">The password that is validated on the server.</param>
        /// <param name="principal">The ADUserPrincipal of the user that was validated. If validation fails, this object will be null.</param>
        /// <returns>true if the credentials are valid; otherwise false.</returns>
        public bool ValidateCredentials(string userName, string password, out ADUserPrincipal principal)
        {
            string resultText = String.Empty;
            return ValidateCredentials(userName, password, out principal, out resultText);
        }

        /// <summary>
        /// Creates the connections to the server and returns a Boolean value that specifies
        /// whether the specified username and password are valid.
        /// </summary>
        /// <param name="userName">The username that is validated on the server.</param>
        /// <param name="password">The password that is validated on the server.</param>
        /// <param name="principal">The ADUserPrincipal of the user that was validated. If validation fails, this object will be null.</param>
        /// <param name="resultText">A text string with the result of the validation.</param>
        /// <returns>true if the credentials are valid; otherwise false.</returns>
        public bool ValidateCredentials(string userName, string password, out ADUserPrincipal principal, out string resultText)
        {
            bool result = false;
            principal = null;
            resultText = string.Empty;

            //Always check the return result to verify the full validation of the user.

            try
            {
                principal = ADUserPrincipal.FindByIdentity(_PrincipalContext, IdentityType.SamAccountName, userName);
            }
            catch
            {
                result = false;
                principal = null;
                resultText = "Failed to query user!";
                return result;
            }

            if (principal == null)
            {
                result = false;
                resultText = "User does not exist!";
                return result;
            }

            try
            {
                result = _PrincipalContext.ValidateCredentials(userName, password);
            }
            catch
            {
                result = false;
                principal = null;
                resultText = "Failed to validate user credentials!";
                return result;
            }

            if (result == false)
            {
                resultText = "Invalid Password!";
                return result;
            }

            result = true;
            resultText = "User Validated!";
            return result;
        }

        /// <summary>
        /// Converts a SecureString to a String
        /// </summary>
        /// <param name="input">SecureString to convert</param>
        /// <returns>String containing the value of the SecureString input</returns>
        public static string ToInsecureString(SecureString input)
        {
            if (input == null)
                return null;

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(input);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        /// <summary>
        /// Converts a String to a SecureString
        /// </summary>
        /// <param name="input">String to convert</param>
        /// <returns>SecureString containing the value of the input String</returns>
        public static SecureString ToSecureString(string input)
        {
            if (input == null)
            {
                return null;
            }
            else
            {
                var output = new SecureString();
                input.ToList().ForEach(output.AppendChar);
                return output;
            }
        }
    }

}