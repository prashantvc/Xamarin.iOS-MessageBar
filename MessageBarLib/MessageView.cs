//
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

using MonoTouch.UIKit;
using System.Drawing;
using System;
using MonoTouch.Foundation;
using MonoTouch.CoreGraphics;

namespace MessageBar
{
	public class MessageView : UIView
	{
		static UIFont TitleFont;
		static UIFont DescriptionFont;
		static UIColor TitleColor;
		const float Padding = 10.0f;
		const int IOS7Identifier = 7;
		const float IconSize = 36.0f;
		const float TextOffset = 2.0f;
		static readonly UIColor DescriptionColor = null;
		float height;
		float width;


		NSString Title { get; set; }

		new NSString Description { get; set; }

		MessageType MessageType { get; set; }

		public Action OnDismiss { get; set; }

		public bool Hit { get; set; }


		public float Height {
			get {
				if (height == 0) {
					SizeF titleLabelSize = TitleSize ();
					SizeF descriptionLabelSize = DescriptionSize ();
					height = Math.Max ((Padding * 2) + titleLabelSize.Height + descriptionLabelSize.Height, (Padding * 2) + IconSize);
				}

				return height;
			}
			private set {
				height = value;
			}
		}
			
		public float Width {
			get {
				if (width == 0) {
					width = GetStatusBarFrame ().Width;
				}

				return width;
			}
			private set {
				width = value;
			}
		}

		internal IStyleSheetProvider StylesheetProvider { get; set; }

		float AvailableWidth {
			get {
				float maxWidth = (Width - (Padding * 3) - IconSize);
				return maxWidth;
			}
		}

		static MessageView ()
		{
			TitleFont = UIFont.BoldSystemFontOfSize (16.0f);
			DescriptionFont = UIFont.SystemFontOfSize (14.0f);
			TitleColor = UIColor.FromWhiteAlpha (1.0f, 1.0f);
			DescriptionColor = UIColor.FromWhiteAlpha (1.0f, 1.0f);
		}

		internal MessageView (string title, string description, MessageType type)
			: this ((NSString)title, (NSString)description, type)
		{
			
		}

		internal MessageView (NSString title, NSString description, MessageType type)
			: base (RectangleF.Empty)
		{
			BackgroundColor = UIColor.Clear;
			ClipsToBounds = false;
			UserInteractionEnabled = true;
			Title = title;
			Description = description;
			MessageType = type;
			Height = 0.0f;
			Width = 0.0f;
			Hit = false;

			NSNotificationCenter.DefaultCenter.AddObserver (UIDevice.OrientationDidChangeNotification, OrientationChanged);
		}

		void OrientationChanged (NSNotification notification){
			Frame = new RectangleF (Frame.X, Frame.Y, GetStatusBarFrame ().Width, Frame.Height);
			SetNeedsDisplay ();
		}

		RectangleF GetStatusBarFrame()
		{
			var windowFrame = OrientFrame (UIApplication.SharedApplication.KeyWindow.Frame);
			var statusFrame = OrientFrame (UIApplication.SharedApplication.StatusBarFrame);

			return new RectangleF (windowFrame.X, windowFrame.Y, windowFrame.Width, statusFrame.Height);
		}

		RectangleF OrientFrame(RectangleF frame){
			if (IsDeviceLandscape(UIDevice.CurrentDevice.Orientation) || IsStatusBarLandscape(UIApplication.SharedApplication.StatusBarOrientation)) {
				frame = new RectangleF (frame.X, frame.Y, frame.Height, frame.Width);
			}

			return frame;
		}

		bool IsDeviceLandscape (UIDeviceOrientation orientation) {
			return orientation == UIDeviceOrientation.LandscapeLeft || orientation == UIDeviceOrientation.LandscapeRight;
		}

