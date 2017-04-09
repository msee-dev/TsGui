﻿//    Copyright (C) 2017 Mike Pohatu

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

// CombinedQuery.cs - combine multiple queries

using System.Collections.Generic;
using System.Xml.Linq;
using TsGui.Linking;

namespace TsGui.Queries
{
    public class CombinedQuery: BaseQuery, ILinkTarget, ILinkingEventHandler
    {
        private QueryList _querylist;
        private MainController _controller;
        private ILinkTarget _linktargetoption;

        public CombinedQuery(XElement inputxml, MainController controller, ILinkTarget targetoption)
        {
            this._querylist = new QueryList(this, controller);
            this._processingwrangler = new ResultWrangler();
            this._processingwrangler.Separator = string.Empty;
            this._reprocess = true;
            this._controller = controller;
            this._linktargetoption = targetoption;
            this.LoadXml(inputxml);
        }

        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);
            
            foreach (XElement x in InputXml.Elements())
            {
                IQuery newquery = QueryFactory.GetQueryObject(x, this._controller, this);
                if (newquery != null) { this._querylist.AddQuery(newquery); }
            }
        }

        public override ResultWrangler GetResultWrangler()
        {
            return this.ProcessQuery();
        }

        public override ResultWrangler ProcessQuery()
        {
            this._processingwrangler = this._processingwrangler.Clone();
            this._processingwrangler.NewSubList();
            this._processingwrangler.AddResultFormatters(this._querylist.GetAllResultFormatters());

            string s = this._processingwrangler.GetString();
            if (this.ShouldIgnore(s) == false) { return this._processingwrangler; }
            else { return null; }
        }

        public void RefreshValue()
        {
            this._linktargetoption.RefreshValue();
        }

        public void OnLinkedSourceValueChanged(ILinkSource source, LinkingEventArgs e)
        { this.RefreshValue(); }
    }
}