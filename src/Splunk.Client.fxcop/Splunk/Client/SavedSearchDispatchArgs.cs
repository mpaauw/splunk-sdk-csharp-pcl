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
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides the arguments required for retrieving transformed search 
    /// results.
    /// </summary>
    /// <remarks>
    /// <para><b>References:</b></para>
    /// <list type="number">
    /// <item><description>
    ///   <a href="http://goo.gl/AfzBJO">REST API Reference: POST 
    ///   saved/searches/{name}/dispatch</a>
    /// </description></item>
    /// </list>
    /// </remarks>
    public sealed class SavedSearchDispatchArgs : Args<SavedSearchDispatchArgs>
    {
        /// <summary>
        /// Gets or sets a value indicating whether a search should be 
        /// dispatched immediately.
        /// </summary>
        /// <value>
        /// <c>true</c>, if the <see cref="SaveSearch"/> job should be
        /// dispatched immediately; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// The default value is <c>false</c>.
        /// </remarks>
        [DataMember(Name = "dispatch.now", EmitDefaultValue = false)]
        [DefaultValue(false)]
        public bool DispatchNow
        { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of timeline buckets for a search.
        /// </summary>
        /// <value>
        /// The maximum number of timeline buckets for a search.
        /// </value>
        /// <remarks>
        /// This default value is <c>0</c>.
        /// </remarks>        
        [DataMember(Name = "dispatch.buckets", EmitDefaultValue = false)]
        [DefaultValue(0)]
        public int DispatchBuckets
        { get; set; }

        /// <summary>
        /// Gets or sets a time string specifying the earliest time to begin a
        /// <see cref="SavedSearch"/> job.
        /// </summary>
        /// <value>
        /// A time string specifying the earliest time to begin the <see cref=
        /// "SavedSearch"/> job.
        /// </value>
        /// <remarks>
        /// This value can be a relative or absolute time. If it is an absolute 
        /// time, use the <see cref="DispatchTimeFormat"/> to format it.
        /// </remarks>
        [DataMember(Name = "dispatch.earliest_time", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string DispatchEarliestTime
        { get; set; }

        /// <summary>
        /// Gets or sets a time string specifying the latest time to begin a 
        /// <see cref="SavedSearch"/> job.
        /// </summary>
        /// <value>
        /// A time string specifying the latest time to begin the <see cref=
        /// "SavedSearch"/> job.
        /// </value>
        /// <remarks>
        /// This value can be a relative or absolute time. If it is an absolute
        /// time, use the <see cref="DispatchTimeFormat"/> to format it.
        /// </remarks>
        [DataMember(Name = "dispatch.latest_time", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string DispatchLatestTime
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether lookups are enabled.
        /// </summary>
        /// <value>
        /// <c>true</c>, if lookups are enabled; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// The default value is <c>true</c>.
        /// </remarks>
        [DataMember(Name = "dispatch.lookups", EmitDefaultValue = false)]
        [DefaultValue(true)]
        public bool DispatchLookups
        { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of results to produce before 
        /// finalizing a <see cref="SavedSearch"/> job.
        /// </summary>
        /// <value>
        /// The maximum number of results to produce before finalizing the
        /// <see cref="SavedSearch"/> job.
        /// </value>
        [DataMember(Name = "dispatch.max_count", EmitDefaultValue = false)]
        [DefaultValue(0)]
        public int DispatchMaxCount
        { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies the maximum length of time to
        /// run before finalizing a <see cref="SavedSearch"/> job.
        /// </summary>
        /// <value>
        /// The maximum length of time to run before finalizing  the <see cref=
        /// "SavedSearch"/> job.
        /// </value>
        [DataMember(Name = "dispatch.max_time", EmitDefaultValue = false)]
        [DefaultValue(0)]
        public int DispatchMaxTime
        { get; set; }

        /// <summary>
        /// Gets or sets a value specifying how frequently Splunk runs the 
        /// map/reduce phase on accumulated map values.
        /// </summary>
        /// <value>
        /// A value specifying how frequently Splunk runs the map/reduce phase
        /// on accumulated map values.
        /// </value>
        /// <remarks>
        /// The default value is <c>10</c>.
        /// </remarks>
        [DataMember(Name = "dispatch.reduce_freq", EmitDefaultValue = false)]
        [DefaultValue(10)]
        public int DispatchReduceFrequency
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to back fill the real-time
        /// window for a <see cref="SavedSearch"/> job.
        /// </summary>
        /// <value>
        /// <c>true</c>, if the real-time window for the <see cref=
        /// "SavedSearch"/> job should be back filled; otherwise, <c>false</c>.
        /// search.
        /// </value>
        /// <remarks>
        /// This value only applies to real-time searches.
        /// </remarks>
        [DataMember(Name = "dispatch.rt_backfill", EmitDefaultValue = false)]
        [DefaultValue(false)]
        public bool DispatchRealTimeBackfill
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a <see cref="SavedSearch"/>
        /// job should run in its own process.
        /// </summary>
        /// <value>
        /// <c>true</c>, if the <see cref="SavedSearch"/> job should run in its
        /// own process; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// The default value is <c>false</c>. A searches against an index must 
        /// run in its own process.
        /// </remarks>
        [DataMember(Name = "dispatch.spawn_process", EmitDefaultValue = false)]
        [DefaultValue(false)]
        public bool DispatchSpawnProcess
        { get; set; }

        /// <summary>
        /// Gets or sets a value specifying the time format Splunk should use 
        /// to parse <see cref="EarliestTime"/> and <see cref="LatestTime"/>.
        /// </summary>
        /// <value>
        /// A value specifying the time format Splunk should use to parse <see
        /// cref="EarliestTime"/> and <see cref="LatestTime"/>.
        /// </value>
        /// <remarks>
        /// The default value is <c>"%FT%T.%Q%:z"</c>.
        /// </remarks>
        [DataMember(Name = "dispatch.time_format", EmitDefaultValue = false)]
        [DefaultValue("%FT%T.%Q%:z")]
        public string DispatchTimeFormat
        { get; set; }

        /// <summary>
        /// Gets or sets the time to live in seconds for the artifacts of a
        /// <see cref="SavedSearch"/> job, if no actions are triggered.
        /// </summary>
        /// <value>
        /// The time to live in seconds for the artifacts of the <see cref=
        /// "SavedSearch"/> job, if no actions are triggered.
        /// </value>
        /// <remarks>
        /// <para>
        /// The default value is <c>"2p"</c> (two scheduled search periods).
        /// </para><para>
        /// If an action is triggered, Splunk changes the time to live (TTL) to
        /// that action's TTL. If multiple actions are triggered, Splunk applies
        /// the maximum TTL to the artifacts. To set an action's TTL, refer to
        /// <a href="http://goo.gl/6YNR2X">alert_actions.conf.spec</a> in the
        /// Splunk Admin Manual.</para>
        /// </remarks>
        [DataMember(Name = "dispatch.time_format", EmitDefaultValue = false)]
        [DefaultValue("2p")]
        public string DispatchTtl
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to start a new search even 
        /// if another instance of the <see cref="SavedSearch"/> is already 
        /// running.
        /// </summary>
        /// <value>
        /// <c>true</c>, if a new search should be started even if another
        /// instance of the <see cref="SavedSearch"/> is already running;
        /// otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// The default value is <c>false</c>
        /// </remarks>
        [DataMember(Name = "force_dispatch", EmitDefaultValue = false)]
        [DefaultValue(false)]
        public bool ForceDispatch
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to trigger alert actions.
        /// </summary>
        /// <value>
        /// <c>true</c>, if alert actions should be triggered; <c>false</c>
        /// otherwise.
        /// </value>
        /// <remarks>
        /// The default value is <c>false</c>.
        /// </remarks>
        [DataMember(Name = "trigger_actions", EmitDefaultValue = false)]
        [DefaultValue(false)]
        public bool TriggerActions
        { get; set; }
    }
}