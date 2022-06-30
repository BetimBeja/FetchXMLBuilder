﻿using WeifenLuo.WinFormsUI.Docking;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
    public static class DockExtensions
    {
        public static void EnsureVisible(this DockContent form, DockPanel parent, DockState defaultState)
        {
            switch (form.DockState)
            {
                case DockState.Unknown:
                case DockState.Hidden:
                    form.Show(parent, defaultState);
                    break;
                case DockState.DockBottomAutoHide:
                    form.DockState = DockState.DockBottom;
                    break;
                case DockState.DockTopAutoHide:
                    form.DockState = DockState.DockTop;
                    break;
                case DockState.DockLeftAutoHide:
                    form.DockState = DockState.DockLeft;
                    break;
                case DockState.DockRightAutoHide:
                    form.DockState = DockState.DockRight;
                    break;
            }
            form.Activate();
        }
    }
}