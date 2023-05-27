namespace Sand.Systems.FallingSand.Elements;

[Element]
public class FireElement : Gas
{

	public override float DisperseRate => 2f;
	TimeSince timeSinceIgnite = 0f;
	TimeSince lastIgnite = 0f;
	public FireElement()
	{
		timeSinceIgnite = 0f;
		lastIgnite = 10f;
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

	public override void OnHit( Sandworker worker, Cell hitCell )
	{
		if ( lastIgnite > 0.05f )
		{
			if ( hitCell is IFlamable flamable )
			{
				flamable.Ignite( worker, hitCell, this );
			}
			if ( worker.GetCell( Position + Vector2Int.Left ) is IFlamable leftcell )
			{
				leftcell.Ignite( worker, (Cell)leftcell, this );
			}
			if ( worker.GetCell( Position + Vector2Int.Right ) is IFlamable rightcell )
			{
				rightcell.Ignite( worker, (Cell)rightcell, this );
			}
			if ( worker.GetCell( Position + Vector2Int.Down * 2 ) is IFlamable downcell )
			{
				downcell.Ignite( worker, (Cell)downcell, this );
			}
			if ( worker.GetCell( Position + Vector2Int.Up ) is IFlamable upcell )
			{
				upcell.Ignite( worker, (Cell)upcell, this );
			}

			lastIgnite = 0f;
		}
	}


}
