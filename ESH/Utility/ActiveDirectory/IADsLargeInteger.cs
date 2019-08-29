using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ESH.Utility.ActiveDirectory
{
    [ComImport]
    [Guid("9068270B-0939-11D1-8BE1-00C04FD8D503")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    internal interface IADsLargeInteger
    {
        [DispId(0x00000002)]
        int HighPart { get; set; }
        [DispId(0x00000003)]
        int LowPart { get; set; }
    }
}
