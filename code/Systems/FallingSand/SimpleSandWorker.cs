using System.Diagnostics;
using Sandbox.Debug;

namespace Sand.Systems.FallingSand;

public class SimpleSandWorker : Sandworker
{

	public SimpleSandWorker( SandWorld world, SandChunk chunk ) : base( world, chunk )
	{
	}

	public override void UpdateCell( Vector2Int Position )
	{
		if ( !wchunk.TryGetTarget( out var chunk ) ) return;
		Cell cell = chunk.GetCell( Position );

		if ( cell.type == 0 ) return;
		//Sand
		if ( cell.type == 1 )
		{
			if ( MoveDown( Position ) )
				return;
			if ( MoveDownSides( Position ) )
				return;
		}
		//water
		if ( cell.type == 2 )
		{
			if ( MoveDown( Position ) )
				return;
			if ( MoveDownSides( Position ) )
				return;
			if ( MoveSides( Position ) )
				return;
		}
	}




	private bool MoveDown( Vector2Int pos )
	{
		//using var _b = Profile.Scope( "MoveDown" );
		var odl = GetCell( pos );
		var other = GetCell( pos + Vector2Int.Down );
		bool boyouend = other.bouyancy >= odl.bouyancy;
		if ( boyouend )
		{
			SetCell( pos, other );
			SetCell( pos + Vector2Int.Down, odl );
			//return true;
		}
		if ( other.type == 0 )
		{
			var oldvel = odl.Velocity;
			oldvel += Vector2Int.Down * 1;
			FinalizeMove( pos, oldvel, boyouend );
			SetCellVelocity( pos, oldvel );
			//MoveCell( pos, pos + Vector2Int.Down * 2, false );
			return true;
		}
		//cell.Velocity = Vector2.Zero;
		return false;
	}

	private bool MoveDownSides( Vector2Int pos )
	{


		//using var _c = Profile.Scope( "MoveDownSides" );
		bool downleft = IsEmpty( pos + (Vector2Int.Down + Vector2Int.Left) );
		bool downright = IsEmpty( pos + (Vector2Int.Down + Vector2Int.Right) );
		if ( downleft && downright )
		{
			//using var _d = Profile.Scope( "MoveDownSides::Random" );
			downleft = Game.Random.Float() > 0.5f;
			downright = !downleft;
		}

		var oldvel = new Vector2Int();

		if ( downleft )
		{
			oldvel = Vector2Int.Down + Vector2Int.Left;
			//MoveCell( pos, pos + Vector2Int.Down + Vector2Int.Left, false );
		}
		else if ( downright )
		{
			oldvel = Vector2Int.Down + Vector2Int.Right;
			//MoveCell( pos, pos + Vector2Int.Down + Vector2Int.Right, false );
		}
		//using var _e = Profile.Scope( "MoveDownSides::Finalize" );
		FinalizeMove( pos, oldvel );

		return downleft || downright;
	}

	private bool MoveSides( Vector2Int pos )
	{
		//using var _c = Profile.Scope( "MoveDownSides" );
		bool downleft = IsEmpty( pos + (Vector2Int.Left) );
		bool downright = IsEmpty( pos + (Vector2Int.Right) );
		if ( downleft && downright )
		{
			//using var _d = Profile.Scope( "MoveDownSides::Random" );
			downleft = Game.Random.Float() > 0.5f;
			downright = !downleft;
		}

		var oldvel = GetCell( pos ).Velocity;

		if ( downleft )
		{
			oldvel += Vector2Int.Left * Game.Random.Int( 1, 10 );
			//MoveCell( pos, pos + Vector2Int.Left, false );
		}
		else if ( downright )
		{
			oldvel += Vector2Int.Right * Game.Random.Int( 1, 10 );
			//MoveCell( pos, pos + Vector2Int.Right, false );
		}
		//using var _e = Profile.Scope( "MoveDownSides::Finalize" );
		FinalizeMove( pos, oldvel );

		return downleft || downright;
	}




	private void FinalizeMove( Vector2Int NewPos, Vector2Int NewVel, bool swap = false )
	{
		if ( NewVel.Length == 1 )
		{
			MoveCell( NewPos, NewPos + NewVel, swap );
			SetCellVelocity( NewPos, !swap ? Vector2Int.Zero : NewVel );
			return;
		}
		var (pos, vel) = CheckPosVelocity( NewPos, NewVel );
		if ( swap || IsEmpty( pos ) )
		{
			MoveCell( NewPos, pos, swap );
			SetCellVelocity( pos, !swap ? vel : NewVel );
		}
		SetCellVelocity( NewPos, !swap ? Vector2Int.Zero : NewVel );
	}



	//Returns the new position and velocity
	private (Vector2Int pos, Vector2Int vel) CheckPosVelocity( Vector2Int newPos, Vector2Int newVel )
	{
		Vector2Int currentPos = newPos;
		Vector2Int currentVel = newVel.Normal;

		// Iterate until there is an obstacle or collision
		while ( IsEmpty( currentPos + currentVel ) )
		{
			currentVel += newVel.Normal;
			if ( currentVel.Length >= newVel.Length )
				break;
		}
		if ( IsEmpty( currentPos + currentVel + newVel.Normal ) )
		{
			return (currentPos + currentVel + newVel.Normal, newVel - currentVel);
		}

		return (currentPos + currentVel - newVel.Normal, Vector2Int.Zero);
	}

	public void CommitChanges()
	{
		if ( !wchunk.TryGetTarget( out var chunk ) ) return;
		//Remove moves that have their destinations filled
		for ( int i = 0; i < chunk.Changes.Count; i++ )
		{
			var moveinfo = chunk.Changes[i];
			if ( (IsEmpty( moveinfo.From ) || !IsEmpty( moveinfo.To )) )
			{
				chunk.Changes.RemoveAt( i );
				i--;
			}
		}

		//sort by destination
		chunk.Changes.Sort( ( a, b ) => a.To.Distance( b.To ).FloorToInt() );

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
					if ( (sourcchunk.IsEmpty( src ) || !IsEmpty( dst )) ) continue;
					//cells[dst] = cells[src];
					//cells[src] = default;
					var idk = sourcchunk.GetCell( src );
					SetCell( dst, idk );
					sourcchunk.SetCell( src, new() );
				}
				else
				{
					//var temp = cells[dst];
					//cells[dst] = cells[src];
					//cells[src] = temp;

					var temp = sourcchunk.GetCell( dst );
					SetCell( dst, sourcchunk.GetCell( src ) );
					sourcchunk.SetCell( src, temp );
				}
				//DrawPixel( dst, srcell.color );
				iprev = i + 1;
			}
		}

		//Changes.Clear();



		chunk.Draw();
	}
}

