/* namespace Sand.UI;
public partial class Cursor : Panel
{
	private Vector2 ForcePos;
	TimeSince end = 0;
	[GameEvent.Client.BuildInput]
	private void buildinput()
	{
		FakeMousePosition -= RealMouse - Sandbox.Mouse.Position;
		if ( FakeMousePosition.x < 0 || FakeMousePosition.y < 0 || FakeMousePosition.x > Screen.Width || FakeMousePosition.y > Screen.Height )
		{
			if ( HasClass( "inactive" ) ) return;
			SetClass( "inactive", true );
			var x = fractionx.Value.Value * Screen.Width / 100;
			var y = fractiony.Value.Value * Screen.Height / 100;
			Log.Info( $"Cursor went out of bounds! {FakeMousePosition} {x} {y}" );
			Sandbox.Mouse.Position = new Vector2( x, y );
			ForcePos = new Vector2( x, y );
			end = 0;
		}
		FakeMousePosition = FakeMousePosition.Clamp( new Vector2( 0, 0 ), new( Screen.Width, Screen.Height ) );
		RealMouse = Sandbox.Mouse.Position;

		FakeMousePositionCorrected = FakeMousePosition * ScaleFromScreen;

		if ( end < 0.05f )
			Mouse.Position = ForcePos;

	}

	protected override void OnClick( MousePanelEvent e )
	{
		base.OnClick( e );
		FakeMousePosition = Sandbox.Mouse.Position;
		SetClass( "inactive", false );
	}

}
 */
