using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using MessageBar;

namespace MessageBarTest
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		UIWindow window;
		UINavigationController navigation;

	    private StringElement changeDisplayPositionElement;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);

		    MessageBarManager.SharedInstance.DisplayDuration = .5f;

			var menu = new RootElement ("Message Test") {
				new Section { 
					new StringElement ("Show Info", () =>
						MessageBarManager.SharedInstance.ShowMessage
						("Info", "This is information", MessageType.Info, 
							delegate {
								Console.WriteLine ("This is callback!");
							})),

					new StringElement ("Show Error", () =>
						MessageBarManager.SharedInstance.ShowMessage
							("Error", "This is error", MessageType.Error)),

					new StringElement ("Show Success", () =>
						MessageBarManager.SharedInstance.ShowMessage
						("Success", "This is success", MessageType.Success))
				},
				new Section {
					new StringElement ("Hide all", MessageBarManager.SharedInstance.HideAll)
				},
                new Section {
				    (changeDisplayPositionElement = new StringElement("Show from bottom", ChangeDisplayPosition))
				},
                new Section {
				    (new StringElement("Increase display duration", () =>
				    {
				        MessageBarManager.SharedInstance.DisplayDuration += .25f;
				        MessageBarManager.SharedInstance.ShowMessage
				            ("Info", string.Format("Duration increased to {0}", MessageBarManager.SharedInstance.DisplayDuration),
				                MessageType.Info);
				    })),
                    (new StringElement("Decrease display duration", () =>
                    {
                        MessageBarManager.SharedInstance.DisplayDuration -= .25f;
                        MessageBarManager.SharedInstance.ShowMessage
                            ("Info", string.Format("Duration decreased to {0}", MessageBarManager.SharedInstance.DisplayDuration),
                                MessageType.Info);
                    }))
				}
			};

			var dvc = new DialogViewController (menu);
			navigation = new UINavigationController ();
			navigation.PushViewController (dvc, false);

			// If you have defined a root view controller, set it here:
			window.RootViewController = navigation;

			// make the window visible
			window.MakeKeyAndVisible ();

			return true;
		}

	    private void ChangeDisplayPosition()
	    {

		    MessageBarManager.SharedInstance.ShowFromBottom = !MessageBarManager.SharedInstance.ShowFromBottom;

		    changeDisplayPositionElement.Caption = 
                MessageBarManager.SharedInstance.ShowFromBottom
                    ? "Show from top"
                    : "Show from bottom";

            changeDisplayPositionElement.GetImmediateRootElement().Reload(changeDisplayPositionElement, UITableViewRowAnimation.Automatic);


            MessageBarManager.SharedInstance.ShowMessage(
                "Info", 
                string.Format("Display from {0}", MessageBarManager.SharedInstance.ShowFromBottom ? "bottom" : "top"), 
                MessageType.Info);
	    }
	}
}

