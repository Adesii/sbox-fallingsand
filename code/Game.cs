namespace Sand;


public partial class SandGame : GameManager
{
	public SandGame()
	{
		/* if ( Game.IsClient )
		{
			Game.RootPanel = new UI.Hud();

		} */
	}

	/// <summary>
	/// A client has joined the server. Make them a pawn to play with
	/// </summary>
	public override void ClientJoined( IClient client )
	{
		base.ClientJoined( client );

		// Create a pawn for this client to play with
		var pawn = new Player();
		client.Pawn = pawn;

		//Chat.AddChatEntry( To.Everyone, client.Name, "joined the game", client.SteamId, true );
	}

	public override void ClientDisconnect( IClient client, NetworkDisconnectionReason reason )
	{
		base.ClientDisconnect( client, reason );
		//Chat.AddChatEntry( To.Everyone, client.Name, "left the game", client.SteamId, true );
	}

	public override void DoPlayerDevCam( IClient client )
	{
		// do nothing
	}
}
