﻿using System.Collections.Generic;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System;
using System.Diagnostics;
using System.ComponentModel;

namespace TsGui
{
    public class TsPage: ITsGuiElement, INotifyPropertyChanged
    {
        private MainController _controller;
        private int _height;
        private int _width;
        private int _headingHeight;
        private string _headingTitle;
        private string _headingText;
        private Thickness _margin = new Thickness(0, 0, 0, 0);
        private List<TsColumn> _columns = new List<TsColumn>();
        private List<IGuiOption> _options = new List<IGuiOption>();
        private List<IEditableGuiOption> _editables = new List<IEditableGuiOption>();
        private Grid _pagepanel;
        private PageLayout _pagelayout;
        private TsPage _previouspage;
        private TsPage _nextpage;
        //private PageWindow _window;

        private bool _islast = false;
        private bool _isfirst = false;
        private bool _gridlines = false;

        //Properties
        #region
        //public PageLayout Window { get; set; }
        public int Width
        {
            get { return this._width; }
            set
            {
                this._width = value;
                this.OnPropertyChanged(this, "Width");
            }
        }
        public int Height
        {
            get { return this._height; }
            set
            {
                this._height = value;
                this.OnPropertyChanged(this, "Height");
            }
        }
        public string HeadingTitle
        {
            get { return this._headingTitle; }
            set
            {
                this._headingTitle = value;
                this.OnPropertyChanged(this, "HeadingTitle");
            }
        }
        public string HeadingText
        {
            get { return this._headingText; }
            set
            {
                this._headingText = value;
                this.OnPropertyChanged(this, "HeadingText");
            }
        }
        public int HeadingHeight
        {
            get { return this._headingHeight; }
            set
            {
                this._headingHeight = value;
                this.OnPropertyChanged(this, "HeadingHeight");
            }
        }
        public bool ShowGridlines
        {
            get { return this._gridlines; }
            set
            {
                this._gridlines = value;
                foreach (TsColumn c in this._columns) { c.ShowGridLines = value; }
            }
        }
        public TsPage PreviousPage
        {
            get { return this._previouspage; }
            set
            {
                this._previouspage = value;
                this.UpdateButtons();
            }
        }
        public TsPage NextPage
        {
            get { return this._nextpage; }
            set
            {
                this._nextpage = value;
                this.UpdateButtons();
            }
        }
        public List<IGuiOption> Options { get { return this._options; } }
        public PageLayout Page { get { return this._pagelayout; } }
        //public Grid Panel { get { return this._pagepanel; } }
        public bool IsLast
        {
            get { return this._islast; }
            set { this._islast = value; }
        }
        public bool IsFirst
        {
            get { return this._isfirst; }
            set { this._isfirst = value; }
        }
        #endregion

        //Setup the INotifyPropertyChanged interface 
        public event PropertyChangedEventHandler PropertyChanged;

        // OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(object sender, string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(sender, new PropertyChangedEventArgs(name));
            }
        }

        //Constructors
        public TsPage(XElement SourceXml, string HeadingTitle, string HeadingText, int Height,int Width,Thickness Margin,MainController RootController)
        {
            Debug.WriteLine("New page constructor");
            //Debug.WriteLine(SourceXml);
            this._controller = RootController;
            this._pagelayout = new PageLayout(this);
            this._pagepanel = this._pagelayout.MainGrid;
            this.Height = Height;
            this.Width = Width;
            this._margin = Margin;
            this.HeadingHeight = 40;
            this.HeadingTitle = HeadingTitle;
            this.HeadingText = HeadingText;
            //this._pagepanel = new Grid();

            this._pagelayout.DataContext = this;
            //this._window.SetBinding(Grid.HeightProperty, new Binding("Height"));
            //this._window.SetBinding(Grid.HeightProperty, new Binding("Width"));

            this.LoadXml(SourceXml);
            this.Build();
        }


        //Methods
        public void LoadXml(XElement InputXml)
        {
            IEnumerable<XElement> columnsXml;
            XElement x;
            int colIndex = 0;

            //now read in the options and add to a dictionary for later use
            columnsXml = InputXml.Elements("Column");
            if (columnsXml != null)
            {
                foreach (XElement xColumn in columnsXml)
                {
                    TsColumn c = new TsColumn(xColumn, colIndex,this._controller);
                    this._columns.Add(c);
                    colIndex++;
                }
            }

            x = InputXml.Element("Width");
            if (x != null)
            { this.Width = Convert.ToInt32(x.Value); }

            x = InputXml.Element("Height");
            if (x != null)
            { this.Height = Convert.ToInt32(x.Value); }

            this.PopulateOptions();
        }

        //build the gui controls.
        public void Build()
        {
            int colindex = 0;

            this._pagepanel.VerticalAlignment = VerticalAlignment.Top;
            this._pagepanel.HorizontalAlignment = HorizontalAlignment.Left;
            //create a last row for the buttons
            RowDefinition colrowdef = new RowDefinition();
            this._pagepanel.RowDefinitions.Add(colrowdef);

            foreach (TsColumn col in this._columns)
            {
                ColumnDefinition coldef = new ColumnDefinition();
                //coldef
                this._pagepanel.ColumnDefinitions.Add(coldef);

                Grid.SetColumn(col.Panel, colindex);
                this._pagepanel.Children.Add(col.Panel);
                colindex++;
            }

            //this._pagelayout.MainGrid.Children.Add(this.Panel);
            this.UpdateButtons();
        }

        //get all the options from the sub columns. this is parsed up the chain to generate the final
        //list of ts variables to set at the end. 
        private void PopulateOptions()
        {
            foreach (TsColumn col in this._columns)
            {
                this._options.AddRange(col.Options);
            }

            foreach (IGuiOption option in this._options)
            {
                if (option is IEditableGuiOption) { this._editables.Add((IEditableGuiOption)option); }
            }
        }

        public bool OptionsValid()
        {
            Debug.WriteLine("OptionsValid called");
            foreach (IEditableGuiOption option in this._editables)
            {
                if (option.IsValid == false)
                {
                    Debug.WriteLine("invalid option found");
                    return false;
                }

            }

            return true;
        }

        public void Cancel()
        {
            this._controller.Cancel();
        }

        public void MovePrevious()
        {
            if (this.OptionsValid() == true)
            {
                this._controller.MovePrevious();
            }
        }

        public void MoveNext()
        {
            if (this.OptionsValid() == true)
            {
                this._controller.MoveNext();
            }
        }

        public void Finish()
        {
            if (this.OptionsValid() == true)
            {
                this._controller.Finish();
            }
        }

        public void Show()
        {
            Debug.WriteLine("Page show called: ");
            //this.Window.Show();
            //this.Window.Activate();
        }

        private void UpdateButtons()
        {
            if (this.NextPage != null)
            {
                this._pagelayout.buttonNext.Visibility = Visibility.Visible;
                this._pagelayout.buttonNext.IsEnabled = true;
                this._pagelayout.buttonFinish.Visibility = Visibility.Hidden;
                this._pagelayout.buttonFinish.IsEnabled = false;
            }

            if (this.PreviousPage != null)
            {
                this._pagelayout.buttonPrev.Visibility = Visibility.Visible;
                this._pagelayout.buttonPrev.IsEnabled = true;
            }
        }
    }
}