		bool IsStatusBarLandscape (UIInterfaceOrientation orientation) {
			return orientation == UIInterfaceOrientation.LandscapeLeft || orientation == UIInterfaceOrientation.LandscapeRight;
		}

		public override void Draw (RectangleF rect)
		{
			var context = UIGraphics.GetCurrentContext ();

			MessageBarStyleSheet styleSheet = StylesheetProvider.StyleSheetForMessageView (this);
			context.SaveState ();

			styleSheet.BackgroundColorForMessageType (MessageType).SetColor ();
			context.FillRect (rect);
			context.RestoreState ();
			context.SaveState ();

			context.BeginPath ();
			context.MoveTo (0, rect.Size.Height);
			context.SetStrokeColorWithColor (styleSheet.StrokeColorForMessageType (MessageType).CGColor);
			context.SetLineWidth (1);
			context.AddLineToPoint (rect.Size.Width, rect.Size.Height);
			context.StrokePath ();
			context.RestoreState ();
			context.SaveState ();

			
			float xOffset = Padding;
			float yOffset = Padding;
			styleSheet.IconImageForMessageType (MessageType).Draw (new RectangleF (xOffset, yOffset, IconSize, IconSize));
			context.SaveState ();
				
			yOffset -= TextOffset;
			xOffset += IconSize + Padding;
			SizeF titleLabelSize = TitleSize ();
			if (string.IsNullOrEmpty (Title) && !string.IsNullOrEmpty (Description)) {
				yOffset = (float)(Math.Ceiling ((double)rect.Size.Height * 0.5) - Math.Ceiling ((double)titleLabelSize.Height * 0.5) - TextOffset);
			}

			TitleColor.SetColor ();
				
			var titleRectangle = new RectangleF (xOffset, yOffset, titleLabelSize.Width, titleLabelSize.Height);
			Title.DrawString (titleRectangle, TitleFont, UILineBreakMode.TailTruncation, UITextAlignment.Left);
			yOffset += titleLabelSize.Height;
				
			SizeF descriptionLabelSize = DescriptionSize ();
			DescriptionColor.SetColor ();
			var descriptionRectangle = new RectangleF (xOffset, yOffset, descriptionLabelSize.Width, descriptionLabelSize.Height);
			Description.DrawString (descriptionRectangle, DescriptionFont, UILineBreakMode.TailTruncation, UITextAlignment.Left);
		}


		SizeF TitleSize ()
		{
			var boundedSize = new SizeF (AvailableWidth, float.MaxValue);
			SizeF titleLabelSize;
			if (!IsRunningiOS7OrLater ()) {

				var attr = new UIStringAttributes (NSDictionary.FromObjectAndKey (TitleFont, (NSString)TitleFont.Name));
				titleLabelSize = Title.GetBoundingRect (
					boundedSize, NSStringDrawingOptions.TruncatesLastVisibleLine, attr, null)
					.Size;

			} else {
				titleLabelSize = Title.StringSize (TitleFont, boundedSize, UILineBreakMode.TailTruncation);
			}

			return titleLabelSize;
		}

		SizeF DescriptionSize ()
		{
			var boundedSize = new SizeF (AvailableWidth, float.MaxValue);
			SizeF descriptionLabelSize;
			if (!IsRunningiOS7OrLater ()) {
				var attr = new UIStringAttributes (NSDictionary.FromObjectAndKey (TitleFont, (NSString)TitleFont.Name));
				descriptionLabelSize = Description.GetBoundingRect (
					boundedSize, NSStringDrawingOptions.TruncatesLastVisibleLine, attr, null)
					.Size;

			} else {
				descriptionLabelSize = Description.StringSize (
					DescriptionFont, boundedSize, UILineBreakMode.TailTruncation);
			}

			return descriptionLabelSize;
		}

		bool IsRunningiOS7OrLater ()
		{
			string systemVersion = UIDevice.CurrentDevice.SystemVersion;

			return systemVersion.Contains ("7");
		}
	}
}
