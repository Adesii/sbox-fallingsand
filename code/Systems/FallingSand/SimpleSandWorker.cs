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

		if ( c == null || c is EmptyCell )
		{
			sleep = true;
			return;
		}
		c.Step( this, out sleep );
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

		//Log.Info( $"Changes: {chunk.Changes.Count}" );


		for ( int i = 0; i < chunk.Changes.Count; i++ )
		{
			if ( i == chunk.Changes.Count - 1 || chunk.Changes[i + 1].To != chunk.Changes[i].To )
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

