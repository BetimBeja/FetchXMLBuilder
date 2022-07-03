﻿using Microsoft.Xrm.Sdk.Metadata;
using Rappen.XTB.FetchXmlBuilder.Builder;
using Rappen.XTB.FetchXmlBuilder.ControlsClasses;
using Rappen.XTB.FetchXmlBuilder.DockControls;
using Rappen.XTB.FetchXmlBuilder.Views;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Rappen.XTB.FetchXmlBuilder.Controls
{
    public partial class attributeControl : FetchXmlElementControlBase
    {
        private readonly AttributeMetadata[] attributes;
        private readonly AttributeMetadata[] allattributes;
        private bool aggregate;
        private Cell cell;

        public attributeControl() : this(null, null, null, null)
        {
        }

        public attributeControl(TreeNode node, AttributeMetadata[] attributes, FetchXmlBuilder fetchXmlBuilder, TreeBuilderControl tree)
        {
            InitializeComponent();
            this.attributes = attributes;
            allattributes = fetchXmlBuilder.GetAllAttribues(node.LocalEntityName()).ToArray();
            InitializeFXB(null, fetchXmlBuilder, tree, node);
        }

        protected override void PopulateControls()
        {
            cmbAttribute.Items.Clear();
            aggregate = Node.IsFetchAggregate();
            panAggregate.Visible = aggregate;
            grpLayout.Visible = !aggregate;
            cmbAggregate.Enabled = aggregate;
            chkGroupBy.Enabled = aggregate;
            if (!aggregate)
            {
                cmbAggregate.SelectedIndex = -1;
                chkGroupBy.Checked = false;
            }

            if (attributes != null)
            {
                foreach (var attribute in attributes)
                {
                    AttributeItem.AddAttributeToComboBox(cmbAttribute, attribute, false, FetchXmlBuilder.friendlyNames);
                }
            }

            UpdateUIFromCell(fxb.dockControlBuilder?.LayoutXML?.GetCell(Node));
        }

        protected override ControlValidationResult ValidateControl(Control control)
        {
            if (control == cmbAttribute)
            {
                if (string.IsNullOrWhiteSpace(cmbAttribute.Text))
                {
                    return new ControlValidationResult(ControlValidationLevel.Error, "Attribute", ControlValidationMessage.IsRequired);
                }
                if (fxb.entities != null)
                {
                    if (!allattributes.Any(a => a.LogicalName == cmbAttribute.Text))
                    {
                        return new ControlValidationResult(ControlValidationLevel.Warning, "Attribute", ControlValidationMessage.NotInMetadata);
                    }
                    if (!cmbAttribute.Items.OfType<AttributeItem>().Any(a => a.ToString() == cmbAttribute.Text))
                    {
                        return new ControlValidationResult(ControlValidationLevel.Info, "Attribute", ControlValidationMessage.NotShowingNow);
                    }
                }
            }
            else if (control == txtAlias)
            {
                if (Node.IsFetchAggregate() && string.IsNullOrWhiteSpace(txtAlias.Text))
                {
                    return new ControlValidationResult(ControlValidationLevel.Error, "Alias must be specified in aggregate queries");
                }
            }

            return base.ValidateControl(control);
        }

        private void chkGroupBy_CheckedChanged(object sender, EventArgs e)
        {
            EnableAggregateControls();
        }

        private void cmbAggregate_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableAggregateControls();
        }

        private void EnableAggregateControls()
        {
            cmbDateGrouping.Enabled = chkGroupBy.Checked;
            chkDistinct.Enabled = aggregate && !chkGroupBy.Checked;
            if (!chkDistinct.Enabled)
            {
                chkDistinct.Checked = false;
            }
            chkUserTZ.Enabled = chkGroupBy.Checked;
            if (!chkGroupBy.Checked)
            {
                cmbDateGrouping.SelectedIndex = -1;
                chkUserTZ.Checked = false;
            }
        }

        private void helpIcon_Click(object sender, EventArgs e)
        {
            FetchXmlBuilder.HelpClick(sender);
        }

        private void cmbAttribute_SelectedIndexChanged(object sender, EventArgs e)
        {
            fxb.ShowMetadata(Metadata());
            if (IsInitialized)
            {
                UpdateCellFromUI();
            }
        }

        internal void UpdateUIFromCell(Cell updatecell)
        {
            if (updatecell?.Attribute != Node)
            {
                return;
            }
            cell = updatecell;
            if (cell == null)
            {
                cell = fxb.dockControlBuilder?.LayoutXML?.AddCell(Node);
            }
            if (cell != null)
            {
                chkLayoutVisible.Checked = cell.Width > 0;
                trkLayoutWidth.Enabled = chkLayoutVisible.Checked;
                trkLayoutWidth.Value = cell.Width;
            }
            else
            {
                grpLayout.Visible = false;
            }
            UpdateCellUI();
        }

        private void UpdateCellFromUI()
        {
            if (cell == null)
            {
                cell = fxb.dockControlBuilder.LayoutXML.AddCell(Node);
            }
            if (cell != null)
            {
                cell.Name = Node.GetAttributeLayoutName();
                cell.Width = chkLayoutVisible.Checked ? trkLayoutWidth.Value : 0;
                fxb.dockControlLayoutXml?.UpdateXML(cell.Parent?.ToXML());
            }
            grpLayout.Visible = cell != null;
            trkLayoutWidth.Enabled = chkLayoutVisible.Checked;
            UpdateCellUI();
        }

        private void UpdateCellUI()
        {
            lblWidth.Text = $"Width: {cell?.Width}";
            lblIndex.Text = $"Display Index: {cell?.DisplayIndex}";
        }

        public override MetadataBase Metadata()
        {
            if (cmbAttribute.SelectedItem is AttributeItem item)
            {
                return item.Metadata;
            }
            return base.Metadata();
        }

        public override void Focus()
        {
            cmbAttribute.Focus();
        }

        private void trkLayoutWidth_Scroll(object sender, EventArgs e)
        {
            if (IsInitialized)
            {
                UpdateCellFromUI();
            }
        }

        private void chkLayoutVisible_CheckedChanged(object sender, EventArgs e)
        {
            if (IsInitialized)
            {
                UpdateCellFromUI();
            }
        }
    }
}