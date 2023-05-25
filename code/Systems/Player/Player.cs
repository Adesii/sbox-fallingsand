using Sand.Systems.FallingSand;
using Sand.UI;

namespace Sand;

public partial class Player : AnimatedEntity
{
	public static Type LeftClick = typeof( SandElement );
	public static Type RightClick = typeof( WaterElement );


	[ConCmd.Client]
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
	/// <summary>
	/// When the player is first created. This isn't called when a player respawns.
	/// </summary>
	public override void Spawn()
	{
		Predictable = true;

		// Default properties
		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
		EnableLagCompensation = true;
		EnableHitboxes = true;

		Tags.Add( "player" );
	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();
		delayspawn();
	}
	private async void delayspawn()
	{
		await GameTask.DelayRealtimeSeconds( 0.2f );
		SandWorld.ZoomToFitMap();
	}
	TimeSince LastPaint = 0;
	private Vector2Int OldPos;

	private Vector2Int DragStart;

	bool dragleft = false;
	bool dragright = false;
	bool dragzoom = false;
	public override void BuildInput()
	{
		base.BuildInput();

		if ( Input.Pressed( "LeftClick" ) && !dragleft )
		{
			OldPos = new Vector2Int( Hud.CorrectMousePosition );
			dragleft = true;
		}

		if ( Input.Down( "LeftClick" ) )
		{
			var newpos = new Vector2Int( Hud.CorrectMousePosition );
			SandWorld.BrushBetween( OldPos, newpos, 10, LeftClick );
			OldPos = newpos;
		}
		else if ( dragleft )
		{
			dragleft = false;
		}

		if ( Input.Pressed( "RightClick" ) && !dragright )
		{
			OldPos = new Vector2Int( Hud.CorrectMousePosition );
			dragright = true;
		}

		if ( Input.Down( "RightClick" ) )
		{
			var newpos = new Vector2Int( Hud.CorrectMousePosition );
			SandWorld.BrushBetween( OldPos, newpos, 10, RightClick );
			OldPos = newpos;
		}
		else if ( dragright )
		{
			dragright = false;
		}


		if ( Input.Pressed( "MiddleClick" ) && !dragzoom )
		{
			DragStart = new Vector2Int( Hud.CorrectMousePosition );
			dragzoom = true;
		}
		if ( Input.Down( "MiddleClick" ) )
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
