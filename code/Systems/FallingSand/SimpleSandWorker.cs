using System.Diagnostics;
using Sandbox.Debug;

namespace Sand.Systems.FallingSand;

public class SimpleSandWorker : Sandworker
{

	public SimpleSandWorker( SandWorld world, SandChunk chunk ) : base( world, chunk )
	{
	}

	public override void UpdateCell( Vector2Int Position, ref bool sleep )
	{
		if ( !wchunk.TryGetTarget( out var chunk ) ) return;
		Cell c = chunk.GetCell( Position );

		if ( c.type == 0 ) return;
		//Sand
		if ( c.type == 1 )
		{
			if ( MoveDown( Position, ref c, ref sleep ) )
				return;
			if ( MoveDownSides( Position, ref c, ref sleep ) )
				return;
		}
		//water
		if ( c.type == 2 )
		{
			if ( MoveDown( Position, ref c, ref sleep ) )
				return;
			if ( MoveDownSides( Position, ref c, ref sleep ) )
				return;
			if ( MoveSides( Position, ref c, ref sleep ) )
				return;
		}

		sleep = true;
	}




	private bool MoveDown( Vector2Int pos, ref Cell c, ref bool sleep )
	{
		//using var _b = Profile.Scope( "MoveDown" );
		var odl = c;
		var other = GetCell( pos + Vector2Int.Down );
		bool boyouend = other.Density > odl.Density;
		if ( boyouend )
		{
			QuickSwap( pos, pos + Vector2Int.Down, ref c, ref other );
			//return true;
		}
		if ( other.type == 0 && odl.type != other.type )
		{
			var oldvel = odl.Velocity;
			oldvel += Vector2Int.Down * 1;
			FinalizeMove( pos, oldvel, boyouend );
			SetCellVelocity( pos, oldvel );
			//Log.Info( $"MoveDown: {oldvel}" );

			sleep = true;
			//MoveCell( pos, pos + Vector2Int.Down * 2, false );
			return true;
		}
		sleep = false;
		//cell.Velocity = Vector2.Zero;
		return false;
	}

	private void QuickSwap( Vector2Int pos, Vector2Int pos2, ref Cell a, ref Cell b )
	{
		SetCell( pos, b, true );
		SetCell( pos2, a, true );
	}

	private bool MoveDownSides( Vector2Int pos, ref Cell c, ref bool sleep )
	{
		//using var _c = Profile.Scope( "MoveDownSides" );
		bool downleft = IsEmpty( pos + Vector2Int.Down + Vector2Int.Left );
		bool downright = false;
		if ( downleft )
		{
			downleft = Game.Random.Float() > 0.5f;
			if ( downleft )
			{
				downright = IsEmpty( pos + Vector2Int.Down + Vector2Int.Right );
			}
		}
		else
		{
			downright = IsEmpty( pos + Vector2Int.Down + Vector2Int.Right );
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
		if ( downleft || downright )
			FinalizeMove( pos, oldvel );

		sleep = (downleft || downright);

		return downleft || downright;
	}

	private bool MoveSides( Vector2Int pos, ref Cell c, ref bool sleep )
	{
		//using var _c = Profile.Scope( "MoveDownSides" );
		bool downleft = IsEmpty( pos + Vector2Int.Left );
		bool downright = false;
		if ( downleft )
		{
			downleft = Game.Random.Float() > 0.5f;
			if ( downleft )
			{
				downright = IsEmpty( pos + Vector2Int.Right );
			}
		}
		else
		{
			downright = IsEmpty( pos + Vector2Int.Right );
		}

		var oldvel = c.Velocity;

		if ( downleft )
		{
			oldvel += Vector2Int.Left * Game.Random.Int( 1, 3 );
			//MoveCell( pos, pos + Vector2Int.Left, false );
		}
		else if ( downright )
		{
			oldvel += Vector2Int.Right * Game.Random.Int( 1, 3 );
			//MoveCell( pos, pos + Vector2Int.Right, false );
		}
		//using var _e = Profile.Scope( "MoveDownSides::Finalize" );
		if ( downleft || downright )
			FinalizeMove( pos, oldvel );

		sleep = (downleft || downright);

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
		SetCellVelocity( NewPos, Vector2Int.Zero );
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
					if ( (sourcchunk.IsEmpty( src ) || !IsEmpty( dst )) ) continue;
					//cells[dst] = cells[src];
					//cells[src] = default;
					var idk = sourcchunk.GetCell( src );
					SetCell( dst, idk, true );
					sourcchunk.SetCell( src, new(), true );
				}
				else
				{
					//var temp = cells[dst];
					//cells[dst] = cells[src];
					//cells[src] = temp;

					var temp = sourcchunk.GetCell( dst );
					SetCell( dst, sourcchunk.GetCell( src ), true );
					sourcchunk.SetCell( src, temp, true );
				}

				if ( sourcchunk != chunk )
					sourcchunk.ShouldWakeup = true;
				//Log.Info( $"dst {dst - chunk.Position}, src {src - chunk.Position}" );
				Vector2Int localdst = dst - chunk.Position;
				Vector2Int localsrc = src - chunk.Position;
				Vector2Int Dir = (dst - src).Normal;
				if ( /* localdst.y <= chunk.Size.y / 4 || localsrc.y <= chunk.Size.y / 4 || */ localdst.y >= chunk.Size.y / 4 || localsrc.y >= chunk.Size.y / 4 )
				{
					//wake up upper chunk
					if ( wworld.TryGetTarget( out var world ) )
					{
						var c = world.GetChunk( new Vector2Int( chunk.Position.x, chunk.Position.y ) - (Dir * chunk.Size) );
						if ( c != null )
						{
							c.sleeping = true;
							c.ShouldWakeup = true;

						}
					}

				}
				//DrawPixel( dst, srcell.color );
				iprev = i + 1;
			}
		}

		//Changes.Clear();



		chunk.Draw();
	}
}

