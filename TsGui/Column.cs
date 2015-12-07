﻿using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using System.Windows.Controls;

namespace TsGui
{
    public class Column
    {
        private List<IGuiOption> options = new List<IGuiOption>();
        private Grid columnpanel;

        public int Index { get; set; }
        public List<IGuiOption> Options { get { return this.options; } }
        public Panel Panel { get { return this.columnpanel; } }
        public int ControlWidth { get; set; }
        public int LabelWidth { get; set; }

        //constructor
        public Column (XElement SourceXml,int PageIndex)
        {
            this.Index = PageIndex;
            this.LoadXml(SourceXml);
            this.Build();
        }

        private void LoadXml(XElement SourceXml)
        {
            IEnumerable<XElement> optionsXml;
            IGuiOption newOption;
            //now read in the options and add to a dictionary for later use
            optionsXml = SourceXml.Elements("GuiOption");
            if (optionsXml != null)
            {
                foreach (XElement xOption in optionsXml)
                {
                    this.options.Add(GuiFactory.CreateGuiOption(xOption));
                }
            }
        }

        public void Build()
        {
            int rowindex = 0;
            Grid colGrid = new Grid();
            ColumnDefinition coldefControls = new ColumnDefinition();
            ColumnDefinition coldefLabels = new ColumnDefinition();

            colGrid.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            colGrid.ColumnDefinitions.Add(coldefLabels);
            colGrid.ColumnDefinitions.Add(coldefControls);
            //colGrid.ShowGridLines = true;
            
            foreach (IGuiOption option in this.options)
            {
                RowDefinition coldefRow = new RowDefinition();
                colGrid.RowDefinitions.Add(coldefRow);

                Grid.SetColumn(option.Label, 0);
                Grid.SetColumn(option.Control, 1);
                Grid.SetRow(option.Label, rowindex);
                Grid.SetRow(option.Control, rowindex);


                colGrid.Children.Add(option.Label);
                colGrid.Children.Add(option.Control);

                rowindex++;
            }

            this.columnpanel = colGrid;
        }
    }
}
