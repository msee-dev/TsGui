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

// TsFreeText.cs - TextBox control for entering text. Can be configured to 
// check for the validity of the entered text

using TsGui.Validation;

using System;
using System.Windows;
using System.Xml.Linq;

namespace TsGui.View.GuiOptions
{
    public class TsFreeText: GuiOptionBase, IGuiOption, IValidationGuiOption
    {
        protected TsFreeTextUI _freetextui;
        protected string _controltext;
        protected string _validationtext;
        protected int _maxlength;
        private ValidationToolTipHandler _validationtooltiphandler;
        private ValidationHandler _validationhandler;

        //Properties
        #region
        //Custom stuff for control
        public string ControlText
        {
            get { return this._controltext; }
            set
            {
                this._controltext = value;
                this.OnPropertyChanged(this, "ControlText");
                this.NotifyUpdate();
                this.Validate();
            }
        }
        public override string CurrentValue { get { return this._controltext; } }
        public bool IsValid { get { return this.Validate(); } }
        public int MaxLength
        {
            get { return this._maxlength; }
            set { this._maxlength = value; this.OnPropertyChanged(this, "MaxLength"); }
        }
        public TsVariable Variable
        {
            get
            {
                if ((this.IsActive == false) && (this.PurgeInactive == true))
                { return null; }
                else
                { return new TsVariable(this.VariableName, this.ControlText); }
            }
        }
        public string ValidationText
        {
            get { return this._validationtext; }
            set { this._validationtext = value; this.OnPropertyChanged(this, "ValidationText"); }
        }
        #endregion

        //Constructor
        public TsFreeText(XElement InputXml, TsColumn Parent, MainController MainController): base (Parent, MainController)
        {
            this.Init(MainController);
            this.LoadXml(InputXml);
        }

        protected TsFreeText(TsColumn Parent, MainController MainController): base(Parent, MainController)
        {
            this.Init(MainController);
        }

        private void Init(MainController MainController)
        {
            //this._controltext = string.Empty;
            this._controller = MainController;

            this._freetextui = new TsFreeTextUI();
            this.Control = this._freetextui;
            this.Label = new TsLabelUI();

            this._validationhandler = new ValidationHandler(this,MainController);
            this._validationtooltiphandler = new ValidationToolTipHandler(this,this._controller);

            this.UserControl.DataContext = this;
            this._controller.WindowLoaded += this.OnWindowLoaded;
            this._freetextui.TextBox.LostFocus += this.OnValidationEvent;
            this.UserControl.IsEnabledChanged += this.OnValidationEvent;
            this.SetDefaults();
        }

        private void SetDefaults()
        {
            this.ControlFormatting.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.ControlFormatting.Padding = new Thickness(3, 2, 3, 2);
        }

        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);

            this.MaxLength = XmlHandler.GetIntFromXAttribute(InputXml, "MaxLength", this.MaxLength);
            this._validationhandler.LoadLegacyXml(InputXml);
            this._validationhandler.AddValidations(InputXml.Elements("Validation"));

            XElement x;         

            x = InputXml.Element("DefaultValue");
            if (x != null)
            {
                XAttribute xusecurrent = x.Attribute("UseCurrent");
                if (xusecurrent != null)
                {
                    //default behaviour is to check if the ts variable is already set. If it is, set that as the default i.e. add a query for 
                    //an environment variable to the start of the query list. 
                    if (!string.Equals(xusecurrent.Value, "false", StringComparison.OrdinalIgnoreCase))
                    {
                        XElement xcurrentquery = new XElement("Query", new XElement("Variable", this.VariableName));
                        xcurrentquery.Add(new XAttribute("Type", "EnvironmentVariable"));
                        x.AddFirst(xcurrentquery);
                    }
                }

                this._controltext = this._controller.EnvironmentController.GetValueFromList(x);
                if (this._controltext == null) { this._controltext = string.Empty; }

                //if required, remove invalid characters and truncate
                string invalchars = this._validationhandler.GetAllInvalidCharacters();
                if (!string.IsNullOrEmpty(invalchars)) { this._controltext = ResultValidator.RemoveInvalid(this.ControlText, this._validationhandler.GetAllInvalidCharacters()); }
                if (this.MaxLength > 0) { this._controltext = ResultValidator.Truncate(this.ControlText, this.MaxLength); }
            }
        }

        //Handle UI events
        #region

        public void OnValidationEvent(bool b)
        { this.Validate(); }

        public void OnValidationEvent(object sender, RoutedEventArgs e)
        { this.Validate(); }

        public void OnValidationEvent(object sender, DependencyPropertyChangedEventArgs e)
        { this.Validate(); }

        public void OnWindowLoaded(object o, RoutedEventArgs e)
        { this.Validate(); }
        #endregion

        public bool Validate()
        {
            //if (this._controller.StartupFinished == false) { return true; }
            if (this.IsActive == false) { this._validationtooltiphandler.Clear(); return true; }

            bool newvalid = this._validationhandler.IsValid(this.ControlText);

            if (newvalid == false)
            {
                string validationmessage = this._validationhandler.ValidationMessage;
                string s = "\"" + this.ControlText + "\" is invalid" + Environment.NewLine;
                if (string.IsNullOrEmpty(validationmessage)) { s = s + _validationhandler.FailedValidationMessage; }
                else { s = s + validationmessage; }

                this.ValidationText = s;
                this._validationtooltiphandler.ShowError();
            }
            else { this._validationtooltiphandler.Clear(); }

            return newvalid;
        }

        public void ClearToolTips()
        { this._validationtooltiphandler.Clear(); }

        public void OnValidationChange()
        { this.Validate(); }
    }
}
