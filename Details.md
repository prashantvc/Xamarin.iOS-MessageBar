Message Bar
----

System-wide dropdown messages/notifications for your iOS apps.

###Features

- Three default message types
- Customise the message icons and background colour
- Queue as many messages/notifications
- Supports callbacks when message is dismissed

###Easy and Simple
	//Show message from Shared instance, with the title and description
	MessageBarManager.SharedInstance.ShowMessage ("Success", "This is success", MessageType.Success)
	
	//Provide callback to execute on dismiss
	MessageBarManager.SharedInstance.ShowMessage ("Info", "This is information", MessageType.Info, 
						() => Console.WriteLine ("This is callback!"))
  

  
---
MessageBar is inspired by the Objective-C library TWMessageBarManager created by Terry Worona, which can be found [here](https://github.com/terryworona/TWMessageBarManager).
