using Sand.util;

namespace Sand.Systems.FallingSand.Elements;

[Element]
public class FireElement : Gas
{

	public override float DisperseRate => 2f;
	TimeSince timeSinceIgnite = 0f;
	TimeSince lastIgnite = 0f;

	public override float HeatTransferRate => 10;
	public FireElement()
	{
		timeSinceIgnite = 0f;
		lastIgnite = 10f;
		Heat = 1900;
	}

	public override Color GetColor()
	{
		CellColor = Color.FromBytes( 251, 183, 65, 255 )
				.Lighten( Game.Random.Float( 0.5f, 1.5f ) ).Darken( (timeSinceIgnite * 2).Clamp( 0, 0.9f ) );
		return CellColor;
	}

	public override void PostStep( Sandworker worker, out bool sleep )
	{
		FinalizeMove( worker, this, Velocity, out Vector2Int NewPos );

		if ( timeSinceIgnite > 1f )
		{
			var smoke = new SmokeElement
			{
				Velocity = Velocity + Vector2.Up * 10f
			};
			worker.SetCell( Position, smoke, true );
		}
		else
		{
			worker.KeepAlive( Position );
		}
		sleep = false;
	}

	public override void HeatElement( Sandworker worker, float heat )
	{
	}


	public override void HeatStep( Sandworker worker, out bool heattransfered )
	{

		heattransfered = true;
		PropagateHeat( worker, 3, Heat );
	}

	public override void OnHit( Sandworker worker, Cell hitCell )
	{
		base.OnHit( worker, hitCell );
		hitCell.HeatElement( worker, Heat );
	}


}
