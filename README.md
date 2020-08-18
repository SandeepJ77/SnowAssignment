Solutions Contains 5 projects:
1. ChatClient: Initiator of chat application
2. ChatSubscriber: Receive messages from Chat client and reply back to chat client
3. Snow.API: Get and post message APIs to inetract with ChatClient & ChatSubscriber console
4. Snow.Utility: Common functionality and Handler communication calls between Console and API
5. Test: Unit test cases

How to run Chat App:

1. Run NATS server on local machine
2. Open "SnowAssignment.sln" in Visual Studio
3. Set "Snow.API" as startup web API project and run into IIS Express mode, Will open Swagger documentation
4. Run ChatSubscriber console, it will ask for username, please provide a username.
5. Run ChatClient console, it will ask for username, please provide a username
6. Enter a message in ChatClient window and press enter to send message. Message will appear in ChatSubscriber window.
7. Enter a reply message in ChatSubscriber and press enter to send message. Message will appear in ChatClient window.
8. Repeat step 4 and 5 multiple time to have some chats.
9. Go to http://localhost:52172/index.html, Invoke "GET" call by passing ChatSubscriber console username as a sender and method will return all the Chat messages received by ChatClient in the response as list of messages
10. Go to http://localhost:52172/index.html, Invoke "POST" call it will send message to receiver and will appear in the console window. 
	Below is the JSON payload
	{
	  "subject": "snowagent",
	  "sender": "<Provide sender name>",
	  "content": "<Provide message content>",
	  "isRead": false
	}