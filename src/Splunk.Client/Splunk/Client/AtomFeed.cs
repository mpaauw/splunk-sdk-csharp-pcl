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
//// [X] AtomFeed: Add properties, not just entries.
//// [O] Documentation

namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// Provides an object representation of a Splunk Atom Feed response.
    /// </summary>
    /// <remarks>
    /// <para><b>References:</b></para>
    /// <list type="number">
    /// <item><description>
    ///     <a href="http://goo.gl/TDthxd">REST API Reference Manual: Accessing
    ///     Splunk resources</a>.
    /// </description></item>
    /// <item><description>
    ///     <a href="http://goo.gl/YVTE9l">REST API Reference Manual: Atom Feed
    ///     responses</a>.
    /// </description></item>
    /// </list>
    /// </remarks>
    public sealed class AtomFeed
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of an <see cref="AtomFeed"/>
        /// </summary>
        public AtomFeed()
        { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the author of the current <see cref="AtomFeed"/> response.
        /// </summary>
        /// <remarks>
        /// <c>"Splunk"</c> is the author of all <see cref="AtomFeed"/> responses.
        /// </remarks>
        public string Author
        { get; private set; }

        /// <summary>
        /// Gets the list of results returned from the endpoint.
        /// </summary>
        public IReadOnlyList<AtomEntry> Entries
        { get; private set; }

        /// <summary>
        /// Gets the version of the Atom Feed generator.
        /// </summary>
        public Version GeneratorVersion
        { get; private set; }

        /// <summary>
        /// Gets the Splunk management URI for accessing the endpoint.
        /// </summary>
        public Uri Id
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyDictionary<string, Uri> Links
        { get; private set; }

        /// <summary>
        /// Gets the list of info, warning, or error messages associated with 
        /// the operation. 
        /// </summary>
        /// <remarks>
        /// Not all responses from an endpoint produce messages.
        /// </remarks>
        public IReadOnlyList<Message> Messages
        { get; private set; }

        /// <summary>
        /// Gets the list of pagination attributes for the response to a GET 
        /// operation.
        /// </summary>
        public Pagination Pagination
        { get; private set; }

        /// <summary>
        /// Gets the human readable name of the endpoint <see cref="Id"/>.
        /// </summary>
        /// <remarks>
        /// This value is typically derived from the last segment of <see cref=
        /// "Id"/>.
        /// </remarks>
        public string Title
        { get; private set; }

        /// <summary>
        /// Gets the date this endpoint was implemented in Splunk.
        /// </summary>
        public DateTime Updated
        { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously reads XML data into the current <see cref="AtomFeed"/>.
        /// </summary>
        /// <param name="reader">
        /// The reader from which to read.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        public async Task ReadXmlAsync(XmlReader reader)
        {
            Contract.Requires<ArgumentNullException>(reader != null, "reader");

            this.Author = null;
            this.Entries = null;
            this.GeneratorVersion = null;
            this.Id = null;
            this.Links = null;
            this.Messages = null;
            this.Pagination = Pagination.None;
            this.Title = null;
            this.Updated = DateTime.MinValue;

            reader.Requires(await reader.MoveToDocumentElementAsync("feed", "entry"));
            var documentElementName = reader.Name;

            List<AtomEntry> entries = null;
            Dictionary<string, Uri> links = null;
            List<Message> messages = null;

            await reader.ReadAsync();

            while (reader.NodeType == XmlNodeType.Element)
            {
                string name = reader.Name;

                switch (name)
                {
                    case "title":

                        this.Title = await reader.ReadElementContentAsync(StringConverter.Instance);
                        break;

                    case "id":

                        this.Id = await reader.ReadElementContentAsync(UriConverter.Instance);
                        break;

                    case "author":

                        await reader.ReadAsync();
                        reader.EnsureMarkup(XmlNodeType.Element, "name");
                        this.Author = await reader.ReadElementContentAsync(StringConverter.Instance);
                        reader.EnsureMarkup(XmlNodeType.EndElement, "author");
                        await reader.ReadAsync();
                        break;

                    case "generator":

                        string build = reader.GetRequiredAttribute("build");
                        string version = reader.GetRequiredAttribute("version");
                        this.GeneratorVersion = VersionConverter.Instance.Convert(string.Join(".", version, build));
                        await reader.ReadAsync();
                        break;

                    case "updated":

                        this.Updated = await reader.ReadElementContentAsync(DateTimeConverter.Instance);
                        break;

                    case "entry":

                        var entry = new AtomEntry();

                        if (entries == null)
                        {
                            entries = new List<AtomEntry>();
                        }

                        entries.Add(entry);

                        await entry.ReadXmlAsync(reader);
                        break;

                    case "link":

                        var href = reader.GetRequiredAttribute("href");
                        var rel = reader.GetRequiredAttribute("rel");

                        if (links == null)
                        {
                            links = new Dictionary<string, Uri>();
                        }

                        links[rel] = UriConverter.Instance.Convert(href);
                        await reader.ReadAsync();
                        break;

                    case "s:messages":

                        bool isEmptyElement = reader.IsEmptyElement;
                        await reader.ReadAsync();

                        if (messages == null)
                        {
                            messages = new List<Message>();
                        }

                        if (isEmptyElement)
                        {
                            continue;
                        }

                        while (reader.NodeType == XmlNodeType.Element && reader.Name == "s:msg")
                        {
                            var value = reader.GetRequiredAttribute("type");
                            var type = EnumConverter<MessageType>.Instance.Convert(value);
                            var text = await reader.ReadElementContentAsStringAsync();
                            
                            messages.Add(new Message(type, text));
                        }

                        if (reader.NodeType == XmlNodeType.EndElement)
                        {
                            reader.EnsureMarkup(XmlNodeType.EndElement, "s:messages");
                            await reader.ReadAsync();
                        }

                        break;

                    case "opensearch:itemsPerPage":

                        int itemsPerPage = await reader.ReadElementContentAsync(Int32Converter.Instance);
                        this.Pagination = new Pagination(itemsPerPage, this.Pagination.StartIndex, this.Pagination.TotalResults);
                        break;

                    case "opensearch:startIndex":

                        int startIndex = await reader.ReadElementContentAsync(Int32Converter.Instance);
                        this.Pagination = new Pagination(this.Pagination.ItemsPerPage, startIndex, this.Pagination.TotalResults);
                        break;

                    case "opensearch:totalResults":

                        int totalResults = await reader.ReadElementContentAsync(Int32Converter.Instance);
                        this.Pagination = new Pagination(this.Pagination.ItemsPerPage, this.Pagination.StartIndex, totalResults);
                        break;

                    default: throw new InvalidDataException(); // TODO: Diagnostics : unexpected start tag
                }
            }

            reader.EnsureMarkup(XmlNodeType.EndElement, documentElementName);
            await reader.ReadAsync();

            if (entries != null)
            {
                this.Entries = new ReadOnlyCollection<AtomEntry>(entries);
            }

            if (links != null)
            {
                this.Links = new ReadOnlyDictionary<string, Uri>(links);
            }

            if (messages != null)
            {
                this.Messages = new ReadOnlyCollection<Message>(messages);
            }
        }

        /// <summary>
        /// Gets a string representation for the current <see cref="AtomFeed"/>.
        /// </summary>
        /// <returns>
        /// A string representation of the current <see cref="AtomFeed"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("AtomFeed(Title={0}, Author={1}, Id={2}, Updated={3})", this.Title, this.Author, this.Id, this.Updated);
        }

        #endregion
    }
}
