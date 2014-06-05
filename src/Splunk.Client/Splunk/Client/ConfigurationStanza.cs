﻿/*
 * Copyright 2014 Splunk, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"): you may
 * not use this file except in compliance with the License. You may obtain
 * a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 */

//// TODO:
//// [O] Contracts
//// [O] Documentation

namespace Splunk.Client
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an object representation of a Splunk configuration stanza.
    /// </summary>
    public class ConfigurationStanza : Entity<Resource>, IConfigurationStanza
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationStanza"/>
        /// class.
        /// </summary>
        /// <param name="service">
        /// An object representing a root Splunk service endpoint.
        /// </param>
        /// <param name="fileName">
        /// Name of a configuration file.
        /// </param>
        /// <param name="stanzaName">
        /// Name of a stanza within <paramref name="fileName"/> containing the 
        /// configuration stanza to be represented by the current instance.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="fileName"/>, or <paramref name="stanzaName"/> are 
        /// <c>null</c> or empty.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> is <c>null</c>.
        /// </exception>
        protected internal ConfigurationStanza(Service service, string fileName, string stanzaName)
            : this(service.Context, service.Namespace, fileName, stanzaName)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationStanza"/>
        /// class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="fileName">
        /// Name of a configuration file.
        /// </param>
        /// <param name="stanzaName">
        /// Name of a stanza within <paramref name="fileName"/> containing the 
        /// configuration stanza to be represented by the current instance.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="fileName"/> or <paramref name="stanzaName"/> are 
        /// <c>null</c> or <paramref name="ns"/> is non-specific. 
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> or <paramref name="ns"/> are <c>null</c>.
        /// </exception>
        protected internal ConfigurationStanza(Context context, Namespace ns, string fileName, string stanzaName)
            : base(context, ns, new ResourceName(ConfigurationCollection.ClassResourceName, fileName, stanzaName))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationStanza"/> 
        /// class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="feed">
        /// A Splunk response atom feed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> or <see cref="feed"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidDataException">
        /// <paramref name="feed"/> is in an invalid format.
        /// </exception>
        protected internal ConfigurationStanza(Context context, AtomFeed feed)
        {
            this.Initialize(context, feed);
        }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref=
        /// "ConfigurationStanza"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code. Use one of these
        /// methods to obtain a <see cref="ConfigurationStanza"/> instance:
        /// <list type="table">
        /// <listheader>
        ///   <term>Method</term>
        ///   <description>Description</description>
        /// </listheader>
        /// <item>
        ///   <term><see cref="Configuration.CreateStanzaAsync"/></term>
        ///   <description>
        ///   Asynchronously creates a new <see cref="ConfigurationStanza"/>
        ///   in the current <see cref="Configuration"/> file.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term><see cref="Configuration.GetStanzaAsync"/></term>
        ///   <description>
        ///   Asynchronously retrieves an existing <see cref="ConfigurationStanza"/>
        ///   in the current <see cref="Configuration"/> file.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term><see cref="Configuration.UpdateStanzaAsync"/></term>
        ///   <description>
        ///   Asynchronously updates an existing <see cref="ConfigurationStanza"/>
        ///   in the current <see cref="Configuration"/> file.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term><see cref="Service.CreateConfigurationStanzaAsync"/></term>
        ///   <description>
        ///   Asynchronously creates a <see cref="ConfigurationStanza"/> 
        ///   identified by configuration file and stanza name.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term><see cref="Service.GetConfigurationStanzaAsync"/></term>
        ///   <description>
        ///   Asynchronously retrieves a <see cref="ConfigurationStanza"/> 
        ///   identified by configuration file and stanza name.
        ///   </description>
        /// </item>
        /// </list>
        /// </remarks>
        public ConfigurationStanza()
        { }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public ConfigurationSetting this[int index]
        {
            get { return new ConfigurationSetting(this.Resources[index]); }
        }

        /// <inheritdoc/>
        public int Count
        {
            get { return this.Resources.Count; }
        }

        /// <inheritdoc/>
        public virtual string Author
        {
            get { return this.GetValue("Author", StringConverter.Instance); }
        }

        #endregion

        #region Methods

        #region Operational interface

        /// <inheritdoc/>
        public virtual async Task<string> GetAsync(string keyName)
        {
            var resourceName = new ResourceName(this.ResourceName, keyName);

            using (var response = await this.Context.GetAsync(this.Namespace, resourceName))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);

                var reader = new StreamReader(response.Stream);
                var value = await reader.ReadToEndAsync();

                return value;
            }
        }

        /// <summary>
        /// Gets an enumerator that iterates through the current <see cref=
        /// "ConfigurationStanza"/>.
        /// </summary>
        /// <returns>
        /// An object for iterating through the current <see cref="ConfigurationStanza"/>.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator that iterates through the current <see cref=
        /// "ConfigurationStanza"/>.
        /// </summary>
        /// <returns>
        /// An object for iterating through the current <see cref="ConfigurationStanza"/>.
        /// </returns>
        public IEnumerator<ConfigurationSetting> GetEnumerator()
        {
            return this.Resources.Select(resource => new ConfigurationSetting(resource)).GetEnumerator();
        }

        /// <summary>
        /// Asynchronously removes the current <see cref="ConfigurationStanza"/>
        /// from its configuration file.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/79v7H3">DELETE 
        /// configs/conf-{file}/{name}</a> endpoint to remove the current
        /// <see cref="ConfigurationStanza"/>.
        /// </remarks>
        public override async Task RemoveAsync()
        {
            var rn = new ResourceName("configs", string.Concat("conf-", this.ResourceName[1]), this.ResourceName[2]);

            using (var response = await this.Context.DeleteAsync(this.Namespace, rn))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
            }
        }

        /// <inheritdoc/>
        public override async Task<bool> UpdateAsync(IEnumerable<Argument> arguments)
        {
            using (var response = await this.Context.PostAsync(this.Namespace, this.ResourceName, arguments))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
                return false;
            }
        }

        /// <inheritdoc/>
        public virtual async Task UpdateAsync(string keyName, string value)
        {
            var resourceName = new ResourceName(this.ResourceName, keyName);
            var arguments = new Argument[] { new Argument("value", value) };

            using (var response = await this.Context.PostAsync(this.Namespace, resourceName, arguments))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
            }
        }

        /// <inheritdoc/>
        public virtual async Task UpdateAsync(string keyName, object value)
        {
            await this.UpdateAsync(keyName, value.ToString());
        }

        #endregion

        #region Infrastructure methods

        /// <inheritdoc/>
        protected override void CreateSnapshot(AtomFeed feed)
        {
            base.CreateSnapshot(feed);
            this.Resources = this.Snapshot.GetValue("Resources") ?? NoResources;
        }

        /// <inheritdoc/>
        protected override void CreateSnapshot(BaseResource resource)
        {
            this.Snapshot = resource;
            this.Resources = this.Snapshot.GetValue("Resources") ?? NoResources;
        }

        #endregion
        
        #endregion

        #region Privates/internals

        static readonly IReadOnlyList<BaseResource> NoResources = new ReadOnlyCollection<BaseResource>(new List<BaseResource>());

        IReadOnlyList<BaseResource> Resources
        { get; set; }

        #endregion
    }
}
