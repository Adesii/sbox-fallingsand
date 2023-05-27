using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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
		set
		{
			if ( instance != null )
			{
				Event.Unregister( instance );
			}
			instance = value;
		}
	}

	SandWorld()
	{
		Event.Register( this );
		ChunkWidth = 64;
		ChunkHeight = 64;
	}

	[ConCmd.Client]
	public static void CenterAround( int x, int y )
	{
		Instance.CenterAroundPosition( new Vector2Int( x, y ) );
	}

	public void CenterAroundPosition( Vector2Int pos )
	{
		WorldPosition = pos - new Vector2Int( new Vector2( ChunkWidth / 2, (ChunkHeight / 2) - 1 ) - ((Screen.Size / 2) * (Hud.ScaleFromScreenGlobal) * ((float)SandWorld.ZoomLevel / 10f)) );
		WorldPosition += new Vector2Int( 0, SandWorld.ChunkHeight );
		//if world limit is even, we need to offset by half a chunk
		if ( Limit.LeftLimit + Limit.RightLimit == 0 )
		{
			WorldPosition += new Vector2Int( ChunkWidth / 2, 0 );
		}
		//if world limit is even, we need to offset by half a chunk
		if ( Limit.UpLimit + Limit.DownLimit == 0 )
		{
			WorldPosition -= new Vector2Int( 0, ChunkHeight / 2 );
		}
	}

	[ConCmd.Client]
	public static void ZoomToFitMap()
	{

		ZoomLevel = 1;
		var worldRect = Limit.GetRect();
		var worldSize = new Vector2( worldRect.Width, worldRect.Height ) * 8;
		var screenSize = Screen.Size;
		var scale = screenSize / worldSize;
		ZoomLevel = (int)(scale.x / 10);
		CenterAround( 0, 0 );
	}

	[ConCmd.Client]
	public static void CreateNewWorld( int left, int right, int top, int down )
	{
		Limit = new( -Math.Abs( left ), Math.Abs( right ), Math.Abs( top ), -Math.Abs( down ) );
		Instance = new SandWorld();
		ZoomToFitMap();
	}

	public struct WorldLimit
	{
		public int LeftLimit;
		public int RightLimit;
		public int UpLimit;
		public int DownLimit;

		//left can be negative, right can be positive, up can be positive, down can be negative
		public WorldLimit( int left, int right, int up, int down )
		{
			LeftLimit = left;
			RightLimit = right;
			UpLimit = up;
			DownLimit = down;
		}

		public bool InBounds( Vector2Int pos )
		{
			pos.y = 1 - pos.y;
			return (pos.x >= LeftLimit || LeftLimit == 0)
			&& (pos.x < RightLimit || RightLimit == 0)
			&& (pos.y >= DownLimit || DownLimit == 0)
			&& (pos.y < UpLimit || UpLimit == 0);
		}

		public Rect GetRect()
		{
			return new Rect( LeftLimit, DownLimit, RightLimit - LeftLimit, UpLimit - DownLimit );
		}
	}

	public static WorldLimit Limit = new( 0, 2, 2, -2 );

	public Texture DrawTexture;
	public Texture CellTexture;


	public static int ChunkWidth;
	public static int ChunkHeight;

	public static Vector2Int WorldPosition = new( 0, 0 );

	private static int Zoom = 1;
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
		return GetChunk( pos )?.GetCell( pos ) ?? null;
	}

	public void SetCell( Vector2Int pos, ref Cell cell, bool wake = false )
	{
		var cc = GetChunk( pos );
		cc?.SetCell( pos, ref cell, wake );
		if ( wake )
		{
			cc?.KeepAlive( pos );
		}
	}


	public void KeepAlive( Vector2Int pos )
	{
		var chunk = GetChunk( pos );
		chunk?.KeepAlive( pos );
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
		//if ( !InBounds( pos ) ) return false;
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
			//Log.Warning( $"Invalid cell {x} {y}  chunklocal = " );
			return;
		}

		if ( Instance.GetCell( new Vector2Int( x, y ) ).GetType() == type )
		{
			//Instance.KeepAlive( new Vector2Int( x, y ) );
			return;
		}
		var cell = TypeLibrary.Create<Cell>( type );
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
			if ( size == 1 )
			{
				SetCellClient( pos.x, pos.y, type );
				Instance.KeepAlive( pos );
				return;
			}
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

		Stopwatch sw = new();
		sw.Start();




		List<Task> tasks = new();

		//Update Cells
		int totalamountofcells = 0;
		foreach ( var chunk in chunks )
		{
			if ( !chunk.Value.cells.IsEmpty && (!chunk.Value.IsCurrentlySleeping || chunk.Value.ShouldWakeup) )
			{
				//if ( chunk.Value.ShouldWakeup )
				//	chunk.Value.ShouldWakeup = false;
				tasks.Add( GameTask.RunInThreadAsync( () =>
							{
								new SimpleSandWorker( this, chunk.Value ).UpdateChunk();

							} ) );
			}
			totalamountofcells += chunk.Value.cells.Count;
		}
		DebugOverlay.ScreenText( $"Active Threads: {tasks.Count}  \n :: {totalamountofcells}", new Vector2( Screen.Width - 300, 50 ), 0.1f );
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

		sw.Stop();
		DebugOverlay.ScreenText( $"Sandworld::Update {sw.ElapsedMilliseconds}ms", new Vector2( Screen.Width - 300, 30 ), sw.Elapsed.Seconds * 2f );


		updating = false;
		LastUpdate = 0;

	}

	public Vector2 ToLocal( Vector2 newpos )
	{
		return newpos - new Vector2( ChunkWidth, ChunkHeight );
	}
}
