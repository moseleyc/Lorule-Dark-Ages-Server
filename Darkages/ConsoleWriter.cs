using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Darkages
{
    public class ControlWriter : TextWriter
    {
        private readonly Control textbox;

        public ControlWriter(Control textbox)
        {
            this.textbox = textbox;
        }

        public override Encoding Encoding => Encoding.ASCII;

        public override void Write(char value)
        {
            textbox.Text += value;
        }

        public override void Write(string value)
        {
            textbox.Text += value;
        }
    }
}