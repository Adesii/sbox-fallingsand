using Sand.util;

namespace Sand.Systems.FallingSand;

public partial class Cell
{
	protected bool MoveLinear( Sandworker worker )
	{
		return MoveLinear( worker, Vector2.Down );
	}
	protected bool MoveLinear( Sandworker worker, Vector2 Dir )
	{
		Vector2 down = Dir; //GetGravityAtPosition( worker, this );
		if ( !SandUtils.CanSwap( worker, this, Position + new Vector2Int( down ) ) )
		{
			// Convert Vertical Velocity to Horizontal Velocity on impact
			if ( ShouldBounceToSide )
			{
				float absy = Math.Abs( Velocity.y );
				if ( absy > 1 )
				{
					Velocity += down.Perpendicular * (absy / HorizontalConversion) * Game.Random.Int( -1, 1 );
				}
				else
				{
					Velocity.x *= 0.9f;
				}
				Velocity.y = 0;
			}
			Velocity = Velocity.Clamp( -MaxVelocity, MaxVelocity );
			return false;
		}

		Velocity += down * 2f;
		Velocity = Velocity.Clamp( -MaxVelocity, MaxVelocity );
		return true;
	}

	protected bool MoveDirection( Sandworker worker, Vector2 dir1, Vector2 dir2, float Vel1 = 1, float Vel2 = 1 )
	{
		bool left = SandUtils.CanSwap( worker, this, Position + new Vector2Int( dir1 ), out var leftcell );
		bool right = SandUtils.CanSwap( worker, this, Position + new Vector2Int( dir2 ), out var rightcell );
		if ( left && right )
		{
			left = Game.Random.Float() > 0.5f;
			right = !left;
		}

		if ( left )
		{
			Velocity += dir1 * Vel1;
		}
		if ( right )
		{
			Velocity += dir2 * Vel2;
		}
		Velocity = Velocity.Clamp( -MaxVelocity, MaxVelocity );
		return left || right;
	}
}
