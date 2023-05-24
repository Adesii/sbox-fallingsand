using System.Diagnostics;
using Sand.util;
using Sandbox.Debug;

namespace Sand.Systems.FallingSand;

public class SimpleSandWorker : Sandworker
{

	public SimpleSandWorker( SandWorld world, SandChunk chunk ) : base( world, chunk )
	{
	}

	public override void UpdateCell( Vector2Int Position, out bool sleep )
	{
		if ( !wchunk.TryGetTarget( out var chunk ) )
		{
			sleep = true;
			return;
		}
		Cell c = chunk.GetCell( Position );

		if ( c == null )
		{
			sleep = true;
			return;
		}
		sleep = true;
		//Sand
		if ( c is SandElement )
		{
			if ( MoveDown( Position, ref c, out sleep ) )
				return;
			if ( MoveDownSides( Position, ref c, out sleep ) )
				return;
		}
		//water
		if ( c is WaterElement )
		{
			if ( MoveDown( Position, ref c, out sleep ) )
				return;
			if ( MoveSides( Position, ref c, out sleep ) )
				return;

		}

	}




	private bool MoveDown( Vector2Int pos, ref Cell c, out bool sleep )
	{
		//using var _b = Profile.Scope( "MoveDown" );
		var odl = c;
		var other = GetCell( pos + Vector2Int.Down );

		bool boyouend = (other?.Density ?? 0) > odl.Density;
		if ( boyouend )
		{
			QuickSwap( pos, pos + Vector2Int.Down, ref c, ref other );
			sleep = true;
			return true;
		}
		if ( odl.GetType() != other?.GetType() )
		{
			var oldvel = odl.Velocity;
			oldvel += Vector2Int.Down * 1;
			sleep = FinalizeMove( pos, oldvel );
			//MoveCell( pos, pos + Vector2Int.Down * 2, false );
			return true;
		}
		sleep = false;
		//cell.Velocity = Vector2.Zero;
		return false;
	}

	private void QuickSwap( Vector2Int pos, Vector2Int pos2, ref Cell a, ref Cell b )
	{
		SetCell( pos, ref b, true );
		SetCell( pos2, ref a, true );
	}

	private bool MoveDownSides( Vector2Int pos, ref Cell c, out bool sleep )
	{
		//using var _c = Profile.Scope( "MoveDownSides" );
		bool downleft = IsEmpty( pos + Vector2Int.Down + Vector2Int.Left );
		bool downright = IsEmpty( pos + Vector2Int.Down + Vector2Int.Right );
		if ( downleft && downright )
		{
			downleft = Game.Random.Float() > 0.5f;
			downright = !downleft;
		}


		var oldvel = new Vector2Int();

		if ( downleft )
		{
			oldvel = Vector2Int.Left + Vector2Int.Down;

			//MoveCell( pos, pos + Vector2Int.Down + Vector2Int.Left, false );
		}
		else if ( downright )
		{
			oldvel = Vector2Int.Right + Vector2Int.Down;
			//MoveCell( pos, pos + Vector2Int.Down + Vector2Int.Right, false );
		}
		//using var _e = Profile.Scope( "MoveDownSides::Finalize" );
		if ( downleft || downright )
			sleep = FinalizeMove( pos, oldvel );
		else
			sleep = true;

		return downleft || downright;
	}

	private bool MoveSides( Vector2Int pos, ref Cell c, out bool sleep )
	{
		//using var _c = Profile.Scope( "MoveDownSides" );
		bool downleft = IsEmpty( pos + Vector2Int.Left );
		bool downright = IsEmpty( pos + Vector2Int.Right );
		if ( downleft && downright )
		{
			downleft = Game.Random.Float() > 0.5f;
			downright = !downleft;
		}

		var oldvel = c.Velocity;

		if ( downleft )
		{
			oldvel = Vector2Int.Left * Game.Random.Float( 1, 5 );
			//MoveCell( pos, pos + Vector2Int.Left, false );
		}
		else if ( downright )
		{
			oldvel = Vector2Int.Right * Game.Random.Float( 1, 5 );
			//MoveCell( pos, pos + Vector2Int.Right, false );
		}
		//using var _e = Profile.Scope( "MoveDownSides::Finalize" );
		if ( downleft || downright )
		{
			FinalizeMove( pos, oldvel );
			sleep = false;
		}
		else
			sleep = true;


		return downleft || downright;
	}




