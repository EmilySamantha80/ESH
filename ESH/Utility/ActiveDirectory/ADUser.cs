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
using System.Reflection;

/// <summary>
/// Full featured C# Active Directory client
/// </summary>
namespace ESH.Utility.ActiveDirectory
{
    /// <summary>
    /// Encapsulates principals that are user accounts.
    /// </summary>
    [DirectoryObjectClass("user")]
    [DirectoryRdnPrefix("CN")]
    public class ADUserPrincipal : UserPrincipal
    {
        /// <summary>
        /// Initializes a new instance of the ADUserPrincipal class by using the specified context, SAM account name, password, and enabled value
        /// </summary>
        /// <param name="context">The System.DirectoryService.AccountManagement.PrincipalContext that specifies the server or domain against which operations are performed.</param>
        public ADUserPrincipal(PrincipalContext context)
            : base(context) { }

        /// <summary>
        /// Initializes a new instance of the ADUserPrincipal class by using the specified context, SAM account name, password, and enabled value
        /// </summary>
        /// <param name="context">The System.DirectoryService.AccountManagement.PrincipalContext that specifies the server or domain against which operations are performed.</param>
        /// <param name="samAccountName">The SAM account name for this computer principal.</param>
        /// <param name="password">The password for this account.</param>
        /// <param name="enabled">A Boolean value that specifies whether the account is enabled.</param>
        public ADUserPrincipal(PrincipalContext context, string samAccountName, string password, bool enabled)
            : base(context, samAccountName, password, enabled) { }

