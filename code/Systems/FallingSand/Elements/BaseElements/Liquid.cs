namespace Sand.Systems.FallingSand.Elements;

public class Liquid : Cell
{
	public virtual float DisperseRate => 5f;
	public override void Step( Sandworker worker, out bool sleep )
	{
		if ( !MoveDown( worker ) )
		{
			//Velocity *= 0.9f;
			if ( !MoveDirection( worker, Vector2Int.Left + Vector2Int.Down, Vector2Int.Right + Vector2Int.Down, 1f, 1f ) )
			{
				//Velocity *= 0.3f;
				if ( !MoveDirection( worker, Vector2Int.Left, Vector2Int.Right, DisperseRate, DisperseRate ) )
				{
					Velocity *= Friction;
				}
			}
		}

		FinalizeMove( worker, this, Velocity );

		//Velocity *= 0.9f;

		//if ( !sleep )
		//	return;


		sleep = Velocity.Length.AlmostEqual( 0 );
		if ( !sleep )
			SandWorld.Instance.KeepAlive( Position );

	}
}

