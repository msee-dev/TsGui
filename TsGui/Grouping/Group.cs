﻿//    Copyright (C) 2016 Mike Pohatu

//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; version 2 of the License.

//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.

//    You should have received a copy of the GNU General Public License along
//    with this program; if not, write to the Free Software Foundation, Inc.,
//    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.

// Group.cs - groups of elements to be enabled and disabled by a toggle

using System.Collections.Generic;
using System;
//using System.Diagnostics;

namespace TsGui
{
    public class Group
    {
        public event GroupStateChange StateEvent;
        
        private List<IGroupable> _elements;
        private GroupState _state;

        //properties
        #region
        public GroupState State
        {
            get { return this._state; }
            set
            {
                this._state = value;
                StateEvent?.Invoke();
            }
        }
        public bool PurgeInactive { get; set; }
        public string ID { get; set; }
        public int Count { get { return this._elements.Count; } }
        #endregion

        //constructor
        public Group (string ID)
        {
            this._elements = new List<IGroupable>();
            this.ID = ID;
            this.State = GroupState.Enabled;
            this.PurgeInactive = false;
        }

        //method
        public void Add(IGroupable GroupableElement)
        {
            this._elements.Add(GroupableElement);
            this.StateEvent += GroupableElement.OnGroupStateChange;
        }
    }
}
