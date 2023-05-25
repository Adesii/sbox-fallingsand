using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sand.UI;
using Sand.util;
using Sandbox.Debug;

namespace Sand.Systems.FallingSand;

public class SandWorld
{
	private static SandWorld instance;
	public static SandWorld Instance
	{
		get
		{
			instance ??= new SandWorld();
			return instance;
		}
	}

	SandWorld()
	{
		Event.Register( this );
		ChunkWidth = 64;
		ChunkHeight = 64;
	}

	public struct WorldLimit
	{
		public int LeftLimit;
		public int RightLimit;
		public int UpLimit;
		public int DownLimit;

		public WorldLimit( int left, int right, int up, int down )
		{
			LeftLimit = left;
			RightLimit = right;
			UpLimit = up;
			DownLimit = down;
		}

		bool CheckWithLimitless( int limit, int pos, bool positive )
		{
			return limit == 0 || ((pos < limit && positive) || (pos > limit && !positive));
		}

		public bool InBounds( Vector2Int pos )
		{
			return CheckWithLimitless( -LeftLimit, pos.x, false ) && CheckWithLimitless( RightLimit, pos.x, true ) && CheckWithLimitless( UpLimit, pos.y, true ) && CheckWithLimitless( -DownLimit, pos.y, false );
		}
	}

	public static WorldLimit Limit = new( 2, 2, 2, 2 );

	public Texture DrawTexture;
	public Texture CellTexture;


	public static int ChunkWidth;
	public static int ChunkHeight;

	public static Vector2Int WorldPosition = new( 0, 0 );

	private static int Zoom = 10;
	public static int ZoomLevel
	{
		get => Zoom;
		set
		{
			//Zoom the world to the middle of the screen
			var oldZoom = Zoom;
			Zoom = value.Clamp( 1, 100 );
			var delta = Zoom - oldZoom;
			var deltaWorld = delta * (Hud.CorrectMousePosition / oldZoom);
			WorldPosition += new Vector2Int( deltaWorld );
		}
	}
	public ConcurrentDictionary<Vector2Int, SandChunk> chunks = new();

	public Cell GetCell( Vector2Int pos )
	{
		return GetChunk( pos )?.GetCell( pos ) ?? new Cell();
	}

	public void SetCell( Vector2Int pos, ref Cell cell, bool wake = false )
	{
		var cc = GetChunk( pos );
		cc?.SetCell( pos, ref cell, wake );
		if ( wake )
		{
			cc.ShouldWakeup = true;
			cc.KeepAlive( pos );
		}
	}


	public void KeepAlive( Vector2Int pos )
	{
		var chunk = GetChunk( pos );
		if ( chunk == null ) return;
		if ( chunk.sleeping )
		{
			chunk.ShouldWakeup = true;
		}
		chunk.KeepAlive( pos );

	}

	public void MoveCell( Vector2Int From, Vector2Int To, bool Swap = false )
	{
		SandChunk src = GetChunk( From );
		if ( src != null )
		{
			SandChunk dst = GetChunk( To );
			dst?.MoveCell( src, From, To, Swap );

			//Changes.Add( new( src, GetIndex( From ), GetIndex( To ), Swap ) );
		}
	}

	public bool IsEmpty( Vector2Int pos )
	{
		if ( !InBounds( pos ) ) return false;
		return GetChunk( pos )?.IsEmpty( pos ) ?? false;
	}

	public bool InBounds( Vector2Int pos )
	{
		SandChunk chunk = GetChunk( pos );
		if ( chunk != null )
		{
			return chunk.InBounds( pos );
		}
		return false;
	}

	public SandChunk GetChunk( Vector2Int pos )
	{
		var local = GetLocalLocationPrecise( pos );
		return GetChunkFinal( local );
	}

	Vector2Int GetLocalLocationPrecise( Vector2Int pos )
	{
		return new( ((float)pos.x / ChunkWidth).FloorToInt(), ((float)pos.y / ChunkHeight).FloorToInt() );

	}

	public SandChunk GetChunkFinal( Vector2Int pos )
	{
		chunks ??= new();
		if ( chunks.TryGetValue( pos, out var chunk ) )
		{
			return chunk;
		}
		if ( !Limit.InBounds( pos ) )
		{
			return null;
		}

		SandChunk newchunk = new( new( ChunkWidth, ChunkHeight ), new Vector2Int( ChunkWidth, ChunkHeight ) * pos );
		newchunk.ShouldWakeup = true;

		return chunks.TryAdd( pos, newchunk ) ? newchunk : null;
	}

	void RemoveEmptyChunks()
	{
		List<Vector2Int> ToRemoveChunks = new();
		foreach ( var chunk in chunks )
		{
			if ( chunk.Value.cells.IsEmpty )
			{
				ToRemoveChunks.Add( chunk.Key );
			}
		}

		foreach ( var chunk in ToRemoveChunks )
		{
			chunks.TryRemove( chunk, out var _ );
		}
	}





