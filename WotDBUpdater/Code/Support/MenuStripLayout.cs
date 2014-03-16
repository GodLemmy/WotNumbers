﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace WotDBUpdater.Code.Support
{
    public class MenuStripLayout : ProfessionalColorTable
    {
        public static Color colorGrayDropDownBack = Color.FromArgb(255, 22, 22, 22);
        public static Color colorGrayMain = Color.FromArgb(255, 45, 45, 45);
        public static Color colorGrayHover = Color.FromArgb(255, 68, 68, 68);
        public static Color colorGrayOverFlowButton = Color.FromArgb(255, 82, 82, 82);
        public static Color colorGrayDiv = Color.FromArgb(255, 96, 96, 96);
        public static Color colorGrayCheckPressed = Color.FromArgb(255, 177, 177, 177);
        public static Color colorBlue = Color.FromArgb(255, 66, 125, 215);
        
        public override Color ButtonSelectedHighlight
        {
            get { return ButtonSelectedGradientMiddle; }
        }
        public override Color ButtonSelectedHighlightBorder
        {
            get { return ButtonSelectedBorder; }
        }
        public override Color ButtonPressedHighlight
        {
            get { return ButtonPressedGradientMiddle; }
        }
        public override Color ButtonPressedHighlightBorder
        {
            get { return ButtonPressedBorder; }
        }
        public override Color ButtonCheckedHighlight
        {
            get { return ButtonCheckedGradientMiddle; }
        }
        public override Color ButtonCheckedHighlightBorder
        {
            get { return ButtonSelectedBorder; }
        }
        public override Color ButtonPressedBorder
        {
            get { return ButtonSelectedBorder; }
        }
        public override Color ButtonSelectedBorder
        {
            get { return Color.FromArgb(255, 45, 45, 45); }
        }
        public override Color ButtonCheckedGradientBegin
        {
            get { return Color.FromArgb(255, 66, 125, 215); }
        }
        public override Color ButtonCheckedGradientMiddle
        {
            get { return Color.FromArgb(255, 66, 125, 215); }
        }
        public override Color ButtonCheckedGradientEnd
        {
            get { return Color.FromArgb(255, 66, 125, 215); }
        }
        public override Color ButtonSelectedGradientBegin
        {
            get { return Color.FromArgb(255, 68, 68, 68); }
        }
        public override Color ButtonSelectedGradientMiddle
        {
            get { return Color.FromArgb(255, 68, 68, 68); }
        }
        public override Color ButtonSelectedGradientEnd
        {
            get { return Color.FromArgb(255, 68, 68, 68); }
        }
        public override Color ButtonPressedGradientBegin
        {
            get { return Color.FromArgb(255, 66, 125, 215); }
        }
        public override Color ButtonPressedGradientMiddle
        {
            get { return Color.FromArgb(255, 66, 125, 215); }
        }
        public override Color ButtonPressedGradientEnd
        {
            get { return Color.FromArgb(255, 66, 125, 215); }
        }
        public override Color CheckBackground
        {
            get { return Color.FromArgb(255, 96, 96, 96); }
        }
        public override Color CheckSelectedBackground
        {
            get { return Color.FromArgb(255, 96, 96, 96); }
        }
        public override Color CheckPressedBackground
        {
            get { return Color.FromArgb(255, 177, 177, 177); }
        }
        public override Color GripDark
        {
            get { return Color.FromArgb(255, 96, 96, 96); }
        }
        public override Color GripLight
        {
            get { return Color.FromArgb(255, 45, 45, 45); }
        }
        public override Color ImageMarginGradientBegin
        {
            get { return Color.FromArgb(255, 22, 22, 22); }
        }
        public override Color ImageMarginGradientMiddle
        {
            get { return Color.FromArgb(255, 22, 22, 22); }
        }
        public override Color ImageMarginGradientEnd
        {
            get { return Color.FromArgb(255, 22, 22, 22); }
        }
        public override Color ImageMarginRevealedGradientBegin
        {
            get { return Color.FromName("Red"); }
        }
        public override Color ImageMarginRevealedGradientMiddle
        {
            get { return Color.FromName("Red"); }
        }
        public override Color ImageMarginRevealedGradientEnd
        {
            get { return Color.FromName("Red"); }
        }
        public override Color MenuStripGradientBegin
        {
            get { return Color.FromArgb(255, 45, 45, 45); }
        }
        public override Color MenuStripGradientEnd
        {
            get { return Color.FromArgb(255, 45, 45, 45); }
        }
        public override Color MenuItemSelected
        {
            get { return Color.FromArgb(255, 45, 45, 45); }
        }
        public override Color MenuItemBorder
        {
            get { return Color.FromArgb(255, 45, 45, 45); }
        }
        public override Color MenuBorder
        {
            get { return Color.FromArgb(255, 69, 69, 69); }
        }
        public override Color MenuItemSelectedGradientBegin
        {
            get { return Color.FromArgb(255, 69, 69, 69); }
        }
        public override Color MenuItemSelectedGradientEnd
        {
            get { return Color.FromArgb(255, 69, 69, 69); }
        }
        public override Color MenuItemPressedGradientBegin
        {
            get { return Color.FromArgb(255, 22, 22, 22); }
        }
        public override Color MenuItemPressedGradientMiddle
        {
            get { return Color.FromArgb(255, 22, 22, 22); }
        }
        public override Color MenuItemPressedGradientEnd
        {
            get { return Color.FromArgb(255, 22, 22, 22); }
        }
        public override Color RaftingContainerGradientBegin
        {
            get { return Color.FromName("White"); }
        }
        public override Color RaftingContainerGradientEnd
        {
            get { return Color.FromName("White"); }
        }
        public override Color SeparatorDark
        {
            get { return Color.FromArgb(255, 45, 45, 45); }
        }
        public override Color SeparatorLight
        {
            get { return Color.FromArgb(255, 96, 96, 96); }
        }
        public override Color StatusStripGradientBegin
        {
            get { return Color.FromArgb(255, 45, 45, 45); }
        }
        public override Color StatusStripGradientEnd
        {
            get { return Color.FromArgb(255, 45, 45, 45); }
        }
        public override Color ToolStripBorder
        {
            get { return Color.FromArgb(255, 45, 45, 45); }
        }
        public override Color ToolStripDropDownBackground
        {
            get { return Color.FromArgb(255, 22, 22, 22); }
        }
        public override Color ToolStripGradientBegin
        {
            get { return Color.FromArgb(255, 45, 45, 45); }
        }
        public override Color ToolStripGradientMiddle
        {
            get { return Color.FromArgb(255, 45, 45, 45); }
        }
        public override Color ToolStripGradientEnd
        {
            get { return Color.FromArgb(255, 45, 45, 45); }
        }
        public override Color ToolStripContentPanelGradientBegin
        {
            get { return Color.FromArgb(255, 45, 45, 45); }
        }
        public override Color ToolStripContentPanelGradientEnd
        {
            get { return Color.FromArgb(255, 45, 45, 45); }
        }
        public override Color ToolStripPanelGradientBegin
        {
            get { return Color.FromArgb(255, 45, 45, 45); }
        }
        public override Color ToolStripPanelGradientEnd
        {
            get { return Color.FromArgb(255, 45, 45, 45); }
        }
        public override Color OverflowButtonGradientBegin
        {
            get { return Color.FromArgb(255, 82, 82, 82); }
        }
        public override Color OverflowButtonGradientMiddle
        {
            get { return Color.FromArgb(255, 82, 82, 82); }
        }
        public override Color OverflowButtonGradientEnd
        {
            get { return Color.FromArgb(255, 82, 82, 82); }
        }
    }
}