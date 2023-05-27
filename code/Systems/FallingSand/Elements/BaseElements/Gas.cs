using Sand.util;

namespace Sand.Systems.FallingSand.Elements;


public class Gas : Liquid
{
	public override float MaxVelocity => 3f;
	public override float DisperseRate => 5f;
	public override int Density => 25;

	protected override float Friction => 0.1f;

	public override void Step( Sandworker worker )
	{
		Vector2 gravity = Vector2.Up;// GetGravityAtPosition( worker, this );

		if ( !MoveGasLinear( worker, gravity ) )
		{
			if ( !MoveDirection( worker, Vector2.Left + gravity, Vector2.Right + gravity, 1f, 1f ) )
			{
				if ( !MoveDirection( worker, Vector2.Left, Vector2.Right, DisperseRate, DisperseRate ) )
				{
					Velocity *= Friction;
				}
				Velocity.x = Velocity.x.Clamp( -DisperseRate, DisperseRate );
			}
		}


	}

	public override void PostStep( Sandworker worker, out bool sleep )
	{
		FinalizeMove( worker, this, Velocity );

		sleep = Velocity.Length.AlmostEqual( 0 );
		if ( !sleep )
			SandWorld.Instance.KeepAlive( Position );
	}

	public bool MoveGasLinear( Sandworker worker, Vector2 Dir )
	{
		Vector2 down = Dir; //GetGravityAtPosition( worker, this );
		if ( !SandUtils.CanSwap( worker, this, Position + Dir ) )
		{
			return false;
		}
		Velocity.x = 0;

		Velocity += down * 1.2f;
		Velocity = Velocity.Clamp( -MaxVelocity, MaxVelocity );
		return true;
	}

}

