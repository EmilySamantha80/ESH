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
    /// Encapsulates group accounts. Group accounts can be arbitrary collections of principal objects or accounts created for administrative purposes.
    /// </summary>
    [DirectoryObjectClass("group")]
    [DirectoryRdnPrefix("CN")]
    public class ADGroupPrincipal : GroupPrincipal
    {
        /// <summary>
        /// Initializes a new instance of the System.DirectoryServices.AccountManagement.GroupPrincipal class and assigns it to the specified context.
        /// </summary>
        /// <param name="context">The System.DirectoryServices.AccountManagement.PrincipalContext that specifies the server or domain against which operations are performed.</param>
        public ADGroupPrincipal(PrincipalContext context)
            : base(context) { }

        /// <summary>
        /// Initializes a new instance of the System.DirectoryServices.AccountManagement.GroupPrincipal class and assigns it to the specified context and SAM account name.
        /// </summary>
        /// <param name="context">The System.DirectoryServices.AccountManagement.PrincipalContext that specifies the server or domain against which operations are performed.</param>
        /// <param name="samAccountName">The SAM account name for this principal.</param>
        public ADGroupPrincipal(PrincipalContext context, string samAccountName)
            : base(context, samAccountName) { }

        private ADGroupPrincipalSearchFilter _AdvancedSearchFilter;
        public ADGroupPrincipalSearchFilter AdvancedSearchFilter
        {
            get
            {
                if (_AdvancedSearchFilter == null)
                {
                    _AdvancedSearchFilter = new ADGroupPrincipalSearchFilter(this);
                }
                return _AdvancedSearchFilter;
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
                            result = "";
                        else
                            result += ";";

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
        /// Returns the ADGroupPrincipal object that matches the specified identity value.
        /// </summary>
        /// <param name="principalContext">The System.DirectoryServices.AccountManagement.PrincipalContext that specifies the server or domain against which operations are performed.</param>
        /// <param name="identityValue">The identity of the principal.</param>
        /// <returns>An ADGroupPrincipal object that matches the specified identity value or null if no matches are found</returns>
        public static new ADGroupPrincipal FindByIdentity(PrincipalContext context, string identityValue)
        {
            return (ADGroupPrincipal)FindByIdentityWithType(context, typeof(ADGroupPrincipal), identityValue);
        }

        /// <summary>
        /// Returns the AGroupPrincipal object that matches the specified identity type, and value.
        /// </summary>
        /// <param name="principalContext">The System.DirectoryServices.AccountManagement.PrincipalContext that specifies the server or domain against which operations are performed.</param>
        /// <param name="identityType">An System.DirectoryServices.AccountManagement.IdentityType enumeration value that specifies the type of the identity value.</param>
        /// <param name="identityValue">The identity of the principal.</param>
        /// <returns>An ADGroupPrincipal object that matches the specified identity value or null if no matches are found</returns>
        public static new ADGroupPrincipal FindByIdentity(PrincipalContext context, IdentityType identityType, string identityValue)
        {
            return (ADGroupPrincipal)FindByIdentityWithType(context, typeof(ADGroupPrincipal), identityType, identityValue);
        }

        /// <summary>
        /// Returns the ADGroupPrincipal objects that match the specified attribute value and match type
        /// </summary>
        /// <param name="principalContext">The System.DirectoryServices.AccountManagement.PrincipalContext that specifies the server or domain against which operations are performed.</param>
        /// <param name="attribute">The attribute name to be queried.</param>
        /// <param name="value">The attribute value to match.</param>
        /// <param name="mt">A System.DirectoryServices.AccountManagement.MatchType object that specifies the type of comparison.</param>
        /// <returns>A List of type ADGroupPrincipal that matches the specified attribute value.</returns>
        public static List<ADGroupPrincipal> FindByAttribute(PrincipalContext context, string attribute, string value)
        {
            return FindByAttribute(context, attribute, value, MatchType.Equals);
        }

        /// <summary>
        /// Returns the ADGroupPrincipal objects that match the specified attribute value
        /// </summary>
        /// <param name="principalContext">The System.DirectoryServices.AccountManagement.PrincipalContext that specifies the server or domain against which operations are performed.</param>
        /// <param name="attribute">The attribute name to be queried.</param>
        /// <param name="value">The attribute value to match.</param>
        /// <returns>A List of type ADGroupPrincipal that matches the specified attribute value.</returns>
        public static List<ADGroupPrincipal> FindByAttribute(PrincipalContext context, string attribute, string value, MatchType mt)
        {
            ADGroupPrincipal filter = new ADGroupPrincipal(context);
            filter.AdvancedSearchFilter.FindByAttribute(attribute, value, typeof(string), mt);
            PrincipalSearcher ps = new PrincipalSearcher(filter);
            return ps.FindAll().Cast<ADGroupPrincipal>().ToList();
        }

        /// <summary>
        /// Returns the ADGroupPrincipal object that matches the specified attribute value and match type
        /// </summary>
        /// <param name="principalContext">The System.DirectoryServices.AccountManagement.PrincipalContext that specifies the server or domain against which operations are performed.</param>
        /// <param name="attribute">The attribute name to be queried.</param>
        /// <param name="value">The attribute value to match.</param>
        /// <param name="mt">A System.DirectoryServices.AccountManagement.MatchType object that specifies the type of comparison.</param>
        /// <returns>An ADGroupPrincipal object that matches the specified attribute value or null if no match is found.</returns>
        public static ADGroupPrincipal FindOneByAttribute(PrincipalContext context, string attribute, string value)
        {
            return FindOneByAttribute(context, attribute, value, MatchType.Equals);
        }

        /// <summary>
        /// Returns the ADGroupPrincipal object that matches the specified attribute value and match type
        /// </summary>
        /// <param name="principalContext">The System.DirectoryServices.AccountManagement.PrincipalContext that specifies the server or domain against which operations are performed.</param>
        /// <param name="attribute">The attribute name to be queried.</param>
        /// <param name="value">The attribute value to match.</param>
        /// <param name="mt">A System.DirectoryServices.AccountManagement.MatchType object that specifies the type of comparison.</param>
        /// <returns>An ADGroupPrincipal object that matches the specified attribute value or null if no match is found.</returns>
        public static ADGroupPrincipal FindOneByAttribute(PrincipalContext context, string attribute, string value, MatchType mt)
        {
            ADGroupPrincipal filter = new ADGroupPrincipal(context);
            filter.AdvancedSearchFilter.FindByAttribute(attribute, value, typeof(string), mt);
            PrincipalSearcher ps = new PrincipalSearcher(filter);
            return (ADGroupPrincipal)ps.FindOne();
        }

        /// <summary>
        /// Returns a List of ADGroupPrincipal objects that matches the values specified in the principal object
        /// </summary>
        /// <param name="principalContext">The System.DirectoryServices.AccountManagement.PrincipalContext that specifies the server or domain against which operations are performed.</param>
        /// <param name="ADUserPrincipal">The ADGroupPrincipal object to be matched from.</param>
        /// <returns>A List of type ADGroupPrincipal containing the objects that match the specified principal values.</returns>
        public static List<ADGroupPrincipal> FindByPrincipalSearcher(PrincipalContext principalContext, ADGroupPrincipal principal)
        {
            var results = new List<ADGroupPrincipal>();

            PrincipalSearcher ps = new PrincipalSearcher(principal);
            foreach (ADGroupPrincipal p in ps.FindAll())
            {
                results.Add(p);
            }
            return results;
        }

        /// <summary>
        /// Returns the ADGroupPrincipal objects that matches the values specified in the principal object
        /// </summary>
        /// <param name="principalContext">The System.DirectoryServices.AccountManagement.PrincipalContext that specifies the server or domain against which operations are performed.</param>
        /// <param name="ADUserPrincipal">The ADGroupPrincipal object to be matched from.</param>
        /// <returns>An ADGroupPrincipal objects that match the specified principal values or null if no match is found.</returns>
        public static ADUserPrincipal FindOneByPrincipalSearcher(PrincipalContext principalContext, ADGroupPrincipal principal)
        {
            PrincipalSearcher ps = new PrincipalSearcher(principal);
            return (ADUserPrincipal)ps.FindOne();
        }

        /// <summary>
        /// Adds a Principal to a group
        /// </summary>
        /// <param name="principal">The Principal to add to the group</param>
        /// <param name="targetGroup">The ADGroupPrincipal that the Principal will be added to</param>
        /// <param name="retryAttempts">Number of retry attempts</param>
        /// <param name="retryDelay">Number of seconds to wait between retries</param>
        /// <returns>The number of attempts taken to add the principal to the group, or -1 if unsuccessful</returns>
        public int AddPrincipalToGroup(Principal principal, int retryAttempts = 0, int retryDelay = 0)
        {
            for (int x = 1; x <= retryAttempts; x++)
            {
                try
                {
                    int i = this.Members.Where(g => g.SamAccountName == principal.SamAccountName).Count();
                    if (i == 0)
                    {
                        this.Members.Add(principal);
                        this.Save();
                    }
                    return x;
                }
                catch
                {
                    for (int x2 = 0; x2 < 10; x2++)
                    {
                        System.Threading.Thread.Sleep(100 * retryDelay);
                    }
                }
            }
            return -1;
        }
    }


    public class ADGroupPrincipalSearchFilter : AdvancedFilters
    {
        public ADGroupPrincipalSearchFilter(Principal p) : base(p) { }
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