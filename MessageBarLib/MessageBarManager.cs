// MessageBarMessageType.cs
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
using System.Collections.Generic;
using System.Threading;

using CoreGraphics;

using Foundation;

using UIKit;

namespace MessageBar
{
    /// <summary>
    /// The StyleSheetProvider interface.
    /// </summary>
    internal interface IStyleSheetProvider
    {
        #region Public Methods and Operators

        /// <summary>
        /// Stylesheet for message view.
        /// </summary>
        /// <returns>
        /// The style sheet for message view.
        /// </returns>
        /// <param name="messageView">
        /// Message view.
        /// </param>
        MessageBarStyleSheet StyleSheetForMessageView(MessageView messageView);

        #endregion
    }

    /// <summary>
    /// The message bar manager.
    /// </summary>
    public class MessageBarManager : NSObject, IStyleSheetProvider
    {
        #region Constants

        /// <summary>
        ///     The dismiss animation duration, in Seconds
        /// </summary>
        private const float DismissAnimationDuration = 0.25f;

        #endregion

        #region Static Fields

        /// <summary>
        ///     The instance
        /// </summary>
        private static MessageBarManager instance;

        #endregion

        #region Fields

        /// <summary>
        ///     The message bar queue
        /// </summary>
        private readonly Queue<MessageView> messageBarQueue;

        /// <summary>
        ///     The initial position
        /// </summary>
        private nfloat initialPosition = 0;

        /// <summary>
        ///     The last message
        /// </summary>
        private MessageView lastMessage;

        /// <summary>
        ///     The message window
        /// </summary>
        private MessageWindow messageWindow;

        /// <summary>
        ///     The show position
        /// </summary>
        private nfloat showPosition = 0;

