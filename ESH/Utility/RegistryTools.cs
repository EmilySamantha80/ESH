// Title:  Registry tools library
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

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESH.Utility
{
    /// <summary>
    /// Registry reading/writing library
    /// </summary>
    public class RegistryTools
    {
        /*
        Usage examples:
        RegistryKey rootKey = Registry.CurrentUser;
        string keyPath = @"Software\MyProgram";
        string valueName = @"MyKey";
        string valueData = @"Test";

        RegistryTools.KeyExists(rootKey, keyPath);
        RegistryTools.ValueExists(rootKey, keyPath, valueName);
        RegistryTools.CreateSubKey(rootKey, keyPath);
        RegistryTools.ReadRegistryValue(rootKey, keyPath, valueName);
        RegistryTools.WriteRegistryValue(rootKey, keyPath, valueName, valueData, RegistryValueKind.String);
        RegistryTools.DeleteValue(rootKey, keyPath, valueName);
        RegistryTools.DeleteSubKey(rootKey, keyPath);
        RegistryTools.DeleteSubKeyTree(rootKey, keyPath);	
        */

        /// <summary>
        /// Holds information about a registry value
        /// </summary>
        public class RegistryValueData
        {
            public bool IsPopulated = false;

            public RegistryKey RootKey = null;
            public string KeyPath = string.Empty;

            public string KeyName = string.Empty;
            public bool KeyExists = false;

            public string ValueName = string.Empty;
            public bool ValueExists = false;

            public object ValueData = string.Empty;
            public bool ValueDataExists = false;

            public RegistryValueKind ValueType = RegistryValueKind.Unknown;
            public string Message = string.Empty;

        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public RegistryTools()
        {

        }

        /// <summary>
        /// Deletes a registry sub-key and all of it's children
        /// </summary>
        /// <param name="rootKey">Microsoft.Win32.RegistryKey object that represents the root key. IE: System.Win32.Registry.CurrentUser</param>
        /// <param name="keyPath">Full path to the key to delete, minus the root key name</param>
        public static void DeleteSubKeyTree(RegistryKey rootKey, string keyPath)
        {
            // Usage example: DeleteSubKeyTree(Registry.CurrentUser, "Software\\MyProgram");

            rootKey.DeleteSubKeyTree(keyPath.Trim('\\'));
        }

        /// <summary>
        /// Deletes a registry sub-key
        /// </summary>
        /// <param name="rootKey">Microsoft.Win32.RegistryKey object that represents the root key. IE: System.Win32.Registry.CurrentUser</param>
        /// <param name="keyPath">Full path to the key to delete, minus the root key name</param>
        public static void DeleteSubKey(RegistryKey rootKey, string keyPath)
        {
            // Usage example: DeleteSubKey(Registry.CurrentUser, "Software\\MyProgram");

            rootKey.DeleteSubKey(keyPath.Trim('\\'));
        }

        /// <summary>
        /// Deletes a registry value
        /// </summary>
        /// <param name="rootKey">Microsoft.Win32.RegistryKey object that represents the root key. IE: System.Win32.Registry.CurrentUser</param>
        /// <param name="keyPath">Full path to the key to delete, minus the root key name</param>
        /// <param name="valueName">Name of the value that is being deleted</param>
        public static void DeleteValue(RegistryKey rootKey, string keyPath, string valueName)
        {
            // Usage example: DeleteValue(Registry.CurrentUser, "Software\\MyProgram", "MyValue");

            using (RegistryKey key = rootKey.OpenSubKey(keyPath, true))
            {
                if (key == null)
                {
                    throw new Exception("Failed to open the specified sub-key");
                }
                key.DeleteValue(valueName);
            }
        }

        /// <summary>
        /// Creates a sub-key in the registry. If the sub-key already exists, the existing sub-key will be returned.
        /// </summary>
        /// <param name="rootKey">Microsoft.Win32.RegistryKey object that represents the root key. IE: System.Win32.Registry.CurrentUser</param>
        /// <param name="keyPath">Full path to the key to create the sub-key under, minus the root key name</param>
        /// <returns>Registry key that was created</returns>
        public static RegistryKey CreateSubKey(RegistryKey rootKey, string keyPath)
        {
            // Usage example: CreateSubKey(Registry.CurrentUser, "Software\\MyProgram");
            return rootKey.CreateSubKey(keyPath.Trim('\\'));
        }

        /// <summary>
        /// Reads a value from the registry
        /// </summary>
        /// <param name="rootKey">Microsoft.Win32.RegistryKey object that represents the root key. IE: System.Win32.Registry.CurrentUser</param>
        /// <param name="keyPath">Full path to the key to check, minus the root key name</param>
        /// <param name="valueName">Name of the value that is being read</param>
        /// <returns>Registry value</returns>
        public static RegistryValueData ReadRegistryValue(RegistryKey rootKey, string keyPath, string valueName)
        {
            // Usage Example: RegistryTools.ReadRegistryValue(Registry.CurrentUser, "Software\\MyProgram", "MyKey");

            RegistryValueData value = new RegistryValueData();

            //Set the root key, key name, and value name
            value.RootKey = rootKey;
            value.KeyName = keyPath;
            value.ValueName = valueName;

            //Check if the key exists
            value.KeyExists = KeyExists(rootKey, keyPath);
            if (value.KeyExists == false)
            {
                //Return an error if the key doesn't exist (could also happen if the user does not have proper permissions to the key)
                value.IsPopulated = false;
                value.Message = "The specified key does not exist!";
                return value;
            }
            else
            {
                //Key exists, check if the value exists
                value.ValueExists = ValueExists(rootKey, keyPath, valueName);
                if (value.ValueExists == false)
                {
                    value.IsPopulated = false;
                    value.Message = "The specified value does not exist!";
                    return value;
                }
                else
                {
                    //We know the value exists, let's open it up and see what's inside
                    using (RegistryKey parentKey = rootKey.OpenSubKey(keyPath))
                    {
                        //Grab the full path to the key
                        value.KeyPath = parentKey.Name;
                        //Attempt to set the value data
                        value.ValueData = parentKey.GetValue(valueName, null);
                        if (value.ValueData == null)
                        {
                            //Something went wrong
                            value.IsPopulated = false;
                            value.Message = "The specified value does contain any data!";
                            return value;
                        }
                        else
                        {
                            value.ValueDataExists = true;
                            value.ValueType = parentKey.GetValueKind(valueName);
                        }
                    }
                }
            }
            value.IsPopulated = true;
            value.Message = "Successful!";
            return value;
        }

        /// <summary>
        /// Writes a value to the registry. If the value does not exist in the registry, it will be created.
        /// </summary>
        /// <param name="rootKey">Microsoft.Win32.RegistryKey object that represents the root key. IE: System.Win32.Registry.CurrentUser</param>
        /// <param name="keyPath">Full path to the key to check, minus the root key name</param>
        /// <param name="valueName">Name of the value that is being written to</param>
        /// <param name="valueData">Data to put into the value</param>
        /// <param name="valueKind">Type of data that is being stored</param>
        /// <returns>Registry value</returns>
        public static RegistryValueData WriteRegistryValue(RegistryKey rootKey, string keyPath, string valueName, object valueData, RegistryValueKind valueKind)
        {
            // Usage example: RegistryTools.WriteRegistryValue(Registry.CurrentUser, "Software\\MyProgram", "MyKey", "Test", RegistryValueKind.String);

            RegistryValueData value = new RegistryValueData();

            //Set the root key, key name, and value name
            value.RootKey = rootKey;
            value.KeyName = keyPath;
            value.ValueName = valueName;

            //Check if the key exists
            value.KeyExists = KeyExists(rootKey, keyPath);
            if (value.KeyExists == false)
            {
                //Return an error if the key doesn't exist (could also happen if the user does not have proper permissions to the key)
                value.IsPopulated = false;
                value.Message = "The specified key does not exist!";
                return value;
            }
            else
            {
                //We don't care if the value exists, as it will be created automagically
                using (RegistryKey parentKey = rootKey.OpenSubKey(keyPath, true))
                {
                    //Grab the full path to the key
                    value.KeyPath = parentKey.Name;
                    //Attempt to set the value data
                    parentKey.SetValue(valueName, valueData, valueKind);
                    value.ValueData = parentKey.GetValue(valueName, null);
                    if (value.ValueData == null)
                    {
                        //Something went wrong
                        value.IsPopulated = false;
                        value.Message = "The specified value does not contain any data!";
                        return value;
                    }
                    else
                    {
                        value.ValueDataExists = true;
                        value.ValueType = parentKey.GetValueKind(valueName);
                    }
                }
            }
            value.IsPopulated = true;
            value.Message = "Successful!";
            return value;
        }

        /// <summary>
        /// Check if a registry key exists
        /// </summary>
        /// <param name="rootKey">Microsoft.Win32.RegistryKey object that represents the root key. IE: System.Win32.Registry.CurrentUser</param>
        /// <param name="keyPath">Full path to the key to check, minus the root key name</param>
        /// <returns>True if the key exists, false if it doesn't</returns>
        public static bool KeyExists(RegistryKey rootKey, string keyPath)
        {
            // Usage Example: KeyExists(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Control\\ComputerName\\ComputerName");

            bool keyFound = false;
            using (RegistryKey key = rootKey.OpenSubKey(keyPath))
            {
                if (key != null)
                {
                    keyFound = true;
                }
            }
            return keyFound;
        }

        /// <summary>
        /// Check if a registry value exists
        /// </summary>
        /// <param name="rootKey">Microsoft.Win32.RegistryKey object that represents the root key. IE: System.Win32.Registry.CurrentUser</param>
        /// <param name="keyPath">Full path to the key to check, minus the root key name</param>
        /// <param name="valueName">Name of the value that is being checked</param>
        /// <returns>True if the key exists, false if it doesn't</returns>
        public static bool ValueExists(RegistryKey rootKey, string keyPath, string valueName)
        {
            // Usage Example: ValueExists(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Control\\ComputerName\\ComputerName", "ComputerName");

            bool valueFound = false;
            object valueData = Registry.GetValue(rootKey.Name + "\\" + keyPath.Trim('\\'), valueName, null);
            if (valueData != null)
            {
                valueFound = true;
            }
            return valueFound;
        }
    }
}