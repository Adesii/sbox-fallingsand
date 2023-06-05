using System.Threading;
using Sand.Systems.FallingSand;

namespace Sand.UI;

public partial class Hud : RootPanel, Sandbox.Menu.IGameMenuPanel
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

	public Hud()
	{
		Log.Info( "Hud created" );
		Current = this;
		Current.TickerRunning = true;
		/* mydelegate += new Action<object>( delegate ( object param )
		{
			Update();
		} ); */
		Ticker();
		SandWorld.ZoomToFitMap();
	}

	bool TickerRunning = false;

	static bool Updated = false;

	//Action<object> mydelegate;
	public async void Ticker()
	{
		Update();
		/* while ( !Updated )
		{
			await GameTask.DelayRealtimeSeconds( RealTime.Delta );
		} */


		await GameTask.DelayRealtimeSeconds( 0.01f );
		if ( TickerRunning && Game.Menu?.Package?.Ident == "sand" )
			Ticker();
	}

	public void Update()
	{
		lock ( this )
		{
			Current.passer();
			Current.idk();
			SandWorld.Instance.Update();
			SandWorld.Instance.DrawChunks();

		}
		//Updated = false;

		//Updated = true;
	}

	public override void DrawBackground( ref RenderState state )
	{
		base.DrawBackground( ref state );
	}




	public static bool GlobalLeftClick;
	public static bool GlobalRightClick;
	public static bool GlobalMiddleClick;



	//[GameEvent.Client.BuildInput]
	protected void passer()
	{
		CorrectMousePosition = (MousePosition * (ScaleFromScreen) * ((float)SandWorld.ZoomLevel / 10f));
		ScaleFromScreenGlobal = ScaleFromScreen;
		if ( !IsHoveringPanel( this ) )
		{
			//Input.SetAction( "LeftClick", leftclick );
			//Input.SetAction( "RightClick", rightclick );
			//Input.SetAction( "MiddleClick", middleclick );
			GlobalLeftClick = leftclick;
			GlobalRightClick = rightclick;
			GlobalMiddleClick = middleclick;
		}
		else
		{
			//Input.SetAction( "LeftClick", false );
			//Input.SetAction( "RightClick", false );
			//Input.SetAction( "MiddleClick", false );
			GlobalLeftClick = false;
			GlobalRightClick = false;
			GlobalMiddleClick = false;
		}
		//Log.Info( $"{Input.Down( "LeftClick" )} {Input.Down( "RightClick" )} {Input.Down( "MiddleClick" )}" );


		//if ( Input.Pressed( "zoom" ) )
		//	Log.Info( "zoom" );
		//Log.Info( middleclick );
	}

	public bool IsHoveringPanel( Panel Source )
	{
		if ( Source != this )
			if ( Source.HasHovered && ((Sandbox.Internal.IPanel)Source).WantsPointerEvents ) return true;
		foreach ( var child in Source.Children )
		{
			if ( IsHoveringPanel( child ) ) return true;
		}
		return false;
	}


	///Player Stuff

	public static Type LeftClick = typeof( Systems.FallingSand.Sand );
	public static Type RightClick = typeof( Systems.FallingSand.Water );

	//[ConCmd.Client]
	public static void SetLeftClick( string name )
	{
		var type = TypeLibrary.GetType( name );
		if ( type == null )
		{
			Log.Error( $"Could not find type {name}" );
			return;
		}
		Log.Info( $"Set left click to {type.TargetType}" );
		LeftClick = type.TargetType;
	}



	bool dragleft = false;
	bool dragright = false;
	bool dragzoom = false;

	TimeSince LastLeftDraw = 0;
	TimeSince LastRightDraw = 0;
	private Vector2Int OldPos;

	private Vector2Int DragStart;

	//[ConVar.Client]
	public static int BrushSize { get; set; } = 5;
	public void idk()
	{
		//Log.Info( "leftclick" );
		if ( leftclick && !dragleft )
		{
			OldPos = new Vector2Int( Hud.CorrectMousePosition );
			dragleft = true;
			LastLeftDraw = 0;
		}

		if ( leftclick )
		{
			if ( LastLeftDraw > Time.Delta * 3 )
			{
				var newpos = new Vector2Int( Hud.CorrectMousePosition );
				SandWorld.BrushBetween( OldPos, newpos, BrushSize, LeftClick );
				OldPos = newpos;
				LastLeftDraw = 0;
			}
		}
		else if ( dragleft )
		{
			dragleft = false;
		}

		if ( rightclick && !dragright )
		{
			OldPos = new Vector2Int( Hud.CorrectMousePosition );
			dragright = true;
			LastRightDraw = 0;
		}

		if ( rightclick )
		{
			if ( LastRightDraw > Time.Delta * 3 )
			{
				var newpos = new Vector2Int( Hud.CorrectMousePosition );
				SandWorld.BrushBetween( OldPos, newpos, BrushSize, RightClick );
				OldPos = newpos;
				LastRightDraw = 0;
			}
		}
		else if ( dragright )
		{
			dragright = false;
		}


		if ( middleclick && !dragzoom )
		{
			DragStart = new Vector2Int( Hud.CorrectMousePosition );
			dragzoom = true;
		}
		if ( middleclick )
		{
			var newpos = new Vector2Int( Hud.CorrectMousePosition );
			var diff = newpos - DragStart;
			SandWorld.WorldPosition += diff;
			DragStart = newpos;
		}
		else if ( dragzoom )
		{
			dragzoom = false;
		}

	}
}