	TimeSince LastUpdate = 0;

	[ConCmd.Client]
	public static void SetCellClient( int x, int y, Type type )
	{
		if ( !Instance.InBounds( new Vector2Int( x, y ) ) )
		{
			Log.Warning( $"Invalid cell {x} {y}  chunklocal = " );
			return;
		}
		Cell cell = null;
		if ( type == typeof( SandElement ) )
		{
			if ( Instance.GetCell( new Vector2Int( x, y ) ) is SandElement )
			{
				return;
			}
			cell = new SandElement();
		}
		if ( type == typeof( WaterElement ) )
		{
			if ( Instance.GetCell( new Vector2Int( x, y ) ) is WaterElement )
			{
				return;
			}
			cell = new WaterElement();
		}

		Instance.SetCell( new Vector2Int( x, y ), ref cell, true );
		//Log.Info( $"Set cell {x} {y} to {type}" );
	}

	public static void BrushBetween( Vector2Int old, Vector2Int final, int size, Type type )
	{
		Vector2Int correctedold = old;
		correctedold.y = ChunkHeight - old.y;
		correctedold.y += ChunkHeight;

		Vector2Int correctedfinal = final;
		correctedfinal.y = ChunkHeight - final.y;
		correctedfinal.y += ChunkHeight;

		correctedold.x -= WorldPosition.x;
		correctedfinal.x -= WorldPosition.x;
		correctedold.y += WorldPosition.y;
		correctedfinal.y += WorldPosition.y;

		Instance.KeepAlive( correctedold );
		Instance.KeepAlive( correctedfinal );



		//correctedfinal *= ZoomLevel;
		//correctedold *= ZoomLevel;

		SandUtils.PointToPointFunction( correctedold, correctedfinal, ( pos ) =>
		{


			for ( int i = -size; i < size; i++ )
			{
				for ( int j = -size; j < size; j++ )
				{
					if ( Instance.InBounds( pos + new Vector2Int( i, j ) ) )
						SetCellClient( pos.x + i, pos.y + j, type );
				}
			}
		} );
	}

	[ConCmd.Client]
	public static void ClearGrid()
	{
		instance.chunks.Clear();
	}

	[GameEvent.Client.Frame]
	public void Update()
	{
		//if ( LastUpdate < 0.1f ) return;
		//SetCellClient( 10, 100, 2 );
		//SetCellBrush( ChunkWidth - 1, ChunkHeight - 1, 10, 1 );
		//SetCellBrush( -100, ChunkHeight - 1, 10, 1 );


		//SetCellClient( 0, ChunkHeight - 1, 1 );

		RealUpdate();




	}

	bool updating = false;
	async void RealUpdate()
	{
		if ( updating ) return;
		//using var _a = Profile.Scope( "Sandworld::Update" );
		updating = true;




		List<Task> tasks = new();

		//Update Cells
		int totalamountofcells = 0;
		foreach ( var chunk in chunks )
		{
			if ( !chunk.Value.cells.IsEmpty && (!chunk.Value.IsCurrentlySleeping || chunk.Value.ShouldWakeup) )
				tasks.Add( GameTask.RunInThreadAsync( () =>
				{
					chunk.Value.ShouldWakeup = false;
					new SimpleSandWorker( this, chunk.Value ).UpdateChunk();

				} ) );

			totalamountofcells += chunk.Value.cells.Count;
		}
		DebugOverlay.ScreenText( $"Active Threads: {tasks.Count}  \n :: {totalamountofcells}", 0, 0.1f );
		await GameTask.WhenAll( tasks.ToArray() );
		tasks.Clear();


		foreach ( var chunk in chunks.Values )
		{
			tasks.Add( GameTask.RunInThreadAsync( () =>
			{
				new SimpleSandWorker( this, chunk ).CommitChanges();
			} ) );
		}
		await GameTask.WhenAll( tasks.ToArray() );
		tasks.Clear();


		foreach ( var chunk in chunks.Values )
		{
			chunk.UpdateRect();
		}





		RemoveEmptyChunks();


		updating = false;
		LastUpdate = 0;
	}

	internal void SetVelocityCell( Vector2Int pos, Vector2Int vel )
	{
		GetChunk( pos ).SetVelocityCell( pos, vel );
	}

	public Vector2 ToLocal( Vector2 newpos )
	{
		return newpos - new Vector2( ChunkWidth, ChunkHeight );
	}

	internal void SetCellVelocity( Vector2Int pos, Vector2Int vel )
	{
		if ( InBounds( pos ) )
			GetChunk( pos ).SetCellVelocity( pos, vel );
	}
}
