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

// ValidationToolTip.cs - tooltip class to handle tooltip validation events in the gui

using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Diagnostics;

using TsGui.View.GuiOptions;

namespace TsGui.Validation
{
    public class ValidationToolTipHandler
    {
        private Color _bordercolor;
        private Color _mouseoverbordercolor;
        private Color _focusbordercolor;
        private Popup _popup;
        private ValidationErrorToolTip _validationerrortooltip;
        private GuiOptionBase _guioption;
        private MainController _controller;

        //Constructor
        public ValidationToolTipHandler(GuiOptionBase GuiOption, MainController MainController)
        {
            this._guioption = GuiOption;
            this._controller = MainController;
            this._controller.WindowMoved += this.OnWindowMoved;

            //record the default colors
            this._bordercolor = this._guioption.ControlFormatting.BorderBrush.Color;
            this._mouseoverbordercolor = this._guioption.ControlFormatting.MouseOverBorderBrush.Color;
            this._focusbordercolor = this._guioption.ControlFormatting.FocusedBorderBrush.Color;

            this._validationerrortooltip = new ValidationErrorToolTip();
            this._popup = new Popup();
            this._popup.AllowsTransparency = true;
            this._popup.Child = this._validationerrortooltip;
            this._popup.PlacementTarget = this._guioption.UserControl;
            if (SystemParameters.MenuDropAlignment == false) { this._popup.Placement = PlacementMode.Right; }
            else { this._popup.Placement = PlacementMode.Left; }
        }

        public void Clear()
        {
            this._popup.IsOpen = false;
            this._guioption.ControlFormatting.BorderBrush.Color = this._bordercolor;
            this._guioption.ControlFormatting.MouseOverBorderBrush.Color = this._mouseoverbordercolor;
            this._guioption.ControlFormatting.FocusedBorderBrush.Color = this._focusbordercolor;
        }

        public void Show()
        {
            this._popup.IsOpen = true;

            //update the colors to red. 
            this._guioption.ControlFormatting.BorderBrush.Color = Colors.Red;
            this._guioption.ControlFormatting.MouseOverBorderBrush.Color = Colors.Red;
            this._guioption.ControlFormatting.FocusedBorderBrush.Color = Colors.Red;
        }

        public void OnWindowMoved(object o, RoutedEventArgs e)
        { Debug.WriteLine("***Window moved"); }
    }
}
