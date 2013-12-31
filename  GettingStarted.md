##Getting Started

By default the MessageBar is accessble through `MessageBarManager.SharedInstance` from anywhere with your app. You can queue as many message as you want to display sequentially by calling `ShowMessage` method or it's overload.


###Show messages

You can show messages by calling `ShowMessage` method.

	//Show message from Shared instance, with the title and description
	MessageBarManager.SharedInstance.ShowMessage ("Success", "This is success", MessageType.Success))
	
Show messages with callback:

	//Provide callback to execute on dismiss
	MessageBarManager.SharedInstance.ShowMessage ("Info", "This is information", MessageType.Info, 
						() => Console.WriteLine ("This is callback!")))
						

###Customisation

You can customise messages by extending `MessageBarStyleSheet` class and assigning it to 
`MessageBarManager.StyleSheet`

You need to override one/more following methods to provide custom icons and colours for messages

	//Customise background colour for message type
	public virtual UIColor BackgroundColorForMessageType (MessageType type)
	
	//Customise icon for message type
	public virtual UIImage IconImageForMessageType (MessageType type)
	
	//Stroke colour for message type
	public virtual UIColor StrokeColorForMessageType (MessageType type)
	
