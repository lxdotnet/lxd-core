using System.Text.RegularExpressions;

namespace Lxdn.Core.Expressions.Utils
{
    internal class PropertyToken
    {
        public PropertyToken(string token)
        {
            Match match = Regex.Match(token, @"^(?'name'.+)\[(?'index'\d+)\]$");
            if (match.Success)
            {
                this.Name = match.Groups["name"].Value;
                this.Index = int.Parse(match.Groups["index"].Value);
            }
            else
            {
                this.Name = token;
                this.Index = -1;
            }
        }

        public string Name { get; private set; }

        public int Index { get; private set; }

        public bool HasIndex
        {
            get { return -1 != this.Index; }
        }
    }
}