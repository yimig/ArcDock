﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCompleteTextBox.Editors
{
    public delegate void TextChangedEventHandler(object sender, TextChangedEventArgs e);
    public delegate void SelectionChangedEventHandler(object sender, TextChangedEventArgs e);

    public class TextChangedEventArgs : EventArgs
    {
        public string Text { get; set; }

        public TextChangedEventArgs(string text)
        {
            Text = text;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
