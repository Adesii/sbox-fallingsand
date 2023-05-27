namespace Sand.Systems.FallingSand.Elements;

public class Steam : Gas
{
	TimeSince GasTimer = 0f;
	public override float MaxVelocity => 1.1f;

	public Steam()
	{
		GasTimer = Game.Random.Float( -1f, 1f );
	}
	public override Color GetColor()
	{
		return Color.FromBytes( 255, 255, 255, 255 ).Darken( 0.5f ).Lighten( Game.Random.Float( 0.8f, 1.2f ) );
	}

	public override void PostStep( Sandworker worker, out bool sleep )
	{
		base.PostStep( worker, out sleep );

		if ( GasTimer > 4f )
		{
			worker.SetCell( Position, new Water() );
			sleep = true;
		}
	}
}

