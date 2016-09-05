using System;
using System.Drawing;
using System.Windows.Forms;
using ThinkGeo.MapSuite.Core;
using ThinkGeo.MapSuite.DesktopEdition;
using System.Collections.Generic;

namespace MultipleLabels
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        private void TestForm_Load(object sender, EventArgs e)
        {
            winformsMap1.MapUnit = GeographyUnit.DecimalDegree;
            winformsMap1.BackgroundOverlay.BackgroundBrush = new GeoSolidBrush(GeoColor.FromArgb(255, 233, 232, 214));

            ShapeFileFeatureLayer austinStreetsLayer = new ShapeFileFeatureLayer(@"..\..\Data\austinstreets.shp");
            austinStreetsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(LineStyles.LocalRoad1);
            austinStreetsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(TextStyles.LocalRoad1("[FENAME] [FETYPE]  [FRADDL]-[TOADDL]"));
            austinStreetsLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay austinStreetsOverlay = new LayerOverlay();
            austinStreetsOverlay.Layers.Add("AustinStreetsLayer", austinStreetsLayer);
            winformsMap1.Overlays.Add("AustinStreetsOverlay", austinStreetsOverlay);

            winformsMap1.CurrentExtent = new RectangleShape(-97.7456279238844, 30.3027064993117, -97.7420552214766, 30.3006304695341);

            winformsMap1.Refresh();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            ShapeFileFeatureLayer austinStreetsLayer = ((LayerOverlay)winformsMap1.Overlays["AustinStreetsOverlay"]).Layers["AustinStreetsLayer"] as ShapeFileFeatureLayer;
            austinStreetsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Clear();
            austinStreetsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(LineStyles.LocalRoad1);

            if (rbnSingleStyle.Checked)
            {
                austinStreetsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(TextStyles.LocalRoad1("[FENAME] [FETYPE]  [FRADDL]-[TOADDL]"));
            }
            else
            {
                TextStyle primaryTextStyle = TextStyles.LocalRoad1("[FENAME] [FETYPE]");
                primaryTextStyle.XOffsetInPixel = 0;                

                TextStyle secondaryTextStyle = TextStyles.LocalRoad1("[FRADDL]-[TOADDL]");
                secondaryTextStyle.YOffsetInPixel =  15;
                secondaryTextStyle.Font = new GeoFont("Arial", 7);
                secondaryTextStyle.Mask = AreaStyles.County1;
                secondaryTextStyle.OverlappingRule = LabelOverlappingRule.AllowOverlapping;
                secondaryTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;

                austinStreetsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(primaryTextStyle);
                austinStreetsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(secondaryTextStyle);
            }

            winformsMap1.Refresh();
        }

        private void winformsMap1_MouseMove(object sender, MouseEventArgs e)
        {
            //Displays the X and Y in screen coordinates.
            statusStrip1.Items["toolStripStatusLabelScreen"].Text = "X:" + e.X + " Y:" + e.Y;

            //Gets the PointShape in world coordinates from screen coordinates.
            PointShape pointShape = ExtentHelper.ToWorldCoordinate(winformsMap1.CurrentExtent, new ScreenPointF(e.X, e.Y), winformsMap1.Width, winformsMap1.Height);

            //Displays world coordinates.
            statusStrip1.Items["toolStripStatusLabelWorld"].Text = "(world) X:" + Math.Round(pointShape.X, 4) + " Y:" + Math.Round(pointShape.Y, 4);
        }
    }
}