        /// <summary>
        ///     The current style sheet
        /// </summary>
        private MessageBarStyleSheet styleSheet;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Prevents a default instance of the <see cref="MessageBarManager" /> class from being created.
        /// </summary>
        /// <remarks>
        ///     <para>An init message is coupled with an alloc (or allocWithZone:) message in the same line of code:</para>
        ///     <para>
        ///         An object isn’t ready to be used until it has been initialized. The init method defined in the NSObject class
        ///         does no initialization; it simply returns self.
        ///     </para>
        ///     <para>
        ///         In a custom implementation of this method, you must invoke super’s designated initializer then initialize and
        ///         return the new object. If the new object can’t be initialized, the method should return nil. For example, a
        ///         hypothetical BuiltInCamera class might return nil from its init method if run on a device that has no camera.
        ///     </para>
        ///     <para>
        ///         In some cases, an init method might return a substitute object. You must therefore always use the object
        ///         returned by init, and not the one returned by alloc or allocWithZone:, in subsequent code.
        ///     </para>
        /// </remarks>
        private MessageBarManager()
        {
            this.messageBarQueue = new Queue<MessageView>();
            this.MessageVisible = false;
            this.MessageBarOffset = 20;
            this.styleSheet = new MessageBarStyleSheet();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the shared instance.
        /// </summary>
        /// <value>
        ///     The shared instance.
        /// </value>
        public static MessageBarManager SharedInstance
        {
            get
            {
                return instance ?? (instance = new MessageBarManager());
            }
        }

        /// <summary>
        ///     Discard all repeated messages enqueued by a freak finger.
        /// </summary>
        /// <value><c>true</c> if discard repeated; otherwise, <c>false</c>.</value>
        public bool DiscardRepeated { get; set; }

        /// <summary>
        ///     Show all messages at the bottom.
        /// </summary>
        /// <value>
        ///     <c>true</c> if show at the bottom; otherwise, <c>false</c>.
        /// </value>
        public bool ShowAtTheBottom { get; set; }

        /// <summary>
        ///     Gets or sets the style sheet.
        /// </summary>
        /// <value>
        ///     The style sheet.
        /// </value>
        public MessageBarStyleSheet StyleSheet
        {
            get
            {
                return this.styleSheet;
            }

            set
            {
                if (value != null)
                {
                    this.styleSheet = value;
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the message bar offset.
        ///     <para>This is the Y-position which the Message Bar will animate to, upon being displayed.</para>
        /// </summary>
        /// <value>
        ///     The message bar offset.
        /// </value>
        private nfloat MessageBarOffset { get; set; }

        /// <summary>
        ///     Gets the message bar queue.
        /// </summary>
        /// <value>
        ///     The message bar queue.
        /// </value>
        private Queue<MessageView> MessageBarQueue
        {
            get
            {
                return this.messageBarQueue;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether any message is visible.
        /// </summary>
        /// <value>
        ///     <c>true</c> if a message visible; otherwise, <c>false</c>.
        /// </value>
        private bool MessageVisible { get; set; }

        /// <summary>
        ///     Gets the message window view.
        /// </summary>
        /// <value>
        ///     The message window view.
        /// </value>
        private UIView MessageWindowView
        {
            get
            {
                return this.GetMessageBarViewController().View;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Hides all currently-displayed messages, and discards the Message queue.
        /// </summary>
        public void HideAll()
        {
            var subviews = this.MessageWindowView.Subviews;

            foreach (UIView subview in subviews)
            {
                var view = subview as MessageView;
                if (view != null)
                {
                    MessageView currentMessageView = view;
                    currentMessageView.RemoveFromSuperview();
                }
            }

            this.MessageVisible = false;
            this.MessageBarQueue.Clear();
            CancelPreviousPerformRequest(this);
        }

        /// <summary>
        /// Shows the message.
        ///     <para>
        /// Adds a new message to the queue, and shows it when all previous messages has finished showing.
        ///         If no messages exists in the queue, will (obviously) be displayed instantly.
        ///     </para>
        /// </summary>
        /// <param name="title">
        /// Message Bar title
        /// </param>
        /// <param name="description">
        /// Message Bar description
        /// </param>
        /// <param name="type">
        /// Message type
        /// </param>
        /// <param name="duration">
        /// The duration. Default (null) is 3 Seconds.
        /// </param>
        /// <param name="onTapped">
        /// OnTapped callback.
        /// </param>
        /// <param name="onDismiss">
        /// OnDismiss callback
        /// </param>
        public void ShowMessage(
            string title, 
            string description, 
            MessageType type, 
            TimeSpan? duration = null, 
            Action onTapped = null, 
            Action onDismiss = null)
        {
            var messageView = new MessageView(title, description, type)
            {
                StylesheetProvider = this, 
                OnTapped = onTapped, 
                OnDismiss = onDismiss, 
                Hidden = true, 
                Duration = duration ?? TimeSpan.FromSeconds(3)
            };

            this.MessageWindowView.AddSubview(messageView);
            this.MessageWindowView.BringSubviewToFront(messageView);

            this.MessageBarQueue.Enqueue(messageView);

            if (!this.MessageVisible)
            {
                this.ShowNextMessage();
            }
        }

        /// <summary>
        /// Stylesheet for message view.
        /// </summary>
        /// <param name="messageView">
        /// Message view.
        /// </param>
        /// <returns>
        /// The style sheet for message view.
        /// </returns>
        public MessageBarStyleSheet StyleSheetForMessageView(MessageView messageView)
        {
            return this.StyleSheet;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Releases the resources used by the NSObject object.
        /// </summary>
        /// <param name="disposing">
        /// If set to <see langword="true"/>, the method is invoked directly and will dispose manage and
        ///     unmanaged resources;   If set to <see langword="false"/> the method is being called by the garbage collector
        ///     finalizer and should only release unmanaged resources.
        /// </param>
        /// <remarks>
        /// <para>
        /// This Dispose method releases the resources used by the NSObject class.
        /// </para>
        /// <para>
        /// This method is called by both the Dispose() method and the object finalizer (Finalize).    When invoked by
        ///         the Dispose method, the parameter disposing <paramref name="disposing"/> is set to <see langword="true"/> and
        ///         any managed object references that this object holds are also disposed or released;  when invoked by the object
        ///         finalizer, on the finalizer thread the value is set to <see langword="false"/>.
        ///     </para>
        /// <para>
        /// Calling the Dispose method when you are finished using the NSObject ensures that all external resources used
        ///         by this managed object are released as soon as possible.  Once you have invoked the Dispose method, the object
        ///         is no longer useful and you should no longer make any calls to it.
        ///     </para>
        /// <para>
        /// For more information on how to override this method and on the Dispose/IDisposable pattern, read the
        ///         ``Implementing a Dispose Method'' document at http://msdn.microsoft.com/en-us/library/fs2xkftw.aspx
        ///     </para>
        /// </remarks>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            instance = null;
        }

        /// <summary>
        /// Invokes the correct dismiss event, depending on the <see cref="dismissedByTap"/> value.
        /// </summary>
        /// <param name="messageView">
        /// The message view.
        /// </param>
        /// <param name="dismissedByTap">
        /// if set to <c>true</c> [dismissed by tap].
        /// </param>
        private static void InvokeDismissEvent(MessageView messageView, bool dismissedByTap)
        {
            if (!dismissedByTap)
            {
                var onDismiss = messageView.OnDismiss;
                if (onDismiss != null)
                {
                    onDismiss();
                }
            }
            else
            {
                var onTapped = messageView.OnTapped;
                if (onTapped != null)
                {
                    onTapped();
                }
            }
        }

        /// <summary>
        /// Invoked when the message is dismissed by the timer.
        /// </summary>
        /// <param name="messageView">
        /// The message view.
        /// </param>
        private void DismissMessage(object messageView)
        {
            var view = messageView as MessageView;
            if (view != null)
            {
                this.InvokeOnMainThread(() => this.DismissMessage(view, false));
            }
        }

        /// <summary>
        /// Dismisses the message.
        /// </summary>
        /// <param name="messageView">
        /// The message view.
        /// </param>
        /// <param name="dismissedByTap">
        /// if set to <c>true</c> [dismissed by tap].
        /// </param>
        private void DismissMessage(MessageView messageView, bool dismissedByTap)
        {
            if (messageView != null && !messageView.Hit)
            {
                messageView.Hit = true;
                UIView.Animate(
                    DismissAnimationDuration, 
                    () => {
                        messageView.Frame = new CGRect(
                            messageView.Frame.X, 
                            this.initialPosition, 
                            messageView.Frame.Width, 
                            messageView.Frame.Height);
                    }, 
                    () => {
                        this.MessageVisible = false;
                        messageView.RemoveFromSuperview();

                        InvokeDismissEvent(messageView, dismissedByTap);

                        if (this.MessageBarQueue.Count > 0)
                        {
                            this.ShowNextMessage();
                        }
                        else
                        {
                            this.lastMessage = null;
                        }
                    });
            }
        }

        /// <summary>
        /// Gets the message bar view controller.
        /// </summary>
        /// <returns>
        /// The <see cref="MessageBarViewController"/>.
        /// </returns>
        private MessageBarViewController GetMessageBarViewController()
        {
            if (this.messageWindow == null)
            {
                this.messageWindow = new MessageWindow
                {
                    Frame = UIApplication.SharedApplication.KeyWindow.Frame, 
                    Hidden = false, 
                    WindowLevel = UIWindowLevel.Normal, 
                    BackgroundColor = UIColor.Clear, 
                    RootViewController = new MessageBarViewController()
                };
            }

            return (MessageBarViewController)this.messageWindow.RootViewController;
        }

        /// <summary>
        /// Gets the next message.
        /// </summary>
        /// <returns>
        /// The <see cref="MessageView"/>.
        /// </returns>
        private MessageView GetNextMessage()
        {
            MessageView message = null;

            if (!this.DiscardRepeated)
            {
                return this.MessageBarQueue.Dequeue();
            }

            // Removes all except the last message.
            while (this.MessageBarQueue.Count > 0)
            {
                message = this.MessageBarQueue.Dequeue();

                if (!this.IsEqualLastMessage(message))
                {
                    break;
                }

                message = null;
            }

            this.lastMessage = message;

            return message;
        }

        /// <summary>
        /// Determines whether the specified message is equal to the last message handled.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsEqualLastMessage(MessageView message)
        {
            return message.Equals(this.lastMessage);
        }

        /// <summary>
        /// Invoked when the message is Tapped.
        /// </summary>
        /// <param name="recognizer">
        /// The recognizer.
        /// </param>
        private void MessageTapped(UIGestureRecognizer recognizer)
        {
            var view = recognizer.View as MessageView;
            if (view != null)
            {
                this.DismissMessage(view, true);
            }
        }

        /// <summary>
        ///     Attempts to retrieve the next message, and if successful, shows it.
        ///     If no message is found, does nothing.
        /// </summary>
        private void ShowNextMessage()
        {
            MessageView messageView = this.GetNextMessage();

            if (messageView != null)
            {
                this.MessageVisible = true;

                if (this.ShowAtTheBottom)
                {
                    this.initialPosition = this.MessageWindowView.Bounds.Height + messageView.Height;
                    this.showPosition = this.MessageWindowView.Bounds.Height - messageView.Height;
                }
                else
                {
                    this.initialPosition = this.MessageWindowView.Bounds.Y - messageView.Height;
                    this.showPosition = this.MessageWindowView.Bounds.Y + this.MessageBarOffset;
                }

                messageView.Frame = new CGRect(0, this.initialPosition, messageView.Width, messageView.Height);
                messageView.Hidden = false;
                messageView.SetNeedsDisplay();

                var gest = new UITapGestureRecognizer(this.MessageTapped);
                messageView.AddGestureRecognizer(gest);

                UIView.Animate(
                    DismissAnimationDuration, 
                    () => messageView.Frame = new CGRect(
                                                  messageView.Frame.X, 
                                                  this.showPosition, 
                                                  messageView.Width, 
                                                  messageView.Height));

                // Need a better way of dissmissing the method
                var dismiss = new Timer(
                    this.DismissMessage, 
                    messageView, 
                    messageView.Duration, 
                    TimeSpan.FromMilliseconds(-1));
            }
        }

        #endregion
    }
}