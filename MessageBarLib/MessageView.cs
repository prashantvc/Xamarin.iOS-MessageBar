// MessageView.cs
//
// Author:
//       Prashant Cholachagudda <pvc@outlook.com>
//
// Copyright (c) 2013 Prashant Cholachagudda
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
using System.Drawing;

using CoreGraphics;

using Foundation;

using UIKit;

namespace MessageBar
{
    /// <summary>
    /// The message view.
    /// </summary>
    public class MessageView : UIView
    {
        #region Constants

        /// <summary>
        ///     The padding
        /// </summary>
        private const float Padding = 10.0f;

        /// <summary>
        ///     The text offset
        /// </summary>
        private const float TextOffset = 2.0f;

        #endregion

        #region Static Fields

        /// <summary>
        ///     The description color
        /// </summary>
        private static readonly UIColor DescriptionColor;

        /// <summary>
        ///     The description font
        /// </summary>
        private static readonly UIFont DescriptionFont;

        /// <summary>
        ///     The title color
        /// </summary>
        private static readonly UIColor TitleColor;

        /// <summary>
        ///     The title font
        /// </summary>
        private static readonly UIFont TitleFont;

        #endregion

        #region Fields

        /// <summary>
        ///     The icon size
        /// </summary>
        private readonly nfloat iconSize = 36.0f;

        /// <summary>
        ///     Thethis.height
        /// </summary>
        private float height;

