namespace Sand.Systems.FallingSand.Elements;

public class MovableSolid : Cell
{
	protected bool IsFreeFalling = true;
	public virtual float Inertia { get; set; } = 0.3f;
	public override int Density => 100;
	public override void Step( Sandworker worker )
	{
		Vector2 gravity = Vector2.Down; //GetGravityAtPosition( worker, this );
										//Right rotated from gravity
		Vector2 right = gravity.Perpendicular;
		//Left rotated from gravity
		Vector2 left = -right;

		if ( !MoveLinear( worker ) )
		{
			if ( IsFreeFalling )
				if ( !MoveDirection( worker, (Vector2)(Vector2Int.Down + Vector2Int.Right), (Vector2)(Vector2Int.Down + Vector2Int.Left), 1f, 1f ) )
				//if ( !MoveDown( worker ) )
				{
					Velocity *= Friction;
					IsFreeFalling = false;
					if ( worker.GetCell( Position + left ) is MovableSolid leftcell )
					{
						leftcell.FreeFall( worker );
					}
					if ( worker.GetCell( Position + right ) is MovableSolid rightcell )
					{
						rightcell.FreeFall( worker );
					}
				}

		}
		else
		{
			IsFreeFalling = true;

		}

	}

	public override void PostStep( Sandworker worker, out bool sleep )
	{
		if ( !FinalizeMove( worker, this, Velocity, out Vector2Int NewPos ) )
		{
			IsFreeFalling = true;
			if ( worker.GetCell( NewPos + Vector2Int.Left ) is MovableSolid leftcell )
			{
				leftcell.FreeFall( worker );
			}
			if ( worker.GetCell( NewPos + Vector2Int.Right ) is MovableSolid rightcell )
			{
				rightcell.FreeFall( worker );
			}
		}

		sleep = Velocity.Length.AlmostEqual( 0 );
		if ( !sleep )
			SandWorld.Instance.KeepAlive( Position );
	}

	private void FreeFall( Sandworker worker )
	{
		if ( IsFreeFalling )
			return;

		IsFreeFalling = Game.Random.Float() > Inertia;

	}
}

