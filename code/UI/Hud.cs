using Sand.Systems.FallingSand;

namespace Sand.UI;

public partial class Hud : RootPanel
{
	bool leftclick = false;
	bool rightclick = false;
	bool middleclick = false;

	public static Vector2 CorrectMousePosition;
	public static float ScaleFromScreenGlobal = 1f;


	public override void OnMouseWheel( float value )
	{
		SandWorld.ZoomLevel += value > 0 ? 1 : -1;
	}

	/* protected override void OnEvent( PanelEvent e )
	{
		//base.OnEvent( e );
		if ( e is MousePanelEvent mpe )
			Log.Info( $"event {mpe.Name} called for {mpe.Button}" );
	} */
	protected override void OnMouseDown( MousePanelEvent e )
	{
		switch ( e.MouseButton )
		{
			case MouseButtons.Left:
				leftclick = true;
				break;
			case MouseButtons.Right:
				rightclick = true;
				break;
			case MouseButtons.Middle:
				middleclick = true;
				break;
		}

	}
	protected override void OnMouseUp( MousePanelEvent e )
	{
		switch ( e.MouseButton )
		{
			case MouseButtons.Left:
				leftclick = false;
				break;
			case MouseButtons.Right:
				rightclick = false;
				break;
			case MouseButtons.Middle:
				middleclick = false;
				break;
		}
	}
	bool helleft = false;
	bool helright = false;
	[GameEvent.Client.BuildInput]
	private void passer()
	{
		CorrectMousePosition = (MousePosition * (ScaleFromScreen) * ((float)SandWorld.ZoomLevel / 10f));
		ScaleFromScreenGlobal = ScaleFromScreen;
		Input.SetAction( "LeftClick", leftclick );

		Input.SetAction( "RightClick", rightclick );
		Input.SetAction( "MiddleClick", middleclick );

		//if ( Input.Pressed( "zoom" ) )
		//	Log.Info( "zoom" );
		//Log.Info( middleclick );
	}
}

