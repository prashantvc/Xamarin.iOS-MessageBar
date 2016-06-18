using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.iOS;
using Xamarin.UITest.Queries;

namespace MessageBarUITests
{
	[TestFixture]
	public class Tests
	{
		iOSApp app;

		[SetUp]
		public void BeforeEachTest()
		{
			// TODO: If the iOS app being tested is included in the solution then open
			// the Unit Tests window, right click Test Apps, select Add App Project
			// and select the app projects that should be tested.
			//
			// The iOS project should have the Xamarin.TestCloud.Agent NuGet package
			// installed. To start the Test Cloud Agent the following code should be
			// added to the FinishedLaunching method of the AppDelegate:
			//
			//    #if ENABLE_TEST_CLOUD
			//    Xamarin.Calabash.Start();
			//    #endif
			app = ConfigureApp
				.iOS
				// TODO: Update this path to point to your iOS app and uncomment the
				// code if the app is not included in the solution.
				//.AppBundle ("../../../iOS/bin/iPhoneSimulator/Debug/MessageBarUITests.iOS.app")
				.StartApp();
		}

		[Test]
		public void WhenTappedOnTheInfoButtonShowInformationMessage()
		{
			app.Tap("Show Info");
			app.Screenshot("Before messagebar");
			   
			app.WaitForElement(e => e.Id("MessageBar"));
			app.Screenshot("Messagebar dispalyed");
			var title = app.Query(e => e.Id("MessageBar").Invoke("Title")).FirstOrDefault();
			var text = app.Query(e => e.Id("MessageBar").Invoke("Description")).FirstOrDefault();
			var type = app.Query(e => e.Id("MessageBar").Invoke("MessageType")).FirstOrDefault();

			Assert.AreEqual("Info", title);
			Assert.AreEqual("This is information", text);
			Assert.AreEqual(type, 2);
		}

		[Test]
		public void GivenAMessageWhenTappedThenDismissTheMessageBar(){

			app.Tap("Show Info");

			app.WaitForElement(e => e.Id("MessageBar"));
			app.Screenshot("Messagebar dispalyed");
			app.Tap(e => e.Id("MessageBar"));

			app.Screenshot("Messagebar dismissed");

			//var expectedCount = app.Query(e => e.Id("MessageBar")).Count();

			//Assert.AreEqual(0, expectedCount, "Mesagebar still visible");
		}
	}
}

