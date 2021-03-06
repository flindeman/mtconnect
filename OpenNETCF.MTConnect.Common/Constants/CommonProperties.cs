﻿// -------------------------------------------------------------------------------------------------------
// LICENSE INFORMATION
//
// - This software is licensed under the MIT shared source license.
// - The "official" source code for this project is maintained at http://mtconnect.codeplex.com
//
// Copyright (c) 2010 OpenNETCF Consulting
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
// associated documentation files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial 
// portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT 
// NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// -------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenNETCF.MTConnect
{
    public static class CommonProperties
    {
        public const string Name = "name";
        public const string NativeName = "nativeName";
        public const string Category = "category";
        public const string ID = "id";
        public const string UUID = "uuid";
        public const string Type = "type";
        public const string Units = "units";
        public const string NativeUnits = "nativeUnits";
        public const string SubType = "subType";
        public const string SampleRate = "sampleRate";
        public const string NativeScale = "nativeScale";
        public const string CoordinateSystem = "coordinateSystem";
        public const string SignificantDigits = "significantDigits";
        public const string Source = "source";
        public const string Qualifier = "qualifier";
        public const string NativeCode = "nativeCode";
        public const string NativeSeverity = "nativeSeverity";
    }
        
    public static class ExtendedProperties
    {
        public const string Writable = "writable";
        public const string ValueType = "valueType";
    }
}
