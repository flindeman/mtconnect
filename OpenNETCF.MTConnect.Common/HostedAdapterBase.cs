﻿// -------------------------------------------------------------------------------------------------------
// LICENSE INFORMATION
//
// - This software is licensed under the MIT shared source license.
// - The "official" source code for this project is maintained at http://mtcagent.codeplex.com
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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

namespace OpenNETCF.MTConnect
{
    public abstract partial class HostedAdapterBase : IHostedAdapter, IDisposable
    {
        protected int m_refreshPeriod = 5000;
        protected const string m_separator = ".";
        protected IAgentInterface m_agentInterface;        
        protected AutoResetEvent m_shutdownEvent = new AutoResetEvent(false);

        public Device Device { get; protected set; }
        public bool Loaded { get; protected set; }
        public virtual string AdapterType { get; set; }
        public virtual string ID { get; set; }

        public abstract IHostedDevice HostedDevice { get; }

        public virtual void OnConfigurationChange() { }
        public virtual void OnError(Exception exception) { }
        public virtual void OnNewAgentInterface() { }
        public virtual void BeforeLoad() { }
        public virtual void AfterLoad() { }
        public ILogService LogService { get; set; }

        public HostedAdapterBase()
        {
            AdapterType = "Model";
        }

        ~HostedAdapterBase()
        {
            Dispose();
        }

        public virtual int RefreshPeriod
        {
            get { return m_refreshPeriod; }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            m_shutdownEvent.Set();
        }

        public virtual void PublishDefaultData() 
        {
            Debug.WriteLine("PublishDefaultData for " + this.Device.Name);

            new Thread(new ThreadStart(delegate
            {
                this.UpdateProperties();
            }))
            {
                IsBackground = true,
                Name = "PublishDefaultData"
            }
            .Start();
        }

        public virtual string AssemblyName
        {
            get { return Path.GetFileNameWithoutExtension(this.GetType().Assembly.FullName); }
        }

        public IAgentInterface AgentInterface
        {
            [DebuggerStepThrough]
            get { return m_agentInterface; }
            set
            {
                if (m_agentInterface == value) return;

                m_agentInterface = value;
                if (m_agentInterface != null)
                {
                    m_agentInterface.DataItemValueChanged += new EventHandler<DataItemValue>(OnDataItemValueChanged);
                }

                OnNewAgentInterface();
            }
        }

        public virtual void OnDataItemValueChanged(object sender, DataItemValue e)
        {
            if (!Loaded) return;
            if (sender == this) return;
            if (m_firstPublish) return;

            CheckForSetProperty(e);
        }

        public virtual void CreateDeviceAndComponents()
        {
            Device = new Device(HostedDevice.Name, HostedDevice.Name, HostedDevice.ID, false);

            // deep copy over the Conditions
            if (HostedDevice.Conditions != null)
            {
                foreach (var c in HostedDevice.Conditions)
                {
                    Device.DataItems.Add(c.Copy());
                }
            }

            WireConditionHandlers(HostedDevice.Conditions);

            if (HostedDevice.Components != null)
            {
                foreach (var component in HostedDevice.Components)
                {
                    var comp = new Component(component.ComponentType, component.Name, component.ID);

                    if (component.Conditions != null)
                    {
                        // deep copy over the Conditions
                        foreach (var c in component.Conditions)
                        {
                            comp.DataItems.Add(c.Copy());
                        }
                    }

                    Device.AddComponent(comp);

                    if (component.Components != null)
                    {
                        foreach (var subcomponent in component.Components)
                        {
                            var subcomp = new Component(subcomponent.ComponentType, subcomponent.Name, subcomponent.ID);
                            comp.Components.Add(subcomp);
                        }
                    }
                }
            }
        }

        private void WireConditionHandlers(ConditionCollection conditions)
        {
            if (conditions == null) return;

            foreach (var c in conditions)
            {
                c.ValueSet += delegate
                {
                    AgentInterface.PublishData(c.ID, c);
                };
            }
        }

        public void LoadPropertiesAndMethods()
        {
            LoadProperties();
            LoadMethods();
        }

        public void StartPublishing()
        {

            var threadName = string.Format("{0}[{1}]", this.GetType().Name, this.HostedDevice.Name);
            new Thread(PublisherProc)
            {
                IsBackground = true,
                Name = threadName
            }
           .Start();

            Loaded = true;
        }

        public virtual void AgentInitialized() 
        { 
            Device.SetAvailability(true);
        }

        protected virtual void OnPublish()
        {
            // derived classes will override this to provide publishing logic
        }

        protected void PublisherProc()
        {
            // publish default/startup property data
            UpdateProperties();

            // do periodic publishing work here
            while (!m_shutdownEvent.WaitOne(RefreshPeriod, false))
            {
                OnPublish();
            }
        }
    }

}
