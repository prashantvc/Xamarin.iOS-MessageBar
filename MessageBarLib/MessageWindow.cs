// MessageWindow.cs
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

using CoreGraphics;

using UIKit;

namespace MessageBar
{
    /// <summary>
    /// The message window.
    /// </summary>
    public class MessageWindow : UIWindow
    {
        #region Public Methods and Operators
        
        /// <summary>
        /// Returns the farthest descendant of the receiver in the view hierarchy (including itself) that contains a specified
        /// point.
        /// </summary>
        /// <param name="point">A point specified in the receiver’s local coordinate system (bounds).</param>
        /// <param name="uievent">The UI event</param>
        /// <returns>
        /// The view object that is the farthest descendent the current view and contains point.
        /// Returns nil if the point lies completely outside the receiver’s view hierarchy.
        /// </returns>
        public override UIView HitTest(CGPoint point, UIEvent uievent)
        {
            var hitView = base.HitTest(point, uievent);

            if (hitView.Equals(this.RootViewController.View))
            {
                hitView = null;
            }

            return hitView;
        }

        #endregion
    }
}