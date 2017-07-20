// Title:  IP Address Tools
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
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ESH.Utility
{
    /// <summary>
    /// IP address tools.
    /// Retrieves details about an IP address or hostname.
    /// </summary>
	public class IPAddressTools
    {
        private string[] _InternalIPRegex;
        /// <summary>
        /// The InternalIPRegex parameter provided when the object was initialized
        /// </summary>
        public string[] InternalIPRegex
        {
            get
            {
                if (_InternalIPRegex == null)
                {
                    return new string[0];
                }
                else
                {
                    return _InternalIPRegex;
                }
            }
            set
            {
                if (_InternalIPRegex == null)
                {
                    _InternalIPRegex = new string[0];
                }
                else
                {
                    _InternalIPRegex = value;
                }
            }
        }

        private bool _IsLocal = false;
        /// <summary>
        /// Whether or not the host is localhost
        /// </summary>
        public bool IsLocal
        {
            get
            {
                return _IsLocal;
            }
        }

        private bool _IsLoopback = false;
        /// <summary>
        /// Whether or not the host is using the loopback address
        /// </summary>
        public bool IsLoopback
        {
            get
            {
                return _IsLoopback;
            }
        }

        private bool _IsValid = false;
        /// <summary>
        /// Whether or not the host/IP is valid
        /// </summary>
		public bool IsValid
        {
            get
            {
                return _IsValid;
            }
        }

        private string _InternalDomainSuffix = string.Empty;
        /// <summary>
        /// Internal domain suffix provided when the object was initialized 
        /// </summary>
		public string InternalDomainSuffix
        {
            get
            {
                return _InternalDomainSuffix;
            }
        }

        /// <summary>
        /// The client's IP address that was found, if any
        /// </summary>
		public IPAddress ClientIPAddress
        {
            get
            {
                if (_IPAddressObject == null)
                {
                    return IPAddress.None;
                }
                else
                {
                    return _IPAddressObject;
                }
            }
        }

        /// <summary>
        /// The family of the host, if any
        /// </summary>
		public AddressFamily ClientIPAddressFamily
        {
            get
            {
                if (_IPAddressObject == null)
                {
                    return IPAddress.None.AddressFamily;
                }
                else
                {
                    return _IPAddressObject.AddressFamily;
                }
            }
        }

        /// <summary>
        /// The host name of the client, if any
        /// </summary>
		public string HostName
        {
            get
            {
                if (_HostEntryObject == null)
                {
                    return string.Empty;
                }
                else
                {
                    if (string.IsNullOrEmpty(_HostEntryObject.HostName))
                    {
                        return string.Empty;
                    }
                    else
                    {
                        return _HostEntryObject.HostName;
                    }
                }
            }
        }

        /// <summary>
        /// Whether or not the host has an internal IP address
        /// </summary>
		public bool IsInternal
        {
            get
            {
                if (_IsLocal)
                {
                    return true;
                }
                else if (!string.IsNullOrWhiteSpace(_InternalDomainSuffix))
                {
                    if (_HostEntryObject != null && _HostEntryObject.HostName.EndsWith(InternalDomainSuffix, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (_IPAddressObject != null)
                {
                    foreach (string internalIP in _InternalIPRegex)
                    {
                        if (Regex.IsMatch(_IPAddressObject.ToString(), internalIP))
                        //if (_ipAddressObject.ToString().StartsWith(internalIP))
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
        }

        private IPAddress _IPAddressObject = null;
        /// <summary>
        /// The raw IPAddressObject for the client
        /// </summary>
        public IPAddress IPAddressObject
        {
            get
            {
                return _IPAddressObject;
            }
        }

        private IPHostEntry _HostEntryObject = null;
        /// <summary>
        /// The raw HostEntryObject for the client
        /// </summary>
		public IPHostEntry HostEntryObject
        {
            get
            {
                return _HostEntryObject;
            }
        }

        private IPHostEntry _LocalHostEntryObject = null;
        /// <summary>
        /// The raw LocalHostEntryObject for the client
        /// </summary>
		public IPHostEntry LocalHostEntryObject
        {
            get
            {
                return _LocalHostEntryObject;
            }
        }

        /// <summary>
        /// Initializes the ClientIP object
        /// </summary>
        /// <param name="clientIPAddress">The client's IP address</param>
        /// <param name="internalDomainSuffix">Optional domain suffix that determines if a host is internal or external</param>
        /// <param name="internalIPRegex">Optional Regex strings that will determine if the host is internal or external</param>
		public IPAddressTools(string clientIPAddress, string internalDomainSuffix = null, string[] internalIPRegex = null)
        {
            _InternalDomainSuffix = internalDomainSuffix;
            _InternalIPRegex = internalIPRegex;

            //Parse the provided IP Address
            try
            {
                _IPAddressObject = IPAddress.Parse(clientIPAddress);
            }
            catch
            {
                _IPAddressObject = IPAddress.None;
            }

            //Check if the IP is valid
            if (_IPAddressObject != IPAddress.None)
            {
                _IsValid = true;
            }
            else
            {
                _IsValid = false;
            }

            ////Try to look up the Host Entry IP in DNS. This will return the IPv4 address if it exists in DNS or IPAddress.None if it doesn't exist in DNS
            ////Uncomment if you only want IPv4, however the data may be incorrect since it only gets the first IP address the DNS server provides
            //IPAddress _ipAddressLookup = ClientIP.GetHostEntryIP(_ipAddressObject.ToString(), AddressFamily.InterNetwork);
            ////If the IP addresses was not found in DNS, just use the provided IP
            //if (_ipAddressLookup != IPAddress.None) {
            //	_ipAddressObject = _ipAddressLookup;
            //}

            //Check if the provided IP is the loopback address
            if (IPAddress.IsLoopback(_IPAddressObject) || _IPAddressObject.IsIPv6LinkLocal)
            {
                _IsLoopback = true;
            }
            else
            {
                _IsLoopback = false;
            }

            //Query the DNS server for all of the provided IP's host entries
            try
            {
                _HostEntryObject = Dns.GetHostEntry(_IPAddressObject);
            }
            catch
            {

            }

            //Query the DNS server for all of the local IP addresses
            try
            {
                _LocalHostEntryObject = Dns.GetHostEntry(IPAddress.Loopback);
            }
            catch
            {

            }

            //Iterate through each local IP and determine if the provided IP address is local 
            foreach (IPAddress ip in _LocalHostEntryObject.AddressList)
            {
                //Check if the IP address is on the server. Loopback and matching IP's are valid.
                if ((ip.ToString() == _IPAddressObject.ToString() || _IPAddressObject.IsIPv6LinkLocal || IPAddress.IsLoopback(_IPAddressObject)) && _IsLocal == false)
                {
                    _IsLocal = true;
                }
            }
        }

        /// <summary>
        /// Gets the IP address for a given host
        /// </summary>
        /// <param name="hostNameOrAddress">Host name/IP address to query</param>
        /// <returns>The IPv4 address for the host</returns>
		public static IPAddress GetHostEntryIP(string hostNameOrAddress)
        {
            return GetHostEntryIP(hostNameOrAddress, AddressFamily.InterNetwork);
        }

        /// <summary>
        /// Gets the IP address for a given host
        /// </summary>
        /// <param name="hostNameOrAddress">Host name/IP address to query</param>
        /// <param name="addressFamily">The family of the IP address</param>
        /// <returns>The IP address for the host</returns>
		public static IPAddress GetHostEntryIP(string hostNameOrAddress, AddressFamily addressFamily)
        {
            IPHostEntry hostEntry = new IPHostEntry();
            try
            {
                hostEntry = Dns.GetHostEntry(hostNameOrAddress);
            }
            catch
            {
                return IPAddress.None;
            }
            foreach (IPAddress ip in hostEntry.AddressList)
            {
                if (ip.AddressFamily == addressFamily)
                {
                    return ip;
                }
            }
            return IPAddress.None;
        }
    }
}
