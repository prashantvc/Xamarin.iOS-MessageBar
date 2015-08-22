// DefaultMessageBarStyleSheet.cs
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

using UIKit;

namespace MessageBar
{
    /// <summary>
    ///     The Message Bar Style Sheet.
    ///     This is responsible for providing the correct UI design, based on the Message Type being provided.
    /// </summary>
    public class MessageBarStyleSheet
    {
        #region Constants

        /// <summary>
        /// The alpha.
        /// </summary>
        private const float Alpha = 0.96f;

        /// <summary>
        /// The error icon.
        /// </summary>
        private const string ErrorIcon = "icon-error.png";

        /// <summary>
        /// The info icon.
        /// </summary>
        private const string InfoIcon = "icon-info.png";

        /// <summary>
        /// The success icon.
        /// </summary>
        private const string SuccessIcon = "icon-success.png";

        #endregion

        #region Fields

        /// <summary>
        /// The error background color.
        /// </summary>
        private readonly UIColor errorBackgroundColor;

        /// <summary>
        /// The error stroke color.
        /// </summary>
        private readonly UIColor errorStrokeColor;

        /// <summary>
        /// The info background color.
        /// </summary>
        private readonly UIColor infoBackgroundColor;

        /// <summary>
        /// The info stroke color.
        /// </summary>
        private readonly UIColor infoStrokeColor;

        /// <summary>
        /// The success background color.
        /// </summary>
        private readonly UIColor successBackgroundColor;

        /// <summary>
        /// The success stroke color.
        /// </summary>
        private readonly UIColor successStrokeColor;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MessageBar.MessageBarStyleSheet" /> class.
        /// </summary>
        public MessageBarStyleSheet()
        {
            this.errorBackgroundColor = UIColor.FromRGBA(1.0f, 0.611f, 0.0f, Alpha);
            this.successBackgroundColor = UIColor.FromRGBA(0.0f, 0.831f, 0.176f, Alpha);
            this.infoBackgroundColor = UIColor.FromRGBA(0.0f, 0.482f, 1.0f, Alpha);
            this.errorStrokeColor = UIColor.FromRGBA(0.949f, 0.580f, 0.0f, 1.0f);
            this.successStrokeColor = UIColor.FromRGBA(0.0f, 0.772f, 0.164f, 1.0f);
            this.infoStrokeColor = UIColor.FromRGBA(0.0f, 0.415f, 0.803f, 1.0f);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Provides the background color for message type
        /// </summary>
        /// <param name="type">
        /// Message type
        /// </param>
        /// <returns>
        /// The background color for message type.
        /// </returns>
        public virtual UIColor BackgroundColorForMessageType(MessageType type)
        {
            UIColor backgroundColor = null;
            switch (type)
            {
                case MessageType.Error:
                    backgroundColor = this.errorBackgroundColor;
                    break;
                case MessageType.Success:
                    backgroundColor = this.successBackgroundColor;
                    break;
                case MessageType.Info:
                    backgroundColor = this.infoBackgroundColor;
                    break;
            }

            return backgroundColor;
        }

        /// <summary>
        /// Provides the icon for message type
        /// </summary>
        /// <param name="type">
        /// Message type
        /// </param>
        /// <returns>
        /// The icon for message type.
        /// </returns>
        public virtual UIImage IconImageForMessageType(MessageType type)
        {
            UIImage iconImage = null;
            switch (type)
            {
                case MessageType.Error:
                    iconImage = UIImage.FromBundle(ErrorIcon);
                    break;
                case MessageType.Success:
                    iconImage = UIImage.FromBundle(SuccessIcon);
                    break;
                case MessageType.Info:
                    iconImage = UIImage.FromBundle(InfoIcon);
                    break;
            }

            return iconImage;
        }

        /// <summary>
        /// Provides the stroke color for message type
        /// </summary>
        /// <param name="type">
        /// Message type
        /// </param>
        /// <returns>
        /// The stroke color for message type.
        /// </returns>
        public virtual UIColor StrokeColorForMessageType(MessageType type)
        {
            UIColor strokeColor = null;
            switch (type)
            {
                case MessageType.Error:
                    strokeColor = this.errorStrokeColor;
                    break;
                case MessageType.Success:
                    strokeColor = this.successStrokeColor;
                    break;
                case MessageType.Info:
                    strokeColor = this.infoStrokeColor;
                    break;
            }

            return strokeColor;
        }

        #endregion
    }
}