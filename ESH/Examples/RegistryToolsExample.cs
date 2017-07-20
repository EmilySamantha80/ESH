using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESH.Utility;
using Microsoft.Win32;

namespace ESH.Examples
{
    public class RegistryToolsExample
    {
        public static void Test()
        {
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
            // Only one of the sub-key deletions can be called (can't delete something that's deleted), so this is commented out. Uncomment it and comment the above line to test DeleteSubKeyTree()
            //RegistryTools.DeleteSubKeyTree(rootKey, keyPath);
        }
    }
}
