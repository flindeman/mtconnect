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
using System.Xml.Linq;

namespace OpenNETCF.MTConnect
{
    public class EntityClient : Client
    {
        private int m_nextSequence = -1;

        public EntityClient(string agentAddress)
            : base(agentAddress)
        {
        }

        public DeviceCollection Probe()
        {
            var xml = GetProbeXml();
            var devices = ConfigParser.ParseConfigFile(xml);
            return devices;
        }

        public DataStream Current()
        {
            var xml = GetCurrentXml();
            return DataStream.FromXml(xml);
        }

        public DataStream Sample()
        {
            return Sample(100);
        }

        public DataStream Sample(int maxItems)
        {
            int next;
            var xml = GetNextSampleXml(m_nextSequence, maxItems, out next);
            if (next > 0)
            {
                m_nextSequence = next;
            }
            if (xml == string.Empty) return null;
            return DataStream.FromXml(xml);
        }
    }

}