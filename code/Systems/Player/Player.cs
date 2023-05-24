using Sand.Systems.FallingSand;
using Sand.UI;

namespace Sand;

public partial class Player : AnimatedEntity
{

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
	TimeSince LastPaint = 0;
	private Vector2Int OldPos;

	private Vector2Int DragStart;

	bool dragleft = false;
	bool dragzoom = false;
	public override void BuildInput()
	{
		base.BuildInput();

		if ( Input.Pressed( "LeftClick" ) && !Input.Down( "jump" ) && !dragleft )
		{
			OldPos = new Vector2Int( Hud.CorrectMousePosition );
			dragleft = true;
		}

		if ( Input.Down( "LeftClick" ) && !Input.Down( "jump" ) )
		{
			var newpos = new Vector2Int( Hud.CorrectMousePosition );
			SandWorld.BrushBetween( OldPos, newpos, 10, typeof( SandElement ) );
			OldPos = newpos;
		}
		else if ( dragleft )
		{
			dragleft = false;
		}

		if ( Input.Pressed( "RightClick" ) )
		{
			OldPos = new Vector2Int( Hud.CorrectMousePosition );
		}

		if ( Input.Down( "RightClick" ) )
		{
			var newpos = new Vector2Int( Hud.CorrectMousePosition );
			SandWorld.BrushBetween( OldPos, newpos, 10, typeof( WaterElement ) );
			OldPos = newpos;
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
