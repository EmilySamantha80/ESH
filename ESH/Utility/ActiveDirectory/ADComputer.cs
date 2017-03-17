// Title:  Full featured Active Directory client in C#
// Author: Emily Heiner
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
using System.DirectoryServices.AccountManagement;
using System.Linq;

/// <summary>
/// Full featured C# Active Directory client
/// </summary>
namespace ESH.Utility.ActiveDirectory
{
    /// <summary>
    /// Encapsulates principals that are computer accounts
    /// </summary>
    [DirectoryObjectClass("computer")]
    [DirectoryRdnPrefix("CN")]
    public class ADComputerPrincipal : ComputerPrincipal
    {
        /// <summary>
        /// Initializes a new instance of the ADComputerPrincipal class by using the specified context
        /// </summary>
        /// <param name="context">The System.DirectoryService.AccountManagement.PrincipalContext that specifies the server or domain against which operations are performed.</param>
        public ADComputerPrincipal(PrincipalContext context)
            : base(context) { }

        /// <summary>
        /// Initializes a new instance of the ADComputerPrincipal class by using the specified context, SAM account name, password, and enabled value
        /// </summary>
        /// <param name="context">The System.DirectoryService.AccountManagement.PrincipalContext that specifies the server or domain against which operations are performed.</param>
        /// <param name="samAccountName">The SAM account name for this computer principal.</param>
        /// <param name="password">The password for this account.</param>
        /// <param name="enabled">A Boolean value that specifies whether the account is enabled.</param>
        public ADComputerPrincipal(PrincipalContext context, string samAccountName, string password, bool enabled)
            : base(context, samAccountName, password, enabled) { }

        private ADComputerPrincipalSearchFilter advancedSearchFilter;
        new public ADComputerPrincipalSearchFilter AdvancedSearchFilter
        {
            get
            {
                if (advancedSearchFilter == null)
                {
                    advancedSearchFilter = new ADComputerPrincipalSearchFilter(this);
                }
                return advancedSearchFilter;
            }
        }

