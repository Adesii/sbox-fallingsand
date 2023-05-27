using Sandbox.Utility;

namespace Sand.Systems.FallingSand.Elements;

public class SmokeElement : Gas
{

	public override float DisperseRate => 5f;
	public override float MaxVelocity => 10f;
	TimeSince timeSinceCreated;
	TimeSince timeSinceLastColorChange;

	public override Color GetColor()
	{
		//change color ever 0.1 seconds
		if ( timeSinceLastColorChange > 0.1f )
		{
			CellColor = Color.FromBytes( 13, 10, 10, 255 )
					.Lighten( Noise.Fbm( 4, Position.x + Time.Now * 10, Position.y + Time.Now * 10 ) )
					.Darken( Game.Random.Float( 0.05f, 0.1f ) )
					.Lighten( (timeSinceCreated.Relative * 0.5f).Clamp( 0, 1f ) );

			timeSinceLastColorChange = 0;
		}
		return CellColor;
	}
	public SmokeElement()
	{
		timeSinceCreated = 0;
		CellColor = Color.FromBytes( 15, 15, 15, 255 ).Lighten( Game.Random.Float( 0.8f, 1.2f ) );
		Heat = 1500;
	}

	public override void Step( Sandworker worker )
	{
		base.Step( worker );
		if ( timeSinceCreated > 0.5f )
		{
			worker.SetCell( Position, new EmptyCell() );
			return;
		}
		else
		{
			worker.KeepAlive( Position );
			return;
		}
	}
}