        /// <summary>
        ///     The width
        /// </summary>
        private nfloat width;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="MessageView"/> class. 
        /// </summary>
        static MessageView()
        {
            TitleFont = UIFont.BoldSystemFontOfSize(16.0f);
            DescriptionFont = UIFont.SystemFontOfSize(14.0f);
            TitleColor = UIColor.FromWhiteAlpha(1.0f, 1.0f);
            DescriptionColor = UIColor.FromWhiteAlpha(1.0f, 1.0f);

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageView"/> class.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        internal MessageView(string title, string description, MessageType type)
            : this((NSString)title, (NSString)description, type)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageView"/> class.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        internal MessageView(NSString title, NSString description, MessageType type)
            : base(CGRect.Empty)
        {
            this.BackgroundColor = UIColor.Clear;
            this.ClipsToBounds = false;
            this.UserInteractionEnabled = true;
            this.Title = title;
            this.Description = description;
            this.MessageType = type;
            this.Height = 0.0f;
            this.Width = 0.0f;
            this.Hit = false;

            NSNotificationCenter.DefaultCenter.AddObserver(
                UIDevice.OrientationDidChangeNotification, 
                this.OrientationChanged);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the duration of the message.
        /// </summary>
        /// <value>
        ///     The duration.
        /// </value>
        public TimeSpan Duration { get; set; }

        /// <summary>
        ///     Gets a value indicating whether this instance has a description.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has description; otherwise, <c>false</c>.
        /// </value>
        public bool HasDescription
        {
            get
            {
                return !string.IsNullOrEmpty(this.Description);
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this instance has a title.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has title; otherwise, <c>false</c>.
        /// </value>
        public bool HasTitle
        {
            get
            {
                return !string.IsNullOrEmpty(this.Title);
            }
        }

        /// <summary>
        ///     Gets the height.
        /// </summary>
        /// <value>
        ///     The height.
        /// </value>
        public float Height
        {
            get
            {
                if (this.height == 0)
                {
                    CGSize titleLabelSize = this.TitleSize();
                    CGSize descriptionLabelSize = this.DescriptionSize();
                    this.height =
                        (float)
                        Math.Max(
                            (Padding * 2) + titleLabelSize.Height + descriptionLabelSize.Height, 
                            (Padding * 2) + this.iconSize);
                }

                return this.height;
            }

            private set
            {
                this.height = value;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="MessageView" /> is hit.
        /// </summary>
        /// <value>
        ///     <c>true</c> if hit; otherwise, <c>false</c>.
        /// </value>
        public bool Hit { get; set; }

        /// <summary>
        ///     Gets or sets the action to invoke on dismissing this view.
        /// </summary>
        /// <value>
        ///     The on dismiss.
        /// </value>
        public Action OnDismiss { get; set; }

        /// <summary>
        ///     Gets or sets the action to invoke on user tapping this view.
        /// </summary>
        /// <value>
        ///     The on tapped.
        /// </value>
        public Action OnTapped { get; set; }

        /// <summary>
        ///     Gets the width.
        /// </summary>
        /// <value>
        ///     The width.
        /// </value>
        public nfloat Width
        {
            get
            {
                if (this.width == 0)
                {
                    this.width = this.GetStatusBarFrame().Width;
                }

                return this.width;
            }

            private set
            {
                this.width = value;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the stylesheet provider.
        /// </summary>
        /// <value>
        ///     The stylesheet provider.
        /// </value>
        internal IStyleSheetProvider StylesheetProvider { get; set; }

        /// <summary>
        ///     Gets the available width.
        /// </summary>
        /// <value>
        ///     The width of the available.
        /// </value>
        private nfloat AvailableWidth
        {
            get
            {
                nfloat maxWidth = this.Width - (Padding * 3) - this.iconSize;
                return maxWidth;
            }
        }

        /// <summary>
        ///     Gets or sets the description of the object, the Objective-C version of ToString.
        /// </summary>
        private new NSString Description { get; set; }

        /// <summary>
        ///     Gets or sets the type of the message.
        /// </summary>
        /// <value>
        ///     The type of the message.
        /// </value>
        private MessageType MessageType { get; set; }

        /// <summary>
        ///     Gets or sets the title.
        /// </summary>
        /// <value>
        ///     The title.
        /// </value>
        private NSString Title { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Draws the view within the passed-in rectangle.
        /// </summary>
        /// <param name="rect">
        /// The <see cref="T:System.Drawing.RectangleF"/> to draw.
        /// </param>
        public override void Draw(CGRect rect)
        {
            var context = UIGraphics.GetCurrentContext();

            MessageBarStyleSheet styleSheet = this.StylesheetProvider.StyleSheetForMessageView(this);
            context.SaveState();

            styleSheet.BackgroundColorForMessageType(this.MessageType).SetColor();
            context.FillRect(rect);
            context.RestoreState();
            context.SaveState();

            context.BeginPath();
            context.MoveTo(0, rect.Size.Height);
            context.SetStrokeColor(styleSheet.StrokeColorForMessageType(this.MessageType).CGColor);
            context.SetLineWidth(1);
            context.AddLineToPoint(rect.Size.Width, rect.Size.Height);
            context.StrokePath();
            context.RestoreState();
            context.SaveState();

            nfloat xOffset = Padding;
            nfloat yOffset = Padding;
            styleSheet.IconImageForMessageType(this.MessageType)
                      .Draw(new CGRect(xOffset, yOffset, this.iconSize, this.iconSize));
            context.SaveState();

            yOffset -= TextOffset;
            xOffset += this.iconSize + Padding;

            CGSize titleLabelSize = this.TitleSize();
            CGSize descriptionLabelSize = this.DescriptionSize();

            if (!this.HasTitle || !this.HasDescription)
            {
                yOffset = (float)(Math.Ceiling(rect.Size.Height * 0.5) - Math.Ceiling(
                    (this.HasTitle ? titleLabelSize : descriptionLabelSize).Height * 0.5) - TextOffset);
            }

            if (this.HasTitle)
            {
                TitleColor.SetColor();
                var titleRectangle = new CGRect(xOffset, yOffset, titleLabelSize.Width, titleLabelSize.Height);
                this.Title.DrawString(titleRectangle, TitleFont, UILineBreakMode.TailTruncation, UITextAlignment.Left);
                yOffset += titleLabelSize.Height;
            }

            if (this.HasDescription)
            {
                DescriptionColor.SetColor();
                var descriptionRectangle = new CGRect(
                    xOffset, 
                    yOffset, 
                    descriptionLabelSize.Width, 
                    descriptionLabelSize.Height);
                this.Description.DrawString(
                    descriptionRectangle, 
                    DescriptionFont, 
                    UILineBreakMode.TailTruncation, 
                    UITextAlignment.Left);
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/>, is equal to this instance.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="System.Object"/> to compare with this instance.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is MessageView))
            {
                return false;
            }

            var messageView = (MessageView)obj;

            return this.Title == messageView.Title && this.MessageType == messageView.MessageType
                   && this.Description == messageView.Description;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether the device is running iOS 7 or later.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool IsRunningiOS7OrLater()
        {
            string systemVersion = UIDevice.CurrentDevice.SystemVersion;

            return IsRunningiOS8OrLater() || systemVersion.Contains("7");
        }

        /// <summary>
        /// Determines whether the device is running iOS 8 or later.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool IsRunningiOS8OrLater()
        {
            var systemVersion = int.Parse(UIDevice.CurrentDevice.SystemVersion.Substring(0, 1));

            return systemVersion >= 8;
        }

        /// <summary>
        /// Gets the size of the description.
        /// </summary>
        /// <returns>
        /// The <see cref="CGSize"/>.
        /// </returns>
        private CGSize DescriptionSize()
        {
            if (!this.HasDescription)
            {
                return CGSize.Empty;
            }

            var boundedSize = new CGSize(this.AvailableWidth, float.MaxValue);
            CGSize descriptionLabelSize;
            if (!IsRunningiOS7OrLater())
            {
                var attr = new UIStringAttributes(NSDictionary.FromObjectAndKey(TitleFont, (NSString)TitleFont.Name));
                descriptionLabelSize = this.Description.GetBoundingRect(
                    boundedSize, 
                    NSStringDrawingOptions.TruncatesLastVisibleLine, 
                    attr, 
                    null)
                                           .Size;
            }
            else
            {
                descriptionLabelSize = this.Description.StringSize(
                    DescriptionFont, 
                    boundedSize, 
                    UILineBreakMode.TailTruncation);
            }

            return descriptionLabelSize;
        }

        /// <summary>
        /// Gets the status bar frame.
        /// </summary>
        /// <returns>
        /// The <see cref="CGRect"/>.
        /// </returns>
        private CGRect GetStatusBarFrame()
        {
            var windowFrame = this.OrientFrame(UIApplication.SharedApplication.KeyWindow.Frame);
            var statusFrame = this.OrientFrame(UIApplication.SharedApplication.StatusBarFrame);

            return new CGRect(windowFrame.X, windowFrame.Y, windowFrame.Width, statusFrame.Height);
        }

        /// <summary>
        /// Determines whether the device is in landscape mode.
        /// </summary>
        /// <param name="orientation">
        /// The orientation.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsDeviceLandscape(UIDeviceOrientation orientation)
        {
            return orientation == UIDeviceOrientation.LandscapeLeft || orientation == UIDeviceOrientation.LandscapeRight;
        }

        /// <summary>
        /// Determines whether the status bar is in landscape mode.
        /// </summary>
        /// <param name="orientation">
        /// The orientation.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsStatusBarLandscape(UIInterfaceOrientation orientation)
        {
            return orientation == UIInterfaceOrientation.LandscapeLeft
                   || orientation == UIInterfaceOrientation.LandscapeRight;
        }

        /// <summary>
        /// Repositions the frame according to the device rotation and OS version.
        /// </summary>
        /// <param name="frame">
        /// The frame.
        /// </param>
        /// <returns>
        /// The <see cref="CGRect"/>.
        /// </returns>
        private CGRect OrientFrame(CGRect frame)
        {
            // This size has already inverted in iOS8, but not on simulator, seems odd
            if (!IsRunningiOS8OrLater() && (this.IsDeviceLandscape(UIDevice.CurrentDevice.Orientation)
                                            || this.IsStatusBarLandscape(
                                                UIApplication.SharedApplication.StatusBarOrientation)))
            {
                frame = new CGRect(frame.X, frame.Y, frame.Height, frame.Width);
            }

            return frame;
        }

        /// <summary>
        /// Invoked when the orientation changes.
        ///     Repositions the view accordingly.
        /// </summary>
        /// <param name="notification">
        /// The notification.
        /// </param>
        private void OrientationChanged(NSNotification notification)
        {
            this.Frame = new CGRect(this.Frame.X, this.Frame.Y, this.GetStatusBarFrame().Width, this.Frame.Height);
            this.SetNeedsDisplay();
        }

        /// <summary>
        /// Gets the size of the title.
        /// </summary>
        /// <returns>
        /// The <see cref="CGSize"/>.
        /// </returns>
        private CGSize TitleSize()
        {
            if (!this.HasTitle)
            {
                return CGSize.Empty;
            }

            var boundedSize = new SizeF((float)this.AvailableWidth, float.MaxValue);
            CGSize titleLabelSize;
            if (!IsRunningiOS7OrLater())
            {
                var attr = new UIStringAttributes(NSDictionary.FromObjectAndKey(TitleFont, (NSString)TitleFont.Name));
                titleLabelSize = this.Title.GetBoundingRect(
                    boundedSize, 
                    NSStringDrawingOptions.TruncatesLastVisibleLine, 
                    attr, 
                    null).Size;
            }
            else
            {
                titleLabelSize = this.Title.StringSize(TitleFont, boundedSize, UILineBreakMode.TailTruncation);
            }

            return titleLabelSize;
        }

        #endregion
    }
}