        /// <summary>
        /// Check if the account expires
        /// </summary>
        /// <returns>Returns true if the account expires</returns>
        public bool AccountExpires
        {
            get
            {
                if (AccountExpirationDate.HasValue == false)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Check if the user account is expired
        /// </summary>
        /// <returns>Returns true if expired</returns>
        public bool IsExpired
        {
            get
            {
                //Copy the account expiration date into a nullable DateTime variable.
                DateTime? accountExpiration = this.AccountExpirationDate;

                //Check for null first. Note that the AccountExpirationDate property is null when
                //it finds 0x0 or 0x7FFFFFFFFFFFFFFF in Active Directory. These values mean that
                //the account never expires.
                if (accountExpiration.HasValue == false)
                {
                    //Expiration date is not set and account never expires.
                    return false;
                }
                else
                {
                    //Compare the value the same way AD Users and Computers does (chop off the time).
                    //Example: Expiration of "08/08/2010 2:00:00 AM" will be represented as:
                    //         "End Of: 08/07/2010", which is the same as "08/08/2010 12:00:00 AM"
                    if (accountExpiration.Value.Date < DateTime.Now)
                    {
                        //Expiration date is set and account has expired.
                        return true;
                    }
                    else
                    {
                        //Expiration date is set but account has not expired yet.
                        return false;
                    }
                }

            }
        }

        /// <summary>
        /// Gets or Sets the allow dial-in property
        /// </summary>
        /// <returns>Returns true if the account is set to allow dial-in</returns>
        [DirectoryProperty("msNPAllowDialin")]
        public bool AllowDialIn
        {
            get
            {
                if (ExtensionGet("msNPAllowDialin").Length == 0)
                {
                    return false;
                }
                return Convert.ToBoolean(ExtensionGet("msNPAllowDialin")[0]);
            }
            set
            {
                ExtensionSet("msNPAllowDialin", value);
                if (value)
                    ExtensionSet("userParameters", "m:                    d	                        ");
                else
                    ExtensionSet("userParameters", null);
            }
        }

        /// <summary>
        /// Gets the date and time the object was created
        /// </summary>
        /// <returns>Returns a nullable DateTime containing the date and time the object was created</returns>
        [DirectoryProperty("whenCreated")]
        public DateTime? WhenCreated
        {
            get
            {
                if (ExtensionGet("whenCreated").Length == 0)
                {
                    return null;
                }
                return Convert.ToDateTime(ExtensionGet("whenCreated")[0]).ToLocalTime();
            }
        }

        /// <summary>
        /// Gets the date and time the object was modified
        /// </summary>
        /// <returns>Returns a nullable DateTime containing the date and time the object was modified</returns>
        [DirectoryProperty("whenChanged")]
        public DateTime? WhenChanged
        {
            get
            {
                if (ExtensionGet("whenChanged").Length == 0)
                {
                    return null;
                }
                return Convert.ToDateTime(ExtensionGet("whenChanged")[0]).ToLocalTime();
            }
        }

        /// <summary>
        /// Gets or sets a Nullable System.DateTime that specifies the date and time that the account expires
        /// </summary>
        /// <returns>a System.DateTime that specifies the date and time that the account expires, or null if the account never expires</returns>
        [DirectoryProperty("accountExpires")]
        public new DateTime? AccountExpirationDate
        {
            get
            {
                if (!base.AccountExpirationDate.HasValue)
                {
                    return null;
                }
                return base.AccountExpirationDate.Value.ToLocalTime();
            }
            set
            {
                if (value.HasValue)
                {
                    base.AccountExpirationDate = value.Value.ToUniversalTime();
                }
                else
                {
                    base.AccountExpirationDate = value;
                }
            }
        }

        /// <summary>
        /// Gets the Nullable System.DateTime that specifies the date and time that the account was locked out
        /// </summary>
        /// <returns>A System.DateTime that specifies the date and time that the account was locked out, or null if no lockout time is set on the account</returns>
        [DirectoryProperty("lockoutTime")]
        public new DateTime? AccountLockoutTime
        {
            get
            {
                if (!base.AccountLockoutTime.HasValue)
                {
                    return null;
                }
                return base.AccountLockoutTime.Value.ToLocalTime();
            }
        }

        /// <summary>
        /// Gets the Nullable System.DateTime that specifies the date and time of the last incorrect password attempt on this account
        /// </summary>
        /// <returns>A Nullable System.DateTime that specifies the date and time of the last incorrect password attempt on this account, or null if no incorrect password tries are recorded</returns>
        [DirectoryProperty("badPasswordTime")]
        public new DateTime? LastBadPasswordAttempt
        {
            get
            {
                if (!base.LastBadPasswordAttempt.HasValue)
                {
                    return null;
                }
                return base.LastBadPasswordAttempt.Value.ToLocalTime();
            }
        }

        /// <summary>
        /// Gets the Nullable System.DateTime that specifies the date and time of the last logon for this account.
        /// NOTE: This value is guaranteed to be synched across domain controllers at least every 14 days, which means this value is only accurate to within 14 days.
        /// </summary>
        /// <returns>A Nullable System.DateTime that specifies the date and time of the last logon for this account</returns>
        [DirectoryProperty("lastLogon")]
        public new DateTime? LastLogon
        {
            get
            {
                if (!base.LastLogon.HasValue)
                {
                    return null;
                }
                return base.LastLogon.Value.ToLocalTime();
            }
        }

        /// <summary>
        /// Gets the Nullable System.DateTime that specifies the last date and time that the password was set for this account.
        /// </summary>
        /// <returns>A Nullable System.DateTime that specifies the last date and time that the password was set for this account</returns>
        [DirectoryProperty("pwdLastSet")]
        public new DateTime? LastPasswordSet
        {
            get
            {
                if (!base.LastPasswordSet.HasValue)
                {
                    return null;
                }
                return base.LastPasswordSet.Value.ToLocalTime();
            }
        }

        /// <summary>
        /// The operating system name string
        /// </summary>
        /// <returns>A string containing the operating system version, or null if the attribute does not exist</returns>

        [DirectoryProperty("operatingSystem")]
        public string OperatingSystem
        {
            get
            {
                if (ExtensionGet("operatingSystem").Length == 0)
                {
                    return null;
                }
                return Convert.ToString(ExtensionGet("operatingSystem")[0]);
            }
            set
            {
                ExtensionSet("operatingSystem", value);
            }
        }

        /// <summary>
        /// The operating system service pack string
        /// </summary>
        /// <returns>A string containing the operating system service pack, or null if the attribute does not exist</returns>
        [DirectoryProperty("operatingSystemServicePack")]
        public string OperatingSystemServicePack
        {
            get
            {
                if (ExtensionGet("operatingSystemServicePack").Length == 0)
                {
                    return null;
                }
                return Convert.ToString(ExtensionGet("operatingSystemServicePack")[0]);
            }
            set
            {
                ExtensionSet("operatingSystemServicePack", value);
            }
        }

        /// <summary>
        /// The operating system version string
        /// </summary>
        /// <returns>A string containing the operating system version, or null if the attribute does not exist</returns>
        [DirectoryProperty("operatingSystemVersion")]
        public string OperatingSystemVersion
        {
            get
            {
                if (ExtensionGet("operatingSystemVersion").Length == 0)
                {
                    return null;
                }
                return Convert.ToString(ExtensionGet("operatingSystemVersion")[0]);
            }
            set
            {
                ExtensionSet("operatingSystemVersion", value);
            }
        }

        /// <summary>
        /// The name of the computer as registered in DNS.
        /// </summary>
        /// <returns>A string containing the dns name of the computer, or null if the attribute does not exist</returns>
        [DirectoryProperty("dNSHostName")]
        public string DnsHostName
        {
            get
            {
                if (ExtensionGet("dNSHostName").Length == 0)
                {
                    return null;
                }
                return Convert.ToString(ExtensionGet("dNSHostName")[0]);
            }
            set
            {
                ExtensionSet("dNSHostName", value);
            }
        }

        /// <summary>
        /// Extension attribute for Microsoft Exchange.
        /// This attribute can be used for storing arbitrary data about the object.
        /// </summary>
        /// <returns>A string containing the value of the attribute or null if the attribute does not exist.</returns>
        [DirectoryProperty("extensionAttribute1")]
        public string ExtensionAttribute1
        {
            get
            {
                if (ExtensionGet("extensionAttribute1").Length == 0)
                {
                    return null;
                }
                return Convert.ToString(ExtensionGet("extensionAttribute1")[0]);
            }
            set
            {
                ExtensionSet("extensionAttribute1", value);
            }
        }

        /// <summary>
        /// Extension attribute for Microsoft Exchange.
        /// This attribute can be used for storing arbitrary data about the object.
        /// </summary>
        /// <returns>A string containing the value of the attribute or null if the attribute does not exist.</returns>
        [DirectoryProperty("extensionAttribute2")]
        public string ExtensionAttribute2
        {
            get
            {
                if (ExtensionGet("extensionAttribute2").Length == 0)
                {
                    return null;
                }
                return Convert.ToString(ExtensionGet("extensionAttribute2")[0]);
            }
            set
            {
                ExtensionSet("extensionAttribute2", value);
            }
        }

        /// <summary>
        /// Extension attribute for Microsoft Exchange.
        /// This attribute can be used for storing arbitrary data about the object.
        /// </summary>
        /// <returns>A string containing the value of the attribute or null if the attribute does not exist.</returns>
        [DirectoryProperty("extensionAttribute3")]
        public string ExtensionAttribute3
        {
            get
            {
                if (ExtensionGet("extensionAttribute3").Length == 0)
                {
                    return null;
                }
                return Convert.ToString(ExtensionGet("extensionAttribute3")[0]);
            }
            set
            {
                ExtensionSet("extensionAttribute3", value);
            }
        }

        /// <summary>
        /// Extension attribute for Microsoft Exchange.
        /// This attribute can be used for storing arbitrary data about the object.
        /// </summary>
        /// <returns>A string containing the value of the attribute or null if the attribute does not exist.</returns>
        [DirectoryProperty("extensionAttribute4")]
        public string ExtensionAttribute4
        {
            get
            {
                if (ExtensionGet("extensionAttribute4").Length == 0)
                {
                    return null;
                }
                return Convert.ToString(ExtensionGet("extensionAttribute4")[0]);
            }
            set
            {
                ExtensionSet("extensionAttribute4", value);
            }
        }

        /// <summary>
        /// Extension attribute for Microsoft Exchange.
        /// This attribute can be used for storing arbitrary data about the object.
        /// </summary>
        /// <returns>A string containing the value of the attribute or null if the attribute does not exist.</returns>
        [DirectoryProperty("extensionAttribute5")]
        public string ExtensionAttribute5
        {
            get
            {
                if (ExtensionGet("extensionAttribute5").Length == 0)
                {
                    return null;
                }
                return Convert.ToString(ExtensionGet("extensionAttribute5")[0]);
            }
            set
            {
                ExtensionSet("extensionAttribute5", value);
            }
        }

        /// <summary>
        /// Extension attribute for Microsoft Exchange.
        /// This attribute can be used for storing arbitrary data about the object.
        /// </summary>
        /// <returns>A string containing the value of the attribute or null if the attribute does not exist.</returns>
        [DirectoryProperty("extensionAttribute6")]
        public string ExtensionAttribute6
        {
            get
            {
                if (ExtensionGet("extensionAttribute6").Length == 0)
                {
                    return null;
                }
                return Convert.ToString(ExtensionGet("extensionAttribute6")[0]);
            }
            set
            {
                ExtensionSet("extensionAttribute6", value);
            }
        }

        /// <summary>
        /// Extension attribute for Microsoft Exchange.
        /// This attribute can be used for storing arbitrary data about the object.
        /// </summary>
        /// <returns>A string containing the value of the attribute or null if the attribute does not exist.</returns>
        [DirectoryProperty("extensionAttribute7")]
        public string ExtensionAttribute7
        {
            get
            {
                if (ExtensionGet("extensionAttribute7").Length == 0)
                {
                    return null;
                }
                return Convert.ToString(ExtensionGet("extensionAttribute7")[0]);
            }
            set
            {
                ExtensionSet("extensionAttribute7", value);
            }
        }

        /// <summary>
        /// Extension attribute for Microsoft Exchange.
        /// This attribute can be used for storing arbitrary data about the object.
        /// </summary>
        /// <returns>A string containing the value of the attribute or null if the attribute does not exist.</returns>
        [DirectoryProperty("extensionAttribute8")]
        public string ExtensionAttribute8
        {
            get
            {
                if (ExtensionGet("extensionAttribute8").Length == 0)
                {
                    return null;
                }
                return Convert.ToString(ExtensionGet("extensionAttribute8")[0]);
            }
            set
            {
                ExtensionSet("extensionAttribute8", value);
            }
        }

        /// <summary>
        /// Extension attribute for Microsoft Exchange.
        /// This attribute can be used for storing arbitrary data about the object.
        /// </summary>
        /// <returns>A string containing the value of the attribute or null if the attribute does not exist.</returns>
        [DirectoryProperty("extensionAttribute9")]
        public string ExtensionAttribute9
        {
            get
            {
                if (ExtensionGet("extensionAttribute9").Length == 0)
                {
                    return null;
                }
                return Convert.ToString(ExtensionGet("extensionAttribute9")[0]);
            }
            set
            {
                ExtensionSet("extensionAttribute9", value);
            }
        }

        /// <summary>
        /// Extension attribute for Microsoft Exchange.
        /// This attribute can be used for storing arbitrary data about the object.
        /// </summary>
        /// <returns>A string containing the value of the attribute or null if the attribute does not exist.</returns>
        [DirectoryProperty("extensionAttribute10")]
        public string ExtensionAttribute10
        {
            get
            {
                if (ExtensionGet("extensionAttribute10").Length == 0)
                {
                    return null;
                }
                return Convert.ToString(ExtensionGet("extensionAttribute10")[0]);
            }
            set
            {
                ExtensionSet("extensionAttribute10", value);
            }
        }

        /// <summary>
        /// Extension attribute for Microsoft Exchange.
        /// This attribute can be used for storing arbitrary data about the object.
        /// </summary>
        /// <returns>A string containing the value of the attribute or null if the attribute does not exist.</returns>
        [DirectoryProperty("extensionAttribute11")]
        public string ExtensionAttribute11
        {
            get
            {
                if (ExtensionGet("extensionAttribute11").Length == 0)
                {
                    return null;
                }
                return Convert.ToString(ExtensionGet("extensionAttribute11")[0]);
            }
            set
            {
                ExtensionSet("extensionAttribute11", value);
            }
        }

        /// <summary>
        /// Extension attribute for Microsoft Exchange.
        /// This attribute can be used for storing arbitrary data about the object.
        /// </summary>
        /// <returns>A string containing the value of the attribute or null if the attribute does not exist.</returns>
        [DirectoryProperty("extensionAttribute12")]
        public string ExtensionAttribute12
        {
            get
            {
                if (ExtensionGet("extensionAttribute12").Length == 0)
                {
                    return null;
                }
                return Convert.ToString(ExtensionGet("extensionAttribute12")[0]);
            }
            set
            {
                ExtensionSet("extensionAttribute12", value);
            }
        }

        /// <summary>
        /// Extension attribute for Microsoft Exchange.
        /// This attribute can be used for storing arbitrary data about the object.
        /// </summary>
        /// <returns>A string containing the value of the attribute or null if the attribute does not exist.</returns>
        [DirectoryProperty("extensionAttribute13")]
        public string ExtensionAttribute13
        {
            get
            {
                if (ExtensionGet("extensionAttribute13").Length == 0)
                {
                    return null;
                }
                return Convert.ToString(ExtensionGet("extensionAttribute13")[0]);
            }
            set
            {
                ExtensionSet("extensionAttribute13", value);
            }
        }

        /// <summary>
        /// Extension attribute for Microsoft Exchange.
        /// This attribute can be used for storing arbitrary data about the object.
        /// </summary>
        /// <returns>A string containing the value of the attribute or null if the attribute does not exist.</returns>
        [DirectoryProperty("extensionAttribute14")]
        public string ExtensionAttribute14
        {
            get
            {
                if (ExtensionGet("extensionAttribute14").Length == 0)
                {
                    return null;
                }
                return Convert.ToString(ExtensionGet("extensionAttribute14")[0]);
            }
            set
            {
                ExtensionSet("extensionAttribute14", value);
            }
        }

        /// <summary>
        /// Extension attribute for Microsoft Exchange.
        /// This attribute can be used for storing arbitrary data about the object.
        /// </summary>
        /// <returns>A string containing the value of the attribute or null if the attribute does not exist.</returns>
        [DirectoryProperty("extensionAttribute15")]
        public string ExtensionAttribute15
        {
            get
            {
                if (ExtensionGet("extensionAttribute15").Length == 0)
                {
                    return null;
                }
                return Convert.ToString(ExtensionGet("extensionAttribute15")[0]);
            }
            set
            {
                ExtensionSet("extensionAttribute15", value);
            }
        }

        /// <summary>
        /// Sets the value of the specified attribute.
        /// For multi-valued attributes, pass an array to the parameter 'value'
        /// </summary>
        /// <param name="attribute">The name of the attribute</param>
        /// <param name="value">The value to set</param>
        public void SetAttribute(string attribute, object value)
        {
            ExtensionSet(attribute, value);
        }

        /// <summary>
        /// Returns the value of the specified attribute.
        /// If more than one value is in the attribute, the values will be separated by a semicolon
        /// </summary>
        /// <param name="attribute">The name of the attribute</param>
        /// <returns>The attribute value</returns>
        public object[] GetAttribute(string attribute)
        {
            return ExtensionGet(attribute);
        }

        /// <summary>
        /// Returns the value of the specified attribute.
        /// If more than one value is in the attribute, the values will be separated by a semicolon
        /// </summary>
        /// <param name="attribute">The name of the attribute to get</param>
        /// <returns>The attribute value</returns>
        public string GetAttributeString(string attribute)
        {
            object[] value = ExtensionGet(attribute);
            if (value.Length == 0)
            {
                return null;
            }
            string result = String.Empty;
            for (int i = 0; i < value.Length; i++)
            {
                result += Convert.ToString(ExtensionGet(attribute)[i]) + ";";
            }
            return result.TrimEnd(';');
        }

        /// <summary>
        /// Returns all of the Active Directory attributes for the specified user.
        /// Multi-valued attributes will be separated by a semicolon.
        /// </summary>
        /// <returns>Dictionary object containing the name/value pairs for the user properties</returns>
        public IDictionary<string, string> GetAttributes()
        {
            var propertyList = new Dictionary<string, string>();

            DirectoryEntry dirEntry = this.GetUnderlyingObject() as DirectoryEntry;
            PropertyCollection properties = dirEntry.Properties;

            foreach (string propertyName in properties.PropertyNames)
            {
                string result = String.Empty;
                PropertyValueCollection property = properties[propertyName];

                if (property != null && property.Count > 0)
                {
                    foreach (object value in property)
                    {
                        if (result == String.Empty)
                        {
                            result = "";
                        }
                        else
                        {
                            result += ";";
                        }
                        result += value.ToString();
                    }
                }
                else
                {
                    result = String.Empty;
                }
                propertyList.Add(propertyName, result);
            }

            dirEntry.Dispose();
            return propertyList;
        }

        /// <summary>
        /// Returns the ADComputerPrincipal object that matches the specified identity value.
        /// </summary>
        /// <param name="principalContext">The System.DirectoryServices.AccountManagement.PrincipalContext that specifies the server or domain against which operations are performed.</param>
        /// <param name="identityValue">The identity of the principal.</param>
        /// <returns>An ADComputerPrincipal object that matches the specified identity value or null if no matches are found</returns>
        public static new ADComputerPrincipal FindByIdentity(PrincipalContext principalContext, string identityValue)
        {
            return (ADComputerPrincipal)FindByIdentityWithType(principalContext, typeof(ADComputerPrincipal), identityValue);
        }

        /// <summary>
        /// Returns the ADComputerPrincipal object that matches the specified identity type, and value.
        /// </summary>
        /// <param name="principalContext">The System.DirectoryServices.AccountManagement.PrincipalContext that specifies the server or domain against which operations are performed.</param>
        /// <param name="identityType">An System.DirectoryServices.AccountManagement.IdentityType enumeration value that specifies the type of the identity value.</param>
        /// <param name="identityValue">The identity of the principal.</param>
        /// <returns>An ADComputerPrincipal object that matches the specified identity value or null if no matches are found</returns>
        public static new ADComputerPrincipal FindByIdentity(PrincipalContext principalContext, IdentityType identityType, string identityValue)
        {
            return (ADComputerPrincipal)FindByIdentityWithType(principalContext, typeof(ADComputerPrincipal), identityType, identityValue);
        }

        /// <summary>
        /// Returns the ADComputerPrincipal objects that match the specified attribute value and match type
        /// </summary>
        /// <param name="principalContext">The System.DirectoryServices.AccountManagement.PrincipalContext that specifies the server or domain against which operations are performed.</param>
        /// <param name="attribute">The attribute name to be queried.</param>
        /// <param name="value">The attribute value to match.</param>
        /// <param name="mt">A System.DirectoryServices.AccountManagement.MatchType object that specifies the type of comparison.</param>
        /// <returns>A List of type ADComputerPrincipal that matches the specified attribute value.</returns>
        public static List<ADComputerPrincipal> FindByAttribute(PrincipalContext principalContext, string attribute, string value, MatchType mt)
        {
            ADComputerPrincipal filter = new ADComputerPrincipal(principalContext);
            filter.AdvancedSearchFilter.FindByAttribute(attribute, value, typeof(string), mt);
            PrincipalSearcher ps = new PrincipalSearcher(filter);
            return ps.FindAll().Cast<ADComputerPrincipal>().ToList();
        }

        /// <summary>
        /// Returns the ADComputerPrincipal objects that match the specified attribute value
        /// </summary>
        /// <param name="principalContext">The System.DirectoryServices.AccountManagement.PrincipalContext that specifies the server or domain against which operations are performed.</param>
        /// <param name="attribute">The attribute name to be queried.</param>
        /// <param name="value">The attribute value to match.</param>
        /// <returns>A List of type ADComputerPrincipal that matches the specified attribute value.</returns>
        public static List<ADComputerPrincipal> FindByAttribute(PrincipalContext principalContext, string attribute, string value)
        {
            return FindByAttribute(principalContext, attribute, value, MatchType.Equals);
        }

        /// <summary>
        /// Returns the ADComputerPrincipal object that matches the specified attribute value and match type
        /// </summary>
        /// <param name="principalContext">The System.DirectoryServices.AccountManagement.PrincipalContext that specifies the server or domain against which operations are performed.</param>
        /// <param name="attribute">The attribute name to be queried.</param>
        /// <param name="value">The attribute value to match.</param>
        /// <param name="mt">A System.DirectoryServices.AccountManagement.MatchType object that specifies the type of comparison.</param>
        /// <returns>An ADComputerPrincipal object that matches the specified attribute value or null if no match is found.</returns>
        public static ADComputerPrincipal FindOneByAttribute(PrincipalContext principalContext, string attribute, string value, MatchType mt)
        {
            var filter = new ADComputerPrincipal(principalContext);
            filter.AdvancedSearchFilter.FindByAttribute(attribute, value, typeof(string), mt);
            PrincipalSearcher ps = new PrincipalSearcher(filter);
            return (ADComputerPrincipal)ps.FindOne();
        }

        /// <summary>
        /// Returns the ADComputerPrincipal object that matches the specified attribute value and match type
        /// </summary>
        /// <param name="principalContext">The System.DirectoryServices.AccountManagement.PrincipalContext that specifies the server or domain against which operations are performed.</param>
        /// <param name="attribute">The attribute name to be queried.</param>
        /// <param name="value">The attribute value to match.</param>
        /// <param name="mt">A System.DirectoryServices.AccountManagement.MatchType object that specifies the type of comparison.</param>
        /// <returns>An ADComputerPrincipal object that matches the specified attribute value or null if no match is found.</returns>
        public static ADComputerPrincipal FindOneByAttribute(PrincipalContext principalContext, string attribute, string value)
        {
            return FindOneByAttribute(principalContext, attribute, value, MatchType.Equals);
        }

        /// <summary>
        /// Returns a List of ADComputerPrincipal objects that matches the values specified in the principal object
        /// </summary>
        /// <param name="principalContext">The System.DirectoryServices.AccountManagement.PrincipalContext that specifies the server or domain against which operations are performed.</param>
        /// <param name="ADUserPrincipal">The ADComputerPrincipal object to be matched from.</param>
        /// <returns>A List of type ADComputerPrincipal containing the objects that match the specified principal values.</returns>
        public static List<ADComputerPrincipal> FindByPrincipalSearcher(PrincipalContext principalContext, ADComputerPrincipal principal)
        {
            var results = new List<ADComputerPrincipal>();

            PrincipalSearcher ps = new PrincipalSearcher(principal);
            foreach (ADComputerPrincipal p in ps.FindAll())
            {
                results.Add(p);
            }
            return results;
        }

        /// <summary>
        /// Returns the ADComputerPrincipal objects that matches the values specified in the principal object
        /// </summary>
        /// <param name="principalContext">The System.DirectoryServices.AccountManagement.PrincipalContext that specifies the server or domain against which operations are performed.</param>
        /// <param name="ADUserPrincipal">The ADComputerPrincipal object to be matched from.</param>
        /// <returns>An ADComputerPrincipal objects that match the specified principal values or null if no match is found.</returns>
        public static ADComputerPrincipal FindOneByPrincipalSearcher(PrincipalContext principalContext, ADComputerPrincipal principal)
        {
            PrincipalSearcher ps = new PrincipalSearcher(principal);
            return (ADComputerPrincipal)ps.FindOne();
        }

    }

    public class ADComputerPrincipalSearchFilter : AdvancedFilters
    {
        public ADComputerPrincipalSearchFilter(Principal p) : base(p) { }
        public void ExtensionAttribute10(string value, MatchType mt)
        {
            this.AdvancedFilterSet("extensionAttribute10", value, typeof(string), mt);
        }
        public void FindByAttribute(string attribute, string value, Type objectType, MatchType mt)
        {
            this.AdvancedFilterSet(attribute, value, objectType, mt);
        }
    }
}