        private ADUserPrincipalSearchFilter _AdvancedSearchFilter;
        new public ADUserPrincipalSearchFilter AdvancedSearchFilter
        {
            get
            {
                if (_AdvancedSearchFilter == null)
                {
                    _AdvancedSearchFilter = new ADUserPrincipalSearchFilter(this);
                }
                return _AdvancedSearchFilter;
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
                    //We are assuming that the time was stored in GMT (as it should be) and convert
                    //it to local time before getting the date part from it.
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
        /// Gets or sets the TCP/IP address for the phone. Used by telephony.
        /// </summary>
        /// <returns>A string containing the value of the attribute or null if the attribute does not exist.</returns>
        [DirectoryProperty("ipPhone")]
        public string IpPhone
        {
            get
            {
                if (ExtensionGet("ipPhone").Length == 0)
                {
                    return null;
                }
                return Convert.ToString(ExtensionGet("ipPhone")[0]);
            }
            set
            {
                ExtensionSet("ipPhone", value);
            }
        }

        /// <summary>
        /// Gets or sets the department
        /// </summary>
        /// <returns>A string containing the value of the attribute or null if the attribute does not exist.</returns>
        [DirectoryProperty("department")]
        public string Department
        {
            get
            {
                if (ExtensionGet("department").Length == 0)
                {
                    return null;
                }
                return Convert.ToString(ExtensionGet("department")[0]);
            }
            set
            {
                ExtensionSet("department", value);
            }
        }

        /// <summary>
        /// Gets or sets the job title
        /// </summary>
        /// <returns>A string containing the value of the attribute or null if the attribute does not exist.</returns>
        [DirectoryProperty("title")]
        public string JobTitle
        {
            get
            {
                if (ExtensionGet("title").Length == 0)
                {
                    return null;
                }
                return Convert.ToString(ExtensionGet("title")[0]);
            }
            set
            {
                ExtensionSet("title", value);
            }
        }

        /// <summary>
        /// Gets or sets the office
        /// </summary>
        /// <returns>A string containing the value of the attribute or null if the attribute does not exist.</returns>
        [DirectoryProperty("physicalDeliveryOfficeName")]
        public string Office
        {
            get
            {
                if (ExtensionGet("physicalDeliveryOfficeName").Length == 0)
                {
                    return null;
                }
                return Convert.ToString(ExtensionGet("physicalDeliveryOfficeName")[0]);
            }
            set
            {
                ExtensionSet("physicalDeliveryOfficeName", value);
            }
        }

        /// <summary>
        /// Gets or sets the notes
        /// </summary>
        /// <returns>A string containing the value of the attribute or null if the attribute does not exist.</returns>
        [DirectoryProperty("info")]
        public string Notes
        {
            get
            {
                if (ExtensionGet("info").Length == 0)
                {
                    return null;
                }
                return Convert.ToString(ExtensionGet("info")[0]);
            }
            set
            {
                ExtensionSet("info", value);
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
        /// Gets or sets the JPEG thumbnail photo for the account.
        /// The image cannot be larger than 100 Kb.
        /// </summary>
        /// <returns>A byte array containing the photo</returns>
        [DirectoryProperty("thumbnailPhoto")]
        public byte[] ThumbnailPhoto
        {
            get
            {
                return (byte[])ExtensionGet("thumbnailPhoto")[0];
            }

            set
            {
                ExtensionSet("thumbnailPhoto", value);
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
        /// </summary>
        /// <returns>A Nullable System.DateTime that specifies the date and time of the last logon for this account</returns>
        [DirectoryProperty("lastLogon")]
        public new DateTime? LastLogon
        {
            get
            {
                // https://www.codeproject.com/Articles/565593/How-to-get-the-REAL-lastlogon-datetime-from-Active
                if (ExtensionGet("lastLogon").Length == 0)
                {
                    return null;
                }
                var lastLogonDate = ExtensionGet("LastLogon")[0];
                // Use lastLogonDate.GetType() instead of typeof(IADsLargeInteger) in .NET Core, since the way IADsLargeInteger is declared is deprecated 
                //var lastLogonDateType = lastLogonDate.GetType();
                var lastLogonDateType = typeof(IADsLargeInteger);

                var highPart = (Int32)lastLogonDateType.InvokeMember("HighPart",
                    BindingFlags.GetProperty, null, lastLogonDate, null);
                var lowPart = (Int32)lastLogonDateType.InvokeMember("LowPart",
                    BindingFlags.GetProperty | BindingFlags.Public, null, lastLogonDate, null);

                var longDate = ((Int64)highPart << 32 | (UInt32)lowPart);

                return longDate > 0 ? (DateTime?)DateTime.FromFileTime(longDate) : null;
            }
        }

        /// <summary>
        /// Gets the Nullable System.DateTime that specifies the date and time of the last logon for this account.
        /// NOTE: This value is guaranteed to be synched across domain controllers at least every 14 days, which means this value is only accurate to within 14 days.
        /// To get the most accurate date/time use the LastLogon property
        /// </summary>
        /// <returns>A Nullable System.DateTime that specifies the date and time of the last logon for this account</returns>
        [DirectoryProperty("lastLogonTimestamp")]
        public DateTime? LastLogonTimestamp
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
        ///  Specifies the delivery address to which e-mail for this recipient should be sent.
        /// </summary>
        /// <returns>A string containing the address</returns>
        [DirectoryProperty("targetAddress")]
        public string TargetAddress
        {
            get
            {
                if (ExtensionGet("targetAddress").Length == 0)
                {
                    return null;
                }
                return Convert.ToString(ExtensionGet("targetAddress")[0]);
            }
            set
            {
                ExtensionSet("targetAddress", value);
            }
        }

        /// <summary>
        /// A proxy address is the address by which a Microsoft Exchange Server recipient object is recognized in a foreign mail system.
        /// Proxy addresses are required for all recipient objects, such as custom recipients and distribution lists.
        /// </summary>
        /// <returns>An array of strings containing the proxy addresses</returns>
        [DirectoryProperty("proxyAddresses")]
        public string[] ProxyAddresses
        {
            get
            {
                return (string[])GetAttribute("proxyAddresses");
            }
            set
            {
                ExtensionSet("proxyAddresses", value);
            }
        }

        /// <summary>
        /// Whether or not the object is displayed in the Global Address List
        /// </summary>
        /// <returns>True if the object is hidden from the Global Address List</returns>
        [DirectoryProperty("msExchHideFromAddressLists")]
        public bool HideFromAddressLists
        {
            get
            {
                if (ExtensionGet("msExchHideFromAddressLists").Length == 0)
                {
                    return false;
                }
                return Convert.ToBoolean(ExtensionGet("msExchHideFromAddressLists")[0]);
            }
            set
            {
                ExtensionSet("msExchHideFromAddressLists", value);
            }
        }

        /// <summary>
        /// Contains an other additioanl mail address.
        /// </summary>
        /// <returns>A string containing the additional address</returns>
        [DirectoryProperty("otherMailbox")]
        public string OtherMailbox
        {
            get
            {
                if (ExtensionGet("otherMailbox").Length == 0)
                {
                    return null;
                }
                return Convert.ToString(ExtensionGet("otherMailbox")[0]);
            }
            set
            {
                ExtensionSet("otherMailbox", value);
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
        /// Returns the ADUserPrincipal object that matches the specified identity value.
        /// </summary>
        /// <param name="principalContext">The System.DirectoryServices.AccountManagement.PrincipalContext that specifies the server or domain against which operations are performed.</param>
        /// <param name="identityValue">The identity of the principal.</param>
        /// <returns>An ADUserPrincipal object that matches the specified identity value or null if no matches are found</returns>
        public static new ADUserPrincipal FindByIdentity(PrincipalContext principalContext, string identityValue)
        {
            return (ADUserPrincipal)FindByIdentityWithType(principalContext, typeof(ADUserPrincipal), identityValue);
        }

        /// <summary>
        /// Returns the ADUserPrincipal object that matches the specified identity type, and value.
        /// </summary>
        /// <param name="principalContext">The System.DirectoryServices.AccountManagement.PrincipalContext that specifies the server or domain against which operations are performed.</param>
        /// <param name="identityType">An System.DirectoryServices.AccountManagement.IdentityType enumeration value that specifies the type of the identity value.</param>
        /// <param name="identityValue">The identity of the principal.</param>
        /// <returns>An ADUserPrincipal object that matches the specified identity value or null if no matches are found</returns>
        public static new ADUserPrincipal FindByIdentity(PrincipalContext principalContext, IdentityType identityType, string identityValue)
        {
            return (ADUserPrincipal)FindByIdentityWithType(principalContext, typeof(ADUserPrincipal), identityType, identityValue);
        }

        /// <summary>
        /// Returns the ADUserPrincipal objects that match the specified attribute value and match type
        /// </summary>
        /// <param name="principalContext">The System.DirectoryServices.AccountManagement.PrincipalContext that specifies the server or domain against which operations are performed.</param>
        /// <param name="attribute">The attribute name to be queried.</param>
        /// <param name="value">The attribute value to match.</param>
        /// <param name="mt">A System.DirectoryServices.AccountManagement.MatchType object that specifies the type of comparison.</param>
        /// <returns>A List of type ADUserPrincipal that matches the specified attribute value.</returns>
        public static List<ADUserPrincipal> FindByAttribute(PrincipalContext principalContext, string attribute, string value, MatchType mt)
        {
            ADUserPrincipal filter = new ADUserPrincipal(principalContext);
            filter.AdvancedSearchFilter.FindByAttribute(attribute, value, typeof(string), mt);
            PrincipalSearcher ps = new PrincipalSearcher(filter);
            return ps.FindAll().Cast<ADUserPrincipal>().ToList();
        }

        /// <summary>
        /// Returns the ADUserPrincipal objects that match the specified attribute value
        /// </summary>
        /// <param name="principalContext">The System.DirectoryServices.AccountManagement.PrincipalContext that specifies the server or domain against which operations are performed.</param>
        /// <param name="attribute">The attribute name to be queried.</param>
        /// <param name="value">The attribute value to match.</param>
        /// <returns>A List of type ADUserPrincipal that matches the specified attribute value.</returns>
        public static List<ADUserPrincipal> FindByAttribute(PrincipalContext principalContext, string attribute, string value)
        {
            return FindByAttribute(principalContext, attribute, value, MatchType.Equals);
        }

        /// <summary>
        /// Returns the ADUserPrincipal object that matches the specified attribute value and match type
        /// </summary>
        /// <param name="principalContext">The System.DirectoryServices.AccountManagement.PrincipalContext that specifies the server or domain against which operations are performed.</param>
        /// <param name="attribute">The attribute name to be queried.</param>
        /// <param name="value">The attribute value to match.</param>
        /// <param name="mt">A System.DirectoryServices.AccountManagement.MatchType object that specifies the type of comparison.</param>
        /// <returns>An ADUserPrincipal object that matches the specified attribute value or null if no match is found.</returns>
        public static ADUserPrincipal FindOneByAttribute(PrincipalContext principalContext, string attribute, string value, MatchType mt)
        {
            var filter = new ADUserPrincipal(principalContext);
            filter.AdvancedSearchFilter.FindByAttribute(attribute, value, typeof(string), mt);
            PrincipalSearcher ps = new PrincipalSearcher(filter);
            return (ADUserPrincipal)ps.FindOne();
        }

        /// <summary>
        /// Returns the ADUserPrincipal object that matches the specified attribute value and match type
        /// </summary>
        /// <param name="principalContext">The System.DirectoryServices.AccountManagement.PrincipalContext that specifies the server or domain against which operations are performed.</param>
        /// <param name="attribute">The attribute name to be queried.</param>
        /// <param name="value">The attribute value to match.</param>
        /// <param name="mt">A System.DirectoryServices.AccountManagement.MatchType object that specifies the type of comparison.</param>
        /// <returns>An ADUserPrincipal object that matches the specified attribute value or null if no match is found.</returns>
        public static ADUserPrincipal FindOneByAttribute(PrincipalContext principalContext, string attribute, string value)
        {
            return FindOneByAttribute(principalContext, attribute, value, MatchType.Equals);
        }

        /// <summary>
        /// Returns a List of ADUserPrincipal objects that matches the values specified in the principal object
        /// </summary>
        /// <param name="principalContext">The System.DirectoryServices.AccountManagement.PrincipalContext that specifies the server or domain against which operations are performed.</param>
        /// <param name="ADUserPrincipal">The ADUserPrincipal object to be matched from.</param>
        /// <returns>A List of type ADUserPrincipal containing the objects that match the specified principal values.</returns>
        public static List<ADUserPrincipal> FindByPrincipalSearcher(PrincipalContext principalContext, ADUserPrincipal principal)
        {
            var results = new List<ADUserPrincipal>();

            PrincipalSearcher ps = new PrincipalSearcher(principal);
            foreach (ADUserPrincipal p in ps.FindAll())
            {
                results.Add(p);
            }
            return results;
        }

        /// <summary>
        /// Returns the ADUserPrincipal objects that matches the values specified in the principal object
        /// </summary>
        /// <param name="principalContext">The System.DirectoryServices.AccountManagement.PrincipalContext that specifies the server or domain against which operations are performed.</param>
        /// <param name="ADUserPrincipal">The ADUserPrincipal object to be matched from.</param>
        /// <returns>An ADUserPrincipal objects that match the specified principal values or null if no match is found.</returns>
        public static ADUserPrincipal FindOneByPrincipalSearcher(PrincipalContext principalContext, ADUserPrincipal principal)
        {
            PrincipalSearcher ps = new PrincipalSearcher(principal);
            return (ADUserPrincipal)ps.FindOne();
        }
    }

    public class ADUserPrincipalSearchFilter : AdvancedFilters
    {
        public ADUserPrincipalSearchFilter(Principal p) : base(p) { }
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