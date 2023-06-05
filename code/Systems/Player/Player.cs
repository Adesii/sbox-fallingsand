using Sand.Systems.FallingSand;
using Sand.UI;

namespace Sand;

public partial class Player : AnimatedEntity
{
	/* public static Type LeftClick = typeof( Systems.FallingSand.Sand );
	public static Type RightClick = typeof( Systems.FallingSand.Water );

	[ConVar.Client]
	public static int BrushSize { get; set; } = 5;


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
	} */
	/// <summary>
	/// When the player is first created. This isn't called when a player respawns.
	/// </summary>
	/* public override void Spawn()
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
 */
	/* public override void ClientSpawn()
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

	TimeSince LastLeftDraw = 0;
	TimeSince LastRightDraw = 0;
	public override void BuildInput()
	{
		base.BuildInput();

		if ( Input.Pressed( "LeftClick" ) && !dragleft )
		{
			OldPos = new Vector2Int( Hud.CorrectMousePosition );
			dragleft = true;
			LastLeftDraw = 0;
		}

		if ( Input.Down( "LeftClick" ) )
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

		if ( Input.Pressed( "RightClick" ) && !dragright )
		{
			OldPos = new Vector2Int( Hud.CorrectMousePosition );
			dragright = true;
			LastRightDraw = 0;
		}

		if ( Input.Down( "RightClick" ) )
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

	}*/

}
