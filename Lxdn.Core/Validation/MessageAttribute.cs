
using System;

namespace Lxdn.Core.Validation
{
    public class MessageAttribute : Attribute
    {
        public MessageAttribute(string text)
        {
            this.Text = text;
        }

        public string Text { get; private set; }
    }
}