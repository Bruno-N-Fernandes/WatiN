﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatiN.Core.Native.Windows.Microsoft
{
    internal class MsaaObject : AssistiveTechnologyObject
    {
        private enum MsaaAccessibleRole
        {
            Alert = 8,
            Animation = 54,
            Application = 14,
            Border = 19,
            ButtonDropDown = 56,
            ButtonDropDownGrid = 58,
            ButtonMenu = 57,
            Caret = 7,
            Cell = 29,
            Character = 32,
            Chart = 17,
            CheckButton = 44,
            Client = 10,
            Clock = 61,
            Column = 27,
            ColumnHeader = 25,
            ComboBox = 46,
            Cursor = 6,
            Diagram = 53,
            Dial = 49,
            Dialog = 18,
            Document = 15,
            DropList = 47,
            Equation = 55,
            Graphic = 40,
            Grip = 4,
            Grouping = 20,
            HelpBalloon = 31,
            HotkeyField = 50,
            Indicator = 39,
            Link = 30,
            List = 33,
            ListItem = 34,
            MenuBar = 2,
            MenuItem = 12,
            MenuPopup = 11,
            Outline = 35,
            OutlineItem = 36,
            PageTab = 37,
            PageTabList = 60,
            Pane = 16,
            ProgressBar = 48,
            PropertyPage = 38,
            PushButton = 43,
            RadioButton = 45,
            Row = 28,
            RowHeader = 26,
            ScrollBar = 3,
            Separator = 21,
            Slider = 51,
            Sound = 5,
            SpinButton = 52,
            StaticText = 41,
            StatusBar = 23,
            Table = 24,
            Text = 42,
            TitleBar = 1,
            Toolbar = 22,
            Tooltip = 13,
            Whitespace = 59,
            Window = 9
        }

        [Flags]
        private enum MsaaAccessibleState
        {
            Unavailable  = 0x00000001,
            Selected = 0x00000002,
            Focused = 0x00000004,
            Pressed = 0x00000008,
            Checked = 0x00000010,
            Mixed = 0x00000020,
            Indeterminate = Mixed,
            ReadOnly = 0x00000040,
            HotTracked = 0x00000080,
            Default = 0x00000100,
            Expanded = 0x00000200,
            Collapsed = 0x00000400,
            Busy = 0x00000800,
            Floating = 0x00001000,
            Marqueed = 0x00002000,
            Animated = 0x00004000,
            Invisible = 0x00008000,
            Offscreen = 0x00010000,
            Sizeable = 0x00020000,
            Movable = 0x00040000,
            SelfVoicing = 0x00080000,
            Focusable = 0x00100000,
            Selectable = 0x00200000,
            Linked = 0x00400000,
            Traversed = 0x00800000,
            MultiSelectable = 0x01000000,
            ExtSelectable = 0x02000000,
            AlertLow = 0x04000000,
            AlertMedium = 0x08000000,
            AlertHigh = 0x10000000,
            Valid = 0x1fffffff
        }

        private Accessibility.IAccessible _rawObject = null;
        private int _objectIndex = 0;

        internal MsaaObject(IntPtr hwnd)
        {
            object apiAccessibleObject = null;
            MsWindowsNativeMethods.AccessibleObjectFromWindow(hwnd, OBJID.WINDOW, ref MsWindowsNativeMethods.IID_IAccessible, ref apiAccessibleObject);
            _rawObject = apiAccessibleObject as Accessibility.IAccessible;
        }

        internal MsaaObject(Accessibility.IAccessible rawObject, int objectIndex)
        {
            _rawObject = rawObject;
            _objectIndex = objectIndex;
        }

        internal override string Name
        {
            get { return _rawObject.get_accName(_objectIndex); }
        }

        internal override AccessibleRole Role
        {
            get
            {
                AccessibleRole baseRole = AccessibleRole.Invalid;
                object roleValue = _rawObject.get_accRole(_objectIndex);
                if (roleValue is int)
                {
                    MsaaAccessibleRole msaaRole = (MsaaAccessibleRole)_rawObject.get_accRole(_objectIndex);
                    baseRole = ConvertMsaaRoleToAccessibleRole(msaaRole);
                }
                return baseRole;
            }
        }

        internal override IList<AccessibleState> StateSet
        {
            get { return ConvertMsaaStateToAccessibleStateSet((MsaaAccessibleState)_rawObject.get_accState(_objectIndex)); }
        }

        internal override int ChildCount
        {
            get { return _rawObject.accChildCount; }
        }

        internal override bool SupportsActions
        {
            get { return !string.IsNullOrEmpty(_rawObject.get_accDefaultAction(_objectIndex)); }
        }

        internal override IList<AssistiveTechnologyObject> GetChildrenByRole(AccessibleRole matchingRole, bool visibleChildrenOnly, bool recursiveSearch)
        {
            List<AssistiveTechnologyObject> childList = new List<AssistiveTechnologyObject>();
            if (_rawObject != null && _objectIndex == 0)
            {
                int countOfChildren = ChildCount;
                object[] children = new object[countOfChildren];
                int returnedChildCount = 0;
                MsWindowsNativeMethods.AccessibleChildren(_rawObject, 0, countOfChildren, children, ref returnedChildCount);
                foreach(object childObject in children)
                {
                    MsaaObject accessibleChild = null;
                    if (childObject is int)
                    {
                        accessibleChild = new MsaaObject(_rawObject, (int)childObject);
                    }
                    else
                    {
                        Accessibility.IAccessible accessibleChildCandidate = childObject as Accessibility.IAccessible;
                        if (accessibleChildCandidate != null)
                            accessibleChild = new MsaaObject(accessibleChildCandidate, 0);
                    }
                    if (accessibleChild != null && accessibleChild.Role != AccessibleRole.TitleBar)
                    {
                        if ((accessibleChild.Role == matchingRole || matchingRole == AccessibleRole.AnyRole) && (!visibleChildrenOnly || accessibleChild.StateSet.Contains(AccessibleState.Visible)))
                        {
                            childList.Add(accessibleChild);
                        }
                        if (recursiveSearch && accessibleChild.ChildCount > 0)
                        {
                            childList.AddRange(accessibleChild.GetChildrenByRole(matchingRole, visibleChildrenOnly, recursiveSearch));
                        }
                    }
                }
            }
            return childList;
        }

        internal override void DoAction(int actionIndex)
        {
            // Note: actionIndex is ignored for MS Windows systems
            _rawObject.accDoDefaultAction(_objectIndex);
        }

        private IList<AccessibleState> ConvertMsaaStateToAccessibleStateSet(MsaaAccessibleState msaaAccessibleState)
        {
            List<AccessibleState> accessibleStateSet = new List<AccessibleState>();
            if ((msaaAccessibleState & MsaaAccessibleState.Unavailable) == MsaaAccessibleState.Unavailable)
                accessibleStateSet.Add(AccessibleState.Invalid);

            if ((msaaAccessibleState & MsaaAccessibleState.Selected) == MsaaAccessibleState.Selected)
                accessibleStateSet.Add(AccessibleState.Selected);

            if ((msaaAccessibleState & MsaaAccessibleState.Focused) == MsaaAccessibleState.Focused)
                accessibleStateSet.Add(AccessibleState.Focused);

            if ((msaaAccessibleState & MsaaAccessibleState.Pressed) == MsaaAccessibleState.Pressed)
                accessibleStateSet.Add(AccessibleState.Pressed);

            if ((msaaAccessibleState & MsaaAccessibleState.Checked) == MsaaAccessibleState.Checked)
                accessibleStateSet.Add(AccessibleState.Checked);

            if ((msaaAccessibleState & MsaaAccessibleState.Indeterminate) == MsaaAccessibleState.Indeterminate)
                accessibleStateSet.Add(AccessibleState.Indeterminate);

            if ((msaaAccessibleState & MsaaAccessibleState.ReadOnly) == MsaaAccessibleState.ReadOnly)
                accessibleStateSet.Add(AccessibleState.SelectableText);

            if ((msaaAccessibleState & MsaaAccessibleState.Default) == MsaaAccessibleState.Default)
                accessibleStateSet.Add(AccessibleState.IsDefault);

            if ((msaaAccessibleState & MsaaAccessibleState.Expanded) == MsaaAccessibleState.Expanded)
                accessibleStateSet.Add(AccessibleState.Expanded);

            if ((msaaAccessibleState & MsaaAccessibleState.Collapsed) == MsaaAccessibleState.Collapsed)
                accessibleStateSet.Add(AccessibleState.Collapsed);

            if ((msaaAccessibleState & MsaaAccessibleState.Busy) == MsaaAccessibleState.Busy)
                accessibleStateSet.Add(AccessibleState.Busy);

            if ((msaaAccessibleState & MsaaAccessibleState.Invisible) == 0 && (msaaAccessibleState & MsaaAccessibleState.Offscreen) == 0)
                accessibleStateSet.Add(AccessibleState.Visible);

            if ((msaaAccessibleState & MsaaAccessibleState.Sizeable) == MsaaAccessibleState.Sizeable)
                accessibleStateSet.Add(AccessibleState.Resizable);

            if ((msaaAccessibleState & MsaaAccessibleState.Focusable) == MsaaAccessibleState.Focusable)
                accessibleStateSet.Add(AccessibleState.Focusable);

            if ((msaaAccessibleState & MsaaAccessibleState.Selectable) == MsaaAccessibleState.Selectable)
                accessibleStateSet.Add(AccessibleState.Selectable);

            if ((msaaAccessibleState & MsaaAccessibleState.Traversed) == MsaaAccessibleState.Traversed)
                accessibleStateSet.Add(AccessibleState.Visited);

            if ((msaaAccessibleState & MsaaAccessibleState.MultiSelectable) == MsaaAccessibleState.MultiSelectable)
                accessibleStateSet.Add(AccessibleState.MultiSelectable);

            return accessibleStateSet;
        }

        private AccessibleRole ConvertMsaaRoleToAccessibleRole(MsaaAccessibleRole baseRole)
        {
            AccessibleRole accRole = AccessibleRole.Unknown;
            switch (baseRole)
            {
                case MsaaAccessibleRole.Alert:
                    accRole = AccessibleRole.Alert;
                    break;
                case MsaaAccessibleRole.Animation:
                    accRole = AccessibleRole.Animation;
                    break;
                case MsaaAccessibleRole.Application:
                    accRole = AccessibleRole.Application;
                    break;
                case MsaaAccessibleRole.ButtonDropDown:
                case MsaaAccessibleRole.ButtonDropDownGrid:
                case MsaaAccessibleRole.ButtonMenu:
                    accRole = AccessibleRole.PushButton;
                    break;
                case MsaaAccessibleRole.Cell:
                    accRole = AccessibleRole.TableCell;
                    break;
                case MsaaAccessibleRole.Chart:
                    accRole = AccessibleRole.Chart;
                    break;
                case MsaaAccessibleRole.CheckButton:
                    accRole = AccessibleRole.CheckBox;
                    break;
                case MsaaAccessibleRole.ColumnHeader:
                    accRole = AccessibleRole.TableColumnHeader;
                    break;
                case MsaaAccessibleRole.ComboBox:
                    accRole = AccessibleRole.ComboBox;
                    break;
                case MsaaAccessibleRole.Dial:
                    accRole = AccessibleRole.Dial;
                    break;
                case MsaaAccessibleRole.Dialog:
                    accRole =  AccessibleRole.Dialog;
                    break;
                case MsaaAccessibleRole.DropList:
                    accRole = AccessibleRole.List;
                    break;
                case MsaaAccessibleRole.Graphic:
                    accRole = AccessibleRole.Image;
                    break;
                case MsaaAccessibleRole.HotkeyField:
                    accRole = AccessibleRole.AcceleratorLabel;
                    break;
                case MsaaAccessibleRole.Link:
                    accRole = AccessibleRole.Link;
                    break;
                case MsaaAccessibleRole.List:
                    accRole = AccessibleRole.List;
                    break;
                case MsaaAccessibleRole.ListItem:
                    accRole = AccessibleRole.ListItem;
                    break;
                case MsaaAccessibleRole.MenuBar:
                    accRole = AccessibleRole.MenuBar;
                    break;
                case MsaaAccessibleRole.MenuItem:
                    accRole = AccessibleRole.MenuItem;
                    break;
                case MsaaAccessibleRole.MenuPopup:
                    accRole = AccessibleRole.Menu;
                    break;
                case MsaaAccessibleRole.PageTab:
                    accRole = AccessibleRole.PageTab;
                    break;
                case MsaaAccessibleRole.PageTabList:
                    accRole = AccessibleRole.PageTabList;
                    break;
                case MsaaAccessibleRole.Pane:
                    accRole = AccessibleRole.Panel;
                    break;
                case MsaaAccessibleRole.ProgressBar:
                    accRole = AccessibleRole.ProgressBar;
                    break;
                case MsaaAccessibleRole.PushButton:
                    accRole = AccessibleRole.PushButton;
                    break;
                case MsaaAccessibleRole.RadioButton:
                    accRole = AccessibleRole.RadioButton;
                    break;
                case MsaaAccessibleRole.RowHeader:
                    accRole = AccessibleRole.TableRowHeader;
                    break;
                case MsaaAccessibleRole.ScrollBar:
                    accRole = AccessibleRole.ScrollBar;
                    break;
                case MsaaAccessibleRole.Separator:
                    accRole = AccessibleRole.Separator;
                    break;
                case MsaaAccessibleRole.Slider:
                    accRole = AccessibleRole.Slider;
                    break;
                case MsaaAccessibleRole.SpinButton:
                    accRole = AccessibleRole.SpinButton;
                    break;
                case MsaaAccessibleRole.StaticText:
                    accRole = AccessibleRole.Label;
                    break;
                case MsaaAccessibleRole.StatusBar:
                    accRole = AccessibleRole.StatusBar;
                    break;
                case MsaaAccessibleRole.Table:
                    accRole = AccessibleRole.Table;
                    break;
                case MsaaAccessibleRole.Text:
                    accRole = AccessibleRole.Text;
                    break;
                case MsaaAccessibleRole.Toolbar:
                    accRole = AccessibleRole.Toolbar;
                    break;
                case MsaaAccessibleRole.Tooltip:
                    accRole = AccessibleRole.ToolTip;
                    break;
                case MsaaAccessibleRole.Window:
                    accRole = AccessibleRole.Window;
                    break;
                case MsaaAccessibleRole.TitleBar:
                    accRole = AccessibleRole.TitleBar;
                    break;

                case MsaaAccessibleRole.Border:
                case MsaaAccessibleRole.Caret:
                case MsaaAccessibleRole.Character:
                case MsaaAccessibleRole.Clock:
                case MsaaAccessibleRole.Client:
                case MsaaAccessibleRole.Cursor:
                case MsaaAccessibleRole.Diagram:
                case MsaaAccessibleRole.Document:
                case MsaaAccessibleRole.Equation:
                case MsaaAccessibleRole.Grip:
                case MsaaAccessibleRole.Grouping:
                case MsaaAccessibleRole.HelpBalloon:
                case MsaaAccessibleRole.Indicator:
                case MsaaAccessibleRole.Outline:
                case MsaaAccessibleRole.OutlineItem:
                case MsaaAccessibleRole.PropertyPage:
                case MsaaAccessibleRole.Row:
                case MsaaAccessibleRole.Sound:
                case MsaaAccessibleRole.Whitespace:
                    accRole = AccessibleRole.Unknown;
                    break;
                default:
                    accRole = AccessibleRole.Invalid;
                    break;
            }
            return accRole;
        }
    }
}
