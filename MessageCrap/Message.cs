﻿#region license
// Copyright (c) 2020 Mike Pohatu
//
// This file is part of TsGui.
//
// TsGui is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, version 3 of the License.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
#endregion
using System;
using System.Threading;

namespace MessageCrap
{
    public class Message
    {
        internal Timer TimeoutTimer;

        /// <summary>
        /// The message ID/GUID. This is unique for every message
        /// </summary>
        public string ID { get; } = Guid.NewGuid().ToString();

        /// <summary>
        /// When the message was sent. Gets set by the MessageHub when it is received
        /// </summary>
        public DateTime Sent { get; internal set; }

        /// <summary>
        /// When this message will timeout waiting for a resonse (default 30 seconds)
        /// </summary>
        public long ResponseTimeoutMilliseconds { get; set; } = 30000;

        /// <summary>
        /// Message hub will track if a response is expected. Only the first response is acted on
        /// </summary>
        public bool ResponseExpected { get; set; }

        /// <summary>
        /// The message topic 
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// The sender of this message
        /// </summary>
        public object Sender { get; internal set; }

        /// <summary>
        /// Any arbitrary payload to be sent with the message
        /// </summary>
        public object Payload { get; set; }

        /// <summary>
        /// The message that initiated the message chain
        /// </summary>
        public Message RootMessage { get; internal set; }

        /// <summary>
        /// The message this message is in response to
        /// </summary>
        public Message RespondingTo { get; internal set; }

        /// <summary>
        /// Internal constructor so message objects have to be created by the MessgaeHub
        /// </summary>
        internal Message() { }

        internal void StartTimer()
        {
            this.TimeoutTimer = new Timer(this.OnTimedOut, null, this.ResponseTimeoutMilliseconds, int.MaxValue);
        }

        internal void StopTimer()
        {
            if (this.TimeoutTimer != null)
            {
                this.TimeoutTimer.Dispose();
            }
        }

        private void OnTimedOut(object source)
        {
            this.StopTimer();
            MessageHub.ReportTimeout(this);
        }
    }
}
