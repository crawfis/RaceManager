using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace RacingEventsTrackSystem
{
    public static class UIChildFinder
    {
        public static DependencyObject FindChild(this DependencyObject reference, string childName)
        {
            DependencyObject foundChild = null;
            if (reference != null)
            {
                int childrenCount = VisualTreeHelper.GetChildrenCount(reference);
                for (int i = 0; i < childrenCount; i++)
                {
                    var child = VisualTreeHelper.GetChild(reference, i);
                    // If the child is not of the request child type child

                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = child;
                        break;
                    }
                    else
                    {
                        DependencyObject res = FindChild(child, childName);
                        if (res != null)
                        {
                            return res;
                        }
                    }
                }
            }
            return foundChild;
        }
    }
}
