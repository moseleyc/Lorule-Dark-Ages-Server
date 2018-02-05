using devDept.Eyeshot;
using System;
using System.Windows.Forms;

namespace ContentBuilder
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private ViewportLayout layout = new ViewportLayout()
        {
             Dock = DockStyle.Fill
        };

        private void Form1_Load(object sender, EventArgs e)
        {
            #region View
            layout.Unlock("EYEPRO-0HP1-4SNN1-290TD-UPFNY");
            layout.Viewports.Add(new Viewport());
            layout.SetView(viewType.Top);
            layout.DisplayMode = displayType.Rendered;
            layout.Rotate.Enabled = false;
            layout.OriginSymbol.Visible = false;
            layout.ViewCubeIcon.Visible = false;
            layout.CoordinateSystemIcon.Visible = false;
            layout.ActionMode = actionType.Pan;
            layout.ToolBar.Visible = false;

            panel1.Controls.Add(layout);
            #endregion
        }
    }
}
