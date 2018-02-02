using devDept.Eyeshot;
using devDept.Eyeshot.Entities;
using devDept.Geometry;
using System;
using System.Drawing;
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
            layout.SetView(viewType.Isometric);
            layout.DisplayMode = displayType.Rendered;
            layout.Rotate.Enabled = false;
            layout.OriginSymbol.Visible = false;
            layout.ViewCubeIcon.Visible = false;
            layout.CoordinateSystemIcon.Visible = false;
            layout.ActionMode = actionType.Pan;
            layout.ToolBar.Visible = false;
            layout.Grid.Step = 32;
            layout.BoundingBox.Visible = true;
            layout.Grid.MajorLineColor = Color.White;
            layout.ViewportBorder.Visible = false;
            layout.Background.BottomColor = Color.White;
            layout.Background.TopColor = Color.White;

            panel1.Controls.Add(layout);
            #endregion


            var img = Image.FromFile("wasp_F1.png");
            var p = new Picture(Plane.XY, 56, 84, img, false);

            p.Rotate(-30, new Vector3D(27, 70));
            layout.Entities.Add(p);

            layout.Invalidate();
        }
    }
}
