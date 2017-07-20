// Title: Reflection utility methods
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
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;

namespace ESH.Utility
{
    public class ReflectionTools
    {
        public static IEnumerable<Type> GetTypesImplementingInterface(Type type)
        {
            var types = new List<Type>();
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in asm.GetTypes())
                {
                    if (type.IsAssignableFrom(t) && !t.IsInterface)
                    {
                        types.Add(t);
                    }
                }
            }
            return types;
            //return AppDomain.CurrentDomain.GetAssemblies()
            //    .SelectMany(s => s.GetTypes())
            //    .Where(p => type.IsAssignableFrom(p) && !p.IsInterface);
        }

        public static Type GetCurrentType()
        {
            try
            {
                return new StackTrace().GetFrame(1).GetMethod().ReflectedType;
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }

        public static Type GetCallingType()
        {
            try
            {
                return new StackTrace().GetFrame(2).GetMethod().ReflectedType;
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }

        public static MethodBase GetCallingMethod()
        {
            try
            {
                return new StackTrace().GetFrame(2).GetMethod();
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }

        public static MethodBase GetCurrentMethod()
        {
            try
            {
                return new StackTrace().GetFrame(1).GetMethod();
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }
    }
}
