// MessageBarViewController.cs
//
// Author:
//       Prashant Cholachagudda <pvc@outlook.com>
//
// Copyright (c) 2014 Prashant Cholachagudda
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.


// Improved and documented by Bjarke Søgaard, https://github.com/Falgantil

using System;
using System.Linq;

using UIKit;

namespace MessageBar
{
    /// <summary>
    ///     The Message Bar View Controller. Nothing fancy about this, just a basic View Controller
    /// </summary>
    public class MessageBarViewController : UIViewController
    {
        #region Constructors and Destructors

        /// <summary>
        /// The preferred UIStatusBarStyle for this UIViewController.
        /// </summary>
        /// <returns>
        /// A UIStatusBarStyle key that specifies the view controller's preferred status bar style.
        /// </returns>
        /// <remarks>
        /// <para>You can override the preferred status bar style for a view controller by implementing the childViewControllerForStatusBarStyle method.</para>
        /// <para>If the return value from this method changes, call the setNeedsStatusBarAppearanceUpdate method.</para>
        /// </remarks>
        public override UIStatusBarStyle PreferredStatusBarStyle()
        {
            Type messageWindowType = typeof(MessageWindow);

            var app = UIApplication.SharedApplication;
            bool isNotMessageWindow = app.KeyWindow.GetType() != messageWindowType;

            UIWindow window = isNotMessageWindow ? app.KeyWindow : app.Windows.FirstOrDefault(k => k.GetType() != messageWindowType);

            if (window != null && window.RootViewController != null)
            {
                return window.RootViewController.PreferredStatusBarStyle();
            }
            return app.StatusBarStyle;
        }

        #endregion
    }
}