	private bool FinalizeMove( Vector2Int NewPos, Vector2Int NewVel )
	{
		/* if ( NewVel.Length == 1 )
		{
			MoveCell( NewPos, NewPos + NewVel, swap );
			SetCellVelocity( NewPos, NewVel );
			return;
		} */
		var (pos, vel, moved) = CheckPosVelocity( NewPos, NewVel );
		if ( IsEmpty( pos ) )
		{
			MoveCell( NewPos, pos );
			SetCellVelocity( pos, vel );
		}
		//SetCellVelocity( NewPos, vel );
		return !moved;
	}



	//Returns the new position and velocity
	private (Vector2Int pos, Vector2Int vel, bool moved) CheckPosVelocity( Vector2Int newPos, Vector2Int newVel )
	{
		Vector2Int currentPos = newPos;

		SandUtils.PointToPointFunction( newPos, newPos + newVel, ( pos ) =>
		{
			bool empty = IsEmpty( pos );
			if ( (empty || pos == newPos) )
			{
				currentPos = pos;
			}
		} );

		// Iterate until there is an obstacle or collision
		/* while ( IsEmpty( currentPos + currentVel ) )
		{
			currentVel += newVel.Normal;
			if ( currentVel.Length >= newVel.Length )
				break;
		}
		if ( IsEmpty( currentPos + currentVel + newVel.Normal ) )
		{
			return (currentPos + currentVel + newVel.Normal, newVel - currentVel);
		} */



		return (currentPos, currentPos - newPos, newPos != currentPos);
	}


	public void CommitChanges()
	{
		if ( !wchunk.TryGetTarget( out var chunk ) ) return;
		//Remove moves that have their destinations filled
		for ( int i = 0; i < chunk.Changes.Count; i++ )
		{
			var moveinfo = chunk.Changes[i];
			if ( !IsEmpty( moveinfo.To ) )
			{
				chunk.Changes.RemoveAt( i );
				i--;
			}
		}

		//sort by destination
		chunk.Changes.Sort( ( a, b ) => (int)a.To.Distance( b.To ) );

		//pick random source for each destination

		int iprev = 0;

		chunk.Changes.Add( new( new( -1, -1 ), -1, -1 ) );

		//Log.Info( $"Changes: {chunk.Changes.Count}" );


		for ( int i = 0; i < chunk.Changes.Count - 1; i++ )
		{
			if ( chunk.Changes[i + 1].To != chunk.Changes[i].To )
			{
				int rand = iprev + Game.Random.Int( i - iprev );

				Vector2Int dst = chunk.Changes[rand].To;
				Vector2Int src = chunk.Changes[rand].From;
				SandChunk sourcchunk = chunk.Changes[rand].Source;

				if ( !chunk.Changes[rand].Swap )
				{
					//if ( (sourcchunk.IsEmpty( src ) || !IsEmpty( dst )) ) continue;
					//cells[dst] = cells[src];
					//cells[src] = default;
					var idk = sourcchunk.GetCell( src );
					var old = GetCell( dst );
					//CommitedMoveCell( chunk.GetIndex( dst ), sourcchunk.GetIndex( src ), ref idk, true );
					SetCell( dst, ref idk, true );
					sourcchunk.SetCell( src, ref old, true );
				}
				else
				{
					//var temp = cells[dst];
					//cells[dst] = cells[src];
					//cells[src] = temp;

					var idk = sourcchunk.GetCell( src );
					var old = GetCell( dst );
					//swap the two cells
					SetCell( dst, ref idk, true );
					sourcchunk.SetCell( src, ref old, true );
				}
				//DrawPixel( dst, srcell.color );
				iprev = i + 1;
			}
		}

		//Changes.Clear();



		chunk.Draw();
	}
}

