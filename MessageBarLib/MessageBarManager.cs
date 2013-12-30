//
// MessageBarManager.cs
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



using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Foundation;
using System.Collections.Generic;
using System.Threading;

namespace MessageBar
{
	interface IStyleSheetProvider
	{
		/// <summary>
		/// Stylesheet for message view.
		/// </summary>
		/// <returns>The style sheet for message view.</returns>
		/// <param name="messageView">Message view.</param>
		MessageBarStyleSheet StyleSheetForMessageView (MessageView messageView);
	}

	public class MessageBarManager : NSObject, IStyleSheetProvider
	{
		public static MessageBarManager SharedInstance {
			get{ return instance ?? (instance = new MessageBarManager ()); }
		}

		MessageBarManager ()
		{
			messageBarQueue = new Queue<MessageView> ();
			MessageVisible = false;
			MessageBarOffset = UIApplication.SharedApplication.StatusBarFrame.Size.Height;
			styleSheet = new MessageBarStyleSheet();
		}

		float MessageBarOffset { get; set; }

		bool MessageVisible{ get; set; }

		Queue<MessageView> MessageBarQueue {
			get{ return messageBarQueue; }
		}

		/// <summary>
		/// Gets or sets the style sheet.
		/// </summary>
		/// <value>The style sheet.</value>
		public MessageBarStyleSheet StyleSheet {
			get {
				return styleSheet;
			}
			set {
				if (value != null) {
					styleSheet = value;
				}
			}
		}

		/// <summary>
		/// Shows the message
		/// </summary>
		/// <param name="title">Messagebar title</param>
		/// <param name="description">Messagebar description</param>
		/// <param name="type">Message type</param>
		public void ShowMessage (string title, string description, MessageType type)
		{
			ShowMessage (title, description, type, null);
		}

		/// <summary>
		/// Shows the message
		/// </summary>
		/// <param name="title">Messagebar title</param>
		/// <param name="description">Messagebar description</param>
		/// <param name="type">Message type</param>
		/// <param name = "onDismiss">OnDismiss callback</param>
		public void ShowMessage (string title, string description, MessageType type, Action onDismiss)
		{
			var messageView = new MessageView (title, description, type);
			messageView.StylesheetProvider = this;
			messageView.OnDismiss = onDismiss;
			messageView.Hidden = true;
			UIApplication.SharedApplication.KeyWindow.InsertSubview (messageView, 1);
			MessageBarQueue.Enqueue (messageView);
		
			if (!MessageVisible) {
				ShowNextMessage ();
			}
		}

		void ShowNextMessage ()
		{
			if (MessageBarQueue.Count > 0) {
				MessageVisible = true;
				MessageView messageView = MessageBarQueue.Dequeue ();
				messageView.Frame = new RectangleF (0, -messageView.Height, messageView.Width, messageView.Height);
				messageView.Hidden = false;
				messageView.SetNeedsDisplay ();

				var gest = new UITapGestureRecognizer (MessageTapped);
				messageView.AddGestureRecognizer (gest);
				if (messageView == null)
					return; 

				UIView.Animate (DismissAnimationDuration, 
					() => 
						messageView.Frame = new RectangleF (messageView.Frame.X, 
						MessageBarOffset + messageView.Frame.Y + messageView.Height, 
						messageView.Width, messageView.Height)
				);

				//Need a better way of dissmissing the method
				var dismiss = new Timer (DismissMessage, messageView, TimeSpan.FromSeconds (DisplayDelay),
					              TimeSpan.FromMilliseconds (-1));
			}
		}

		/// <summary>
		/// Hides all messages
		/// </summary>
		public void HideAll ()
		{
			MessageView currentMessageView = null;
			var subviews = UIApplication.SharedApplication.KeyWindow.Subviews;

			foreach (UIView subview in subviews) {
				var view = subview as MessageView;
				if (view != null) {
					currentMessageView = view;
					currentMessageView.RemoveFromSuperview ();
				}
			}

			MessageVisible = false;
			MessageBarQueue.Clear ();
			CancelPreviousPerformRequest (this);
		}

		void MessageTapped (UIGestureRecognizer recognizer)
		{
			var view = recognizer.View as MessageView;
			if (view != null) {
				DismissMessage (view);
			}
		}

		void DismissMessage (object messageView)
		{
			var view = messageView as MessageView;
			if (view != null) {
				InvokeOnMainThread (() =>	DismissMessage (view));
			}
		}

		void DismissMessage (MessageView messageView)
		{
			if (messageView != null && !messageView.Hit) {

				messageView.Hit = true;
				UIView.Animate (DismissAnimationDuration, 
					delegate {
						messageView.Frame = new RectangleF (
							messageView.Frame.X, 
							messageView.Frame.Y - messageView.Height - MessageBarOffset, 
							messageView.Width, messageView.Height);
					}, 
					delegate {
						MessageVisible = false;
						messageView.RemoveFromSuperview ();

						var action = messageView.OnDismiss;
						if (action != null) {
							action ();
						}

						if (MessageBarQueue.Count > 0) {
							ShowNextMessage ();
						}
					}
				);
			}
		}

		const float DisplayDelay = 3.0f;
		const float DismissAnimationDuration = 0.25f;
		MessageBarStyleSheet styleSheet;
		readonly Queue<MessageView> messageBarQueue;
		static MessageBarManager instance;

		#region IStyleSheetProvider implementation

		public MessageBarStyleSheet StyleSheetForMessageView (MessageView messageView)
		{
			return StyleSheet;
		}

		#endregion

	}
}
